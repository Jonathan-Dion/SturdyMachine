using System;

using UnityEngine;
using SturdyMachine.Offense;

#if UNITY_EDITOR
using UnityEditor;
using NWH.VehiclePhysics2;
#endif

namespace SturdyMachine.Features.Fight 
{
    /// <summary>
    /// All types of combat modes possible
    /// </summary>
    public enum FightingModeType { Default, Passive, Agressif}

    /// <summary>
    /// Allows you to configure the movements that the bot will be able to make during these combat phases
    /// </summary>
    [Serializable, Tooltip("Allows you to configure the movements that the bot will be able to make during these combat phases")]
    public struct FightModeData {

        /// <summary>
        /// GameObject of the enemy bot that we want to configure
        /// </summary>
        [Tooltip("GameObject of the enemy bot that we want to configure")]
        public GameObject ennemyBot;

        /// <summary>
        /// Allows the configuration of all combos relating to this bot
        /// </summary>
        [Tooltip("Allows the configuration of all combos relating to this bot")]
        public FightComboSequenceData[] fightComboSequenceData;
    }

    /// <summary>
    /// Allows configuration of combos
    /// </summary>
    [Serializable, Tooltip("Allows configuration of combos")]
    public struct FightComboSequenceData {

        /// <summary>
        /// Designates the combat mode that this combo
        /// </summary>
        [Tooltip("Designates the combat mode that this combo")]
        public FightingModeType fightingModeType;

        /// <summary>
        /// The list of information concerning all the Offenses of this combo
        /// </summary>
        [Tooltip("The list of information concerning all the Offenses of this combo")]
        public FightOffenseData[] fightOffenseData;

        /// <summary>
        /// Indicates if the combo was executed completely
        /// </summary>
        [Tooltip("Indicates if the combo was executed completely")]
        public bool isCompleted;
    }

    /// <summary>
    /// Allows the configuration of an Offense
    /// </summary>
    [Serializable, Tooltip("Allows the configuration of an Offense")]
    public struct FightOffenseData {

        /// <summary>
        /// The Offense you want the bot to execute
        /// </summary>
        [Tooltip("The Offense you want the bot to execute")]
        public Offense.Offense offense;

        /// <summary>
        /// The waiting time when the Offense is of the Offense type
        /// </summary>
        [Tooltip("The waiting time when the Offense is of the Offense type")]
        public float waithingTime;

        /// <summary>
        /// The waiting time before executing the next Offense
        /// </summary>
        [Tooltip("The waiting time before executing the next Offense")]
        public float cooldownTime;

        /// <summary>
        /// Allows you to indicate whether this Offense was executed
        /// </summary>
        [Tooltip("Allows you to indicate whether this Offense was executed")]
        public bool isCompleted;
    }

    /// <summary>
    /// Module managing the fight sequence as well as the combat behavior of a MonsterBot
    /// </summary>
    [Serializable]
    public partial class FightModule : FeatureModule
    {
        #region Attribut

        /// <summary>
        /// Store the configuration of all combos of all EnnemyBot
        /// </summary>
        [SerializeField, Tooltip("Store the configuration of all combos of all EnnemyBot")]
        FightModeData[] _fightModeData;

        /// <summary>
        /// The index which represents the bot which should play its combat sequences
        /// </summary>
        [SerializeField, Tooltip("The index which represents the bot which should play its combat sequences")]
        int _currentFightModeDataIndex = -1;

        /// <summary>
        /// The index of the previous bot that was in the combat sequence
        /// </summary>
        [Tooltip("The index of the previous bot that was in the combat sequence")]
        int _lastFightModeDataIndex;

        /// <summary>
        /// The index that represents the combo the bot is playing
        /// </summary>
        [SerializeField, Tooltip("The index that represents the combo the bot is playing")]
        int _currentFightComboSequenceDataIndex;

        /// <summary>
        /// The index that represents the Offense that is currently playing in the combo list
        /// </summary>
        [SerializeField, Tooltip("The index that represents the Offense that is currently playing in the combo list")]
        int _currentFightOffenseDataIndex;

        /// <summary>
        /// The maximum loading time of an Offense (Stance Loading)
        /// </summary>
        [SerializeField, Tooltip("The maximum loading time of an Offense (Stance Loading)")]
        float _currentMaxWaithingTime;

