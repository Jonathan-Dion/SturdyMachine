using System;
using UnityEngine;

using SturdyMachine.Features.HitConfirm;
using SturdyMachine.Component;
using SturdyMachine.Offense;

#if UNITY_EDITOR
using UnityEditor;
using NWH.NUI;
using NWH.VehiclePhysics2;
#endif

namespace SturdyMachine.Features.StateConfirm {

    /// <summary>
    /// Allows you to remember important combat information for each bot
    /// </summary>
    [Serializable, Tooltip("Allows you to remember important combat information for each bot")]
    public struct StateBotData {

        public float damageIntensity;

        public StateConfirmMode stateConfirmMode;
    }

    /// <summary>
    /// Handles animations that should be played after HitConfirm and handles taunting enemyBots when in Stagger
    /// </summary>
    [Serializable, Tooltip("Handles animations that should be played after HitConfirm and handles taunting enemyBots when in Stagger")]
    public partial class  StateConfirmModule : FeatureModule
    {
        #region Attributes

        [SerializeField]
        StateBotData _sturdyStateBotData;

        StateBotData[] _enemyBotStateBotData;

        /// <summary>
        /// Represents the list of blocked moves in an enemy bot's combo sequence
        /// </summary>
        [SerializeField, Tooltip("Represents the list of blocked moves in an enemy bot's cobo sequence")]
        bool[] _isBlockingSequenceComboOffense;

        /// <summary>
        /// Represents the blocking index of combo sequence offenses
        /// </summary>
        [SerializeField, Tooltip("Represents the blocking index of combo sequence offenses")]
        byte _currentBlockingSequenceComboOffense;

        System.Random _blockingEnemyBotRandomChance;

        #endregion

        #region Properties

        public override FeatureModuleCategory GetFeatureModuleCategory() => FeatureModuleCategory.StateConfirm;

        public StateBotData GetSturdyStateBotData => _sturdyStateBotData;

        /// <summary>
        /// Checks the number of offenses blocked in the current combo sequence
        /// </summary>
        /// <returns>Returns whether all offenses in the combo sequence have been blocked</returns>
        bool GetIsCompletedBlockingSequenceComboOffense()
        {

            for (byte i = 0; i < _isBlockingSequenceComboOffense.Length; ++i)
            {

                if (_isBlockingSequenceComboOffense[i])
                    continue;

                return false;
            }

            return true;
        }

        bool GetIsSturdyBotOnBlockingMode => FEATURE_MANAGER.GetHitConfirmModule.GetIsSturdyBotOnBlockingMode;

        public bool GetIsCurrentEnemyBotOnBlockingMode => _enemyBotStateBotData[FEATURE_MANAGER.GetFocusModule.GetCurrentEnemyBotIndex].stateConfirmMode == StateConfirmMode.Blocking;

        StateBotData GetDefendingBotData {

            get {

                //Sturdy
                if (FEATURE_MANAGER.GetHitConfirmModule.GetDefendingBotType == BotType.SturdyBot)
                    return _sturdyStateBotData;

                //EnemyBot
                return _enemyBotStateBotData[FEATURE_MANAGER.GetFocusModule.GetCurrentEnemyBotIndex];
            }
        }

        Offense.Offense GetBlockingOffense => FEATURE_MANAGER.GetHitConfirmModule.GetDefendingHitConfirmBlockingData().blockingOffense;

        AnimationClip GetAttackerAnimationClipOnHitConfirm
        {
            get
            {
                //SturdyBot
                if (FEATURE_MANAGER.GetHitConfirmModule.GetAttackerBotType == BotType.SturdyBot)
                    return FEATURE_MANAGER.GetSpecificOffenseManagerBotByType(FEATURE_MANAGER.GetHitConfirmModule.GetAttackerBotType).GetCurrentOffense.GetAnimationClip(AnimationClipOffenseType.KeyposeOut);

                //EnemyBot
                if (!GetIsCompletedBlockingSequenceComboOffense())
                    return FEATURE_MANAGER.GetSpecificOffenseManagerBotByType(FEATURE_MANAGER.GetHitConfirmModule.GetAttackerBotType).GetCurrentOffense.GetAnimationClip(AnimationClipOffenseType.KeyposeOut);

                return FEATURE_MANAGER.GetSpecificOffenseManagerBotByType(FEATURE_MANAGER.GetHitConfirmModule.GetAttackerBotType).GetCurrentOffense.GetAnimationClip(AnimationClipOffenseType.Stagger);
            }
        }

        #endregion

        #region Methods

        public override void Initialize(FeatureManager pFeatureManager)
        {
            base.Initialize(pFeatureManager);

            _enemyBotStateBotData = new StateBotData[FEATURE_MANAGER.GetEnemyBotObject.Length];

            _blockingEnemyBotRandomChance = new System.Random();
        }

