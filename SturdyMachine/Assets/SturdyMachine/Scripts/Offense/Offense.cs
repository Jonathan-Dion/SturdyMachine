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
    /// Represents all possible directions of an Offense
    /// </summary>
    public enum OffenseDirection { DEFAULT, NEUTRAL, RIGHT, LEFT, STANCE }

    /// <summary>
    /// Represents all possible types of an Offense
    /// </summary>
    public enum OffenseType { DEFAULT, DEFLECTION, EVASION, SWEEP, STRIKE, HEAVY, DEATHBLOW, DAMAGEHIT, REPEL, STANCE};

    /// <summary>
    /// Store basic Offense information
    /// </summary>
    [CreateAssetMenu(fileName = "NewOffense", menuName = "SturdyMachine/Offense/Offense", order = 1)]
    public class Offense : ScriptableObject
    {
        #region Attribut

        /// <summary>
        /// Represents the Direction of this Offense
        /// </summary>
        [SerializeField, Tooltip("Represents the Direction of this Offense")]
        OffenseDirection _offenseDirection;

        /// <summary>
        /// Represents the type of this Offense
        /// </summary>
        [SerializeField, Tooltip("Represents the type of this Offense")]
        OffenseType _offenseType;

        /// <summary>
        /// Represents the full animation of this Offense
        /// </summary>
        [SerializeField, Tooltip("Represents the full animation of this Offense")]
        AnimationClip _fullAnimationClip;

        /// <summary>
        /// Represents the AnimationClip that should be played during HitConfirm
        /// </summary>
        [SerializeField, Tooltip("Represents the AnimationClip that should be played during HitConfirm")]
        AnimationClip _keyposeOutAnimationClip;

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
        /// Returns the AnimationClip of this Offense based on the state parameter
        /// </summary>
        /// <param name="pIsKeyposeClip">State of AnimationClip</param>
        /// <returns>Returns the correct AnimationClip</returns>
        public AnimationClip GetAnimationClip(bool pIsKeyposeClip = false) => !pIsKeyposeClip ? _fullAnimationClip : _keyposeOutAnimationClip;

        /// <summary>
        /// Returns the AnimationClio based on a name
        /// </summary>
        /// <param name="pAnimationClipName">The name of the AnimationClip</param>
        /// <returns>Returns the correct Animation based on the clip name as a parameter</returns>
        public AnimationClip GetAnimationClip(string pAnimationClipName) {
        
            //Complete
            if (_fullAnimationClip.name == pAnimationClipName)
                return _fullAnimationClip;

            //KeyposeOut
            return _keyposeOutAnimationClip;
        }

        /// <summary>
        /// Returns the number of frames of an AnimationClip
        /// </summary>
        /// <param name="pIsKeyPoseOut">If the desired AnimationClip is a KeyPoseOut</param>
        /// <returns>Returns the number of frames of an AnimationClip of this Offense depending on the state of the bool parameter</returns>
        public float GetNumberOfFrame(bool pIsKeyPoseOut) {

            if (pIsKeyPoseOut)
                return _keyposeOutAnimationClip.length * _keyposeOutAnimationClip.frameRate;

            return _fullAnimationClip.length * _fullAnimationClip.frameRate;
        }

        #endregion

#if UNITY_EDITOR

        [CustomEditor(typeof(Offense))]
        public class OffenseEditor : NUIEditor
        {
            public override bool OnInspectorNUI()
            {
                if (!base.OnInspectorNUI())
                    return false;

                EditorGUI.BeginChangeCheck();

                if (drawer.Field("_offenseDirection", true, null, "Direction: ").enumValueIndex != 0) {

                    drawer.Field("_offenseType", true, null, "Type: ");

                    DrawAnimationClip();
                }

                drawer.EndEditor(this);
                return true;
            }

            /// <summary>
            /// Shows the number of AnimationClip frames
            /// </summary>
            /// <param name="pSerializedObject">AnimationClip serialized</param>
            /// <returns>Returns if the value of Serialized Clip Animation is assigned</returns>
            bool DrawAnimationClipData(UnityEngine.Object pSerializedObject) {

                if (!pSerializedObject)
                    return false;

                AnimationClip clip = pSerializedObject as AnimationClip;

                drawer.Label($"{clip.length * clip.frameRate} frames", true);

                drawer.Space(10f);

                return true;
            }

            void DrawAnimationClip() {

                drawer.BeginSubsection("Animation");

                if (DrawAnimationClipData(drawer.Field("_fullAnimationClip", true, null, "Complete: ").objectReferenceValue))
                    DrawAnimationClipData(drawer.Field("_keyposeOutAnimationClip", true, null, "KeyposeOut: ").objectReferenceValue);

                drawer.EndSubsection();
            }
        }

        /*[CustomPropertyDrawer(typeof(CooldownData))]
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
        }*/

#endif
    }
}