using System;
using System.IO;

using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
using NWH.NUI;
using NWH.VehiclePhysics2;
#endif

namespace SturdyMachine.Settings.GameplaySettings.NADTimeSettings {

    public enum NADTimeType { None, Neutral, Advantage, Disadvantage }

    [Serializable]
    public struct NADTimeData {

        public NADTimeType nadTimeType;

        public float multiplier;

        [ColorUsage(true, true)]
        public Color nadTimeColor;
    }

    public class NADTimeSettings : ScriptableObject {

        #region Attributes

        static string _nadTimeFileName = "NADTimeSettings";

        static NADTimeSettings _nadTimeSettings;

        [SerializeField]
        NADTimeData[] _nadTimeData;

        #endregion

        #region Properties

        static string GetNADTimeSettingAssetPath => $"{GameplaySettings.GetGameplaySettingFullPath}/{_nadTimeFileName}.asset";

        public static NADTimeSettings GetNADTimeSettings()
        {
            //Directory
            if (!Directory.Exists($"{GameplaySettings.GetGameplaySettingFullPath}"))
            {
                Directory.CreateDirectory(GameplaySettings.GetGameplaySettingFullPath);

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

        public float GetCurrentNADTimeMultiplicator(NADTimeType pCurrentNADTimeType) {

            if (_nadTimeData.Length == 0)
                return 1f;

            for (byte i = 0; i < _nadTimeData.Length; ++i) {

                if (_nadTimeData[i].nadTimeType != pCurrentNADTimeType)
                    continue;

                return _nadTimeData[i].multiplier;
            }

            return 1f;
        }

        public Color GetCurrentNADTimeMeshColor(NADTimeType pCurrentNADTimeType) {

            if (_nadTimeData.Length == 0)
                return Color.black;

            for (byte i = 0; i < _nadTimeData.Length; ++i)
            {

                if (_nadTimeData[i].nadTimeType != pCurrentNADTimeType)
                    continue;

                return _nadTimeData[i].nadTimeColor;
            }

            return Color.black;
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
        NADTimeType _nadTimeType;

        public override bool OnNUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (!base.OnNUI(position, property, label))
                return false;

            _nadTimeType = (NADTimeType)drawer.Field("nadTimeType", true).enumValueIndex;

            if (_nadTimeType != NADTimeType.None) {
                
                float currentNADTimeMultiplicator = drawer.FloatSlider("multiplier", 0f, 1f, "0%", "100%", true).floatValue;

                if (_nadTimeType == NADTimeType.Disadvantage) {
                    
                    if (currentNADTimeMultiplicator < 1)
                        drawer.FindProperty("multiplier").floatValue += 1f;
                }

                drawer.Field("nadTimeColor", true, null, "NADTime MeshColor: ");
            }

            drawer.EndProperty();
            return true;
        }
    }

#endif
}