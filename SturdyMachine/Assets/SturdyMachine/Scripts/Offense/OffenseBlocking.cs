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
    [Serializable, Tooltip("")]
    public struct OffenseBlockingData {

        public Offense offense;

        public float minRange;

        public float maxRange;
    }

    [Serializable, Tooltip("")]
    public struct BlockingData {

        public BotType botType;

        public OffenseBlockingData[] offenseBlockingData;
    }

    /// <summary>
    /// Store all elements of an offense blocking
    /// </summary>
    [CreateAssetMenu(fileName = "NewOffenseBlockingConfig", menuName = "SturdyMachine/Offense/Blocking/BlockingOffense", order = 51)]
    public class OffenseBlocking : Offense {

        #region Attribut

        [SerializeField]
        BlockingData[] _blockingData;

        #endregion

        #region Get

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

                drawer.Label($"{offense.GetLengthClip(false) * offense.GetAnimationClip().frameRate} frames");

                float minValue = drawer.FloatSlider("minRange", 0, offense.GetLengthClip(false) * offense.GetAnimationClip().frameRate).floatValue;
                drawer.FloatSlider("maxRange", minValue, offense.GetLengthClip(false) * offense.GetAnimationClip().frameRate);
            }

            drawer.EndSubsection();
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
}