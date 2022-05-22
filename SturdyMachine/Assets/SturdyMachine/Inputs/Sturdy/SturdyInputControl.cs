using System;

using UnityEngine;

using UnityEngine.InputSystem.Interactions;

namespace SturdyMachine.Inputs 
{
    public partial class SturdyInputControl : MonoBehaviour 
    {
        SturdyMachineControls _sturdyMachineControls;

        bool _isInitialized, _isEnabled;

        bool _isStanceActivated;

        OffenseDirection _currentOffenseDirection;
        OffenseType _currentOffenseType;

        public bool GetIsActivated => _isInitialized && _isEnabled;
        public bool GetIsInitialized => _isInitialized;

        public bool GetIsStanceActivated => _isStanceActivated;
        public OffenseDirection GetOffenseDirection => _currentOffenseDirection;
        public OffenseType GetOffenseType => _currentOffenseType;

        void Initialize()
        {
            _sturdyMachineControls.Deflection.Enable();
            _sturdyMachineControls.Sweep.Enable();
            _sturdyMachineControls.Strikes.Enable();
            _sturdyMachineControls.Heavy.Enable();
            _sturdyMachineControls.DeathBlow.Enable();

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
            _sturdyMachineControls.Strikes.Disable();
            _sturdyMachineControls.Heavy.Disable();
            _sturdyMachineControls.DeathBlow.Disable();

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

            _sturdyMachineControls.Strikes.Strikes.performed += context =>
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

            _sturdyMachineControls.Strikes.Strikes.canceled += context =>
            {
                if (_isStanceActivated)
                    _isStanceActivated = false;
            };

            #endregion

            #region Heavy

            _sturdyMachineControls.Heavy.Heavy.performed += context =>
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

            _sturdyMachineControls.Heavy.Heavy.canceled += context =>
            {
                if (_isStanceActivated)
                    _isStanceActivated = false;
            };

            #endregion

            #region DeathBlow

            _sturdyMachineControls.DeathBlow.DeathBlow.performed += context =>
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

            _sturdyMachineControls.DeathBlow.DeathBlow.canceled += context =>
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
        }
    }
}