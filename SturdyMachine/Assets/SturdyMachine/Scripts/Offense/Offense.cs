using UnityEngine;
using System;
using SturdyMachine.Component;
using SturdyMachine.Offense.Blocking;

#if UNITY_EDITOR
using NWH.NUI;
using UnityEditor;
using NWH.VehiclePhysics2;
#endif

namespace SturdyMachine.Offense 
{
    /// <summary>
    /// Represents possible types of offense
    /// </summary>
    public enum AnimationClipOffenseType { Full, KeyposeOut, Parry, Stagger }

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

        public bool isActivated;

        /// <summary>
        /// Damage information for a light attack
        /// </summary>
        [Tooltip("Damage information for a light attack")]
        public IntensityDamageData lightStanceIntensityDamageData;

        /// <summary>
        /// Damage information for a medium attack
        /// </summary>
        [Tooltip("Damage information for a medium attack")]
        public IntensityDamageData mediumStanceIntensityDamageData;

        /// <summary>
        /// Damage information for a hight attack
        /// </summary>
        [Tooltip("Damage information for a hight attack")]
        public IntensityDamageData hightStanceIntensityDamageData;
    }

    /// <summary>
    /// Configuration information regarding attack intensity
    /// </summary>
    [Serializable, Tooltip("Configuration information regarding attack intensity")]
    public struct IntensityDamageData{

        public bool isActivated;

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
    /// Allows you to configure the blocking zone to visually match the Offense of the attacking Bot.
    /// </summary>
    [Serializable, Tooltip("Allows you to configure the blocking zone to visually match the Offense of the attacking Bot.")]
    public struct DeflectionBlockingRangeData {

        /// <summary>
        /// Represents the minimum value of the zone
        /// </summary>
        [Tooltip("Represents the minimum value of the zone")]
        public BlockingRangeData minDeflectionBlockingRangeData;

        /// <summary>
        /// Represents the maximum value of the zone
        /// </summary>
        [Tooltip("Represents the maximum value of the zone")]
        public BlockingRangeData maxDeflectionBlockingRangeData;
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

        /// <summary>
        /// Configuration information regarding attack intensity
        /// </summary>
        [SerializeField, Tooltip("Configuration information regarding attack intensity")]
        IntensityDamageData _intensityDamageData;

        /// <summary>
        /// Allows you to configure the blocking zone to visually match the Offense of the attacking Bot.
        /// </summary>
        [SerializeField, Tooltip("Allows you to configure the blocking zone to visually match the Offense of the attacking Bot.")]
        DeflectionBlockingRangeData _deflectionBlockingRageData;

        float _currentDamage;

        #endregion

        #region Properties

        /// <summary>
        /// Return the direction of this Offense
        /// </summary>
        public OffenseDirection GetOffenseDirection => _offenseDirection;

        /// <summary>
        /// Return the type of this Offense
        /// </summary>
        public OffenseType GetOffenseType => _offenseType;

        /// <summary>
        /// Returns the AnimationClip of this Offense depending on the type of offense in parameter
        /// </summary>
        /// <param name="pAnimationClipOffenseType">Offense type of animationClip you need</param>
        /// <returns>Returns the correct AnimationClip</returns>
        public AnimationClip GetAnimationClip(AnimationClipOffenseType pAnimationClipOffenseType) 
        {
            //KeyposeOut
            if (pAnimationClipOffenseType == AnimationClipOffenseType.KeyposeOut)
                return _keyposeOutAnimationClip;

            //Parry
            if (pAnimationClipOffenseType == AnimationClipOffenseType.Parry)
                return _parryAnimationClip;

            //Stagger
            if (pAnimationClipOffenseType == AnimationClipOffenseType.Stagger) 
                return _staggerAnimationClip ? _staggerAnimationClip : null;
            
            //Full
            return _fullAnimationClip;
        }

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
        /// Returns the number of frames of an animationClip depending on the type set as parameter
        /// </summary>
        /// <param name="pAnimationClipOffenseType">The type of offense from the desired animationClip</param>
        /// <returns>Returns the number of frames of an AnimationClip of this Offense depending on the state of the bool parameter</returns>
        public float GetLengthClip(AnimationClipOffenseType pAnimationClipOffenseType) {

            //KeyposeOut
            if (pAnimationClipOffenseType == AnimationClipOffenseType.KeyposeOut)
                return _keyposeOutAnimationClip.length;

            //Parry
            if (pAnimationClipOffenseType == AnimationClipOffenseType.Parry)
                return _parryAnimationClip.length;

            //Stagger
            if (pAnimationClipOffenseType == AnimationClipOffenseType.Stagger)
                return _staggerAnimationClip ? _staggerAnimationClip.length : 0;

            //Full
            return _fullAnimationClip.length;
        }

        /// <summary>
        /// Returns information regarding the damages of this Offense
        /// </summary>
        public StanceIntensityData GetStanceIntensityData => _stanceIntensityData;

        public float GetCurrentDamageIntensity(BotType pBotType)
        {

            //Sturdy
            if (pBotType == BotType.SturdyBot)
                return _currentDamage;

            //Enemy
            if (_intensityDamageData.isActivated)
                return _intensityDamageData.damageIntensity;

            return 0;
        }

        public bool GetIsInStagger(string pAnimationClipName) {
        
            if (_staggerAnimationClip == null)
                return false;

            return GetAnimationClip(pAnimationClipName) == _staggerAnimationClip;
        }

        public float GetCurrentCooldown(string pAnimationClipName) {

            if (_defaultCooldownTimer == 0)
                return GetAnimationClip(pAnimationClipName).length;

            return _defaultCooldownTimer;
        }

        /// <summary>
        /// Returns information regarding the Deflection BlockingRange configuration
        /// </summary>
        public DeflectionBlockingRangeData GetDeflectionBlockingRangeData => _deflectionBlockingRageData;

        public bool GetIsInDeflectionRange(float pNormalizedTime){

            //Min
            if (pNormalizedTime > _deflectionBlockingRageData.minDeflectionBlockingRangeData.rangeTime) {

                if (pNormalizedTime < _deflectionBlockingRageData.maxDeflectionBlockingRangeData.rangeTime)
                    return true;
            }

            return false;
        }

        bool GetIsStanceIntensity(float pNormalizedTime, float intensityTime) => pNormalizedTime < intensityTime;

        float GetCurrentIntensityDamage(float pNormalizedTime) 
        {
            //StanceDamageIntensity
            if (_stanceIntensityData.isActivated) 
                return GetCurrentStanceIntensityDamage(pNormalizedTime);

            //DamageIntensity
            return _intensityDamageData.damageIntensity;
        }

        float GetCurrentStanceIntensityDamage(float pNormalizedTime) 
        {
            //Light
            if (GetIsStanceIntensity(pNormalizedTime, GetStanceIntensityData.lightStanceIntensityDamageData.intensityTime))
                return GetStanceIntensityData.lightStanceIntensityDamageData.damageIntensity;

            /*//Light to Medium
            if (GetIsStanceIntensity(pNormalizedTime, GetStanceIntensityData.mediumStanceIntensityDamageData.intensityTime))
                return GetStanceIntensityData.lightStanceIntensityDamageData.damageIntensity + (pNormalizedTime / GetStanceIntensityData.mediumStanceIntensityDamageData.damageIntensity);
            */

            //Medium
            if (GetIsStanceIntensity(pNormalizedTime, GetStanceIntensityData.mediumStanceIntensityDamageData.intensityTime))
                return GetStanceIntensityData.mediumStanceIntensityDamageData.damageIntensity;

            /*//Medium to Hight
            if (GetIsStanceIntensity(pNormalizedTime, GetStanceIntensityData.hightStanceIntensityDamageData.intensityTime))
                return GetStanceIntensityData.mediumStanceIntensityDamageData.damageIntensity + (pNormalizedTime / GetStanceIntensityData.hightStanceIntensityDamageData.damageIntensity);
            */

            //High
            return GetStanceIntensityData.hightStanceIntensityDamageData.damageIntensity;
        }

        #endregion

        #region Method

        public void StanceIntensityDamagae(float pNormalizedTime)
        {
            _currentDamage = GetCurrentIntensityDamage(pNormalizedTime);
        }

        #endregion

#if UNITY_EDITOR

        [CustomEditor(typeof(Offense))]
        public class OffenseEditor : NUIEditor
        {

            OffenseType offenseType = OffenseType.DEFAULT;
            OffenseDirection offenseDirection = OffenseDirection.DEFAULT;

            AnimationClip _fullAnimationClip;

            public override bool OnInspectorNUI()
            {
                if (!base.OnInspectorNUI())
                    return false;

                EditorGUI.BeginChangeCheck();

                offenseDirection = (OffenseDirection)drawer.Field("_offenseDirection", true, null, "Direction: ").enumValueIndex;

                if (offenseDirection != 0) {

                    offenseType = (OffenseType)drawer.Field("_offenseType", true, null, "Type: ").enumValueIndex;

                    DrawAnimationClip();

                    DrawDeflectionBlockingRange();

                    DrawIntensity();
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

                _fullAnimationClip = drawer.Field("_fullAnimationClip", true, null, "Complete: ").objectReferenceValue as AnimationClip;

                if (_fullAnimationClip) {

                    drawer.Label($"{_fullAnimationClip.length} seconds", true);

                    drawer.Space(10f);

                    DrawAnimationClipData(drawer.Field("_keyposeOutAnimationClip", true, null, "KeyposeOut: ").objectReferenceValue);
                }

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

            void DrawIntensity() {

                if (offenseType == OffenseType.DEFLECTION)
                    return;

                DrawDamageIntensity();

                DrawStanceIntensity();                
            }

            void DrawDamageIntensity() 
            {
                if (offenseDirection == OffenseDirection.STANCE)
                    return;

                drawer.BeginSubsection("DamageIntensity");

                drawer.Field("_intensityDamageData");

                drawer.EndSubsection();
            }

            void DrawStanceIntensity() 
            {
                if (offenseDirection != OffenseDirection.STANCE)
                    return;

                drawer.BeginSubsection("Stance Intensity");

                drawer.Field("_stanceIntensityData");

                drawer.EndSubsection();
            }

            void DrawDeflectionBlockingRange() {

                if (offenseType != OffenseType.DEFLECTION)
                    return;

                drawer.Property("_deflectionBlockingRageData");

                if (drawer.FindProperty("_deflectionBlockingRageData") != null) {

                    drawer.FindProperty("_deflectionBlockingRageData").FindPropertyRelative("minDeflectionBlockingRangeData").FindPropertyRelative("offenseFrameCount").floatValue = _fullAnimationClip.length * _fullAnimationClip.frameRate;
                    drawer.FindProperty("_deflectionBlockingRageData").FindPropertyRelative("maxDeflectionBlockingRangeData").FindPropertyRelative("offenseFrameCount").floatValue = _fullAnimationClip.length * _fullAnimationClip.frameRate;
                }
            }
        }

        [CustomPropertyDrawer(typeof(StanceIntensityData))]
        public partial class StanceIntensityDataDrawer : ComponentNUIPropertyDrawer
        {
            public override bool OnNUI(Rect position, SerializedProperty property, GUIContent label)
            {
                if (!base.OnNUI(position, property, label))
                    return false;

                if (drawer.Field("isActivated", true).boolValue) {

                    drawer.Property("lightStanceIntensityDamageData");
                    drawer.Property("mediumStanceIntensityDamageData");
                    drawer.Property("hightStanceIntensityDamageData");
                }

                drawer.EndProperty();
                return true;
            }
        }

        [CustomPropertyDrawer(typeof(IntensityDamageData))]
        public partial class IntensityDamageDataDataDrawer : ComponentNUIPropertyDrawer
        {
            public override bool OnNUI(Rect position, SerializedProperty property, GUIContent label)
            {
                if (!base.OnNUI(position, property, label))
                    return false;

                if (drawer.Field("isActivated").boolValue) {

                    drawer.Field("intensityTime", true, "%", "Time: ");
                    drawer.Field("damageIntensity", true, null, "Damage: ");
                }

                drawer.EndProperty();
                return true;
            }
        }

        [CustomPropertyDrawer(typeof(DeflectionBlockingRangeData))]
        public partial class DeflectionBlockingRangeDataDrawer : ComponentNUIPropertyDrawer
        {
            public override bool OnNUI(Rect position, SerializedProperty property, GUIContent label)
            {
                if (!base.OnNUI(position, property, label))
                    return false;

                drawer.Field("minDeflectionBlockingRangeData");
                drawer.Field("maxDeflectionBlockingRangeData");

                drawer.EndProperty();
                return true;
            }
        }

#endif
    }
}