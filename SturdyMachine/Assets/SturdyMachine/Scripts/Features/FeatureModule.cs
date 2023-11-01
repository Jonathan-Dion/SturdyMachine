using System;
using UnityEngine;

using SturdyMachine.Inputs;
using SturdyMachine.Offense.Blocking;
using SturdyMachine.Offense;

#if UNITY_EDITOR
using NWH.NUI;
using UnityEditor;
using NWH.VehiclePhysics2;
#endif

using SturdyMachine.Component;

namespace SturdyMachine.Features
{
    public struct EnnemyBotData {

        public GameObject ennemyObject;

        public Vector3 focusRange;

        public OffenseManager offenseManager;

        public Animator animator;

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

        /// <summary>
        /// Transform player
        /// </summary>
        protected Transform _sturdyTransform;

        protected Animator _sturdyBotAnimator;

        protected OffenseBlockingConfig _offenseBlockingConfig;

        protected OffenseManager _sturdyOffenseManager;

        protected EnnemyBotData[] _ennemyBotData;

        /// <summary>
        /// FeatureModule type
        /// </summary>
        /// <returns>Return the current featureModule category</returns>
        public abstract FeatureModuleCategory GetFeatureModuleCategory();

        public virtual void OnAwake(SturdyComponent pSturdyComponent, GameObject[] pEnnemyBot, Vector3[] pEnnemyBotFocusRange, OffenseManager[] pEnnemyBotOffenseManager, Animator[] pEnnemyBotAnimator, float[] pEnnemyBotBlockingChance) {

            base.OnAwake(pSturdyComponent);

            EnnemyBotDataInit(pEnnemyBot, pEnnemyBotFocusRange, pEnnemyBotOffenseManager, pEnnemyBotAnimator, pEnnemyBotBlockingChance);
        }

        public virtual void Initialize(OffenseBlockingConfig pOffenseBlockingConfig, OffenseManager pSturdyOffenseManager, SturdyInputControl pSturdyInputControl, Transform pSturdyTransform, Animator pSturdyAnimator)
        {
            base.Initialize();

            _offenseBlockingConfig = pOffenseBlockingConfig;

            _sturdyOffenseManager = pSturdyOffenseManager;

            _isLeftFocusActivated = pSturdyInputControl.GetIsLeftFocusActivated;

            _isRightFocusActivated = pSturdyInputControl.GetIsLeftFocusActivated;

            _sturdyTransform = pSturdyTransform;

            _sturdyBotAnimator = pSturdyAnimator;
        }

        void EnnemyBotDataInit(GameObject[] pEnnemyBot, Vector3[] pEnnemyBotFocusRange, OffenseManager[] pEnnemyBotOffenseManager, Animator[] pEnnemyBotAnimator, float[] pEnnemyBotBlockingChance) {

            _ennemyBotData = new EnnemyBotData[pEnnemyBot.Length];

            for (byte i = 0; i < _ennemyBotData.Length; ++i) {

                _ennemyBotData[i] = new EnnemyBotData();

                _ennemyBotData[i].ennemyObject = pEnnemyBot[i];

                _ennemyBotData[i].focusRange = pEnnemyBotFocusRange[i];

                _ennemyBotData[i].offenseManager = pEnnemyBotOffenseManager[i];

                _ennemyBotData[i].animator = pEnnemyBotAnimator[i];

                _ennemyBotData[i].blockingChance = pEnnemyBotBlockingChance[i];
            }
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