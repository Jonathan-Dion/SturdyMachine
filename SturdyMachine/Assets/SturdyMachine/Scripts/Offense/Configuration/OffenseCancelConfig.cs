using UnityEngine;
using System;
using NWH.VehiclePhysics2;

#if UNITY_EDITOR
using UnityEditor;
using NWH.NUI;
#endif

namespace SturdyMachine.Offense
{
    public enum OffenseDirection { DEFAULT, NEUTRAL, RIGHT, LEFT, STANCE }
    public enum OffenseType { DEFAULT, DEFLECTION, EVASION, SWEEP, STRIKE, HEAVY, DEATHBLOW, DAMAGEHIT, REPEL };
    public enum OffensePaternType { DEFAULT, PASSIVE, AGGRESSIVE };

    /// <summary>
    /// Base struct for OffenseData
    /// </summary>
    [Serializable]
    public struct OffenseCancelData {

        public OffenseDirection offenseDirection;
        public OffenseType offenseType;
    }

    /// <summary>
    /// Stock all attack offenses that can be blocked by it
    /// </summary>
    [Serializable]
    public struct OffenseCancelDataGroup {

        public OffenseDirection offenseDirection;
        public OffenseType offenseType;

        public OffenseCancelData[] standardOffenseCancel;
        public OffenseCancelData[] specialOffenseCancel;
    }

    /// <summary>
    /// Configuration file that indicates all offenses that can be blocked
    /// </summary>
    [CreateAssetMenu(fileName = "OffenceCancelConfig", menuName = "SturdyMachine/Offence/Configuration/OffenceCancelData", order = 51)]
    public class OffenseCancelConfig : ScriptableObject
    {
        [SerializeField]
        OffenseCancelDataGroup[] _offenseCancelDataGroup;

        #region Get

        public OffenseCancelDataGroup[] GetOffenseCancelDataGroup => _offenseCancelDataGroup;

        #endregion
    }

#if UNITY_EDITOR

    [CustomEditor(typeof(OffenseCancelConfig))]
    [CanEditMultipleObjects]
    public class OffenseCancelConfigEditor : NUIEditor
    {
        public override bool OnInspectorNUI()
        {
            if (!base.OnInspectorNUI())
                return false;

            EditorGUI.BeginChangeCheck();

            #region Offense configuration

            drawer.BeginSubsection("Offense Configuration");

            drawer.ReorderableList("_offenseCancelDataGroup");

            drawer.EndSubsection();

            #endregion

            drawer.EndEditor(this);
            return true;
        }
    }

    [CustomPropertyDrawer(typeof(OffenseCancelData))]
    public partial class OffenseCancelDataDrawer : ComponentNUIPropertyDrawer
    {
        public override bool OnNUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (!base.OnNUI(position, property, label))
                return false;

            drawer.Field("offenseDirection");
            drawer.Field("offenseType");

            drawer.EndProperty();
            return true;
        }
    }

    [CustomPropertyDrawer(typeof(OffenseCancelDataGroup))]
    public partial class OffenseCancelDataGroupDrawer : ComponentNUIPropertyDrawer
    {
        public override bool OnNUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (!base.OnNUI(position, property, label))
                return false;

            drawer.Field("offenseDirection");
            
            //OffenseType
            if (drawer.Field("offenseType").enumValueIndex != 0)
            {

                #region Standard

                drawer.BeginSubsection("Standard");

                drawer.ReorderableList("standardOffenseCancel");

                drawer.EndSubsection();

                #endregion

                #region Special

                drawer.BeginSubsection("Special");

                drawer.ReorderableList("specialOffenseCancel");

                drawer.EndSubsection();

                #endregion
            }

            drawer.EndProperty();
            return true;
        }
    }

#endif

}