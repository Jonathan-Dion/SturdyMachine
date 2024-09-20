using System;
using System.IO;

using UnityEngine;

using SturdyMachine.Settings.GameplaySettings;
using SturdyMachine.Settings.GameplaySettings.NADTimeSettings;

#if UNITY_EDITOR
using UnityEditor;
using NWH.VehiclePhysics2;
using NWH.NUI;
#endif

namespace SturdyMachine.Settings.GameplaySettings.StateConfirmSettings {

    [Serializable]
    public struct StaggerStateData
    {

        public AnimationClip stunAnimationClip;
        public AnimationClip recoveryStunAnimationClip;

        public float maxStunTimer;
    }

    public class StateConfirmSettings : ScriptableObject {

        #region Attributes

        [SerializeField]
        StaggerStateData _staggerStateData;

        static string _stateConfirmFileName = "StateConfirmSettings";

        static StateConfirmSettings _stateConfirmSettings;

        #endregion

        #region Properties

        static string GetstateConfirmSettingAssetPath => $"{GameplaySettings.GetGameplaySettingFullPath}/{_stateConfirmFileName}.asset";

        public static StateConfirmSettings GetStateConfirmSettings()
        {
            //Directory
            if (!Directory.Exists($"{GameplaySettings.GetGameplaySettingFullPath}"))
            {
                Directory.CreateDirectory(GameplaySettings.GetGameplaySettingFullPath);

                AssetDatabase.Refresh();
            }

            //File
            if (!AssetDatabase.LoadAssetAtPath<StateConfirmSettings>(GetstateConfirmSettingAssetPath))
            {
                AssetDatabase.CreateAsset(CreateInstance<StateConfirmSettings>(), GetstateConfirmSettingAssetPath);

                AssetDatabase.SaveAssets();
            }

            return AssetDatabase.LoadAssetAtPath<StateConfirmSettings>(GetstateConfirmSettingAssetPath);

        }

        public StaggerStateData GetStaggerStateData => _staggerStateData;

        #endregion

        #region Methods

        [MenuItem("SturdyMachine/Settings/Game/Gameplay/StateConfirm/Edit")]
        public static void EditGameSettings()
        {
            _stateConfirmSettings = GetStateConfirmSettings();
            EditorUtility.FocusProjectWindow();

            Selection.activeObject = _stateConfirmSettings;
        }

        #endregion
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(StateConfirmSettings))]
    public class StateConfirmSettingsEditor : NUIEditor
    {
        public override bool OnInspectorNUI()
        {
            if (!base.OnInspectorNUI())
                return false;

            EditorGUI.BeginChangeCheck();

            drawer.Field("_staggerStateData");

            drawer.EndEditor(this);
            return true;
        }
    }

    [CustomPropertyDrawer(typeof(StaggerStateData))]
    public partial class StaggerStateDataDrawer : ComponentNUIPropertyDrawer
    {
        public override bool OnNUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (!base.OnNUI(position, property, label))
                return false;

            drawer.Field("stunAnimationClip", true, null, "Stun: ");
            drawer.Field("recoveryStunAnimationClip", true, null, "Recovery: ");
            drawer.Field("maxStunTimer", true, "sec", "Timer: ");

            drawer.EndProperty();
            return true;
        }
    }

#endif

}