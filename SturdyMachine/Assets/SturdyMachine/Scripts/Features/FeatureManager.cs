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

#if UNITY_EDITOR
using UnityEditor;
using NWH.NUI;
using NWH.VehiclePhysics2;
#endif

namespace SturdyMachine.Features 
{
    /// <summary>
    /// Saves all necessary bot elements to be able to operate modules with Bots
    /// </summary>
    [Serializable, Tooltip("Saves all necessary bot elements to be able to operate modules with Bots")]
    public struct BotDataCache
    {
        /// <summary>
        /// Bot Type
        /// </summary>
        [Tooltip("Bot Type")]
        public BotType botType;

        /// <summary>
        /// Information about the bot's GameObject
        /// </summary>
        [Tooltip("Information about the bot's GameObject")]
        public GameObject botObject;

        /// <summary>
        /// Bot focus range information
        /// </summary>
        [Tooltip("Bot focus range information")]
        public Vector3 focusRange;

        /// <summary>
        /// The bot animator
        /// </summary>
        [Tooltip("The bot animator")]
        public Animator botAnimator;

        /// <summary>
        /// Bot Offense Manager
        /// </summary>
        [Tooltip("Bot Offense Manager")]
        public OffenseManager offenseManager;

        /// <summary>
        /// Represents if the Bot is in the blocking phase
        /// </summary>
        [Tooltip("Represents if the Bot is in the blocking phase")]
        public bool isBlocking;

        /// <summary>
        /// Represents if the Bot is in the hitting phase
        /// </summary>
        [Tooltip("Represents if the Bot is in the hitting phase")]
        public bool isHitting;

        public FightOffenseSequence fightOffenseSequence;
    }

    /// <summary>
    /// All cached information regarding the Focus module
    /// </summary>
    [Serializable, Tooltip("All cached information regarding the Focus module")]
    public struct FocusDataCache
    {
        /// <summary>
        /// The focus of the object
        /// </summary>
        [Tooltip("The focus of the object")]
        public GameObject currentEnnemyBotFocus;

        /// <summary>
        /// Represents if the Bot that was in focus has been changed
        /// </summary>
        [Tooltip("Represents if the Bot that was in focus has been changed")]
        public bool ifEnnemyBotFocusChanged;

        /// <summary>
        /// Represents the index of the enemy Bot that is assigned as Focus
        /// </summary>
        [Tooltip("Represents the index of the enemy Bot that is assigned as Focus")]
        public int currentEnnemyBotFocusIndex;
    }

    /// <summary>
    /// All cached information regarding the HitConfirm module
    /// </summary>
    [Serializable, Tooltip("All cached information regarding the HitConfirm module")]
    public struct HitConfirmDataCache
    {
        /// <summary>
        /// Indicates whether HitConfirm has been activated
        /// </summary>
        [Tooltip("Indicates whether HitConfirm has been activated")]
        public bool isInHitConfirm;

        /// <summary>
        /// Represents the cached information of the attacking Bot
        /// </summary>
        [Tooltip("Represents the cached information of the attacking Bot")]
        public BotDataCache attackingBotDataCache;

        /// <summary>
        /// Represents the cached information of the defending Bot
        /// </summary>
        [Tooltip("Represents the cached information of the defending Bot")]
        public BotDataCache defendingBotDataCache;

        /// <summary>
        /// Indicates the wait time in seconds that HitConfirm should take when activated
        /// </summary>
        [Tooltip("Indicates the wait time in seconds that HitConfirm should take when activated")]
        public float hitConfirmMaxTimer;

        /// <summary>
        /// Allows you to check if the Bot Animator speed values ​​have been changed
        /// </summary>
        [Tooltip("Allows you to check if the Bot Animator speed values ​​have been changed")]
        public bool isAssignSpeedBot;
    }

    /// <summary>
    /// All cached information regarding the Fight module
    /// </summary>
    [Serializable, Tooltip("All cached information regarding the Fight module")]
    public struct FightDataCache
    {
        /// <summary>
        /// Represents cached enemy Bot FightOffenseData information
        /// </summary>
        [Tooltip("Represents cached enemy Bot FightOffenseData information")]
        public FightOffenseData currentFightOffenseData;

        public int offenseComboCount;
    }

    /// <summary>
    /// All cached information from all feature modules
    /// </summary>
    [Serializable, Tooltip("All cached information from all feature modules")]
    public struct FeatureCacheData
    {
        /// <summary>
        /// The list of information concerning all ennemyBot
        /// </summary>
        [Tooltip("The list of information concerning all ennemyBot")]
        public BotDataCache[] ennemyBotDataCache;

