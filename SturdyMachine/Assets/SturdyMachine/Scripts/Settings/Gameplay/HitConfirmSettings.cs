using System;
using System.IO;

using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
using NWH.VehiclePhysics2;
using NWH.NUI;
#endif

namespace SturdyMachine.Settings.GameplaySettings.HitConfirmSettings
{

    public class HitConfirmSettings : ScriptableObject
    {

        #region Attributes

        /// <summary>
        /// Represents the time the hitConfirm should play
        /// </summary>
        [SerializeField, Tooltip("Represents the time the hitConfirm should play")]
        float _waitTimer;

        static string _hitConfirmFileName = "HitConfirmSettings";

        static HitConfirmSettings _hitConfirmSettings;

        #endregion

        #region Properties

        static string GetHitConfirmSettingsSettingAssetPath => $"{GameplaySettings.GetGameplaySettingFullPath}/{_hitConfirmFileName}.asset";

        public static HitConfirmSettings GetHitConfirmSettings()
        {
            //Directory
            if (!Directory.Exists($"{GameplaySettings.GetGameplaySettingFullPath}"))
            {
                Directory.CreateDirectory(GameplaySettings.GetGameplaySettingFullPath);

                AssetDatabase.Refresh();
            }

            //File
            if (!AssetDatabase.LoadAssetAtPath<HitConfirmSettings>(GetHitConfirmSettingsSettingAssetPath))
            {
                AssetDatabase.CreateAsset(CreateInstance<HitConfirmSettings>(), GetHitConfirmSettingsSettingAssetPath);

                AssetDatabase.SaveAssets();
            }

            return AssetDatabase.LoadAssetAtPath<HitConfirmSettings>(GetHitConfirmSettingsSettingAssetPath);

        }

        public float GetWaitTimer => _waitTimer;

        #endregion

        #region Methods

        [MenuItem("SturdyMachine/Settings/Game/Gameplay/HitConfirm/Edit")]
        public static void EditGameSettings()
        {
            _hitConfirmSettings = GetHitConfirmSettings();
            EditorUtility.FocusProjectWindow();

            Selection.activeObject = _hitConfirmSettings;
        }

        #endregion
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(HitConfirmSettings))]
    public class HitConfirmSettingsEditor : NUIEditor
    {
        public override bool OnInspectorNUI()
        {
            if (!base.OnInspectorNUI())
                return false;

            EditorGUI.BeginChangeCheck();

            drawer.Field("_waitTimer", true, "sec", "Freeze timer: ");

            drawer.EndEditor(this);
            return true;
        }
    }

#endif

}