using System;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
using NWH.NUI;
#endif

namespace SturdyMachine.Features 
{
    [Serializable]
    public abstract class FeatureModuleWrapper : MonoBehaviour 
    {
        public virtual FeatureModule.FeatureModuleCategory GetFeatureModuleCategory => GetFeatureModule().GetFeatureModuleCategory();

        public abstract FeatureModule GetFeatureModule();

        public abstract void SetFeatureModule(FeatureModule pFeatureModule);
    }

#if UNITY_EDITOR

    [CustomEditor(typeof(FeatureModuleWrapper), true)]
    [CanEditMultipleObjects]
    public partial class FeatureModuleWrapperEditor : NVP_NUIEditor
    {
        public override void OnInspectorGUI()
        {
            //OnInspectorNUI();
            EditorGUILayout.HelpBox(
                new GUIContent("To change module settings go to 'Modules' tab of FeatureManager."));
        }


        public override bool OnInspectorNUI()
        {
            drawer.BeginEditor(serializedObject);
            drawer.Property(drawer.serializedObject.FindProperty("_module"));
            drawer.EndEditor(this);
            return true;
        }


        public override bool UseDefaultMargins()
        {
            return false;
        }
    }

#endif
}
