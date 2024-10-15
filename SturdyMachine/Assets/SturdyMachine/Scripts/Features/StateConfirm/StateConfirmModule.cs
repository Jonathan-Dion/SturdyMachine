using System;
using UnityEngine;

using SturdyMachine.Settings.GameplaySettings.StateConfirmSettings;
using SturdyMachine.Features.HitConfirm;
using SturdyMachine.Component;
using SturdyMachine.Offense;
using SturdyMachine.Settings.GameplaySettings;

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

        [SerializeField]
        float _currentBlockingChanceValue;

        System.Random _blockingEnemyBotRandomChance;

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

        StateConfirmSettings GetStateConfirmSetting => GameplaySettings.GetGameplaySettings().GetStateConfirmSettings;

        BlockingChanceData GetBlockingChanceData(BotType pCurrentBotType) => GetStateConfirmSetting.GetBlockingChanceData(pCurrentBotType);

        float GetCurrentBlockingChancePercent => 100 - (_currentBlockingChanceValue * 100);

        #endregion

        #region Methods

        public override void Initialize(FeatureManager pFeatureManager)
        {
            base.Initialize(pFeatureManager);

            _enemyBotStateBotData = new StateBotData[featureManager.GetEnemyBotObject.Length];

            _blockingEnemyBotRandomChance = new System.Random();

            _currentBlockingChanceValue = GetBlockingChanceData(featureManager.GetCurrentEnemyBotType).minBlockingChance;
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
                featureManager.GetIsGoodAttackOffenseDirection());

            if (!GetHitConfirmModule.GetIsHitConfirmActivated)
            {
                _enemyAnimationClipOffenseType = AnimationClipOffenseType.Full;

                _sturdyStateBotData.stateConfirmMode = StateConfirmMode.None;

                if (GetEnemyBotStateConfirmMode != StateConfirmMode.Stagger)
                    _enemyBotStateBotData[featureManager.GetCurrentEnemyBotIndex].stateConfirmMode = StateConfirmMode.None;

                if (_isBlockingSequenceComboOffense.Length != featureManager.GetFightsModule.GetNbrOfEnemyBotOffenseBlocking)
                    _isBlockingSequenceComboOffense = new bool[featureManager.GetFightsModule.GetNbrOfEnemyBotOffenseBlocking];

                if (!_isStaggerTauntActivated)
                    return true;

                if (_currentStaggerTauntTime == 0f)
                {
                    if (featureManager.GetSpecificBotAnimationClipByType(BotType.SkinnyBot) != GetStateConfirmSetting.GetStaggerStateData.stunAnimationClip)
                    {
                        if (featureManager.GetSpecificAnimatorStateInfoByBotType(BotType.SkinnyBot).normalizedTime > 0.98)
                            featureManager.GetSpecificBotAnimatorByType(BotType.SkinnyBot).Play(GetStateConfirmSetting.GetStaggerStateData.stunAnimationClip.name);
                    }
                }

                _currentStaggerTauntTime += Time.deltaTime;

                if (_currentStaggerTauntTime > GetStateConfirmSetting.GetStaggerStateData.maxStunTimer)
                {
                    _currentStaggerTauntTime = 0;
                    _isStaggerTauntActivated = false;
                }
                else if (GetStateConfirmSetting.GetStaggerStateData.maxStunTimer - _currentStaggerTauntTime <= GetStateConfirmSetting.GetStaggerStateData.recoveryStunAnimationClip.length) 
                {
                    _currentBlockingChanceValue = 100f;
                    featureManager.GetSpecificBotAnimatorByType(BotType.SkinnyBot).Play(GetStateConfirmSetting.GetStaggerStateData.recoveryStunAnimationClip.name);
                }

                else if (featureManager.GetSpecificAnimatorStateInfoByBotType(BotType.SkinnyBot).normalizedTime > 0.98)
                    featureManager.GetSpecificBotAnimatorByType(BotType.SkinnyBot).Play(GetStateConfirmSetting.GetStaggerStateData.stunAnimationClip.name, -1, 0);

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

                        _currentBlockingChanceValue -= GetStateConfirmSetting.GetBlockingChanceData(featureManager.GetCurrentEnemyBotType).maxDecreaseBlockingChance;
                    }

                    //Hitting
                    else
                    {
                        _sturdyStateBotData.stateConfirmMode = StateConfirmMode.Hitting;
                        _currentBlockingChanceValue -= GetStateConfirmSetting.GetBlockingChanceData(featureManager.GetCurrentEnemyBotType).minDecreaseBlockingChance;
                    }

                    if (_currentBlockingChanceValue < 0)
                        _currentBlockingChanceValue = 0;

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
            else 
            {
                if (_enemyBotStateBotData[featureManager.GetCurrentEnemyBotIndex].stateConfirmMode == StateConfirmMode.Stagger)
                    _isStaggerTauntActivated = true;

                if (_isStaggerTauntActivated)
                {
                    if (_currentBlockingChanceValue != 100f)
                        _enemyBotStateBotData[featureManager.GetCurrentEnemyBotIndex].stateConfirmMode = StateConfirmMode.Hitting;
                    else
                        _enemyBotStateBotData[featureManager.GetCurrentEnemyBotIndex].stateConfirmMode = StateConfirmMode.Blocking;
                }

                else if (GetEnemyBotStateConfirmMode == StateConfirmMode.None)
                    _enemyBotStateBotData[featureManager.GetCurrentEnemyBotIndex].stateConfirmMode = _blockingEnemyBotRandomChance.Next(0, 100) > GetCurrentBlockingChancePercent ? StateConfirmMode.Blocking : StateConfirmMode.Hitting;

                if (_currentBlockingChanceValue != 100f) 
                {
                    //Hit
                    if (_enemyBotStateBotData[featureManager.GetCurrentEnemyBotIndex].stateConfirmMode == StateConfirmMode.Hitting)
                        _currentBlockingChanceValue = Mathf.Clamp(_currentBlockingChanceValue += GetBlockingChanceData(featureManager.GetCurrentEnemyBotType).additiveBlockingChance, 0f, 100f);
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

                featureManager.ApplyCurrentOffense(GetHitConfirmModule.GetDefendingBotType, OffenseType.DEFLECTION, GetHitConfirmModule.GetBlockingOffenseDirection, GetCurrentBlockingAnimationClipOffenseType);

                return true;
            }

            //Allows you to assign the Offense that corresponds to the Bot that should be parried
            if (GetDefendingBotData.stateConfirmMode == StateConfirmMode.Parry) {

                _enemyAnimationClipOffenseType = AnimationClipOffenseType.Stagger;

                //DefendingBot
                featureManager.ApplyCurrentOffense(GetHitConfirmModule.GetDefendingBotType, OffenseType.DEFLECTION, GetHitConfirmModule.GetBlockingOffenseDirection, AnimationClipOffenseType.Parry);

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
                featureManager.ApplyCurrentOffense(GetHitConfirmModule.GetDefendingBotType, OffenseType.STUN, GetHitConfirmModule.GetDefendingOffenseBlockingData.offense.GetOffenseDirection, AnimationClipOffenseType.Full);
            else
                featureManager.ApplyCurrentOffense(GetHitConfirmModule.GetDefendingBotType, OffenseType.DAMAGEHIT, GetHitConfirmModule.GetBlockingOffenseDirection, AnimationClipOffenseType.Full);

            #endregion

            /*if (GetSpecificHitConfirmBlockingDataByType(_currentDefendingBotType).isHitting)
                {
                    if (_currentAttackerBotType == BotType.SturdyBot)
                        _sturdyDamageIntensity = featureManager.GetPlayerBotOffenseManager.GetLastOffense.GetCurrentDamageIntensity(BotType.SturdyBot);

                    else
                        _enemyDamageIntensity = featureManager.GetEnemyBotFocusedOffenseManager.GetCurrentOffense.GetCurrentDamageIntensity(featureManager.GetCurrentEnemyBotType);
                }*/

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

            float currentBlockingChance = drawer.FindProperty("_currentBlockingChanceValue").floatValue;
            //currentBlockingChance += currentBlockingChance > 0 ? 1 : 0;

            drawer.Label($"Blocking chance: {currentBlockingChance * 100}%");

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