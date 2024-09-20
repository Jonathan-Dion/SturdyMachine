using System.IO;

using UnityEngine;

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

        [SerializeField]
        StateConfirmSettings.StateConfirmSettings _stateConfirmSettings;

        [SerializeField]
        HitConfirmSettings.HitConfirmSettings _hitConfirmSettings;

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

        public NADTimeSettings.NADTimeSettings GetNADTimeSettings {

            get {
            
                if (!_nadTimeSettings)
                    _nadTimeSettings = NADTimeSettings.NADTimeSettings.GetNADTimeSettings();

                return _nadTimeSettings;
            }
        }

        public StateConfirmSettings.StateConfirmSettings GetStateConfirmSettings
        {

            get
            {

                if (!_stateConfirmSettings)
                    _stateConfirmSettings = StateConfirmSettings.StateConfirmSettings.GetStateConfirmSettings();

                return _stateConfirmSettings;
            }
        }

        public HitConfirmSettings.HitConfirmSettings GetHitConfirmSettings
        {

            get
            {

                if (!_hitConfirmSettings)
                    _hitConfirmSettings = HitConfirmSettings.HitConfirmSettings.GetHitConfirmSettings();

                return _hitConfirmSettings;
            }
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
            _stateConfirmSettings = StateConfirmSettings.StateConfirmSettings.GetStateConfirmSettings();
            _hitConfirmSettings = HitConfirmSettings.HitConfirmSettings.GetHitConfirmSettings();
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

            drawer.Field("_nadTimeSettings", true, null, "NADTime: ");
            drawer.Field("_stateConfirmSettings", true, null, "StateConfirm: ");
            drawer.Field("_hitConfirmSettings", true, null, "HitConfirm: ");

            drawer.EndEditor(this);
            return true;
        }
    }

#endif
}