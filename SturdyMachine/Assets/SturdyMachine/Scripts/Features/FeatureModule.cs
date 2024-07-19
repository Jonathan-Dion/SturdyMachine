using System;
using UnityEngine;

using SturdyMachine.Inputs;
using SturdyMachine.Offense.Blocking;
using SturdyMachine.Offense;
using SturdyMachine.Features.HitConfirm;
using SturdyMachine.Features.Fight;
using SturdyMachine.Features.Focus;
using SturdyMachine.Features.Fight.Sequence;



#if UNITY_EDITOR
using NWH.NUI;
using UnityEditor;
using NWH.VehiclePhysics2;
#endif

using SturdyMachine.Component;

namespace SturdyMachine.Features
{
    /// <summary>
    /// Alls features module categorys
    /// </summary>
    public enum FeatureModuleCategory { Focus, Fight, HitConfirm }

    [DisallowMultipleComponent]
    [Serializable]
    public abstract class FeatureModule : SturdyModuleComponent
    {
        #region Attribute

        public static FeatureManager FEATURE_MANAGER;

        public static FightOffenseSequenceManager FIGHT_OFFENSE_SEQUENCE_MANAGER;

        public static OffenseManager STURDYBOT_OFFENSE_MANAGER;

        public static OffenseManager[] ENEMYBOT_OFFENSE_MANAGER;

        protected Animator _currentEnemyAnimatorController;

        public static BotType[] ENEMYBOT_TYPE;

        #endregion

        #region Get

        /// <summary>
        /// FeatureModule type
        /// </summary>
        /// <returns>Return the current featureModule category</returns>
        public abstract FeatureModuleCategory GetFeatureModuleCategory();

        public Animator GetCurrentEnemyBotAnimatorController
        {
            get
            {
                if (!_currentEnemyAnimatorController)
                    _currentEnemyAnimatorController = FEATURE_MANAGER.GetFocusModule.GetCurrentEnemyBotFocus.GetComponent<Animator>();

                return _currentEnemyAnimatorController;
            }

        }

        /// <summary>
        /// Allows you to check the normalized time of the enemy Bot which matches that which is in Focus by the player
        /// </summary>
        /// <returns>Returns the normalized time of the enemy Bot's clip which matches that which is in Focus by the player</returns>
        public AnimatorStateInfo GetEnnemyBotAnimatorStateInfo => GetCurrentEnemyBotAnimatorController.GetCurrentAnimatorStateInfo(0);

        public AnimatorClipInfo GetCurrentEnemyBotAnimatorClipInfo => GetCurrentEnemyBotAnimatorController.GetCurrentAnimatorClipInfo(0)[0];

        public OffenseManager GetCurrentEnemyBotOffenseManager => ENEMYBOT_OFFENSE_MANAGER[FEATURE_MANAGER.GetFocusModule.GetCurrentEnemyBotIndex];

        public BotType GetCurrentEnemyBotType => ENEMYBOT_TYPE[FEATURE_MANAGER.GetFocusModule.GetCurrentEnemyBotIndex];

