using System.IO;

using UnityEngine;

using SturdyMachine.Settings.GameplaySettings;


#if UNITY_EDITOR
using UnityEditor;
using NWH.VehiclePhysics2;
using NWH.NUI;
#endif

namespace SturdyMachine.Settings
{

    public class GameSettings : ScriptableObject
    {
        #region Attributes

        static string _gameSettingsPath = "Assets/SturdyMachine";
        static string _gameSettingsFolderName = "Settings";
        static string _gameSettingsFileName = "GameSettings";

        static GameSettings _gameSettings;

        [SerializeField]
        GameplaySettings.GameplaySettings _gameplaySettings;

        #endregion

        #region Properties

        public static string GetGameSettingsFolderPath => $"{_gameSettingsPath}/{_gameSettingsFolderName}";
        public static string GetGameSettingsAssetPath => $"{GetGameSettingsFolderPath}/{_gameSettingsFileName}.asset";

        public GameplaySettings.GameplaySettings GetGameplaySettings {

            get {
            
                if (!_gameplaySettings)
                    _gameplaySettings = GameplaySettings.GameplaySettings.GetGameplaySettings();

                return _gameplaySettings;
            }
        }

        public static GameSettings GetGameSettings() {

            //Directory
            if (!Directory.Exists(GetGameSettingsFolderPath)) {
            
                Directory.CreateDirectory(GetGameSettingsFolderPath);

                AssetDatabase.Refresh();
            }

            //File
            if (!AssetDatabase.LoadAssetAtPath<GameSettings>(GetGameSettingsAssetPath)) {

                AssetDatabase.CreateAsset(CreateInstance<GameSettings>(), GetGameSettingsAssetPath);


                AssetDatabase.SaveAssets();
            }

            return AssetDatabase.LoadAssetAtPath<GameSettings>(GetGameSettingsAssetPath);

        }

        #endregion

        #region Methods

        [MenuItem("SturdyMachine/Settings/Game/Edit")]
        public static void EditGameSettings() {
        
            _gameSettings = GetGameSettings();

            EditorUtility.FocusProjectWindow();

            Selection.activeObject = _gameSettings;
        }

        void OnEnable()
        {
            _gameplaySettings = GetGameplaySettings;
        }

        #endregion
    }

#if UNITY_EDITOR

    [CustomEditor(typeof(GameSettings))]
    public class GameSettingsEditor : NUIEditor
    {
        public override bool OnInspectorNUI()
        {
            if (!base.OnInspectorNUI())
                return false;

            EditorGUI.BeginChangeCheck();

            drawer.Field("_gameplaySettings");

            drawer.EndEditor(this);
            return true;
        }
    }

#endif
}