using System;
using UnityEngine;

using SturdyMachine.Inputs;
using SturdyMachine.Offense.Blocking;
using SturdyMachine.Offense;
using SturdyMachine.Features.HitConfirm;

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

    /// <summary>
    /// Saves all necessary bot elements to be able to operate modules with Bots
    /// </summary>
    [Serializable, Tooltip("Saves all necessary bot elements to be able to operate modules with Bots")]
    public struct BotData {

        /// <summary>
        /// Information about the bot's GameObject
        /// </summary>
        [Tooltip("Information about the bot's GameObject")]
        public GameObject botObject;

        /// <summary>
        /// Bot focus range information
        /// </summary>
        [Tooltip("Bot focus range information")]
        public Vector3 focusRange;
    }

    [DisallowMultipleComponent]
    [Serializable]
    public abstract class FeatureModule : SturdyModuleComponent
    {
        #region Attribut

        /// <summary>
        /// The list of information concerning all ennemyBot
        /// </summary>
        protected BotData[] _ennemyBotData;

        /// <summary>
        /// 
        /// </summary>
        protected BotData _sturdyBotData;

        #endregion

        #region Get

        /// <summary>
        /// FeatureModule type
        /// </summary>
        /// <returns>Return the current featureModule category</returns>
        public abstract FeatureModuleCategory GetFeatureModuleCategory();

        #endregion

        #region Method

        public virtual void Initialize(BotData pSturdyBotData, BotData[] pEnnemyBotData)
        {
            base.Initialize();

            _sturdyBotData = pSturdyBotData;

            _ennemyBotData = pEnnemyBotData;
        }

        public virtual bool OnUpdate(bool pIsLeftFocus, bool pIsRightFocus) {

            if (!base.OnUpdate())
                return false;

            return true;
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