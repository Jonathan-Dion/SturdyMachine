using System;

using UnityEngine;

#if UNITY_EDITOR
using NWH.NUI;
using NWH.VehiclePhysics2;
using UnityEditor;
#endif

namespace SturdyMachine.Offense.Blocking
{
    /// <summary>
    /// Records information regarding all OffenseBlocking based on an Offense
    /// </summary>
    [Serializable, Tooltip("Records information regarding all OffenseBlocking based on an Offense")]
    public struct OffenseBlockingConfigData {

        /// <summary>
        /// Type of Offense
        /// </summary>
        [Tooltip("Type of Offense")]
        public OffenseType offenseType;

        /// <summary>
        /// Direction of Offense
        /// </summary>
        [Tooltip("Direction of Offense")]
        public OffenseDirection offenseDirection;

        /// <summary>
        /// Array of OffenseBlocking
        /// </summary>
        [Tooltip("Array of OffenseBlocking")]
        public OffenseBlocking[] offenseBlocking;
    }
    
    /// <summary>
    /// Store all OffenseBlocking
    /// </summary>
    [CreateAssetMenu(fileName = "NewOffenseBlockingConfig", menuName = "SturdyMachine/Offense/Configuration/Blocking", order = 1)]
    public class OffenseBlockingConfig : ScriptableObject {

        #region Attribut

        /// <summary>
        /// Array to record information for all OffenseBlockings
        /// </summary>
        [SerializeField, Tooltip("Array to record information for all OffenseBlockings")]
        OffenseBlockingConfigData[] _offenseBlockingConfigData;

        #endregion

        #region Get

        /// <summary>
        /// Returns the list of all OffenseBlockings
        /// </summary>
        public OffenseBlockingConfigData[] GetOffenseBlockingConfigData => _offenseBlockingConfigData;

        #endregion


#if UNITY_EDITOR

        [CustomEditor(typeof(OffenseBlockingConfig))]
        [CanEditMultipleObjects]
        public class OffenseBlockingConfigEditor : NUIEditor
        {
            public override bool OnInspectorNUI()
            {
                if (!base.OnInspectorNUI())
                    return false;

                EditorGUI.BeginChangeCheck();

                drawer.ReorderableList("_offenseBlockingConfigData");

                drawer.EndEditor(this);
                return true;
            }
        }

        [CustomPropertyDrawer(typeof(OffenseBlockingConfigData))]
        public partial class OffenseBlockingConfigDataDrawer : ComponentNUIPropertyDrawer
        {
            public override bool OnNUI(Rect position, SerializedProperty property, GUIContent label)
            {
                if (!base.OnNUI(position, property, label))
                    return false;

                if (drawer.Field("offenseDirection", true, null, "Direction: ").enumValueIndex != 0) {

                    if (drawer.Field("offenseType", true, null, "Type: ").enumValueIndex != 0)
                        drawer.ReorderableList("offenseBlocking");
                }

                drawer.EndProperty();
                return true;
            }
        }

#endif
    }
}