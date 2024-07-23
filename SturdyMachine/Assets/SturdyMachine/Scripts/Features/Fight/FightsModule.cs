using System;

using UnityEngine;
using SturdyMachine.Offense;
using SturdyMachine.Offense.Blocking;
using SturdyMachine.Features.Fight.Sequence;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;
using NWH.VehiclePhysics2;
#endif

namespace SturdyMachine.Features.Fight{

    /// <summary>
    /// All types of combat modes possible
    /// </summary>
    public enum FightingModeType { Default, Passive, Agressif }

    /// <summary>
    /// Allows you to configure the movements that the bot will be able to make during these combat phases
    /// </summary>
    [Serializable, Tooltip("Allows you to configure the movements that the bot will be able to make during these combat phases")]
    public struct FightModeData{

        public byte currentEnemyBotIndex;

        public FightSequenceData[] fightSequenceData;
    }

    /// <summary>
    /// Allows configuration of combos
    /// </summary>
    [Serializable, Tooltip("Allows configuration of combos")]
    public struct FightComboSequenceData{

        /// <summary>
        /// Allows you to indicate whether this combo should be designated as the default one
        /// </summary>
        [Tooltip("Allows you to indicate whether this combo should be designated as the default one")]
        public bool isDefault;

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
    public struct FightOffenseData{

        /// <summary>
        /// The Offense you want the bot to execute
        /// </summary>
        [Tooltip("The Offense you want the bot to execute")]
        public Offense.Offense offense;

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
    public partial class FightsModule : FeatureModule{
        
        #region Attribut

        /// <summary>
        /// Store the configuration of all combos of all EnemyBot
        /// </summary>
        [SerializeField, Tooltip("Store the configuration of all combos of all EnemyBot")]
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
        /// Indicates the current index in the Offense sequence list
        /// </summary>
        [SerializeField, Tooltip("Indicates the current index in the Offense sequence list")]
        int _currentFightOffenseSequenceIndex;

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

        bool _isLastHitConfirmState;

        byte _nbrOfEnemyBotOffenseBlocking;

        #endregion

        #region Properties

        public override FeatureModuleCategory GetFeatureModuleCategory() => FeatureModuleCategory.Fight;

        /// <summary>
        /// Allows the management of information on the next Offenses of a combo
        /// </summary>
        /// <returns>Returns information about a combo's next Offense</returns>
        FightOffenseData GetNextOffenseData(){

            //Saves index information for all of the Bot's combat combos
            int nextFightModeDataIndex = _currentFightModeDataIndex;
            int nextFightComboSequenceDataIndex = _currentFightComboSequenceDataIndex;
            int nextFightOffenseDataIndex = _currentFightOffenseDataIndex;

            //Security that allows the FightOffenseData index to be increased based on the total number
            ++nextFightOffenseDataIndex;

            if (nextFightOffenseDataIndex > GetFightOffenseData.Length - 1){

                _fightModeData[_currentFightModeDataIndex].fightSequenceData[_currentFightOffenseSequenceIndex].fightComboSequenceData[_currentFightComboSequenceDataIndex].isCompleted = true;

                ++nextFightComboSequenceDataIndex;

                nextFightOffenseDataIndex = 0;

                _nbrOfEnemyBotOffenseBlocking = GetBlockingOffenseCount();
            }

            //Security that allows the FightComboSequenceData index to be increased based on the total number
            if (nextFightComboSequenceDataIndex > GetFightComboSequenceData.Length - 1){

                nextFightComboSequenceDataIndex = 0;
                nextFightOffenseDataIndex = 0;

                DefaultFightModeSetup();
            }

            //Assigns the correct indexes for all components of this Bot's FightModule
            _currentFightModeDataIndex = nextFightModeDataIndex;
            _currentFightComboSequenceDataIndex = nextFightComboSequenceDataIndex;
            _currentFightOffenseDataIndex = nextFightOffenseDataIndex;

            //Returns information about a combo's next Offense
            return _fightModeData[_currentFightModeDataIndex].fightSequenceData[_currentFightOffenseSequenceIndex].fightComboSequenceData[_currentFightComboSequenceDataIndex].fightOffenseData[_currentFightOffenseDataIndex];
        }

        /// <summary>
        /// Returns the complete list of Offense combo sequences
        /// </summary>
        FightSequenceData[] GetFightSequenceData => _fightModeData[_currentFightModeDataIndex].fightSequenceData;

