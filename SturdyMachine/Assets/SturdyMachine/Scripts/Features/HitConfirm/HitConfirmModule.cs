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

        /// <summary>
        /// Represents the blocking offense that must be played
        /// </summary>
        [Tooltip("Represents the blocking offense that must be played")]
        public Offense.Offense blockingOffense;

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

        /// <summary>
        /// Represents the player's current cooldown type
        /// </summary>
        CooldownType _currentCooldownTime;

        DamageDataCache _damageDataCache;

        #endregion

        #region Properties

        public DamageDataCache GetDamageDataCache => _damageDataCache;

        /// <summary>
        /// Returns the player's current cooldown type
        /// </summary>
        public CooldownType GetCurrentCooldownType => _currentCooldownTime;

        public override FeatureModuleCategory GetFeatureModuleCategory() => FeatureModuleCategory.HitConfirm;

        /// <summary>
        /// Allows checking if the structure of the responding Bot needs to be initialized
        /// </summary>
        /// //<param name="pBotOffenseManager">The offenseManager of the bot you want to check</param>
        /// <param name="pHitConfirmBlockingData">The structure containing the information regarding the Defendant Bot's HitConfirm</param>
        /// <returns>Returns the state if the structure containing the Defending bot's HitConfirm information should be assigned</returns>
        bool GetIsBlockingDataInit(OffenseManager pBotOffenseManager, HitConfirmBlockingData pHitConfirmBlockingData) {

            //Checks if the bot present is in the attack phase
            if (!pBotOffenseManager.GetCurrentOffense.GetOffenseIsInAttackMode) {
                
                if (FEATURE_MANAGER.GetSpecificBotAnimationClipByType(_currentAttackerBotType) != FEATURE_MANAGER.GetSpecificOffenseManagerBotByType(_currentAttackerBotType).GetCurrentOffense.GetAnimationClip(AnimationClipOffenseType.KeyposeOut))
                    return false;
            }

            return pHitConfirmBlockingData.Equals(new HitConfirmBlockingData());
        }

        /// <summary>
        /// Allows you to assign the correct information regarding blocking the attacking bot's offense
        /// </summary>
        /// <param name="pAttackerBotType">Type of attacking bot</param>
        /// <param name="pAttackerOffenseManager">The offenseManager of the attacking bot</param>
        /// <returns></returns>
        OffenseBlockingData GetAttackerOffenseBlockingData(BotType pAttackerBotType, OffenseManager pAttackerOffenseManager, out OffenseBlockingConfigData pOffenseBlockingConfigData) {

            pOffenseBlockingConfigData = new OffenseBlockingConfigData();

            //Iterates through all OffenseBlockingConfig
            for (int i = 0; i < FEATURE_MANAGER.GetOffenseBlockingConfig.GetOffenseBlockingConfigData.Length; ++i)
            {
                //Iterates through all OffenseBlocking
                for (int j = 0; j < FEATURE_MANAGER.GetOffenseBlockingConfig.GetOffenseBlockingConfigData[i].offenseBlocking.Length; ++j)
                {
                    //Iterates through all blocking information based on Bot type
                    for (int k = 0; k < FEATURE_MANAGER.GetOffenseBlockingConfig.GetOffenseBlockingConfigData[i].offenseBlocking[j].GetBlockingData.Length; ++k)
                    {
                        //Checks if the Bot type matches the attacking one
                        if (FEATURE_MANAGER.GetOffenseBlockingConfig.GetOffenseBlockingConfigData[i].offenseBlocking[j].GetBlockingData[k].botType != pAttackerBotType)
                            continue;

                        //Iterates through the list of blocking sections based on the attacking bot
                        for (int l = 0; l < FEATURE_MANAGER.GetOffenseBlockingConfig.GetOffenseBlockingConfigData[i].offenseBlocking[j].GetBlockingData[k].offenseBlockingData.Length; ++l)
                        {
                            //Checks if the Offense the attacking Bot is playing matches the one in the block list
                            if (pAttackerOffenseManager.GetCurrentOffense != FEATURE_MANAGER.GetOffenseBlockingConfig.GetOffenseBlockingConfigData[i].offenseBlocking[j].GetBlockingData[k].offenseBlockingData[l].offense)
                                continue;

                            //Assigns the correct OffenseBlocking information based on the Offense and the correct type of the attacking Bot
                            pOffenseBlockingConfigData = FEATURE_MANAGER.GetOffenseBlockingConfig.GetOffenseBlockingConfigData[i];

                            //Returns the correct list of information about the blocking section of the Attacking Bot's Offense
                            return FEATURE_MANAGER.GetOffenseBlockingConfig.GetOffenseBlockingConfigData[i].offenseBlocking[j].GetBlockingData[k].offenseBlockingData[l];
                        }
                    }
                }
            }

            return new OffenseBlockingData();
        }

        /// <summary>
        /// Allows you to assign the HitConfirm information of the defendant Bot
        /// </summary>
        /// <param name="pAttackerBotType">The type of the attacking bot</param>
        /// <param name="pAttackerBotOffenseManager">The offenseManager of the attacking bot</param>
        /// <param name="pDefenderBotOffenseManager">The offenseManager of the defending bot</param>
        /// <returns>Returns all information regarding the HitConfirm of the defending Bot</returns>
        HitConfirmBlockingData GetHitConfirmBlockingData(BotType pAttackerBotType, OffenseManager pAttackerBotOffenseManager, OffenseManager pDefenderBotOffenseManager) {
        
            HitConfirmBlockingData hitConfirmBlockingData = new HitConfirmBlockingData();

            //Assigns the correct information regarding the blocking section of the attacking Bot's Offense
            hitConfirmBlockingData.offenseBlockingData = GetAttackerOffenseBlockingData(pAttackerBotType, pAttackerBotOffenseManager, out OffenseBlockingConfigData pOffenseBlockingConfigData);

            //Assigns the correct Offense that must be played in order to block the Attacking Bot's Offense
            hitConfirmBlockingData.blockingOffense = pDefenderBotOffenseManager.GetOffense(pOffenseBlockingConfigData.offenseType, pOffenseBlockingConfigData.offenseDirection);

            hitConfirmBlockingData.hittingOffense = pDefenderBotOffenseManager.GetOffense(OffenseType.DAMAGEHIT, pOffenseBlockingConfigData.offenseDirection);

            return hitConfirmBlockingData;
        }

        /// <summary>
        /// Checks if the Defending Bot's HitConfirm was assigned correctly
        /// </summary>
        /// <param name="pAttackerBotType">Type of attacking bot</param>
        /// <param name="pAttackerBotOffenseManager">The offenseManager of the attacking bot</param>
        /// <param name="pDefenderBotType">Type of defending bot</param>
        /// <param name="pDefenderBotOffenseManager">The offenseManager of the defending bot</param>
        /// <returns>Returns whether the HitConfirm of the attacking and defending Bot were assigned correctly</returns>
        bool GetIsBlockingDataSetup(BotType pAttackerBotType, OffenseManager pAttackerBotOffenseManager, BotType pDefenderBotType, OffenseManager pDefenderBotOffenseManager) {

            //Checks if information regarding OffenseBlocking of the defending Bot should be assigned
            if (GetIsBlockingDataInit(pAttackerBotOffenseManager, GetSpecificHitConfirmBlockingDataByType(pDefenderBotType)))
            {
                HitConfirmBlockingData hitConfirmBlockingData = GetHitConfirmBlockingData(pAttackerBotType, pAttackerBotOffenseManager, pDefenderBotOffenseManager);

                //Assigns information regarding the HitConfirm of the defending bot
                if (!GetSpecificHitConfirmBlockingDataByType(pDefenderBotType).Equals(hitConfirmBlockingData))
                {
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

                return true;
            }

            return false;
        }

        /// <summary>
        /// Checks if the attacking Bot's Offense clip is in the blocking section
        /// </summary>
        /// <param name="pRangeValue">The value of the range in %</param>
        /// <param name="pCurrentNormalizedTime">The % value of the Offense clip of the attacking Bot</param>
        /// <returns>Return if Attacking Bot's Offense clip is in the blocking section</returns>
        bool GetIsOutBlockingRange(float pRangeValue, float pCurrentNormalizedTime) {
        
            if (pRangeValue < 1f)
                return pCurrentNormalizedTime > pRangeValue;

            return pCurrentNormalizedTime >= 0.85f;
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

        /// <summary>
        /// Checks if the Offense change during HitConfirm was made
        /// </summary>
        /// <param name="pNextOffense">The next offense applied</param>
        /// <param name="pAnimationClipOffenseType">The animationCLip type of the next offense applied that you want to check</param>
        /// <returns>Returns whether the Offense was correctly assigned to the Bot Animator</returns>
        bool GetIsNextOffenseApplied(HitConfirmBlockingData pHitConfirmBlockingData, Offense.Offense pNextOffense, AnimationClipOffenseType pAnimationClipOffenseType) {

            //Hitting or Blocking
            if (pHitConfirmBlockingData.isHitting || pHitConfirmBlockingData.isBlocking)
                return FEATURE_MANAGER.GetSpecificBotAnimationClipByType(_currentAttackerBotType) == pNextOffense.GetAnimationClip(AnimationClipOffenseType.Full);

            return FEATURE_MANAGER.GetSpecificBotAnimationClipByType(_currentAttackerBotType) == pNextOffense.GetAnimationClip(pAnimationClipOffenseType);
        }

        /// <summary>
        /// Allows you to check if BlockingData has been configured
        /// </summary>
        /// <returns>Returns if blockingData has been configured</returns>
        bool GetIsBlockingDataSetup()
        {
            //Checks if the defending Bot is the player
            if (GetIsBlockingDataSetup(FEATURE_MANAGER.GetCurrentEnemyBotType, FEATURE_MANAGER.GetSpecificOffenseManagerBotByType(FEATURE_MANAGER.GetCurrentEnemyBotType), BotType.SturdyBot, FEATURE_MANAGER.GetSpecificOffenseManagerBotByType(BotType.SturdyBot)))
                return true;

            //Checks if the defending Bot is the Enemy Bot
            if (GetIsBlockingDataSetup(BotType.SturdyBot, FEATURE_MANAGER.GetSpecificOffenseManagerBotByType(BotType.SturdyBot), FEATURE_MANAGER.GetCurrentEnemyBotType, FEATURE_MANAGER.GetSpecificOffenseManagerBotByType(FEATURE_MANAGER.GetCurrentEnemyBotType)))
                return true;

            if (FEATURE_MANAGER.GetSpecificOffenseManagerBotByType(BotType.SturdyBot).GetIsStance(FEATURE_MANAGER.GetSpecificOffenseManagerBotByType(BotType.SturdyBot).GetCurrentOffense))
                _currentCooldownTime = CooldownType.DISADVANTAGE;

            if (!_playerHitConfirmBlockingData.Equals(new HitConfirmBlockingData()))
                _playerHitConfirmBlockingData = new HitConfirmBlockingData();

            if (!_ennemyHitConfirmBlockingData.Equals(new HitConfirmBlockingData()))
                return true;

            return false;
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
                if (FEATURE_MANAGER.GetSpecificBotAnimationClipByType(_currentDefendingBotType) != GetDefendingHitConfirmBlockingData().blockingOffense.GetAnimationClip(AnimationClipOffenseType.Full))
                    return false;

                if (!FEATURE_MANAGER.GetSpecificOffenseManagerBotByType(BotType.SturdyBot).GetCurrentOffense.GetIsInDeflectionRange(FEATURE_MANAGER.GetSpecificAnimatorStateInfoByBotType(_currentDefendingBotType).normalizedTime))
                    return false;

                return true;
            }
        }

        public bool GetIsSturdyBotOnHittingMode => GetIsOutBlockingRange(GetSpecificHitConfirmBlockingDataByType(_currentAttackerBotType).offenseBlockingData.maxBlockingRangeData.rangeTime, FEATURE_MANAGER.GetSpecificAnimatorStateInfoByBotType(_currentAttackerBotType).normalizedTime);

        public BotType GetDefendingBotType => _currentDefendingBotType;
        public BotType GetAttackerBotType => _currentAttackerBotType;

        public bool GetIsHitConfirmActivated => _isHitConfirmActivated;

        AudioClip GetHitConfirmAudioClip()
        {

            //SturdyBot
            if (_currentDefendingBotType == BotType.SturdyBot)
            {
                //Blocking
                if (FEATURE_MANAGER.GetStateConfirmModule.GetSturdyStateBotData.stateConfirmMode == StateConfirmMode.Blocking)
                    return _blockingAudioClip;

                //Parray
                if (FEATURE_MANAGER.GetStateConfirmModule.GetSturdyStateBotData.stateConfirmMode == StateConfirmMode.Parry)
                    return _parryAudioClip;

                //Hitting
                return _hittingAudioClip;
            }

            //EnemyBot
            if (FEATURE_MANAGER.GetStateConfirmModule.GetIsCurrentEnemyBotOnBlockingMode)
                return _blockingAudioClip;

            return _hittingAudioClip;
        }

        #endregion

        #region Method

        public override void Initialize(FeatureManager pFeatureManager)
        {
            base.Initialize();

            _hitConfirmAudioSource = FEATURE_MANAGER.GetSturdyComponent.GetComponent<AudioSource>();
        }

        public override bool OnUpdate(bool pIsLeftFocus, bool pIsRightFocus)
        {
            if (!base.OnUpdate())
                return false;

            //Manage HitConfirm for each Bot when the HitConfirm is not activated
            if (!FEATURE_MANAGER.GetHitConfirmModule.GetIsHitConfirmActivated) 
            {
                if (FEATURE_MANAGER.GetSpecificBotAnimationClipByType(FEATURE_MANAGER.GetCurrentEnemyBotType) != FEATURE_MANAGER.GetSpecificOffenseManagerBotByType(FEATURE_MANAGER.GetCurrentEnemyBotType).GetCurrentOffense.GetAnimationClip(AnimationClipOffenseType.Full))
                    return false;

                if (GetIsBlockingDataSetup())
                    HitConfirmSetup();

                return true;
            }

            if (!_ifHitConfirmSpeedApplied)
            {
                _hitConfirmAudioSource.clip = GetHitConfirmAudioClip();

                FEATURE_MANAGER.GetSpecificBotAnimatorByType(BotType.SturdyBot).speed = 0;
                EnnemyBotHitConfirmAnimatorSpeed(0);

                _ifHitConfirmSpeedApplied = !_ifHitConfirmSpeedApplied;
            }

            HitConfirmSpeedSetup();

            return true;
        }

        /// <summary>
        /// Allows you to manage the activation and assignment of information concerning the HitConfirm
        /// </summary>
        /// <param name="pDefenderHitConfirmBlockingData">Information regarding the HitConfirm of the defending Bot</param>
        /// <param name="pFeatureCacheData">The basic cached information qi brings together all other feature modules</param>
        void HitConfirmSetup() {

            //Protection that checks if all information regarding the Defending Bot's HitConfirm has been assigned
            if (GetSpecificHitConfirmBlockingDataByType(_currentDefendingBotType).Equals(new HitConfirmBlockingData()))
                return;

            //Protection allowing to check if the clips of the two Bots have been correctly assigned in the Animator
            if (!GetIsNextOffenseApplied(GetSpecificHitConfirmBlockingDataByType(_currentAttackerBotType), FEATURE_MANAGER.GetSpecificOffenseManagerBotByType(_currentAttackerBotType).GetCurrentOffense, AnimationClipOffenseType.Full))
                return;

            //Checks if the attacking Bot's clip exceeds the minimum value of the blocking section
            if (FEATURE_MANAGER.GetSpecificAnimatorStateInfoByBotType(_currentAttackerBotType).normalizedTime < GetSpecificHitConfirmBlockingDataByType(_currentDefendingBotType).offenseBlockingData.minBlockingRangeData.rangeTime)
                return;

            _currentCooldownTime = CooldownType.NEUTRAL;

            HitConfirmDataCacheSetup();
        }

        /// <summary>
        /// Allows you to manage the waiting time in seconds before the HitConfirm ends
        /// </summary>
        /// <param name="pFeatureCacheData">The basic cached information qi brings together all other feature modules</param>
        void HitConfirmSpeedSetup() {

            if (!_hitConfirmAudioSource.isPlaying)
                _hitConfirmAudioSource.Play();

            _currentHitConfirmTime += Time.deltaTime;

            //Checks if the wait time matches with the timer
            if (_currentHitConfirmTime >= _currentMaxHitConfirmTimer) {

                _damageDataCache.sturdyDamageIntensity = 0;
                _damageDataCache.enemyDamageIntensity = 0;

                _currentHitConfirmTime = 0;

                //Reset the Animator speed to normal for Bots
                EnnemyBotHitConfirmAnimatorSpeed(1f);

                FEATURE_MANAGER.GetSpecificBotAnimatorByType(BotType.SturdyBot).speed = 1;

                _ifHitConfirmSpeedApplied = !_ifHitConfirmSpeedApplied;

                _ennemyHitConfirmBlockingData = new HitConfirmBlockingData();
                _playerHitConfirmBlockingData = new HitConfirmBlockingData();

                _isHitConfirmActivated = false;
            }

        }

        /// <summary>
        /// Allows the assignment of the Animator speed of all enemy Bots
        /// </summary>
        /// <param name="pSpeed">The speed value that we want to assign to the Animators of enemy Bots</param>
        void EnnemyBotHitConfirmAnimatorSpeed(float pSpeed) {

            for (byte i = 0; i < FEATURE_MANAGER.GetEnemyBotAnimator.Length; ++i)
                FEATURE_MANAGER.GetEnemyBotAnimator[i].speed = pSpeed;

            FEATURE_MANAGER.GetSpecificBotAnimatorByType(FEATURE_MANAGER.GetCurrentEnemyBotType).speed = pSpeed;
        }

        /// <summary>
        /// Allows the assignment of information regarding the HitConfirm status
        /// </summary>
        void HitConfirmDataCacheSetup() {

            _isHitConfirmActivated = true;

            _currentMaxHitConfirmTimer = _waitTimer;

            if (GetSpecificHitConfirmBlockingDataByType(_currentDefendingBotType).isHitting) 
            {
                if (_currentAttackerBotType == BotType.SturdyBot)
                    _damageDataCache.sturdyDamageIntensity = FEATURE_MANAGER.GetSpecificOffenseManagerBotByType(BotType.SturdyBot).GetLastOffense.GetCurrentDamageIntensity(BotType.SturdyBot);
            
                else
                    _damageDataCache.enemyDamageIntensity = FEATURE_MANAGER.GetSpecificOffenseManagerBotByType(FEATURE_MANAGER.GetCurrentEnemyBotType).GetCurrentOffense.GetCurrentDamageIntensity(FEATURE_MANAGER.GetCurrentEnemyBotType);
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

            drawer.Field("blockingOffense", false);
            drawer.Property("offenseBlockingData");

            drawer.EndProperty();
            return true;
        }
    }

#endif
}