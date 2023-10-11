using System;

using UnityEngine;
using UnityEngine.InputSystem.Interactions;

using SturdyMachine.Component;
using SturdyMachine.Offense;

#if UNITY_EDITOR
using UnityEditor;
using NWH.NUI;
#endif

namespace SturdyMachine.Inputs 
{
    /// <summary>
    /// Class for managing all gameplay input for the player
    /// </summary>
    public partial class SturdyInputControl : SturdyComponent 
    {
        #region Attribut

        /// <summary>
        /// InputControl gnerated by SturdyMachineControls InputSystem
        /// </summary>
        SturdyMachineControls _sturdyMachineControls;

        /// <summary>
        /// Represents the state of the key to execute the Stance type offense
        /// </summary>
        bool _isStanceActivated;

        /// <summary>
        /// Represents the state of the key to execute a left focus
        /// </summary>
        [SerializeField, Tooltip("Represents the state of the key to execute a left focus")]
        bool _isLeftFocus;

        /// <summary>
        /// Represents the state of the key to execute a left focus
        /// </summary>
        [SerializeField, Tooltip("Represents the state of the key to execute a left focus")]
        bool _isRightFocus;

        /// <summary>
        /// Represents the direction of offense you want to run
        /// </summary>
        OffenseDirection _currentOffenseDirection;

        /// <summary>
        /// Represents the type of offense you want to run
        /// </summary>
        OffenseType _currentOffenseType;

        #endregion

        #region Get

        /// <summary>
        /// Return the current state of Stance Offense type
        /// </summary>
        public bool GetIsStanceActivated => _isStanceActivated;

        /// <summary>
        /// Return the current direction of offense you want to run
        /// </summary>
        public OffenseDirection GetOffenseDirection => _currentOffenseDirection;

        /// <summary>
        /// Return the current type of offense you want to run
        /// </summary>
        public OffenseType GetOffenseType => _currentOffenseType;

        /// <summary>
        /// Return the current state of the key to execute a left focus
        /// </summary>
        public bool GetIsLeftFocusActivated => _isLeftFocus;

        /// <summary>
        /// Return the current state of the key to execute a right focus
        /// </summary>
        public bool GetIsRightFocusActivated => _isRightFocus;

        #endregion

        public override void Initialize()
        {
            base.Initialize();

            _sturdyMachineControls.Deflection.Enable();
            _sturdyMachineControls.Sweep.Enable();

            //Stance
            _sturdyMachineControls.Stance.Enable();

            //Focus
            _sturdyMachineControls.Focus.Enable();
        }

        public override void OnAwake()
        {
            base.OnAwake();

            _sturdyMachineControls = new SturdyMachineControls();

            OffenseStanceSetup();
        }

        public override bool OnLateUpdate() {

            if (!base.OnLateUpdate())
                return false;

            SturdySetup();

            return true;

        }

        public override void OnDisabled()
        {
            base.OnDisabled();

            _sturdyMachineControls.Deflection.Disable();
            _sturdyMachineControls.Sweep.Disable();

            _sturdyMachineControls.Stance.Disable();

            _sturdyMachineControls.Focus.Disable();
        }

        /// <summary>
        /// Method for managing the assignment of the Stance type offense based on the state of the keys assigned to it
        /// </summary>
        void SturdySetup()
        {
            //If the current Stance offense type state if false
            if (!_isStanceActivated)
            {
                //Assign de default offense type
                if (_currentOffenseType != OffenseType.DEFAULT)
                    _currentOffenseType = OffenseType.DEFAULT;
            }

            //Assign the Stance offense type if the current Stance offense type state is true
            if (_currentOffenseDirection != OffenseDirection.STANCE)
                _currentOffenseDirection = OffenseDirection.STANCE;
        }

