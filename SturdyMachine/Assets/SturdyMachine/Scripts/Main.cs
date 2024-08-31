using System;

using UnityEngine;

using SturdyMachine.Component;
using SturdyMachine.Inputs;
using SturdyMachine.Bot;
using SturdyMachine.UI;

using SturdyMachine.Features;
using SturdyMachine.Features.Fight.Sequence;

using SturdyMachine.Offense;
using SturdyMachine.Offense.Blocking;
using System.Collections.Generic;

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
    public partial class Main : BaseComponent
    {
        #region Attribut

        [SerializeField]
        FeatureManager _featureManager = new FeatureManager();

        [SerializeField]
        SturdyInputControl _sturdyInputControl;

        [SerializeField]
        SturdyBot _sturdyBot;

        [SerializeField]
        OffenseBlockingConfig _offenseBlockingConfig;

        [SerializeField]
        EnnemyBot[] _ennemyBot;

        [SerializeField]
        SturdyInputControlDebugData _sturdyInputControlDebugData;

        [SerializeField]
        FightOffenseSequenceManager _fightOffenseSequenceManager;

        [SerializeField]
        GameplayUI _gameplayUI;

        [SerializeField]
        float _fpsCapsLock;

        float _currentFpsDelay, _maxFpsDelay;

        #endregion

        #region Properties

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
        /// Returns if the party was paused
        /// </summary>
        bool GetIsPauseGameplay => _gameplayUI.GetBattleUI.GetIsEndGame();

        /// <summary>
        /// Returns a list of necessary SturdyBot components for the functionality modules to function
        /// </summary>
        List<object> GetSturdyBotComponent => new List<object>()
        {
            _sturdyBot.gameObject,
            _sturdyBot.GetAnimator,
            _sturdyBot.GetOffenseManager,
        };

        /// <summary>
        /// Returns all EnemyBot components that are necessary for the feature modules to function
        /// </summary>
        /// <param name="pEnemyBot">The enemy bot you want to extract components from</param>
        /// <returns></returns>
        List<object> GetEnemyBotComponent(Bot.Bot pEnemyBot) => new List<object>()
        {
            pEnemyBot.GetBotType,
            pEnemyBot.gameObject,
            pEnemyBot.GetAnimator,
            pEnemyBot.GetOffenseManager,
            pEnemyBot.GetFocusRange,

        };

        /// <summary>
        /// Returns the components of all enemy bots in the game
        /// </summary>
        List<List<object>> GetAllEnemyBotComponent
        {
            get 
            {
                List<List<object>> allEnemyBotComponent = new List<List<object>>();

                for (byte i = 0; i < _ennemyBot.Length; ++i)
                    allEnemyBotComponent.Add(GetEnemyBotComponent(_ennemyBot[i]));

                return allEnemyBotComponent;
            }
        }

        #endregion

        #region Method

        void Awake()
        {
            _sturdyInputControl = new SturdyInputControl();

            _sturdyInputControl.OnAwake();

            _sturdyBot.OnAwake();

            for (int i = 0; i < _ennemyBot.Length; ++i)
                _ennemyBot[i].OnAwake();

            _featureManager.OnAwake(this);

            _gameplayUI.OnAwake();
        }

        void Start()
        {
            _gameplayUI.OnStart(BaseUIType.Battle);

            _gameplayUI.GetBattleUI.GetResetButton.onClick.AddListener(InitGame);

            if (_fpsCapsLock == 0)
                _fpsCapsLock = 30;

            _maxFpsDelay = 1 / _fpsCapsLock;
        }

        void Update()
        {
            if (!base.OnUpdate())
                return;

            _gameplayUI.OnUpdate(_featureManager.GetHitConfirmModule.GetIsHitConfirmActivated, _featureManager.GetHitConfirmModule.GetCurrentEnemyDamageIntensity, _featureManager.GetHitConfirmModule.GetCurrentSturdyDamageIntensity);

            if (GetIsPauseGameplay)
                return;

            _sturdyBot.OnUpdate(GetSturdyOffenseDirection(), GetSturdyOffenseType(), _featureManager.GetStateConfirmModule.GetCurrentCooldownType, _featureManager.GetHitConfirmModule.GetIsHitConfirmActivated);

            for (int i = 0; i < _ennemyBot.Length; ++i)
                _ennemyBot[i].OnUpdate();

            _featureManager.OnUpdate(_sturdyInputControl.GetIsLeftFocusActivated, _sturdyInputControl.GetIsRightFocusActivated);
        }

        void LateUpdate()
        {
            if (!base.OnLateUpdate())
                return;

            if (GetIsPauseGameplay)
                return;

            _sturdyInputControl.OnLateUpdate();

            _sturdyBot.OnLateUpdate();

            for (int i = 0; i < _ennemyBot.Length; ++i)
                _ennemyBot[i].OnLateUpdate();
        }

        void OnEnable()
        {
            base.OnEnabled();

            _sturdyInputControl.OnEnabled();
            _sturdyBot.OnEnabled();
            _featureManager.OnEnabled();

            for (int i = 0; i < _ennemyBot.Length; ++i)
                _ennemyBot[i].OnEnabled();

            _gameplayUI.OnEnabled();

        }

        void OnDisable()
        {
            base.OnDisabled();

            _sturdyInputControl.OnDisabled();
            _sturdyBot.OnDisabled();
            _featureManager.OnDisabled();

            for (int i = 0; i < _ennemyBot.Length; ++i) 
                _ennemyBot[i].OnDisabled();

            _gameplayUI.OnDisabled();
        }

        public override void Initialize()
        {
            base.Initialize();

            _fightOffenseSequenceManager = Instantiate(_fightOffenseSequenceManager);

            _sturdyBot.Initialize();

            _featureManager.Initialize(GetSturdyBotComponent, GetAllEnemyBotComponent, _fightOffenseSequenceManager, _offenseBlockingConfig);

            _gameplayUI.Initialize();
        }

        void InitGame() {

            _gameplayUI.OnStart(BaseUIType.Battle);

            _gameplayUI.OnEnabled();
            _gameplayUI.Initialize();
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
            
            EditorGUI.BeginChangeCheck();

            drawer.Property("_sturdyInputControlDebugData");

            drawer.Field("_fpsCapsLock", true, "fps", "Caps Lock: ");

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
            drawer.Field("_gameplayUI");

            if (drawer.Field("_sturdyBot").objectReferenceValue == null)
                drawer.Info("Vous devez assigner le Prefab SturdyMachine afin de pouvoir continuer la configuration!", MessageType.Error);
            else
            {
                //drawer.Field("_battleUI");
                drawer.Field("_fightOffenseSequenceManager", true, null, "Offense sequence manager: ");

                DrawOffenseConfiguration();
            }
        }

        void DrawOffenseConfiguration() 
        {
            drawer.BeginSubsection("Offense");

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