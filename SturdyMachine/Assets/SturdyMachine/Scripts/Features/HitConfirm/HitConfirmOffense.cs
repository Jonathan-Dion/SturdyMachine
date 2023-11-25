

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

        /// <summary>
        /// The current offense type of this HitConfirmSubSequence
        /// </summary>
        [SerializeField, Tooltip("The current offense type of this HitConfirmSubSequence")]
        OffenseType _offenseType;

        /// <summary>
        /// The current direction of this HitConfirmSubSequence.
        /// </summary>
        [SerializeField, Tooltip("The current direction of this HitConfirmSubSequence")]
        OffenseDirection _offenseDirection;

        /// <summary>
        /// SubSequence information when the bot parries and when it gets hit
        /// </summary>
        [SerializeField, Tooltip("SubSequence information when the bot parries and when it gets hit")]
        HitConfirmData _hitConfirmData;

        #endregion

        #region Get

        /// <summary>
        /// Return the current OffenseType for this HitConfirmSubSequence
        /// </summary>
        public OffenseType GetOffenseType => _offenseType;

        /// <summary>
        /// Return the current OffenseDirection for this HitConfirmSubSequence
        /// </summary>
        public OffenseDirection GetOffenseDirection => _offenseDirection;

        /// <summary>
        /// Allows you to return the correct HitConfirmType of the current SubSequence
        /// </summary>
        /// <param name="pIsAttackerBotBlocked">Combat state of the attacking bot</param>
        /// <returns>Return the correct HitConfirmType of the current SubSequence</returns>
        public HitConfirmType GetCurrentHitConfirmType(bool pIsAttackerBotBlocked) => GetCurrentHitConfirmSubSequenceData(pIsAttackerBotBlocked).hitConfirmType;

        /// <summary>
        /// Return the HitConfirmSubSequenceData for this Offense
        /// </summary>
        public HitConfirmData GetHitConfirmData => _hitConfirmData;

        /// <summary>
        /// Checks to return the correct sequence based on the combat state of the attacking bot
        /// </summary>
        /// <param name="pIsAttackerBotBlocked">Combat state of the attacking bot</param>
        /// <returns>Returns the correct sequence for Hit Confirm</returns>
        public HitConfirmSequenceData GetHitConfirmSequenceData(bool pIsAttackerBotBlocked) 
        {
            //Parry
            if (pIsAttackerBotBlocked)
                return _hitConfirmData.parryHitConfirmSequenceData;

            //Hitting
            return _hitConfirmData.hittingHitConfirmSequenceData;
            
        }

        /// <summary>
        /// Allows you to send the current SubSequence
        /// </summary>
        /// <param name="pIsAttackerBotBlocked">Combat state of the attacking bot</param>
        /// <returns>Returns the current SubSequence</returns>
        public HitConfirmSubSequenceData GetCurrentHitConfirmSubSequenceData(bool pIsAttackerBotBlocked) 
        {
            HitConfirmSequenceData hitConfirmSequenceData = GetHitConfirmSequenceData(pIsAttackerBotBlocked);

            return hitConfirmSequenceData.hitConfirmSubSequenceData[hitConfirmSequenceData.currentHitConfirmSubSequenceIndex];
        }

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