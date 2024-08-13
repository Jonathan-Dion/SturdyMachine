using System;

using UnityEngine;

using SturdyMachine.Offense.Blocking;
using SturdyMachine.Offense;
using SturdyMachine.Component;

#if UNITY_EDITOR
using NWH.VehiclePhysics2;
using UnityEditor;
#endif

namespace SturdyMachine.Features.HitConfirm {

    /// <summary>
    /// Identify the type of HitConfirm
    /// </summary>
    public enum HitConfirmType { None, Normal, Slow, Stop }

    /// <summary>
    /// Brings together the information necessary for HitConfirm management
    /// </summary>
    [Serializable, Tooltip("Brings together the information necessary for HitConfirm management")]
    public struct HitConfirmBlockingData {

        public OffenseType blockingOffenseType;

        public OffenseDirection blockingOffenseDirection;

        /// <summary>
        /// Represents information allowing blocking of the attacking offense
        /// </summary>
        [Tooltip("Represents information allowing blocking of the attacking offense")]
        public OffenseBlockingData offenseBlockingData;

        /// <summary>
        /// Represents the Offense that will be played if the defending Bot gets hit
        /// </summary>
        [Tooltip("Represents the Offense that will be played if the defending Bot gets hit")]
        public Offense.Offense hittingOffense;

        public bool isHitting, isBlocking;
    }

    /// <summary>
    /// Represents the module allowing the management of HitConfirm during a fight
    /// </summary>
    [Serializable, Tooltip("Represents the module allowing the management of HitConfirm during a fight")]
    public partial class HitConfirmModule : FeatureModule {

        #region Attributes

        /// <summary>
        /// Represents HitConfirm information when the enemy bot is attacked
        /// </summary>
        [SerializeField, Tooltip("Represents HitConfirm information when the enemy bot is attacked")]
        HitConfirmBlockingData _ennemyHitConfirmBlockingData;

        /// <summary>
        /// Represents HitConfirm information when the player bot is attacked
        /// </summary>
        [SerializeField, Tooltip("Represents HitConfirm information when the player bot is attacked")]
        HitConfirmBlockingData _playerHitConfirmBlockingData;

        /// <summary>
        /// Represents the audio that should play when a hit is activated
        /// </summary>
        [SerializeField, Tooltip("Represents the audio that should play when a hit is activated")]
        AudioClip _hittingAudioClip;

        /// <summary>
        /// Represents the audio that should play when a block is activated
        /// </summary>
        [SerializeField, Tooltip("Represents the audio that should play when a block is activated")]
        AudioClip _blockingAudioClip;

        /// <summary>
        /// Represents the sound that should be played when the player successfully performs a Parry
        /// </summary>
        [SerializeField, Tooltip("Represents the sound that should be played when the player successfully performs a Parry")]
        AudioClip _parryAudioClip;

        /// <summary>
        /// Represents the audioSource used to play the AudioClips depending on the HitConfirm state
        /// </summary>
        [SerializeField, Tooltip("Represents the audioSource used to play the AudioClips depending on the HitConfirm state")]
        AudioSource _hitConfirmAudioSource;

        /// <summary>
        /// Represents the time the hitConfirm should play
        /// </summary>
        [SerializeField, Tooltip("Represents the time the hitConfirm should play")]
        float _waitTimer;

        /// <summary>
        /// Represents the time in seconds present for the timer
        /// </summary>
        float _currentHitConfirmTime, _currentMaxHitConfirmTimer;

        /// <summary>
        /// Protection allowing the speed to be correctly assigned to zero during HitConfirm
        /// </summary>
        bool _ifHitConfirmSpeedApplied;

        /// <summary>
        /// Represents the activation status of HitConfirm
        /// </summary>
        bool _isHitConfirmActivated;

        /// <summary>
        /// Represents the type of bot that is in attack mode and defense mode
        /// </summary>
        BotType _currentAttackerBotType, _currentDefendingBotType;

        float _sturdyDamageIntensity, _enemyDamageIntensity;

        #endregion

        #region Properties

        public float GetCurrentSturdyDamageIntensity => _sturdyDamageIntensity;

        public float GetCurrentEnemyDamageIntensity => _enemyDamageIntensity;

        public override FeatureModuleCategory GetFeatureModuleCategory() => FeatureModuleCategory.HitConfirm;

