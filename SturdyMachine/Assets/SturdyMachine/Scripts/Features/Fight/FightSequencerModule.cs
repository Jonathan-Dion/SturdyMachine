using System;

using UnityEngine;
using SturdyMachine.Offense;
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
    public partial class FightSequencerModule : FeatureModule{
        
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
        /// Indicates the number of moves that must be blocked in a sequence in order to activate a Parry
        /// </summary>
        byte _nbrOfEnemyBotOffenseBlocking;

        #endregion

        #region Properties

        public override FeatureModuleCategory GetFeatureModuleCategory() => FeatureModuleCategory.Fight;

        /// <summary>
        /// Returns the waiting delay before applying the next one
        /// </summary>
        float GetCurrentMaxWaithingTimer => featureManager.GetSpecificBotAnimationClipByType(featureManager.GetCurrentEnemyBotType).length + GetCurrentOffenseData().cooldownTime;

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
        public FightOffenseData[] GetFightOffenseData => GetFightComboSequenceData[_currentFightComboSequenceDataIndex].fightOffenseData;

        /// <summary>
        /// Returns information from the Bot's current FightOffenseData
        /// </summary>
        FightOffenseData GetCurrentOffenseData() {

            if (GetFightOffenseData.Length == 0)
                return new FightOffenseData();

            return GetFightOffenseData[_currentFightOffenseDataIndex];
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
        /// Returns the number of offenses that should be blocked in the current combo sequence
        /// </summary>
        public byte GetNbrOfEnemyBotOffenseBlocking => _nbrOfEnemyBotOffenseBlocking;

        #endregion

        #region Methods

        public override void Initialize(FeatureManager pFeatureManager)
        {
            base.Initialize();

            //Check if there are enemy bots in the scene
            if (featureManager.GetEnemyBotObject.Length == 0)
                return;

            //Allows you to initialize the Offense sequence list
            List<byte> comboSequenceDataIndex = new List<byte>();

            _fightModeData = new FightModeData[featureManager.GetEnemyBotObject.Length];

            System.Random random = new System.Random();

            //Iterates through the list of fightModeData
            for (byte i = 0; i < _fightModeData.Length; ++i){

                _fightModeData[i] = new FightModeData();

                _fightModeData[i].currentEnemyBotIndex = i;

                _fightModeData[i].fightSequenceData = featureManager.GetFightSequenceDatas(featureManager.GetEnemyBotType(i));

                FightComboSequenceData[] defaultFightComboSequenceData = new FightComboSequenceData[0];

                //Assigns the list of Offense sequences by iterating through all Offense sequences
                for (byte j = 0; j < _fightModeData[i].fightSequenceData.Length; ++j){

                    //Assign sequence list elements with a random if there is no sequence that is set as default
                    int fightComboSequenceDataIndex = 0;

                    for (byte k = 0; k < _fightModeData[i].fightSequenceData[j].fightComboSequenceData.Length; ++k)
                    {
                        if (!_fightModeData[i].fightSequenceData[j].fightComboSequenceData[i].isDefault)
                            continue;

                        fightComboSequenceDataIndex = i;

                        //Assigns the combo sequence that is assigned as the default
                        defaultFightComboSequenceData = new FightComboSequenceData[featureManager.GetFightSequenceDatas(featureManager.GetEnemyBotType(i))[j].fightComboSequenceData.Length];

                        for (byte l = 0; l < defaultFightComboSequenceData.Length; ++l)
                            defaultFightComboSequenceData[l] = featureManager.GetFightSequenceDatas(featureManager.GetEnemyBotType(i))[j].fightComboSequenceData[l];

                        defaultFightComboSequenceData = new FightComboSequenceData[1];
                        defaultFightComboSequenceData[0] = featureManager.GetFightSequenceDatas(featureManager.GetEnemyBotType(i))[j].fightComboSequenceData[fightComboSequenceDataIndex];

                        _fightModeData[i].fightSequenceData[j].fightComboSequenceData = defaultFightComboSequenceData;

                        _nbrOfEnemyBotOffenseBlocking = GetBlockingOffenseCount();

                        return;
                    }

                    byte currentComboSequenceDataIndex = (byte)random.Next(_fightModeData[i].fightSequenceData[j].fightComboSequenceData.Length);

                    while (comboSequenceDataIndex.Contains(currentComboSequenceDataIndex))
                        currentComboSequenceDataIndex = (byte)random.Next(_fightModeData[i].fightSequenceData[j].fightComboSequenceData.Length);

                    comboSequenceDataIndex.Add(currentComboSequenceDataIndex);

                    _fightModeData[i].fightSequenceData[j].fightComboSequenceData[currentComboSequenceDataIndex] = featureManager.GetFightSequenceDatas(featureManager.GetEnemyBotType(i))[j].fightComboSequenceData[fightComboSequenceDataIndex];

                    _nbrOfEnemyBotOffenseBlocking = GetBlockingOffenseCount();

                    return;

                }

            }

        }

        public override bool OnUpdate(bool pIsLeftFocus, bool pIsRightFocus)
        {
            if (!base.OnUpdate())
                return false;

            //Checks if combo sequences have been configured
            if (_fightModeData.Length == 0)
                return true;

            if (GetFightOffenseData.Length == 0)
                return true;

            //Suspends the management of Offense combos if HitConfirm is activated
            if (featureManager.GetHitConfirmModule.GetIsHitConfirmActivated) 
            {
                _currentWaithingTime = 0;

                return true;
            }

            if (featureManager.GetStateConfirmModule.GetIsEnemyBotOnStaggerMode) {

                _currentWaithingTime = featureManager.GetSpecificBotAnimationClipByType(featureManager.GetCurrentEnemyBotType).length;                

                return true;
            }

            //Assigns all the correct information if the Focus has been changed
            if (featureManager.GetFocusModule.GetIsEnemyBotFocusChanged){

                //Allows the assignment of the index of the Bot which is in Focus with the player
                for (int i = 0; i < _fightModeData.Length; ++i)
                {
                    //Checks if the Bot that is assigned as Focus matches the one in the list
                    if (_fightModeData[i].currentEnemyBotIndex != featureManager.GetFocusModule.GetCurrentEnemyBotIndex)
                        continue;

                    //Assigns FightMode index based on enemy Bot
                    if (_currentFightModeDataIndex != i)
                        _currentFightModeDataIndex = i;

                    break;
                }

                //Allows the assignment of the index that corresponds to the bot's combo sequence
                for (int i = 0; i < GetFightComboSequenceData.Length; ++i)
                {
                    //Checks if the combo sequence has already been completed
                    if (GetFightComboSequenceData[i].isCompleted)
                        continue;

                    _currentFightComboSequenceDataIndex = i;

                    break;
                }

                //Assignment of the FightOffenseData index of the enemy Bot in Focus
                for (int i = 0; i < GetFightOffenseData.Length; ++i)
                {
                    //Checks if the FightOffenseData of the enemy Bot in Focus has already been completed
                    if (GetFightOffenseData[i].isCompleted)
                        continue;

                    _currentFightOffenseDataIndex = i;

                    break;
                }

                //Apply the Offense that should be played on the new enemy Bot
                ApplyOffense();
            }

            //Assigns the new Offense when the time limit ends
            _currentWaithingTime += Time.deltaTime;

            //Assigns the same clip again if its normalized time has exceeded the percentage desired in parameter
            if (_currentWaithingTime >= GetCurrentMaxWaithingTimer){

                if (featureManager.GetSpecificOffenseManagerBotByType(featureManager.GetCurrentEnemyBotType).GetCurrentOffense)
                    featureManager.GetSpecificBotAnimatorByType(featureManager.GetCurrentEnemyBotType).Play(GetCurrentOffenseData().offense.GetAnimationClip(AnimationClipOffenseType.Full).name);
            }

            //Returns that the wait time has been reached
            if (_currentWaithingTime >= featureManager.GetSpecificBotAnimationClipByType(featureManager.GetCurrentEnemyBotType).length){
                
                _currentWaithingTime = 0;

                if (GetFightOffenseData.Length != 0)
                    GetFightOffenseData[_currentFightOffenseDataIndex].isCompleted = true;

                ++_currentFightOffenseDataIndex;

                if (_currentFightOffenseDataIndex > GetFightOffenseData.Length - 1){

                    _fightModeData[_currentFightModeDataIndex].fightSequenceData[_currentFightOffenseSequenceIndex].fightComboSequenceData[_currentFightComboSequenceDataIndex].isCompleted = true;

                    ++_currentFightComboSequenceDataIndex;

                    _currentFightOffenseDataIndex = 0;

                    _nbrOfEnemyBotOffenseBlocking = GetBlockingOffenseCount();
                }

                //Security that allows the FightComboSequenceData index to be increased based on the total number
                if (_currentFightComboSequenceDataIndex > GetFightComboSequenceData.Length - 1){

                    _currentFightComboSequenceDataIndex = 0;
                    _currentFightOffenseDataIndex = 0;

                    DefaultFightModeSetup();
                }

                ApplyOffense();
            }

            return true;
        }

        public override void OnEnabled(){
            base.OnEnabled();

            _currentFightModeDataIndex = 0;
            _currentFightComboSequenceDataIndex = 0;
            _currentFightOffenseDataIndex = 0;
        }

        public override void OnDisabled()
        {
            base.OnDisabled();

            DefaultFightModeSetup();
        }

        /// <summary>
        /// Allows you to assign all the necessary information as well as the next Offense of a Bot
        /// </summary>
        void ApplyOffense()
        {
            //Allows the assignment of the same Offense as the previous one
            if (featureManager.GetSpecificBotAnimationClipByType(featureManager.GetCurrentEnemyBotType).name == GetCurrentOffenseData().offense.GetAnimationClip(AnimationClipOffenseType.Full).name)
            {
                if (_currentWaithingTime >= GetCurrentMaxWaithingTimer) 
                    featureManager.GetSpecificBotAnimatorByType(featureManager.GetCurrentEnemyBotType).Play(GetCurrentOffenseData().offense.GetAnimationClip(AnimationClipOffenseType.Full).name, -1, 0);                    

                return;
            }

            //Apply the next Offense to the enemy Bot
            featureManager.GetSpecificOffenseManagerBotByType(featureManager.GetCurrentEnemyBotType).AssignCurrentOffense(GetCurrentOffenseData().offense.GetAnimationClip(AnimationClipOffenseType.Full).name);
            featureManager.GetSpecificBotAnimatorByType(featureManager.GetCurrentEnemyBotType).Play(GetCurrentOffenseData().offense.GetAnimationClip(AnimationClipOffenseType.Full).name);
        }

        /// <summary>
        /// Assign all default FightCombo Sequence information as well as the FightOffenseData of the enemy Bot present
        /// </summary>
        void DefaultFightModeSetup()
        {
            if (_fightModeData.Length == 0)
                return;

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

        #endregion
    }

#if UNITY_EDITOR

    [CustomPropertyDrawer(typeof(FightSequencerModule))]
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

                if (offense.GetOffenseType != OffenseType.STANCE)
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