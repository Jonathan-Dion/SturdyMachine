using System;
using System.IO;

using UnityEngine;

using SturdyMachine.Bot;

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

    [Serializable]
    public struct BlockingChanceData
    {
        public BotType botType;

        public float minBlockingChange;

        public float minSubtractiveBlockingChance, maxSubstractiveBlockingChance;

        public float additiveBlockingChance;
    }

    public class StateConfirmSettings : ScriptableObject {

        #region Attributes

        [SerializeField]
        StaggerStateData _staggerStateData;

        [SerializeField]
        BlockingChanceData[] _blockingChanceData;

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
            drawer.ReorderableList("_blockingChanceData");

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

    [CustomPropertyDrawer(typeof(BlockingChanceData))]
    public partial class BlockingChanceDataDrawer : ComponentNUIPropertyDrawer
    {
        public override bool OnNUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (!base.OnNUI(position, property, label))
                return false;

            if (drawer.Field("botType").enumValueIndex != 0)
            {
                //Min BlockingChance
                drawer.Label($"{drawer.FloatSlider("minBlockingChange", 0f, 1f, "0%", "100%", true).floatValue * 100}%", true);

                drawer.BeginSubsection("Substractive");

                drawer.Label($"{drawer.FloatSlider("minSubtractiveBlockingChance", 0f, 1f, "0%", "100%", true).floatValue * 100}%", true);
                drawer.Label($"{drawer.FloatSlider("maxSubstractiveBlockingChance", 0f, 1f, "0%", "100%", true).floatValue * 100}%", true);

                drawer.EndSubsection();

                drawer.Label($"{drawer.FloatSlider("additiveBlockingChance", 0f, 1f, "0%", "100%", true).floatValue * 100}%", true);
            }

            drawer.EndProperty();
            return true;
        }
    }

#endif

}