        /// <summary>
        /// Returns the list of information present in the Bot's FightComboSequence
        /// </summary>
        FightComboSequenceData[] GetFightComboSequenceData => GetFightSequenceData[_currentFightOffenseSequenceIndex].fightComboSequenceData;

        /// <summary>
        /// Returns the list of information present in the Bot's FightOffenseData
        /// </summary>
        FightOffenseData[] GetFightOffenseData => GetFightComboSequenceData[_currentFightComboSequenceDataIndex].fightOffenseData;

        /// <summary>
        /// Returns information from the Bot's current FightOffenseData
        /// </summary>
        FightOffenseData GetCurrentOffenseData() {

            if (GetFightOffenseData.Length == 0)
                return new FightOffenseData();

            return GetFightOffenseData[_currentFightOffenseDataIndex];
        }

        /// <summary>
        /// Allows you to check if the Offense the Bot is currently playing needs to be replayed
        /// </summary>
        /// <param name="pFeatureCacheData">The basic cached information qi brings together all other feature modules</param>
        /// <param name="pPourcentageTime">Clip activation percentage</param>
        /// <returns>Returns if Bot Offense needs to be replayed</returns>
        bool GetIfNeedLooping(float pPourcentageTime){

            if (!FEATURE_MANAGER.GetCurrentEnemyBotOffenseManager.GetCurrentOffense)
                return false;

            //Checks if the Bot's Current Offense has been assigned
            if (!FEATURE_MANAGER.GetCurrentEnemyBotOffenseManager.GetCurrentOffense == GetCurrentOffenseData().offense)
                return false;

            //Checks if the normalized time of the Bot Offense clip has exceeded the desired percentage setting
            return FEATURE_MANAGER.GetCurrentEnemyBotAnimatorStateInfo.normalizedTime > pPourcentageTime;
        }

        /// <summary>
        /// Allows timeout management with the next Offense
        /// </summary>
        /// <param name="pFeatureCacheData">The basic cached information qi brings together all other feature modules</param>
        /// <returns>Returns if the waiting time has been reached in order to be able to play the next Offense</returns>
        bool OffenseDelaySetup(){

            _currentWaithingTime += Time.deltaTime;

            //Assigns the same clip again if its normalized time has exceeded the percentage desired in parameter
            if (GetIfNeedLooping(0.98f)) {

                if (FEATURE_MANAGER.GetCurrentEnemyBotOffenseManager.GetCurrentOffense)
                    FEATURE_MANAGER.GetCurrentEnemyBotAnimator.Play(GetCurrentOffenseData().offense.GetAnimationClip(AnimationClipOffenseType.Full).name);
            }

            //Returns that the wait time has been reached
            if (_currentWaithingTime >= FEATURE_MANAGER.GetCurrentEnemyBotAnimationClip.length)
            {
                _currentWaithingTime = 0;

                if (GetFightOffenseData.Length != 0)
                    GetFightOffenseData[_currentFightOffenseDataIndex].isCompleted = true;

                return true;
            }

            return false;
        }

        /// <summary>
        /// Allows you to assign the number of Offenses that must be blocked in a combo in order to be able to do a Parry
        /// </summary>
        /// <returns>Returns the number of Offenses that must be blocked in a combo</returns>
        byte GetBlockingOffenseCount() {

            byte blockingOffenseCount = 0;

            for (byte i = 0; i < _fightModeData[_currentFightModeDataIndex].fightSequenceData[_currentFightOffenseSequenceIndex].fightComboSequenceData[_currentFightComboSequenceDataIndex].fightOffenseData.Length; ++i) {

                //Direction
                if (_fightModeData[_currentFightModeDataIndex].fightSequenceData[_currentFightOffenseSequenceIndex].fightComboSequenceData[_currentFightComboSequenceDataIndex].fightOffenseData[i].offense.GetOffenseDirection == OffenseDirection.STANCE)
                    continue;

                //Type
                if (_fightModeData[_currentFightModeDataIndex].fightSequenceData[_currentFightOffenseSequenceIndex].fightComboSequenceData[_currentFightComboSequenceDataIndex].fightOffenseData[i].offense.GetOffenseType == OffenseType.STANCE)
                    continue;

                ++blockingOffenseCount;
            }

            return blockingOffenseCount;
        }

