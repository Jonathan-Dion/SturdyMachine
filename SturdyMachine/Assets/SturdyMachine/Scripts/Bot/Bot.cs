using UnityEngine;

using SturdyMachine.Equipment;
using SturdyMachine.Offense.Manager;

#if UNITY_EDITOR
using NWH.NUI;
using UnityEditor;
#endif

namespace SturdyMachine 
{
    public abstract class Bot : MonoBehaviour 
    {
        [SerializeField]
        protected Weapon _fusionBlade;

        [SerializeField]
        protected Animator _animator;

        [SerializeField]
        protected OffenseManager _offenseManager;

        [SerializeField]
        protected bool _isHitting, _isBlocking;

        [SerializeField]
        bool _isInitialized, _isEnabled;

        protected Offense.Offense _currentOffense;

        bool _isAlreadyRepel;

        public bool GetIsActivated => _isInitialized && _isEnabled;
        public bool GetIsInitialized => _isInitialized;

        public OffenseManager GetOffenseManager => _offenseManager;

        public Animator GetAnimator => _animator;

        public virtual void Initialize() 
        {
            _isInitialized = true;
        }

        public virtual void Awake() 
        {
            _animator = GetComponent<Animator>();
        }

        public virtual void UpdateRemote(OffenseDirection pOffenseDirection, OffenseType pOffenseType, bool pIsStanceActivated, Features.Fight.FightModule pFightModule, bool pIsSturdyBot = true) 
        {
            if (!_isInitialized)
                return;

            if (_offenseManager != null) 
            {
                if (GetIsStandardOffense(pFightModule, pIsStanceActivated, pIsSturdyBot)) 
                {
                    if (_isAlreadyRepel)
                        _isAlreadyRepel = false;

                    _offenseManager.SetAnimation(_animator, pOffenseDirection, pOffenseType, pIsStanceActivated);
                }
            }

            _fusionBlade.Update();
        }

        public virtual void LateUpdateRemote(bool pIsMonsterBot = true)
        {
            if (_offenseManager.GetNextOffense())
            {
                if (_offenseManager.GetNextOffense().GetOffenseType != OffenseType.DEFAULT)
                    _fusionBlade.LateUpdateRemote();

                if (pIsMonsterBot)
                {
                    if (_offenseManager.GetCurrentOffense().GetOffenseType == OffenseType.DEFAULT)
                        _fusionBlade.LateUpdateRemote(false);
                }
                else if (_animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f)
                    _fusionBlade.LateUpdateRemote(false);
            }
            else
                _fusionBlade.LateUpdateRemote(_offenseManager.GetCurrentOffense().GetOffenseType == OffenseType.DEFAULT);
        }

        public virtual void OnCollisionEnter(Collision pCollision) 
        {
            _fusionBlade.OnCollisionEnter(pCollision);
        }

        public virtual void OnCollisionExit(Collision pCollision) 
        {
            _fusionBlade.OnCollisionExit(pCollision);
        }

        public virtual void Enable()
        {
            if (!_isInitialized)
            {
                if (Application.isPlaying)
                    Initialize();
            }

            _isEnabled = true;
        }

        public virtual void Disable()
        {
            _isEnabled = false;
        }

        public virtual void ToogleState()
        {
            if (_isEnabled)
                Disable();
            else
                Enable();
        }

        public virtual void SetDefault() { }

        bool GetIsStandardOffense(Features.Fight.FightModule pFightModule, bool pIsStanceActivated, bool pIsSturdyBot = true) 
        {
            //SturdyBot
            if (pIsSturdyBot)
                return GetFightBlockingOffense(pFightModule.GetSturdyBotFightBlocking, pIsStanceActivated);

            //MonsterBot
            else if (pFightModule.GetMonsterBotFightBlocking.instanciateID != -1) 
            {
                if (pFightModule.GetMonsterBotFightBlocking.instanciateID == transform.GetInstanceID())
                    return GetFightBlockingOffense(pFightModule.GetMonsterBotFightBlocking, pIsStanceActivated);
            }

            return true;
        }

        bool GetFightBlockingOffense(Features.Fight.FightBlocking pFightBlocking, bool pIsStanceActivated) 
        {
            //Hitting
            if (pFightBlocking.isHitting)
            {
                if (_offenseManager.GetCurrentOffense().GetOffenseType != OffenseType.DAMAGEHIT)
                    _offenseManager.SetAnimation(_animator, OffenseDirection.DEFAULT, OffenseType.DAMAGEHIT, pIsStanceActivated, true);

                return false;
            }

            //Blocking
            else if (pFightBlocking.isBlocking)
            {
                if (!_isAlreadyRepel)
                {
                    _isAlreadyRepel = true;

                    if (_offenseManager.GetCurrentOffense().GetOffenseType != OffenseType.REPEL)
                        _offenseManager.SetAnimation(_animator, OffenseDirection.DEFAULT, OffenseType.REPEL, pIsStanceActivated, true);

                    return false;
                }
                //else if (!_offenseManager.GetIsDefaultStance())
                //    _offenseManager.SetAnimation(_animator, OffenseDirection.STANCE, OffenseType.DEFAULT, pIsStanceActivated);

            }

            return true;
        }
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