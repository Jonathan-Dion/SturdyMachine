using System;

using UnityEngine;
using UnityEngine.InputSystem.Interactions;

using SturdyMachine.Component;
using SturdyMachine.Offense;
using UnityEngine.InputSystem;
using System.Runtime.Remoting.Contexts;

#if UNITY_EDITOR
using UnityEditor;
using NWH.NUI;
#endif

namespace SturdyMachine.Inputs 
{
    /// <summary>
    /// Class for managing all gameplay input for the player
    /// </summary>
    public partial class SturdyInputControl : SturdyModuleComponent 
    {
        #region Attribut

        /// <summary>
        /// InputControl gnerated by SturdyMachineControls InputSystem
        /// </summary>
        SturdyMachineControls _sturdyMachineControls;

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

        OffenseType _lastStanceOffenseType;

        bool _isStanceActivated;

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

        public virtual void OnAwake()
        {
            _sturdyMachineControls = new SturdyMachineControls();

            OffenseInputSetup();
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
            if (!GetIsStanceActivated)
            {
                //Assign de default offense type
                if (_currentOffenseType != OffenseType.DEFAULT)
                    _currentOffenseType = OffenseType.DEFAULT;
            }

            //Assign the Stance offense type if the current Stance offense type state is true
            if (_currentOffenseDirection != OffenseDirection.STANCE)
                _currentOffenseDirection = OffenseDirection.STANCE;
        }

        #region StanceInput

        /// <summary>
        /// Method for managing states concerning Stance type offenses
        /// </summary>
        /// <param name="pInputAction">Inputs linked to the player's ActionMap</param>
        /// <param name="pOffenseType">Type of offense linked to the ActionMap</param>
        void SpecificOffenseStanceSetup(InputAction pInputAction, OffenseType pOffenseType) {

            //Check if there is a key press in the Stance section
            pInputAction.performed += context => {

                //Assign the Stance offense & OffenseType if the input interaction HoldInteraction is true
                if (context.interaction is HoldInteraction) {

                    if (!_isStanceActivated)
                        _isStanceActivated = true;

                    if (_currentOffenseDirection != OffenseDirection.STANCE)
                        _currentOffenseDirection = OffenseDirection.STANCE;
                        
                    if (_currentOffenseType != pOffenseType)
                        _currentOffenseType = pOffenseType;

                    _lastStanceOffenseType = _currentOffenseType;
                }
            };

            //Check if there is a key onpress in the Stance section
            pInputAction.canceled += context =>
            {
                if (_isStanceActivated)
                    _isStanceActivated = false;

                if (_currentOffenseDirection != OffenseDirection.DEFAULT)
                    _currentOffenseDirection = OffenseDirection.DEFAULT;

                _lastStanceOffenseType = OffenseType.DEFAULT;
            };
        }

        /// <summary>
        /// Method for managing all Stance OffenseType ActionMaps
        /// </summary>
        void OffenseStanceSetup() {

            //Strike
            SpecificOffenseStanceSetup(_sturdyMachineControls.Stance.Strikes, OffenseType.STRIKE);

            //Heavy
            SpecificOffenseStanceSetup(_sturdyMachineControls.Stance.Heavy, OffenseType.HEAVY);

            //DeathBlow
            SpecificOffenseStanceSetup(_sturdyMachineControls.Stance.DeathBlow, OffenseType.DEATHBLOW);
        }

        #endregion

        #region DirectionOffenseInput

        /// <summary>
        /// Method for managing states concerning OffenseDirection offenses
        /// </summary>
        /// <param name="pInputAction">Inputs linked to the player's ActionMap</param>
        /// <param name="pOffenseDirection">OffenseDirection linked to the ActionMap</param>
        void SpecificOffenseDirectionSetup(InputAction pInputAction, OffenseDirection pOffenseDirection)
        {
            //Check if there is a key press in the OffenseDirection offense section
            pInputAction.performed += context =>
            {
                //Check if the OffenseDirection offense inputs interaction PressInteraction is true 
                if (context.interaction is PressInteraction)
                {
                    if (_currentOffenseDirection != pOffenseDirection)
                        _currentOffenseDirection = pOffenseDirection;

                    //Assign the OffenseType if the StanceOffense type is not activated
                    if (!GetIsStanceActivated)
                    {
                        if (_currentOffenseType != OffenseType.DEFLECTION)
                            _currentOffenseType = OffenseType.DEFLECTION;
                    }
                }
            };

            //Check if there is a key onpress in the OffenseDirection
            pInputAction.canceled += context =>
            {
                if (GetIsStanceActivated)
                {
                    if (_currentOffenseDirection != OffenseDirection.STANCE)
                        _currentOffenseDirection = OffenseDirection.STANCE;

                    if (_currentOffenseType != _lastStanceOffenseType)
                        _currentOffenseType = _lastStanceOffenseType;
                }
            };
        }

        /// <summary>
        /// Method for managing all OffenseDirection ActionMaps
        /// </summary>
        void OffenseDirectionSetup() {

            //Left
            SpecificOffenseDirectionSetup(_sturdyMachineControls.Deflection.Left, OffenseDirection.LEFT);

            //Neutral
            SpecificOffenseDirectionSetup(_sturdyMachineControls.Deflection.Neutral, OffenseDirection.NEUTRAL);

            //Right
            SpecificOffenseDirectionSetup(_sturdyMachineControls.Deflection.Right, OffenseDirection.RIGHT);
        }

        #endregion

        /// <summary>
        /// Method for managing OffenseType and OffenseDirection depending on the keys used
        /// </summary>
        void OffenseInputSetup() {

            //Stance
            OffenseStanceSetup();

            //Direction
            OffenseDirectionSetup();

            #region Evasion

            //Check if there is a key press in the Evasion offense section
            _sturdyMachineControls.Deflection.Evasion.performed += context =>
            {
                //Check if the Evasion offense inputs interaction PressInteraction is true 
                if (context.interaction is PressInteraction)
                {
                    //Assign the Neutral Sweep offense if the Stance offense type is activated
                    if (GetIsStanceActivated)
                    {
                        if (_currentOffenseType == OffenseType.STRIKE)
                        {

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
                if (GetIsStanceActivated)
                {
                    if (_currentOffenseDirection != OffenseDirection.STANCE)
                        _currentOffenseDirection = OffenseDirection.STANCE;

                    if (_currentOffenseType == OffenseType.SWEEP)
                        _currentOffenseType = OffenseType.STRIKE;
                }
            };

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

            /*#region Neutral

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

            #endregion*/
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