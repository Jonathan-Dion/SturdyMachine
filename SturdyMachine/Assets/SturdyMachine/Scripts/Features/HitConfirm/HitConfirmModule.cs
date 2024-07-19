using System;

using UnityEngine;

using SturdyMachine.Offense.Blocking;
using SturdyMachine.Offense;
using SturdyMachine.Component;

using SturdyMachine.Features.Fight.Sequence;


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
    }

    /// <summary>
    /// Represents the module allowing the management of HitConfirm during a fight
    /// </summary>
    [Serializable, Tooltip("Represents the module allowing the management of HitConfirm during a fight")]
    public partial class HitConfirmModule : FeatureModule {

        #region Attribut

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
        float _currentHitConfirmTime;

        /// <summary>
        /// Protection allowing the speed to be correctly assigned to zero during HitConfirm
        /// </summary>
        bool _ifHitConfirmSpeedApplied;

        bool _isHitConfirmActivated;

        #endregion

        #region Get

        public override FeatureModuleCategory GetFeatureModuleCategory() => FeatureModuleCategory.HitConfirm;

        public bool GetIsHitConfirmActivated => _isHitConfirmActivated;

        /// <summary>
        /// Allows checking if the structure of the responding Bot needs to be initialized
        /// </summary>
        /// <param name="pHitConfirmBlockingData">The structure containing the information regarding the Defendant Bot's HitConfirm</param>
        /// <returns>Returns the state if the structure containing the Defending bot's HitConfirm information should be assigned</returns>
        bool GetIsBlockingDataInit(OffenseManager pCurrentOffenseManagerBot, HitConfirmBlockingData pHitConfirmBlockingData) {

            //Checks if the bot present is in the attack phase
            if (!pCurrentOffenseManagerBot.GetCurrentOffense().GetOffenseIsInAttackMode)
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
        OffenseBlockingData GetAttackerOffenseBlockingData(BotType pAttackerBotType, Offense.Offense pCurrentAttackerBotOffense, OffenseBlockingConfig pOffenseBlockingConfig, out OffenseBlockingConfigData pOffenseBlockingConfigData) {

            pOffenseBlockingConfigData = new OffenseBlockingConfigData();

            //Iterates through all OffenseBlockingConfig
            for (int i = 0; i < pOffenseBlockingConfig.GetOffenseBlockingConfigData.Length; ++i)
            {
                //Iterates through all OffenseBlocking
                for (int j = 0; j < pOffenseBlockingConfig.GetOffenseBlockingConfigData[i].offenseBlocking.Length; ++j)
                {
                    //Iterates through all blocking information based on Bot type
                    for (int k = 0; k < pOffenseBlockingConfig.GetOffenseBlockingConfigData[i].offenseBlocking[j].GetBlockingData.Length; ++k)
                    {
                        //Checks if the Bot type matches the attacking one
                        if (pOffenseBlockingConfig.GetOffenseBlockingConfigData[i].offenseBlocking[j].GetBlockingData[k].botType != pAttackerBotType)
                            continue;

                        //Iterates through the list of blocking sections based on the attacking bot
                        for (int l = 0; l < pOffenseBlockingConfig.GetOffenseBlockingConfigData[i].offenseBlocking[j].GetBlockingData[k].offenseBlockingData.Length; ++l)
                        {
                            //Checks if the Offense the attacking Bot is playing matches the one in the block list
                            if (pCurrentAttackerBotOffense != pOffenseBlockingConfig.GetOffenseBlockingConfigData[i].offenseBlocking[j].GetBlockingData[k].offenseBlockingData[l].offense)
                                continue;

                            //Assigns the correct OffenseBlocking information based on the Offense and the correct type of the attacking Bot
                            pOffenseBlockingConfigData = pOffenseBlockingConfig.GetOffenseBlockingConfigData[i];

                            //Returns the correct list of information about the blocking section of the Attacking Bot's Offense
                            return pOffenseBlockingConfig.GetOffenseBlockingConfigData[i].offenseBlocking[j].GetBlockingData[k].offenseBlockingData[l];
                        }
                    }
                }
            }

            return new OffenseBlockingData();
        }

        /// <summary>
        /// Allows you to assign the HitConfirm information of the defendant Bot
        /// </summary>
        /// <param name="pAttackerBotDataCache">The hidden information of the attacking bot</param>
        /// <param name="pDefenderBotDataCache">The hidden information of the defendant bot</param>
        /// <param name="pOffenseBlockingConfig">Structure for recording all necessary information regarding the attacking offense</param>
        /// <returns>Returns all information regarding the HitConfirm of the defending Bot</returns>
        HitConfirmBlockingData GetHitConfirmBlockingData(BotType pAttackerBotType, Offense.Offense pCurrentAttackerBotOffense, OffenseManager pDefenderBotOffenseManager, OffenseBlockingConfig pOffenseBlockingConfig) {
        
            HitConfirmBlockingData hitConfirmBlockingData = new HitConfirmBlockingData();

            //Assigns the correct information regarding the blocking section of the attacking Bot's Offense
            hitConfirmBlockingData.offenseBlockingData = GetAttackerOffenseBlockingData(pAttackerBotType, pCurrentAttackerBotOffense, pOffenseBlockingConfig, out OffenseBlockingConfigData pOffenseBlockingConfigData);

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
        bool GetIsBlockingDataSetup(BotType pAttackerBotType, ref HitConfirmBlockingData pAttackerHitConfirmBlockingData, OffenseManager pAttackerBotOffenseManager, ref HitConfirmBlockingData pDefenderHitConfirmBlockingData, OffenseManager pDefenderBotOffenseManager, OffenseBlockingConfig pOffenseBlockingConfig) {

            //Checks if information regarding OffenseBlocking of the defending Bot should be assigned
            if (GetIsBlockingDataInit(pAttackerBotOffenseManager, pDefenderHitConfirmBlockingData))
            {
                HitConfirmBlockingData hitConfirmBlockingData = GetHitConfirmBlockingData(pAttackerBotType, pAttackerBotOffenseManager.GetCurrentOffense(), pDefenderBotOffenseManager, pOffenseBlockingConfig);

                //Assigns information regarding the HitConfirm of the defending bot
                if (!pDefenderHitConfirmBlockingData.Equals(hitConfirmBlockingData))
                    pDefenderHitConfirmBlockingData = hitConfirmBlockingData;

                if (_isBlockComboOffense.Length != FEATURE_MANAGER.GetFightModule.GetCurrentOffenseAttackSequenceCount)
                    _isBlockComboOffense = new bool[FEATURE_MANAGER.GetFightModule.GetCurrentOffenseAttackSequenceCount];

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
        bool GetIsNextOffenseApplied(BotDataCache pBotDataCache, Offense.Offense pNextOffense, AnimationClipOffenseType pAnimationClipOffenseType) {

            //Hitting of Blocking
            if (pBotDataCache.isHitting || pBotDataCache.isBlocking)
                return GetCurrentAnimationClipPlayed(pBotDataCache) == pNextOffense.GetAnimationClip(AnimationClipOffenseType.Full);

            return GetCurrentAnimationClipPlayed(pBotDataCache) == pNextOffense.GetAnimationClip(pAnimationClipOffenseType);
        }

        /// <summary>
        /// Allows you to check if BlockingData has been configured
        /// </summary>
        /// <param name="pFeatureCacheData">The basic cached information qi brings together all other feature modules</param>
        /// <param name="pOffenseBlockingConfig">Structure for recording all necessary information regarding the attacking offense</param>
        /// <returns>Returns if blockingData has been configured</returns>
        bool GetIsBlockingDataSetup(OffenseBlockingConfig pOffenseBlockingConfig)
        {
            //Checks if the defending Bot is the player's Bot
            if (GetIsBlockingDataSetup(GetCurrentEnemyBotType, ref _ennemyHitConfirmBlockingData, GetCurrentEnemyBotOffenseManager, ref _playerHitConfirmBlockingData, STURDYBOT_OFFENSE_MANAGER, pOffenseBlockingConfig))
                return true;

            //Checks if the defending Bot is the Ennemy Bot
            if (GetIsBlockingDataSetup(BotType.SturdyBot, ref _playerHitConfirmBlockingData, STURDYBOT_OFFENSE_MANAGER, ref _ennemyHitConfirmBlockingData, GetCurrentEnemyBotOffenseManager, pOffenseBlockingConfig))
                return true;

            if (pFeatureCacheData.sturdyBotDataCache.offenseManager.GetIsStance(pFeatureCacheData.sturdyBotDataCache.offenseManager.GetCurrentOffense()))
                pFeatureCacheData.hitConfirmDataCache.currentCooldownType = CooldownType.DISADVANTAGE;

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

        AnimationClip GetKeyposeAnimationClip(BotDataCache pBotDataCache, bool pIsAttackingBot)
        {
            if (!pIsAttackingBot)
                return pBotDataCache.offenseManager.GetCurrentOffense().GetAnimationClip(AnimationClipOffenseType.KeyposeOut);

            if (pBotDataCache.botType == Component.BotType.SturdyBot)
                return pBotDataCache.offenseManager.GetCurrentOffense().GetAnimationClip(AnimationClipOffenseType.KeyposeOut);

            if (!GetIsCompletedBlockComboOffense())
                return pBotDataCache.offenseManager.GetCurrentOffense().GetAnimationClip(AnimationClipOffenseType.KeyposeOut);

            return pBotDataCache.offenseManager.GetCurrentOffense().GetAnimationClip(AnimationClipOffenseType.Stagger);
        }

        #endregion

        #region Method

        public override void Initialize(FeatureManager pFeatureManager, FightOffenseSequenceManager pFightOffenseSequenceManager, BotType[] pEnemyBotType)
        {
            base.Initialize();

            _currentBlockingOffenseIndex = 0;
        }

        public override bool OnUpdate(bool pIsLeftFocus, bool pIsRightFocus, Vector3 pFocusRange, OffenseBlockingConfig pOffenseBlockingConfig)
        {
            if (!base.OnUpdate())
                return false;

            //Manage HitConfirm for each Bot when the HitConfirm is not activated
            if (!FEATURE_MANAGER.GetHitConfirmModule.GetIsHitConfirmActivated)
            {
                if (!FEATURE_MANAGER.GetFightModule.GetIsEnemyBotPlayFightOffense)
                    return false;

                if (GetIsBlockingDataSetup(pOffenseBlockingConfig))
                    HitConfirmSetup(GetDefendingHitConfirmBlockingData(), ref pFeatureCacheData);

                return true;
            }

            if (!_ifHitConfirmSpeedApplied)
            {
                EnnemyBotHitConfirmAnimatorSpeed(ref pFeatureCacheData, 0);

                pFeatureCacheData.sturdyBotDataCache.botAnimator.speed = 0;

                _ifHitConfirmSpeedApplied = !_ifHitConfirmSpeedApplied;
            }

            HitConfirmSpeedSetup(ref pFeatureCacheData);

            return true;
        }

        /// <summary>
        /// Allows you to manage the activation and assignment of information concerning the HitConfirm
        /// </summary>
        /// <param name="pDefenderHitConfirmBlockingData">Information regarding the HitConfirm of the defending Bot</param>
        /// <param name="pFeatureCacheData">The basic cached information qi brings together all other feature modules</param>
        void HitConfirmSetup(HitConfirmBlockingData pDefenderHitConfirmBlockingData, ref FeatureCacheData pFeatureCacheData) {

            //Protection that checks if all information regarding the Defending Bot's HitConfirm has been assigned
            if (pDefenderHitConfirmBlockingData.Equals(new HitConfirmBlockingData()))
                return;

            //Protection allowing to check if the clips of the two Bots have been correctly assigned in the Animator
            if (!GetIsNextOffenseApplied(pFeatureCacheData.hitConfirmDataCache.attackingBotDataCache, GetHitConfirmDataCache(pFeatureCacheData).attackingBotDataCache.offenseManager.GetCurrentOffense(), AnimationClipOffenseType.Full))
                return;

            //Checks if the attacking Bot's clip exceeds the minimum value of the blocking section
            if (GetCurrentNormalizedTime(GetHitConfirmDataCache(pFeatureCacheData).attackingBotDataCache) < pDefenderHitConfirmBlockingData.offenseBlockingData.minBlockingRangeData.rangeTime)
                return;

            pFeatureCacheData.hitConfirmDataCache.currentCooldownType = CooldownType.NEUTRAL;

            //Blocking
            if (pFeatureCacheData.hitConfirmDataCache.defendingBotDataCache.botType == Component.BotType.SturdyBot)
            {
                if (pFeatureCacheData.hitConfirmDataCache.defendingBotDataCache.botAnimator.GetCurrentAnimatorClipInfo(0)[0].clip == GetDefendingHitConfirmBlockingData().blockingOffense.GetAnimationClip(AnimationClipOffenseType.Full)) {

                    if (pFeatureCacheData.sturdyBotDataCache.offenseManager.GetCurrentOffense().GetIsInDeflectionRange(GetCurrentNormalizedTime(pFeatureCacheData.hitConfirmDataCache.defendingBotDataCache))) {

                        _isBlockComboOffense[_currentBlockingOffenseIndex] = true;

                        HitConfirmDataCacheSetup(ref pFeatureCacheData, _blockingAudioClip, ref pFeatureCacheData.hitConfirmDataCache.defendingBotDataCache.isBlocking, pDefenderHitConfirmBlockingData);
                    }
                }
            }

            //Hitting
            HittingSetup(ref pFeatureCacheData, pDefenderHitConfirmBlockingData, GetCurrentNormalizedTime(pFeatureCacheData.hitConfirmDataCache.attackingBotDataCache));
        
        }

        void HittingSetup(ref FeatureCacheData pFeatureCacheData, HitConfirmBlockingData pDefenderHitConfirmBlackingData, float pDefendingNormalizedTime)
        {
            //Sturdy
            if (GetHitConfirmDataCache(pFeatureCacheData).defendingBotDataCache.botType == Component.BotType.SturdyBot){
                
                if (!GetHitConfirmDataCache(pFeatureCacheData).defendingBotDataCache.isBlocking){
                    if (GetIsOutBlockingRange(pDefenderHitConfirmBlackingData.offenseBlockingData.maxBlockingRangeData.rangeTime, pDefendingNormalizedTime))
                        HitConfirmDataCacheSetup(ref pFeatureCacheData, _hittingAudioClip, ref pFeatureCacheData.hitConfirmDataCache.defendingBotDataCache.isHitting, pDefenderHitConfirmBlackingData);
                }

                return;
            }

            //EnnemyBot
            System.Random random = new System.Random();

            int blockingChance = random.Next(0, 100);

            if (blockingChance > 50) {

                HitConfirmDataCacheSetup(ref pFeatureCacheData, _blockingAudioClip, ref pFeatureCacheData.hitConfirmDataCache.defendingBotDataCache.isBlocking, GetDefendingHitConfirmBlockingData());

                return;
            }

            //pFeatureCacheData.sturdyBotDataCache.offenseManager.SetCooldownDataType(CooldownType.ADVANTAGE);

            HitConfirmDataCacheSetup(ref pFeatureCacheData, _hittingAudioClip, ref pFeatureCacheData.hitConfirmDataCache.defendingBotDataCache.isHitting, GetDefendingHitConfirmBlockingData());
        }

        /// <summary>
        /// Allows you to manage the waiting time in seconds before the HitConfirm ends
        /// </summary>
        /// <param name="pFeatureCacheData">The basic cached information qi brings together all other feature modules</param>
        void HitConfirmSpeedSetup(ref FeatureCacheData pFeatureCacheData) {

            _currentHitConfirmTime += Time.deltaTime;

            //Checks if the wait time matches with the timer
            if (_currentHitConfirmTime >= GetHitConfirmDataCache(pFeatureCacheData).hitConfirmMaxTimer) {

                pFeatureCacheData.damageDataCache.sturdyDamageIntensity = 0;
                pFeatureCacheData.damageDataCache.enemyDamageIntensity = 0;

                _currentHitConfirmTime = 0;

                //Reset the Animator speed to normal for Bots
                EnnemyBotHitConfirmAnimatorSpeed(ref pFeatureCacheData, 1f);

                pFeatureCacheData.sturdyBotDataCache.botAnimator.speed = 1;

                if (_currentBlockingOffenseIndex == _isBlockComboOffense.Length) {

                    if (GetIsCompletedBlockComboOffense())
                    {
                        pFeatureCacheData.sturdyBotDataCache.botAnimator.Play(pFeatureCacheData.sturdyBotDataCache.offenseManager.GetCurrentOffense().GetAnimationClip(AnimationClipOffenseType.Parry).name);

                        pFeatureCacheData.audioSource.clip = _parryAudioClip;

                        pFeatureCacheData.audioSource.Play();
                    }

                    _isBlockComboOffense = new bool[0];
                    _currentBlockingOffenseIndex = 0;
                }

                _ifHitConfirmSpeedApplied = !_ifHitConfirmSpeedApplied;

                _ennemyHitConfirmBlockingData = new HitConfirmBlockingData();

                pFeatureCacheData.hitConfirmDataCache.isInHitConfirm = false;
            }

        }

        /// <summary>
        /// Allows the assignment of Offenses corresponding to the HitConfirm situation to the Bot
        /// </summary>
        /// <param name="pBotDataCache">Cached Bot Information</param>
        /// <param name="pDefenderHitConfirmBlockingData">Information regarding the HitConfirm of the defending Bot</param>
        void ActivateHitConfirm(ref BotDataCache pBotDataCache, HitConfirmBlockingData pDefenderHitConfirmBlockingData, ref DamageDataCache pDamageDataCache, bool pIsAttackerBot) {

            AnimationClip keyposeClip = null;

            //Allows you to assign the Offense that corresponds to the Bot that should be hit
            if (pBotDataCache.isHitting) {

                pBotDataCache.offenseManager.CurrentOffenseClipNameSetup(pBotDataCache.offenseManager.GetOffense(OffenseType.DAMAGEHIT, pDefenderHitConfirmBlockingData.blockingOffense.GetOffenseDirection).GetAnimationClip(AnimationClipOffenseType.Full).name);

                if (pBotDataCache.botAnimator.GetCurrentAnimatorClipInfo(0)[0].clip != pBotDataCache.offenseManager.GetCurrentOffense().GetAnimationClip(AnimationClipOffenseType.Full))
                    pBotDataCache.botAnimator.Play(pBotDataCache.offenseManager.GetCurrentOffense().GetAnimationClip(AnimationClipOffenseType.Full).name);
                else
                    pBotDataCache.botAnimator.Play(pBotDataCache.offenseManager.GetCurrentOffense().GetAnimationClip(AnimationClipOffenseType.Full).name, -1, 0f);

                return;
            }

            //Allows you to assign the Offense that corresponds to the Bot that should be block
            if (pBotDataCache.isBlocking){

                pBotDataCache.offenseManager.CurrentOffenseClipNameSetup(pBotDataCache.offenseManager.GetOffense(pDefenderHitConfirmBlockingData.blockingOffense.GetOffenseType, pDefenderHitConfirmBlockingData.blockingOffense.GetOffenseDirection).GetAnimationClip(pBotDataCache.botType == Component.BotType.SturdyBot ? AnimationClipOffenseType.KeyposeOut : AnimationClipOffenseType.Full).name);          

                pBotDataCache.botAnimator.Play(pBotDataCache.offenseManager.GetCurrentOffense().GetAnimationClip(pBotDataCache.botType == Component.BotType.SturdyBot ? AnimationClipOffenseType.KeyposeOut : AnimationClipOffenseType.Full).name);

                return;
            }

            keyposeClip = GetKeyposeAnimationClip(pBotDataCache, pIsAttackerBot);

            if (!keyposeClip)
                return;

            //Allows the assignment of the Offense that corresponds to the attacking Bot
            pBotDataCache.botAnimator.Play(keyposeClip.name);
            pBotDataCache.offenseManager.CurrentOffenseClipNameSetup(keyposeClip.name);
        }

        /// <summary>
        /// Allows the assignment of the Animator speed of all enemy Bots
        /// </summary>
        /// <param name="pEnnemyBotDataCache">The list of cached enemy Bots</param>
        /// <param name="pSpeed">The speed value that we want to assign to the Animators of enemy Bots</param>
        void EnnemyBotHitConfirmAnimatorSpeed(ref FeatureCacheData pFeatureCacheData, float pSpeed) {

            for (byte i = 0; i < pFeatureCacheData.ennemyBotDataCache.Length; ++i)

                pFeatureCacheData.ennemyBotDataCache[i].botAnimator.speed = pSpeed;
        }

        /// <summary>
        /// Allows the assignment of information regarding the HitConfirm status
        /// </summary>
        /// <param name="pFeatureCacheData">The basic cached information qi brings together all other feature modules</param>
        /// <param name="pHitConfirmAudioClip">The audioClip that the HitConfirm should play</param>
        /// <param name="pHitConfirmState">The state of HitConfirm that must be changed</param>
        /// <param name="pDefenderHitConfirmBlockingData">Information regarding the HitConfirm of the defending Bot</param>
        void HitConfirmDataCacheSetup(ref FeatureCacheData pFeatureCacheData, AudioClip pHitConfirmAudioClip, ref bool pHitConfirmState, HitConfirmBlockingData pDefenderHitConfirmBlockingData) {

            pFeatureCacheData.hitConfirmDataCache.isInHitConfirm = true;

            pFeatureCacheData.hitConfirmDataCache.hitConfirmMaxTimer = _waitTimer;
            pHitConfirmState = true;

            pFeatureCacheData.audioSource.clip = pHitConfirmAudioClip;

            pFeatureCacheData.audioSource.Play();

            //Assigns the Offenses corresponding to the HitConfirm situation to the attacking Bot
            ActivateHitConfirm(ref pFeatureCacheData.hitConfirmDataCache.attackingBotDataCache, pDefenderHitConfirmBlockingData, ref pFeatureCacheData.damageDataCache, true);

            //Assigns the Offenses corresponding to the HitConfirm situation to the defending Bot
            ActivateHitConfirm(ref pFeatureCacheData.hitConfirmDataCache.defendingBotDataCache, pDefenderHitConfirmBlockingData, ref pFeatureCacheData.damageDataCache, false);

            if (GetHitConfirmDataCache(pFeatureCacheData).defendingBotDataCache.isHitting) 
            {
                if (GetHitConfirmDataCache(pFeatureCacheData).attackingBotDataCache.botType == Component.BotType.SturdyBot)
                    pFeatureCacheData.damageDataCache.sturdyDamageIntensity = GetHitConfirmDataCache(pFeatureCacheData).attackingBotDataCache.offenseManager.GetLastOffense.GetCurrentDamageIntensity(Component.BotType.SturdyBot);
            
                else
                    pFeatureCacheData.damageDataCache.enemyDamageIntensity = GetHitConfirmDataCache(pFeatureCacheData).attackingBotDataCache.offenseManager.GetCurrentOffense().GetCurrentDamageIntensity(Component.BotType.SkinnyBot);
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