using System;

using UnityEngine;

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
    /// Configuration file that keeps track of all offenses of a bot
    /// </summary>
    [CreateAssetMenu(fileName = "NewOffenseManager", menuName = "SturdyMachine/Offense/Manager", order = 52)]
    public class OffenseManager : ScriptableObject
    {
        #region Attributes

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

        float _currentDamage;

        System.Random _randomOffenseCategory;

        float _currentCooldownTime;

        #endregion

        #region Properties

        System.Random GetRandomOffenseCategory {

            get {
            
                if (_randomOffenseCategory == null)
                    _randomOffenseCategory = new System.Random();

                return _randomOffenseCategory;
            }
        }

        /// <summary>
        /// The current Offense of Bot is playing
        /// </summary>
        /// <returns>Return the current Offense</returns>
        public Offense GetCurrentOffense => _currentOffense;

        /// <summary>
        /// The next Offense of Bot is playing
        /// </summary>
        /// <returns>Return the next Offense</returns>
        public Offense GetNextOffense => _nextOffense;

        /// <summary>
        /// The last Offense of Bot is playing
        /// </summary>
        /// <returns>Return the last Offense is playing</returns>
        public Offense GetLastOffense => _lastOffense;

        public Offense GetOffense(OffenseType pOffenseCategoryType, OffenseDirection pOffenseDirection)
        {
            //StanceType
            if (GetIsStance(pOffenseCategoryType, pOffenseDirection))
                return GetOffense(_offenseStanceCategoryData, pOffenseCategoryType, pOffenseDirection);

            //Other Offense
            return GetOffense(_offenseCategoryData, pOffenseCategoryType, pOffenseDirection);
        }

        public Offense GetOffense(OffenseCategoryData[] pOffenseCategoryData, OffenseType pOffenseType, OffenseDirection pOffenseDirection)
        {
            for (int i = 0; i < pOffenseCategoryData.Length; ++i)
            {
                //Continue the iteration if the type of the category assigned as a parameter is not equal to that of the category
                if (!GetIsGoodOffenseCategoryType(pOffenseCategoryData[i], pOffenseType, pOffenseDirection))
                    continue;

                int currentOffenseCategoryIndex = GetOffenseCategoryIndex(pOffenseCategoryData[i].offenseCategory, pOffenseType, pOffenseDirection);

                if (pOffenseCategoryData[i].offenseCategory[currentOffenseCategoryIndex].GetOffenseCategoryDirection != OffenseDirection.DEFAULT)
                {

                    if (pOffenseCategoryData[i].offenseCategory[currentOffenseCategoryIndex].GetOffenseCategoryDirection != pOffenseDirection)
                        continue;
                }

                return GetOffense(pOffenseCategoryData[i].offenseCategory[currentOffenseCategoryIndex].GetOffense, pOffenseType, pOffenseDirection);
            }

            return null;
        }

        Offense GetOffense(Offense[] pOffense, OffenseType pOffenseType, OffenseDirection pOffenseCategoryDirection)
        {
            for (int i = 0; i < pOffense.Length; ++i)
            {
                //Continue the iteration if the type of Offense desired as a parameter is not equal to the type of Offense
                if (pOffense[i].GetOffenseType != pOffenseType)
                    continue;

                //Continue the iteration if the direction of Offense desired as a parameter is not equal to the direction of Offense
                if (pOffense[i].GetOffenseDirection != pOffenseCategoryDirection)
                    continue;

                return pOffense[i];
            }

            return null;
        }

        public float GetCurrentMaxCooldownTime => GetCurrentOffense.GetCurrentCooldown(GetCurrentOffense.GetAnimationClip(AnimationClipOffenseType.Full).name);

        Offense GetOffenseWithOffenseName(OffenseCategoryData[] pOffenseCategoryData, string pOffenseName)
        {
            for (byte i = 0; i < pOffenseCategoryData.Length; ++i)
            {
                for (byte j = 0; j < pOffenseCategoryData[i].offenseCategory.Length; ++j)
                {
                    for (byte k = 0; k < pOffenseCategoryData[i].offenseCategory[j].GetOffense.Length; ++k)
                    {
                        if (pOffenseCategoryData[i].offenseCategory[j].GetOffense[k].name != pOffenseName)
                            continue;

                        return pOffenseCategoryData[i].offenseCategory[j].GetOffense[k];
                    }
                }
            }

            return null;
        }

        Offense GetOffense(OffenseCategoryData[] pOffenseCategoryData, string pAnimationClipName)
        {
            AnimationClip offenseAnimationClip = null;

            for (byte i = 0; i < pOffenseCategoryData.Length; ++i)
            {
                for (byte j = 0; j < pOffenseCategoryData[i].offenseCategory.Length; ++j)
                {
                    for (byte k = 0; k < pOffenseCategoryData[i].offenseCategory[j].GetOffense.Length; ++k)
                    {
                        offenseAnimationClip = pOffenseCategoryData[i].offenseCategory[j].GetOffense[k].GetAnimationClip(pAnimationClipName);

                        if (!offenseAnimationClip)
                            continue;

                        //Full
                        if (offenseAnimationClip == pOffenseCategoryData[i].offenseCategory[j].GetOffense[k].GetAnimationClip(AnimationClipOffenseType.Full))
                            return pOffenseCategoryData[i].offenseCategory[j].GetOffense[k];

                        //Parry
                        if (offenseAnimationClip == pOffenseCategoryData[i].offenseCategory[j].GetOffense[k].GetAnimationClip(AnimationClipOffenseType.Parry))
                            return pOffenseCategoryData[i].offenseCategory[j].GetOffense[k];

                        //Stagger
                        if (pOffenseCategoryData[i].offenseCategory[j].GetOffense[k].GetAnimationClip(AnimationClipOffenseType.Stagger))
                        {
                            if (offenseAnimationClip == pOffenseCategoryData[i].offenseCategory[j].GetOffense[k].GetAnimationClip(AnimationClipOffenseType.Stagger))
                                return pOffenseCategoryData[i].offenseCategory[j].GetOffense[k];
                        }

                        //KeyposeOut
                        if (pOffenseCategoryData[i].offenseCategory[j].GetOffense[k].GetAnimationClip(AnimationClipOffenseType.KeyposeOut))
                        {
                            if (offenseAnimationClip == pOffenseCategoryData[i].offenseCategory[j].GetOffense[k].GetAnimationClip(AnimationClipOffenseType.KeyposeOut))
                                return pOffenseCategoryData[i].offenseCategory[j].GetOffense[k];
                        }
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// Indicates if the Current Offense is already assigned
        /// </summary>
        /// <param name="pAnimationClip">The AnimationClip you need check</param>
        /// <returns>Returns if the Bot's Current Offense needs to be assigned</returns>
        public bool GetIsCurrentOffenseAlreadyAssigned(AnimationClip pAnimationClip) => _currentOffense.GetAnimationClip(pAnimationClip.name) == pAnimationClip;

        public bool GetIsCurrentOffenseAlreadyAssigned(AnimationClip pAnimationClip, OffenseType pOffenseType, OffenseDirection pOffenseDirection, AnimationClipOffenseType pAnimationClipOffenseType) {

            if (_currentOffense.GetOffenseType != pOffenseType)
                return false;

            if (_currentOffense.GetOffenseDirection != pOffenseDirection)
                return false;

            return _currentOffense.GetAnimationClip(pAnimationClipOffenseType) == pAnimationClip;
        }

        public bool GetIsNextOffenseAreStrikeType(float pNormalizedTime) 
        {
            if (!_lastOffense)
                return false;

            if (_lastOffense.GetOffenseDirection != OffenseDirection.STANCE)
                return false;

            if (_currentOffense.GetOffenseDirection == OffenseDirection.STANCE)
                return false;

            if (pNormalizedTime < 0.80f)
                return false;

            if (_currentOffense.GetOffenseType != _nextOffense.GetOffenseType)
                return false;

            return _nextOffense.GetOffenseType == OffenseType.STRIKE;
        }

        /// <summary>
        /// Check if the bot can change animation
        /// </summary>
        /// <returns>Returns the state if the bot can change animation</returns>
        public bool GetIsNeedApplyNextOffense() 
        {
            if (!_currentOffense)
                return false;

            if (!_nextOffense)
                return false;

            return _nextOffense != _currentOffense;
        }

        int GetRandomCategoryOffense(int pOffenseCategorySize) => 100 / pOffenseCategorySize;

        int GetOffenseCategoryIndex(OffenseCategory[] pOffenseCategory, OffenseType pOffenseType, OffenseDirection pOffenseDirection) {

            //Randomize offense index
            if (!GetIsStance(pOffenseType, pOffenseDirection)) 
            {
                if (pOffenseCategory.Length > 1)
                {
                    int currentRandomValueEveryOffenseCategory = GetRandomCategoryOffense(pOffenseCategory.Length);

                    int randomValueOffenseCategory = GetRandomOffenseCategory.Next(0, 100);

                    for (byte i = 0; i < pOffenseCategory.Length; ++i)
                    {
                        if (randomValueOffenseCategory > currentRandomValueEveryOffenseCategory * (i + 1))
                            continue;

                        return i;
                    }

                }

                return 0;
            }

            for (byte i = 0; i < pOffenseCategory.Length; ++i) 
            {
                for (byte j = 0; j < pOffenseCategory[i].GetOffense.Length; ++j) 
                {
                    if (pOffenseCategory[i].GetOffense[j].GetOffenseType != pOffenseType)
                        continue;

                    if (pOffenseCategory[i].GetOffense[j].GetOffenseDirection != pOffenseDirection)
                        continue;

                    return i;
                }
            }

            return 0;
        }
        

        /// <summary>
        /// Allows you to check information regarding category types based on the type of Offense
        /// </summary>
        /// <param name="pOffenseCategoryData">Category of Offense that needs to be checked</param>
        /// <param name="pOffenseType">Type of Offense</param>
        /// <param name="pOffenseCategoryDirection">Category direction</param>
        /// <returns>Returns if the category corresponds to the information in parameter</returns>
        bool GetIsGoodOffenseCategoryType(OffenseCategoryData pOffenseCategoryData, OffenseType pOffenseType, OffenseDirection pOffenseCategoryDirection) {

            if (pOffenseCategoryData.offenseCategoryType == OffenseType.STANCE)
                return $"{pOffenseCategoryData.offenseCategoryType}" == $"{pOffenseCategoryDirection}";

            return pOffenseCategoryData.offenseCategoryType == pOffenseType;
        }

        bool GetIsGoodOffenseCategoryType(OffenseType pOffenseCategoryType, OffenseType pOffenseType, OffenseDirection pOffenseDirection)
        {
            if (pOffenseCategoryType == OffenseType.STANCE)
                return $"{pOffenseCategoryType}" == $"{pOffenseDirection}";

            return pOffenseCategoryType == pOffenseType;
        }

        public bool GetIsCooldownActivated(CooldownType pCurrentCooldownType) {

            if (GetIsStance(_lastOffense))
                return false;

            if (GetIsStance(_currentOffense))
                return false;

            _currentCooldownTime += Time.deltaTime;

            if (_currentCooldownTime >= GetCurrentMaxCooldownTime * GetCurrentCooldownMultiplicator(pCurrentCooldownType)) {

                _currentCooldownTime = 0;

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

        public bool GetIsStance(OffenseType pOffenseType, OffenseDirection pOffenseDirection) 
        {
            if (pOffenseType == OffenseType.DEFAULT)
                return pOffenseDirection == OffenseDirection.STANCE;

            return pOffenseDirection == OffenseDirection.STANCE;
        }

        public float GetCurrentCooldownMultiplicator(CooldownType pCurrentCooldownType) {

            Debug.Log(pCurrentCooldownType);

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

        public void AssignCurrentOffense(string pAnimationClipName) 
        {
            if (_currentOffense)
            {
                AnimationClip clip = _currentOffense.GetAnimationClip(pAnimationClipName);

                if (clip)
                {

                    if (clip.name == pAnimationClipName)
                        return;
                }
            }

            _currentOffense = GetOffense(_offenseCategoryData, pAnimationClipName);

            if (!_currentOffense)
                _currentOffense = GetOffense(_offenseStanceCategoryData, pAnimationClipName);
        }

        public void AssignNextOffense(OffenseType pOffenseType, OffenseDirection pOffenseDirection) 
        {
            if (_nextOffense == GetOffense(pOffenseType, pOffenseDirection))
                return;

            _nextOffense = GetOffense(pOffenseType, pOffenseDirection);
        }

        public void OnEnable()
        {
            _currentOffense = GetOffense(_offenseStanceCategoryData, OffenseType.DEFAULT, OffenseDirection.STANCE);
        }

        public void OnDisable()
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

#endif
    }
}