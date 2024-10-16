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

        #region Attributes

        /// <summary>
        /// The type of Bot you want set up Offense combos
        /// </summary>
        [SerializeField, Tooltip("The type of Bot you want set up Offense combos")]
        BotType botType;

        /// <summary>
        /// All information regarding offense combo sequences for this bot
        /// </summary>
        [SerializeField, Tooltip("All information regarding offense combo sequences for this bot")]
        FightSequenceData[] fightSequenceData;

        #endregion

        #region Properties

        public BotType GetBotType => botType;

        public FightSequenceData[] GetFightSequenceDatas => fightSequenceData;

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

            drawer.Field("botType");

            drawer.ReorderableList("fightSequenceData");

            drawer.EndEditor(this);
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