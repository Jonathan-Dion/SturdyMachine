using System;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;

using SturdyMachine.Component;

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

        #endregion

        #region Get

        /// <summary>
        /// Return all the feature modules that bot has
        /// </summary>
        public List<FeatureModule> GetFeatureModules => _featureModule;

        /// <summary>
        /// Allow to return specific feature module
        /// </summary>
        /// <param name="pFeatureModuleCategory">The categoryFeature of the module you need to fetch</param>
        /// <returns>Returns the feature module matching the Feature category sent as a parameter</returns>
        public FeatureModule GetSpecificFeatureModule(FeatureModule.FeatureModuleCategory pFeatureModuleCategory) 
        {
            //Iterates through the array of feature modules
            for (int i = 0; i < _featureModule.Count; ++i) 
            {
                if (_featureModule[i].GetFeatureModuleCategory() != pFeatureModuleCategory)
                    continue;

                return _featureModule[i];
            }

            return null;
        }

        /// <summary>
        /// Return specific feature module
        /// </summary>
        /// <typeparam name="FM">FeatureManager</typeparam>
        /// <returns></returns>
        public FM GetFeatureModule<FM>() where FM : FeatureModule{

            return _featureModule.FirstOrDefault(module => module.GetType() == typeof(FM)) as FM;
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

        #region Method

        public override void Initialize()
        {
            base.Initialize();

            for (int i = 0; i < _featureModule.Count; ++i)
                _featureModule[i].Initialize();
        }

        public override void OnAwake(SturdyComponent pSturdyComponent)
        {
            base.OnAwake(pSturdyComponent);

            ReloadFeatureModule();

            for (int i = 0; i < _featureModule.Count; ++i)
                _featureModule[i].OnAwake(pSturdyComponent);
        }

        public override bool OnUpdate()
        {
            if (!base.OnUpdate())
                return false;

            for (int i = 0; i < _featureModule.Count; ++i)
                _featureModule[i].OnUpdate();

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

            Manager.Main main = SerializedPropertyHelper.GetTargetObjectWithProperty(property) as Manager.Main;

            FeatureManager featureManager = SerializedPropertyHelper.GetTargetObjectOfProperty(property) as FeatureManager;

            if (featureManager == null)
                drawer.EndProperty();

            if (_reloadFeatureModuleFlag)
            {
                _reloadFeatureModuleFlag = false;

                featureManager.ReloadFeatureModule(main as SturdyComponent);
            }

            drawer.Space();

            FeatureModuleWrapper[] moduleWrappers = main.GetComponents<FeatureModuleWrapper>();

            if (moduleWrappers.Length == 0)
            {
                drawer.Info("Use 'Add Component' button to add a module. Modules will appear here as they are added.");
                drawer.EndProperty();

                return true;
            }

            drawer.Label("Module Categories:");

            FeatureModule.FeatureModuleCategory[] moduleCategories =
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

            FeatureModule.FeatureModuleCategory category = moduleCategories[categoryIndex];

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