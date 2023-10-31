
using UnityEngine;

using NWH.VehiclePhysics2;
using System;
using SturdyMachine.Offense;

#if UNITY_EDITOR
using UnityEditor;
using NWH.NUI;
#endif

namespace SturdyMachine.Features.HitConfirm 
{
    /// <summary>
    /// Store Sequence configuration based on OffenseType
    /// </summary>
    [Serializable]
    public struct HitConfirmOffenseData {

        /// <summary>
        /// The animationClip for the current Offense
        /// </summary>
        [Tooltip("The animationClip for the current Offense")]
        public OffenseType offenseType;

        /// <summary>
        /// Array grouping all the Sequences of an OffenseType
        /// </summary>
        [Tooltip("Array grouping all the Sequences of an OffenseType")]
        public HitConfirmOffense[] hitConfirmOffense;
    }

    /// <summary>
    /// Store all HitConfirmSubSequence for each Offense
    /// </summary>
    [CreateAssetMenu(fileName = "NewHitConfirmOffenseManager", menuName = "SturdyMachine/HitConfirm/Manager", order = 51)]
    public class HitConfirmOffenseManager : ScriptableObject {

        #region Attribut

        /// <summary>
        /// Array grouping all HitConfirmSequence of Deflection OffenseType
        /// </summary>
        [SerializeField, Tooltip("Array grouping all HitConfirmSequence of Deflection offense type")]
        HitConfirmOffenseData _hitConfirmOffenseDeflectionData;

        /// <summary>
        /// HitConfirmSequence of Evasion OffenseType
        /// </summary>
        [SerializeField, Tooltip("HitConfirmSequence of Evasion offense type")]
        HitConfirmOffenseData _hitConfirmOffenseEvasionData;

        /// <summary>
        /// HitConfirmSequence of Sweep OffenseType
        /// </summary>
        [SerializeField, Tooltip("HitConfirmSequence of Sweep OffenseType")]
        HitConfirmOffenseData _hitConfirmOffenseSweepData;

        /// <summary>
        /// Array grouping all HitConfirmSequence of Strike OffenseType
        /// </summary>
        [SerializeField, Tooltip("Array grouping all HitConfirmSequence of Strike OffenseType")]
        HitConfirmOffenseData _hitConfirmOffenseStrikeData;

        /// <summary>
        /// Array grouping all HitConfirmSequence of Heavy OffenseType
        /// </summary>
        [SerializeField, Tooltip("Array grouping all HitConfirmSequence of Heavy OffenseType")]
        HitConfirmOffenseData _hitConfirmOffenseHeavyData;

        /// <summary>
        /// Array grouping all HitConfirmSequence of Deathblow OffenseType
        /// </summary>
        [SerializeField, Tooltip("Array grouping all HitConfirmSequence of Deathblow OffenseType")]
        HitConfirmOffenseData _hitConfirmOffenseDeathblowData;

        #endregion

        #region Get

        /// <summary>
        /// Manages the sending of the correct SubSequence array depending on the OffenseType chosen as a parameter
        /// </summary>
        /// <param name="pOffenseType">The type of sequence array</param>
        /// <returns></returns>
        public HitConfirmOffense[] GetHitConfirmOffense(OffenseType pOffenseType) {

            //Deflection
            if (GetIfIsGoodHitConfirmOffense(_hitConfirmOffenseDeflectionData, pOffenseType))
                return _hitConfirmOffenseDeflectionData.hitConfirmOffense;

            //Evasion
            if (GetIfIsGoodHitConfirmOffense(_hitConfirmOffenseEvasionData, pOffenseType))
                return _hitConfirmOffenseEvasionData.hitConfirmOffense;

            //Sweep
            if (GetIfIsGoodHitConfirmOffense(_hitConfirmOffenseSweepData, pOffenseType))
                return _hitConfirmOffenseSweepData.hitConfirmOffense;

            //Strike
            if (GetIfIsGoodHitConfirmOffense(_hitConfirmOffenseStrikeData, pOffenseType))
                return _hitConfirmOffenseEvasionData.hitConfirmOffense;

            //Heavy
            if (GetIfIsGoodHitConfirmOffense(_hitConfirmOffenseHeavyData, pOffenseType))
                return _hitConfirmOffenseHeavyData.hitConfirmOffense;
                
            //Deathblow
            return _hitConfirmOffenseDeathblowData.hitConfirmOffense;
        }

        /// <summary>
        /// Checks if the OffenseType of HitConfirmOffenseData matches
        /// </summary>
        /// <param name="pHitConfirmOffenseData">The configuration of the sequence to check</param>
        /// <param name="pOffenseType">The OffenseType we want to check</param>
        /// <returns>Returns if the OffenseType chosen as a parameter matches the HitConfirmOffenseData</returns>
        bool GetIfIsGoodHitConfirmOffense(HitConfirmOffenseData pHitConfirmOffenseData, OffenseType pOffenseType) => pHitConfirmOffenseData.offenseType == pOffenseType;

        #endregion
    }

#if UNITY_EDITOR

    [CustomEditor(typeof(HitConfirmOffenseManager))]
    public class HitConfirmOffenseManagerEditor : NUIEditor
    {
        public override bool OnInspectorNUI()
        {
            if (!base.OnInspectorNUI())
                return false;

            EditorGUI.BeginChangeCheck();

            //Deflection
            DrawSpecificHitConfirmData("Deflection");

            //Evasion
            DrawSpecificHitConfirmData("Evasion");

            //Sweep
            DrawSpecificHitConfirmData("Sweep");

            //Strike
            DrawSpecificHitConfirmData("Strike");

            //Heavy
            DrawSpecificHitConfirmData("Heavy");

            //Deathblow
            DrawSpecificHitConfirmData("Deathblow");

            drawer.EndEditor(this);
            return true;
        }

        void DrawSpecificHitConfirmData(string pHitConfirmOffenseDataType) {

            drawer.BeginSubsection($"{pHitConfirmOffenseDataType}");

            drawer.Property($"_hitConfirmOffense{pHitConfirmOffenseDataType}Data");

            drawer.EndSubsection();
        }
    }

    [CustomPropertyDrawer(typeof(HitConfirmOffenseData))]
    public partial class HitConfirmOffenseDataDrawer : ComponentNUIPropertyDrawer
    {
        public override bool OnNUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (!base.OnNUI(position, property, label))
                return false;

            if (drawer.Field("offenseType").enumValueIndex != 0)
                drawer.ReorderableList("hitConfirmOffense");

            drawer.EndProperty();
            return true;
        }
    }

#endif
}