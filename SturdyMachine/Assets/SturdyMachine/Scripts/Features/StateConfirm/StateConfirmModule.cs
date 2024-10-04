using System;
using UnityEngine;

using SturdyMachine.Settings.GameplaySettings.StateConfirmSettings;
using SturdyMachine.Settings.GameplaySettings;
using SturdyMachine.Settings;
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

        [SerializeField]
        float _currentBlockingChance;

        float _currentRandomBlockingChance;

        bool _isStaggerTauntActivated;

        float _currentStaggerTauntTime;

        AnimationClipOffenseType _enemyAnimationClipOffenseType;

        #endregion

        #region Properties

        public override FeatureModuleCategory GetFeatureModuleCategory() => FeatureModuleCategory.StateConfirm;

        public StateBotData GetSturdyStateBotData => _sturdyStateBotData;

        public bool GetIsCurrentEnemyBotOnBlockingMode => _enemyBotStateBotData[featureManager.GetCurrentEnemyBotIndex].stateConfirmMode == StateConfirmMode.Blocking;

        public StateBotData GetDefendingBotData {

            get {

                //Sturdy
                if (GetHitConfirmModule.GetDefendingBotType == BotType.SturdyBot)
                    return _sturdyStateBotData;

                //EnemyBot
                return _enemyBotStateBotData[featureManager.GetCurrentEnemyBotIndex];
            }
        }

        AnimationClip GetAttackerAnimationClipOnHitConfirm
        {
            get
            {
                //SturdyBot
                if (GetHitConfirmModule.GetAttackerBotType == BotType.SturdyBot)
                    return featureManager.GetAttackerBotOffenseManager.GetCurrentOffense.GetAnimationClip(AnimationClipOffenseType.KeyposeOut);

                //EnemyBot
                //Checks the number of offenses blocked in the current combo sequence
                for (byte i = 0; i < _isBlockingSequenceComboOffense.Length; ++i){
                    
                    if (_isBlockingSequenceComboOffense[i])
                        continue;

                    return featureManager.GetAttackerBotOffenseManager.GetCurrentOffense.GetAnimationClip(AnimationClipOffenseType.KeyposeOut);
                }

                return featureManager.GetAttackerBotOffenseManager.GetCurrentOffense.GetAnimationClip(AnimationClipOffenseType.Stagger);
            }
        }

        public bool GetIsEnemyBotOnStaggerMode {

            get {

                if (_isStaggerTauntActivated)
                    return true;

                return GetEnemyBotStateConfirmMode == StateConfirmMode.Stagger;
            }
        }

        public StateConfirmMode GetEnemyBotStateConfirmMode => _enemyBotStateBotData[featureManager.GetCurrentEnemyBotIndex].stateConfirmMode;

        HitConfirmModule GetHitConfirmModule => featureManager.GetHitConfirmModule;

        AnimationClipOffenseType GetCurrentBlockingAnimationClipOffenseType 
        {
            get 
            {
                //Sturdy
                if (GetHitConfirmModule.GetDefendingBotType == BotType.SturdyBot)
                    return AnimationClipOffenseType.KeyposeOut;

                //EnemyBot
                return AnimationClipOffenseType.Full;
            }
        }

        public AnimationClipOffenseType GetEnemyAnimationClipOffenseType => _enemyAnimationClipOffenseType;

        float GetCurrentPercentBlockingChance => Mathf.Abs(100f - _currentBlockingChance);

        BlockingChanceData GetCurrentEnemyBlockingChanceData => GameSettings.GetGameSettings().GetGameplaySettings.GetStateConfirmSettings.GetCurrentBlockingChanceData(BotType.SkinnyBot);

        #endregion

        #region Methods

        public override void Initialize(FeatureManager pFeatureManager)
        {
            base.Initialize(pFeatureManager);

            _enemyBotStateBotData = new StateBotData[featureManager.GetEnemyBotObject.Length];

            _blockingEnemyBotRandomChance = new System.Random();

            _currentBlockingChance = GetCurrentEnemyBlockingChanceData.minBlockingChange;
        }

        public override bool OnUpdate(bool pIsLeftFocus, bool pIsRightFocus, bool pIsGoodOffenseDirection)
        {
            if (!base.OnUpdate())
                return false;

            if (GetEnemyBotStateConfirmMode == StateConfirmMode.Stagger) {

                if (featureManager.GetSpecificAnimatorStateInfoByBotType(GetHitConfirmModule.GetAttackerBotType).normalizedTime > 1)
                    _enemyBotStateBotData[featureManager.GetCurrentEnemyBotIndex].stateConfirmMode = StateConfirmMode.None;
            }

            NADTime.NADTime.Update(featureManager.GetPlayerBotOffenseManager.GetCurrentOffense.GetOffenseIsInAttackMode, _sturdyStateBotData.stateConfirmMode, 
                _enemyBotStateBotData[featureManager.GetCurrentEnemyBotIndex].stateConfirmMode, 
                featureManager.GetHitConfirmModule.GetIsGoodOffenseDirection(featureManager.GetEnemyBotFocusedOffenseManager.GetCurrentOffense.GetOffenseDirection, featureManager.GetPlayerBotOffenseManager.GetCurrentOffense.GetOffenseDirection, featureManager.GetPlayerBotOffenseManager.GetCurrentOffense));

            if (!GetHitConfirmModule.GetIsHitConfirmActivated)
            {
                _enemyAnimationClipOffenseType = AnimationClipOffenseType.Full;

                _sturdyStateBotData.stateConfirmMode = StateConfirmMode.None;

                if (GetEnemyBotStateConfirmMode != StateConfirmMode.Stagger)
                    _enemyBotStateBotData[featureManager.GetCurrentEnemyBotIndex].stateConfirmMode = StateConfirmMode.None;

                if (!GetHitConfirmModule.GetDefendingHitConfirmBlockingData().Equals(new HitConfirmBlockingData()))
                {
                    if (_isBlockingSequenceComboOffense.Length != featureManager.GetFightsModule.GetNbrOfEnemyBotOffenseBlocking)
                        _isBlockingSequenceComboOffense = new bool[featureManager.GetFightsModule.GetNbrOfEnemyBotOffenseBlocking];

                    return true;
                }

                if (_isStaggerTauntActivated) {

                    if (_currentStaggerTauntTime == 0f) {

                        if (featureManager.GetSpecificBotAnimationClipByType(BotType.SkinnyBot) != GameplaySettings.GetGameplaySettings().GetStateConfirmSettings.GetStaggerStateData.stunAnimationClip)
                        {
                            if (featureManager.GetSpecificAnimatorStateInfoByBotType(BotType.SkinnyBot).normalizedTime > 0.98)
                                featureManager.GetSpecificBotAnimatorByType(BotType.SkinnyBot).Play(GameplaySettings.GetGameplaySettings().GetStateConfirmSettings.GetStaggerStateData.stunAnimationClip.name);
                        }
                    }

                    _currentStaggerTauntTime += Time.deltaTime;

                    if (_currentStaggerTauntTime > GameplaySettings.GetGameplaySettings().GetStateConfirmSettings.GetStaggerStateData.maxStunTimer)
                    {

                        _currentStaggerTauntTime = 0;
                        _isStaggerTauntActivated = false;
                    }
                    else if (GameplaySettings.GetGameplaySettings().GetStateConfirmSettings.GetStaggerStateData.maxStunTimer - _currentStaggerTauntTime <= GameplaySettings.GetGameplaySettings().GetStateConfirmSettings.GetStaggerStateData.recoveryStunAnimationClip.length)
                        featureManager.GetSpecificBotAnimatorByType(BotType.SkinnyBot).Play(GameplaySettings.GetGameplaySettings().GetStateConfirmSettings.GetStaggerStateData.recoveryStunAnimationClip.name);

                    else {

                        if (featureManager.GetSpecificAnimatorStateInfoByBotType(BotType.SkinnyBot).normalizedTime > 0.98)
                            featureManager.GetSpecificBotAnimatorByType(BotType.SkinnyBot).Play(GameplaySettings.GetGameplaySettings().GetStateConfirmSettings.GetStaggerStateData.stunAnimationClip.name, -1, 0);
                    }

                    return true;
                }

                return true;
            }

            #region BotData

            _enemyAnimationClipOffenseType = AnimationClipOffenseType.KeyposeOut;

            //Sturdy
            if (GetHitConfirmModule.GetDefendingBotType == BotType.SturdyBot)
            {

                if (_sturdyStateBotData.stateConfirmMode == StateConfirmMode.None)
                {

                    //Blocking
                    if (GetHitConfirmModule.GetIsSturdyBotOnBlockingMode)
                    {

                        _sturdyStateBotData.stateConfirmMode = StateConfirmMode.Blocking;

                        _isBlockingSequenceComboOffense[_currentBlockingSequenceComboOffense] = true;
                    }

                    //Hitting
                    else
                        _sturdyStateBotData.stateConfirmMode = StateConfirmMode.Hitting;

                    if (_sturdyStateBotData.stateConfirmMode == StateConfirmMode.Blocking)
                        _currentBlockingChance -= GetCurrentEnemyBlockingChanceData.maxSubstractiveBlockingChance;
                    else if (_sturdyStateBotData.stateConfirmMode == StateConfirmMode.Hitting)
                        _currentBlockingChance -= GetCurrentEnemyBlockingChanceData.minSubtractiveBlockingChance;

                    if (_currentBlockingChance < GetCurrentEnemyBlockingChanceData.minBlockingChange)
                        _currentBlockingChance = GetCurrentEnemyBlockingChanceData.minBlockingChange;

                    ++_currentBlockingSequenceComboOffense;

                    if (_currentBlockingSequenceComboOffense == _isBlockingSequenceComboOffense.Length)
                    {

                        for (byte i = 0; i < _isBlockingSequenceComboOffense.Length; ++i)
                        {

                            if (!_isBlockingSequenceComboOffense[i])
                                break;

                            if (i < _isBlockingSequenceComboOffense.Length - 1)
                                continue;

                            _sturdyStateBotData.stateConfirmMode = StateConfirmMode.Parry;
                            _enemyBotStateBotData[featureManager.GetCurrentEnemyBotIndex].stateConfirmMode = StateConfirmMode.Stagger;

                            _isBlockingSequenceComboOffense = new bool[0];
                        }

                        _currentBlockingSequenceComboOffense = 0;

                        for (byte i = 0; i < _isBlockingSequenceComboOffense.Length; ++i)
                            _isBlockingSequenceComboOffense[i] = false;
                    }
                }
            }

            //Enemy
            else if (_enemyBotStateBotData[featureManager.GetCurrentEnemyBotIndex].stateConfirmMode == StateConfirmMode.Stagger)
                _isStaggerTauntActivated = true;

            else
            {
                if (_isStaggerTauntActivated)
                    _enemyBotStateBotData[featureManager.GetCurrentEnemyBotIndex].stateConfirmMode = StateConfirmMode.Hitting;

                else if (GetEnemyBotStateConfirmMode == StateConfirmMode.None) 
                {
                    if (_currentBlockingChance == 100f)
                        _enemyBotStateBotData[featureManager.GetCurrentEnemyBotIndex].stateConfirmMode = StateConfirmMode.Blocking;
                    else
                        _enemyBotStateBotData[featureManager.GetCurrentEnemyBotIndex].stateConfirmMode = _blockingEnemyBotRandomChance.Next(0, 100) > GetCurrentPercentBlockingChance ? StateConfirmMode.Blocking : StateConfirmMode.Hitting;

                    if (_enemyBotStateBotData[featureManager.GetCurrentEnemyBotIndex].stateConfirmMode == StateConfirmMode.Hitting)
                        _currentBlockingChance += GetCurrentEnemyBlockingChanceData.additiveBlockingChance;

                    if (_currentBlockingChance > 100)
                        _currentBlockingChance = 100;

                }
            }


            #endregion

            #region AnimationState Bot

            //Attacker
            if (featureManager.GetSpecificBotAnimatorByType(GetHitConfirmModule.GetAttackerBotType).GetCurrentAnimatorClipInfo(0)[0].clip != GetAttackerAnimationClipOnHitConfirm) {

                //Allows the assignment of the Offense that corresponds to the attacking Bot
                if (GetHitConfirmModule.GetIsHitConfirmActivated){

                    featureManager.GetSpecificBotAnimatorByType(GetHitConfirmModule.GetAttackerBotType).Play(GetAttackerAnimationClipOnHitConfirm.name);
                    featureManager.GetAttackerBotOffenseManager.AssignCurrentOffense(GetAttackerAnimationClipOnHitConfirm.name);
                }
            }

            //Defending
            //Allows you to assign the Offense that corresponds to the Bot that should be block
            if (GetDefendingBotData.stateConfirmMode == StateConfirmMode.Blocking) {

                featureManager.ApplyCurrentOffense(GetHitConfirmModule.GetDefendingBotType, OffenseType.DEFLECTION, GetHitConfirmModule.GetDefendingHitConfirmBlockingData().blockingOffenseDirection, GetCurrentBlockingAnimationClipOffenseType);

                return true;
            }

            //Allows you to assign the Offense that corresponds to the Bot that should be parried
            if (GetDefendingBotData.stateConfirmMode == StateConfirmMode.Parry) {

                _enemyAnimationClipOffenseType = AnimationClipOffenseType.Stagger;

                //DefendingBot
                featureManager.ApplyCurrentOffense(GetHitConfirmModule.GetDefendingBotType, OffenseType.DEFLECTION, GetHitConfirmModule.GetDefendingHitConfirmBlockingData().blockingOffenseDirection, AnimationClipOffenseType.Parry);

                //AttackerBot
                featureManager.ApplyCurrentOffense(GetHitConfirmModule.GetAttackerBotType, featureManager.GetAttackerBotOffenseManager.GetCurrentOffense.GetOffenseType, featureManager.GetAttackerBotOffenseManager.GetCurrentOffense.GetOffenseDirection, AnimationClipOffenseType.Stagger);

                return true;
            }

            //Hitting
            if (featureManager.GetDefendingBotOffenseManager.GetCurrentOffense.GetOffenseType == OffenseType.DAMAGEHIT){

                if (featureManager.GetSpecificBotAnimationClipByType(GetHitConfirmModule.GetDefendingBotType) == featureManager.GetDefendingBotOffenseManager.GetCurrentOffense.GetAnimationClip(AnimationClipOffenseType.Full))
                    featureManager.GetSpecificBotAnimatorByType(GetHitConfirmModule.GetDefendingBotType).Play(featureManager.GetSpecificBotAnimationClipByType(GetHitConfirmModule.GetDefendingBotType).name, -1, 0);

            }

            if (_isStaggerTauntActivated) 
            {
                featureManager.ApplyCurrentOffense(GetHitConfirmModule.GetDefendingBotType, OffenseType.STUN, GetHitConfirmModule.GetDefendingHitConfirmBlockingData().offenseBlockingData.offense.GetOffenseDirection, AnimationClipOffenseType.Full);

                _currentBlockingChance = 100f;
            }
            else
                featureManager.ApplyCurrentOffense(GetHitConfirmModule.GetDefendingBotType, OffenseType.DAMAGEHIT, GetHitConfirmModule.GetDefendingHitConfirmBlockingData().offenseBlockingData.offense.GetOffenseDirection, AnimationClipOffenseType.Full);

            #endregion

            return true;
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

            drawer.Field("_currentBlockingChance", false);

            drawer.Space();

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