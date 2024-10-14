using System;

using UnityEngine;

using SturdyMachine.Offense.Blocking;
using SturdyMachine.Offense;
using SturdyMachine.Component;
using SturdyMachine.Settings;

#if UNITY_EDITOR
using NWH.VehiclePhysics2;
using UnityEditor;
#endif

namespace SturdyMachine.Features.HitConfirm {

    /// <summary>
    /// Represents the module allowing the management of HitConfirm during a fight
    /// </summary>
    [Serializable, Tooltip("Represents the module allowing the management of HitConfirm during a fight")]
    public partial class HitConfirmModule : FeatureModule {

        #region Attributes

        /// <summary>
        /// Represents the audioSource used to play the AudioClips depending on the HitConfirm state
        /// </summary>
        [SerializeField, Tooltip("Represents the audioSource used to play the AudioClips depending on the HitConfirm state")]
        AudioSource _hitConfirmAudioSource;

        /// <summary>
        /// Represents the time in seconds present for the timer
        /// </summary>
        float _currentHitConfirmTime;

        /// <summary>
        /// Protection allowing the speed to be correctly assigned to zero during HitConfirm
        /// </summary>
        bool _ifHitConfirmSpeedApplied;

        /// <summary>
        /// Represents the activation status of HitConfirm
        /// </summary>
        bool _isHitConfirmActivated;

        bool _isHitConfirmDataInitialized;

        /// <summary>
        /// Represents the type of bot that is in attack mode and defense mode
        /// </summary>
        [SerializeField]
        BotType _currentAttackerBotType, _currentDefendingBotType;

        [SerializeField]
        OffenseType _blockingOffenseType;

        [SerializeField]
        OffenseDirection _blockingOffenseDirection;

        float _sturdyDamageIntensity, _enemyDamageIntensity;

        byte _currentOffenseBlockingConfigDataIndex, _currentOffenseBlockingIndex, _currentBlockingDataIndex, _currentOffenseBlockingData;

        #endregion

        #region Properties

        public float GetCurrentSturdyDamageIntensity => _sturdyDamageIntensity;

        public float GetCurrentEnemyDamageIntensity => _enemyDamageIntensity;

        public override FeatureModuleCategory GetFeatureModuleCategory() => FeatureModuleCategory.HitConfirm;

        public bool GetIsSturdyBotOnBlockingMode {

            get 
            {
                //OffenseType
                if (featureManager.GetPlayerBotOffenseManager.GetCurrentOffense.GetOffenseType != _blockingOffenseType)
                    return false;

                //OffenseDirection
                if (featureManager.GetPlayerBotOffenseManager.GetCurrentOffense.GetOffenseDirection != _blockingOffenseDirection)
                    return false;

                if (!featureManager.GetPlayerBotOffenseManager.GetCurrentOffense.GetIsInDeflectionRange(featureManager.GetSpecificAnimatorStateInfoByBotType(_currentDefendingBotType).normalizedTime))
                    return false;

                return true;
            }
        }

        public BotType GetDefendingBotType => _currentDefendingBotType;
        public BotType GetAttackerBotType => _currentAttackerBotType;

        public bool GetIsHitConfirmActivated => _isHitConfirmActivated;

        public OffenseType GetBlockingOffenseType => _blockingOffenseType;
        public OffenseDirection GetBlockingOffenseDirection => _blockingOffenseDirection;

        public OffenseBlockingData GetDefendingOffenseBlockingData => featureManager.GetOffenseBlockingConfig.GetOffenseBlockingConfigData[_currentOffenseBlockingConfigDataIndex]
                                                                                                             .offenseBlocking[_currentOffenseBlockingIndex]
                                                                                                             .GetBlockingData[_currentBlockingDataIndex]
                                                                                                             .offenseBlockingData[_currentOffenseBlockingData];

        #endregion

        #region Methods

        public override void Initialize(FeatureManager pFeatureManager)
        {
            base.Initialize();

            _hitConfirmAudioSource = featureManager.GetSturdyComponent.GetComponent<AudioSource>();
        }

