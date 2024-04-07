﻿using System;

using UnityEngine;
using System.Runtime.Remoting.Messaging;


#if UNITY_EDITOR
using UnityEditor;
using NWH.NUI;
using NWH.VehiclePhysics2;
#endif

namespace SturdyMachine.Offense 
{
    public enum CooldownType { NEUTRAL, ADVANTAGE, DISADVANTAGE }

    /// <summary>
    /// Store information about OffenseCategory type
    /// </summary>
    [Serializable, Tooltip("Store information about OffenseCategory type")]
    public struct OffenseCategoryData {

        /// <summary>
        /// The type of the category
        /// </summary>
        [Tooltip("The type of the category")]
        public OffenseType offenseCategoryType;

        /// <summary>
        /// The Offense category list matching the category type
        /// </summary>
        [Tooltip("The Offense category list matching the category type")]
        public OffenseCategory[] offenseCategory;
    }

    /// <summary>
    /// Represents Cooldown information based on the current Offense
    /// </summary>
    [Serializable, Tooltip("Represents Cooldown information based on the current Offense")]
    public struct CooldownData {

        /// <summary>
        /// Indicates if the Offense executes a Cooldown
        /// </summary>
        [Tooltip("Indicates if the Offense executes a Cooldown")]
        public bool isActivated;

        /// <summary>
        /// Represents the current cooldown wait time of the Offense currently playing
        /// </summary>
        [SerializeField, Tooltip("Represents the current cooldown wait time of the Offense currently playing")]
        public float currentCooldownTime;

        /// <summary>
        /// Represents the maximum Cooldown time before being able to play another Offense
        /// </summary>
        [SerializeField, Tooltip("Represents the maximum Cooldown time before being able to play another Offense")]
        public float currentMaxCooldownTime;
    }

    /// <summary>
    /// Configuration file that keeps track of all offenses of a bot
    /// </summary>
    [CreateAssetMenu(fileName = "NewOffenseManager", menuName = "SturdyMachine/Offense/Manager", order = 52)]
    public class OffenseManager : ScriptableObject
    {
        #region Attribut

        /// <summary>
        /// List representing all categories of Offenses
        /// </summary>
        [SerializeField, Tooltip("List representing all categories of Offenses")]
        OffenseCategoryData[] _offenseCategoryData;

        /// <summary>
        /// List of Stance categories
        /// </summary>
        [SerializeField, Tooltip("List of Stance categories")]
        OffenseCategoryData[] _offenseStanceCategoryData;

        /// <summary>
        /// Designates the Offense the bot is currently playing
        /// </summary>
        [SerializeField, Tooltip("Designates the Offense the bot is currently playing")]
        Offense _currentOffense;

        /// <summary>
        /// Designates the next Offense that should be played based on the type and direction requested
        /// </summary>
        [SerializeField, Tooltip("Designates the next Offense that should be played based on the type and direction requested")]
        Offense _nextOffense;

        /// <summary>
        /// Designates the last Offense that the bot played
        /// </summary>
        [SerializeField, Tooltip("Designates the last Offense that the bot played")]
        Offense _lastOffense;

        /// <summary>
        /// Indicates information regarding the Cooldown of the present Offense
        /// </summary>
        [SerializeField, Tooltip("Indicates information regarding the Cooldown of the present Offense")]
        CooldownData _currentCooldownData;

        #endregion

        #region Get

        /// <summary>
        /// Allows you to return the correct category depending on the type of category chosen as a parameter
        /// </summary>
        /// <param name="pOffenseCategoryType">The type of category you need</param>
        /// <returns>Returns the correct Offense category</returns>
        public OffenseCategoryData GetSpecificOffenseCategoryData(OffenseType pOffenseCategoryType) {

            for (int i = 0; i < _offenseCategoryData.Length; ++i) {

                if (_offenseCategoryData[i].offenseCategoryType != pOffenseCategoryType)
                    continue;

                return _offenseCategoryData[i];
            }

            return new OffenseCategoryData();
        }

        /// <summary>
        /// The current Offense of Bot is playing
        /// </summary>
        /// <returns>Return the current Offense</returns>
        public Offense GetCurrentOffense() => _currentOffense;

        /// <summary>
        /// The next Offense of Bot is playing
        /// </summary>
        /// <returns>Return the next Offense</returns>
        public Offense GetNextOffense() => _nextOffense;

        /// <summary>
        /// The last Offense of Bot is playing
        /// </summary>
        /// <returns>Return the last Offense is playing</returns>
        public Offense GetLastOffense => _lastOffense;

