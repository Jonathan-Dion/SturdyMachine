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
        byte _currentBlockingSequenceComboOffense, _lastcurrentBlockingSequenceComboOffense;

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

        public bool GetIsCurrentEnemyBotOnBlockingMode => _enemyBotStateBotData[GetCurrentEnemyBotIndex].stateConfirmMode == StateConfirmMode.Blocking;

        StateBotData GetDefendingBotData {

            get {

                //Sturdy
                if (FEATURE_MANAGER.GetHitConfirmModule.GetDefendingBotType == BotType.SturdyBot)
                    return _sturdyStateBotData;

                //EnemyBot
                return _enemyBotStateBotData[GetCurrentEnemyBotIndex];
            }
        }

        Offense.Offense GetBlockingOffense => FEATURE_MANAGER.GetHitConfirmModule.GetDefendingHitConfirmBlockingData().blockingOffense;

        AnimationClip GetAttackerAnimationClipOnHitConfirm
        {
            get
            {
                //SturdyBot
                if (GetAttackerBotType == BotType.SturdyBot)
                    return FEATURE_MANAGER.GetSpecificOffenseManagerBotByType(GetAttackerBotType).GetCurrentOffense.GetAnimationClip(AnimationClipOffenseType.KeyposeOut);

                //EnemyBot
                if (!GetIsCompletedBlockingSequenceComboOffense())
                    return FEATURE_MANAGER.GetSpecificOffenseManagerBotByType(GetAttackerBotType).GetCurrentOffense.GetAnimationClip(AnimationClipOffenseType.KeyposeOut);

                return FEATURE_MANAGER.GetSpecificOffenseManagerBotByType(GetAttackerBotType).GetCurrentOffense.GetAnimationClip(AnimationClipOffenseType.Stagger);
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

            if (!GetIsHitConfirmActivated)
            {
                if (!FEATURE_MANAGER.GetHitConfirmModule.GetDefendingHitConfirmBlockingData().Equals(new HitConfirmBlockingData()))
                {
                    if (_isBlockingSequenceComboOffense.Length != FEATURE_MANAGER.GetFightsModule.GetNbrOfEnemyBotOffenseBlocking)
                        _isBlockingSequenceComboOffense = new bool[FEATURE_MANAGER.GetFightsModule.GetNbrOfEnemyBotOffenseBlocking];

                    return true;
                }

                if (GetDefendingBotType == Component.BotType.SturdyBot)
                {
                    _currentBlockingSequenceComboOffense = 0;

                    _isBlockingSequenceComboOffense = new bool[0];
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

            //Blocking
            if (GetIsSturdyBotOnBlockingMode)
            {
                _isBlockingSequenceComboOffense[_currentBlockingSequenceComboOffense] = true;

                _sturdyStateBotData.stateConfirmMode = StateConfirmMode.Blocking;
            }

            //Hitting
            else
                _sturdyStateBotData.stateConfirmMode = StateConfirmMode.Hitting;

            if (_lastcurrentBlockingSequenceComboOffense != _currentBlockingSequenceComboOffense)
            {

                ++_currentBlockingSequenceComboOffense;

                _lastcurrentBlockingSequenceComboOffense = _currentBlockingSequenceComboOffense;
            }

            if (_currentBlockingSequenceComboOffense == _isBlockingSequenceComboOffense.Length) {

                if (GetIsCompletedBlockingSequenceComboOffense()) {

                    _sturdyStateBotData.stateConfirmMode = StateConfirmMode.Parry;
                    _enemyBotStateBotData[GetCurrentEnemyBotIndex].stateConfirmMode = StateConfirmMode.Stagger;

                    FEATURE_MANAGER.GetSpecificBotAnimatorByType(BotType.SturdyBot).Play(FEATURE_MANAGER.GetSpecificOffenseManagerBotByType(BotType.SturdyBot).GetCurrentOffense.GetAnimationClip(AnimationClipOffenseType.Parry).name);
                }
            }
        }

        void EnemyStateBotData() {

            if (FEATURE_MANAGER.GetHitConfirmModule.GetDefendingBotType == BotType.SturdyBot)
                return;

            if (_enemyBotStateBotData[GetCurrentEnemyBotIndex].stateConfirmMode == StateConfirmMode.Stagger)
                return;

            if (_blockingEnemyBotRandomChance.Next(0, 100) > 50)
                _enemyBotStateBotData[GetCurrentEnemyBotIndex].stateConfirmMode = StateConfirmMode.Blocking;
            else
                _enemyBotStateBotData[GetCurrentEnemyBotIndex].stateConfirmMode = StateConfirmMode.Hitting;
        }

        void ActivateDefendingBotAnimationState() {

            //Allows you to assign the Offense that corresponds to the Bot that should be hit
            if (GetDefendingBotData.stateConfirmMode == StateConfirmMode.Hitting)
            {
                FEATURE_MANAGER.GetSpecificOffenseManagerBotByType(GetDefendingBotType).CurrentOffenseClipNameSetup(FEATURE_MANAGER.GetSpecificOffenseManagerBotByType(GetDefendingBotType).GetOffense(OffenseType.DAMAGEHIT, GetBlockingOffense.GetOffenseDirection).GetAnimationClip(AnimationClipOffenseType.Full).name);

                if (FEATURE_MANAGER.GetSpecificBotAnimationClipByType(GetDefendingBotType) != FEATURE_MANAGER.GetSpecificOffenseManagerBotByType(GetDefendingBotType).GetCurrentOffense.GetAnimationClip(AnimationClipOffenseType.Full))
                    FEATURE_MANAGER.GetSpecificBotAnimatorByType(GetDefendingBotType).Play(FEATURE_MANAGER.GetSpecificOffenseManagerBotByType(GetDefendingBotType).GetCurrentOffense.GetAnimationClip(AnimationClipOffenseType.Full).name);
                else
                    FEATURE_MANAGER.GetSpecificBotAnimatorByType(GetDefendingBotType).Play(FEATURE_MANAGER.GetSpecificOffenseManagerBotByType(GetDefendingBotType).GetCurrentOffense.GetAnimationClip(AnimationClipOffenseType.Full).name, -1, 0f);

                return;
            }

            //Allows you to assign the Offense that corresponds to the Bot that should be block
            FEATURE_MANAGER.GetSpecificOffenseManagerBotByType(GetDefendingBotType).CurrentOffenseClipNameSetup(FEATURE_MANAGER.GetSpecificOffenseManagerBotByType(GetDefendingBotType).GetOffense(GetBlockingOffense.GetOffenseType, GetBlockingOffense.GetOffenseDirection).GetAnimationClip(GetDefendingBotType == BotType.SturdyBot ? AnimationClipOffenseType.KeyposeOut : AnimationClipOffenseType.Full).name);

            FEATURE_MANAGER.GetSpecificBotAnimatorByType(GetDefendingBotType).Play(FEATURE_MANAGER.GetSpecificOffenseManagerBotByType(GetDefendingBotType).GetCurrentOffense.GetAnimationClip(GetDefendingBotType == BotType.SturdyBot ? AnimationClipOffenseType.KeyposeOut : AnimationClipOffenseType.Full).name);
        }

        void ActivateAttackerBotAnimationState() {

            //Allows the assignment of the Offense that corresponds to the attacking Bot
            FEATURE_MANAGER.GetSpecificBotAnimatorByType(GetAttackerBotType).Play(GetAttackerAnimationClipOnHitConfirm.name);
            FEATURE_MANAGER.GetSpecificOffenseManagerBotByType(GetAttackerBotType).CurrentOffenseClipNameSetup(GetAttackerAnimationClipOnHitConfirm.name);

        }

        #endregion
    }
}