

using UnityEngine;

using NWH.VehiclePhysics2;
using SturdyMachine.Offense;

#if UNITY_EDITOR
using UnityEditor;
using NWH.NUI;
#endif

namespace SturdyMachine.Features.HitConfirm {

    /// <summary>
    /// Store HitConfirmSubSequence information for each Offense
    /// </summary>
    [CreateAssetMenu(fileName = "NewHitConfirmOffense", menuName = "SturdyMachine/HitConfirm/OffenseData", order = 51)]
    public class HitConfirmOffense : ScriptableObject {

        #region Attribut

        

        #endregion

        #region Get

        

        #endregion

#if UNITY_EDITOR

        [CustomEditor(typeof(HitConfirmOffense))]
        public class HitConfirmOffenseEditor : NUIEditor
        {
            public override bool OnInspectorNUI()
            {
                if (!base.OnInspectorNUI())
                    return false;

                EditorGUI.BeginChangeCheck();

                if (drawer.Field("_offenseDirection").enumValueIndex != 0)
                {
                    drawer.Field("_offenseType");

                    drawer.Property("_hitConfirmData");
                }

                drawer.EndEditor(this);
                return true;
            }
        }

#endif
    }
}