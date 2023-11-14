using System;

using UnityEngine;

using SturdyMachine.Inputs;
using SturdyMachine.Features.Focus;
using SturdyMachine.Component;
using SturdyMachine.Offense;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace SturdyMachine.Features.Focus 
{
    /// <summary>
    /// Module allowing the management of MonsterBot selection in fights
    /// </summary>
    [Serializable]
    public partial class FocusModule : FeatureModule 
    {
        /// <summary>
        /// The focus of the object
        /// </summary>
        [SerializeField, Tooltip("he focus of the object")]
        Transform _currentFocus;

        /// <summary>
        /// EnnemyBot Index
        /// </summary>
        [SerializeField, Tooltip("EnnemyBot Index")]
        int _currentEnnemyBotIndex;

        /// <summary>
        /// Value representing if you are already looking left
        /// </summary>
        bool _lastLookLeftState;

        /// <summary>
        /// Value representing if you are already looking right
        /// </summary>
        bool _lastLookRightState;

        /// <summary>
        /// Time in seconds before the next focus change
        /// </summary>
        [SerializeField, Range(0f, 5f), Tooltip("Time in seconds before the next focus change")]
        float _maxTimer;

        /// <summary>
        /// The time of the timer currently
        /// </summary>
        [SerializeField, Tooltip("The time of the timer currently")]
        float _currentTimer;

        /// <summary>
        /// Returns the EnnemyBot GameObject that is in focus
        /// </summary>
        public BotData GetCurrentEnnemyBotData => _ennemyBotData[_currentEnnemyBotIndex];

        /// <summary>
        /// Returns the object the player is looking at
        /// </summary>
        public Transform GetCurrentFocus => _currentFocus;

        public override FeatureModuleCategory GetFeatureModuleCategory()
        {
            return FeatureModuleCategory.Focus;
        }

        public override bool OnUpdate()
        {
            if (!base.OnUpdate())
                return false;

            LookSetup();

            return true;
        }

        /// <summary>
        /// Assigns current Focus as well as player positioning and the MonsterBot that wants to battle so that it looks at itself
        /// </summary>
        void LookSetup() 
        {
            if (_ennemyBotData.Length == 0)
                return;

            //Manages the positioning of the MonsterBot
            MonsterBotLook();

            //Manages the positioning of the player
            SturdyBotLook();
        }

        /// <summary>
        /// Manages the speed of rotation so that there is fluidity in its movement
        /// </summary>
        void MonsterBotLook() 
        {
            for (int i = 0; i < _ennemyBotData.Length; ++i)
            {
                if (_ennemyBotData[i].botObject != null) 
                {
                    //Smooths the rotation so that it is fluid
                    if (_ennemyBotData[i].botObject.transform.rotation != Quaternion.Slerp(_ennemyBotData[i].botObject.transform.rotation, Quaternion.LookRotation(_sturdyBotData.botObject.transform.position - _ennemyBotData[i].botObject.transform.position), 0.07f))
                        _ennemyBotData[i].botObject.transform.rotation = Quaternion.Slerp(_ennemyBotData[i].botObject.transform.rotation, Quaternion.LookRotation(_sturdyBotData.botObject.transform.position - _ennemyBotData[i].botObject.transform.position), 0.07f);
                }
            }
        }

        /// <summary>
        /// Manage the axis of rotation according to the input of the player and assign the correct value to CurrentMonsterBotIndex
        /// </summary>
        void SturdyBotLook() 
        {
            //Checks if there is a MosnterBot on the battlefield
            if (_ennemyBotData.Length > 1)
            {
                //Checks if the player wants to look left
                if (_isLeftFocusActivated)
                {
                    //Checks if the player is not already looking to the left
                    if (!_lastLookLeftState)
                    {
                        //Assigns the correct index of the MonsterBot the player wants to watch
                        if (_currentEnnemyBotIndex > 0)
                            --_currentEnnemyBotIndex;

                        _lastLookLeftState = true;
                    }
                }
                else if (_lastLookLeftState)
                    _lastLookLeftState = false;

                //Checks if the player wants to look left
                else if (_isRightFocusActivated)
                {
                    //Checks if the player is not already looking to the right
                    if (!_lastLookRightState)
                    {
                        //Assigns the correct index of the MonsterBot the player wants to watch
                        if (_currentEnnemyBotIndex < _ennemyBotData.Length - 1)
                            ++_currentEnnemyBotIndex;

                        _lastLookRightState = true;
                    }
                }
                else if (_lastLookRightState)
                    _lastLookRightState = false;
            }

            //Assign the currentFocus transform based on assigned index
            if (_currentFocus != _ennemyBotData[_currentEnnemyBotIndex].botObject.transform)
                _currentFocus = _ennemyBotData[_currentEnnemyBotIndex].botObject.transform;

            //Manages a smooth rotation that allows the player to pivot quietly towards the right target
            _sturdyBotData.botObject.transform.rotation = Quaternion.Slerp(_sturdyBotData.botObject.transform.rotation, Quaternion.LookRotation(_ennemyBotData[_currentEnnemyBotIndex].botObject.transform.position - _sturdyBotData.botObject.transform.position), 0.07f);

            //Manages smooth rotation that allows the MonterBot to pivot quietly towards the player
            _sturdyBotData.botObject.transform.position = Vector3.Lerp(_sturdyBotData.botObject.transform.position, _ennemyBotData[_currentEnnemyBotIndex].botObject.transform.position - _ennemyBotData[_currentEnnemyBotIndex].focusRange, 0.5f);
        }
    }

#if UNITY_EDITOR

    [CustomPropertyDrawer(typeof(FocusModule))]
    public partial class FocusModuleDrawer : FeatureModuleDrawer 
    {
        public override bool OnNUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (!base.OnNUI(position, property, label))
                return false;

            drawer.BeginSubsection("Debug value");

            drawer.Field("_currentEnnemyBotIndex", false);

            drawer.EndSubsection();

            drawer.EndProperty();
            return true;
        }
    }

#endif

}