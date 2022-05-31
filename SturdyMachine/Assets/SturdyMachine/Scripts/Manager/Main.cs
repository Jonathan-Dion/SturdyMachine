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
        SturdyInputControl _sturdyInputControl = new SturdyInputControl();

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

        public bool GetIsInitialized => _isInitialized;

        void Awake()
        {
            Initialize();

            _sturdyInputControl.Awake();

            _featureManager.Awake(gameObject);

            _sturdyBot.Awake();

            for (int i = 0; i < _monsterBot.Length; ++i)
                _monsterBot[i].Awake();
        }

        void Update()
        {
            if (!_isInitialized)
                return;

            _sturdyBot.UpdateRemote(_sturdyInputControl.GetOffenseDirection, _sturdyInputControl.GetOffenseType, _sturdyInputControl.GetIsStanceActivated);

            for (int i = 0; i < _monsterBot.Length; ++i)
                _monsterBot[i].UpdateRemote();

            _featureManager.Update();
            _featureManager.UpdateFocus(_sturdyBot.gameObject.transform.position);
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
            _featureManager.Enable();

            for (int i = 0; i < _monsterBot.Length; ++i)
                _monsterBot[i].Enable();
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
            _sturdyInputControl = new SturdyInputControl();

            _featureManager.Initialize();

            _sturdyBot.Initialize();

            for (int i = 0; i < _monsterBot.Length; ++i)
                _monsterBot[i].Initialize();

            _isInitialized = true;
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