using UnityEngine;

using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Interactions;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Humanoid.Bot.Sturdy
{
    [RequireComponent(typeof(PlayerInput))]
    public class SturdyBot : Bot
    {
        SturdyMachineControls _sturdyMachineControl;

        bool _isStanceActivated;

        OffenseDirection _currentOffenseDirection;
        OffenseType _currentOffenseType;

        public override void Awake()
        {
            _sturdyMachineControl = new SturdyMachineControls();

            base.Awake();

            OffenseStanceSetup();
        }

        public override void Start()
        {
            _currentOffenseDirection = OffenseDirection.STANCE;
            _currentOffenseType = OffenseType.DEFAULT;

            base.Start();
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
        }

        void Update() 
        {
            base.CustomUpdate(_currentOffenseDirection, _currentOffenseType, _isStanceActivated);

            if (!_isStanceActivated)
            {
                if (_currentOffenseType != OffenseType.DEFAULT)
                    _currentOffenseType = OffenseType.DEFAULT;
            }

            if (_currentOffenseDirection != OffenseDirection.STANCE)
                _currentOffenseDirection = OffenseDirection.STANCE;
        }

        void LateUpdate() 
        {
            base.CustomLateUpdate(_currentOffenseDirection);
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

            /*---DEFLECTION---*/

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

        public override void OnCollisionEnter(Collision pCollision)
        {
            base.OnCollisionEnter(pCollision);
        }

        public override void OnColliserExit(Collision pCollision)
        {
            base.OnColliserExit(pCollision);
        }

#if UNITY_EDITOR
        public override void CustomOnInspectorGUI()
        {
            base.CustomOnInspectorGUI();
        }

#endif
    }
}