        public override bool OnUpdate(bool pIsLeftFocus, bool pIsRightFocus)
        {
            if (!base.OnUpdate())
                return false;

            if (!FEATURE_MANAGER.GetHitConfirmModule.GetIsHitConfirmActivated)
            {
                _sturdyStateBotData.stateConfirmMode = StateConfirmMode.None;

                if (!FEATURE_MANAGER.GetHitConfirmModule.GetDefendingHitConfirmBlockingData().Equals(new HitConfirmBlockingData()))
                {
                    if (_isBlockingSequenceComboOffense.Length != FEATURE_MANAGER.GetFightsModule.GetNbrOfEnemyBotOffenseBlocking)
                        _isBlockingSequenceComboOffense = new bool[FEATURE_MANAGER.GetFightsModule.GetNbrOfEnemyBotOffenseBlocking];

                    return true;
                }

                return true;
            }

            SturdyStateBotData();
            EnemyStateBotData();

            ActivateAttackerBotAnimationState();
            ActivateDefendingBotAnimationState();

            return true;
        }

        void SturdyStateBotData() {

            if (FEATURE_MANAGER.GetHitConfirmModule.GetDefendingBotType != BotType.SturdyBot)
                return;

            if (_sturdyStateBotData.stateConfirmMode != StateConfirmMode.None)
                return;

            //Blocking
            if (GetIsSturdyBotOnBlockingMode)
            {
                _isBlockingSequenceComboOffense[_currentBlockingSequenceComboOffense] = true;

                _sturdyStateBotData.stateConfirmMode = StateConfirmMode.Blocking;
            }

            //Hitting
            else
                _sturdyStateBotData.stateConfirmMode = StateConfirmMode.Hitting;

            ++_currentBlockingSequenceComboOffense;

            if (_currentBlockingSequenceComboOffense == _isBlockingSequenceComboOffense.Length) {

                if (GetIsCompletedBlockingSequenceComboOffense()) {

                    _sturdyStateBotData.stateConfirmMode = StateConfirmMode.Parry;
                    _enemyBotStateBotData[FEATURE_MANAGER.GetFocusModule.GetCurrentEnemyBotIndex].stateConfirmMode = StateConfirmMode.Stagger;

                    _isBlockingSequenceComboOffense = new bool[0];
                }

                _currentBlockingSequenceComboOffense = 0;
                SetDefaultBlockingSequenceComboOffenseValue();
            }
        }

        void SetDefaultBlockingSequenceComboOffenseValue() {

            for (byte i = 0; i < _isBlockingSequenceComboOffense.Length; ++i)
                _isBlockingSequenceComboOffense[i] = false;
        }

        void EnemyStateBotData() {

            if (FEATURE_MANAGER.GetHitConfirmModule.GetDefendingBotType == BotType.SturdyBot)
                return;

            if (_enemyBotStateBotData[FEATURE_MANAGER.GetFocusModule.GetCurrentEnemyBotIndex].stateConfirmMode == StateConfirmMode.Stagger)
                return;

            if (_blockingEnemyBotRandomChance.Next(0, 100) > 50)
                _enemyBotStateBotData[FEATURE_MANAGER.GetFocusModule.GetCurrentEnemyBotIndex].stateConfirmMode = StateConfirmMode.Blocking;
            else
                _enemyBotStateBotData[FEATURE_MANAGER.GetFocusModule.GetCurrentEnemyBotIndex].stateConfirmMode = StateConfirmMode.Hitting;
        }

