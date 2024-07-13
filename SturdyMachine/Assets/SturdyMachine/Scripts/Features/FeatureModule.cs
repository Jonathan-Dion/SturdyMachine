using System;
using UnityEngine;

using SturdyMachine.Inputs;
using SturdyMachine.Offense.Blocking;
using SturdyMachine.Offense;
using SturdyMachine.Features.HitConfirm;
using SturdyMachine.Features.Fight;

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
        #region Get

        /// <summary>
        /// FeatureModule type
        /// </summary>
        /// <returns>Return the current featureModule category</returns>
        public abstract FeatureModuleCategory GetFeatureModuleCategory();

        public FocusDataCache GetFocusDataCache(FeatureCacheData pFeatureCacheData) => pFeatureCacheData.focusDataCache;

        public FightDataCache GetFightDataCache(FeatureCacheData pFeatureCacheData) => pFeatureCacheData.fightDataCache;

        public HitConfirmDataCache GetHitConfirmDataCache(FeatureCacheData pFeatureCacheData) => pFeatureCacheData.hitConfirmDataCache;

        public BotDataCache GetCurrentEnnemyBotDataFocus(ref FeatureCacheData pFeatureCacheData)
        {
            if (pFeatureCacheData.focusDataCache.Equals(new FocusDataCache()))
                return new BotDataCache();

            if (!pFeatureCacheData.focusDataCache.currentEnnemyBotFocus)
                return new BotDataCache();

            if (pFeatureCacheData.ennemyBotDataCache[pFeatureCacheData.focusDataCache.currentEnnemyBotFocusIndex].botObject != pFeatureCacheData.focusDataCache.currentEnnemyBotFocus)
                SetEnnemyBotFocusIndex(ref pFeatureCacheData);

            return pFeatureCacheData.ennemyBotDataCache[pFeatureCacheData.focusDataCache.currentEnnemyBotFocusIndex];
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

        #endregion

        #region Method

        public virtual void Initialize(ref FeatureCacheData pFeatureCacheData)
        {
            base.Initialize();
        }

        public virtual bool OnUpdate(bool pIsLeftFocus, bool pIsRightFocus, OffenseBlockingConfig pOffenseBlockingConfig, ref FeatureCacheData pFeatureCacheData) {

            if (!base.OnUpdate())
                return false;

            return true;
        }

        public virtual bool OnFixedUpdate(bool pIsLeftFocus, bool pIsRightFocus, OffenseBlockingConfig pOffenseBlockingConfig, ref FeatureCacheData pFeatureCacheData) {

            if (!base.OnFixedUpdate())
                return false;

            return true;
        }

        /// <summary>
        /// Allows you to assign the index of the enemy Bot that is currently in the player's Focus
        /// </summary>
        /// <param name="pFeatureCacheData">The basic cached information qi brings together all other feature modules</param>
        void SetEnnemyBotFocusIndex(ref FeatureCacheData pFeatureCacheData)
        {
            for (int i = 0; i < pFeatureCacheData.ennemyBotDataCache.Length; ++i)
            {

                if (pFeatureCacheData.focusDataCache.currentEnnemyBotFocus != pFeatureCacheData.ennemyBotDataCache[i].botObject)
                    continue;

                pFeatureCacheData.focusDataCache.currentEnnemyBotFocusIndex = i;

                return;
            }
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