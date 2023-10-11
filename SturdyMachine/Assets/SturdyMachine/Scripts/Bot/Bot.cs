using UnityEngine;

using SturdyMachine.Equipment;
using SturdyMachine.Offense;
using SturdyMachine.Utilities;
using Codice.Utils;
using Unity.Plastic.Newtonsoft.Json.Bson;

#if UNITY_EDITOR
using NWH.NUI;
using UnityEditor;
#endif

namespace SturdyMachine 
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

        #endregion

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
        public virtual void OnUpdate(OffenseDirection pOffenseDirection, OffenseType pOffenseType, bool pIsStanceActivated, Features.Fight.FightModule pFightModule) {

            if (!base.OnUpdate())
                return;

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

        bool GetIsStandardOffense(Features.Fight.FightModule pFightModule, bool pIsStanceActivated, bool pIsMonsterBot = false) 
        {
            /*//SturdyBot
            if (!pIsMonsterBot)
                return GetFightBlockingOffense(pFightModule.GetSturdyBotFightBlocking, pIsStanceActivated);

            //MonsterBot
            else if (pFightModule.GetMonsterBotFightBlocking.instanciateID != -1) 
            {
                if (pFightModule.GetMonsterBotFightBlocking.instanciateID == transform.GetInstanceID())
                    return GetFightBlockingOffense(pFightModule.GetMonsterBotFightBlocking, pIsStanceActivated, pIsMonsterBot);
            }*/

            return true;
        }

        /*bool GetFightBlockingOffense(Features.Fight.FightBlocking pFightBlocking, bool pIsStanceActivated, bool pIsMonsterBot = false) 
        {
            //Hitting
            if (pFightBlocking.isHitting)
            {
                if (_offenseManager.GetCurrentOffense())
                {
                    if (_offenseManager.GetCurrentOffense().GetOffenseType != OffenseType.DAMAGEHIT)
                        _offenseManager.SetAnimation(_animator, OffenseDirection.DEFAULT, OffenseType.DAMAGEHIT, pIsStanceActivated, pIsMonsterBot);

                    return false;
                }
            }

            //Blocking
            else if (pFightBlocking.isBlocking)
            {
                if (!_isAlreadyRepel)
                {
                    _isAlreadyRepel = true;

                    if (_offenseManager.GetCurrentOffense().GetOffenseType != OffenseType.REPEL)
                        _offenseManager.SetAnimation(_animator, OffenseDirection.DEFAULT, OffenseType.REPEL, pIsStanceActivated, true);
                }
                //else if (!_offenseManager.GetIsDefaultStance())
                //    _offenseManager.SetAnimation(_animator, OffenseDirection.STANCE, OffenseType.DEFAULT, pIsStanceActivated);

                return false;

            }

            return true;
        }*/
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

            drawer.Field("_isInitialized", false);
            drawer.Field("_isEnabled", false);

            drawer.Space();

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