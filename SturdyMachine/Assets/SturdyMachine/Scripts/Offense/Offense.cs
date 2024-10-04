using System;
using System.Collections.Generic;

using UnityEngine;

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
    public enum OffenseType { DEFAULT, DEFLECTION, STRIKE, HEAVY, DAMAGEHIT, STANCE, STUN, TAUNT};

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
    /// Allows you to remember all the information concerning the offense in question
    /// </summary>
    [Serializable, Tooltip("Allows you to remember all the information concerning the offense in question")]
    public struct OffenseData {

        /// <summary>
        /// The type of animationClip
        /// </summary>
        [Tooltip("The type of animationClip")]
        public AnimationClipOffenseType animationClipOffenseType;

        /// <summary>
        /// Represents the AnimationClip matching the type
        /// </summary>
        [Tooltip("Represents the Clip Animation matching the type")]
        public AnimationClip offenseAnimationClip;

        /// <summary>
        /// Represents the audio that should be played when executing this offense
        /// </summary>
        [Tooltip("Represents the audio that should be played when executing this offense")]
        public AudioClip offenseAudioClip;

        /// <summary>
        /// Checks if the Animation type matches that in the parameter
        /// </summary>
        /// <param name="pAnimationClipOffenseType">The type of Animation we want to check if it matches</param>
        /// <returns>Returns the state if the type of Animation you want to check as a parameter matches the one in this structure</returns>
        public bool GetIfAnimationClipOffenseTypeMatches(AnimationClipOffenseType pAnimationClipOffenseType) => pAnimationClipOffenseType == animationClipOffenseType;

        public bool GetIfAnimationClipNameMatches(string pAnimationClipName) {

            if (offenseAnimationClip == null)
                return false;

            return offenseAnimationClip.name == pAnimationClipName;
        }
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

    [Serializable]
    public struct CooldownData {

        public float currentPercentage;

        public float currentCooldown;
    }

    /// <summary>
    /// Store basic Offense information
    /// </summary>
    [CreateAssetMenu(fileName = "NewOffense", menuName = "SturdyMachine/Offense/Offense", order = 1)]
    public class Offense : ScriptableObject
    {
        #region Attributes

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
        /// Represents the full animation data for this Offense
        /// </summary>
        [SerializeField, Tooltip("Represents the full animation data for this Offense")]
        OffenseData _fullOffenseData;

        /// <summary>
        /// Represents the keyposeOut animation data that should be played during HitConfirm
        /// </summary>
        [SerializeField, Tooltip("Represents the keyposeOut animation data that should be played during HitConfirm")]
        OffenseData _keyposeOutOffenseData;

        /// <summary>
        /// Animation data that should be played when a player successfully blocks all Offense attacks in a sequence
        /// </summary>
        [SerializeField, Tooltip("Animation data that should be played when a player successfully blocks all Offense attacks in a sequence")]
        OffenseData _parryOffenseData;

        /// <summary>
        /// Animation data that should be played when the player successfully blocks an entire combo sequence
        /// </summary>
        [SerializeField, Tooltip("Animation data that should be played when the player successfully blocks an entire combo sequence")]
        OffenseData _staggerOffenseData;

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
        DeflectionBlockingRangeData _deflectionBlockingRangeData;

        [SerializeField]
        CooldownData _cooldownData;

        float _currentDamage;

        OffenseData[] _offenseDatas;

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
            for (byte i = 0; i < _offenseDatas.Length; ++i) {

                if (!_offenseDatas[i].GetIfAnimationClipOffenseTypeMatches(pAnimationClipOffenseType))
                    continue;

                return _offenseDatas[i].offenseAnimationClip;
            }

            return null;
        }

        /// <summary>
        /// Returns the AnimationClip based on a name
        /// </summary>
        /// <param name="pAnimationClipName">The name of the AnimationClip</param>
        /// <returns>Returns the correct Animation based on the clip name as a parameter</returns>
        public AnimationClip GetAnimationClip(string pAnimationClipName) {

            for (byte i = 0; i < _offenseDatas.Length; ++i) {

                if (_offenseDatas[i].offenseAnimationClip == null) {

                    //Debug.LogWarning($"The AnimationClip for Offense {_offenseDatas[i].animationClipOffenseType} type with {_offenseType} {_offenseDirection} is not correctly assigned!");

                    continue;
                }

                if (!_offenseDatas[i].GetIfAnimationClipNameMatches(pAnimationClipName))
                    continue;

                return _offenseDatas[i].offenseAnimationClip;
            }

            return null;
        }

        /// <summary>
        /// Returns the number of frames of an animationClip depending on the type set as parameter
        /// </summary>
        /// <param name="pAnimationClipOffenseType">The type of offense from the desired animationClip</param>
        /// <returns>Returns the number of frames of an AnimationClip of this Offense depending on the state of the bool parameter</returns>
        public float GetLengthClip(AnimationClipOffenseType pAnimationClipOffenseType) {

            for (byte i = 0; i < _offenseDatas.Length; ++i) {

                if (_offenseDatas[i].offenseAnimationClip == null){

                    Debug.LogWarning($"The AnimationClip for Offense {_offenseDatas[i].animationClipOffenseType} type with {_offenseType} {_offenseDirection} is not correctly assigned!");

                    continue;
                }

                if (!_offenseDatas[i].GetIfAnimationClipOffenseTypeMatches(pAnimationClipOffenseType))
                    continue;

                return _offenseDatas[i].offenseAnimationClip.length;
            }

            //Full
            return 0f;
        }

        public AudioClip GetAudioOffenseDataClip(AnimationClipOffenseType pAnimationClipOffenseType) {

            for (byte i = 0; i < _offenseDatas.Length; ++i){

                if (_offenseDatas[i].offenseAudioClip == null){

                    if (_offenseType == OffenseType.DEFAULT) {

                        if (_offenseDirection == OffenseDirection.STANCE)
                            continue;
                    }

                    Debug.LogWarning($"The AudioClip for Offense {_offenseDatas[i].animationClipOffenseType} type with {_offenseType} {_offenseDirection} is not correctly assigned!");

                    continue;
                }

                if (!_offenseDatas[i].GetIfAnimationClipOffenseTypeMatches(pAnimationClipOffenseType))
                    continue;

                return _offenseDatas[i].offenseAudioClip;
            }

            return null;
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

            for (byte i = 0; i < _offenseDatas.Length; ++i) {

                if (_offenseDatas[i].offenseAnimationClip == null){
                    Debug.LogWarning($"The AnimationClip for Offense {_offenseDatas[i].animationClipOffenseType} type with {_offenseType} {_offenseDirection} is not correctly assigned!");

                    continue;
                }

                if (!_offenseDatas[i].GetIfAnimationClipNameMatches(pAnimationClipName))
                    continue;

                return true;
            }

            return false;
        }

        public float GetCurrentCooldown(string pAnimationClipName) {

            if (_cooldownData.currentPercentage == 0f)
                return GetAnimationClip(pAnimationClipName).length;

            return GetAnimationClip(pAnimationClipName).length * _cooldownData.currentPercentage;
        }

        /// <summary>
        /// Returns information regarding the Deflection BlockingRange configuration
        /// </summary>
        public DeflectionBlockingRangeData GetDeflectionBlockingRangeData => _deflectionBlockingRangeData;

        public bool GetIsInDeflectionRange(float pNormalizedTime) {

            //Min
            if (pNormalizedTime > _deflectionBlockingRangeData.minDeflectionBlockingRangeData.rangeTime) {

                if (pNormalizedTime < _deflectionBlockingRangeData.maxDeflectionBlockingRangeData.rangeTime)
                    return true;
            }

            return false;
        }

        bool GetIsStanceIntensity(float pNormalizedTime, float intensityTime) => pNormalizedTime < intensityTime;

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

        public bool GetOffenseIsInStanceMode 
        {
            get 
            {
                //DefaultStance
                if (_offenseType == OffenseType.DEFAULT)
                    return true;

                return _offenseDirection == OffenseDirection.STANCE;
            }
        }

        /// <summary>
        /// Checks if the bot is in attack phase
        /// </summary>
        /// <returns>Returns if the bot chosen as parameter is in the attack phase</returns>
        public bool GetOffenseIsInAttackMode 
        {
            get 
            {
                //Check if the current offense is DamageHit type
                if (_offenseType == OffenseType.DAMAGEHIT)
                    return false;

                //Check if the current offense if Stun type
                if (_offenseType == OffenseType.STUN)
                    return false;

                //Checks if the current offense is Stance mode
                if (GetOffenseIsInStanceMode)
                    return false;

                //Checks if the current offense is in the Deflection
                return _offenseType != OffenseType.DEFLECTION;
                    
            }
        }

        #endregion

        #region Method

        public void StanceIntensityDamagae(float pNormalizedTime)
        {
            //StanceDamageIntensity
            if (_stanceIntensityData.isActivated) {

                _currentDamage = GetCurrentStanceIntensityDamage(pNormalizedTime);

                return;
            }

            //DamageIntensity
            _currentDamage = _intensityDamageData.damageIntensity;
        }

        void OnEnable()
        {
            List<OffenseData> offenseDatas = new List<OffenseData>();

            //Full
            offenseDatas.Add(_fullOffenseData);

            //KeyposeOut
            if (!_keyposeOutOffenseData.Equals(new OffenseData()))
                offenseDatas.Add(_keyposeOutOffenseData);

            //Parry
            if (!_parryOffenseData.Equals(new OffenseData()))
                offenseDatas.Add(_parryOffenseData);

            //Stagger
            if (!_staggerOffenseData.Equals(new OffenseData()))
                offenseDatas.Add(_staggerOffenseData);

            _offenseDatas = offenseDatas.ToArray();
        }

        #endregion

#if UNITY_EDITOR

        [CustomEditor(typeof(Offense))]
        public class OffenseEditor : NUIEditor
        {

            OffenseType offenseType = OffenseType.DEFAULT;
            OffenseDirection offenseDirection = OffenseDirection.DEFAULT;

            AnimationClip _fullAnimationClip;

            OffenseData _currentOffenseData;

            public override bool OnInspectorNUI()
            {
                if (!base.OnInspectorNUI())
                    return false;

                EditorGUI.BeginChangeCheck();

                offenseDirection = (OffenseDirection)drawer.Field("_offenseDirection", true, null, "Direction: ").enumValueIndex;

                if (offenseDirection != 0) {

                    offenseType = (OffenseType)drawer.Field("_offenseType", true, null, "Type: ").enumValueIndex;

                    drawer.BeginSubsection("Animation");

                    //AnimationClip

                    _fullAnimationClip = drawer.Field("_fullOffenseData", true, null, "Complete: ").FindPropertyRelative("offenseAnimationClip").objectReferenceValue as AnimationClip;

                    if (_fullAnimationClip)
                    {
                        drawer.Label($"{_fullAnimationClip.length} seconds", true);

                        drawer.Space(10f);

                        AnimationClip keyposeOutAnimationClip = drawer.Field("_keyposeOutOffenseData", true, null, "KeyposeOut: ").FindPropertyRelative("offenseAnimationClip").objectReferenceValue as AnimationClip;

                        if (keyposeOutAnimationClip){
                            drawer.Label($"{keyposeOutAnimationClip.length} seconds", true);

                            drawer.Space(10f);
                        }

                        if (offenseType != OffenseType.DEFLECTION)
                        {
                            //Stagger
                            if (offenseType != OffenseType.STANCE)
                            {
                                if (offenseDirection != OffenseDirection.STANCE)
                                    drawer.Field("_staggerOffenseData", true, null, "Stagger: ");
                            }
                        }

                        //Parry
                        else

                            drawer.Field("_parryOffenseData", true, null, "Parry: ");
                    }

                    drawer.EndSubsection();

                    //DeflectionBlockingRange
                    if (_fullAnimationClip) {

                        if (offenseType == OffenseType.DEFLECTION) {

                            drawer.Property("_deflectionBlockingRangeData");

                            if (drawer.FindProperty("_deflectionBlockingRangeData") != null)
                            {

                                drawer.FindProperty("_deflectionBlockingRangeData").FindPropertyRelative("minDeflectionBlockingRangeData").FindPropertyRelative("offenseFrameCount").floatValue = _fullAnimationClip.length * _fullAnimationClip.frameRate;
                                drawer.FindProperty("_deflectionBlockingRangeData").FindPropertyRelative("maxDeflectionBlockingRangeData").FindPropertyRelative("offenseFrameCount").floatValue = _fullAnimationClip.length * _fullAnimationClip.frameRate;
                            }
                        }
                    }

                    //Intensity
                    if (offenseType != OffenseType.DEFLECTION) {

                        //Damage
                        if (offenseDirection != OffenseDirection.STANCE)
                        {

                            drawer.BeginSubsection("DamageIntensity");

                            drawer.Field("_intensityDamageData");

                            drawer.EndSubsection();
                        }

                        //Stance
                        else {

                            drawer.BeginSubsection("Stance Intensity");

                            drawer.Field("_stanceIntensityData");

                            drawer.EndSubsection();
                        }

                    }
                }                
                
                drawer.Property("_cooldownData");

                if (drawer.FindProperty("_cooldownData") != null) {
                
                    if (_fullAnimationClip)
                        drawer.FindProperty("_cooldownData").FindPropertyRelative("currentCooldown").floatValue = drawer.FindProperty("_cooldownData").FindPropertyRelative("currentPercentage").floatValue * _fullAnimationClip.length;
                }


                drawer.EndEditor(this);
                return true;
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

        [CustomPropertyDrawer(typeof(OffenseData))]
        public partial class OffenseDataDrawer : ComponentNUIPropertyDrawer
        {
            public override bool OnNUI(Rect position, SerializedProperty property, GUIContent label)
            {
                if (!base.OnNUI(position, property, label))
                    return false;

                if (drawer.Field("offenseAnimationClip", true, null, "AnimationClip: ").objectReferenceValue) {

                    drawer.Field("animationClipOffenseType", true, null, "Type: ");

                    drawer.Field("offenseAudioClip", true, null, "AudioClip: ");
                }

                drawer.EndProperty();
                return true;
            }
        }

        [CustomPropertyDrawer(typeof(CooldownData))]
        public partial class CooldownDataDrawer : ComponentNUIPropertyDrawer
        {
            public override bool OnNUI(Rect position, SerializedProperty property, GUIContent label)
            {
                if (!base.OnNUI(position, property, label))
                    return false;

                drawer.FloatSlider("currentPercentage", 0, 1, "0%", "100%", true);

                drawer.Field("currentCooldown", false, "sec");

                drawer.EndProperty();
                return true;
            }
        }

#endif
    }
}