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

        #endregion

        #region Get

        public override FeatureModuleCategory GetFeatureModuleCategory()
        {
            return FeatureModuleCategory.Focus;
        }

        #endregion

        #region Method

        public override void Initialize(ref FeatureCacheData pFeatureCacheData)
        {
            base.Initialize();

            pFeatureCacheData.focusDataCache = new FocusDataCache();
        }

        public override bool OnUpdate(bool pIsLeftFocus, bool pIsRightFocus, OffenseBlockingConfig pOffenseBlockingConfig, ref FeatureCacheData pFeatureCacheData)
        {
            if (!base.OnUpdate())
                return false;

            if (pFeatureCacheData.hitConfirmDataCache.isInHitConfirm)
                return true;

            LookSetup(pIsLeftFocus, pIsRightFocus, ref pFeatureCacheData);

            return true;
        }

        /// <summary>
        /// Assigns current Focus as well as player positioning and the MonsterBot that wants to battle so that it looks at itself
        /// </summary>
        void LookSetup(bool pIsLeftFocus, bool pIsRightFocus, ref FeatureCacheData pFeatureCacheData) 
        {
            if (pFeatureCacheData.ennemyBotDataCache.Length == 0)
                return;

            //Manages the positioning of the MonsterBot
            EnnemyBotLook(ref pFeatureCacheData);

            //Manages the positioning of the player
            SturdyBotLook(ref pFeatureCacheData, pIsLeftFocus, pIsRightFocus);
        }

        /// <summary>
        /// Manages the speed of rotation so that there is fluidity in its movement
        /// </summary>
        void EnnemyBotLook(ref FeatureCacheData pFeatureCacheData) 
        {
            for (int i = 0; i < pFeatureCacheData.ennemyBotDataCache.Length; ++i)
            {
                if (pFeatureCacheData.ennemyBotDataCache[i].botObject != null) 
                {
                    //Smooths the rotation so that it is fluid
                    if (pFeatureCacheData.ennemyBotDataCache[i].botObject.transform.rotation != Quaternion.Slerp(pFeatureCacheData.ennemyBotDataCache[i].botObject.transform.rotation, Quaternion.LookRotation(pFeatureCacheData.sturdyBotDataCache.botObject.transform.position - pFeatureCacheData.ennemyBotDataCache[i].botObject.transform.position), 0.07f))
                        pFeatureCacheData.ennemyBotDataCache[i].botObject.transform.rotation = Quaternion.Slerp(pFeatureCacheData.ennemyBotDataCache[i].botObject.transform.rotation, Quaternion.LookRotation(pFeatureCacheData.sturdyBotDataCache.botObject.transform.position - pFeatureCacheData.ennemyBotDataCache[i].botObject.transform.position), 0.07f);
                }
            }
        }

        /// <summary>
        /// Manage the axis of rotation according to the input of the player and assign the correct value to CurrentMonsterBotIndex
        /// </summary>
        void SturdyBotLook(ref FeatureCacheData pFeatureCacheData, bool pIsLeftFocus, bool pIsRightFocus) 
        {
            //Checks if there is a MosnterBot on the battlefield
            if (pFeatureCacheData.ennemyBotDataCache.Length > 1)
            {
                //Checks if the player wants to look left
                if (pIsLeftFocus)
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
                else if (pIsRightFocus)
                {
                    //Checks if the player is not already looking to the right
                    if (!_lastLookRightState)
                    {
                        //Assigns the correct index of the MonsterBot the player wants to watch
                        if (_currentEnnemyBotIndex < pFeatureCacheData.ennemyBotDataCache.Length - 1)
                            ++_currentEnnemyBotIndex;

                        _lastLookRightState = true;
                    }
                }
                else if (_lastLookRightState)
                    _lastLookRightState = false;
            }

            //Assign the currentFocus transform based on assigned index
            if (pFeatureCacheData.focusDataCache.currentEnnemyBotFocus != pFeatureCacheData.ennemyBotDataCache[_currentEnnemyBotIndex].botObject) {

                pFeatureCacheData.focusDataCache.ifEnnemyBotFocusChanged = true;

                pFeatureCacheData.focusDataCache.currentEnnemyBotFocus = pFeatureCacheData.ennemyBotDataCache[_currentEnnemyBotIndex].botObject;
            }

            //Manages a smooth rotation that allows the player to pivot quietly towards the right target
            pFeatureCacheData.sturdyBotDataCache.botObject.transform.rotation = Quaternion.Slerp(pFeatureCacheData.sturdyBotDataCache.botObject.transform.rotation, Quaternion.LookRotation(pFeatureCacheData.ennemyBotDataCache[_currentEnnemyBotIndex].botObject.transform.position - pFeatureCacheData.sturdyBotDataCache.botObject.transform.position), 0.07f);

            //Manages smooth rotation that allows the MonterBot to pivot quietly towards the player
            pFeatureCacheData.sturdyBotDataCache.botObject.transform.position = Vector3.Lerp(pFeatureCacheData.sturdyBotDataCache.botObject.transform.position, pFeatureCacheData.ennemyBotDataCache[_currentEnnemyBotIndex].botObject.transform.position - pFeatureCacheData.ennemyBotDataCache[_currentEnnemyBotIndex].focusRange, 0.5f);
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