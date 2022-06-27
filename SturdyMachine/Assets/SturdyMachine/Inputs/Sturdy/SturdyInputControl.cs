using System;

using UnityEngine;

using UnityEngine.InputSystem.Interactions;

#if UNITY_EDITOR
using UnityEditor;
using NWH.NUI;
#endif

namespace SturdyMachine.Inputs 
{
    public partial class SturdyInputControl : MonoBehaviour 
    {
        SturdyMachineControls _sturdyMachineControls;

        [SerializeField]
        bool _isInitialized, _isEnabled;

        bool _isStanceActivated;

        //Focus
        [SerializeField]
        bool _isLeftFocus, _isRightFocus;

        OffenseDirection _currentOffenseDirection;
        OffenseType _currentOffenseType;

        public bool GetIsActivated => _isInitialized && _isEnabled;
        public bool GetIsInitialized => _isInitialized;

        public bool GetIsStanceActivated => _isStanceActivated;
        public OffenseDirection GetOffenseDirection => _currentOffenseDirection;
        public OffenseType GetOffenseType => _currentOffenseType;

        public bool GetIsLeftFocusActivated => _isLeftFocus;
        public bool GetIsRightFocusActivated => _isRightFocus;

        void Initialize()
        {
            _sturdyMachineControls.Deflection.Enable();
            _sturdyMachineControls.Sweep.Enable();

            //Stance
            _sturdyMachineControls.Stance.Enable();

            //Focus
            _sturdyMachineControls.Focus.Enable();

            _isInitialized = true;
        }

        public virtual void Awake() 
        {
            _sturdyMachineControls = new SturdyMachineControls();

            OffenseStanceSetup();
        }

        public virtual void LateUpdate() 
        {
            if (!GetIsActivated)
                return;

            SturdySetup();
        }

        public virtual void OnEnable() 
        {
            if (!_isInitialized)
            {
                if (Application.isPlaying)
                    Initialize();
            }

            _isEnabled = true;
        }

        public virtual void OnDisable() 
        {
            _sturdyMachineControls.Deflection.Disable();
            _sturdyMachineControls.Sweep.Disable();

            _sturdyMachineControls.Stance.Disable();

            _sturdyMachineControls.Focus.Disable();

            _isEnabled = false;
            _isInitialized = false;
        }

        void SturdySetup()
        {
            if (!_isStanceActivated)
            {
                if (_currentOffenseType != OffenseType.DEFAULT)
                    _currentOffenseType = OffenseType.DEFAULT;
            }

            if (_currentOffenseDirection != OffenseDirection.STANCE)
                _currentOffenseDirection = OffenseDirection.STANCE;
        }

        void OffenseStanceSetup()
        {
            #region Strikes

            _sturdyMachineControls.Stance.Strikes.performed += context =>
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

            _sturdyMachineControls.Stance.Strikes.canceled += context =>
            {
                if (_isStanceActivated)
                    _isStanceActivated = false;
            };

            #endregion

            #region Heavy

            _sturdyMachineControls.Stance.Heavy.performed += context =>
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

            _sturdyMachineControls.Stance.Heavy.canceled += context =>
            {
                if (_isStanceActivated)
                    _isStanceActivated = false;
            };

            #endregion

            #region DeathBlow

            _sturdyMachineControls.Stance.DeathBlow.performed += context =>
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

            _sturdyMachineControls.Stance.DeathBlow.canceled += context =>
            {
                if (_isStanceActivated)
                    _isStanceActivated = false;

            };

            #endregion

            #region Deflection

            //Neutral
            _sturdyMachineControls.Deflection.Neutral.performed += context =>
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

            _sturdyMachineControls.Deflection.Neutral.canceled += context =>
            {
                if (_isStanceActivated)
                {
                    if (_currentOffenseDirection != OffenseDirection.STANCE)
                        _currentOffenseDirection = OffenseDirection.STANCE;
                }
            };

            //Right
            _sturdyMachineControls.Deflection.Right.performed += context =>
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

            _sturdyMachineControls.Deflection.Right.canceled += context =>
            {
                if (_isStanceActivated)
                {
                    if (_currentOffenseDirection != OffenseDirection.STANCE)
                        _currentOffenseDirection = OffenseDirection.STANCE;
                }
            };

            //Left
            _sturdyMachineControls.Deflection.Left.performed += context =>
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

            _sturdyMachineControls.Deflection.Left.canceled += context =>
            {
                if (_isStanceActivated)
                {
                    if (_currentOffenseDirection != OffenseDirection.STANCE)
                        _currentOffenseDirection = OffenseDirection.STANCE;
                }
            };

            //Evasion
            _sturdyMachineControls.Deflection.Evasion.performed += context =>
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

            _sturdyMachineControls.Deflection.Evasion.canceled += context =>
            {
                if (_isStanceActivated)
                {
                    if (_currentOffenseDirection != OffenseDirection.STANCE)
                        _currentOffenseDirection = OffenseDirection.STANCE;
                }
            };

            #endregion

            #region Focus

            //Left
            _sturdyMachineControls.Focus.Left.started += context =>
            {
                if (context.interaction is PressInteraction)
                    _isLeftFocus = true;
            };

            _sturdyMachineControls.Focus.Left.canceled += context => 
            {
                if (_isLeftFocus)
                    _isLeftFocus = false;
            };

            //Right
            _sturdyMachineControls.Focus.Right.started += context =>
            {
                if (context.interaction is PressInteraction)
                    _isRightFocus = true;
            };

            _sturdyMachineControls.Focus.Right.canceled += context =>
            {
                if (_isRightFocus)
                    _isRightFocus = false;
            };

            #endregion
        }
    }

#if UNITY_EDITOR

    [CustomEditor(typeof(SturdyInputControl))]
    public partial class SturdyInputControlEditor : NVP_NUIEditor
    {
        public override bool OnInspectorNUI()
        {
            if (!base.OnInspectorNUI())
                return false;

            drawer.BeginSubsection("Debug Value");

            drawer.Field("_isInitialized", false, null, "Is Initialized: ");

            drawer.BeginSubsection("Focus");

            drawer.Field("_isLeftFocus", false, null, "LookLeft: ");
            drawer.Field("_isRightFocus", false, null, "LookRight: ");

            drawer.EndSubsection();

            drawer.EndSubsection();

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