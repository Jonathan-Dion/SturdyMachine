using System;
using UnityEngine;

#if UNITY_EDITOR
using NWH.NUI;
using UnityEditor;
#endif

namespace SturdyMachine.Equipment 
{
    [RequireComponent(typeof(MeshRenderer))]
    [RequireComponent(typeof(BoxCollider))]
    [RequireComponent(typeof(Rigidbody))]
    public abstract class Equipment : MonoBehaviour
    {
        [SerializeField]
        protected MeshRenderer _meshRenderer;

        [SerializeField]
        protected BoxCollider _boxCollider;

        [SerializeField]
        protected Rigidbody _rigidbody;

        [SerializeField]
        protected ParticleSystem _equipmentImpact;

        [SerializeField]
        protected bool _isInitialized, _isEnabled;

        Vector3 _contactPosition;

        public bool GetIsActivated => _isInitialized && _isEnabled;
        public bool GetIsInitialized => _isInitialized;

        public virtual void Initialize() 
        {
            _contactPosition = Vector3.zero;

            _isInitialized = true;
        }

        public virtual void Awake() 
        {
            _meshRenderer = GetComponent<MeshRenderer>();
            _boxCollider = GetComponent<BoxCollider>();
            _rigidbody = GetComponent<Rigidbody>();
        }

        public virtual void Update() { }

        public virtual void LateUpdateRemote(OffenseDirection pOffenseDirection) { }

        public virtual void OnCollisionEnter(Collision pCollision)
        {
            if (_contactPosition != pCollision.GetContact(0).point)
            {
                _contactPosition = transform.InverseTransformPoint(pCollision.transform.position);

                _equipmentImpact.transform.localPosition = _contactPosition;

                if (!_equipmentImpact.transform.gameObject.activeSelf)
                {
                    _equipmentImpact.transform.gameObject.SetActive(true);
                    _equipmentImpact.Play();
                }
            }
        }

        public virtual void OnCollisionExit(Collision pCollision)
        {
            if (_equipmentImpact.transform.localPosition != Vector3.zero)
            {
                _equipmentImpact.transform.localPosition = Vector3.zero;

                _contactPosition = _equipmentImpact.transform.localPosition;

                if (_equipmentImpact.transform.gameObject.activeSelf)
                    _equipmentImpact.transform.gameObject.SetActive(false);
            }
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
    }

#if UNITY_EDITOR

    [CustomEditor(typeof(Equipment))]
    public class EquipmentEditor : NUIEditor
    {
        public override bool OnInspectorNUI()
        {
            if (!base.OnInspectorNUI())
                return false;

            drawer.BeginSubsection("Debug Value");

            drawer.Field("_isInitialized", false);
            drawer.Field("_isEnabled", false);

            drawer.Space();

            drawer.Field("_meshRenderer", false);
            drawer.Field("_boxCollider", false);
            drawer.Field("_rigidbody", false);

            drawer.EndSubsection();

            drawer.BeginSubsection("Configuration");

            drawer.Field("_equipmentImpact");

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