using System;

using UnityEngine;

using NWH.NUI;
using NWH.VehiclePhysics2;
using SturdyMachine.Component;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace SturdyMachine.Offense.Blocking
{
    /// <summary>
    /// Records information regarding blocking information based on offense
    /// </summary>
    [Serializable, Tooltip("Records information regarding blocking information based on offense")]
    public struct OffenseBlockingData {

        /// <summary>
        /// The Attack Offense
        /// </summary>
        [Tooltip("The Attack Offense")]
        public Offense offense;

        /// <summary>
        /// The starting BlockingRangeData for the blocking section
        /// </summary>
        [Tooltip("The starting BlockingRangeData for the blocking section")]
        public BlockingRangeData minBlockingRangeData;

        /// <summary>
        /// The end BlockingRangeData for the blocking section
        /// </summary>
        [Tooltip("The end BlockingRangeData for the blocking section")]
        public BlockingRangeData maxBlockingRangeData;
    }

    /// <summary>
    /// Represents information about the blocking section values
    /// </summary>
    [Serializable, Tooltip("Represents information about the blocking section values")]
    public struct BlockingRangeData 
    {
        /// <summary>
        /// The blocking value in frame
        /// </summary>
        [Tooltip("The blocking value in frame")]
        public float rangeFrame;

        /// <summary>
        /// The blocking value in percentage
        /// </summary>
        [Tooltip("The blocking value in percentage")]
        public float rangeTime;

        /// <summary>
        /// Indicates the number of frames the Offense has
        /// </summary>
        [Tooltip("Indicates the number of frames the Offense has")]
        public float offenseFrameCount;
    }

    /// <summary>
    /// Saves blocking sections of all offenses based on Bot type
    /// </summary>
    [Serializable, Tooltip("Saves blocking sections of all offenses based on Bot type")]
    public struct BlockingData {

        /// <summary>
        /// Bot type
        /// </summary>
        [Tooltip("Bot type")]
        public BotType botType;

        /// <summary>
        /// The list of information regarding the sections of all Offenses of this bot
        /// </summary>
        [Tooltip("The list of information regarding the sections of all Offenses of this bot")]
        public OffenseBlockingData[] offenseBlockingData;
    }

    /// <summary>
    /// Store all elements of an offense blocking
    /// </summary>
    [CreateAssetMenu(fileName = "NewOffenseBlockingConfig", menuName = "SturdyMachine/Offense/Blocking/BlockingOffense", order = 51)]
    public class OffenseBlocking : Offense {

        #region Attribut

        /// <summary>
        /// List containing all information regarding all blocking sections of all Offenses depending on Bot type
        /// </summary>
        [SerializeField, Tooltip("List containing all information regarding all blocking sections of all Offenses depending on Bot type")]
        BlockingData[] _blockingData;

        #endregion

        #region Get

        /// <summary>
        /// Returns the list of information regarding all sections of all Offenses based on Bot type
        /// </summary>
        public BlockingData[] GetBlockingData => _blockingData;

        #endregion
    }

    [CustomEditor(typeof(OffenseBlocking))]
    public class OffenseBlockingEditor : NUIEditor
    {
        public override bool OnInspectorNUI()
        {
            if (!base.OnInspectorNUI())
                return false;

            EditorGUI.BeginChangeCheck();

            drawer.ReorderableList("_blockingData");

            drawer.EndEditor(this);
            return true;
        }
    }

    [CustomPropertyDrawer(typeof(OffenseBlockingData))]
    public partial class OffenseBlockingDataDrawer : ComponentNUIPropertyDrawer
    {
        public override bool OnNUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (!base.OnNUI(position, property, label))
                return false;

            DrawOffenseInformation();

            drawer.EndProperty();
            return true;
        }

        void DrawOffenseInformation()
        {
            drawer.BeginSubsection("Data");

            Offense offense = drawer.Field("offense").objectReferenceValue as Offense;

            if (offense)
            {
                //MinBlockingRange
                DrawBlockingRangeDataInformation("minBlockingRangeData", offense);

                //MaxBlockingRange
                DrawBlockingRangeDataInformation("maxBlockingRangeData", offense);
            }

            drawer.EndSubsection();
        }

        void DrawBlockingRangeDataInformation(string pPropertyName, Offense pOffense) {

            drawer.Property(pPropertyName);

            drawer.FindProperty(pPropertyName).FindPropertyRelative("offenseFrameCount").floatValue = pOffense.GetLengthClip(false) * pOffense.GetAnimationClip().frameRate;
        }
    }

    [CustomPropertyDrawer(typeof(BlockingData))]
    public partial class BlockingDataDrawer : ComponentNUIPropertyDrawer
    {
        public override bool OnNUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (!base.OnNUI(position, property, label))
                return false;

            if (drawer.Field("botType").enumValueIndex != 0)
                drawer.ReorderableList("offenseBlockingData");

            drawer.EndProperty();
            return true;
        }
    }

    [CustomPropertyDrawer(typeof(BlockingRangeData))]
    public partial class BlockingRangeDataDrawer : ComponentNUIPropertyDrawer
    {
        public override bool OnNUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (!base.OnNUI(position, property, label))
                return false;

            float offenseFrameCount = drawer.Field("offenseFrameCount", false, "Frames").floatValue;
            float rangeFrameValue = drawer.FloatSlider("rangeFrame", 0, offenseFrameCount, $"0 frames", $"{offenseFrameCount} frames").floatValue;

            if (rangeFrameValue == 0) {

                drawer.Info("You must enter a value so that HitConfirm management works correctly", MessageType.Warning);

                drawer.EndProperty();
                return true;
            }


            if (offenseFrameCount != 0) {
            
                float rangeTimeValue = drawer.Field("rangeTime", false, "%").floatValue = GetFramePourcentage(rangeFrameValue, offenseFrameCount);

                drawer.Label($"{rangeTimeValue * 100} %");
            }

            drawer.EndProperty();
            return true;
        }

        float GetFramePourcentage(float pCurrentRangeFrameValue, float pFrameCounter) => pCurrentRangeFrameValue / pFrameCounter;
    }
}