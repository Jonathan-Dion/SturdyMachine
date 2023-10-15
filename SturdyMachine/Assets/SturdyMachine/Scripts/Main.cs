using UnityEngine;

using SturdyMachine.Component;
using SturdyMachine.Features;
using SturdyMachine.Inputs;

using SturdyMachine.Offense;
using SturdyMachine.Offense.Blocking;
using SturdyMachine.Features.Fight;

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

        public MonsterBot[] GetMonsterBot => _monsterBot;

        public FeatureManager GetFeatureManager => _featureManager;

        public bool GetIsInitialized => _isInitialized;

        void Awake()
        {
            _sturdyInputControl = new SturdyInputControl();

            _sturdyInputControl.OnAwake();

            _featureManager.OnAwake(this);

            _sturdyBot.OnAwake();

            for (int i = 0; i < _monsterBot.Length; ++i)
                _monsterBot[i].OnAwake();
        }

        void Update()
        {
            if (!_isInitialized)
                return;

            _sturdyBot.OnUpdate(_sturdyInputControl.GetOffenseDirection, _sturdyInputControl.GetOffenseType, _sturdyInputControl.GetIsStanceActivated, _featureManager.GetSpecificFeatureModule(FeatureModule.FeatureModuleCategory.Fight) as Features.Fight.FightModule);

            //_sturdyBot.UpdateRemote(_sturdyInputControl.GetOffenseDirection, _sturdyInputControl.GetOffenseType, _sturdyInputControl.GetIsStanceActivated, _featureManager.GetSpecificFeatureModule(FeatureModule.FeatureModuleCategory.Fight) as Features.Fight.FightModule);

            /*for (int i = 0; i < _monsterBot.Length; ++i)
                _monsterBot[i].UpdateRemote(_featureManager.GetSpecificFeatureModule(FeatureModule.FeatureModuleCategory.Fight) as Features.Fight.FightModule);*/

            _featureManager.OnUpdate();
        }

        void LateUpdate()
        {
            _sturdyInputControl.OnLateUpdate();

            //_sturdyBot.LateUpdateRemote(false);

            for (int i = 0; i < _monsterBot.Length; ++i)
                _monsterBot[i].OnLateUpdate();
        }

        void FixedUpdate()
        {
            _featureManager.OnFixedUpdate();
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