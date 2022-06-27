using System;
using UnityEngine;

#if UNITY_EDITOR
using NWH.NUI;
using UnityEditor;
using NWH.VehiclePhysics2;
#endif


namespace SturdyMachine.Features
{
    [DisallowMultipleComponent]
    [Serializable]
    public abstract class FeatureModule : SturdyComponent 
    {
        public enum FeatureModuleCategory { Focus, Fight}

        public abstract FeatureModuleCategory GetFeatureModuleCategory();
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