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

        public Offense.Offense idleOffense;

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

        /// <summary>
        /// Represents whether this Offense is being carried out
        /// </summary>
        [Tooltip("Represents whether this Offense is being carried out")]
        public bool isPlaying;
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
        int _currentFightModeDataIndex;

        /// <summary>
        /// Represents the index to fetch cached information about the enemy bot
        /// </summary>
        [SerializeField, Tooltip("Represents the index to fetch cached information about the enemy bot")]
        int _currentEnnemyBotCacheIndex;

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
        float _currentWaithingTime;

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
        /// Indicates whether the check for combo indexes should be run
        /// </summary>
        [SerializeField, Tooltip("Indicates whether the check for combo indexes should be run")]
        bool _isComboTcheck;

        GameObject _currentEnnemyBotFocus;

        #endregion

        #region Get

        public override FeatureModuleCategory GetFeatureModuleCategory() => FeatureModuleCategory.Fight;

        Offense.Offense GetCurrentOffense => GetOffense(GetCurrentOffenseData, _ennemyBotData[_currentEnnemyBotCacheIndex].botAnimator.GetCurrentAnimatorClipInfo(0)[0].clip);

        Offense.Offense GetOffense(FightOffenseData pFightOffenseData, AnimationClip pAnimationClip) {

            if (pFightOffenseData.offense.GetAnimationClip(pAnimationClip.name))
                return pFightOffenseData.offense;

            return null;
        }

        FightOffenseData GetNextOffenseData() {

            int nextFightModeDataIndex = _currentFightModeDataIndex;
            int nextFightComboSequenceDataIndex = _currentFightComboSequenceDataIndex;
            int nextFightOffenseDataIndex = _currentFightOffenseDataIndex;

            ++nextFightOffenseDataIndex;

            if (nextFightOffenseDataIndex > GetFightOffenseData.Length - 1) {

                ++nextFightComboSequenceDataIndex;

                nextFightOffenseDataIndex = 0;
            }

            if (nextFightComboSequenceDataIndex > GetFightComboSequenceData.Length - 1) {

                nextFightComboSequenceDataIndex = 0;
                nextFightOffenseDataIndex = 0;

                DefaultFightModeSetup();
            }

            _currentFightModeDataIndex = nextFightModeDataIndex;
            _currentFightComboSequenceDataIndex = nextFightComboSequenceDataIndex;
            _currentFightOffenseDataIndex = nextFightOffenseDataIndex;

            return _fightModeData[nextFightModeDataIndex].fightComboSequenceData[nextFightComboSequenceDataIndex].fightOffenseData[nextFightOffenseDataIndex];
        }

        GameObject GetCurrentEnnemyBotFocus => _currentEnnemyBotFocus;

        FightComboSequenceData[] GetFightComboSequenceData => _fightModeData[_currentFightModeDataIndex].fightComboSequenceData;

        FightOffenseData[] GetFightOffenseData => GetFightComboSequenceData[_currentFightComboSequenceDataIndex].fightOffenseData;

        FightOffenseData GetCurrentOffenseData => GetFightOffenseData[_currentFightOffenseDataIndex];

        Animator GetEnnemyBotAnimator => _ennemyBotData[_currentEnnemyBotCacheIndex].botAnimator;

        bool GetIfWaithingTime => _currentMaxWaithingTime > 0;

        bool GetIfNeedLooping(float pPourcentageTime) {

            if (!GetCurrentOffense)
                return false;

            if (_currentMaxWaithingTime == GetCurrentOffense.GetLengthClip(false))
                return false;

            return GetEnnemyBotNormalizedTime() > pPourcentageTime;
        }

        bool OffenseDelaySetup(){

            if (!GetIfWaithingTime)
                return false;

            _currentWaithingTime += Time.deltaTime;

            if (GetIfNeedLooping(0.95f))
                GetEnnemyBotAnimator.Play(GetCurrentOffense.GetAnimationClip().name);
                
            if (_currentWaithingTime >= _currentMaxWaithingTime){

                _currentMaxWaithingTime = 0;
                _currentWaithingTime = 0;

                GetFightOffenseData[_currentFightOffenseDataIndex].isCompleted = true;

                return true;
            }

            return false;
        }

        bool GetIfChangeEnnemyFocus(Transform pCurrentFocusEnnemyBot) {

            if (!_currentEnnemyBotFocus)
                return true;

            return _currentEnnemyBotFocus.transform != pCurrentFocusEnnemyBot;
        }

        float GetTimer(FightOffenseData pFightOffenseData, bool pIsStance)
        {

            if (pIsStance)

                return pFightOffenseData.waithingTime != 0 ? pFightOffenseData.waithingTime : pFightOffenseData.offense.GetLengthClip(false);

            return pFightOffenseData.cooldownTime != 0 ? pFightOffenseData.offense.GetLengthClip(false) + pFightOffenseData.cooldownTime : pFightOffenseData.offense.GetLengthClip(false);
        }

        float GetEnnemyBotNormalizedTime() => GetEnnemyBotAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime;

        #endregion

        #region Method

        void EnnemyBotFocus(Transform pCurrentFocusEnnemyBot)
        {
            //EnnemyBot FightModeData
            FightModeDataFocusInit(pCurrentFocusEnnemyBot);

            //EnnemyBot cache
            EnnemyBotFocusCacheInit(pCurrentFocusEnnemyBot);

            _currentEnnemyBotFocus = _ennemyBotData[_currentEnnemyBotCacheIndex].botObject;
        }

        void FightModeDataFocusInit(Transform pCurrentFocusEnnemyBot) {

            for (int i = 0; i < _fightModeData.Length; ++i)
            {

                if (_fightModeData[i].ennemyBot != pCurrentFocusEnnemyBot)
                    continue;

                if (_currentFightModeDataIndex != i)
                    _currentFightModeDataIndex = i;

                return;
            }
        }

        void EnnemyBotFocusCacheInit(Transform pCurrentFocusEnnemyBot) {

            for (int i = 0; i < _ennemyBotData.Length; ++i)
            {

                if (_ennemyBotData[i].botObject != pCurrentFocusEnnemyBot)
                    continue;

                if (_currentEnnemyBotCacheIndex != i)
                    _currentEnnemyBotCacheIndex = i;

                break;
            }
        }

        void FightModeDataInit() {

            //FightComboSequence
            FightComboSequenceDataInit();

            //FightOffense
            FightOffenseDataInit();
        }

        void FightComboSequenceDataInit() {

            for (int i = 0; i < GetFightComboSequenceData.Length; ++i)
            {

                if (GetFightComboSequenceData[i].isCompleted)
                    continue;

                _currentFightComboSequenceDataIndex = i;

                return;
            }
        }

        void FightOffenseDataInit() {

            for (int i = 0; i < GetFightOffenseData.Length; ++i)
            {

                if (GetFightOffenseData[i].isCompleted)
                    continue;

                _currentFightOffenseDataIndex = i;

                return;
            }
        }

        void ApplyOffense(FightOffenseData pFightOffenseData) {

            _currentMaxWaithingTime = GetTimer(pFightOffenseData, pFightOffenseData.offense.GetOffenseType == OffenseType.STANCE);

            if (GetEnnemyBotAnimator.GetCurrentAnimatorClipInfo(0)[0].clip.name == pFightOffenseData.offense.GetAnimationClip().name) {

                GetEnnemyBotAnimator.Play(pFightOffenseData.offense.GetAnimationClip().name, -1, 0);

                return;
            }

            GetEnnemyBotAnimator.Play(pFightOffenseData.offense.GetAnimationClip().name);
        }

        void DefaultFightModeSetup() {

            for (int i = 0; i < GetFightComboSequenceData.Length; ++i) {

                for (int j = 0; j < GetFightComboSequenceData[i].fightOffenseData.Length; ++j)
                    GetFightComboSequenceData[i].fightOffenseData[j].isCompleted = false;

                GetFightComboSequenceData[i].isCompleted = false;
            }
        }

        public override bool OnUpdate(bool pIsLeftFocus, bool pIsRightFocus, Transform pCurrentFocusEnnemyBot)
        {
            if (!base.OnUpdate())
                return false;

            if (GetIfChangeEnnemyFocus(pCurrentFocusEnnemyBot)) {

                EnnemyBotFocus(pCurrentFocusEnnemyBot);

                FightModeDataInit();

                ApplyOffense(GetCurrentOffenseData);
            }

            if (OffenseDelaySetup()) {

                ApplyOffense(GetNextOffenseData());

                return true;
            }

            return true;
        }

        public override void OnEnabled()
        {
            base.OnEnabled();

            _currentFightModeDataIndex = 0;
            _currentFightComboSequenceDataIndex = 0;
            _currentFightOffenseDataIndex = 0;
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
                    drawer.Field("waithingTime", true, "secs", "Waithing: ");
                else
                    drawer.Field("cooldownTime", true, "secs", "Cooldown: ");
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