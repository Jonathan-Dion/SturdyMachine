using System;

using UnityEngine;

using SturdyMachine.Offense.Blocking;
using SturdyMachine.Offense;
using NWH.VehiclePhysics2;
using UnityEditor.Experimental.GraphView;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace SturdyMachine.Features.HitConfirm {

    /// <summary>
    /// Identify the type of HitConfirm
    /// </summary>
    public enum HitConfirmType { None, Normal, Slow, Stop }

    [Serializable, Tooltip("")]
    public struct HitConfirmBlockingData {

        public Offense.Offense blockingOffense;

        public OffenseBlockingData offenseBlockingData;
    }

    [Serializable]
    public partial class HitConfirmModule : FeatureModule {

        #region Attribut

        [SerializeField, Tooltip("")]
        HitConfirmBlockingData _ennemyHitConfirmBlockingData;

        [SerializeField, Tooltip("")]
        HitConfirmBlockingData _playerHitConfirmBlockingData;

        #endregion

        #region Get

        public override FeatureModuleCategory GetFeatureModuleCategory() => FeatureModuleCategory.HitConfirm;

        bool GetIfThisBotAttack(BotDataCache pBotDataCache) {

            if (!pBotDataCache.offenseManager.GetCurrentOffense())
                return false;

            if (pBotDataCache.offenseManager.GetCurrentOffense().GetOffenseDirection == OffenseDirection.STANCE)
                return false;

            if (pBotDataCache.offenseManager.GetSpecificOffenseCategoryData(pBotDataCache.offenseManager.GetCurrentOffense().GetOffenseType).offenseCategoryType == Offense.OffenseType.DEFLECTION)
                return false;

            if (pBotDataCache.offenseManager.GetCurrentOffense().GetOffenseType == Offense.OffenseType.STANCE)
                return false;

            return true;
        }

        bool GetIfBlockingDataInit(BotDataCache pBotDataCache, ref HitConfirmBlockingData pHitConfirmBlockingData) {

            if (GetIfThisBotAttack(pBotDataCache)){

                if (!pHitConfirmBlockingData.Equals(new HitConfirmBlockingData()))
                    pHitConfirmBlockingData = new HitConfirmBlockingData();

                return true;
            }

            return false;
        }

        OffenseBlockingData GetAttackerOffenseBlockingData(BotDataCache pAttackerBotDataCache, OffenseBlockingConfig pOffenseBlockingConfig, out OffenseBlockingConfigData pOffenseBlockingConfigData) {

            pOffenseBlockingConfigData = new OffenseBlockingConfigData();

            for (int i = 0; i < pOffenseBlockingConfig.GetOffenseBlockingConfigData.Length; ++i)
            {
                for (int j = 0; j < pOffenseBlockingConfig.GetOffenseBlockingConfigData[i].offenseBlocking.Length; ++j)
                {

                    for (int k = 0; k < pOffenseBlockingConfig.GetOffenseBlockingConfigData[i].offenseBlocking[j].GetBlockingData.Length; ++k)
                    {

                        if (pOffenseBlockingConfig.GetOffenseBlockingConfigData[i].offenseBlocking[j].GetBlockingData[k].botType != pAttackerBotDataCache.botType)
                            continue;

                        for (int l = 0; l < pOffenseBlockingConfig.GetOffenseBlockingConfigData[i].offenseBlocking[j].GetBlockingData[k].offenseBlockingData.Length; ++l)
                        {

                            if (pAttackerBotDataCache.offenseManager.GetCurrentOffense() != pOffenseBlockingConfig.GetOffenseBlockingConfigData[i].offenseBlocking[j].GetBlockingData[k].offenseBlockingData[j].offense)
                                continue;

                            pOffenseBlockingConfigData = pOffenseBlockingConfig.GetOffenseBlockingConfigData[i];

                            return pOffenseBlockingConfig.GetOffenseBlockingConfigData[i].offenseBlocking[j].GetBlockingData[k].offenseBlockingData[j];
                        }
                    }
                }
            }

            return new OffenseBlockingData();
        }

        HitConfirmBlockingData GetHitConfirmBlockingData(BotDataCache pAttackerBotDataCache, BotDataCache pDefenderBotDataCache, OffenseBlockingConfig pOffenseBlockingConfig) {
        
            HitConfirmBlockingData hitConfirmBlockingData = new HitConfirmBlockingData();

            OffenseBlockingData attackerBlockingData = GetAttackerOffenseBlockingData(pAttackerBotDataCache, pOffenseBlockingConfig, out OffenseBlockingConfigData pOffenseBlockingConfigData);

            hitConfirmBlockingData.blockingOffense = pDefenderBotDataCache.offenseManager.GetOffense(pOffenseBlockingConfigData.offenseType, pOffenseBlockingConfigData.offenseDirection);

            hitConfirmBlockingData.offenseBlockingData = attackerBlockingData;

            return hitConfirmBlockingData;
        }

        bool GetIfBlockingDataSetup(BotDataCache pAttackerBotDataCache, ref HitConfirmBlockingData pAttackerHitConfirmBlockingData, ref HitConfirmBlockingData pDefenderHitConfirmBlockingData, ref FeatureCacheData pFeatureCacheData, OffenseBlockingConfig pOffenseBlockingConfig) {

            if (!pAttackerBotDataCache.Equals(new BotDataCache()))
            {
                if (GetIfBlockingDataInit(GetCurrentEnnemyBotDataFocus(ref pFeatureCacheData), ref pAttackerHitConfirmBlockingData))
                {
                    HitConfirmBlockingData hitConfirmBlockingData = GetHitConfirmBlockingData(GetCurrentEnnemyBotDataFocus(ref pFeatureCacheData), pFeatureCacheData.sturdyBotDataCache, pOffenseBlockingConfig);

                    if (!pDefenderHitConfirmBlockingData.Equals(hitConfirmBlockingData))
                        pDefenderHitConfirmBlockingData = hitConfirmBlockingData;

                    return true;
                }
            }

            return false;
        }

        #endregion

        #region Method

        public override void Initialize(ref FeatureCacheData pFeatureCacheData)
        {
            base.Initialize();

            pFeatureCacheData.hitConfirmDataCache = new HitConfirmDataCache();
        }

        public override bool OnUpdate(bool pIsLeftFocus, bool pIsRightFocus, OffenseBlockingConfig pOffenseBlockingConfig, ref FeatureCacheData pFeatureCacheData)
        {
            if (!base.OnUpdate())
                return false;

            BlockingDataSetup(ref pFeatureCacheData, pOffenseBlockingConfig);

            return true;
        }

        void BlockingDataSetup(ref FeatureCacheData pFeatureCacheData, OffenseBlockingConfig pOffenseBlockingConfig) {

            //Player
            if (GetIfBlockingDataSetup(pFeatureCacheData.sturdyBotDataCache, ref _ennemyHitConfirmBlockingData, ref _playerHitConfirmBlockingData, ref pFeatureCacheData, pOffenseBlockingConfig))
                return;

            //Ennemy
            if (GetIfBlockingDataSetup(GetCurrentEnnemyBotDataFocus(ref pFeatureCacheData), ref _playerHitConfirmBlockingData, ref _ennemyHitConfirmBlockingData, ref pFeatureCacheData, pOffenseBlockingConfig))
                return;

            if (!_playerHitConfirmBlockingData.Equals(new HitConfirmBlockingData()))
                _playerHitConfirmBlockingData = new HitConfirmBlockingData();

            if (!_ennemyHitConfirmBlockingData.Equals(new HitConfirmBlockingData()))
                _ennemyHitConfirmBlockingData = new HitConfirmBlockingData();
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