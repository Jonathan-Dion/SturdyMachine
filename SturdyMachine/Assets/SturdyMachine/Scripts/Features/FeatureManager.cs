using System;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;

using SturdyMachine.Component;
using SturdyMachine.Offense;
using SturdyMachine.Inputs;
using SturdyMachine.Offense.Blocking;
using SturdyMachine.Features.Fight;
using SturdyMachine.Features.Focus;
using SturdyMachine.Features.Fight.Sequence;
using System.Runtime.InteropServices;
using SturdyMachine.Features.HitConfirm;



#if UNITY_EDITOR
using UnityEditor;
using NWH.NUI;
using NWH.VehiclePhysics2;
#endif

namespace SturdyMachine.Features 
{
    [Serializable]
    public partial class FeatureManager : SturdyModuleComponent
    {
        #region Attribut

        /// <summary>
        /// Array that has all the feature modules that the bot has
        /// </summary>
        [SerializeField]
        List<FeatureModule> _featureModule;

        GameObject[] _enemyBotObject;

        GameObject _sturdyBotObject;

        #endregion

        #region Get

        /// <summary>
        /// Return all the feature modules that bot has
        /// </summary>
        public List<FeatureModule> GetFeatureModules => _featureModule;

        public FeatureModule GetSpecificFeatureModule(FeatureModuleCategory pFeatureModuleCategory) {

            for (int i = 0; i < _featureModule.Count; ++i) {

                if (_featureModule[i].GetFeatureModuleCategory() != pFeatureModuleCategory)
                    continue;

                return _featureModule[i];
            }

            return null;
        }

        public FocusModule GetFocusModule => GetSpecificFeatureModule(FeatureModuleCategory.Focus) as FocusModule;
        
        public HitConfirmModule GetHitConfirmModule => GetSpecificFeatureModule(FeatureModuleCategory.HitConfirm) as HitConfirmModule;

        public GameObject GetSturdyBotObject => _sturdyBotObject;

        /// <summary>
        /// Return specific feature module
        /// </summary>
        /// <typeparam name="FM">FeatureManager</typeparam>
        /// <returns></returns>
        public FM GetFeatureModule<FM>() where FM : FeatureModule{

            return _featureModule.FirstOrDefault(module => module.GetType() == typeof(FM)) as FM;
        }

        FeatureModule[] GetFeatureModuleOrdered 
        {
            get 
            {
                FeatureModule[] featureModules = new FeatureModule[_featureModule.Count];

                FeatureModuleCategory[] featureModuleCategory = (FeatureModuleCategory[])Enum.GetValues(typeof(FeatureModuleCategory));

                for (int i = 0; i < featureModuleCategory.Length; ++i)
                {
                    for (int j = 0; j < _featureModule.Count; ++j)
                    {
                        if (_featureModule[j].GetFeatureModuleCategory() != featureModuleCategory[i])
                            continue;

                        featureModules[i] = _featureModule[j];

                        break;
                    }
                }

                return featureModules;
            }
        }

        public GameObject[] GetEnemyBotObject => _enemyBotObject;

        #endregion

        #region Method

        public virtual void Initialize(GameObject[] pEnemyBotObject, GameObject pSturdyBotObject, FightOffenseSequenceManager pFightOffenseSequenceManager, BotType[] pEnemyBotType) 
        {
            base.Initialize();

            _enemyBotObject = pEnemyBotObject;
            _sturdyBotObject = pSturdyBotObject;

            _featureModule = GetFeatureModuleOrdered.ToList();

            for (byte i = 0; i < _featureModule.Count; ++i)
                _featureModule[i].Initialize(this, pFightOffenseSequenceManager, pEnemyBotType);
        }

        public override void OnAwake(SturdyComponent pSturdyComponent) {

            base.OnAwake(pSturdyComponent);

            ReloadFeatureModule();

            for (byte i = 0; i < _featureModule.Count; ++i)
                _featureModule[i].OnAwake(pSturdyComponent);
        }