        /// <summary>
        /// Allows you to check if there is a combo that is assigned as default in all combo sequences
        /// </summary>
        /// <param name="fightSequenceData">The combo sequence we need to check</param>
        /// <param name="pFightComboSequenceIndex">Returns the index of the combo that is assigned as default</param>
        /// <returns>Returns if there is a combo sequence that is assigned as default</returns>
        bool GetIfDefaultSequence(FightSequenceData fightSequenceData, out int pFightComboSequenceIndex) {

            pFightComboSequenceIndex = 0;

            for (byte i = 0; i < fightSequenceData.fightComboSequenceData.Length; ++i){
                
                if (!fightSequenceData.fightComboSequenceData[i].isDefault)
                    continue;

                pFightComboSequenceIndex = i;

                return true;
            }

            return false;
        }

        public byte GetNbrOfEnemyBotOffenseBlocking => _nbrOfEnemyBotOffenseBlocking;

        #endregion

        #region Method

        public override void Initialize()
        {
            base.Initialize();

            //Check if there are enemy bots in the scene
            if (FEATURE_MANAGER.GetEnemyBotObject.Length == 0)
                return;

            FightOffenseSequenceInit();

        }

        public override bool OnUpdate(bool pIsLeftFocus, bool pIsRightFocus)
        {
            if (!base.OnUpdate())
                return false;

            if (_fightModeData.Length == 0)
                return true;

            //Suspends the management of Offense combos if HitConfirm is activated
            if (FEATURE_MANAGER.GetHitConfirmModule.GetIsHitConfirmActivated) 
            {
                _currentWaithingTime = 0;

                return true;
            }

            //Assigns all the correct information if the Focus has been changed
            if (FEATURE_MANAGER.GetFocusModule.GetIsEnemyBotFocusChanged){

                //Assigns the index that corresponds to the new Bot the player is looking at
                EnnemyBotFocus();

                //Assigns the new basic information of the FightModule depending on the new Bot that is in Focus by the player
                FightModeDataInit();

                //Apply the Offense that should be played on the new enemy Bot
                ApplyOffense();
            }

            //Assigns the new Offense when the time limit ends
            if (OffenseDelaySetup()){

                GetNextOffenseData();

                ApplyOffense();

                return true;
            }

            return true;
        }

        public override void OnEnabled(){
            base.OnEnabled();

            _currentFightModeDataIndex = 0;
            _currentFightComboSequenceDataIndex = 0;
            _currentFightOffenseDataIndex = 0;
        }

        /// <summary>
        /// Allows the assignment of combat information with the new enemy Bot that is in Focus
        /// </summary>
        void EnnemyBotFocus()
        {
            //EnnemyBot FightModeData
            FightModeDataFocusInit();

            //EnnemyBot cache
            EnnemyBotFocusCacheInit();
        }

        /// <summary>
        /// Allows the assignment of the index of the Bot which is in Focus with the player
        /// </summary>
        void FightModeDataFocusInit()
        {
            //Iterates through the FightMode list of all enemy Bots
            for (int i = 0; i < _fightModeData.Length; ++i)
            {
                //Checks if the Bot that is assigned as Focus matches the one in the list
                if (_fightModeData[i].currentEnemyBotIndex != FEATURE_MANAGER.GetFocusModule.GetCurrentEnemyBotIndex)
                    continue;

                //Assigns FightMode index based on enemy Bot
                if (_currentFightModeDataIndex != i)
                    _currentFightModeDataIndex = i;

                return;
            }
        }

        /// <summary>
        /// Allows the assignment of the cached information index of enemy Bots that corresponds to the one assigned as the player's Focus
        /// </summary>
        void EnnemyBotFocusCacheInit()
        {
            //Iterates through the list of cached information of all enemy Bots
            for (byte i = 0; i < FEATURE_MANAGER.GetEnemyBotObject.Length; ++i)
            {
                //Checks if the Bot recorded as the Cached Focus matches the one in the Cached Enemy Bot list
                if (FEATURE_MANAGER.GetFocusModule.GetCurrentEnemyBotIndex != i)
                    continue;

                //Assigns the correct index of the cache enemy Bot
                if (_currentEnnemyBotCacheIndex != i)
                    _currentEnnemyBotCacheIndex = i;

                break;
            }
        }

        /// <summary>
        /// Allows the assignment of FightMode information based on the state of the enemy Bot assigned as the cached player's Focus
        /// </summary>
        void FightModeDataInit()
        {
            //FightComboSequence
            FightComboSequenceDataInit();

            //FightOffense
            FightOffenseDataInit();
        }

