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
                return GetIsIntoBlockingRange(_offenseBlockingData[i], pAttackerAnimator, pAttackerOffense);
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
        public bool GetIsBlocking(Offense pDefenderOffense, Offense pAttackerOffense, Animator pAttackerBotAnimator, bool pIsPlayer)
        {
            //Iterates through the blocking offense array to check if the attacking offense matches one that is configured in the array.
            for (int i = 0; i < _offenseBlockingData.Length; ++i) {

                //Checks if the attack offense is present in the table that has been configured
                if (pAttackerOffense != _offenseBlockingData[i].offense)
                    continue;

                if (pIsPlayer) {
                
                     if (GetIsIntoBlockingRange(_offenseBlockingData[i], pAttackerBotAnimator, pAttackerOffense))
                        return pDefenderOffense == _deflectionOffense;

                     return false;
                }

                //Returns the result if the BlockingOffense was used in the correct BlockingRange range
                return GetIsIntoBlockingRange(_offenseBlockingData[i], pAttackerBotAnimator, pAttackerOffense);
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

        /// <summary>
        /// Checks if the BlockingOffense was performed in the blocking range
        /// </summary>
        /// <param name="pOffenceBlockingData">OffenseBlockingData for defending bot</param>
        /// <param name="pAttackerBotAnimator">AttackerBot animator</param>
        /// <param name="pAttackerOffense">Offense of AttackerBot</param>
        /// <returns>Returns if the offense was made in the section and the bot can block the attacker's offense</returns>
        bool GetIsIntoBlockingRange(OffenseBlockingData pOffenceBlockingData, Animator pAttackerBotAnimator, Offense pAttackerOffense) {

            float currentFrame = pAttackerBotAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime * pAttackerOffense.GetClip.frameRate / 1;

            if (currentFrame >= pOffenceBlockingData.minBlockingRange)
                return currentFrame <= pOffenceBlockingData.maxBlockingRange;

            return false;
        }

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

            EditorGUI.BeginChangeCheck();

            drawer.BeginSubsection("Data");

            ShowOffenseData();

            drawer.EndSubsection();

            drawer.EndProperty();
            return true;
        }

        void ShowOffenseData() {

            Offense offense = drawer.Field("offense").objectReferenceValue as Offense;

            if (!offense)
                return;

            drawer.Label($"{offense.GetClip.frameRate} frames");

            drawer.FloatSlider("minBlockingRange", 0, offense.GetClip.frameRate, "", "", true);

            drawer.FloatSlider("maxBlockingRange", 0, offense.GetClip.frameRate, "", "", true);
        }
    }
}