        /// <summary>
        /// Allows you to assign the HitConfirm information of the defendant Bot
        /// </summary>
        /// <param name="pAttackerBotType">The type of the attacking bot</param>
        /// <param name="pAttackerBotOffenseManager">The offenseManager of the attacking bot</param>
        /// <param name="pDefenderBotOffenseManager">The offenseManager of the defending bot</param>
        /// <returns>Returns all information regarding the HitConfirm of the defending Bot</returns>
        HitConfirmBlockingData GetHitConfirmBlockingData(BotType pAttackerBotType, OffenseManager pAttackerBotOffenseManager, OffenseManager pDefenderBotOffenseManager) {
        
            HitConfirmBlockingData hitConfirmBlockingData = new HitConfirmBlockingData();

            OffenseBlockingConfigData offenseBlockingConfigData = new OffenseBlockingConfigData();

            //Iterates through all OffenseBlockingConfig
            for (byte i = 0; i < featureManager.GetOffenseBlockingConfig.GetOffenseBlockingConfigData.Length; ++i) {

                //Iterates through all OffenseBlocking
                for (byte j = 0; j < featureManager.GetOffenseBlockingConfig.GetOffenseBlockingConfigData[i].offenseBlocking.Length; ++j) {

                    //Iterates through all blocking information based on Bot type
                    for (int k = 0; k < featureManager.GetOffenseBlockingConfig.GetOffenseBlockingConfigData[i].offenseBlocking[j].GetBlockingData.Length; ++k) {

                        //Checks if the Bot type matches the attacking one
                        if (featureManager.GetOffenseBlockingConfig.GetOffenseBlockingConfigData[i].offenseBlocking[j].GetBlockingData[k].botType != pAttackerBotType)
                            continue;

                        //Iterates through the list of blocking sections based on the attacking bot
                        for (int l = 0; l < featureManager.GetOffenseBlockingConfig.GetOffenseBlockingConfigData[i].offenseBlocking[j].GetBlockingData[k].offenseBlockingData.Length; ++l) {

                            //Checks if the Offense the attacking Bot is playing matches the one in the block list
                            if (pAttackerBotOffenseManager.GetCurrentOffense != featureManager.GetOffenseBlockingConfig.GetOffenseBlockingConfigData[i].offenseBlocking[j].GetBlockingData[k].offenseBlockingData[l].offense)
                                continue;

                            //Assigns the correct OffenseBlocking information based on the Offense and the correct type of the attacking Bot
                            offenseBlockingConfigData = featureManager.GetOffenseBlockingConfig.GetOffenseBlockingConfigData[i];

                            //Assign the correct list of information about the blocking section of the Attacking Bot's Offense
                            hitConfirmBlockingData.offenseBlockingData = featureManager.GetOffenseBlockingConfig.GetOffenseBlockingConfigData[i].offenseBlocking[j].GetBlockingData[k].offenseBlockingData[l];
                        }
                    }
                }
            }

            //Assigns the correct Offense that must be played in order to block the Attacking Bot's Offense
            hitConfirmBlockingData.blockingOffenseType = offenseBlockingConfigData.offenseType;

            hitConfirmBlockingData.blockingOffenseDirection = offenseBlockingConfigData.offenseDirection;

            hitConfirmBlockingData.hittingOffense = pDefenderBotOffenseManager.GetOffense(OffenseType.DAMAGEHIT, offenseBlockingConfigData.offenseDirection);

            return hitConfirmBlockingData;
        }

        /// <summary>
        /// Checks if the Defending Bot's HitConfirm was assigned correctly
        /// </summary>
        /// <param name="pAttackerBotType">Type of attacking bot</param>
        /// <param name="pAttackerBotOffenseManager">The offenseManager of the attacking bot</param>
        /// <param name="pDefenderBotType">Type of defending bot</param>
        /// <param name="pDefenderBotOffenseManager">The offenseManager of the defending bot</param>
        void HitConfirmBlockingData(BotType pAttackerBotType, OffenseManager pAttackerBotOffenseManager, BotType pDefenderBotType, OffenseManager pDefenderBotOffenseManager) {

            if (!pAttackerBotOffenseManager.GetCurrentOffense.GetOffenseIsInAttackMode)
                return;

            if (featureManager.GetSpecificBotAnimationClipByType(pAttackerBotType) != pAttackerBotOffenseManager.GetCurrentOffense.GetAnimationClip(AnimationClipOffenseType.Full))
                return;

            if (!GetSpecificHitConfirmBlockingDataByType(pDefenderBotType).Equals(new HitConfirmBlockingData()))
                return;

            //Checks if information regarding OffenseBlocking of the defending Bot should be assigned
            HitConfirmBlockingData hitConfirmBlockingData = GetHitConfirmBlockingData(pAttackerBotType, pAttackerBotOffenseManager, pDefenderBotOffenseManager);

            //Assigns information regarding the HitConfirm of the defending bot
            
            //AttackerBot
            _currentAttackerBotType = pAttackerBotType;

            //DefenderBot
            _currentDefendingBotType = pDefenderBotType;

            //Sturdy
            if (pDefenderBotType == BotType.SturdyBot)
                _playerHitConfirmBlockingData = hitConfirmBlockingData;

            else
                _ennemyHitConfirmBlockingData = hitConfirmBlockingData;
        }

