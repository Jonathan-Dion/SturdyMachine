using System;

using UnityEngine;
using SturdyMachine.Component;

#if UNITY_EDITOR
using UnityEditor;
using NWH.VehiclePhysics2;
using NWH.NUI;
#endif

namespace SturdyMachine.Features.Fight.Sequence {

    /// <summary>
    /// Information about all combos for a Bot type
    /// </summary>
    [Serializable, Tooltip("Information about all combos for a Bot type")]
    public struct FightOffenseSequenceData {

        /// <summary>
        /// The type of Bot you want set up Offense combos
        /// </summary>
        [Tooltip("The type of Bot you want set up Offense combos")]
        public BotType botType;

        /// <summary>
        /// All information regarding offense combo sequences for this bot
        /// </summary>
        [Tooltip("All information regarding offense combo sequences for this bot")]
        public FightSequenceData[] fightSequenceData;
    }

    /// <summary>
    /// Information regarding combo sequences depending on the mode of a combat mode
    /// </summary>
    [Serializable, Tooltip("Information regarding combo sequences depending on the mode of a combat mode")]
    public struct FightSequenceData {

        /// <summary>
        /// Designates the combat mode that this combo
        /// </summary>
        [Tooltip("Designates the combat mode that this combo")]
        public FightingModeType fightingModeType;

        /// <summary>
        /// Table concerning all Offense combos
        /// </summary>
        [Tooltip("Table concerning all Offense combos")]
        public FightComboSequenceData[] fightComboSequenceData;
    }

    [CreateAssetMenu(fileName = "NewFightOffenseSequence", menuName = "SturdyMachine/Features/Sequences/Offense", order = 1)]
    public class FightOffenseSequence : ScriptableObject {

        #region Attribut

        /// <summary>
        /// Information regarding all Offense combos
        /// </summary>
        [SerializeField, Tooltip("Information regarding all Offense combos")]
        FightOffenseSequenceData _fightOffenseSequenceData;

        #endregion

        #region Get

        /// <summary>
        /// Returns Offense combos
        /// </summary>
        public FightOffenseSequenceData GetFightOffenseSequenceData => _fightOffenseSequenceData;

        #endregion

        #region Method

        #endregion
    }


#if UNITY_EDITOR

    [CustomEditor(typeof(FightOffenseSequence))]
    public class FightOffenseSequenceEditor : NUIEditor
    {
        public override bool OnInspectorNUI()
        {
            if (!base.OnInspectorNUI())
                return false;

            EditorGUI.BeginChangeCheck();

            drawer.Property("_fightOffenseSequenceData");

            drawer.EndEditor(this);
            return true;
        }
    }

    [CustomPropertyDrawer(typeof(FightOffenseSequenceData))]
    public partial class FightOffenseSequenceDataDrawer : ComponentNUIPropertyDrawer
    {
        public override bool OnNUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (!base.OnNUI(position, property, label))
                return false;

            if (drawer.Field("botType").enumValueIndex != 0)
                drawer.ReorderableList("fightSequenceData");
                
            drawer.EndProperty();
            return true;
        }
    }

    [CustomPropertyDrawer(typeof(FightSequenceData))]
    public partial class FightSequenceDataDrawer : ComponentNUIPropertyDrawer
    {
        public override bool OnNUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (!base.OnNUI(position, property, label))
                return false;

            drawer.Field("fightingModeType");

            drawer.ReorderableList("fightComboSequenceData");

            drawer.EndProperty();
            return true;
        }
    }

#endif
}