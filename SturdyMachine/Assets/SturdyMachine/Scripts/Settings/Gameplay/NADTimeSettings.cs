using System;
using System.IO;

using UnityEngine;

using SturdyMachine.Settings.GameplaySettings;

#if UNITY_EDITOR
using UnityEditor;
using NWH.NUI;
using NWH.VehiclePhysics2;
#endif

namespace SturdyMachine.Settings.NADTimeSettings {

    public enum NADTimeType { None, Neutral, Advantage, Disadvantage }

    [Serializable]
    public struct NADTimeData {

        public NADTimeType nadTimeType;

        public float multiplier;
    }

    public class NADTimeSettings : ScriptableObject {

        #region Attributes

        static string _nadTimeFileName = "NADTimeSettings";

        static NADTimeSettings _nadTimeSettings;

        [SerializeField]
        NADTimeData[] _nadTimeData;

        #endregion

        #region Properties

        static string GetNADTimeSettingAssetPath => $"{GameplaySettings.GameplaySettings.GetGameplaySettingFullPath}/{_nadTimeFileName}.asset";

        public static NADTimeSettings GetNADTimeSettings()
        {
            //Directory
            if (!Directory.Exists($"{GameplaySettings.GameplaySettings.GetGameplaySettingFullPath}"))
            {
                Directory.CreateDirectory(GameplaySettings.GameplaySettings.GetGameplaySettingFullPath);

                AssetDatabase.Refresh();
            }

            //File
            if (!AssetDatabase.LoadAssetAtPath<NADTimeSettings>(GetNADTimeSettingAssetPath))
            {
                AssetDatabase.CreateAsset(CreateInstance<NADTimeSettings>(), GetNADTimeSettingAssetPath);

                AssetDatabase.SaveAssets();
            }

            return AssetDatabase.LoadAssetAtPath<NADTimeSettings>(GetNADTimeSettingAssetPath);

        }

        #endregion

        #region Methods

        [MenuItem("SturdyMachine/Settings/Game/Gameplay/NADTime/Edit")]
        public static void EditGameSettings()
        {
            _nadTimeSettings = GetNADTimeSettings();

            EditorUtility.FocusProjectWindow();

            Selection.activeObject = _nadTimeSettings;
        }

        #endregion
    }

#if UNITY_EDITOR

    [CustomEditor(typeof(NADTimeSettings))]
    public class NADTimeSettingsEditor : NUIEditor
    {
        public override bool OnInspectorNUI()
        {
            if (!base.OnInspectorNUI())
                return false;

            EditorGUI.BeginChangeCheck();

            drawer.ReorderableList("_nadTimeData");

            drawer.EndEditor(this);
            return true;
        }
    }

    [CustomPropertyDrawer(typeof(NADTimeData))]
    public partial class NADTimeDataDrawer : ComponentNUIPropertyDrawer
    {
        public override bool OnNUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (!base.OnNUI(position, property, label))
                return false;

            if (drawer.Field("nadTimeType", true).enumValueIndex != 0)
                drawer.FloatSlider("multiplier", -1f, 1f, "0%", "100%", true);

            drawer.EndProperty();
            return true;
        }
    }

#endif
}