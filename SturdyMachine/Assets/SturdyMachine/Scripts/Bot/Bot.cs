using UnityEngine;

using SturdyMachine.Component;
using SturdyMachine.Equipment;

using SturdyMachine.Offense;
using SturdyMachine.Features.Fight;

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
        protected Weapon _fusionBlade;

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
        /// If this Bot is not a Player
        /// </summary>
        [SerializeField, Tooltip("If this Bot is not a Player")]
        protected bool _isEnemyBot;

        /// <summary>
        /// The current offense the Bot is executing
        /// </summary>
        protected Offense.Offense _currentOffense;

        /// <summary>
        /// If the Bot is already doing a Repel
        /// </summary>
        bool _isAlreadyRepel;

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
        /// Returns the current frame as the clip that is currently playing
        /// </summary>
        public float GetCurrentFrame => _animator.GetCurrentAnimatorStateInfo(0).normalizedTime * _currentOffense.GetClipFrames / 1;

        bool GetIsMonsterBotStandardOffense(OffenseFightBlocking pMonsterOffenseFightBlocking)
        {

            if (pMonsterOffenseFightBlocking.instanciateID == -1)
                return true;

            if (pMonsterOffenseFightBlocking.instanciateID != transform.GetInstanceID())
                return true;

            return false;
        }

        bool GetIsStandardOffense(FightModule pFightModule, bool pIsStanceActivated, bool pIsMonsterBot = false)
        {
            //SturdyBot
            if (!pIsMonsterBot)
                return GetFightBlockingOffense(pFightModule.GetOffenseSturdyBotBlocking, pIsStanceActivated);

            //MonsterBot
            if (!GetIsMonsterBotStandardOffense(pFightModule.GetOffenseMonsterBotBlocking))
                return GetFightBlockingOffense(pFightModule.GetOffenseMonsterBotBlocking, pIsStanceActivated, pIsMonsterBot);

            return true;
        }

        bool GetIsHitting(bool pIsHitting, bool pIsStanceActivated, bool pIsEnnemyBot = false)
        {
            if (!pIsHitting)
                return false;

            if (!_offenseManager.GetCurrentOffense())
                return false;

            //_offenseManager.SetAnimation(_animator, OffenseDirection.DEFAULT, OffenseType.DAMAGEHIT, pIsStanceActivated, pIsEnnemyBot);

            return true;
        }

        bool GetIsBlocking(bool pIsBlocking, bool pIsStanceActivated)
        {

            if (!pIsBlocking)
                return true;

            if (!_isAlreadyRepel)
            {
                _isAlreadyRepel = true;

                if (_offenseManager.GetCurrentOffense().GetOffenseType != OffenseType.REPEL)
                    _offenseManager.SetAnimation(_animator, OffenseDirection.DEFAULT, OffenseType.REPEL, pIsStanceActivated, true);
            }

            else if (!_offenseManager.GetIsStanceOffense)
                _offenseManager.SetAnimation(_animator, OffenseDirection.STANCE, OffenseType.DEFAULT, pIsStanceActivated);

            return true;
        }

        bool GetFightBlockingOffense(OffenseFightBlocking pOffenseFightBlocking, bool pIsStanceActivated, bool pIsEnnemyBot = false)
        {
            base.ToogleState(ref pOffenseFightBlocking.isHitting);

            //Hitting
            if (!GetIsHitting(pOffenseFightBlocking.isHitting, pIsStanceActivated, pIsEnnemyBot)) {

                //Blocking
                return GetIsBlocking(pOffenseFightBlocking.isBlocking, pIsStanceActivated);
            }

            return false;
        }

        #endregion

        #region Method

        public override void OnAwake()
        {
            base.OnAwake();

            _animator = GetComponent<Animator>();
        }

        /// <summary>
        /// Manage the offense on each frame with Offense configuration on parameter
        /// </summary>
        /// <param name="pOffenseDirection">The direction of offense you want to play</param>
        /// <param name="pOffenseType">The type of offense you want to play</param>
        /// <param name="pIsStanceActivated">If it's a Stance type offense</param>
        /// <param name="pFightModule">The module that allows you to manage combat</param>
        public virtual bool OnUpdate(OffenseDirection pOffenseDirection, OffenseType pOffenseType, bool pIsStanceActivated, FightModule pFightModule) {

            if (!base.OnUpdate())
                return false;

            if (_offenseManager != null)
            {
                if (GetIsStandardOffense(pFightModule, pIsStanceActivated, _isEnemyBot))
                {
                    if (_isAlreadyRepel)
                        _isAlreadyRepel = false;

                    
                    _offenseManager.SetAnimation(_animator, pOffenseDirection, pOffenseType, pIsStanceActivated, _isEnemyBot);
                }
            }

            _fusionBlade.OnUpdate();

            return true;
        }

        public override bool OnLateUpdate()
        {
            if (!base.OnLateUpdate())
                return false;

            //Check if the next offense is available
            if (!_offenseManager.GetNextOffense())
            {
                //Check if the current offense is available
                if (_offenseManager.GetCurrentOffense())
                {
                    //Check if the current offense type is not Default
                    if (_offenseManager.GetCurrentOffense().GetOffenseType == OffenseType.DEFAULT)
                        _fusionBlade.LateUpdateRemote();
                }

                return true;
            }

            //If the current bot is the player
            else if (!_isEnemyBot)
            {
                //Check if the next offense equal a Default offense type
                if (_offenseManager.GetNextOffense().GetOffenseType == OffenseType.DEFAULT)
                {
                    //Check if the animation that the bot is currently playing has finished
                    if (_animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f)
                        _fusionBlade.LateUpdateRemote(false);
                }
            }
            
            else
            {
                //Check if the nest offense equal to Default offense type
                if (_offenseManager.GetNextOffense().GetOffenseType == OffenseType.DEFAULT)
                {
                    _fusionBlade.LateUpdateRemote(false);

                    return true;
                }
                else
                    _fusionBlade.LateUpdateRemote(_offenseManager.GetNextOffense().GetOffenseType == OffenseType.DEFAULT ? false : true);
            }

            return true;
        }

        public virtual void OnCollisionEnter(Collision pCollision) 
        {
            _fusionBlade.OnCollisionEnter(pCollision);
        }

        public virtual void OnCollisionExit(Collision pCollision) 
        {
            _fusionBlade.OnCollisionExit(pCollision);
        }

        #endregion
    }

#if UNITY_EDITOR

    [CustomEditor(typeof(Bot))]
    public class BotEditor : NUIEditor
    {
        public override bool OnInspectorNUI()
        {
            if (!base.OnInspectorNUI())
                return false;

            drawer.BeginSubsection("Debug Value");

            drawer.Field("_animator", false);

            drawer.EndSubsection();

            drawer.BeginSubsection("Configuration");

            drawer.Field("_offenseManager");
            drawer.Field("_fusionBlade");

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