        /// <summary>
        /// The remaining time for the Offense to load (Stance Loading)
        /// </summary>
        [SerializeField, Tooltip("The remaining time for the Offense to load (Stance Loading)")]
        float _currentWainthingTime;

        /// <summary>
        /// The maximum time the drinker must take before executing his next Offense in his combo
        /// </summary>
        [SerializeField, Tooltip("The maximum time the drinker must take before executing his next Offense in his combo")]
        float _currentMaxcooldownTime;

        /// <summary>
        /// The time remaining before the bot executes its next Offense in its combo
        /// </summary>
        [SerializeField, Tooltip("The time remaining before the bot executes its next Offense in its combo")]
        float _currentcooldownTime;

        /// <summary>
        /// The GameObject of the bot that is in the player's Focus
        /// </summary>
        [SerializeField, Tooltip("The GameObject of the bot that is in the player's Focus")]
        GameObject _currentEnnemyFightGameObject;

        /// <summary>
        /// Indicates whether the check for combo indexes should be run
        /// </summary>
        [SerializeField, Tooltip("Indicates whether the check for combo indexes should be run")]
        bool _isComboTcheck;
        
        #endregion

        #region Get

        public override FeatureModuleCategory GetFeatureModuleCategory() => FeatureModuleCategory.Fight;

        /// <summary>
        /// Allows you to assign the correct GameObject which is assigned as the player's Focus
        /// </summary>
        /// <param name="pCurrentFocusEnnemyBot">The GameObject that is assigned as the player's Focus</param>
        /// <returns>Indicates if the GameObject has been changed</returns>
        bool CurrentFocusEnnemyBot(GameObject pCurrentFocusEnnemyBot)
        {
            if (_currentEnnemyFightGameObject)
            {

                if (_currentEnnemyFightGameObject == pCurrentFocusEnnemyBot)
                    return false;
            }

            for (int i = 0; i < _ennemyBotData.Length; ++i)
            {

                if (_ennemyBotData[i].botObject != pCurrentFocusEnnemyBot)
                    continue;

                _currentEnnemyFightGameObject = pCurrentFocusEnnemyBot;

                return true;
            }

            return false;
        }

        /// <summary>
        /// Allows you to check if the index which defines the bat which is in the combat sequence is still correct
        /// </summary>
        /// <returns>Returns the index that matches the GameObject that is assigned as the player's focus</returns>
        public int GetFightModeIndex() {

            for (int i = 0; i < _fightModeData.Length; ++i) {

                if (_currentEnnemyFightGameObject != _fightModeData[i].ennemyBot)
                    continue;

                return i;
            }

            return -1;
        }

        public FightComboSequenceData GetCurrentComboSequenceData => _fightModeData[_currentFightModeDataIndex].fightComboSequenceData[_currentFightComboSequenceDataIndex];

        public FightOffenseData GetCurrentFightOffenseData => GetCurrentComboSequenceData.fightOffenseData[_currentFightOffenseDataIndex];

        /// <summary>
        /// Returns the AnimationClip of the Offense in the combo
        /// </summary>
        public AnimationClip GetCurrentOffenseDataClip => GetCurrentOffense.GetAnimationClip();

        public Offense.Offense GetCurrentOffense => GetCurrentFightOffenseData.offense;

        #endregion

        #region Method

