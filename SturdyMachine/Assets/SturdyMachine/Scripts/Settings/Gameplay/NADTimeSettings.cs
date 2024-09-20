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
                drawer.FloatSlider("multiplier", 0f, 1f, "0%", "100%", true);

                if (_nadTimeType == NADTimeType.Disadvantage)
                    drawer.FindProperty("multiplier").floatValue += 1f;
            }

            drawer.EndProperty();
            return true;
        }
    }

#endif
}