        /// <summary>
        /// Allows you to return the HitConfirmBlockingData of the defending Bot
        /// </summary>
        /// <returns>Returns the HitConfirmBlockingData of the defending Bot</returns>
        public HitConfirmBlockingData GetDefendingHitConfirmBlockingData() {

            //Player
            if (!_playerHitConfirmBlockingData.Equals(new HitConfirmBlockingData()))
                return _playerHitConfirmBlockingData;

            //Ennemy
            return _ennemyHitConfirmBlockingData;
        }

        public HitConfirmBlockingData GetSpecificHitConfirmBlockingDataByType(BotType pSpecificBotType) 
        {
            //Sturdy
            if (pSpecificBotType == BotType.SturdyBot)
                return _playerHitConfirmBlockingData;

            return _ennemyHitConfirmBlockingData;
        }

        public bool GetIsSturdyBotOnBlockingMode {

            get 
            {
                //OffenseType
                if (featureManager.GetSpecificOffenseManagerBotByType(BotType.SturdyBot).GetCurrentOffense.GetOffenseType != GetDefendingHitConfirmBlockingData().blockingOffenseType)
                    return false;

                //OffenseDirection
                if (featureManager.GetSpecificOffenseManagerBotByType(BotType.SturdyBot).GetCurrentOffense.GetOffenseDirection != GetDefendingHitConfirmBlockingData().blockingOffenseDirection)
                    return false;

                if (!featureManager.GetSpecificOffenseManagerBotByType(BotType.SturdyBot).GetCurrentOffense.GetIsInDeflectionRange(featureManager.GetSpecificAnimatorStateInfoByBotType(_currentDefendingBotType).normalizedTime))
                    return false;

                return true;
            }
        }

        public BotType GetDefendingBotType => _currentDefendingBotType;
        public BotType GetAttackerBotType => _currentAttackerBotType;

        public bool GetIsHitConfirmActivated => _isHitConfirmActivated;

        #endregion

        #region Methods

        public override void Initialize(FeatureManager pFeatureManager)
        {
            base.Initialize();

            _hitConfirmAudioSource = featureManager.GetSturdyComponent.GetComponent<AudioSource>();
        }

