using System;

using UnityEngine;
using SturdyMachine.Inputs;

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
        /// MonsterBot Index
        /// </summary>
        [SerializeField, Tooltip("MonsterBot Index")]
        int _currentMonsterBotIndex;

        /// <summary>
        /// Transform player
        /// </summary>
        Transform _sturdyTransform;

        /// <summary>
        /// Array of all MonsterBot in the scene
        /// </summary>
        MonsterBot[] _monsterBot;

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
        /// Player input control
        /// </summary>
        Inputs.SturdyInputControl _sturdyInputControl;

        /// <summary>
        /// Returns the index of the MonsterBot that is in focus
        /// </summary>
        public int GetCurrentMonsterBotIndex => _currentMonsterBotIndex;

        /// <summary>
        /// Returns the object the player is looking at
        /// </summary>
        public Transform GetCurrentFocus => _currentFocus;

        public override FeatureModuleCategory GetFeatureModuleCategory()
        {
            return FeatureModuleCategory.Focus;
        }

        /// <summary>
        /// Initialize all that concerns the MonsterBot
        /// </summary>
        /// <param name="pMonsterBot"></param>
        void InitializeMonsterBot(MonsterBot[] pMonsterBot) {

            //Random current MonsterBot index
            System.Random random = new System.Random();

            if (_monsterBot.Length > 0)
                _currentMonsterBotIndex = random.Next(_monsterBot.Length - 1);

            //Assign MonsterBot array
            _monsterBot = pMonsterBot;
        }

        /// <summary>
        /// Initialize FocusModule
        /// </summary>
        /// <param name="pMonsterBot">MonsterBot array who currently on fight section</param>
        /// <param name="pSturdyBot">Player Bot</param>
        public virtual void InitializeModule(MonsterBot[] pMonsterBot, SturdyBot pSturdyBot, SturdyInputControl pSturdyInputControl) {

            base.Initialize();

            _sturdyTransform = pSturdyBot.transform;

            InitializeMonsterBot(pMonsterBot);

            _sturdyInputControl = pSturdyInputControl;
        }

        public override void OnAwake()
        {
            base.OnAwake();

            LookSetup();
        }

        /// <summary>
        /// Assigns current Focus as well as player positioning and the MonsterBot that wants to battle so that it looks at itself
        /// </summary>
        void LookSetup() 
        {
            if (_monsterBot.Length == 0)
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
            for (int i = 0; i < _monsterBot.Length; ++i)
            {
                if (_monsterBot[i] != null) 
                {
                    //Smooths the rotation so that it is fluid
                    if (_monsterBot[i].transform.rotation != Quaternion.Slerp(_monsterBot[i].transform.rotation, Quaternion.LookRotation(_sturdyTransform.position - _monsterBot[i].transform.position), 0.07f))
                        _monsterBot[i].transform.rotation = Quaternion.Slerp(_monsterBot[i].transform.rotation, Quaternion.LookRotation(_sturdyTransform.position - _monsterBot[i].transform.position), 0.07f);
                }
            }
        }

        /// <summary>
        /// Manage the axis of rotation according to the input of the player and assign the correct value to CurrentMonsterBotIndex
        /// </summary>
        void SturdyBotLook() 
        {
            //Checks if there is a MosnterBot on the battlefield
            if (_monsterBot.Length > 1)
            {
                //Checks if the player wants to look left
                if (_sturdyInputControl.GetIsLeftFocusActivated)
                {
                    //Checks if the player is not already looking to the left
                    if (!_lastLookLeftState)
                    {
                        //Assigns the correct index of the MonsterBot the player wants to watch
                        if (_currentMonsterBotIndex > 0)
                            --_currentMonsterBotIndex;

                        _lastLookLeftState = true;
                    }
                }
                else if (_lastLookLeftState)
                    _lastLookLeftState = false;

                //Checks if the player wants to look left
                else if (_sturdyInputControl.GetIsRightFocusActivated)
                {
                    //Checks if the player is not already looking to the right
                    if (!_lastLookRightState)
                    {
                        //Assigns the correct index of the MonsterBot the player wants to watch
                        if (_currentMonsterBotIndex < _monsterBot.Length - 1)
                            ++_currentMonsterBotIndex;

                        _lastLookRightState = true;
                    }
                }
                else if (_lastLookRightState)
                    _lastLookRightState = false;
            }

            //Assign the currentFocus transform based on assigned index
            if (_currentFocus != _monsterBot[_currentMonsterBotIndex].transform)
                _currentFocus = _monsterBot[_currentMonsterBotIndex].transform;

            //Manages a smooth rotation that allows the player to pivot quietly towards the right target
            _sturdyTransform.rotation = Quaternion.Slerp(_sturdyTransform.rotation, Quaternion.LookRotation(_monsterBot[_currentMonsterBotIndex].transform.position - _sturdyTransform.position), 0.07f);

            //Manages smooth rotation that allows the MonterBot to pivot quietly towards the player
            _sturdyTransform.position = Vector3.Lerp(_sturdyTransform.position, _monsterBot[_currentMonsterBotIndex].transform.position - _monsterBot[_currentMonsterBotIndex].GetFocusRange, 0.5f);
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

            drawer.Field("_currentMonsterBotIndex", false);

            drawer.EndSubsection();

            drawer.EndProperty();
            return true;
        }
    }

#endif

}