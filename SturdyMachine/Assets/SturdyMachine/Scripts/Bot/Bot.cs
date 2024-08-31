using System;

using UnityEngine;

using SturdyMachine.Component;
using SturdyMachine.Offense;
using SturdyMachine.ParticlesState;
using SturdyMachine.Audio;
using SturdyMachine.Equipment;


#if UNITY_EDITOR
using NWH.NUI;
using UnityEditor;
#endif

namespace SturdyMachine.Bot
{
    /// <summary>
    /// All types of Bot
    /// </summary>
    public enum BotType { Default, SturdyBot, SkinnyBot }

    /// <summary>
    /// Base class for alls Bots
    /// </summary>
    public abstract class Bot : BaseComponent 
    {
        #region Attributes

        [SerializeField]
        protected BotType _botType;

        /// <summary>
        /// Animator for this Bot
        /// </summary>
        [SerializeField, Tooltip("Animator for this Bot")]
        protected Animator _animator;

        /// <summary>
        /// Manager of all Offenses for this Bot
        /// </summary>
        [SerializeField, Tooltip("Manager of all Offenses for this Bot")]
        protected OffenseManager _offenseManager;

        /// <summary>
        /// State that represents whether the current bot has been hit
        /// </summary>
        [SerializeField, Tooltip("State that represents whether the current bot has been hit")]
        protected bool _isHitting;

        /// <summary>
        /// State that represents whether the current bot has blocked
        /// </summary>
        [SerializeField, Tooltip("State that represents whether the current bot has blocked")]
        protected bool _isBlocking;

        /// <summary>
        /// Distance that matches the player's positioning when looking at this bot
        /// </summary>
        [SerializeField, Tooltip("Distance that matches the player's positioning when looking at this bot")]
        protected Vector3 _focusRange;

        [SerializeField]
        protected AudioSource _audioSource;

        [SerializeField]
        protected Weapon _weapon;

        AudioOffenseMaster _audioOffenseMaster;

        ParticlesState.ParticlesState _currentParticlesState;

        bool _isFullStanceCharge;

        #endregion

        #region Properties

        public BotType GetBotType => _botType;

        /// <summary>
        /// Return the current offense the Bot is executing
        /// </summary>
        public OffenseManager GetOffenseManager => _offenseManager;

        /// <summary>
        /// Return the current Animator for this Bot
        /// </summary>
        public Animator GetAnimator => _animator;

        /// <summary>
        /// Return distance that matches the player's positioning when looking at this bot
        /// </summary>
        public Vector3 GetFocusRange => _focusRange;

        #endregion

        #region Method

        public override void OnAwake()
        {
            base.OnAwake();

            _audioOffenseMaster = new AudioOffenseMaster(_audioSource);

            _animator = GetComponent<Animator>();

            if (_weapon)
                _weapon.OnAwake();
        }

        /// <summary>
        /// Manage the offense on each frame with Offense configuration on parameter
        /// </summary>
        /// <param name="pOffenseDirection">The Direction of the Next Desired Offense</param>
        /// <param name="pOffenseType">The Type of the Next Desired Offense</param>
        /// <param name="pCurrentCooldownType">Represents the bot's current cooldown type</param>
        /// <param name="pAnimationClipOffenseType">Represents the type of animationClip of the next offense to be checked with the current one of the bot</param>
        public virtual bool OnUpdate(OffenseDirection pOffenseDirection, OffenseType pOffenseType, CooldownType pCurrentCooldownType, bool pIsHitConfirmActivated, AnimationClipOffenseType pAnimationClipOffenseType = AnimationClipOffenseType.Full) {

            if (!base.OnUpdate())
                return false;

            if (!pIsHitConfirmActivated)
                OffenseSetup(pOffenseDirection, pOffenseType, pCurrentCooldownType, pAnimationClipOffenseType);

            //_currentParticlesState.OnUpdate(_offenseManager.GetCurrentOffense.GetOffenseType, _offenseManager.GetCurrentOffense.GetOffenseDirection, pIsHitConfirmActivated);

            _audioOffenseMaster.UpdateAudio(_offenseManager.GetCurrentOffense.GetOffenseType, _offenseManager.GetCurrentOffense.GetOffenseDirection, pAnimationClipOffenseType, _offenseManager.GetCurrentOffense.GetAudioOffenseDataClip(pAnimationClipOffenseType));

            if (_weapon)
                _weapon.OnUpdate(_offenseManager.GetCurrentOffense.GetOffenseType, _offenseManager.GetCurrentOffense.GetOffenseDirection, pAnimationClipOffenseType);

            return true;
        }