        /// <summary>
        /// Allows you to find the right Offense depending on the type and direction set in parameter
        /// </summary>
        /// <param name="pOffenseType">The type of Offense you want</param>
        /// <param name="pOffenseDirection">The direction of Offense you want</param>
        /// <returns></returns>
        public Offense GetOffense(OffenseType pOffenseType, OffenseDirection pOffenseDirection) {

            //Iterates through all Offense categories that have been configured in this ScriptableObject
            for (int i = 0; i < _offenseCategoryData.Length; ++i) {

                for (int j = 0; j < _offenseCategoryData[i].offenseCategory.Length; ++j) {

                    //Iterates all Offenses that have been configured in this Offense category
                    for (int k = 0; k < _offenseCategoryData[i].offenseCategory[j].GetOffense.Length; ++k) {

                        //Checks if the type of Offense present matches the one desired in the parameter
                        if (_offenseCategoryData[i].offenseCategory[j].GetOffense[k].GetOffenseType != pOffenseType)
                            continue;

                        //Checks if the direction of Offense present matches the one desired in the parameter
                        if (_offenseCategoryData[i].offenseCategory[j].GetOffense[k].GetOffenseDirection != pOffenseDirection)
                            continue;

                        //Returns the correct Offense which matches the two pieces of information assigned as a parameter
                        return _offenseCategoryData[i].offenseCategory[j].GetOffense[k];
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// Indicates if the Current Offense needs to be assigned
        /// </summary>
        /// <param name="pBotAnimator">The Bot Animator</param>
        /// <returns>Returns if the Bot's Current Offense needs to be assigned</returns>
        public bool GetCurrentOffenseAssigned(Animator pBotAnimator) {

            //If the CurrentOffense is null
            if (!_currentOffense)
                return false;

            //If the name of the clip in the Bot animator matches one of the two clips in the current Offense
            if (_currentOffense.GetAnimationClip(pBotAnimator.GetCurrentAnimatorClipInfo(0)[0].clip.name) == pBotAnimator.GetCurrentAnimatorClipInfo(0)[0].clip)
                return true;
            
            return false;
        }

        /// <summary>
        /// Indicates if the NextOffense is already assigned on CurrentOffense
        /// </summary>
        public bool GetNextOffenseAssigned => _currentOffense == _nextOffense;

        /// <summary>
        /// Check if the bot can change animation
        /// </summary>
        /// <returns>Returns the state if the bot can change animation</returns>
        public bool GetIsApplyNextOffense() {

            if (!_nextOffense)
                return false;

            if (!_currentOffense)
                return false;

            return _nextOffense != _currentOffense;
        }

        /// <summary>
        /// Allows verification of the name of a clip with all AnimationClips of an Offense
        /// </summary>
        /// <param name="pOffense">The Offense to check</param>
        /// <param name="pAnimationClipName">The name of the AnimationClip</param>
        /// <returns></returns>
        bool GetIsOffenseWithName(Offense pOffense, string pAnimationClipName) {

            //Complete
            if (pOffense.GetAnimationClip().name == pAnimationClipName)
                return true;

            //Parry
            if (pOffense.GetParryAnimationClip) {

                if (pOffense.GetParryAnimationClip.name == pAnimationClipName)
                    return true;
            }

            //KeyposeOut
            AnimationClip keyposeOutClip = pOffense.GetAnimationClip(true);

            if (!keyposeOutClip)
                return false;

            return pOffense.GetAnimationClip(true).name == pAnimationClipName;
        }

        /// <summary>
        /// Allows the assignment of CurrentOffense with a specific OffenseCategoryData array
        /// </summary>
        /// <param name="pOffenseCategoryData">The specific OffenseCategoryData array</param>
        /// <param name="pAnimationClipName">The name of the AnimationClip</param>
        /// <returns>Returns if the Current Offense was assigned correctly</returns>
        bool CurrentOffenseSpecificSetup(OffenseCategoryData[] pOffenseCategoryData, string pAnimationClipName)
        {
            for (int i = 0; i < pOffenseCategoryData.Length; ++i)
            {

                for (int j = 0; j < pOffenseCategoryData[i].offenseCategory.Length; ++j) {

                    for (int k = 0; k < pOffenseCategoryData[i].offenseCategory[j].GetOffense.Length; ++k) {

                        //Continue iteration if the name of the clip in the Animator does not match that of the present Offense
                        if (!GetIsOffenseWithName(pOffenseCategoryData[i].offenseCategory[j].GetOffense[k], pAnimationClipName))
                            continue;

                        //Assigns the last Offense if the current Offense is not null
                        if (_currentOffense)
                            _lastOffense = _currentOffense;

                        _currentOffense = pOffenseCategoryData[i].offenseCategory[j].GetOffense[k];

                        return true;
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// Allows the assignment of NextOffense with a specific OffenseCategoryData array
        /// </summary>
        /// <param name="pOffenseCategoryData">The specific OffenseCategoryData array</param>
        /// <param name="pOffenseCategoryType">The type of category</param>
        /// <param name="pOffenseDirection">The direction of category</param>
        /// <returns>Returns if the NextOffense was assigned correctly</returns>
        bool NextOffenseSpecificSetup(OffenseCategoryData[] pOffenseCategoryData, OffenseType pOffenseType, OffenseDirection pOffenseCategoryDirection)
        {
            for (int i = 0; i < pOffenseCategoryData.Length; ++i)
            {
                //Continue the iteration if the type of the category assigned as a parameter is not equal to that of the category
                if (!GetIsGoodCategory(pOffenseCategoryData[i], pOffenseType, pOffenseCategoryDirection))
                    continue;

                for (int j = 0; j < pOffenseCategoryData[i].offenseCategory.Length; ++j) {

                    if (pOffenseCategoryData[i].offenseCategory[j].GetOffenseCategoryDirection != OffenseDirection.DEFAULT) {

                        if (pOffenseCategoryData[i].offenseCategory[j].GetOffenseCategoryDirection != pOffenseCategoryDirection)
                            continue;
                    }

                    for (int k = 0; k < pOffenseCategoryData[i].offenseCategory[j].GetOffense.Length; ++k) {

                        //Continue the iteration if the type of Offense desired as a parameter is not equal to the type of Offense
                        if (pOffenseCategoryData[i].offenseCategory[j].GetOffense[k].GetOffenseType != pOffenseType)
                            continue;

                        //Continue the iteration if the direction of Offense desired as a parameter is not equal to the direction of Offense
                        if (pOffenseCategoryData[i].offenseCategory[j].GetOffense[k].GetOffenseDirection != pOffenseCategoryDirection)
                            continue;

                        _nextOffense = pOffenseCategoryData[i].offenseCategory[j].GetOffense[k];

                        if (!GetIsStance(_nextOffense)) {

                            _currentCooldownData.isActivated = true;

                            _currentCooldownData.currentMaxCooldownTime = _nextOffense.GetCurrentCooldown(_nextOffense.GetAnimationClip().name);
                        }

                        return true;
                    }
                }
            }

            return false;

        }

        /// <summary>
        /// Allows you to check information regarding category types based on the type of Offense
        /// </summary>
        /// <param name="pOffenseCategoryData">Category of Offense that needs to be checked</param>
        /// <param name="pOffenseType">Type of Offense</param>
        /// <param name="pOffenseCategoryDirection">Category direction</param>
        /// <returns>Returns if the category corresponds to the information in parameter</returns>
        bool GetIsGoodCategory(OffenseCategoryData pOffenseCategoryData, OffenseType pOffenseType, OffenseDirection pOffenseCategoryDirection) {

            if (pOffenseCategoryData.offenseCategoryType == OffenseType.STANCE)
            {

                if ($"{pOffenseCategoryData.offenseCategoryType}" != $"{pOffenseCategoryDirection}")
                    return false;
            }

            else if (pOffenseCategoryData.offenseCategoryType != pOffenseType)
                return false;

            return true;
        }

        public bool GetIsCooldownActivated(CooldownType pCurrentCooldownType) {

            if (GetIsStance(_currentOffense))
                return false;

            if (!_currentCooldownData.isActivated)
                return false;

            _currentCooldownData.currentCooldownTime += Time.deltaTime;

            if (_currentCooldownData.currentCooldownTime >= _currentCooldownData.currentMaxCooldownTime * GetCurrentCooldownMultiplicator(pCurrentCooldownType)) {

                _currentCooldownData.isActivated = false;

                _currentCooldownData.currentCooldownTime = 0;
                _currentCooldownData.currentMaxCooldownTime = 0;

                return false;
            }

            return true;
            
        }

        public bool GetIsStance(Offense pOffense) {

            if (!pOffense)
                return false;

            if (pOffense.GetOffenseDirection == OffenseDirection.STANCE)
                return true;

            return pOffense.GetOffenseType == OffenseType.STANCE;
        }

        public float GetCurrentCooldownMultiplicator(CooldownType pCurrentCooldownType) {

            //Disadvantage
            if (pCurrentCooldownType == CooldownType.DISADVANTAGE)
                return 1.25f;

            //Advantage
            if (pCurrentCooldownType == CooldownType.ADVANTAGE)
                return 0.75f;

            //Neutral
            return 1f;
        }

        #endregion

        #region Method

        /// <summary>
        /// Manages the Current Offense assignment
        /// </summary>
        /// <param name="pAnimationClipName">The name of the AnimationClip</param>
        public void CurrentOffenseSetup(string pAnimationClipName)
        {

            if (_currentOffense)
            {
                AnimationClip clip = _currentOffense.GetAnimationClip(pAnimationClipName);

                if (clip) {

                    if (clip.name == pAnimationClipName)
                        return;
                }
            }

            if (CurrentOffenseSpecificSetup(_offenseCategoryData, pAnimationClipName))
                return;

            CurrentOffenseSpecificSetup(_offenseStanceCategoryData, pAnimationClipName);
        }

        /// <summary>
        /// Manages the Next Offense assignment
        /// </summary>
        /// <param name="pOffenseCategoryType">The type of Offense category</param>
        /// <param name="pOffenseDirection">The direction of the desired Offense</param>
        /// <returns>Returns the correct Offense according to the type of category and the direction of the Offense assigned as a parameter</returns>
        public void NextOffenseSetup(OffenseType pOffenseCategoryType, OffenseDirection pOffenseDirection)
        {

            //Other Offense
            if (NextOffenseSpecificSetup(_offenseCategoryData, pOffenseCategoryType, pOffenseDirection))
                return;

            //Stance Offense
            NextOffenseSpecificSetup(_offenseStanceCategoryData, pOffenseCategoryType, pOffenseDirection);
        }

        void OnDisable()
        {
            _lastOffense = null;
            _currentOffense = null;
            _nextOffense = null;
        }

        #endregion


#if UNITY_EDITOR

        [CustomEditor(typeof(OffenseManager))]
        public class OffenseManagerEditor : NUIEditor
        {
            public override bool OnInspectorNUI()
            {
                if (!base.OnInspectorNUI())
                    return false;

                EditorGUI.BeginChangeCheck();

                if (Application.isPlaying)
                    DrawDebugValue();

                drawer.ReorderableList("_offenseCategoryData");
                drawer.ReorderableList("_offenseStanceCategoryData");

                drawer.EndEditor(this);
                return true;
            }

            /// <summary>
            /// Show all necessary information in the editor
            /// </summary>
            void DrawDebugValue() {

                drawer.BeginSubsection("Debug Value");

                //Offense
                DrawOffenseDebug();

                //Cooldown
                drawer.Property("_currentCooldownData");

                drawer.EndSubsection();
            }

            /// <summary>
            /// Show all necessary Offenses information in the editor
            /// </summary>
            void DrawOffenseDebug() {

                drawer.BeginSubsection("Offense");

                drawer.Field("_lastOffense", false, null, "Last: ");
                drawer.Field("_currentOffense", false, null, "Current: ");
                drawer.Field("_nextOffense", false, null, "Next: ");

                drawer.EndSubsection();
            }
        }

        [CustomPropertyDrawer(typeof(OffenseCategoryData))]
        public partial class OffenseCategoryDataDrawer : ComponentNUIPropertyDrawer
        {
            public override bool OnNUI(Rect position, SerializedProperty property, GUIContent label)
            {
                if (!base.OnNUI(position, property, label))
                    return false;

                if (drawer.Field("offenseCategoryType", true, null, "Type of this category: ").enumValueIndex != 0)
                    drawer.ReorderableList("offenseCategory");

                drawer.EndProperty();
                return true;
            }
        }

        [CustomPropertyDrawer(typeof(CooldownData))]
        public partial class CooldownDataDrawer : ComponentNUIPropertyDrawer
        {
            public override bool OnNUI(Rect position, SerializedProperty property, GUIContent label)
            {
                if (!base.OnNUI(position, property, label))
                    return false;

                if (drawer.Field("isActivated", false).boolValue) {

                    drawer.Field("currentCooldownTime", false, "sec", "Current: ");
                    drawer.Field("currentMaxCooldownTime", false, "sec", "Max: ");
                }

                drawer.EndProperty();
                return true;
            }
        }

#endif
    }
}