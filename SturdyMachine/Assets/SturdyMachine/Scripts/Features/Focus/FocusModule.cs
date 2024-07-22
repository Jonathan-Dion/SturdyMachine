using System;

using UnityEngine;

using SturdyMachine.Inputs;
using SturdyMachine.Features.Focus;
using SturdyMachine.Component;
using SturdyMachine.Offense;
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
        #region Attribut

        /// <summary>
        /// EnnemyBot Index
        /// </summary>
        [SerializeField, Tooltip("EnnemyBot Index")]
        int _currentEnnemyBotIndex, _lastEnemyBotIndex;

        bool _isEnemyBotFocusChanged;

        #endregion

        #region Properties

        public override FeatureModuleCategory GetFeatureModuleCategory() => FeatureModuleCategory.Focus;

        public bool GetIsEnemyBotFocusChanged => _isEnemyBotFocusChanged;

        public int GetCurrentEnemyBotIndex => _currentEnnemyBotIndex;

        #endregion

        #region Methods

        public override bool OnUpdate(bool pIsLeftFocus, bool pIsRightFocus)
        {
            if (!base.OnUpdate())
                return false;

            if (FEATURE_MANAGER.GetEnemyBotObject.Length == 0)
                return false;

            if (FEATURE_MANAGER.GetHitConfirmModule.GetIsHitConfirmActivated)
                return true;

            //Manages the positioning of the EnemyBot
            for (int i = 0; i < FEATURE_MANAGER.GetEnemyBotObject.Length; ++i)
            {
                if (!FEATURE_MANAGER.GetEnemyBotObject[i])
                    continue;

                //Smooths the rotation so that it is fluid
                FEATURE_MANAGER.GetEnemyBotObject[i].transform.rotation = Quaternion.Slerp(FEATURE_MANAGER.GetEnemyBotObject[i].transform.rotation, Quaternion.LookRotation(FEATURE_MANAGER.GetSturdyBotObject.transform.position - FEATURE_MANAGER.GetEnemyBotObject[i].transform.position), 0.07f);
            }

            //Manages the positioning of the player
            //Checks if there is a EnemyBot on the battlefield
            if (FEATURE_MANAGER.GetEnemyBotObject.Length > 1)
            {

                //Checks if the player wants to look left
                if (pIsLeftFocus)
                {

                    //Assigns the correct index of the MonsterBot the player wants to watch
                    if (_currentEnnemyBotIndex > 0)
                        --_currentEnnemyBotIndex;
                }

                //Checks if the player wants to look left
                if (pIsRightFocus)
                {
                    //Assigns the correct index of the MonsterBot the player wants to watch
                    if (_currentEnnemyBotIndex < FEATURE_MANAGER.GetEnemyBotObject.Length - 1)
                        ++_currentEnnemyBotIndex;
                }
            }

            //Assign the currentFocus transform based on assigned index
            _isEnemyBotFocusChanged = _lastEnemyBotIndex != _currentEnnemyBotIndex;

            if (_isEnemyBotFocusChanged)
                _lastEnemyBotIndex = _currentEnnemyBotIndex;

            //Manages a smooth rotation that allows the player to pivot quietly towards the right target
            FEATURE_MANAGER.GetSturdyBotObject.transform.rotation = Quaternion.Slerp(FEATURE_MANAGER.GetSturdyBotObject.transform.rotation, Quaternion.LookRotation(FEATURE_MANAGER.GetCurrentEnemyBotObject.transform.position - FEATURE_MANAGER.GetSturdyBotObject.transform.position), 0.07f);

            //Manages smooth rotation that allows the MonterBot to pivot quietly towards the player
            FEATURE_MANAGER.GetSturdyBotObject.transform.position = Vector3.Lerp(FEATURE_MANAGER.GetSturdyBotObject.transform.position, FEATURE_MANAGER.GetCurrentEnemyBotObject.transform.position - FEATURE_MANAGER.GetCurrentEnemyBotFocusRange, 0.5f);

            return true;
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