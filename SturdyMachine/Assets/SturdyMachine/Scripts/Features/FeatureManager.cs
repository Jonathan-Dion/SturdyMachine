using System;
using System.Linq;
using System.Collections.Generic;

using UnityEngine;

using SturdyMachine.Component;

using SturdyMachine.Offense;
using SturdyMachine.Offense.Blocking;

using SturdyMachine.Features.Fight;
using SturdyMachine.Features.Focus;
using SturdyMachine.Features.Fight.Sequence;
using SturdyMachine.Features.HitConfirm;
using SturdyMachine.Features.StateConfirm;

#if UNITY_EDITOR
using UnityEditor;
using NWH.NUI;
using NWH.VehiclePhysics2;
#endif

namespace SturdyMachine.Features 
{
    public struct DamageDataCache
    {
        public float sturdyDamageIntensity;

        public float enemyDamageIntensity;
    }

    [Serializable]
    public partial class FeatureManager : ModuleComponent
    {
        #region Attribut

        /// <summary>
        /// Array that has all the feature modules that the bot has
        /// </summary>
        [SerializeField]
        List<FeatureModule> _featureModule;

        //EnemyBot Components
        BotType[] _enemyBotType;
        GameObject[] _enemyBotObject;
        Animator[] _enemyBotAnimator;
        Vector3[] _enemyBotFocusRange;
        OffenseManager[] _enemyBotOffenseManager;

        //SturdyBot Components
        GameObject _sturdyBotObject;
        Animator _sturdyBotAnimator;
        OffenseManager _sturdyBotOffenseManager;

        FightOffenseSequenceManager _fightOffenseSequenceManager;
        OffenseBlockingConfig _offenseBlockingConfig;

        #endregion

        #region Properties

        /// <summary>
        /// Return all the feature modules that bot has
        /// </summary>
        public List<FeatureModule> GetFeatureModules => _featureModule;

        /// <summary>
        /// Allow to return specific feature module
        /// </summary>
        /// <param name="pFeatureModuleCategory">The categoryFeature of the module you need to fetch</param>
        /// <returns>Returns the feature module matching the Feature category sent as a parameter</returns>
        FeatureModule GetSpecificFeatureModule(FeatureModuleCategory pFeatureModuleCategory) 
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
        public void ReloadFeatureModule(BaseComponent pBaseComponent = null)
        {
            if (_featureModule == null)
                _featureModule = new List<FeatureModule>();
            else
                _featureModule.Clear();

            if (pBaseComponent)
                _baseComponent = pBaseComponent;

            List<FeatureModuleWrapper> featureModuleWrapper = _baseComponent.GetComponents<FeatureModuleWrapper>()?.ToList();

            if (featureModuleWrapper.Count == 0 || featureModuleWrapper == null)
                return;

            for (int i = 0; i < featureModuleWrapper.Count; ++i)
                _featureModule.Add(featureModuleWrapper[i].GetFeatureModule());
        }

        public FocusModule GetFocusModule => GetSpecificFeatureModule(FeatureModuleCategory.Focus) as FocusModule;

        public FightsModule GetFightsModule => GetSpecificFeatureModule(FeatureModuleCategory.Fight) as FightsModule;

        public HitConfirmModule GetHitConfirmModule => GetSpecificFeatureModule(FeatureModuleCategory.HitConfirm) as HitConfirmModule;

        public StateConfirmModule GetStateConfirmModule => GetSpecificFeatureModule(FeatureModuleCategory.StateConfirm) as StateConfirmModule;

        //EnemyBot Component
        public BotType GetEnemyBotType(byte pIndex) => _enemyBotType[pIndex];
        public BotType GetCurrentEnemyBotType => _enemyBotType[GetFocusModule.GetCurrentEnemyBotIndex];
        public GameObject[] GetEnemyBotObject => _enemyBotObject;
        public GameObject GetCurrentEnemyBotObject => _enemyBotObject[GetFocusModule.GetCurrentEnemyBotIndex];
        public Animator[] GetEnemyBotAnimator => _enemyBotAnimator;
        public Vector3 GetCurrentEnemyBotFocusRange => _enemyBotFocusRange[GetFocusModule.GetCurrentEnemyBotIndex];

