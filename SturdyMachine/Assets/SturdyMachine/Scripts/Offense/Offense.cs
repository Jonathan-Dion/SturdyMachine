using UnityEngine;
using System;
using UnityEditor.Graphs;
using NWH.VehiclePhysics2;
using UnityEditor.Experimental.GraphView;

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
    /// Information regarding damage value based on a stance's charge time
    /// </summary>
    [Serializable, Tooltip("Information regarding damage value based on a stance's charge time")]
    public struct StanceIntensityData {

        /// <summary>
        /// Damage information for a light attack
        /// </summary>
        [Tooltip("Damage information for a light attack")]
        public StanceIntensityDamageData lightStanceIntensityDamageData;

        /// <summary>
        /// Damage information for a medium attack
        /// </summary>
        [Tooltip("Damage information for a medium attack")]
        public StanceIntensityDamageData mediumStanceIntensityDamageData;

        /// <summary>
        /// Damage information for a hight attack
        /// </summary>
        [Tooltip("Damage information for a hight attack")]
        public StanceIntensityDamageData hightStanceIntensityDamageData;
    }

    /// <summary>
    /// Configuration information regarding attack intensity
    /// </summary>
    [Serializable, Tooltip("Configuration information regarding attack intensity")]
    public struct StanceIntensityDamageData{

        /// <summary>
        /// The charge time in percentage in order to be able to apply the damage value
        /// </summary>
        [Tooltip("The charge time in percentage in order to be able to apply the damage value"), Range(0, 1)]
        public float intensityTime;

        /// <summary>
        /// The number of damage which corresponds to this intensity
        /// </summary>
        [Tooltip("The number of damage which corresponds to this intensity")]
        public float damageIntensity;
    }

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

        /// <summary>
        /// Animation that should be played when a player successfully blocks all Offense attacks in a sequence
        /// </summary>
        [SerializeField, Tooltip("Animation that should be played when a player successfully blocks all Offense attacks in a sequence")]
        AnimationClip _parryAnimationClip;

        /// <summary>
        /// Animation that should be played when the player successfully blocks an entire combo sequence
        /// </summary>
        [SerializeField, Tooltip("Animation that should be played when the player successfully blocks an entire combo sequence")]
        AnimationClip _staggerAnimationClip;

        /// <summary>
        /// Base Cooldown time
        /// </summary>
        [SerializeField, Tooltip("Base Cooldown time")]
        float _defaultCooldownTimer;

        /// <summary>
        /// Damage Information for this Offense
        /// </summary>
        [SerializeField, Tooltip("Damage Information for this Offense")]
        StanceIntensityData _stanceIntensityData;

        float _currentDamage;

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

            //Stagger
            if (_staggerAnimationClip) {

                if (_staggerAnimationClip.name == pAnimationClipName)
                    return _staggerAnimationClip;
            }

            //KeyposeOut
            return _keyposeOutAnimationClip;
        }

        /// <summary>
        /// Returns the number of frames of an AnimationClip
        /// </summary>
        /// <param name="pIsKeyPoseOut">If the desired AnimationClip is a KeyPoseOut</param>
        /// <returns>Returns the number of frames of an AnimationClip of this Offense depending on the state of the bool parameter</returns>
        public float GetLengthClip(bool pIsKeyPoseOut) {

            if (pIsKeyPoseOut)
                return _keyposeOutAnimationClip.length;

            return _fullAnimationClip.length;
        }

        /// <summary>
        /// Returns Offense when you successfully block all attacks in a combo sequence
        /// </summary>
        public AnimationClip GetParryAnimationClip => _parryAnimationClip;

        /// <summary>
        /// Returns the animation that should be played when the player successfully blocks an entire combo sequence
        /// </summary>
        public AnimationClip GetStaggerAnimationClip => _staggerAnimationClip;

        /// <summary>
        /// Returns information regarding the damages of this Offense
        /// </summary>
        public StanceIntensityData GetStanceIntensityData => _stanceIntensityData;

        bool GetIsStanceIntensity(float pNormalizedTime, float intensityTime) => pNormalizedTime < intensityTime;

        public bool GetIsInStagger(string pAnimationClipName) {
        
            if (_staggerAnimationClip == null)
                return false;

            return GetAnimationClip(pAnimationClipName) == _staggerAnimationClip;
        }

        public float GetCurrentDamage => _currentDamage;

        public float GetCurrentCooldown(string pAnimationClipName) {

            if (_defaultCooldownTimer == 0)
                return GetAnimationClip(pAnimationClipName).length;

            return _defaultCooldownTimer;
        }

        #endregion

        #region Method

        public void IntensityDamage(float pNormalizedTime)
        {

            //Light
            if (GetIsStanceIntensity(pNormalizedTime, GetStanceIntensityData.lightStanceIntensityDamageData.intensityTime))
            {

                _currentDamage = GetStanceIntensityData.lightStanceIntensityDamageData.damageIntensity;

                return;
            }

            //Light to Medium
            if (GetIsStanceIntensity(pNormalizedTime, GetStanceIntensityData.mediumStanceIntensityDamageData.intensityTime))
            {

                _currentDamage = GetStanceIntensityData.lightStanceIntensityDamageData.damageIntensity + (pNormalizedTime / GetStanceIntensityData.mediumStanceIntensityDamageData.damageIntensity);

                return;
            }

            //Medium to Hight
            if (GetIsStanceIntensity(pNormalizedTime, GetStanceIntensityData.hightStanceIntensityDamageData.intensityTime))
            {

                _currentDamage = GetStanceIntensityData.mediumStanceIntensityDamageData.damageIntensity + (pNormalizedTime / GetStanceIntensityData.hightStanceIntensityDamageData.damageIntensity);

                return;
            }

            //Hight
            _currentDamage = GetStanceIntensityData.hightStanceIntensityDamageData.damageIntensity;
        }

        #endregion

