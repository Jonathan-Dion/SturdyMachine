using UnityEngine;

using SturdyMachine.Component;
using SturdyMachine.Inputs;
using SturdyMachine.Bot;

using SturdyMachine.Features;
using SturdyMachine.Features.Fight;

using SturdyMachine.Offense;
using SturdyMachine.Offense.Blocking;
using SturdyMachine.Features.HitConfirm;
using System;
using NWH.VehiclePhysics2;
using UnityEditor.Experimental.GraphView;

#if UNITY_EDITOR
using UnityEditor;
using NWH.NUI;
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

    public partial class Main : SturdyComponent
    {
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
        MonsterBot[] _monsterBot;

        [SerializeField]
        SturdyInputControlDebugData _sturdyInputControlDebugData;

        public SturdyBot GetSturdyBot => _sturdyBot;

        public OffenseBlockingConfig GetOffenseBlockingConfig => _offenseBlockingConfig;

        public SturdyInputControl GetSturdyInputControl => _sturdyInputControl;

        GameObject[] GetEnnemyBot() {

            GameObject[] ennemyBot = new GameObject[_monsterBot.Length];

            for (byte i = 0; i < ennemyBot.Length; ++i)
                ennemyBot[i] = _monsterBot[i].gameObject;

            return ennemyBot;
        
        }

        Vector3[] GetEnnemyBotRangeFocus() {
        
            Vector3[] ennemyBotFocusRange = new Vector3[_monsterBot.Length];

            for (int i = 0; i < ennemyBotFocusRange.Length; ++i)
                ennemyBotFocusRange[i] = _monsterBot[i].GetFocusRange;

            return ennemyBotFocusRange;

        }
        
        Animator[] GetEnnemyBotAnimator() {

            Animator[] animator = new Animator[_monsterBot.Length];

            for (byte i = 0; i < _monsterBot.Length; ++i)
                animator[i] = _monsterBot[i].GetAnimator;

            return animator;
        
        }

        float[] GetEnnemyBotBlockingChance() {

            float[] ennemyBotBlockingChance = new float[_monsterBot.Length];

            for (byte i = 0; i < _monsterBot.Length; ++i)
                ennemyBotBlockingChance[i] = _monsterBot[i].GetBlockingChance;

            return ennemyBotBlockingChance;
        }

        OffenseManager[] GetEnnemyBotOffenseManager() {
        
            OffenseManager[] ennemyBotOffenseManager = new OffenseManager[_monsterBot.Length];

            for (byte i = 0; i < _monsterBot.Length; ++i)
                ennemyBotOffenseManager[i] = Instantiate(_monsterBot[i].GetOffenseManager);

            return ennemyBotOffenseManager;
        }

        BotData[] GetEnnemyBotData() {

            BotData[] ennemyBotData = new BotData[_monsterBot.Length];

            GameObject[] ennemyBotObject = GetEnnemyBot();

            Vector3[] ennemyBotRangeFocus = GetEnnemyBotRangeFocus();

            Animator[] ennemyBotAnimator = GetEnnemyBotAnimator();

            float[] ennemyBotBlockingChance = GetEnnemyBotBlockingChance();

            OffenseManager[] ennemyBotOffenseManager = GetEnnemyBotOffenseManager();

            for (byte i = 0; i < ennemyBotData.Length; ++i) {

                ennemyBotData[i] = new BotData();

                ennemyBotData[i].botObject = ennemyBotObject[i];

                ennemyBotData[i].focusRange = ennemyBotRangeFocus[i];

                ennemyBotData[i].animator = ennemyBotAnimator[i];

                ennemyBotData[i].blockingChance = ennemyBotBlockingChance[i];

                ennemyBotData[i].offenseManager = ennemyBotOffenseManager[i];
            
            }

            return ennemyBotData;
        
        }

        BotData GetSturdyBotData() {

            BotData sturdyBotData = new BotData();

            sturdyBotData.botObject = _sturdyBot.gameObject;

            sturdyBotData.animator = _sturdyBot.GetAnimator;

            sturdyBotData.offenseManager = _sturdyBot.GetOffenseManager;

            return sturdyBotData;
        }

        public FeatureManager GetFeatureManager => _featureManager;

        public OffenseDirection GetSturdyOffenseDirection() {

            if (_sturdyInputControlDebugData.isActivated)
                return _sturdyInputControlDebugData.offenseDirection;

            return _sturdyInputControl.GetOffenseDirection;
        }

        public OffenseType GetSturdyOffenseType()
        {

            if (_sturdyInputControlDebugData.isActivated)
                return _sturdyInputControlDebugData.offenseType;

            return _sturdyInputControl.GetOffenseType;
        }

        public bool GetIsSturdyOffenseStance()
        {

            if (_sturdyInputControlDebugData.isActivated)
                return _sturdyInputControlDebugData.offenseDirection == OffenseDirection.STANCE;

            return _sturdyInputControl.GetIsStanceActivated;
        }

        void Awake()
        {
            base.OnAwake();

            _sturdyInputControl = new SturdyInputControl();

            _sturdyInputControl.OnAwake();

            _sturdyBot.OnAwake();

            for (int i = 0; i < _monsterBot.Length; ++i)
                _monsterBot[i].OnAwake();

            _featureManager.OnAwake(this, GetEnnemyBotData());
        }

        void Update()
        {
            if (!base.OnUpdate())
                return;

            _sturdyBot.OnUpdate(GetSturdyOffenseDirection(), GetSturdyOffenseType(), _offenseCancelConfig);

            /*for (int i = 0; i < _monsterBot.Length; ++i)
                _monsterBot[i].OnUpdate(_featureManager.GetFeatureModule<FightModule>());*/

            //_featureManager.OnUpdate();
        }

        void LateUpdate()
        {
            if (!base.OnLateUpdate())
                return;

            _sturdyInputControl.OnLateUpdate();

            _sturdyBot.OnLateUpdate();

            for (int i = 0; i < _monsterBot.Length; ++i)
                _monsterBot[i].OnLateUpdate();
        }

        void FixedUpdate()
        {
            if (!base.OnFixedUpdate())
                return;
        }

        void OnEnable()
        {
            base.OnEnabled();

            _sturdyInputControl.OnEnabled();
            _sturdyBot.OnEnabled();
            _featureManager.OnEnabled();

            for (int i = 0; i < _monsterBot.Length; ++i)
                _monsterBot[i].OnEnabled();

        }

        void OnDisable()
        {
            base.OnDisabled();

            _sturdyInputControl.OnDisabled();
            _sturdyBot.OnDisabled();
            _featureManager.OnDisabled();

            for (int i = 0; i < _monsterBot.Length; ++i) 
                _monsterBot[i].OnDisabled();
        }

        public override void Initialize()
        {
            base.Initialize();

            _featureManager.Initialize(GetSturdyBotData(), _sturdyInputControl, _offenseBlockingConfig);

            _featureManager.Initialize();

            _sturdyBot.Initialize();

            MonsterBotInit();
        }

        void MonsterBotInit() 
        {
            Features.Fight.FightModule fightModule = new Features.Fight.FightModule();

            FightModuleWrapper fightModuleWrapper = GetComponent<Features.Fight.FightModuleWrapper>();

            if (fightModuleWrapper)
                fightModule = fightModuleWrapper.GetFightModule;

            for (int i = 0; i < _monsterBot.Length; ++i)
                _monsterBot[i].Initialize(fightModule.GetFightOffenseSequence);
        }
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

            drawer.ReorderableList("_monsterBot");
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