        public override bool OnUpdate(bool pIsLeftFocus, bool pIsRightFocus)
        {
            if (!base.OnUpdate())
                return false;

            //Manage HitConfirm for each Bot when the HitConfirm is not activated
            if (!featureManager.GetHitConfirmModule.GetIsHitConfirmActivated) 
            {
                //Checks if the defending Bot is the player
                HitConfirmBlockingData(featureManager.GetCurrentEnemyBotType, featureManager.GetSpecificOffenseManagerBotByType(featureManager.GetCurrentEnemyBotType), BotType.SturdyBot, featureManager.GetSpecificOffenseManagerBotByType(BotType.SturdyBot));

                //Checks if the defending Bot is the Enemy Bot
                HitConfirmBlockingData(BotType.SturdyBot, featureManager.GetSpecificOffenseManagerBotByType(BotType.SturdyBot), featureManager.GetCurrentEnemyBotType, featureManager.GetSpecificOffenseManagerBotByType(featureManager.GetCurrentEnemyBotType));

                if (_playerHitConfirmBlockingData.Equals(new HitConfirmBlockingData())) {

                    if (_ennemyHitConfirmBlockingData.Equals(new HitConfirmBlockingData()))
                        return false;
                }

                //Allows you to manage the activation and assignment of information concerning the HitConfirm
                if (featureManager.GetSpecificBotAnimationClipByType(_currentAttackerBotType) == featureManager.GetSpecificOffenseManagerBotByType(_currentAttackerBotType).GetCurrentOffense.GetAnimationClip(AnimationClipOffenseType.Full)){

                    //Checks if the attacking Bot's clip exceeds the minimum value of the blocking section
                    if (featureManager.GetSpecificAnimatorStateInfoByBotType(_currentAttackerBotType).normalizedTime >= GetSpecificHitConfirmBlockingDataByType(_currentDefendingBotType).offenseBlockingData.minBlockingRangeData.rangeTime){

                        _isHitConfirmActivated = true;

                        _currentMaxHitConfirmTimer = _waitTimer;

                        if (GetSpecificHitConfirmBlockingDataByType(_currentDefendingBotType).isHitting){
                            
                            if (_currentAttackerBotType == BotType.SturdyBot)
                                _sturdyDamageIntensity = featureManager.GetSpecificOffenseManagerBotByType(BotType.SturdyBot).GetLastOffense.GetCurrentDamageIntensity(BotType.SturdyBot);

                            else
                                _enemyDamageIntensity = featureManager.GetSpecificOffenseManagerBotByType(featureManager.GetCurrentEnemyBotType).GetCurrentOffense.GetCurrentDamageIntensity(featureManager.GetCurrentEnemyBotType);
                        }

                        return true;
                    }
                }

                return true;
            }

            if (!_ifHitConfirmSpeedApplied)
            {
                _hitConfirmAudioSource.clip = _hittingAudioClip;

                //Sturdy
                if (_currentDefendingBotType == BotType.SturdyBot) {

                    //Blocking
                    if (featureManager.GetStateConfirmModule.GetSturdyStateBotData.stateConfirmMode == StateConfirmMode.Blocking)
                        _hitConfirmAudioSource.clip = _blockingAudioClip;

                    //Parry
                    else if (featureManager.GetStateConfirmModule.GetSturdyStateBotData.stateConfirmMode == StateConfirmMode.Parry)
                        _hitConfirmAudioSource.clip = _parryAudioClip;
                }

                //EnemyBot
                else if (featureManager.GetStateConfirmModule.GetIsCurrentEnemyBotOnBlockingMode)
                    _hitConfirmAudioSource.clip = _blockingAudioClip;

                featureManager.GetSpecificBotAnimatorByType(BotType.SturdyBot).speed = 0;

                EnnemyBotHitConfirmAnimatorSpeed(0);

                _ifHitConfirmSpeedApplied = !_ifHitConfirmSpeedApplied;
            }

            //Allows you to manage the waiting time in seconds before the HitConfirm ends
            if (!_hitConfirmAudioSource.isPlaying)
                _hitConfirmAudioSource.Play();

            _currentHitConfirmTime += Time.deltaTime;

            //Checks if the wait time matches with the timer
            if (_currentHitConfirmTime >= _currentMaxHitConfirmTimer){

                _sturdyDamageIntensity = 0;
                _enemyDamageIntensity = 0;

                _currentHitConfirmTime = 0;

                //Reset the Animator speed to normal for Bots
                EnnemyBotHitConfirmAnimatorSpeed(1f);

                featureManager.GetSpecificBotAnimatorByType(BotType.SturdyBot).speed = 1;

                _ifHitConfirmSpeedApplied = !_ifHitConfirmSpeedApplied;

                _ennemyHitConfirmBlockingData = new HitConfirmBlockingData();
                _playerHitConfirmBlockingData = new HitConfirmBlockingData();

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

            drawer.Property("_playerHitConfirmBlockingData");
            drawer.Property("_ennemyHitConfirmBlockingData");

            GUI.enabled = true;

            drawer.EndSubsection();

            drawer.BeginSubsection("Configuration");
            drawer.Field("_waitTimer", true, "sec", "Timer: ");
            drawer.Field("_hittingAudioClip");
            drawer.Field("_blockingAudioClip");
            drawer.Field("_parryAudioClip");

            drawer.EndSubsection();

            drawer.EndProperty();
            return true;
        }
    }

    [CustomPropertyDrawer(typeof(HitConfirmBlockingData))]
    public partial class HitConfirmBlockingDataDrawer : ComponentNUIPropertyDrawer
    {
        public override bool OnNUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (!base.OnNUI(position, property, label))
                return false;

            drawer.Field("blockingOffenseType", false, null, "Offense Type: ");
            drawer.Field("blockingOffenseDirection", false, null, "Offense Direction: ");

            drawer.Space();
            
            drawer.Property("offenseBlockingData");

            drawer.EndProperty();
            return true;
        }
    }

#endif
}