        /// <summary>
        /// All cached information regarding the Player Bot
        /// </summary>
        [Tooltip("All cached information regarding the Player Bot")]
        public BotDataCache sturdyBotDataCache;

        /// <summary>
        /// All cached information regarding the Focus module
        /// </summary>
        [Tooltip("All cached information regarding the Focus module")]
        public FocusDataCache focusDataCache;

        /// <summary>
        /// All cached information regarding the HitConfirm module
        /// </summary>
        [Tooltip("All cached information regarding the HitConfirm module")]
        public HitConfirmDataCache hitConfirmDataCache;

        /// <summary>
        /// All cached information regarding the FightData module
        /// </summary>
        [Tooltip("All cached information regarding the FightData module")]
        public FightDataCache fightDataCache;

        /// <summary>
        /// AudioSource which allows HitConfirm to play AudioClips
        /// </summary>
        [Tooltip("AudioSource which allows HitConfirm to play AudioClips")]
        public AudioSource audioSource;
    }

    [Serializable]
    public partial class FeatureManager : SturdyModuleComponent
    {
        #region Attribut

        /// <summary>
        /// Array that has all the feature modules that the bot has
        /// </summary>
        [SerializeField]
        List<FeatureModule> _featureModule;

        /// <summary>
        /// All information regarding all feature modules
        /// </summary>
        [Tooltip("All information regarding all feature modules")]
        FeatureCacheData _featureCacheData;

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
        public FeatureModule GetSpecificFeatureModule(FeatureModuleCategory pFeatureModuleCategory) 
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

        /// <summary>
        /// Returns if HitConfirm is activated
        /// </summary>
        public bool GetIfHitConfirmActivated => _featureCacheData.hitConfirmDataCache.isInHitConfirm;

        #endregion

        #region Method

        public virtual void Initialize(BotDataCache pSturdyBotDataCache, BotDataCache[] pEnnemyBotDataCache) 
        {
            base.Initialize();

            FeatureCacheInit(pSturdyBotDataCache, pEnnemyBotDataCache);

            for (byte i = 0; i < _featureModule.Count; ++i)
                _featureModule[i].Initialize(ref _featureCacheData);
        }

        public override void OnAwake(SturdyComponent pSturdyComponent) {

            base.OnAwake(pSturdyComponent);

            ReloadFeatureModule();

            for (byte i = 0; i < _featureModule.Count; ++i)
                _featureModule[i].OnAwake(pSturdyComponent);
        }

        public virtual bool OnUpdate(bool pIsLeftFocus, bool pIsRightFocus, OffenseBlockingConfig pOffenseBlockingConfig)
        {
            if (!base.OnUpdate())
                return false;

            FeatureModuleSetup(pIsLeftFocus, pIsRightFocus, pOffenseBlockingConfig);

            return true;
        }

        public virtual bool OnFixedUpdate(bool pIsLeftFocus, bool pIsRightFocus, OffenseBlockingConfig pOffenseBlockingConfig)
        {

            if (!base.OnFixedUpdate())
                return false;

            for (byte i = 0; i < _featureModule.Count; ++i)
                _featureModule[i].OnFixedUpdate(pIsLeftFocus, pIsRightFocus, pOffenseBlockingConfig, ref _featureCacheData);

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
        /// Allows initialization regarding the basic cached information of the FeatureDatCache
        /// </summary>
        /// <param name="pSturdyBotDataCache">Information about the Player Bot</param>
        /// <param name="pEnnemyBotDataCache">The list of information of all enemy Bots</param>
        void FeatureCacheInit(BotDataCache pSturdyBotDataCache, BotDataCache[] pEnnemyBotDataCache) {

            _featureCacheData = new FeatureCacheData();

            _featureCacheData.ennemyBotDataCache = pEnnemyBotDataCache;

            _featureCacheData.sturdyBotDataCache = pSturdyBotDataCache;

            _featureCacheData.audioSource = _sturdyComponent.GetComponent<AudioSource>();
        }

        /// <summary>
        /// Allows calling the OnUpdate method of all enemy Bots
        /// </summary>
        /// <param name="pIsLeftFocus">Status of Focus shift to the left for the FocusDataCache</param>
        /// <param name="pIsRightFocus">Status of Focus shift to the right for the FocusDataCache</param>
        /// <param name="pOffenseBlockingConfig">Structure for recording all necessary information regarding the attacking offense</param>
        void FeatureModuleSetup(bool pIsLeftFocus, bool pIsRightFocus, OffenseBlockingConfig pOffenseBlockingConfig) {

            for (int i = 0; i < _featureModule.Count; ++i)
                _featureModule[i].OnUpdate(pIsLeftFocus, pIsRightFocus, pOffenseBlockingConfig, ref _featureCacheData);
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