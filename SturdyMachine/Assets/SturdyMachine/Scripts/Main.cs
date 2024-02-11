using UnityEngine;

using SturdyMachine.Component;
using SturdyMachine.Inputs;
using SturdyMachine.Bot;

using SturdyMachine.Features;

using SturdyMachine.Offense;
using SturdyMachine.Offense.Blocking;
using System;

#if UNITY_EDITOR
using UnityEditor;
using NWH.NUI;
using NWH.VehiclePhysics2;
#endif

namespace SturdyMachine.Manager 
{
    /// <summary>
    /// Section to make debugging inputs simpler
    /// </summary>
    [Serializable, Tooltip("Section to make debugging inputs simpler")]
    public struct SturdyInputControlDebugData
    {
        /// <summary>
        /// Indicates whether the tool is enabled
        /// </summary>
        [Tooltip("Indicates whether the tool is enabled")]
        public bool isActivated;

        /// <summary>
        /// Indicates the type of Offense you want to check
        /// </summary>
        [Tooltip("Indicates the type of Offense you want to check")]
        public OffenseDirection offenseDirection;

        /// <summary>
        /// Indicates the direction of Offense you want to check
        /// </summary>
        [Tooltip("Indicates the direction of Offense you want to check")]
        public OffenseType offenseType;
    }

    [RequireComponent(typeof(AudioSource))]
    public partial class Main : SturdyComponent
    {
        #region Attribut

        [SerializeField]
        FeatureManager _featureManager = new FeatureManager();

        [SerializeField]
        SturdyInputControl _sturdyInputControl;

        [SerializeField]
        SturdyBot _sturdyBot;

        [SerializeField]
        OffenseCancelConfig _offenseCancelConfig;

        [SerializeField]
        OffenseBlockingConfig _offenseBlockingConfig;

        [SerializeField]
        EnnemyBot[] _ennemyBot;

        [SerializeField]
        SturdyInputControlDebugData _sturdyInputControlDebugData;

        #endregion

        #region Get

        /// <summary>
        /// Allows access to the direction of the Offense selected with the inputs
        /// </summary>
        /// <returns>Returns the direction of the selected Offense with the inputs</returns>
        public OffenseDirection GetSturdyOffenseDirection() {

            if (_sturdyInputControlDebugData.isActivated)
                return _sturdyInputControlDebugData.offenseDirection;

            return _sturdyInputControl.GetOffenseDirection;
        }

        /// <summary>
        /// Allows access to the direction of the Offense selected with the inputs
        /// </summary>
        /// <returns>Returns the type of the selected Offense with the inputs</returns>
        public OffenseType GetSturdyOffenseType()
        {

            if (_sturdyInputControlDebugData.isActivated)
                return _sturdyInputControlDebugData.offenseType;

            return _sturdyInputControl.GetOffenseType;
        }

        /// <summary>
        /// Allows you to record all important information from all EnnemyBot
        /// </summary>
        /// <returns>Returns a list that contains all the important information for each EnnemyBot</returns>
        BotDataCache[] GetEnnemyBotDataCache() {

            BotDataCache[] botDataCache = new BotDataCache[_ennemyBot.Length];

            for (int i = 0; i < _ennemyBot.Length; ++i)
                botDataCache[i] = GetBotDataCacheInit(_ennemyBot[i]);

            return botDataCache;
        
        }

        /// <summary>
        /// Allows you to assign all important information for a specific EnnemyBot
        /// </summary>
        /// <param name="pEnnemyBot">The specific EnnemyBot</param>
        /// <returns>Returns a structure with all the important information concerning the EnnemyBot as a parameter</returns>
        BotDataCache GetBotDataCacheInit(EnnemyBot pEnnemyBot) {

            BotDataCache botDataCache = new BotDataCache();

            botDataCache.botObject = pEnnemyBot.gameObject;
            botDataCache.botType = pEnnemyBot.GetBotType;
            botDataCache.focusRange = pEnnemyBot.GetFocusRange;
            botDataCache.botAnimator = pEnnemyBot.GetAnimator;
            botDataCache.offenseManager = pEnnemyBot.GetOffenseManager;

            return botDataCache;
        }

        /// <summary>
        /// Allows player bot information to be initialized for caching
        /// </summary>
        /// <returns>Returns player bot information to cache</returns>
        BotDataCache GetSturdyBotDataCache() {

            BotDataCache botDataCache = new BotDataCache();

            botDataCache.botObject = _sturdyBot.gameObject;
            botDataCache.botType = _sturdyBot.GetBotType;
            botDataCache.botAnimator = _sturdyBot.GetAnimator;
            botDataCache.offenseManager = _sturdyBot.GetOffenseManager;

            return botDataCache;
        }