        public override bool OnUpdate(bool pIsLeftFocus, bool pIsRightFocus, Transform pCurrentFocusEnnemyBot)
        {
            if (!base.OnUpdate())
                return false;

            if (!pCurrentFocusEnnemyBot)
                return false;

            if (CurrentFocusEnnemyBot(pCurrentFocusEnnemyBot.gameObject)) {

                int fightModeIndex = GetFightModeIndex();

                if (fightModeIndex != _currentFightModeDataIndex) {
                
                    _lastFightModeDataIndex = _currentFightModeDataIndex;

                    _currentFightModeDataIndex = fightModeIndex;
                }
            }

            if (_isComboTcheck) {

                for (int i = 0; i < _fightModeData[_currentFightModeDataIndex].fightComboSequenceData.Length; ++i)
                {

                    if (_fightModeData[_currentFightModeDataIndex].fightComboSequenceData[i].isCompleted)
                        continue;

                    _currentFightComboSequenceDataIndex = i;
                }
            }

            if (_ennemyBotData[_currentFightModeDataIndex].botAnimator.GetCurrentAnimatorClipInfo(0)[0].clip != GetCurrentOffenseDataClip) {

                if (GetCurrentOffense.GetOffenseType == OffenseType.STANCE)
                    _currentMaxWaithingTime = GetCurrentFightOffenseData.waithingTime;

                else
                    _currentMaxcooldownTime = GetCurrentFightOffenseData.cooldownTime;

                _ennemyBotData[_currentFightModeDataIndex].botAnimator.Play(GetCurrentOffenseDataClip.name);
            }

            if (_currentMaxWaithingTime != 0) {

                ++_currentWainthingTime;

                if (_ennemyBotData[_currentFightModeDataIndex].botAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.95)
                    _ennemyBotData[_currentFightModeDataIndex].botAnimator.Play(GetCurrentOffenseDataClip.name);

                if (_currentWainthingTime >= _currentMaxWaithingTime) {

                    _currentMaxWaithingTime = 0;

                    _fightModeData[_currentFightModeDataIndex].fightComboSequenceData[_currentFightComboSequenceDataIndex].fightOffenseData[_currentFightOffenseDataIndex].isCompleted = true;

                    if (_currentFightOffenseDataIndex + 1 < GetCurrentComboSequenceData.fightOffenseData.Length)
                        ++_currentFightOffenseDataIndex;
                    else if (!GetCurrentComboSequenceData.isCompleted) {

                        _fightModeData[_currentFightModeDataIndex].fightComboSequenceData[_currentFightComboSequenceDataIndex].isCompleted = true;

                        _isComboTcheck = true;

                        return true;

                    }
                        
                }

            }


            return true;
        }

        void LoopingCombo() {

            for (int i = 0; i < _fightModeData.Length; ++i) {

                for (int j = 0; j < _fightModeData[i].fightComboSequenceData.Length; ++j) {

                    for (int k = 0; k < _fightModeData[i].fightComboSequenceData[j].fightOffenseData.Length; ++k)
                        _fightModeData[i].fightComboSequenceData[j].fightOffenseData[k].isCompleted = false;

                    _fightModeData[i].fightComboSequenceData[j].isCompleted = false;
                }
            }

            _currentFightModeDataIndex = 0;
            _currentFightComboSequenceDataIndex = 0;
            _currentFightOffenseDataIndex = 0;

        }

        public override void OnEnabled()
        {
            base.OnEnabled();

            _lastFightModeDataIndex = _currentFightModeDataIndex;
        }

        #endregion
    }

#if UNITY_EDITOR

    [CustomPropertyDrawer(typeof(FightModule))]
    public partial class FightModuleDrawer : FeatureModuleDrawer{
        public override bool OnNUI(Rect position, SerializedProperty property, GUIContent label){
            if (!base.OnNUI(position, property, label))
                return false;

            drawer.ReorderableList("_fightModeData");

            drawer.EndProperty();
            return true;
        }
    }

    [CustomPropertyDrawer(typeof(FightModeData))]
    public partial class FightModeDataDrawer : ComponentNUIPropertyDrawer
    {
        public override bool OnNUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (!base.OnNUI(position, property, label))
                return false;

            if (drawer.Field("ennemyBot").objectReferenceValue)
                drawer.ReorderableList("fightComboSequenceData");

            drawer.EndProperty();
            return true;
        }
    }

    [CustomPropertyDrawer(typeof(FightOffenseData))]
    public partial class FightOffenseDataDrawer : ComponentNUIPropertyDrawer
    {
        Offense.Offense offense;

        public override bool OnNUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (!base.OnNUI(position, property, label))
                return false;

            offense = drawer.Field("offense").objectReferenceValue as Offense.Offense;

            if (offense) {

                if (offense.GetOffenseType == OffenseType.STANCE)
                    drawer.Field("waithingTime", true, "Frames", "Waithing: ");
                else
                    drawer.Field("cooldownTime", true, "Frames", "Cooldown: ");
            }

            drawer.EndProperty();
            return true;
        }
    }

    [CustomPropertyDrawer(typeof(FightComboSequenceData))]
    public partial class FightComboSequenceDataDrawer : ComponentNUIPropertyDrawer
    {
        public override bool OnNUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (!base.OnNUI(position, property, label))
                return false;

            drawer.Field("fightingModeType", true, null, "Combo fightingMode: ");

            drawer.ReorderableList("fightOffenseData");

            drawer.EndProperty();
            return true;
        }
    }

#endif
}