        void ActivateDefendingBotAnimationState() {

            //Allows you to assign the Offense that corresponds to the Bot that should be block
            if (GetDefendingBotData.stateConfirmMode == StateConfirmMode.Blocking) {

                FEATURE_MANAGER.GetSpecificOffenseManagerBotByType(FEATURE_MANAGER.GetHitConfirmModule.GetDefendingBotType).CurrentOffenseClipNameSetup(FEATURE_MANAGER.GetSpecificOffenseManagerBotByType(FEATURE_MANAGER.GetHitConfirmModule.GetDefendingBotType).GetOffense(GetBlockingOffense.GetOffenseType, GetBlockingOffense.GetOffenseDirection).GetAnimationClip(FEATURE_MANAGER.GetHitConfirmModule.GetDefendingBotType == BotType.SturdyBot ? AnimationClipOffenseType.KeyposeOut : AnimationClipOffenseType.Full).name);

                FEATURE_MANAGER.GetSpecificBotAnimatorByType(FEATURE_MANAGER.GetHitConfirmModule.GetDefendingBotType).Play(FEATURE_MANAGER.GetSpecificOffenseManagerBotByType(FEATURE_MANAGER.GetHitConfirmModule.GetDefendingBotType).GetCurrentOffense.GetAnimationClip(FEATURE_MANAGER.GetHitConfirmModule.GetDefendingBotType == BotType.SturdyBot ? AnimationClipOffenseType.KeyposeOut : AnimationClipOffenseType.Full).name);

                return;
            }

            //Allows you to assign the Offense that corresponds to the Bot that should be parried
            if (GetDefendingBotData.stateConfirmMode == StateConfirmMode.Parry){

                FEATURE_MANAGER.GetSpecificBotAnimatorByType(BotType.SturdyBot).Play(FEATURE_MANAGER.GetSpecificOffenseManagerBotByType(BotType.SturdyBot).GetCurrentOffense.GetAnimationClip(AnimationClipOffenseType.Parry).name);

                FEATURE_MANAGER.GetSpecificBotAnimatorByType(FEATURE_MANAGER.GetHitConfirmModule.GetDefendingBotType).Play(FEATURE_MANAGER.GetSpecificOffenseManagerBotByType(BotType.SturdyBot).GetCurrentOffense.GetAnimationClip(AnimationClipOffenseType.Parry).name);

                return;
            }

            //Allows you to assign the Offense that corresponds to the Bot that should be hit
            FEATURE_MANAGER.GetSpecificOffenseManagerBotByType(FEATURE_MANAGER.GetHitConfirmModule.GetDefendingBotType).CurrentOffenseClipNameSetup(FEATURE_MANAGER.GetSpecificOffenseManagerBotByType(FEATURE_MANAGER.GetHitConfirmModule.GetDefendingBotType).GetOffense(OffenseType.DAMAGEHIT, GetBlockingOffense.GetOffenseDirection).GetAnimationClip(AnimationClipOffenseType.Full).name);

            if (FEATURE_MANAGER.GetSpecificBotAnimationClipByType(FEATURE_MANAGER.GetHitConfirmModule.GetDefendingBotType) != FEATURE_MANAGER.GetSpecificOffenseManagerBotByType(FEATURE_MANAGER.GetHitConfirmModule.GetDefendingBotType).GetCurrentOffense.GetAnimationClip(AnimationClipOffenseType.Full))
                FEATURE_MANAGER.GetSpecificBotAnimatorByType(FEATURE_MANAGER.GetHitConfirmModule.GetDefendingBotType).Play(FEATURE_MANAGER.GetSpecificOffenseManagerBotByType(FEATURE_MANAGER.GetHitConfirmModule.GetDefendingBotType).GetCurrentOffense.GetAnimationClip(AnimationClipOffenseType.Full).name);
            else
                FEATURE_MANAGER.GetSpecificBotAnimatorByType(FEATURE_MANAGER.GetHitConfirmModule.GetDefendingBotType).Play(FEATURE_MANAGER.GetSpecificOffenseManagerBotByType(FEATURE_MANAGER.GetHitConfirmModule.GetDefendingBotType).GetCurrentOffense.GetAnimationClip(AnimationClipOffenseType.Full).name, -1, 0f);
        }

        void ActivateAttackerBotAnimationState() {

            //Allows the assignment of the Offense that corresponds to the attacking Bot
            FEATURE_MANAGER.GetSpecificBotAnimatorByType(FEATURE_MANAGER.GetHitConfirmModule.GetAttackerBotType).Play(GetAttackerAnimationClipOnHitConfirm.name);
            FEATURE_MANAGER.GetSpecificOffenseManagerBotByType(FEATURE_MANAGER.GetHitConfirmModule.GetAttackerBotType).CurrentOffenseClipNameSetup(GetAttackerAnimationClipOnHitConfirm.name);
        }

        #endregion
    }

#if UNITY_EDITOR

    [CustomPropertyDrawer(typeof(StateConfirmModule))]
    public partial class StateConfirmModuleDrawer : FeatureModuleDrawer
    {
        public override bool OnNUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (!base.OnNUI(position, property, label))
                return false;

            drawer.BeginSubsection("Debug Value");

            GUI.enabled = false;

            drawer.ReorderableList("_isBlockingSequenceComboOffense");            

            GUI.enabled = true;

            drawer.Field("_currentBlockingSequenceComboOffense", false);

            drawer.Field("_sturdyStateBotData");

            drawer.EndSubsection();

            drawer.EndProperty();
            return true;
        }
    }

    [CustomPropertyDrawer(typeof(StateBotData))]
    public partial class StateBotDataDrawer : ComponentNUIPropertyDrawer
    {
        public override bool OnNUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (!base.OnNUI(position, property, label))
                return false;

            drawer.Field("damageIntensity", false);
            drawer.Field("stateConfirmMode", false);

            drawer.EndProperty();
            return true;
        }
    }

#endif
}