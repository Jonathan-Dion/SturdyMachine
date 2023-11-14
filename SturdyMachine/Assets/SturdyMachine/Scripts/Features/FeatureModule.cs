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
    public struct BotData {

        public GameObject botObject;

        public OffenseManager offenseManager;

        public HitConfirmOffenseManager hitConfirmOffenseManager;

        public Animator animator;

        //EnnemyBot only!
        public Vector3 focusRange;
        public float blockingChance;
    }

    [DisallowMultipleComponent]
    [Serializable]
    public abstract class FeatureModule : SturdyModuleComponent
    {
        /// <summary>
        /// Alls features module categorys
        /// </summary>
        public enum FeatureModuleCategory { Focus, Fight, HitConfirm }

        protected bool _isLeftFocusActivated;

        protected bool _isRightFocusActivated;        

        protected BotData _sturdyBotData;

        protected BotData[] _ennemyBotData;

        protected OffenseBlockingConfig _offenseBlockingConfig;

        /// <summary>
        /// FeatureModule type
        /// </summary>
        /// <returns>Return the current featureModule category</returns>
        public abstract FeatureModuleCategory GetFeatureModuleCategory();

        public virtual void OnAwake(SturdyComponent pSturdyComponent, BotData[] pEnnemyBotData) {

            base.OnAwake(pSturdyComponent);

            _ennemyBotData = pEnnemyBotData;
        }

        public virtual void Initialize(BotData pStrudyBotData, SturdyInputControl pSturdyInputControl, OffenseBlockingConfig pOffenseBlockingConfig)
        {
            base.Initialize();

            _isLeftFocusActivated = pSturdyInputControl.GetIsLeftFocusActivated;

            _isRightFocusActivated = pSturdyInputControl.GetIsLeftFocusActivated;

            _sturdyBotData = pStrudyBotData;

            _offenseBlockingConfig = pOffenseBlockingConfig;
        }
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