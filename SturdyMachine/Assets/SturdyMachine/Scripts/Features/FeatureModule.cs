using System;
using UnityEngine;

#if UNITY_EDITOR
using NWH.NUI;
using UnityEditor;
using NWH.VehiclePhysics2;
#endif

using SturdyMachine.Component;

namespace SturdyMachine.Features
{
    [DisallowMultipleComponent]
    [Serializable]
    public abstract class FeatureModule : SturdyModuleComponent
    {
        /// <summary>
        /// Alls features module categorys
        /// </summary>
        public enum FeatureModuleCategory { Focus, Fight}

        /// <summary>
        /// FeatureModule type
        /// </summary>
        /// <returns>Return the current featureModule category</returns>
        public abstract FeatureModuleCategory GetFeatureModuleCategory();

        public virtual void Initialize(SturdyComponent pSturdyComponent) {

            base.Initialize();

            _sturdyComponent = pSturdyComponent;
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