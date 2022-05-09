using UnityEngine;
using UnityEngine.InputSystem.Interactions;

#if UNITY_EDITOR
using UnityEditor;
#endif

using ICustomEditor.Class;
using Feature.Manager;

using Humanoid.Bot.Sturdy;
using Humanoid.Bot.Monster;
using SturdyMachine.Offense.Blocking.Manager;

namespace GameManager 
{
    public class GameManager : UnityICustomEditor
    {
        //Sturdy
        [SerializeField]
        SturdyMachineControls _sturdyMachineControl;

        [SerializeField]
        SturdyBot _sturdyBot;

        //MonsterBot
        [SerializeField]
        MonsterBot _monsterBot;

        [SerializeField]
        OffenseCancelConfig _offenseCancelConfig;

        [SerializeField]
        OffenseBlockingConfig _offenseBlockingConfig;

        OffenseDirection _currentOffenseDirection;
        OffenseType _currentOffenseType;

        bool _isStanceActivated;

        [SerializeField]
        FeatureManager _featureManager;

        public FeatureManager GetFeatureManager { get { return _featureManager; } }

        public override void Awake()
        {
            //Sturdy
            _sturdyMachineControl = new SturdyMachineControls();

            OffenseStanceSetup();

            _sturdyBot.Awake();

            //MonsterBot
            _monsterBot.Awake();

            _featureManager.Awake();
        }

        public override void Start()
        {
            _currentOffenseDirection = OffenseDirection.STANCE;
            _currentOffenseType = OffenseType.DEFAULT;

            //Sturdy
            _sturdyBot.Start();

            //MonsterBot
            _monsterBot.Start();

            _featureManager.Start();
        }

        void Update()
        {
            SturdySetup();

            //MonsterBot
            _monsterBot.CustomUpdate(OffenseDirection.LEFT, OffenseType.STRIKE);

            _featureManager.CustomUpdate(_sturdyBot.transform.position);
        }

        void LateUpdate()
        {
            _sturdyBot.LateUpdateRemote(_currentOffenseDirection);

            _monsterBot.CustomLateUpdate(OffenseDirection.LEFT);

            _featureManager.LateUpdate();
        }

        void OnEnable()
        {
            _sturdyMachineControl.Deflection.Enable();
            _sturdyMachineControl.Sweep.Enable();
            _sturdyMachineControl.Strikes.Enable();
            _sturdyMachineControl.Heavy.Enable();
            _sturdyMachineControl.DeathBlow.Enable();
        }

        void OnDisable()
        {
            _sturdyMachineControl.Deflection.Disable();
            _sturdyMachineControl.Sweep.Disable();
            _sturdyMachineControl.Strikes.Disable();
            _sturdyMachineControl.Heavy.Disable();
            _sturdyMachineControl.DeathBlow.Disable();
        }

