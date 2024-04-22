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
        #region Attribut

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

        #region Get

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

            if (!_offenseManager.GetIsApplyNextOffense())
                return false;

            if (_animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.95f)
                return true;

            return pOffenseCancelConfig.GetIsCancelCurrentOffense(_offenseManager.GetCurrentOffense(), _offenseManager.GetNextOffense());
        }

        /// <summary>
        /// Return distance that matches the player's positioning when looking at this bot
        /// </summary>
        public Vector3 GetFocusRange => _focusRange;

        #endregion

        #region Method

        public override void Initialize()
        {
            base.Initialize();

            _offenseManager = Instantiate(_offenseManager);
        }

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
        /// <param name="pOffenseDirection">The direction of offense you want to play</param>
        /// <param name="pOffenseType">The type of offense you want to play</param>
        /// <param name="pIsStanceActivated">If it's a Stance type offense</param>
        /// <param name="pFightModule">The module that allows you to manage combat</param>
        public virtual bool OnUpdate(OffenseDirection pOffenseDirection, OffenseType pOffenseType, OffenseCancelConfig pOffenseCancelConfig, CooldownType pCurrentCooldownType, bool pIsKeyPoseOut = false) {

            if (!base.OnUpdate())
                return false;

            OffenseSetup(pOffenseDirection, pOffenseType, pOffenseCancelConfig, pCurrentCooldownType, pIsKeyPoseOut);

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
        /// <param name="pIsKeyPoseOut">If to play the full animation or the one for HitConfirm</param>
        void OffenseSetup(OffenseDirection pOffenseDirection, OffenseType pOffenseType, OffenseCancelConfig pOffenseCancelConfig, CooldownType pCurrentCooldownType, bool pIsKeyPoseOut) {

            CurrentOffenseSetup();

            NextOffenseSetup(pOffenseDirection, pOffenseType);

            DamageSetup();

            if (!GetIsPlayNextOffense(pOffenseCancelConfig, pCurrentCooldownType))
                return;

            if (_animator.GetCurrentAnimatorClipInfo(0)[0].clip == _offenseManager.GetNextOffense().GetAnimationClip(pIsKeyPoseOut)) {

                _animator.Play(_animator.GetCurrentAnimatorClipInfo(0)[0].clip.name, -1, 0f);

                return;
            }

            _animator.Play(_offenseManager.GetNextOffense().GetAnimationClip(pIsKeyPoseOut).name);
        }

        void DamageSetup() {

            if (!_offenseManager.GetCurrentOffense())
                return;

            if (_offenseManager.GetCurrentOffense().GetOffenseDirection != OffenseDirection.STANCE)
                return;

            if (_offenseManager.GetCurrentOffense().GetOffenseType == OffenseType.DEFAULT)
                return;

            if (!_offenseManager.GetNextOffense())
                return;

            if (_offenseManager.GetNextOffense() != _offenseManager.GetCurrentOffense())
                return;

            _offenseManager.GetCurrentOffense().StanceIntensityDamagae(_animator.GetCurrentAnimatorStateInfo(0).normalizedTime);
        }

        /// <summary>
        /// Allows management of the current Offense assignment
        /// </summary>
        void CurrentOffenseSetup() {

            //If the Current Offense is already assigned correctly
            if (_offenseManager.GetCurrentOffenseAssigned(_animator))
                return;

            //Assigns the correct Offense based on the name of the animationClip in the bot's animator
            _offenseManager.CurrentOffenseClipNameSetup(_animator.GetCurrentAnimatorClipInfo(0)[0].clip.name);
        }

        /// <summary>
        /// Manages NextOffense assignment based on type and direction
        /// </summary>
        /// <param name="pOffenseDirection">The Direction of the Next Desired Offense</param>
        /// <param name="pOffenseType">The Type of the Next Desired Offense</param>
        void NextOffenseSetup(OffenseDirection pOffenseDirection, OffenseType pOffenseType) {

            _offenseManager.NextOffenseSetup(pOffenseType, pOffenseDirection);

            if (_offenseManager.GetNextOffenseAssigned)
                return;
        
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