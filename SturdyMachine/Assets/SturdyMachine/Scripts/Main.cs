using UnityEngine;

using SturdyMachine.Component;
using SturdyMachine.Inputs;
using SturdyMachine.Bot;

using SturdyMachine.Features;
using SturdyMachine.Features.Fight;

using SturdyMachine.Offense;
using SturdyMachine.Offense.Blocking;

#if UNITY_EDITOR
using UnityEditor;
using NWH.NUI;
#endif

namespace SturdyMachine.Manager 
{
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

        float[] GetEnnemyBotBlockingRange() {

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

        public FeatureManager GetFeatureManager => _featureManager;

        public bool GetIsInitialized => _isInitialized;

        void Awake()
        {
            base.OnAwake();

            _sturdyInputControl = new SturdyInputControl();

            _sturdyInputControl.OnAwake();

            _featureManager.OnAwake(this, GetEnnemyBot(), GetEnnemyBotRangeFocus(), GetEnnemyBotOffenseManager(), GetEnnemyBotAnimator(), GetEnnemyBotBlockingRange());

            _sturdyBot.OnAwake();

            for (int i = 0; i < _monsterBot.Length; ++i)
                _monsterBot[i].OnAwake();
        }

        void Update()
        {
            if (!base.OnUpdate())
                return;

            _sturdyBot.OnUpdate(_sturdyInputControl.GetOffenseDirection, _sturdyInputControl.GetOffenseType, _sturdyInputControl.GetIsStanceActivated, _featureManager.GetSpecificFeatureModule(FeatureModule.FeatureModuleCategory.Fight) as FightModule);

            for (int i = 0; i < _monsterBot.Length; ++i)
                _monsterBot[i].OnUpdate(_featureManager.GetFeatureModule<FightModule>());

            _featureManager.OnUpdate();
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

            _featureManager.Initialize(_offenseBlockingConfig, _sturdyBot.GetOffenseManager, _sturdyInputControl, _sturdyBot.transform, _sturdyBot.GetAnimator);

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

#endif
}