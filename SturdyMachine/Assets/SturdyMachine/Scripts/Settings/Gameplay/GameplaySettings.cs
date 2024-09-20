using System.IO;

using UnityEngine;

using SturdyMachine.Settings.NADTimeSettings;

#if UNITY_EDITOR
using UnityEditor;
using NWH.VehiclePhysics2;
using NWH.NUI;
#endif

namespace SturdyMachine.Settings.GameplaySettings {

    public class GameplaySettings : ScriptableObject
    {
        #region Attributes

        static string _gameSettingsFileName = "GameplaySettings";

        static GameplaySettings _gameplaySettings;

        [SerializeField]
        NADTimeSettings.NADTimeSettings _nadTimeSettings;

        #endregion

        #region Properties

        public static string GetGameplaySettingFullPath => $"{GameSettings.GetGameSettingsFolderPath}/{_gameSettingsFileName}";

        static string GetGameplaySettingAssetPath => $"{GetGameplaySettingFullPath}/{_gameSettingsFileName}.asset";

        public static GameplaySettings GetGameplaySettings()
        {

            //Directory
            if (!Directory.Exists($"{GetGameplaySettingFullPath}"))
            {
                Directory.CreateDirectory(GetGameplaySettingFullPath);

                AssetDatabase.Refresh();
            }

            //File
            if (!AssetDatabase.LoadAssetAtPath<GameplaySettings>(GetGameplaySettingAssetPath))
            {
                AssetDatabase.CreateAsset(CreateInstance<GameplaySettings>(), GetGameplaySettingAssetPath);

                AssetDatabase.SaveAssets();
            }

            return AssetDatabase.LoadAssetAtPath<GameplaySettings>(GetGameplaySettingAssetPath);

        }

        #endregion

        #region Methods

        [MenuItem("SturdyMachine/Settings/Game/Gameplay/Edit")]
        public static void EditGameSettings()
        {
            _gameplaySettings = GetGameplaySettings();

            EditorUtility.FocusProjectWindow();

            Selection.activeObject = _gameplaySettings;
        }

        void OnEnable()
        {
            _nadTimeSettings = NADTimeSettings.NADTimeSettings.GetNADTimeSettings();
        }

        #endregion
    }

#if UNITY_EDITOR

    [CustomEditor(typeof(GameplaySettings))]
    public class GameplaySettingsEditor : NUIEditor
    {
        public override bool OnInspectorNUI()
        {
            if (!base.OnInspectorNUI())
                return false;

            EditorGUI.BeginChangeCheck();

            drawer.Field("_nadTimeSettings");

            drawer.EndEditor(this);
            return true;
        }
    }

#endif
}