        //SturdyBot Component
        public GameObject GetSturdyBotObject => _sturdyBotObject;
        public Animator GetSpecificBotAnimatorByType(BotType pSpecificBotType) 
        {
            //Sturdy
            if (pSpecificBotType == BotType.SturdyBot)
                return _sturdyBotAnimator;

            return _enemyBotAnimator[GetFocusModule.GetCurrentEnemyBotIndex];
        }

        public FightOffenseSequenceManager GetFightOffenseSequenceManager => _fightOffenseSequenceManager;
        public OffenseBlockingConfig GetOffenseBlockingConfig => _offenseBlockingConfig;

        public FightOffenseSequenceData GetFightOffenseSequenceData(BotType pEnemyBotType) => GetFightOffenseSequenceManager.GetFightOffenseSequence(pEnemyBotType).GetFightOffenseSequenceData;

        public OffenseManager GetSpecificOffenseManagerBotByType(BotType pSpecificBotType) 
        {
            //Sturdy
            if (pSpecificBotType == BotType.SturdyBot)
                return _sturdyBotOffenseManager;

            return _enemyBotOffenseManager[GetFocusModule.GetCurrentEnemyBotIndex];
        }

        public AnimationClip GetSpecificBotAnimationClipByType(BotType pSpecificBotType)
        {
            //Sturdy
            if (pSpecificBotType == BotType.SturdyBot)
                return _sturdyBotAnimator.GetCurrentAnimatorClipInfo(0)[0].clip;

            return _enemyBotAnimator[GetFocusModule.GetCurrentEnemyBotIndex].GetCurrentAnimatorClipInfo(0)[0].clip;
        }

        public AnimatorStateInfo GetSpecificAnimatorStateInfoByBotType(BotType pSpecificBotType)
        {
            //Sturdy
            if (pSpecificBotType == BotType.SturdyBot)
                return _sturdyBotAnimator.GetCurrentAnimatorStateInfo(0);

            return _enemyBotAnimator[GetFocusModule.GetCurrentEnemyBotIndex].GetCurrentAnimatorStateInfo(0);
        }

        bool GetIsRandomizeOffenseIndex(AnimationClipOffenseType pAnimationClipOffense) 
        {
            if (pAnimationClipOffense == AnimationClipOffenseType.Parry)
                return false;

            if (pAnimationClipOffense == AnimationClipOffenseType.Stagger)
                return false;

            return true;
        }

        #endregion

        #region Method

        public virtual void Initialize(List<object> pSturdyBotComponent, List<List<object>> pEnemyBotComponent, FightOffenseSequenceManager pFightOffenseSequenceManager, OffenseBlockingConfig pOffenseBlockingConfig) 
        {
            base.Initialize();

            _fightOffenseSequenceManager = pFightOffenseSequenceManager;
            _offenseBlockingConfig = pOffenseBlockingConfig;

            //SturdyBot
            for (byte i = 0; i < pSturdyBotComponent.Count; ++i) {

                //GameObject
                if (pSturdyBotComponent[i] as GameObject)
                {

                    _sturdyBotObject = pSturdyBotComponent[i] as GameObject;

                    continue;
                }

                //Animator
                if (pSturdyBotComponent[i] as Animator)
                {

                    _sturdyBotAnimator = pSturdyBotComponent[i] as Animator;

                    continue;
                }

                //OffenseManager
                if (pSturdyBotComponent[i] as OffenseManager)
                {

                    _sturdyBotOffenseManager = pSturdyBotComponent[i] as OffenseManager;
                }
            }

            //EnemyBot
            _enemyBotType = new BotType[pEnemyBotComponent.Count];
            _enemyBotObject = new GameObject[pEnemyBotComponent.Count];
            _enemyBotAnimator = new Animator[pEnemyBotComponent.Count];
            _enemyBotOffenseManager = new OffenseManager[pEnemyBotComponent.Count];
            _enemyBotFocusRange = new Vector3[pEnemyBotComponent.Count];

            for (byte i = 0; i < pEnemyBotComponent.Count; ++i) {

                for (byte j = 0; j < pEnemyBotComponent[i].Count; ++j) {

                    //BotType
                    if (pEnemyBotComponent[i][j] is BotType enemyBotType) {

                        _enemyBotType[i] = enemyBotType;

                        continue;
                    }

                    //GameObject
                    if (pEnemyBotComponent[i][j] is GameObject enemyBotObject) {

                        _enemyBotObject[i] = enemyBotObject;

                        continue;
                    }

                    //Animator
                    if (pEnemyBotComponent[i][j] is Animator enemyBotAnimator)
                    {

                        _enemyBotAnimator[i] = enemyBotAnimator;

                        continue;
                    }

                    //OffenseManager
                    if (pEnemyBotComponent[i][j] is OffenseManager enemyBotOffenseManager)
                    {

                        _enemyBotOffenseManager[i] = enemyBotOffenseManager;

                        _enemyBotOffenseManager[i].AssignCurrentOffense(_enemyBotAnimator[i].GetCurrentAnimatorClipInfo(0)[0].clip.name);

                        continue;
                    }

                    //FocusRange
                    if (pEnemyBotComponent[i][j] is Vector3 enemyBotFocusRange)
                    {

                        _enemyBotFocusRange[i] = enemyBotFocusRange;

                        continue;
                    }
                }
            }                

            for (byte i = 0; i < _featureModule.Count; ++i)
                _featureModule[i].Initialize(this);
        }

