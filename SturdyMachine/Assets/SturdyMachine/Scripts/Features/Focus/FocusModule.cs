using System;

using UnityEngine;

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
        byte _currentEnnemyBotIndex;

        [SerializeField]
        int _lastEnemyBotIndex;

        /// <summary>
        /// Indicate if the enemy bot changes the player's focus
        /// </summary>
        bool _isEnemyBotFocusChanged;

        #endregion

        #region Properties

        public override FeatureModuleCategory GetFeatureModuleCategory() => FeatureModuleCategory.Focus;

        /// <summary>
        /// Returns focus change state
        /// </summary>
        public bool GetIsEnemyBotFocusChanged => _isEnemyBotFocusChanged;

        /// <summary>
        /// Returns the index of the enemy bot that the player has focus on
        /// </summary>
        public byte GetCurrentEnemyBotIndex => _currentEnnemyBotIndex;

        #endregion

        #region Methods

        public override void Initialize(FeatureManager pFeatureManager)
        {
            base.Initialize(pFeatureManager);

            _lastEnemyBotIndex = -1;
        }

        public override bool OnUpdate(bool pIsLeftFocus, bool pIsRightFocus, bool pIsGoodOffenseDirection)
        {
            if (!base.OnUpdate())
                return false;

            //Check the number of enemy bots
            if (featureManager.GetEnemyBotObject.Length == 0)
                return false;

            //Cancels verification if HitConfirm is enabled
            if (featureManager.GetHitConfirmModule.GetIsHitConfirmActivated)
                return true;

            //Manages the positioning of the EnemyBot
            for (int i = 0; i < featureManager.GetEnemyBotObject.Length; ++i)
            {
                if (!featureManager.GetEnemyBotObject[i])
                    continue;

                //Smooths the rotation so that it is fluid
                featureManager.GetEnemyBotObject[i].transform.rotation = Quaternion.Slerp(featureManager.GetEnemyBotObject[i].transform.rotation, Quaternion.LookRotation(featureManager.GetSturdyBotObject.transform.position - featureManager.GetEnemyBotObject[i].transform.position), 0.07f);
            }

            //Manages the positioning of the player. Checks if there is a EnemyBot on the battlefield
            if (featureManager.GetEnemyBotObject.Length > 1)
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
                    if (_currentEnnemyBotIndex < featureManager.GetEnemyBotObject.Length - 1)
                        ++_currentEnnemyBotIndex;
                }
            }

            //Assign the currentFocus transform based on assigned index
            _isEnemyBotFocusChanged = _lastEnemyBotIndex != _currentEnnemyBotIndex;

            if (_isEnemyBotFocusChanged)
                _lastEnemyBotIndex = _currentEnnemyBotIndex;

            //Manages a smooth rotation that allows the player to pivot quietly towards the right target
            featureManager.GetSturdyBotObject.transform.rotation = Quaternion.Slerp(featureManager.GetSturdyBotObject.transform.rotation, Quaternion.LookRotation(featureManager.GetCurrentEnemyBotObject.transform.position - featureManager.GetSturdyBotObject.transform.position), 0.07f);

            //Manages smooth rotation that allows the MonterBot to pivot quietly towards the player
            featureManager.GetSturdyBotObject.transform.position = Vector3.Lerp(featureManager.GetSturdyBotObject.transform.position, featureManager.GetCurrentEnemyBotObject.transform.position - featureManager.GetCurrentEnemyBotFocusRange, 0.5f);

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