        /// <summary>
        /// Allows the assignment of the index that corresponds to the bot's combo sequence
        /// </summary>
        void FightComboSequenceDataInit()
        {
            //Iterates through the list of combo sequences of the enemy Bot that has been configured
            for (int i = 0; i < GetFightComboSequenceData.Length; ++i)
            {
                //Checks if the combo sequence has already been completed
                if (GetFightComboSequenceData[i].isCompleted)
                    continue;

                _currentFightComboSequenceDataIndex = i;

                return;
            }
        }

        /// <summary>
        /// Allows the assignment of the FightOffenseData index of the enemy Bot in Focus
        /// </summary>
        void FightOffenseDataInit()
        {
            //Iterates through the list of FightOffenseData of the Bot assigned to Focus
            for (int i = 0; i < GetFightOffenseData.Length; ++i)
            {
                //Checks if the FightOffenseData of the enemy Bot in Focus has already been completed
                if (GetFightOffenseData[i].isCompleted)
                    continue;

                _currentFightOffenseDataIndex = i;

                return;
            }
        }

        /// <summary>
        /// Allows you to assign all the necessary information as well as the next Offense of a Bot
        /// </summary>
        /// <param name="pFightOffenseData">FightOffenseData information for the next Combo Offense</param>
        /// <param name="pFeatureCacheData">Allows the assignment of the cached information index of enemy Bots that corresponds to the one assigned as the player's Focus</param>
        void ApplyOffense()
        {
            //Allows the assignment of the same Offense as the previous one
            if (FEATURE_MANAGER.GetCurrentEnemyBotAnimationClip.name == GetCurrentOffenseData().offense.GetAnimationClip(AnimationClipOffenseType.Full).name)
            {
                if (GetIfNeedLooping(98f)) 
                    FEATURE_MANAGER.GetCurrentEnemyBotAnimator.Play(GetCurrentOffenseData().offense.GetAnimationClip(AnimationClipOffenseType.Full).name, -1, 0);                    

                return;
            }

            //Apply the next Offense to the enemy Bot
            FEATURE_MANAGER.GetCurrentEnemyBotOffenseManager.CurrentOffenseClipNameSetup(GetCurrentOffenseData().offense.GetAnimationClip(AnimationClipOffenseType.Full).name);
            FEATURE_MANAGER.GetCurrentEnemyBotAnimator.Play(GetCurrentOffenseData().offense.GetAnimationClip(AnimationClipOffenseType.Full).name);
        }

        /// <summary>
        /// Assign all default FightCombo Sequence information as well as the FightOffenseData of the enemy Bot present
        /// </summary>
        void DefaultFightModeSetup()
        {
            for (byte i = 0; i < GetFightSequenceData.Length; ++i) {

                //Iterates through the list of FightComboSequenceData of the enemy Bot
                for (byte j = 0; j < GetFightSequenceData[i].fightComboSequenceData.Length; ++j)
                {
                    //Iterates through the FightOffenseData list of the enemy Bot's FightComboSequenceData
                    for (byte k = 0; k < GetFightComboSequenceData[i].fightOffenseData.Length; ++k)
                        GetFightSequenceData[i].fightComboSequenceData[j].fightOffenseData[k].isCompleted = false;

                    GetFightSequenceData[i].fightComboSequenceData[j].isCompleted = false;
                }
            }

            _nbrOfEnemyBotOffenseBlocking = GetBlockingOffenseCount();
        }