        public override void OnAwake(BaseComponent pBaseComponent) {

            base.OnAwake(pBaseComponent);

            ReloadFeatureModule();
        }

        public virtual bool OnUpdate(bool pIsLeftFocus, bool pIsRightFocus)
        {
            if (!base.OnUpdate())
                return false;

            for (int i = 0; i < _featureModule.Count; ++i) 
            {
                if (!_featureModule[i].OnUpdate(pIsLeftFocus, pIsRightFocus))
                    return true;
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

            for (byte i = 0; i < _enemyBotOffenseManager.Length; ++i)
                _enemyBotOffenseManager[i].OnDisable();
        }

        public void ApplyCurrentOffense(BotType pCurrentBotType, OffenseType pOffenseType, OffenseDirection pOffenseDirection, AnimationClipOffenseType pAnimationClipOffenseType) 
        {
            if (GetSpecificOffenseManagerBotByType(pCurrentBotType).GetIsCurrentOffenseAlreadyAssigned(GetSpecificBotAnimationClipByType(pCurrentBotType), pOffenseType, pOffenseDirection, pAnimationClipOffenseType))
                return;

            if (GetIsRandomizeOffenseIndex(pAnimationClipOffenseType))
                GetSpecificOffenseManagerBotByType(pCurrentBotType).AssignCurrentOffense(GetSpecificOffenseManagerBotByType(pCurrentBotType).GetOffense(pOffenseType, pOffenseDirection).GetAnimationClip(pAnimationClipOffenseType).name);

            else
                GetSpecificOffenseManagerBotByType(pCurrentBotType).AssignCurrentOffense(GetSpecificOffenseManagerBotByType(pCurrentBotType).GetCurrentOffense.GetAnimationClip(pAnimationClipOffenseType).name);

            if (GetSpecificBotAnimationClipByType(pCurrentBotType) != GetSpecificOffenseManagerBotByType(pCurrentBotType).GetCurrentOffense.GetAnimationClip(pAnimationClipOffenseType))
                GetSpecificBotAnimatorByType(pCurrentBotType).Play(GetSpecificOffenseManagerBotByType(pCurrentBotType).GetCurrentOffense.GetAnimationClip(pAnimationClipOffenseType).name);
            else
                GetSpecificBotAnimatorByType(pCurrentBotType).Play(GetSpecificOffenseManagerBotByType(pCurrentBotType).GetCurrentOffense.GetAnimationClip(pAnimationClipOffenseType).name, -1, 0f);
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

            BaseComponent BaseComponent = SerializedPropertyHelper.GetTargetObjectWithProperty(property) as BaseComponent;

            FeatureManager featureManager = SerializedPropertyHelper.GetTargetObjectOfProperty(property) as FeatureManager;

            if (featureManager == null)
                drawer.EndProperty();

            if (_reloadFeatureModuleFlag)
            {
                _reloadFeatureModuleFlag = false;

                featureManager.ReloadFeatureModule(BaseComponent);
            }

            drawer.Space();

            FeatureModuleWrapper[] moduleWrappers = BaseComponent.GetComponents<FeatureModuleWrapper>();

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