        public override bool OnUpdate(bool pIsLeftFocus, bool pIsRightFocus, bool pIsGoodOffenseDirection)
        {
            if (!base.OnUpdate())
                return false;

            //Manage HitConfirm for each Bot when the HitConfirm is not activated
            if (!featureManager.GetHitConfirmModule.GetIsHitConfirmActivated) 
            {
                //Sturdy
                if (featureManager.GetPlayerBotOffenseManager.GetCurrentOffense.GetOffenseIsInAttackMode) 
                {
                    if (featureManager.GetIsGoodAttackOffenseDirection())
                    {
                        if (_currentAttackerBotType != BotType.SturdyBot) 
                        {
                            _currentAttackerBotType = BotType.SturdyBot;

                            _isHitConfirmDataInitialized = false;
                        }

                        _currentDefendingBotType = featureManager.GetCurrentEnemyBotType;
                    }

                    else if (_currentAttackerBotType != BotType.None)
                        _currentAttackerBotType = BotType.None;
                }

                //EnemyBot
                if (featureManager.GetEnemyBotFocusedOffenseManager.GetCurrentOffense.GetOffenseIsInAttackMode) 
                {
                    if (_currentAttackerBotType == BotType.None) 
                    {
                        _currentAttackerBotType = featureManager.GetCurrentEnemyBotType;
                        _currentDefendingBotType = BotType.SturdyBot;

                        _isHitConfirmDataInitialized = false;
                    }
                }
                
                if (_currentAttackerBotType == BotType.None)
                    return false;

                if (!_isHitConfirmDataInitialized) 
                {
                    HitConfirmBlockingData(GetAttackerBotType, featureManager.GetSpecificOffenseManagerBotByType(GetAttackerBotType), GetDefendingBotType, featureManager.GetSpecificOffenseManagerBotByType(GetDefendingBotType));
                    _isHitConfirmDataInitialized = true;
                }

                //Allows you to manage the activation and assignment of information concerning the HitConfirm
                if (featureManager.GetSpecificBotAnimationClipByType(_currentAttackerBotType).name != featureManager.GetAttackerBotOffenseManager.GetCurrentOffense.GetAnimationClip(AnimationClipOffenseType.Full).name)
                    return false;

                //Checks if the attacking Bot's clip exceeds the minimum value of the blocking section
                if (featureManager.GetSpecificAnimatorStateInfoByBotType(_currentAttackerBotType).normalizedTime < GetDefendingOffenseBlockingData.maxBlockingRangeData.rangeTime)
                    return false;

                _isHitConfirmActivated = true;

                _isHitConfirmDataInitialized = false;

                return true;
            }

            if (!_ifHitConfirmSpeedApplied)
            {
                _hitConfirmAudioSource.clip = featureManager.GetDefendingBotOffenseManager.GetCurrentOffense.GetAudioOffenseDataClip(AnimationClipOffenseType.Full);

                //Parry
                if (featureManager.GetStateConfirmModule.GetSturdyStateBotData.stateConfirmMode == StateConfirmMode.Parry)
                    _hitConfirmAudioSource.clip = featureManager.GetDefendingBotOffenseManager.GetCurrentOffense.GetAudioOffenseDataClip(AnimationClipOffenseType.Parry);                

                featureManager.GetSpecificBotAnimatorByType(BotType.SturdyBot).speed = 0;

                EnnemyBotHitConfirmAnimatorSpeed(0);

                _ifHitConfirmSpeedApplied = !_ifHitConfirmSpeedApplied;
            }

            //Allows you to manage the waiting time in seconds before the HitConfirm ends
            if (!_hitConfirmAudioSource.isPlaying) {
            
                if (_hitConfirmAudioSource.clip)
                    _hitConfirmAudioSource.Play();
            }

            _currentHitConfirmTime += Time.deltaTime;

            //Checks if the wait time matches with the timer
            if (_currentHitConfirmTime >= GameSettings.GetGameSettings().GetGameplaySettings.GetHitConfirmSettings.GetWaitTimer){

                _sturdyDamageIntensity = 0;
                _enemyDamageIntensity = 0;

                _currentHitConfirmTime = 0;

                //Reset the Animator speed to normal for Bots
                EnnemyBotHitConfirmAnimatorSpeed(1f);

                featureManager.GetSpecificBotAnimatorByType(BotType.SturdyBot).speed = 1;

                _ifHitConfirmSpeedApplied = !_ifHitConfirmSpeedApplied;

                _isHitConfirmActivated = false;
            }

            return true;
        }