        /*public FocusDataCache GetFocusDataCache(FeatureCacheData pFeatureCacheData) => pFeatureCacheData.focusDataCache;

        public FightDataCache GetFightDataCache(FeatureCacheData pFeatureCacheData) => pFeatureCacheData.fightDataCache;

        public HitConfirmDataCache GetHitConfirmDataCache(FeatureCacheData pFeatureCacheData) => pFeatureCacheData.hitConfirmDataCache;

        public BotDataCache GetCurrentEnnemyBotDataFocus(ref FeatureCacheData pFeatureCacheData)
        {
            if (GetFocusDataCache(pFeatureCacheData).Equals(new FocusDataCache()))
                return new BotDataCache();

            if (!GetFocusDataCache(pFeatureCacheData).currentEnnemyBotFocus)
                return new BotDataCache();

            if (pFeatureCacheData.ennemyBotDataCache[GetFocusDataCache(pFeatureCacheData).currentEnnemyBotFocusIndex].botObject != pFeatureCacheData.focusDataCache.currentEnnemyBotFocus)
                SetEnnemyBotFocusIndex(ref pFeatureCacheData);

            return pFeatureCacheData.ennemyBotDataCache[GetFocusDataCache(pFeatureCacheData).currentEnnemyBotFocusIndex];
        }

        public Offense.Offense GetEnnemyBotOffense(FeatureCacheData pFeatureCacheData)
        {
            if (pFeatureCacheData.fightDataCache.currentFightOffenseData.offense.GetAnimationClip(GetEnnemyBotAnimator(ref pFeatureCacheData).GetCurrentAnimatorClipInfo(0)[0].clip.name))
                return pFeatureCacheData.fightDataCache.currentFightOffenseData.offense;

            return null;
        }

        public Animator GetEnnemyBotAnimator(ref FeatureCacheData pFeatureCacheData) => GetCurrentEnnemyBotDataFocus(ref pFeatureCacheData).botAnimator;

        public AnimationClip GetCurrentAnimationClipPlayed(BotDataCache pBotDataCache) => pBotDataCache.botAnimator.GetCurrentAnimatorClipInfo(0)[0].clip;

        public float GetCurrentNormalizedTime(BotDataCache pBotDataCache) => pBotDataCache.botAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime;

        public OffenseCategoryData GetEnnemyOffenseDamageHitCategoryData(BotDataCache pBotDataCache) => pBotDataCache.offenseManager.GetSpecificOffenseCategoryData(OffenseType.DAMAGEHIT);

        public bool GetIsEnemyBotPlayFightOffense(FeatureCacheData pFeatureCacheData) 
        {
            if (GetFocusDataCache(pFeatureCacheData).Equals(new FocusDataCache()))
                return false;

            return GetCurrentAnimationClipPlayed(GetCurrentEnnemyBotDataFocus(ref pFeatureCacheData)) == GetCurrentEnnemyBotDataFocus(ref pFeatureCacheData).offenseManager.GetCurrentOffense().GetAnimationClip(AnimationClipOffenseType.Full);
        }*/

        #endregion

        #region Method

        public virtual void Initialize(FeatureManager pFeatureManager, FightOffenseSequenceManager pFightOffenseSequenceManager, BotType[] pEnemyBotType)
        {
            FEATURE_MANAGER = pFeatureManager;
            FIGHT_OFFENSE_SEQUENCE_MANAGER = pFightOffenseSequenceManager;

            ENEMYBOT_TYPE = pEnemyBotType;

            base.Initialize();
        }

        public virtual bool OnStart(OffenseManager[] pEnemyBotOffenseManager) 
        {
            if (!base.OnStart())
                return false;

            ENEMYBOT_OFFENSE_MANAGER = pEnemyBotOffenseManager;

            return true;
        }

        public virtual bool OnUpdate(bool pIsLeftFocus, bool pIsRightFocus, Vector3 pFocusRange, OffenseBlockingConfig pOffenseBlockingConfig) {

            if (!base.OnUpdate())
                return false;

            return true;
        }

        protected virtual void BotLook(GameObject pBotObject, GameObject pLookAtBotObject, Vector3 pLookAtFocusRange) 
        {
            //Manages smooth rotation that allows the MonterBot to pivot quietly towards the player
            pBotObject.transform.position = Vector3.Lerp(pBotObject.transform.position, pLookAtBotObject.transform.position - pLookAtFocusRange, 0.5f);
            
            //Manages a smooth rotation that allows the player to pivot quietly towards the right target
            pBotObject.transform.rotation = Quaternion.Slerp(pBotObject.transform.rotation, Quaternion.LookRotation(pLookAtBotObject.transform.position - pBotObject.transform.position), 0.07f);

        }

        #endregion
    }

#if UNITY_EDITOR

    [CustomPropertyDrawer(typeof(FeatureModule), false)]
    public partial class FeatureModuleDrawer : ComponentNUIPropertyDrawer 
    {
        FeatureModule featureModule;

        public override bool OnNUI(Rect position, SerializedProperty property, GUIContent label)
        {
            drawer.BeginProperty(position, property, label);

            featureModule = SerializedPropertyHelper.GetTargetObjectOfProperty(drawer.serializedProperty) as FeatureModule;
            
            if (featureModule == null)
            {
                Debug.LogError(
                    "FeatureModule is not a target object of ModuleDrawer. Make sure all modules inherit from FeatureModule.");
                drawer.EndProperty();
                return false;
            }

            bool expanded = drawer.Header(featureModule.GetType().Name);

            return expanded;
        }
    }

#endif
}