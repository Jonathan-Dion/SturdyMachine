using System;
using System.IO;

using UnityEngine;

using SturdyMachine.Settings.GameplaySettings;
using SturdyMachine.Settings.GameplaySettings.NADTimeSettings;
using SturdyMachine.Component;


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

        public float minBlockingChance;

        public float minDecreaseBlockingChance, maxDecreaseBlockingChance;

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

        public BlockingChanceData GetBlockingChanceData(BotType pCurrentBotType) 
        {
            for (byte i = 0; i < _blockingChanceData.Length; ++i) 
            {
                if (_blockingChanceData[i].botType != pCurrentBotType)
                    continue;

                return _blockingChanceData[i];
            }

            Debug.LogError($"The BlockingChanceData is not configured for {pCurrentBotType}");

            return new BlockingChanceData();
        }

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
        float currentDecreaseBlockingChance;

        public override bool OnNUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (!base.OnNUI(position, property, label))
                return false;

            if (drawer.Field("botType").enumValueIndex != 0) 
            {
                float blockingChanceValue = drawer.FloatSlider("minBlockingChance", 0, 1, "0%", "100%").floatValue;

                drawer.Label($"{blockingChanceValue * 100f}%", true);

                drawer.BeginSubsection("Decrease BlockingChance");

                blockingChanceValue = drawer.FloatSlider("minDecreaseBlockingChance", 0, 1, "0%", "100%").floatValue;

                drawer.Label($"Min: {blockingChanceValue * 100f}%", true);

                blockingChanceValue = drawer.FloatSlider("maxDecreaseBlockingChance", blockingChanceValue, 1, "0%", "100%").floatValue;

                drawer.Label($"Max: {blockingChanceValue * 100f}%", true);

                drawer.EndSubsection();

                blockingChanceValue = drawer.FloatSlider("additiveBlockingChance", 0, 1, "0%", "100%").floatValue;

                drawer.Label($"{blockingChanceValue * 100f}%", true);
            }

            drawer.EndProperty();
            return true;
        }
    }

#endif

}