        /// <summary>
        /// Allows the assignment of the Animator speed of all enemy Bots
        /// </summary>
        /// <param name="pSpeed">The speed value that we want to assign to the Animators of enemy Bots</param>
        void EnnemyBotHitConfirmAnimatorSpeed(float pSpeed) {

            for (byte i = 0; i < featureManager.GetEnemyBotAnimator.Length; ++i)
                featureManager.GetEnemyBotAnimator[i].speed = pSpeed;

            featureManager.GetSpecificBotAnimatorByType(featureManager.GetCurrentEnemyBotType).speed = pSpeed;
        }

        /// <summary>
        /// Checks if the Defending Bot's HitConfirm was assigned correctly
        /// </summary>
        /// <param name="pAttackerBotType">Type of attacking bot</param>
        /// <param name="pAttackerBotOffenseManager">The offenseManager of the attacking bot</param>
        /// <param name="pDefenderBotType">Type of defending bot</param>
        /// <param name="pDefenderBotOffenseManager">The offenseManager of the defending bot</param>
        void HitConfirmBlockingData(BotType pAttackerBotType, OffenseManager pAttackerBotOffenseManager, BotType pDefenderBotType, OffenseManager pDefenderBotOffenseManager)
        {
            //Iterates through all OffenseBlockingConfig
            for (byte i = 0; i < featureManager.GetOffenseBlockingConfig.GetOffenseBlockingConfigData.Length; ++i)
            {
                //Iterates through all OffenseBlocking
                for (byte j = 0; j < featureManager.GetOffenseBlockingConfig.GetOffenseBlockingConfigData[i].offenseBlocking.Length; ++j)
                {
                    //Iterates through all blocking information based on Bot type
                    for (byte k = 0; k < featureManager.GetOffenseBlockingConfig.GetOffenseBlockingConfigData[i].offenseBlocking[j].GetBlockingData.Length; ++k)
                    {
                        //Checks if the Bot type matches the attacking one
                        if (featureManager.GetOffenseBlockingConfig.GetOffenseBlockingConfigData[i].offenseBlocking[j].GetBlockingData[k].botType != pAttackerBotType)
                            continue;

                        _currentOffenseBlockingConfigDataIndex = i;
                        _currentOffenseBlockingIndex = j;
                        _currentBlockingDataIndex = k;

                        //Iterates through the list of blocking sections based on the attacking bot
                        for (byte l = 0; l < featureManager.GetOffenseBlockingConfig.GetOffenseBlockingConfigData[i].offenseBlocking[j].GetBlockingData[k].offenseBlockingData.Length; ++l)
                        {
                            //Checks if the Offense the attacking Bot is playing matches the one in the block list
                            if (pAttackerBotOffenseManager.GetCurrentOffense != featureManager.GetOffenseBlockingConfig.GetOffenseBlockingConfigData[i].offenseBlocking[j].GetBlockingData[k].offenseBlockingData[l].offense)
                                continue;

                            _currentOffenseBlockingData = l;

                            return;
                        }
                    }
                }
            }
        }

        #endregion
    }

#if UNITY_EDITOR

    [CustomPropertyDrawer(typeof(HitConfirmModule))]
    public partial class HitConfirmModuleDrawer : FeatureModuleDrawer
    {
        public override bool OnNUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (!base.OnNUI(position, property, label))
                return false;

            drawer.BeginSubsection("Debug Value");

            GUI.enabled = false;
            
            drawer.Field("_currentAttackerBotType");
            drawer.Field("_currentDefendingBotType");

            drawer.BeginSubsection("BlockingOffenseData");

            drawer.Field("_blockingOffenseType", true, null, "Type: ");
            drawer.Field("_blockingOffenseDirection", true, null, "Direction: ");

            drawer.EndSubsection();

            GUI.enabled = true;

            drawer.EndSubsection();

            drawer.EndProperty();
            return true;
        }
    }

#endif
}