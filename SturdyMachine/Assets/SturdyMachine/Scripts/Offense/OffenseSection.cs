using System;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
using NWH.NUI;
#endif

namespace SturdyMachine.Offense {

    /// <summary>
    /// Store all Offenses of a specific category
    /// </summary>
    [CreateAssetMenu(fileName = "NewOffenseCategory", menuName = "SturdyMachine/Offense/Category", order = 52)]
    public class OffenseCategory : ScriptableObject {

        #region Attribut

        /// <summary>
        /// (Optional) The direction of offense in this category"
        /// </summary>
        [SerializeField, Tooltip("(Optional) The direction of offense in this category")]
        OffenseDirection _offenseCategoryDirection;

        /// <summary>
        /// List of all Offenses in this category
        /// </summary>
        [SerializeField, Tooltip("List of all Offenses in this category")]
        Offense[] _offense;

        #endregion

        #region Get

        /// <summary>
        /// Return the current OffenseCategoryDirection of this category
        /// </summary>
        public OffenseDirection GetOffenseCategoryDirection => _offenseCategoryDirection;

        /// <summary>
        /// Returns all Offenses in this category
        /// </summary>
        public Offense[] GetOffense => _offense;

        #endregion
    }

#if UNITY_EDITOR

    [CustomEditor(typeof(OffenseCategory))]
    public class OffenseCategoryEditor : NUIEditor
    {
        public override bool OnInspectorNUI()
        {
            if (!base.OnInspectorNUI())
                return false;

            EditorGUI.BeginChangeCheck();

            drawer.Field("_offenseCategoryDirection", true, null, "Direction: ");

            drawer.ReorderableList("_offense");

            drawer.EndEditor(this);
            return true;
        }
    }

#endif
}