        /// <summary>
        /// Allows you to initialize the Offense sequence list
        /// </summary>
        void FightOffenseSequenceInit() {

            List<byte> comboSequenceDataIndex = new List<byte>();

            _fightModeData = new FightModeData[FEATURE_MANAGER.GetEnemyBotObject.Length];

            //Iterates through the list of fightModeData
            for (byte i = 0; i < _fightModeData.Length; ++i)
            {
                _fightModeData[i] = new FightModeData();

                _fightModeData[i].currentEnemyBotIndex = i;

                _fightModeData[i].fightSequenceData = FEATURE_MANAGER.GetFightOffenseSequenceData(FEATURE_MANAGER.GetEnemyBotType(i)).fightSequenceData;

                //Assigns the list of Offense sequences by iterating through all Offense sequences
                for (byte j = 0; j < _fightModeData[i].fightSequenceData.Length; ++j){

                    //Assign sequence list elements with a random if there is no sequence that is set as default
                    if (!GetIfDefaultSequence(_fightModeData[i].fightSequenceData[j], out int fightComboSequenceDataIndex)){

                        byte currentComboSequenceDataIndex = 0;

                        System.Random random = new System.Random();

                        for (byte k = 0; k < _fightModeData[i].fightSequenceData[j].fightComboSequenceData.Length; ++k){

                            currentComboSequenceDataIndex = (byte)random.Next(_fightModeData[i].fightSequenceData[j].fightComboSequenceData.Length);

                            while (comboSequenceDataIndex.Contains(currentComboSequenceDataIndex))
                                currentComboSequenceDataIndex = (byte)random.Next(_fightModeData[i].fightSequenceData[j].fightComboSequenceData.Length);

                            comboSequenceDataIndex.Add(currentComboSequenceDataIndex);

                            _fightModeData[i].fightSequenceData[j].fightComboSequenceData[k] = FEATURE_MANAGER.GetFightOffenseSequenceData(FEATURE_MANAGER.GetEnemyBotType(i)).fightSequenceData[j].fightComboSequenceData[fightComboSequenceDataIndex];
                        }

                        continue;
                    }

                    //Assigns the combo sequence that is assigned as the default
                    FightComboSequenceData[] defaultFightComboSequenceData = new FightComboSequenceData[FEATURE_MANAGER.GetFightOffenseSequenceData(FEATURE_MANAGER.GetEnemyBotType(i)).fightSequenceData[j].fightComboSequenceData.Length];

                    for (byte k = 0; k < defaultFightComboSequenceData.Length; ++k)
                        defaultFightComboSequenceData[k] = FEATURE_MANAGER.GetFightOffenseSequenceData(FEATURE_MANAGER.GetEnemyBotType(i)).fightSequenceData[j].fightComboSequenceData[k];

                    defaultFightComboSequenceData = new FightComboSequenceData[1];
                    defaultFightComboSequenceData[0] = FEATURE_MANAGER.GetFightOffenseSequenceData(FEATURE_MANAGER.GetEnemyBotType(i)).fightSequenceData[j].fightComboSequenceData[fightComboSequenceDataIndex];

                    _fightModeData[i].fightSequenceData[j].fightComboSequenceData = defaultFightComboSequenceData;

                    continue;

                }

            }

            _nbrOfEnemyBotOffenseBlocking = GetBlockingOffenseCount();
        }

        #endregion
    }

#if UNITY_EDITOR

    [CustomPropertyDrawer(typeof(FightsModule))]
    public partial class FightModuleDrawer : FeatureModuleDrawer{
        public override bool OnNUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (!base.OnNUI(position, property, label))
                return false;

            drawer.BeginSubsection("Debug Value");

            drawer.Field("_currentFightModeDataIndex", false, null, "FightMode Index: ");
            drawer.Field("_currentFightOffenseSequenceIndex", false, null, "OffenseSequence Index: ");
            drawer.Field("_currentFightComboSequenceDataIndex", false, null, "ComboSequence Index: ");
            drawer.Field("_currentFightOffenseDataIndex", false, null, "FightOffense Index: ");

            drawer.EndSubsection();

            drawer.ReorderableList("_fightModeData");

            drawer.EndProperty();
            return true;
        }
    }

    [CustomPropertyDrawer(typeof(FightModeData))]
    public partial class FightModeDataDrawer : ComponentNUIPropertyDrawer{
        public override bool OnNUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (!base.OnNUI(position, property, label))
                return false;

            GUI.enabled = false;

            if (drawer.Field("ennemyBot").objectReferenceValue)
                drawer.ReorderableList("fightSequenceData");

            GUI.enabled = true;

            drawer.EndProperty();
            return true;
        }
    }

    [CustomPropertyDrawer(typeof(FightOffenseData))]
    public partial class FightOffenseDataDrawer : ComponentNUIPropertyDrawer{
        Offense.Offense offense;

        public override bool OnNUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (!base.OnNUI(position, property, label))
                return false;

            offense = drawer.Field("offense").objectReferenceValue as Offense.Offense;

            if (offense)
            {

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
    public partial class FightComboSequenceDataDrawer : ComponentNUIPropertyDrawer{
        public override bool OnNUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (!base.OnNUI(position, property, label))
                return false;

            drawer.Field("isDefault");

            drawer.ReorderableList("fightOffenseData");

            drawer.Field("isCompleted", false);

            drawer.EndProperty();
            return true;
        }
    }

#endif
}