        #endregion

        #region Method

        void Awake()
        {
            base.OnAwake();

            _sturdyInputControl = new SturdyInputControl();

            _sturdyInputControl.OnAwake();

            _sturdyBot.OnAwake();

            for (int i = 0; i < _ennemyBot.Length; ++i)
                _ennemyBot[i].OnAwake();

            _featureManager.OnAwake(this);
        }

        void Update()
        {
            if (!base.OnUpdate())
                return;

            if (!_featureManager.GetIfHitConfirmActivated)
                _sturdyBot.OnUpdate(GetSturdyOffenseDirection(), GetSturdyOffenseType(), _offenseCancelConfig);

            for (int i = 0; i < _ennemyBot.Length; ++i)
                _ennemyBot[i].OnUpdate();

            _featureManager.OnUpdate(_sturdyInputControl.GetIsLeftFocusActivated, _sturdyInputControl.GetIsRightFocusActivated, _offenseBlockingConfig);
        }

        void LateUpdate()
        {
            if (!base.OnLateUpdate())
                return;

            _sturdyInputControl.OnLateUpdate();

            _sturdyBot.OnLateUpdate();

            for (int i = 0; i < _ennemyBot.Length; ++i)
                _ennemyBot[i].OnLateUpdate();
        }

        void FixedUpdate()
        {
            if (!base.OnFixedUpdate())
                return;

            _featureManager.OnFixedUpdate(_sturdyInputControl.GetIsLeftFocusActivated, _sturdyInputControl.GetIsRightFocusActivated, _offenseBlockingConfig);
        }

        void OnEnable()
        {
            base.OnEnabled();

            _sturdyInputControl.OnEnabled();
            _sturdyBot.OnEnabled();
            _featureManager.OnEnabled();

            for (int i = 0; i < _ennemyBot.Length; ++i)
                _ennemyBot[i].OnEnabled();

        }

        void OnDisable()
        {
            base.OnDisabled();

            _sturdyInputControl.OnDisabled();
            _sturdyBot.OnDisabled();
            _featureManager.OnDisabled();

            for (int i = 0; i < _ennemyBot.Length; ++i) 
                _ennemyBot[i].OnDisabled();
        }

        public override void Initialize()
        {
            base.Initialize();

            _sturdyBot.Initialize();

            _featureManager.Initialize(GetSturdyBotDataCache(), GetEnnemyBotDataCache());
        }

        #endregion
    }

#if UNITY_EDITOR

    [CustomEditor(typeof(Main))]
    [CanEditMultipleObjects]
    public partial class MainEditor : NVP_NUIEditor
    {
        public override bool OnInspectorNUI()
        {
            if (!base.OnInspectorNUI())
                return false;
            
            drawer.Property("_sturdyInputControlDebugData");

            EditorGUI.BeginChangeCheck();

            // Draw toolbar
            int categoryTab = drawer.HorizontalToolbar("categoryTab",
                                                       new[]
                                                       {
                                                           "Features",
                                                           "Configuration"
                                                       }, true, true);
            drawer.Space(2);

            if (categoryTab == 0)
                drawer.Property("_featureManager");

            else if (categoryTab == 1)
                DrawConfigurationTab();
            

            drawer.EndEditor(this);
            return true;
        }

        void DrawConfigurationTab() 
        {
            if (drawer.Field("_sturdyBot").objectReferenceValue == null)
                drawer.Info("Vous devez assigner le Prefab SturdyMachine afin de pouvoir continuer la configuration!", MessageType.Error);
            else
            {
                DrawOffenseConfiguration();
            }
        }

        void DrawOffenseConfiguration() 
        {
            drawer.BeginSubsection("Offense");

            drawer.Field("_offenseCancelConfig", true, null, "Cancel: ");
            drawer.Field("_offenseBlockingConfig", true, null, "Blocking: ");

            drawer.EndSubsection();

            drawer.Space();

            drawer.ReorderableList("_ennemyBot");
        }
    }

    [CustomPropertyDrawer(typeof(SturdyInputControlDebugData))]
    public partial class SturdyInputControlDebugDataDrawer : ComponentNUIPropertyDrawer
    {
        public override bool OnNUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (!base.OnNUI(position, property, label))
                return false;

            if (drawer.Field("isActivated").boolValue) {

                drawer.Field("offenseDirection");
                drawer.Field("offenseType");
            }

            drawer.EndProperty();
            return true;
        }
    }

#endif
}