#if UNITY_EDITOR

        [CustomEditor(typeof(Offense))]
        public class OffenseEditor : NUIEditor
        {

            OffenseType offenseType = OffenseType.DEFAULT;
            OffenseDirection offenseDirection = OffenseDirection.DEFAULT;

            public override bool OnInspectorNUI()
            {
                if (!base.OnInspectorNUI())
                    return false;

                EditorGUI.BeginChangeCheck();

                offenseDirection = (OffenseDirection)drawer.Field("_offenseDirection", true, null, "Direction: ").enumValueIndex;

                if (offenseDirection != 0) {

                    offenseType = (OffenseType)drawer.Field("_offenseType", true, null, "Type: ").enumValueIndex;

                    DrawAnimationClip();

                    DrawStanceIntensity();
                }

                drawer.Field("_defaultCooldownTimer", true, "sec", "Cooldown: ");

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

                drawer.Label($"{clip.length} seconds", true);

                drawer.Space(10f);

                return true;
            }

            void DrawAnimationClip() {

                drawer.BeginSubsection("Animation");

                if (DrawAnimationClipData(drawer.Field("_fullAnimationClip", true, null, "Complete: ").objectReferenceValue))
                    DrawAnimationClipData(drawer.Field("_keyposeOutAnimationClip", true, null, "KeyposeOut: ").objectReferenceValue);

                //Parry
                if (offenseType == OffenseType.DEFLECTION)
                    drawer.Field("_parryAnimationClip");
                //Stagger
                else if (offenseType != OffenseType.STANCE) 
                {
                    if (offenseDirection != OffenseDirection.STANCE)
                        drawer.Field("_staggerAnimationClip");
                }

                drawer.EndSubsection();
            }

            void DrawStanceIntensity() {

                if (offenseType == OffenseType.DEFAULT)
                    return;

                if (offenseDirection != OffenseDirection.STANCE)
                    return;

                drawer.BeginSubsection("Stance Intensity");

                drawer.Field("_stanceIntensityData");

                drawer.EndSubsection();
            }
        }

        [CustomPropertyDrawer(typeof(StanceIntensityData))]
        public partial class StanceIntensityDataDrawer : ComponentNUIPropertyDrawer
        {
            public override bool OnNUI(Rect position, SerializedProperty property, GUIContent label)
            {
                if (!base.OnNUI(position, property, label))
                    return false;

                drawer.Property("lightStanceIntensityDamageData");
                drawer.Property("mediumStanceIntensityDamageData");
                drawer.Property("hightStanceIntensityDamageData");

                drawer.EndProperty();
                return true;
            }
        }

        [CustomPropertyDrawer(typeof(StanceIntensityDamageData))]
        public partial class StanceIntensityDamageDataDrawer : ComponentNUIPropertyDrawer
        {
            public override bool OnNUI(Rect position, SerializedProperty property, GUIContent label)
            {
                if (!base.OnNUI(position, property, label))
                    return false;

                drawer.Field("intensityTime", true, "%", "Time: ");
                drawer.Field("damageIntensity", true, null, "Damage: ");

                drawer.EndProperty();
                return true;
            }
        }

#endif
    }
}