        public virtual bool OnUpdate(bool pIsLeftFocus, bool pIsRightFocus, Vector3 pFocusRange)
        {
            if (!base.OnUpdate())
                return false;

            for (int i = 0; i < _featureModule.Count; ++i) {

                if (!_featureModule[i].OnUpdate(pIsLeftFocus, pIsRightFocus, pFocusRange))
                    break;
            }

            return true;
        }

        public override void OnEnabled()
        {
            base.OnEnabled();

            for (int i = 0; i < _featureModule.Count; ++i)
                _featureModule[i].OnEnabled();
        }

        public override void OnDisabled()
        {
            base.OnDisabled();

            for (int i = 0; i < _featureModule.Count; ++i)
                _featureModule[i].OnDisabled();
        }

        /// <summary>
        /// Call when you need refresh all feature module on FeatureManager
        /// </summary>
        public void ReloadFeatureModule(SturdyComponent pSturdyComponent = null)
        {
            if (_featureModule == null)
                _featureModule = new List<FeatureModule>();
            else
                _featureModule.Clear();

            if (pSturdyComponent)
                _sturdyComponent = pSturdyComponent;

            List<FeatureModuleWrapper> featureModuleWrapper = _sturdyComponent.GetComponents<FeatureModuleWrapper>()?.ToList();

            if (featureModuleWrapper.Count == 0 || featureModuleWrapper == null)
                return;

            for (int i = 0; i < featureModuleWrapper.Count; ++i)
                _featureModule.Add(featureModuleWrapper[i].GetFeatureModule());
        }

        #endregion
    }

#if UNITY_EDITOR

    [CustomPropertyDrawer(typeof(FeatureManager))]
    public partial class FeatureManagerDrawer : ComponentNUIPropertyDrawer 
    {
        bool _reloadFeatureModuleFlag;

        public FeatureManagerDrawer() 
        {
            _reloadFeatureModuleFlag = true;
        }

        public override bool OnNUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (!base.OnNUI(position, property, label))
                return false;

            SturdyComponent sturdyComponent = SerializedPropertyHelper.GetTargetObjectWithProperty(property) as SturdyComponent;

            FeatureManager featureManager = SerializedPropertyHelper.GetTargetObjectOfProperty(property) as FeatureManager;

            if (featureManager == null)
                drawer.EndProperty();

            if (_reloadFeatureModuleFlag)
            {
                _reloadFeatureModuleFlag = false;

                featureManager.ReloadFeatureModule(sturdyComponent);
            }

            drawer.Space();

            FeatureModuleWrapper[] moduleWrappers = sturdyComponent.GetComponents<FeatureModuleWrapper>();

            if (moduleWrappers.Length == 0)
            {
                drawer.Info("Use 'Add Component' button to add a module. Modules will appear here as they are added.");
                drawer.EndProperty();

                return true;
            }   

            drawer.Label("Module Categories:");

            FeatureModuleCategory[] moduleCategories =
                featureManager.GetFeatureModules.Select(m => m.GetFeatureModuleCategory()).Distinct().OrderBy(x => x).ToArray();

            int categoryIndex =
                drawer.HorizontalToolbar("moduleCategories", moduleCategories.Select(m => m.ToString()).ToArray());

            if (categoryIndex < 0)
                categoryIndex = 0;

            if (categoryIndex > moduleCategories.Length) 
            {
                drawer.EndProperty();
                return true;
            }

            drawer.Space(3);

            FeatureModuleCategory category = moduleCategories[categoryIndex];

            for (int i = 0; i < moduleWrappers.Length; ++i)
            {
                if (moduleWrappers[i] == null || moduleWrappers[i].GetFeatureModule() == null)
                    continue;

                if (moduleWrappers[i].GetFeatureModule().GetFeatureModuleCategory() != category)
                    continue;

                drawer.EmbeddedObjectEditor<NVP_NUIEditor>(moduleWrappers[i], drawer.positionRect);
            }

            drawer.EndProperty();
            return true;
        }
    }

#endif
}