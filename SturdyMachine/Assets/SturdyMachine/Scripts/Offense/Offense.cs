using System;

using UnityEngine;

using NWH.VehiclePhysics2;

#if UNITY_EDITOR
using NWH.NUI;
using UnityEditor;
#endif

namespace SturdyMachine.Offense 
{
    /// <summary>
    /// Store all type of cooldown for this Offense
    /// </summary>
    [Serializable]
    public struct CooldownData {

        /// <summary>
        /// Cooldown timer when this are Attack type and is blocked with another offense.
        /// </summary>
        [Tooltip("Cooldown timer when this are Attack type and is blocked with another offense.")]
        public float blockingCooldown;

        /// <summary>
        /// Cooldown when this offense are attack type and is used
        /// </summary>
        [Tooltip("Cooldown when this offense are attack type and is used")]
        public float offenseCooldown;

        /// <summary>
        /// Cooldown when this offense is special offense and needs charging time after being used
        /// </summary>
        [Tooltip("Cooldown when this offense is special offense and needs charging time after being used")]
        public float maxCooldown;
    }

    /// <summary>
    /// Store basic Offense information
    /// </summary>
    [CreateAssetMenu(fileName = "NewCustomAnimation", menuName = "SturdyMachine/Offence/OffenceData", order = 1)]
    public class Offense : ScriptableObject
    {
        #region Attribut

        /// <summary>
        /// The animationClip for the current Offense
        /// </summary>
        [SerializeField, Tooltip("The animationClip for the current Offense")]
        protected AnimationClip _clip;

        /// <summary>
        /// The current direction of this offense.
        /// </summary>
        [SerializeField, Tooltip("The current direction of this offense.")]
        protected OffenseDirection _offenseDirection;

        /// <summary>
        /// The current offense tyoe of this offense
        /// </summary>
        [SerializeField, Tooltip("The current offense tyoe of this offense")]
        protected OffenseType _offenseType;

        /// <summary>
        /// The animationClip when this offense type are Deflection and when this offense block another offense
        /// </summary>
        [SerializeField, Tooltip("The animationClip when this offense type are Deflection and when this offense block another offense")]
        protected AnimationClip _repelClip;

        /// <summary>
        /// Represents the number of frames of the Offense AnimationClip
        /// </summary>
        [SerializeField, Tooltip("Represents the number of frames of the Offense AnimationClip")]
        protected float _clipFrame;

        /// <summary>
        /// Store all type of cooldown for this Offense
        /// </summary>
        [SerializeField, Tooltip("Store all type of cooldown for this Offense")]
        protected CooldownData _cooldownData;

        /// <summary>
        /// Represent the max cooldown time for this Offense
        /// </summary>
        [SerializeField, Tooltip("Represent the max cooldown time for this Offense")]
        float _maxCooldownTime;

        #endregion

        #region Get

        /// <summary>
        /// Return the direction of this Offense
        /// </summary>
        public OffenseDirection GetOffenseDirection => _offenseDirection;

        /// <summary>
        /// Return the type of this Offense
        /// </summary>
        public OffenseType GetOffenseType => _offenseType;

        /// <summary>
        /// Return the AnimationClip of this Offense
        /// </summary>
        public AnimationClip GetClip => _clip;

        /// <summary>
        /// Return the AnimationClip Repel for this Offense
        /// </summary>
        public AnimationClip GetRepelClip => _repelClip;

        /// <summary>
        /// Returns if the offense has a cooldown
        /// </summary>
        public bool GetIsCooldownAvailable => _maxCooldownTime > 0;

        /// <summary>
        /// Return the max cooldown time for this Offense
        /// </summary>
        public float GetMaxCooldownTime => _maxCooldownTime;

        /// <summary>
        /// Manages the status feedback of the offense according to the desired offense as a parameter
        /// </summary>
        /// <param name="pOffenseDirection">The Direction of Offense You Want to Check</param>
        /// <param name="pOffenseType">The type of Offense You Want to Check</param>
        /// <returns>Return if this offense according to the Offense on parameter</returns>
        public bool GetIsGoodOffense(OffenseDirection pOffenseDirection, OffenseType pOffenseType)
        {
            //If the Offense direction concord with that as parameter 
            if (pOffenseDirection == _offenseDirection) 
            {
                //Return true if the Offense type on parameter is a Repel Offense type
                if (pOffenseType == OffenseType.REPEL)
                {
                    if (_repelClip)
                        return true;
                }

                //Return true if the Offense type on parameter concord with that as parameter 
                else if (pOffenseType == _offenseType)
                    return true;
            }

            return false;
        }

        public float GetClipFrames => _clipFrame;

        #endregion

        void OnDisable(){

            _maxCooldownTime = 0;
        }

#if UNITY_EDITOR

        [CustomEditor(typeof(Offense))]
        public class OffenseEditor : NUIEditor
        {
            AnimationClip clip;
            float clipFrame;

            public override bool OnInspectorNUI()
            {
                if (!base.OnInspectorNUI())
                    return false;

                EditorGUI.BeginChangeCheck();

                drawer.BeginSubsection("Offense");

                ShowOffenseInformations();

                ShowClip();

                ShowCooldown();

                drawer.EndSubsection();

                drawer.EndEditor(this);
                return true;
            }

            void ShowClip() {

                drawer.BeginSubsection("Animation clip");

                clip = drawer.Field("_clip").objectReferenceValue as AnimationClip;

                clipFrame = drawer.Field("_clipFrame", true, "frames", "Numbers of frames: ").floatValue;

                drawer.Field("_repelClip", true, null, "Repel: ");

                drawer.EndSubsection();
            }

            void ShowOffenseInformations() {

                drawer.BeginSubsection("Informations");

                drawer.Field("_offenseDirection", true, null, "Direction: ");
                int offenseTypeIndex = drawer.Field("_offenseType", true, null, "Type: ").enumValueIndex;

                drawer.Space();

                if (offenseTypeIndex > 1) {

                    if (offenseTypeIndex <= 6) {

                        if (clip)
                            drawer.Label($"{clipFrame} frames");

                        //drawer.Property("_hitConfirmData");
                    }
                }

                drawer.EndSubsection();
            }

            void ShowCooldown() {

                drawer.BeginSubsection("Cooldown");

                drawer.Field("_maxCooldownTime");

                drawer.Property("_cooldownData");

                drawer.EndSubsection();
            }
        }

        [CustomPropertyDrawer(typeof(CooldownData))]
        public partial class CooldownDataDrawer : ComponentNUIPropertyDrawer
        {
            public override bool OnNUI(Rect position, SerializedProperty property, GUIContent label)
            {
                if (!base.OnNUI(position, property, label))
                    return false;

                drawer.Field("blockingCooldown", true, "sec", "Blocking: ");
                drawer.Field("offenseCooldown", true, "sec", "Offense: ");
                drawer.Field("maxCooldown", true, "sec", "Max: ");

                drawer.EndProperty();
                return true;
            }
        }

#endif
    }
}