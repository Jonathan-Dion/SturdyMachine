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

        public virtual void UpdateRemote(OffenseDirection pOffenseDirection, OffenseType pOffenseType, bool pIsStance) 
        {
            if (!_isInitialized)
                return;

            if (_offenseManager != null)
                _offenseManager.SetAnimation(_animator, pOffenseDirection, pOffenseType, pIsStance);

            _fusionBlade.Update();
        }

        public virtual void LateUpdateRemote(OffenseDirection pOffenseDirection) 
        {
            _fusionBlade.LateUpdateRemote(_offenseManager.GetCurrentOffenseDirection);
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