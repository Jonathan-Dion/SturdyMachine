using UnityEngine;
using System;

#if UNITY_EDITOR
using UnityEditor;
using NWH.NUI;
using NWH.VehiclePhysics2;
#endif

namespace SturdyMachine.Offense
{
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

        public bool GetIsCancelCurrentOffense(Offense pCurrentOffense, Offense pNextOffense) {

            if (pCurrentOffense.GetOffenseDirection == OffenseDirection.STANCE)
                return true;

            for (int i = 0; i < _offenseCancelDataGroup.Length; ++i) {
            
                if (pNextOffense.GetOffenseDirection != OffenseDirection.DEFAULT) {

                    if (pNextOffense.GetOffenseDirection != _offenseCancelDataGroup[i].offenseDirection)
                        continue;
                }

                if (pNextOffense.GetOffenseType != _offenseCancelDataGroup[i].offenseType)
                    continue;

                if (GetIsCancelWithOffenseCancelData(_offenseCancelDataGroup[i].standardOffenseCancel, pCurrentOffense))
                    return true;

                return GetIsCancelWithOffenseCancelData(_offenseCancelDataGroup[i].specialOffenseCancel, pCurrentOffense);
            }

            return false;
        }

        bool GetIsCancelWithOffenseCancelData(OffenseCancelData[] pOffenseCancelData, Offense pCurrentOffense) {

            for (int i = 0; i < pOffenseCancelData.Length; ++i) {

                if (pOffenseCancelData[i].offenseDirection != OffenseDirection.DEFAULT) {
                
                    if (pOffenseCancelData[i].offenseDirection != pCurrentOffense.GetOffenseDirection)
                        continue;
                }

                if (pOffenseCancelData[i].offenseType != pCurrentOffense.GetOffenseType)
                    continue;

                return true;
            }

            return false;
        }

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