        /// <summary>
        /// Method for managing the assignment of the direction and type of offense based on the keys pressed
        /// </summary>
        void OffenseStanceSetup()
        {
            #region Strikes

            //Check if there is a key press in the Stance Strikes section
            _sturdyMachineControls.Stance.Strikes.performed += context =>
            {
                //Check if the Strikes Stance offense inputs interaction HoldInteracton is true 
                if (context.interaction is HoldInteraction)
                {
                    if (!_isStanceActivated)
                        _isStanceActivated = true;
                }
                else if (_isStanceActivated)
                    _isStanceActivated = false;

                //Assign the Strikes Stance offense if the input interaction HoldInteraction is true
                if (_isStanceActivated)
                {
                    if (_currentOffenseDirection != OffenseDirection.STANCE)
                        _currentOffenseDirection = OffenseDirection.STANCE;

                    if (_currentOffenseType != OffenseType.STRIKE)
                        _currentOffenseType = OffenseType.STRIKE;
                }
            };

            //Check if there is a key onpress in the Stance Strikes section
            _sturdyMachineControls.Stance.Strikes.canceled += context =>
            {
                if (_isStanceActivated)
                    _isStanceActivated = false;
            };

            #endregion

            #region Heavy

            //Check if there is a key press in the Stance Heavy section
            _sturdyMachineControls.Stance.Heavy.performed += context =>
            {
                //Check if the Strikes Stance offense inputs interaction HoldInteracton is true 
                if (context.interaction is HoldInteraction)
                {
                    if (!_isStanceActivated)
                        _isStanceActivated = true;
                }
                else if (_isStanceActivated)
                    _isStanceActivated = false;

                //Assign the Heavy Stance offense if the input interaction HoldInteraction is true
                if (_isStanceActivated)
                {
                    if (_currentOffenseDirection != OffenseDirection.STANCE)
                        _currentOffenseDirection = OffenseDirection.STANCE;

                    if (_currentOffenseType != OffenseType.HEAVY)
                        _currentOffenseType = OffenseType.HEAVY;
                }
            };

            //Check if there is a key onpress in the Stance Heavy section
            _sturdyMachineControls.Stance.Heavy.canceled += context =>
            {
                if (_isStanceActivated)
                    _isStanceActivated = false;
            };

            #endregion

            #region DeathBlow

            //Check if there is a key press in the Stance Deathblow section
            _sturdyMachineControls.Stance.DeathBlow.performed += context =>
            {
                //Check if the Deathblow Stance offense inputs interaction HoldInteracton is true 
                if (context.interaction is HoldInteraction)
                {
                    if (!_isStanceActivated)
                        _isStanceActivated = true;
                }
                else if (_isStanceActivated)
                    _isStanceActivated = false;

                //Assign the Deathblow Stance offense if the input interaction HoldInteraction is true
                if (_isStanceActivated)
                {
                    if (_currentOffenseDirection != OffenseDirection.STANCE)
                        _currentOffenseDirection = OffenseDirection.STANCE;

                    if (_currentOffenseType != OffenseType.DEATHBLOW)
                        _currentOffenseType = OffenseType.DEATHBLOW;
                }
            };

            //Check if there is a key onpress in the Stance Deathblow section
            _sturdyMachineControls.Stance.DeathBlow.canceled += context =>
            {
                if (_isStanceActivated)
                    _isStanceActivated = false;

            };

            #endregion

            #region Deflection

            #region Neutral

            //Check if there is a key press in the Neutral Deflection offense section
            _sturdyMachineControls.Deflection.Neutral.performed += context =>
            {
                //Check if the Neutral Deflection offense inputs interaction PressInteraction is true 
                if (context.interaction is PressInteraction)
                {
                    _currentOffenseDirection = OffenseDirection.NEUTRAL;

                    //Assign the Deflection offense type if the Stance offense type is not activated
                    if (!_isStanceActivated)
                    {
                        if (_currentOffenseType != OffenseType.DEFLECTION)
                            _currentOffenseType = OffenseType.DEFLECTION;
                    }
                }
            };

            //Check if there is a key onpress in the Neutral deflection
            _sturdyMachineControls.Deflection.Neutral.canceled += context =>
            {
                if (_isStanceActivated)
                {
                    if (_currentOffenseDirection != OffenseDirection.STANCE)
                        _currentOffenseDirection = OffenseDirection.STANCE;
                }
            };

            #endregion

            #region Right

            //Check if there is a key press in the Right Deflection offense section
            _sturdyMachineControls.Deflection.Right.performed += context =>
            {
                //Check if the Right Deflection offense inputs interaction PressInteraction is true 
                if (context.interaction is PressInteraction)
                {
                    if (_currentOffenseDirection != OffenseDirection.RIGHT)
                        _currentOffenseDirection = OffenseDirection.RIGHT;

                    //Assign the Deflection offense type if the Stance offense type is not activated
                    if (!_isStanceActivated)
                    {
                        if (_currentOffenseType != OffenseType.DEFLECTION)
                            _currentOffenseType = OffenseType.DEFLECTION;
                    }
                }
            };

            //Check if there is a key onpress in the Right deflection
            _sturdyMachineControls.Deflection.Right.canceled += context =>
            {
                if (_isStanceActivated)
                {
                    if (_currentOffenseDirection != OffenseDirection.STANCE)
                        _currentOffenseDirection = OffenseDirection.STANCE;
                }
            };

            #endregion

            #region Left

            //Check if there is a key press in the Right Deflection offense section
            _sturdyMachineControls.Deflection.Left.performed += context =>
            {
                //Check if the Left Deflection offense inputs interaction PressInteraction is true 
                if (context.interaction is PressInteraction)
                {
                    if (_currentOffenseDirection != OffenseDirection.LEFT)
                        _currentOffenseDirection = OffenseDirection.LEFT;

                    //Assign the Deflection offense type if the Stance offense type is not activated
                    if (!_isStanceActivated)
                    {
                        if (_currentOffenseType != OffenseType.DEFLECTION)
                            _currentOffenseType = OffenseType.DEFLECTION;
                    }
                };
            };

            //Check if there is a key onpress in the Left deflection
            _sturdyMachineControls.Deflection.Left.canceled += context =>
            {
                if (_isStanceActivated)
                {
                    if (_currentOffenseDirection != OffenseDirection.STANCE)
                        _currentOffenseDirection = OffenseDirection.STANCE;
                }
            };

            #endregion

            #region Evasion

            //Check if there is a key press in the Evasion offense section
            _sturdyMachineControls.Deflection.Evasion.performed += context =>
            {
                //Check if the Evasion offense inputs interaction PressInteraction is true 
                if (context.interaction is PressInteraction)
                {
                    //Assign the Neutral Sweep offense if the Stance offense type is activated
                    if (_isStanceActivated)
                    {
                        if (_currentOffenseType == OffenseType.STRIKE) {

                            if (_currentOffenseDirection != OffenseDirection.NEUTRAL)
                                _currentOffenseDirection = OffenseDirection.NEUTRAL;

                            if (_currentOffenseType != OffenseType.SWEEP)
                                _currentOffenseType = OffenseType.SWEEP;
                        }
                    }

                    //Assign the Neutral Evasion offense if the Stance offense type is not activated
                    else
                    {
                        if (_currentOffenseDirection != OffenseDirection.NEUTRAL)
                            _currentOffenseDirection = OffenseDirection.NEUTRAL;

                        if (_currentOffenseType != OffenseType.EVASION)
                            _currentOffenseType = OffenseType.EVASION;
                    }
                }
            };

            //Check if there is a key onpress in the Evasion deflection
            _sturdyMachineControls.Deflection.Evasion.canceled += context =>
            {
                if (_isStanceActivated)
                {
                    if (_currentOffenseDirection != OffenseDirection.STANCE)
                        _currentOffenseDirection = OffenseDirection.STANCE;

                    if (_currentOffenseType == OffenseType.SWEEP)
                        _currentOffenseType = OffenseType.STRIKE;
                }
            };

            #endregion

            #endregion

            #region Focus

            #region Left

            //Check if there is a key press to change the player's LookLeft
            _sturdyMachineControls.Focus.Left.started += context =>
            {
                //Check if the LookLeft inputs interaction PressInteraction is true 
                if (context.interaction is PressInteraction)
                    _isLeftFocus = true;
            };

            //Check if there is a key onPress to change the player's LookLeft
            _sturdyMachineControls.Focus.Left.canceled += context => 
            {
                if (_isLeftFocus)
                    _isLeftFocus = false;
            };

            #endregion

            #region Right

            //Check if there is a key press to change the player's LookRight
            _sturdyMachineControls.Focus.Right.started += context =>
            {
                //Check if the LookRight inputs interaction PressInteraction is true 
                if (context.interaction is PressInteraction)
                    _isRightFocus = true;
            };

            //Check if there is a key onPress to change the player's LookRight
            _sturdyMachineControls.Focus.Right.canceled += context =>
            {
                if (_isRightFocus)
                    _isRightFocus = false;
            };

            #endregion

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