        void OffenseStanceSetup()
        {
            #region Strikes

            _sturdyMachineControl.Strikes.Strikes.performed += context =>
            {
                if (context.interaction is HoldInteraction)
                {
                    if (!_isStanceActivated)
                        _isStanceActivated = true;
                }
                else if (_isStanceActivated)
                    _isStanceActivated = false;

                if (_isStanceActivated)
                {
                    if (_currentOffenseDirection != OffenseDirection.STANCE)
                        _currentOffenseDirection = OffenseDirection.STANCE;

                    if (_currentOffenseType != OffenseType.STRIKE)
                        _currentOffenseType = OffenseType.STRIKE;
                }
            };

            _sturdyMachineControl.Strikes.Strikes.canceled += context =>
            {
                if (_isStanceActivated)
                    _isStanceActivated = false;
            };

            #endregion

            #region Heavy

            _sturdyMachineControl.Heavy.Heavy.performed += context =>
            {
                if (context.interaction is HoldInteraction)
                {
                    if (!_isStanceActivated)
                        _isStanceActivated = true;
                }
                else if (_isStanceActivated)
                    _isStanceActivated = false;

                if (_isStanceActivated)
                {
                    if (_currentOffenseDirection != OffenseDirection.STANCE)
                        _currentOffenseDirection = OffenseDirection.STANCE;

                    if (_currentOffenseType != OffenseType.HEAVY)
                        _currentOffenseType = OffenseType.HEAVY;
                }
            };

            _sturdyMachineControl.Heavy.Heavy.canceled += context =>
            {
                if (_isStanceActivated)
                    _isStanceActivated = false;
            };

            #endregion

            #region DeathBlow

            _sturdyMachineControl.DeathBlow.DeathBlow.performed += context =>
            {
                if (context.interaction is HoldInteraction)
                {
                    if (!_isStanceActivated)
                        _isStanceActivated = true;
                }
                else if (_isStanceActivated)
                    _isStanceActivated = false;

                if (_isStanceActivated)
                {
                    if (_currentOffenseDirection != OffenseDirection.STANCE)
                        _currentOffenseDirection = OffenseDirection.STANCE;

                    if (_currentOffenseType != OffenseType.DEATHBLOW)
                        _currentOffenseType = OffenseType.DEATHBLOW;
                }
            };

            _sturdyMachineControl.DeathBlow.DeathBlow.canceled += context =>
            {
                if (_isStanceActivated)
                    _isStanceActivated = false;

            };

            #endregion

            #region Deflection

            //Neutral
            _sturdyMachineControl.Deflection.Neutral.performed += context =>
            {
                if (context.interaction is PressInteraction)
                {
                    _currentOffenseDirection = OffenseDirection.NEUTRAL;

                    if (!_isStanceActivated)
                    {
                        if (_currentOffenseType != OffenseType.DEATHBLOW)
                            _currentOffenseType = OffenseType.DEFLECTION;
                    }
                }
            };

            _sturdyMachineControl.Deflection.Neutral.canceled += context =>
            {
                if (_isStanceActivated)
                {
                    if (_currentOffenseDirection != OffenseDirection.STANCE)
                        _currentOffenseDirection = OffenseDirection.STANCE;
                }
            };

            //Right
            _sturdyMachineControl.Deflection.Right.performed += context =>
            {
                if (context.interaction is PressInteraction)
                {
                    if (_currentOffenseDirection != OffenseDirection.RIGHT)
                        _currentOffenseDirection = OffenseDirection.RIGHT;

                    if (!_isStanceActivated)
                    {
                        if (_currentOffenseType != OffenseType.DEFLECTION)
                            _currentOffenseType = OffenseType.DEFLECTION;
                    }
                }
            };

            _sturdyMachineControl.Deflection.Right.canceled += context =>
            {
                if (_isStanceActivated)
                {
                    if (_currentOffenseDirection != OffenseDirection.STANCE)
                        _currentOffenseDirection = OffenseDirection.STANCE;
                }
            };

            //Left
            _sturdyMachineControl.Deflection.Left.performed += context =>
            {
                if (context.interaction is PressInteraction)
                {
                    if (_currentOffenseDirection != OffenseDirection.LEFT)
                        _currentOffenseDirection = OffenseDirection.LEFT;

                    if (!_isStanceActivated)
                    {
                        if (_currentOffenseType != OffenseType.DEFLECTION)
                            _currentOffenseType = OffenseType.DEFLECTION;
                    }
                };
            };

            _sturdyMachineControl.Deflection.Left.canceled += context =>
            {
                if (_isStanceActivated)
                {
                    if (_currentOffenseDirection != OffenseDirection.STANCE)
                        _currentOffenseDirection = OffenseDirection.STANCE;
                }
            };

            //Evasion
            _sturdyMachineControl.Deflection.Evasion.performed += context =>
            {
                if (context.interaction is PressInteraction)
                {
                    if (_isStanceActivated)
                    {
                        if (_currentOffenseDirection != OffenseDirection.NEUTRAL)
                            _currentOffenseDirection = OffenseDirection.NEUTRAL;

                        if (_currentOffenseType != OffenseType.SWEEP)
                            _currentOffenseType = OffenseType.SWEEP;

                    }
                    else
                    {
                        if (_currentOffenseDirection != OffenseDirection.NEUTRAL)
                            _currentOffenseDirection = OffenseDirection.NEUTRAL;

                        if (_currentOffenseType != OffenseType.EVASION)
                            _currentOffenseType = OffenseType.EVASION;
                    }
                }
            };

            _sturdyMachineControl.Deflection.Evasion.canceled += context =>
            {
                if (_isStanceActivated)
                {
                    if (_currentOffenseDirection != OffenseDirection.STANCE)
                        _currentOffenseDirection = OffenseDirection.STANCE;
                }
            };

            #endregion
        }

        void SturdySetup()
        {
            _sturdyBot.UpdateRemote(_currentOffenseDirection, _currentOffenseType, _isStanceActivated, _featureManager.GetFocusManager.GetCurrentFocus);

            if (!_isStanceActivated)
            {
                if (_currentOffenseType != OffenseType.DEFAULT)
                    _currentOffenseType = OffenseType.DEFAULT;
            }

            if (_currentOffenseDirection != OffenseDirection.STANCE)
                _currentOffenseDirection = OffenseDirection.STANCE;
        }

#if UNITY_EDITOR

        public override void CustomOnEnable()
        {
            base.CustomOnEnable();

            _featureManager = GetComponent<FeatureManager>();
        }

        public override void CustomOnInspectorGUI()
        {
            GUI.enabled = false;

            _featureManager = (FeatureManager)EditorGUILayout.ObjectField("FeatureManager", _featureManager, typeof(FeatureManager), true);

            GUI.enabled = true;

            #region Configuration

            EditorGUILayout.BeginVertical(GUI.skin.box);

            EditorGUILayout.LabelField("Configuration", _guiStyle);

            #region Offense

            EditorGUILayout.BeginVertical(GUI.skin.box);

            EditorGUILayout.LabelField("Offense", _guiStyle);

            _offenseCancelConfig = EditorGUILayout.ObjectField("Offense cancel:", _offenseCancelConfig, typeof(OffenseCancelConfig), true) as OffenseCancelConfig;
            _offenseBlockingConfig = EditorGUILayout.ObjectField("Offense blocking:", _offenseBlockingConfig, typeof(OffenseBlockingConfig), true) as OffenseBlockingConfig;

            EditorGUILayout.EndVertical();

            #endregion

            EditorGUILayout.EndVertical();

            #endregion

            EditorGUILayout.BeginVertical(GUI.skin.box);

            EditorGUILayout.LabelField("SturdyBot", _guiStyle);

            _sturdyBot = EditorGUILayout.ObjectField("SturdyBot:", _sturdyBot, typeof(SturdyBot), true) as SturdyBot;

            EditorGUILayout.EndVertical();

            //MonsterBot
            EditorGUILayout.BeginVertical(GUI.skin.box);

            EditorGUILayout.LabelField("MonsterBot", _guiStyle);

            _monsterBot = EditorGUILayout.ObjectField("MonsterBot :", _monsterBot, typeof(MonsterBot), true) as MonsterBot;

            EditorGUILayout.EndVertical();

            EditorGUILayout.Space();

        }

        public override void CustomOnDisable()
        {
            base.CustomOnDisable();
        }

#endif
    }
}


