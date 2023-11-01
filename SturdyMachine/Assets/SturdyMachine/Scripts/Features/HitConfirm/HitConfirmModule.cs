using System;

using UnityEngine;

using SturdyMachine.Features.Focus;
using SturdyMachine.Features.Fight;

using SturdyMachine.Component;

using SturdyMachine.Offense;

using NWH.VehiclePhysics2;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace SturdyMachine.Features.HitConfirm {

    /// <summary>
    /// Identify the type of HitConfirm
    /// </summary>
    public enum HitConfirmType { None, Normal, Slow, Stop }

    /// <summary>
    /// Allows configuration of HitConfirm data depending on its type
    /// </summary>
    [Serializable, Tooltip("Allows configuration of HitConfirm data depending on its type")]
    public struct HitConfirmSubSequenceData {

        /// <summary>
        /// Type of HitConfirm
        /// </summary>
        public HitConfirmType hitConfirmType;

        /// <summary>
        /// Offense Animation Delay
        /// </summary>
        public float hitConfirmDelay;

        /// <summary>
        /// Animation speed when HitConfirm is in Slow mode
        /// </summary>
        public float slowHitConfirmSpeed;
    }

    /// <summary>
    /// Allows you to record all subsequences of hitConfirm
    /// </summary>
    [Serializable, Tooltip("Allows you to record all subsequences of hitConfirm")]
    public struct HitConfirmSequenceData {

        /// <summary>
        /// Array containing all subsequences of hitConfirm
        /// </summary>
        public HitConfirmSubSequenceData[] hitConfirmSubSequenceData;

        /// <summary>
        /// Index of the HitConfirm subsequence that is selected
        /// </summary>
        public int currentHitConfirmSubSequenceIndex;
    }

    /// <summary>
    /// Allows you to record the sequence when the bot parries and when it gets hit
    /// </summary>
    [Serializable, Tooltip("Allows you to record the sequence when the bot parries and when it gets hit")]
    public struct HitConfirmData{

        /// <summary>
        /// Subsequences when the bot successfully blocks
        /// </summary>
        public HitConfirmSequenceData parryHitConfirmSequenceData;

        /// <summary>
        /// Subsequences when the bot is hit
        /// </summary>
        public HitConfirmSequenceData hittingHitConfirmSequenceData;
    }

    [Serializable]
    public partial class HitConfirmModule : FeatureModule {

        #region Attribut

        [SerializeField]
        AudioSource _audioSource;

        [SerializeField]
        AudioClip _audioClip;

        [SerializeField]
        float _basePitch;

        [SerializeField]
        float _pitchMultiplicator;

        [SerializeField]
        HitConfirmSequenceData _currentHitConfirmSequenceData;

        FocusModule _focusModule;

        bool _isHitConfirmInit;

        int _currentFrame;

        bool _ifHitConfirmFinished;

        bool _isAlreadyPlayed;

        #endregion

        #region Get

        public override FeatureModuleCategory GetFeatureModuleCategory()
        {
            return FeatureModuleCategory.HitConfirm;
        }

        float GetHitConfirmSpeed(HitConfirmSequenceData pHitConfirmSequenceData) {

            //Slow
            if (GetHitConfirmType(pHitConfirmSequenceData) == HitConfirmType.Slow)
                return GetCurrentHitConfirmSubSequenceData(pHitConfirmSequenceData).slowHitConfirmSpeed;

            //Stop
            return 0;
        }

        bool GetIsNextHitConfirmSubSequence() {

            if (_currentHitConfirmSequenceData.currentHitConfirmSubSequenceIndex != _currentHitConfirmSequenceData.hitConfirmSubSequenceData.Length - 1) {

                ++_currentHitConfirmSequenceData.currentHitConfirmSubSequenceIndex;

                return true;
            }

            _currentHitConfirmSequenceData.currentHitConfirmSubSequenceIndex = 0;

            return false;
        }

        bool GetIsHitConfirmSubSequence(Bot pBot, OffenseFightBlocking pOffenseFightBlocking, bool pIsPlayer) {

            if (!GetIsNormalizedTimeOffenseBlocking(pBot, pOffenseFightBlocking, pIsPlayer))
                return false;

            if (!_isAlreadyPlayed)
            {

                if (!_audioSource.isPlaying)
                    _audioSource.Play();

                _isAlreadyPlayed = true;
            }

            if (pBot.GetAnimator.speed == GetHitConfirmSpeed(_currentHitConfirmSequenceData))
                return true;

            pBot.GetAnimator.speed = GetHitConfirmSpeed(_currentHitConfirmSequenceData);

            return true;
        }

        bool GetIsNormalizedTimeOffenseBlocking(OffenseManager pOffenseManager, Animator pAnimator, OffenseFightBlocking pOffenseFightBlocking, bool pIsPlayer)
        {
            if (pOffenseFightBlocking.isHitting){

                if (pOffenseManager.GetCurrentOffense()){

                    if (pOffenseManager.GetCurrentOffense().GetOffenseType != OffenseType.DAMAGEHIT){

                        pOffenseManager.SetAnimation(pAnimator, OffenseDirection.DEFAULT, OffenseType.DAMAGEHIT, pOffenseManager.GetIsStance(), !pIsPlayer);

                        return false;
                    }
                }

                if (pAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime < 0.75f)
                    return false;

                return true;
            }

            if (pOffenseFightBlocking.isBlocking) {

                if (pOffenseManager.GetCurrentOffense()) {

                    if (pOffenseManager.GetCurrentOffense().GetOffenseType == OffenseType.DEFLECTION) {

                        if (pAnimator.GetCurrentAnimatorClipInfo(0)[0].clip != pOffenseManager.GetCurrentOffense().GetClip)
                            return false;

                        if (pAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime < 0.75f)
                            return false;
                    }
                }

                return true;
            }

            return true;

        }

        #endregion

        #region Method

        public override void Initialize()
        {
            base.Initialize();

            _audioSource.clip = _audioClip;

            _audioSource.pitch = _basePitch;
        }

        public override void OnAwake(SturdyComponent pSturdyComponent, GameObject[] pEnnemyBot, Vector3[] pEnnemyBotFocusRange, OffenseManager[] pEnnemyBotOffenseManager, Animator[] pEnnemyBotAnimator, float[] pEnnemyBotBlockingChance)
        {
            base.OnAwake(pSturdyComponent, pEnnemyBot, pEnnemyBotFocusRange, pEnnemyBotOffenseManager, pEnnemyBotAnimator, pEnnemyBotBlockingChance);
        
            _focusModule = _sturdyComponent.GetComponent<FocusModuleWrapper>().GetFocusModule;
        }

        public override bool OnUpdate()
        {
            if (!base.OnUpdate())
                return false;

            HitConfirmSequenceDataInit(_main.GetFeatureManager);

            HitConfirmSequence();

            return true;
        }

        void HitConfirmSequenceDataInit(FeatureManager pFeatureManager) {

            if (!pFeatureManager.GetFeatureModule<FightModule>().GetIsHitConfirm()) {

                if (_ifHitConfirmFinished)
                    _ifHitConfirmFinished = false;

                if (_isAlreadyPlayed)
                    _isAlreadyPlayed = false;

                return;
            }

            if (!_isHitConfirmInit)
            {
                if (_ifHitConfirmFinished)
                    return;

                if (!_currentHitConfirmSequenceData.Equals(GetHitConfirmSequenceData(pFeatureManager.GetFeatureModule<FightModule>())))
                    _currentHitConfirmSequenceData = GetHitConfirmSequenceData(pFeatureManager.GetFeatureModule<FightModule>());

                //Sturdy
                if (GetIsHitConfirmSubSequence(_main.GetSturdyBot, pFeatureManager.GetFeatureModule<FightModule>().GetOffenseSturdyBotBlocking, true)) {

                    //EnnemyBot
                    if (GetIsHitConfirmSubSequence(pFeatureManager.GetFeatureModule<FocusModule>().GetCurrentMonsterBot, pFeatureManager.GetFeatureModule<FightModule>().GetOffenseMonsterBotBlocking, false)) {

                        if (!_isHitConfirmInit)
                            _isHitConfirmInit = true;
                    }
                }
            }

        }

        void HitConfirmSequence() {

            if (!_isHitConfirmInit)
                return;

            if (_ifHitConfirmFinished)
                return;

            ++_currentFrame;

            if (_currentFrame >= GetCurrentHitConfirmSubSequenceData(_currentHitConfirmSequenceData).hitConfirmDelay) {

                _currentFrame = 0;

                if (!GetIsNextHitConfirmSubSequence()) {

                    _isHitConfirmInit = false;

                    //Sturdy
                    if (_sturdyBotAnimator.speed != 1)
                        _sturdyBotAnimator.speed = 1;

                    //EnnemyBot
                    if (_focusModule.GetCurrentEnnemyBotData.animator.speed != 1)
                        _focusModule.GetCurrentEnnemyBotData.animator.speed = 1;

                    _ifHitConfirmFinished = true;

                    return;
                }
            }

        }

        #endregion
    }

#if UNITY_EDITOR

    [CustomPropertyDrawer(typeof(HitConfirmModule))]
    public partial class HitConfirmModuleDrawer : FeatureModuleDrawer
    {
        public override bool OnNUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (!base.OnNUI(position, property, label))
                return false;

            drawer.Field("_pitchMultiplicator", false);

            if (drawer.Field("_audioSource").objectReferenceValue) {

                if (drawer.Field("_audioClip").objectReferenceValue)
                    drawer.Field("_basePitch");
            }

            drawer.Property("_hitConfirmSequenceData");

            drawer.EndProperty();
            return true;
        }
    }

    [CustomPropertyDrawer(typeof(HitConfirmSubSequenceData))]
    public partial class HitConfirmSubSequenceDataDrawer : ComponentNUIPropertyDrawer
    {
        public override bool OnNUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (!base.OnNUI(position, property, label))
                return false;

            int hitConfirmTypeIndex = drawer.Field("hitConfirmType").enumValueIndex;

            if (hitConfirmTypeIndex != 0) {

                drawer.Field("hitConfirmDelay", true, "frames", "Delay: ");

                //Slow
                if (hitConfirmTypeIndex == 2)
                    drawer.Field("slowHitConfirmSpeed", true, null, "Slow speed: ");
            }

            drawer.EndProperty();
            return true;
        }
    }

    [CustomPropertyDrawer(typeof(HitConfirmSequenceData))]
    public partial class HitConfirmSequenceDataDrawer : ComponentNUIPropertyDrawer
    {
        public override bool OnNUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (!base.OnNUI(position, property, label))
                return false;

            drawer.Field("currentHitConfirmSubSequenceIndex", false, null, "SubSequence index: ");

            drawer.ReorderableList("hitConfirmSubSequenceData");

            drawer.EndProperty();
            return true;
        }
    }

    [CustomPropertyDrawer(typeof(HitConfirmData))]
    public partial class HitConfirmDataDrawer : ComponentNUIPropertyDrawer
    {
        public override bool OnNUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (!base.OnNUI(position, property, label))
                return false;

            drawer.Property("parryHitConfirmSequenceData");

            drawer.Property("hittingHitConfirmSequenceData");

            drawer.EndProperty();
            return true;
        }
    }

#endif
}