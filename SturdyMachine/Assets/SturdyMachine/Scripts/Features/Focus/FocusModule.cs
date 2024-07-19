using System;

using UnityEngine;

using SturdyMachine.Offense.Blocking;

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
        #region Attributes

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

        bool _ifEnnemyBotFocusChanged;

        #endregion

        #region Properties

        public override FeatureModuleCategory GetFeatureModuleCategory()
        {
            return FeatureModuleCategory.Focus;
        }

        public bool GetIfEnemyBotFocusChanged => _ifEnnemyBotFocusChanged;

        public GameObject GetCurrentEnemyBotFocus => FEATURE_MANAGER.GetEnemyBotObject[_currentEnnemyBotIndex];

        public int GetCurrentEnemyBotIndex => _currentEnnemyBotIndex;

        #endregion

        #region Methods

        public override bool OnUpdate(bool pIsLeftFocus, bool pIsRightFocus, Vector3 pFocusRange)
        {
            if (!base.OnUpdate(pIsLeftFocus, pIsRightFocus, pFocusRange))
                return false;

            if (GetHitConfirmModule.GetIsHitConfirmActivated)
                return true;

            LookAtFocus(pIsLeftFocus, pIsRightFocus, pFocusRange);

            return true;
        }

        /// <summary>
        /// Assigns current Focus as well as player positioning and the MonsterBot that wants to battle so that it looks at itself
        /// </summary>
        void LookAtFocus(bool pIsLeftFocus, bool pIsRightFocus, Vector3 pFocusRange) 
        {
            if (FEATURE_MANAGER.GetEnemyBotObject.Length == 0)
                return;

            //Manages the positioning of the EnemyBot
            for (int i = 0; i < FEATURE_MANAGER.GetEnemyBotObject.Length; ++i)
            {
                if (!FEATURE_MANAGER.GetEnemyBotObject[i])
                    continue;

                //Smooths the rotation so that it is fluid
                Quaternion slerpingRotation = Quaternion.Slerp(FEATURE_MANAGER.GetEnemyBotObject[i].transform.rotation, Quaternion.LookRotation(FEATURE_MANAGER.GetSturdyBotObject.transform.position - FEATURE_MANAGER.GetEnemyBotObject[i].transform.position), 0.07f);
                
                if (FEATURE_MANAGER.GetEnemyBotObject[i].transform.rotation != slerpingRotation)
                    FEATURE_MANAGER.GetEnemyBotObject[i].transform.rotation = slerpingRotation;
            }

            //Manages the positioning of the player
            SturdyBotLookAtFocus(pIsLeftFocus, pIsRightFocus, pFocusRange);
        }

        /// <summary>
        /// Manage the axis of rotation according to the input of the player and assign the correct value to CurrentMonsterBotIndex
        /// </summary>
        void SturdyBotLookAtFocus(bool pIsLeftFocus, bool pIsRightFocus, Vector3 pFocusRange) 
        {
            if (_ifEnnemyBotFocusChanged)
                _ifEnnemyBotFocusChanged = false;

            //Checks if there is a MosnterBot on the battlefield
            if (FEATURE_MANAGER.GetEnemyBotObject.Length > 1)
            {
                //Checks if the player wants to look left
                if (pIsLeftFocus)
                {
                    //Checks if the player is not already looking to the left
                    if (!_lastLookLeftState)
                    {
                        //Assigns the correct index of the MonsterBot the player wants to watch
                        if (_currentEnnemyBotIndex > 0) {

                            --_currentEnnemyBotIndex;

                            _ifEnnemyBotFocusChanged = true;
                        }

                        _lastLookLeftState = true;
                    }
                }
                else if (_lastLookLeftState)
                    _lastLookLeftState = false;

                //Checks if the player wants to look left
                else if (pIsRightFocus)
                {
                    //Checks if the player is not already looking to the right
                    if (!_lastLookRightState)
                    {
                        //Assigns the correct index of the MonsterBot the player wants to watch
                        if (_currentEnnemyBotIndex < FEATURE_MANAGER.GetEnemyBotObject.Length - 1) {

                            ++_currentEnnemyBotIndex;

                            _ifEnnemyBotFocusChanged = true;
                        }

                        _lastLookRightState = true;
                    }
                }
                else if (_lastLookRightState)
                    _lastLookRightState = false;
            }

            BotLook(FEATURE_MANAGER.GetSturdyBotObject, FEATURE_MANAGER.GetEnemyBotObject[_currentEnnemyBotIndex], pFocusRange);
        }

        #endregion
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