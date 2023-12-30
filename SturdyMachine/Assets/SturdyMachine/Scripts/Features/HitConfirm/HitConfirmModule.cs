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
        HitConfirmOffense _currentHitConfirmOffense;

        HitConfirmSequenceData _currentHitConfirmSequenceData;

        FocusModule _focusModule;
        FightModule _fightModule;

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
            if (_currentHitConfirmOffense.GetCurrentHitConfirmType(_fightModule.GetOffenseFightBlockingAttackerBot().isBlocking) == HitConfirmType.Slow)
                return _currentHitConfirmOffense.GetCurrentHitConfirmSubSequenceData(_fightModule.GetOffenseFightBlockingAttackerBot().isBlocking).slowHitConfirmSpeed;

            //Stop
            return 0;
        }

        bool GetIsNextHitConfirmSubSequence(ref HitConfirmSequenceData pHitConfirmSequenceData) {

            if (pHitConfirmSequenceData.currentHitConfirmSubSequenceIndex != pHitConfirmSequenceData.hitConfirmSubSequenceData.Length - 1) {

                ++pHitConfirmSequenceData.currentHitConfirmSubSequenceIndex;

                return true;
            }

            pHitConfirmSequenceData.currentHitConfirmSubSequenceIndex = 0;

            return false;
        }

        bool GetIsHitConfirmSubSequence(OffenseManager pOffenseManager, Animator pAnimator, OffenseFightBlocking pOffenseFightBlocking, bool pIsPlayer) {

            if (!GetIsNormalizedTimeOffenseBlocking(pOffenseManager, pAnimator, pOffenseFightBlocking, pIsPlayer))
                return false;

            if (!_isAlreadyPlayed)
            {

                if (!_audioSource.isPlaying)
                    _audioSource.Play();

                _isAlreadyPlayed = true;
            }

            if (pAnimator.speed == GetHitConfirmSpeed(_currentHitConfirmSequenceData))
                return true;

            pAnimator.speed = GetHitConfirmSpeed(_currentHitConfirmSequenceData);

            return true;
        }

        bool GetIsNormalizedTimeOffenseBlocking(OffenseManager pOffenseManager, Animator pAnimator, OffenseFightBlocking pOffenseFightBlocking, bool pIsPlayer)
        {
            if (pOffenseFightBlocking.isHitting){

                if (pOffenseManager.GetCurrentOffense()){

                    if (pOffenseManager.GetCurrentOffense().GetOffenseType != OffenseType.DAMAGEHIT){

                        //pOffenseManager.SetAnimation(pAnimator, OffenseDirection.DEFAULT, OffenseType.DAMAGEHIT, pOffenseManager.GetIsStance(), !pIsPlayer);

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

                        if (pAnimator.GetCurrentAnimatorClipInfo(0)[0].clip != pOffenseManager.GetCurrentOffense().GetAnimationClip(false))
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

        public override void OnAwake(SturdyComponent pSturdyComponent, BotData[] pEnnemyBotData)
        {
            base.OnAwake(pSturdyComponent, pEnnemyBotData);
        
            _focusModule = _sturdyComponent.GetComponent<FocusModuleWrapper>().GetFocusModule;

            _fightModule = _sturdyComponent.GetComponent<FightModuleWrapper>().GetFightModule;
        }

        public override bool OnUpdate()
        {
            if (!base.OnUpdate())
                return false;

            HitConfirmSequenceDataInit();

            HitConfirmSequence();

            return true;
        }

        void HitConfirmSequenceDataInit() {

            if (!_fightModule.GetIsHitConfirm()) {

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

                if (_currentHitConfirmOffense != _fightModule.GetDefenderHitConfirmOffense())
                    _currentHitConfirmOffense = _fightModule.GetDefenderHitConfirmOffense();

                if (!_currentHitConfirmSequenceData.Equals(_currentHitConfirmOffense.GetHitConfirmSequenceData(_fightModule.GetOffenseFightBlockingAttackerBot().isBlocking)))
                    _currentHitConfirmSequenceData = _currentHitConfirmOffense.GetHitConfirmSequenceData(_fightModule.GetOffenseFightBlockingAttackerBot().isBlocking);

                //Sturdy
                if (GetIsHitConfirmSubSequence(_sturdyBotData.offenseManager, _sturdyBotData.animator, _fightModule.GetOffenseSturdyBotBlocking, true)) {

                    //EnnemyBot
                    if (GetIsHitConfirmSubSequence(_focusModule.GetCurrentEnnemyBotData.offenseManager, _focusModule.GetCurrentEnnemyBotData.animator, _fightModule.GetOffenseMonsterBotBlocking, false)) {

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

            if (_currentFrame >= _currentHitConfirmOffense.GetCurrentHitConfirmSubSequenceData(_fightModule.GetOffenseFightBlockingAttackerBot().isBlocking).hitConfirmDelay) {

                _currentFrame = 0;

                if (!GetIsNextHitConfirmSubSequence(ref _currentHitConfirmSequenceData)) {

                    _isHitConfirmInit = false;

                    //Sturdy
                    if (_sturdyBotData.animator.speed != 1)
                        _sturdyBotData.animator.speed = 1;

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