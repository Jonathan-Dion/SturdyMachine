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

        [SerializeField]
        AudioClip _parryAudioClip;

        /// <summary>
        /// Represents the audioSource used to play the AudioClips depending on the HitConfirm state
        /// </summary>
        [SerializeField, Tooltip("Represents the audioSource used to play the AudioClips depending on the HitConfirm state")]
        AudioSource _hitConfirmAudioSource;

        [SerializeField]
        float _waitTimer;

        [SerializeField]
        bool[] _isBlockComboOffense;

        [SerializeField]
        int _currentBlockingOffenseIndex;

        /// <summary>
        /// Represents the time in seconds present for the timer
        /// </summary>
        float _currentHitConfirmTime, _currentMaxHitConfirmTimer;

        /// <summary>
        /// Protection allowing the speed to be correctly assigned to zero during HitConfirm
        /// </summary>
        bool _ifHitConfirmSpeedApplied;

        bool _isHitConfirmActivated;

        BotType _currentAttackerBotType, _currentDefendingBotType;

        CooldownType _currentCooldownTime;

        DamageDataCache _damageDataCache;

        #endregion

        #region Properties

        public DamageDataCache GetDamageDataCache => _damageDataCache;

        public CooldownType GetCurrentCooldownType => _currentCooldownTime;

        public override FeatureModuleCategory GetFeatureModuleCategory() => FeatureModuleCategory.HitConfirm;

        public bool GetIsHitConfirmActivated => _isHitConfirmActivated;

        /// <summary>
        /// Allows checking if the structure of the responding Bot needs to be initialized
        /// </summary>
        /// //<param name="pBotOffenseManager">The offenseManager of the bot you want to check</param>
        /// <param name="pHitConfirmBlockingData">The structure containing the information regarding the Defendant Bot's HitConfirm</param>
        /// <returns>Returns the state if the structure containing the Defending bot's HitConfirm information should be assigned</returns>
        bool GetIsBlockingDataInit(OffenseManager pBotOffenseManager, HitConfirmBlockingData pHitConfirmBlockingData) {

            //Checks if the bot present is in the attack phase
            if (!pBotOffenseManager.GetCurrentOffense.GetOffenseIsInAttackMode)
                return false;

            return pHitConfirmBlockingData.Equals(new HitConfirmBlockingData());
        }

        /// <summary>
        /// Allows you to assign the correct information regarding blocking the attacking bot's offense
        /// </summary>
        /// <param name="pAttackerBotDataCache">The hidden information of the attacking bot</param>
        /// <param name="pOffenseBlockingConfig">The scriptableObject which has all the information regarding the blocking values ​​of all bot types and all offenses</param>
        /// <param name="pOffenseBlockingConfigData">Structure for recording all necessary information regarding the attacking offense</param>
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
        /// <param name="pAttackerBotDataCache">The hidden information of the attacking bot</param>
        /// <param name="pAttackerHitConfirmBlockingData">The structure of the HitConfirm of the attacking Bot</param>
        /// <param name="pDefenderBotDataCache">The hidden information of the defending bot</param>
        /// <param name="pDefenderHitConfirmBlockingData">The structure of the HitConfirm of the defending Bot</param>
        /// <param name="pFeatureCacheData">The basic cached information qi brings together all other feature modules</param>
        /// <param name="pOffenseBlockingConfig">Structure for recording all necessary information regarding the attacking offense</param>
        /// <returns>Returns whether the HitConfirm of the attacking and defending Bot were assigned correctly</returns>
        bool GetIsBlockingDataSetup(BotType pAttackerBotType, OffenseManager pAttackerBotOffenseManager, BotType pDefenderBotType, OffenseManager pDefenderBotOffenseManager) {

            //Checks if information regarding OffenseBlocking of the defending Bot should be assigned
            if (GetIsBlockingDataInit(pAttackerBotOffenseManager, GetSpecificHitConfirmBlockingDataByType(pDefenderBotType)))
            {
                HitConfirmBlockingData hitConfirmBlockingData = GetHitConfirmBlockingData(pAttackerBotType, pAttackerBotOffenseManager, pDefenderBotOffenseManager);

                //Assigns information regarding the HitConfirm of the defending bot
                if (!GetSpecificHitConfirmBlockingDataByType(pDefenderBotType).Equals(hitConfirmBlockingData))
                {
                    //Sturdy
                    if (pDefenderBotType == BotType.SturdyBot)
                        _playerHitConfirmBlockingData = hitConfirmBlockingData;

                    else
                        _ennemyHitConfirmBlockingData = hitConfirmBlockingData;

                    //AttackerBot
                    _currentAttackerBotType = pAttackerBotType;

                    //DefenderBot
                    _currentDefendingBotType = pDefenderBotType;
                }

                if (_isBlockComboOffense.Length != FEATURE_MANAGER.GetFightsModule.GetNbrOfEnemyBotOffenseBlocking)
                    _isBlockComboOffense = new bool[FEATURE_MANAGER.GetFightsModule.GetNbrOfEnemyBotOffenseBlocking];

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
        HitConfirmBlockingData GetDefendingHitConfirmBlockingData() {

            //Player
            if (!_playerHitConfirmBlockingData.Equals(new HitConfirmBlockingData()))
                return _playerHitConfirmBlockingData;

            //Ennemy
            return _ennemyHitConfirmBlockingData;
        }

        /// <summary>
        /// Checks if the Offense change during HitConfirm was made
        /// </summary>
        /// <param name="pBotDataCache">A bot's botDataCache checked</param>
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
            //Checks if the defending Bot is the player's Bot
            if (GetIsBlockingDataSetup(BotType.SturdyBot, FEATURE_MANAGER.GetSturdyBotOffenseManager, FEATURE_MANAGER.GetCurrentEnemyBotType, FEATURE_MANAGER.GetCurrentEnemyBotOffenseManager))
                return true;

            //Checks if the defending Bot is the Ennemy Bot
            if (GetIsBlockingDataSetup(FEATURE_MANAGER.GetCurrentEnemyBotType, FEATURE_MANAGER.GetCurrentEnemyBotOffenseManager, BotType.SturdyBot, FEATURE_MANAGER.GetSturdyBotOffenseManager))
                return true;

            if (FEATURE_MANAGER.GetSturdyBotOffenseManager.GetIsStance(FEATURE_MANAGER.GetSturdyBotOffenseManager.GetCurrentOffense))
                _currentCooldownTime = CooldownType.DISADVANTAGE;

            if (!_playerHitConfirmBlockingData.Equals(new HitConfirmBlockingData()))
                _playerHitConfirmBlockingData = new HitConfirmBlockingData();

            if (!_ennemyHitConfirmBlockingData.Equals(new HitConfirmBlockingData()))
                return true;

            return false;
        }

        bool GetIsCompletedBlockComboOffense() {

            for (byte i = 0; i < _isBlockComboOffense.Length; ++i) {

                if (_isBlockComboOffense[i])
                    continue;

                return false;
            }

            return true;
        }

        AnimationClip GetKeyposeAnimationClip(BotType pCurrentBotType, OffenseManager pBotOffenseManager, bool pIsAttackingBot)
        {
            if (!pIsAttackingBot)
                return pBotOffenseManager.GetCurrentOffense.GetAnimationClip(AnimationClipOffenseType.KeyposeOut);

            if (pCurrentBotType == BotType.SturdyBot)
                return pBotOffenseManager.GetCurrentOffense.GetAnimationClip(AnimationClipOffenseType.KeyposeOut);

            if (!GetIsCompletedBlockComboOffense())
                return pBotOffenseManager.GetCurrentOffense.GetAnimationClip(AnimationClipOffenseType.KeyposeOut);

            return pBotOffenseManager.GetCurrentOffense.GetAnimationClip(AnimationClipOffenseType.Stagger);
        }

        OffenseManager GetSpecificOffenseManagerByType(BotType pSpecificBotType)
        {
            //Sturdy
            if (pSpecificBotType == BotType.SturdyBot)
                return FEATURE_MANAGER.GetSturdyBotOffenseManager;

            //SkinnyBot
            return FEATURE_MANAGER.GetCurrentEnemyBotOffenseManager;
        }

        public HitConfirmBlockingData GetSpecificHitConfirmBlockingDataByType(BotType pSpecificBotType) 
        {
            //Sturdy
            if (pSpecificBotType == BotType.SturdyBot)
                return _playerHitConfirmBlockingData;

            return _ennemyHitConfirmBlockingData;
        }

        Animator GetSpecificBotAnimatorByType(BotType pSpecificBotType) 
        {
            //Sturdy
            if (pSpecificBotType == BotType.SturdyBot)
                return FEATURE_MANAGER.GetSturdyBotAnimator;

            return FEATURE_MANAGER.GetCurrentEnemyBotAnimator;
        }

        #endregion

        #region Method

        public override void Initialize()
        {
            base.Initialize();

            _currentBlockingOffenseIndex = 0;
        }

        public override bool OnUpdate(bool pIsLeftFocus, bool pIsRightFocus)
        {
            if (!base.OnUpdate())
                return false;

            //Manage HitConfirm for each Bot when the HitConfirm is not activated
            if (!FEATURE_MANAGER.GetHitConfirmModule.GetIsHitConfirmActivated) 
            {
                if (!FEATURE_MANAGER.GetCurrentEnemyBotOffenseManager.GetCurrentOffense)
                    return false;

                if (FEATURE_MANAGER.GetCurrentEnemyBotAnimationClip != FEATURE_MANAGER.GetCurrentEnemyBotOffenseManager.GetCurrentOffense.GetAnimationClip(AnimationClipOffenseType.Full))
                    return false;

                if (GetIsBlockingDataSetup(_currentAttackerBotType, GetSpecificOffenseManagerByType(_currentAttackerBotType), _currentDefendingBotType, GetSpecificOffenseManagerByType(_currentDefendingBotType)))
                    HitConfirmSetup();

                return false;
            }

            if (!_ifHitConfirmSpeedApplied)
            {
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

            //Blocking
            if (_currentDefendingBotType == BotType.SturdyBot)
            {
                if (FEATURE_MANAGER.GetSpecificBotAnimationClipByType(_currentDefendingBotType) == GetDefendingHitConfirmBlockingData().blockingOffense.GetAnimationClip(AnimationClipOffenseType.Full)) {

                    if (FEATURE_MANAGER.GetSturdyBotOffenseManager.GetCurrentOffense.GetIsInDeflectionRange(FEATURE_MANAGER.GetSpecificAnimatorStateInfoByBotType(_currentDefendingBotType).normalizedTime)) {

                        _isBlockComboOffense[_currentBlockingOffenseIndex] = true;

                        HitConfirmDataCacheSetup(_blockingAudioClip);
                    }
                }
            }

            //Hitting
            HittingSetup(GetSpecificHitConfirmBlockingDataByType(_currentDefendingBotType));
        
        }

        void HittingSetup(HitConfirmBlockingData pDefenderHitConfirmBlackingData)
        {
            //Sturdy
            if (_currentDefendingBotType == BotType.SturdyBot){
                
                if (!GetSpecificHitConfirmBlockingDataByType(_currentDefendingBotType).isBlocking){
                    
                    if (GetIsOutBlockingRange(pDefenderHitConfirmBlackingData.offenseBlockingData.maxBlockingRangeData.rangeTime, FEATURE_MANAGER.GetSpecificAnimatorStateInfoByBotType(_currentDefendingBotType).normalizedTime))
                        HitConfirmDataCacheSetup(_hittingAudioClip);
                }

                return;
            }

            //EnnemyBot
            System.Random random = new System.Random();

            int blockingChance = random.Next(0, 100);

            if (blockingChance > 50) {

                HitConfirmDataCacheSetup(_blockingAudioClip);

                return;
            }

            //pFeatureCacheData.sturdyBotDataCache.offenseManager.SetCooldownDataType(CooldownType.ADVANTAGE);

            HitConfirmDataCacheSetup(_hittingAudioClip);
        }

        /// <summary>
        /// Allows you to manage the waiting time in seconds before the HitConfirm ends
        /// </summary>
        /// <param name="pFeatureCacheData">The basic cached information qi brings together all other feature modules</param>
        void HitConfirmSpeedSetup() {

            _currentHitConfirmTime += Time.deltaTime;

            //Checks if the wait time matches with the timer
            if (_currentHitConfirmTime >= _currentMaxHitConfirmTimer) {

                _damageDataCache.sturdyDamageIntensity = 0;
                _damageDataCache.enemyDamageIntensity = 0;

                _currentHitConfirmTime = 0;

                //Reset the Animator speed to normal for Bots
                EnnemyBotHitConfirmAnimatorSpeed(1f);

                if (_currentBlockingOffenseIndex == _isBlockComboOffense.Length) {

                    if (GetIsCompletedBlockComboOffense())
                    {
                        FEATURE_MANAGER.GetSturdyBotAnimator.Play(FEATURE_MANAGER.GetSturdyBotOffenseManager.GetCurrentOffense.GetAnimationClip(AnimationClipOffenseType.Parry).name);

                        _hitConfirmAudioSource.clip = _parryAudioClip;

                        _hitConfirmAudioSource.Play();
                    }

                    _isBlockComboOffense = new bool[0];
                    _currentBlockingOffenseIndex = 0;
                }

                _ifHitConfirmSpeedApplied = !_ifHitConfirmSpeedApplied;

                _ennemyHitConfirmBlockingData = new HitConfirmBlockingData();

                _isHitConfirmActivated = false;
            }

        }

        /// <summary>
        /// Allows the assignment of Offenses corresponding to the HitConfirm situation to the Bot
        /// </summary>
        /// <param name="pBotDataCache">Cached Bot Information</param>
        /// <param name="pDefenderHitConfirmBlockingData">Information regarding the HitConfirm of the defending Bot</param>
        void ActivateHitConfirm(BotType pBotType, HitConfirmBlockingData pDefenderHitConfirmBlockingData, bool pIsAttackerBot) {

            AnimationClip keyposeClip = null;

            //Allows you to assign the Offense that corresponds to the Bot that should be hit
            if (pDefenderHitConfirmBlockingData.isHitting) {

                FEATURE_MANAGER.GetSpecificOffenseManagerBotByType(pBotType).CurrentOffenseClipNameSetup(FEATURE_MANAGER.GetSpecificOffenseManagerBotByType(pBotType).GetOffense(OffenseType.DAMAGEHIT, pDefenderHitConfirmBlockingData.blockingOffense.GetOffenseDirection).GetAnimationClip(AnimationClipOffenseType.Full).name);

                if (FEATURE_MANAGER.GetSpecificBotAnimationClipByType(pBotType) != FEATURE_MANAGER.GetSpecificOffenseManagerBotByType(pBotType).GetCurrentOffense.GetAnimationClip(AnimationClipOffenseType.Full))
                    GetSpecificBotAnimatorByType(pBotType).Play(FEATURE_MANAGER.GetSpecificOffenseManagerBotByType(pBotType).GetCurrentOffense.GetAnimationClip(AnimationClipOffenseType.Full).name);
                else
                    GetSpecificBotAnimatorByType(pBotType).Play(FEATURE_MANAGER.GetSpecificOffenseManagerBotByType(pBotType).GetCurrentOffense.GetAnimationClip(AnimationClipOffenseType.Full).name, -1, 0f);

                return;
            }

            //Allows you to assign the Offense that corresponds to the Bot that should be block
            if (pDefenderHitConfirmBlockingData.isBlocking){

                FEATURE_MANAGER.GetSpecificOffenseManagerBotByType(pBotType).CurrentOffenseClipNameSetup(FEATURE_MANAGER.GetSpecificOffenseManagerBotByType(pBotType).GetOffense(pDefenderHitConfirmBlockingData.blockingOffense.GetOffenseType, pDefenderHitConfirmBlockingData.blockingOffense.GetOffenseDirection).GetAnimationClip(pBotType == BotType.SturdyBot ? AnimationClipOffenseType.KeyposeOut : AnimationClipOffenseType.Full).name);

                GetSpecificBotAnimatorByType(pBotType).Play(FEATURE_MANAGER.GetSpecificOffenseManagerBotByType(pBotType).GetCurrentOffense.GetAnimationClip(pBotType == BotType.SturdyBot ? AnimationClipOffenseType.KeyposeOut : AnimationClipOffenseType.Full).name);

                return;
            }

            keyposeClip = GetKeyposeAnimationClip(pBotType, GetSpecificOffenseManagerByType(pBotType), pIsAttackerBot);

            if (!keyposeClip)
                return;

            //Allows the assignment of the Offense that corresponds to the attacking Bot
            GetSpecificBotAnimatorByType(pBotType).Play(keyposeClip.name);
            GetSpecificOffenseManagerByType(pBotType).CurrentOffenseClipNameSetup(keyposeClip.name);
        }

        /// <summary>
        /// Allows the assignment of the Animator speed of all enemy Bots
        /// </summary>
        /// <param name="pSpeed">The speed value that we want to assign to the Animators of enemy Bots</param>
        void EnnemyBotHitConfirmAnimatorSpeed(float pSpeed) {

            for (byte i = 0; i < FEATURE_MANAGER.GetEnemyBotAnimator.Length; ++i)
                FEATURE_MANAGER.GetEnemyBotAnimator[i].speed = pSpeed;

            FEATURE_MANAGER.GetSturdyBotAnimator.speed = pSpeed;
        }

        /// <summary>
        /// Allows the assignment of information regarding the HitConfirm status
        /// </summary>
        /// <param name="pFeatureCacheData">The basic cached information qi brings together all other feature modules</param>
        /// <param name="pHitConfirmAudioClip">The audioClip that the HitConfirm should play</param>
        /// <param name="pHitConfirmState">The state of HitConfirm that must be changed</param>
        /// <param name="pDefenderHitConfirmBlockingData">Information regarding the HitConfirm of the defending Bot</param>
        void HitConfirmDataCacheSetup(AudioClip pHitConfirmAudioClip) {

            _isHitConfirmActivated = true;

            _currentMaxHitConfirmTimer = _waitTimer;

            _hitConfirmAudioSource.clip = pHitConfirmAudioClip;

            _hitConfirmAudioSource.Play();

            //Assigns the Offenses corresponding to the HitConfirm situation to the attacking Bot
            ActivateHitConfirm(_currentAttackerBotType, GetSpecificHitConfirmBlockingDataByType(_currentAttackerBotType), true);

            //Assigns the Offenses corresponding to the HitConfirm situation to the defending Bot
            ActivateHitConfirm(_currentDefendingBotType, GetSpecificHitConfirmBlockingDataByType(_currentDefendingBotType), false);

            if (GetSpecificHitConfirmBlockingDataByType(_currentDefendingBotType).isHitting) 
            {
                if (_currentAttackerBotType == BotType.SturdyBot)
                    _damageDataCache.sturdyDamageIntensity = FEATURE_MANAGER.GetSturdyBotOffenseManager.GetLastOffense.GetCurrentDamageIntensity(BotType.SturdyBot);
            
                else
                    _damageDataCache.enemyDamageIntensity = FEATURE_MANAGER.GetCurrentEnemyBotOffenseManager.GetCurrentOffense.GetCurrentDamageIntensity(FEATURE_MANAGER.GetCurrentEnemyBotType);
            }

            ++_currentBlockingOffenseIndex;
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

            drawer.Field("_currentBlockingOffenseIndex", false);

            drawer.ReorderableList("_isBlockComboOffense");

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