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
    public enum FeatureModuleCategory { Focus, Fight, HitConfirm, StateConfirm, Audio }

    public enum StateConfirmMode { None, Blocking, Hitting, Stagger, Parry }

    [DisallowMultipleComponent]
    [Serializable]
    public abstract class FeatureModule : ModuleComponent
    {
        #region Attributes

        public static FeatureManager featureManager;

        #endregion

        #region Properties

        /// <summary>
        /// FeatureModule type
        /// </summary>
        /// <returns>Return the current featureModule category</returns>
        public abstract FeatureModuleCategory GetFeatureModuleCategory();

        #endregion

        #region Methods

        public virtual void Initialize(FeatureManager pFeatureManager)
        {
            base.Initialize();

            featureManager = pFeatureManager;
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