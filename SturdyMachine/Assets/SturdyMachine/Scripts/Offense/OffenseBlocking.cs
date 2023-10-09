using System;

using UnityEngine;

using NWH.NUI;
using NWH.VehiclePhysics2;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace SturdyMachine.Offense.Blocking
{
    public enum BlockingType { Second, FrameRate }

    /// <summary>
    /// Store OffenseBlocking information
    /// </summary>
    [Serializable]
    public struct OffenseBlockingData {

        /// <summary>
        /// Changes the verification management according to the chosen type (Frame or Second)
        /// </summary>
        public BlockingType blockingType;

        /// <summary>
        /// Defensive Offense (Deflection)
        /// </summary>
        public Offense offense;

        /// <summary>
        /// Min value for BlockingRange
        /// </summary>
        public float minBlockingRange;

        /// <summary>
        /// Max value for BlockingRange
        /// </summary>
        public float maxBlockingRange;
    }

    /// <summary>
    /// Store all elements of an offense blocking
    /// </summary>
    [CreateAssetMenu(fileName = "NewOffenseBlockingConfig", menuName = "Offence/CustomBlocingOffense", order = 51)]
    public class OffenseBlocking : Offense {

        #region Property

        /// <summary>
        /// array containing all the information concerning the Offense that can be blocked by the Deflection offense.
        /// </summary>
        [SerializeField, Tooltip("Array containing all the information concerning the Offense that can be blocked by the Deflection offense.")]
        OffenseBlockingData[] _offenseBlockingData;

        /// <summary>
        /// The DeflectionOffense on which we want to configure the blocking of attack offenses
        /// </summary>
        [SerializeField, Tooltip("The DeflectionOffense on which we want to configure the blocking of attack offenses")]
        Offense _deflectionOffense;

        #endregion

        #region Get

        /// <summary>
        /// Return array of OffenseBlockingData
        /// </summary>
        public OffenseBlockingData[] GetOffenseBlockingData => _offenseBlockingData;

        /// <summary>
        /// Return the offense that has been configured to block attacking offenses
        /// </summary>
        public Offense GetDeflectionOffense => _deflectionOffense;

        /// <summary>
        /// Checks if the OffenseBlocking can block the attacking offense in the BlockingRange
        /// </summary>
        /// <param name="pAttackerOffense">The attack offense of the attacker </param>
        /// <param name="pAttackerAnimator">L'animator of the attacker</param>
        /// <returns>Returns the result if the attacking offense can be blocked</returns>
        public bool GetIsHitting(Offense pAttackerOffense, Animator pAttackerAnimator)
        {
            //Checks if the attacker animatorClip animation is not the same as the present offense
            if (!pAttackerAnimator.GetCurrentAnimatorClipInfo(0)[0].clip == pAttackerOffense.GetClip)
                return false;

            //Iterate through the array of all offenses that have been configured
            for (int i = 0; i < _offenseBlockingData.Length; ++i)
            {
                //Checks if the attacking offense is present in the array
                if (pAttackerOffense != _offenseBlockingData[i].offense)
                    continue;

                //Return if the attacking offense has been blocked in range
                return Mathf.Clamp(pAttackerAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime, _offenseBlockingData[i].minBlockingRange, _offenseBlockingData[i].maxBlockingRange) == pAttackerAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime;
            }

            return false;
        }

        /// <summary>
        /// Checks if the OffenseBlocking can block the attacking offense
        /// </summary>
        /// <param name="pDefenderOffense">The BlockingOffense which is select to attempt to block the attacking offense</param>
        /// <param name="pAttackerOffense">The attack offense that was sent</param>
        /// <param name="pAttackerBotAnimator">The Animator of attacker</param>
        /// <returns>Returns the result if the BlockingOffense successfully blocked the attacking offense</returns>
        public bool GetIsBlocking(Offense pDefenderOffense, Offense pAttackerOffense, Animator pAttackerBotAnimator)
        {
            //Checks if the BlockingO that was used matches the one that was configured
            if (pDefenderOffense != _deflectionOffense)
                return false;

            //Iterates through the blocking offense array to check if the attacking offense matches one that is configured in the array.
            for (int i = 0; i < _offenseBlockingData.Length; ++i) {

                //Checks if the attack offense is present in the table that has been configured
                if (pAttackerOffense != _offenseBlockingData[i].offense)
                    continue;

                //Returns the result if the BlockingOffense was used in the correct BlockingRange range
                return Mathf.Clamp(pAttackerBotAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime, _offenseBlockingData[i].minBlockingRange, _offenseBlockingData[i].maxBlockingRange) == pAttackerBotAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime;
            }

            return false;
        }

        /// <summary>
        /// Returns the state if the Offense matches the one sent as a parameter
        /// </summary>
        /// <param name="pCurrentIndex">The OffenseBlocking Index</param>
        /// <param name="pCurrentOffense">The Offense You Want to Check</param>
        /// <returns>Returns if the Offense sent as a parameter matches that of the OffenseBlocking that you want to check</returns>
        public bool GetIsGoodOffenseBlocking(int pCurrentIndex, Offense pCurrentOffense) => _offenseBlockingData[pCurrentIndex].offense == pCurrentOffense;

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

            drawer.BeginSubsection("Informations");

            drawer.Field("_deflectionOffense");

            drawer.ReorderableList("_offenseBlockingData");

            drawer.EndSubsection();

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

            drawer.BeginSubsection("Data");

            drawer.Field("offense");

            ShowOffenseData();

            drawer.EndSubsection();

            drawer.EndProperty();
            return true;
        }

        void ShowOffenseData() {

            drawer.Label($"{(drawer.Field("offense").serializedObject.targetObject as Offense).GetClip.length} seconds");

            drawer.FloatSlider("minBlockingRange", 0, (drawer.Field("offense").serializedObject.targetObject as Offense).GetClip.length, "", "", true);

            drawer.FloatSlider("maxBlockingRange", drawer.Field("minBlockingRange").floatValue, (drawer.Field("offense").serializedObject.targetObject as Offense).GetClip.length, "", "", true);
        }
    }
}