        /// <summary>
        /// Manages the AnimationClip that the bot plays
        /// </summary>
        /// <param name="pOffenseDirection">The Direction of the Next Desired Offense</param>
        /// <param name="pOffenseType">The Type of the Next Desired Offense</param>
        /// <param name="pOffenseCancelConfig">Class that contains all cancel restrictions</param>
        /// <param name="pCurrentCooldownType">Represents the bot's current cooldown type</param>
        /// <param name="pAnimationClipOffenseType">Represents the type of animationClip of the next offense to be checked with the current one of the bot</param>
        void OffenseSetup(OffenseDirection pOffenseDirection, OffenseType pOffenseType, CooldownType pCurrentCooldownType, AnimationClipOffenseType pAnimationClipOffenseType) {

            //If the Current Offense is already assigned correctly. Assigns the correct Offense based on the name of the animationClip in the bot's animator
            if (!_offenseManager.GetIsCurrentOffenseAlreadyAssigned(_animator.GetCurrentAnimatorClipInfo(0)[0].clip))
                _offenseManager.AssignCurrentOffense(_animator.GetCurrentAnimatorClipInfo(0)[0].clip.name);

            _offenseManager.AssignNextOffense(pOffenseType, pOffenseDirection);

            _offenseManager.GetCurrentOffense.StanceIntensityDamagae(_animator.GetCurrentAnimatorStateInfo(0).normalizedTime);

            if (_botType != BotType.SturdyBot)
                return;

            if (_offenseManager.GetIsCooldownActivated(pCurrentCooldownType))
                return;

            if (!_offenseManager.GetIsNeedApplyNextOffense())
                return;

            if (_animator.GetCurrentAnimatorClipInfo(0)[0].clip == _offenseManager.GetNextOffense.GetAnimationClip(pAnimationClipOffenseType)) {

                _animator.Play(_animator.GetCurrentAnimatorClipInfo(0)[0].clip.name, -1, 0f);

                return;
            }

            if (_offenseManager.GetIsNextOffenseAreStrikeType(_animator.GetCurrentAnimatorStateInfo(0).normalizedTime))
                pAnimationClipOffenseType = AnimationClipOffenseType.Full;

            _animator.Play(_offenseManager.GetNextOffense.GetAnimationClip(pAnimationClipOffenseType).name);
        }

        void DamageSetup() {

            if (!_offenseManager.GetCurrentOffense)
                return;

            if (_offenseManager.GetCurrentOffense.GetOffenseDirection != OffenseDirection.STANCE)
                return;

            if (_offenseManager.GetCurrentOffense.GetOffenseType == OffenseType.DEFAULT)
                return;

            if (!_offenseManager.GetNextOffense)
                return;

            if (_offenseManager.GetNextOffense != _offenseManager.GetCurrentOffense)
                return;

            
        }

        public override void OnEnabled()
        {
            base.OnEnabled();

            _weapon.OnEnabled();
        }

        public override void OnDisabled()
        {
            base.OnDisabled();

            _offenseManager.OnDisable();

            _weapon.OnDisabled();
        }

        #endregion
    }

#if UNITY_EDITOR

    [CustomEditor(typeof(Bot))]
    public class BotEditor : BaseComponentEditor
    {
        Bot bot;

        public override bool OnInspectorNUI()
        {
            if (!base.OnInspectorNUI())
                return false;

            if (bot != (Bot)target)
                bot = (Bot)target;

            if (drawer.Field("_botType").enumValueIndex == 0)
                return false;

            drawer.Field("_audioSource");
            drawer.Field("_weapon");

            drawer.BeginSubsection("Configuration");

            drawer.BeginSubsection("Offense");

            drawer.Field("_offenseManager");

            drawer.EndSubsection();

            drawer.Field("_focusRange");

            drawer.EndSubsection();

            drawer.EndEditor(this);
            return true;
        }

        public override bool UseDefaultMargins()
        {
            return false;
        }
    }

#endif

}