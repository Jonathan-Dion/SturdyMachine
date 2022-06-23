using SturdyMachine.Features;
using SturdyMachine.Inputs;

using UnityEngine;
using SturdyMachine.Offense.Blocking.Manager;

#if UNITY_EDITOR
using UnityEditor;
using NWH.NUI;
#endif

namespace SturdyMachine.Manager 
{
    public partial class Main : MonoBehaviour
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
        bool _isInitialized;

        public OffenseBlockingConfig GetOffenseBlockingConfig => _offenseBlockingConfig;

        public bool GetIsInitialized => _isInitialized;

        void Awake()
        {
            _sturdyInputControl = new SturdyInputControl();

            _sturdyInputControl.Awake();

            _featureManager.Awake(this, _sturdyBot);

            _sturdyBot.Awake();

            for (int i = 0; i < _monsterBot.Length; ++i)
                _monsterBot[i].Awake();
        }

        void Update()
        {
            if (!_isInitialized)
                return;

            _sturdyBot.UpdateRemote(_sturdyInputControl.GetOffenseDirection, _sturdyInputControl.GetOffenseType, _sturdyInputControl.GetIsStanceActivated, _featureManager.GetSpecificFeatureModule(FeatureModule.FeatureModuleCategory.Fight) as Features.Fight.FightModule, true);

            for (int i = 0; i < _monsterBot.Length; ++i)
                _monsterBot[i].UpdateRemote();

            _featureManager.UpdateRemote(_monsterBot, _sturdyBot, _sturdyInputControl);
        }

        void LateUpdate()
        {
            _sturdyInputControl.LateUpdate();
        }

        void FixedUpdate()
        {
            _featureManager.FixedUpdate();
        }

        void OnEnable()
        {
            _sturdyInputControl.OnEnable();
            _sturdyBot.Enable();
            _featureManager.Enable(_monsterBot, _sturdyBot);

            for (int i = 0; i < _monsterBot.Length; ++i)
                _monsterBot[i].Enable();

            Initialize();
        }

        void OnDisable()
        {
            _sturdyInputControl.OnDisable();
            _sturdyBot.Disable();
            _featureManager.Disable();

            for (int i = 0; i < _monsterBot.Length; ++i) 
            {
                _monsterBot[i].Disable();
            }
        }

        void Initialize() 
        {
            _featureManager.Initialize(_monsterBot, _sturdyBot);

            _sturdyBot.Initialize();

            MonsterBotInit();

            CleanMemory();

            _isInitialized = true;
        }

        void CleanMemory() 
        {
            _featureManager.CleanMemory();
        }

        void MonsterBotInit() 
        {
            Features.Fight.FightModule fightModule = GetComponent<Features.Fight.FightModuleWrapper>().GetFightModule;

            for (int i = 0; i < _monsterBot.Length; ++i)
                _monsterBot[i].Initialize(fightModule.GetFightDataGroup);
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

            drawer.BeginSubsection("Debug Value");

            drawer.Field("_isInitialized", false, null, "Is Initialized: ");

            drawer.EndSubsection();

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