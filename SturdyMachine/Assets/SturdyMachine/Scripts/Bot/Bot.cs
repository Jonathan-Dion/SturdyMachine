using UnityEngine;

using SturdyMachine.Component;
using SturdyMachine.Equipment;

using SturdyMachine.Offense;
using SturdyMachine.Features.Fight;
using SturdyMachine.Features.HitConfirm;
using System;

#if UNITY_EDITOR
using NWH.NUI;
using UnityEditor;
#endif

namespace SturdyMachine.Bot
{
    /// <summary>
    /// Base class for alls Bots
    /// </summary>
    public abstract class Bot : SturdyComponent 
    {
        #region Attributes

        /// <summary>
        /// Current weapon for this Bot
        /// </summary>
        [SerializeField, Tooltip("Current weapon for this Bot")]
        protected Weapon _weapon;

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

        bool _isFullStanceCharge;

        #endregion

        #region Properties

        /// <summary>
        /// Return the current offense the Bot is executing
        /// </summary>
        public OffenseManager GetOffenseManager => _offenseManager;

        /// <summary>
        /// Return the current Animator for this Bot
        /// </summary>
        public Animator GetAnimator => _animator;

        /// <summary>
        /// Allows you to make all the necessary checks to see if the Bot can play the next Offense
        /// </summary>
        /// <returns>Returns if the Offense change can be done with the next</returns>
        bool GetIsPlayNextOffense(OffenseCancelConfig pOffenseCancelConfig, CooldownType pCurrentCooldownType) {

            if (_botType != BotType.SturdyBot)
                return false;

            if (_offenseManager.GetIsCooldownActivated(pCurrentCooldownType))
                return false;

            if (_offenseManager.GetIsNextOffenseAreStrikeType(_animator.GetCurrentAnimatorStateInfo(0).normalizedTime))
                return true;

            if (!_offenseManager.GetIsNeedApplyNextOffense())
                return false;

            if (_animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.95f)
                return true;

            return pOffenseCancelConfig.GetIsCancelCurrentOffense(_offenseManager.GetCurrentOffense, _offenseManager.GetNextOffense);
        }

        /// <summary>
        /// Return distance that matches the player's positioning when looking at this bot
        /// </summary>
        public Vector3 GetFocusRange => _focusRange;

        /// <summary>
        /// Returns the animationClip that the animator is currently playing for this bot
        /// </summary>
        public AnimationClip GetCurrentAnimationClipPlayed => _animator.GetCurrentAnimatorClipInfo(0)[0].clip;

        #endregion

        #region Method

        public override void OnAwake()
        {
            base.OnAwake();

            _animator = GetComponent<Animator>();

            if (_weapon)
                _weapon.OnAwake();
        }

        /// <summary>
        /// Manage the offense on each frame with Offense configuration on parameter
        /// </summary>
        /// <param name="pOffenseDirection">The Direction of the Next Desired Offense</param>
        /// <param name="pOffenseType">The Type of the Next Desired Offense</param>
        /// <param name="pOffenseCancelConfig">Class that contains all cancel restrictions</param>
        /// <param name="pCurrentCooldownType">Represents the bot's current cooldown type</param>
        /// <param name="pAnimationClipOffenseType">Represents the type of animationClip of the next offense to be checked with the current one of the bot</param>
        public virtual bool OnUpdate(OffenseDirection pOffenseDirection, OffenseType pOffenseType, OffenseCancelConfig pOffenseCancelConfig, CooldownType pCurrentCooldownType, AnimationClipOffenseType pAnimationClipOffenseType = AnimationClipOffenseType.Full) {

            if (!base.OnUpdate())
                return false;

            OffenseSetup(pOffenseDirection, pOffenseType, pOffenseCancelConfig, pCurrentCooldownType, pAnimationClipOffenseType);

            _weapon.OnUpdate();

            return true;
        }

        public virtual void OnCollisionEnter(Collision pCollision) 
        {
            //_weapon.OnCollisionEnter(pCollision);
        }

        public virtual void OnCollisionExit(Collision pCollision) 
        {
            //_weapon.OnCollisionExit(pCollision);
        }

        /// <summary>
        /// Manages the AnimationClip that the bot plays
        /// </summary>
        /// <param name="pOffenseDirection">The Direction of the Next Desired Offense</param>
        /// <param name="pOffenseType">The Type of the Next Desired Offense</param>
        /// <param name="pOffenseCancelConfig">Class that contains all cancel restrictions</param>
        /// <param name="pCurrentCooldownType">Represents the bot's current cooldown type</param>
        /// <param name="pAnimationClipOffenseType">Represents the type of animationClip of the next offense to be checked with the current one of the bot</param>
        void OffenseSetup(OffenseDirection pOffenseDirection, OffenseType pOffenseType, OffenseCancelConfig pOffenseCancelConfig, CooldownType pCurrentCooldownType, AnimationClipOffenseType pAnimationClipOffenseType) {

            CurrentOffenseAssignation();

            _offenseManager.AssignNextOffense(pOffenseType, pOffenseDirection);

            DamageSetup();

            if (!GetIsPlayNextOffense(pOffenseCancelConfig, pCurrentCooldownType))
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

            _offenseManager.GetCurrentOffense.StanceIntensityDamagae(_animator.GetCurrentAnimatorStateInfo(0).normalizedTime);
        }

        /// <summary>
        /// Allows management of the current Offense assignment
        /// </summary>
        void CurrentOffenseAssignation() {

            //If the Current Offense is already assigned correctly
            if (_offenseManager.GetIsCurrentOffenseAlreadyAssigned(_animator.GetCurrentAnimatorClipInfo(0)[0].clip))
                return;

            //Assigns the correct Offense based on the name of the animationClip in the bot's animator
            _offenseManager.AssignCurrentOffense(_animator.GetCurrentAnimatorClipInfo(0)[0].clip.name);
        }

        public override void OnEnabled()
        {
            base.OnEnabled();

            if (_weapon)
                _weapon.OnEnabled();
        }

        public override void OnDisabled()
        {
            base.OnDisabled();

            _offenseManager.OnDisable();

            if (_weapon)
                _weapon.OnDisabled();
        }

        #endregion
    }

#if UNITY_EDITOR

    [CustomEditor(typeof(Bot))]
    public class BotEditor : SturdyComponentEditor
    {
        Bot bot;

        public override bool OnInspectorNUI()
        {
            if (!base.OnInspectorNUI())
                return false;

            if (bot != (Bot)target)
                bot = (Bot)target;

            drawer.BeginSubsection("Configuration");

            drawer.BeginSubsection("Offense");

            drawer.Field("_offenseManager");

            drawer.EndSubsection();

            drawer.Field("_weapon");

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