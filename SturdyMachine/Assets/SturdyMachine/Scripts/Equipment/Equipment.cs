using UnityEngine;

using SturdyMachine.Component;

#if UNITY_EDITOR
using NWH.NUI;
using UnityEditor;
#endif

namespace SturdyMachine.Equipment 
{
    /// <summary>
    /// Base class for all equipment
    /// </summary>
    [RequireComponent(typeof(MeshRenderer))]
    [RequireComponent(typeof(BoxCollider))]
    [RequireComponent(typeof(Rigidbody))]
    public abstract class Equipment : BaseComponent
    {
        /// <summary>
        /// Mesh component for this equipment
        /// </summary>
        [SerializeField, Tooltip("Mesh component for this equipment")]
        protected MeshRenderer _meshRenderer;

        /// <summary>
        /// BoxCollider component for this equipment
        /// </summary>
        [SerializeField, Tooltip("BoxCollider component for this equipment")]
        protected BoxCollider _boxCollider;

        /// <summary>
        /// Rigidbody component for this equipment
        /// </summary>
        [SerializeField, Tooltip("Rigidbody component for this equipment")]
        protected Rigidbody _rigidbody;

        /// <summary>
        /// ParticleSystem component for create impact hit effect for this equipment
        /// </summary>
        [SerializeField, Tooltip("ParticleSystem component for create impact hit effect for this equipment")]
        protected ParticleSystem _equipmentImpact;

        public override void OnAwake()
        {
            base.OnAwake();

            _meshRenderer = GetComponent<MeshRenderer>();
            _boxCollider = GetComponent<BoxCollider>();
            _rigidbody = GetComponent<Rigidbody>();

            if (_equipmentImpact)
                _equipmentImpact.gameObject.SetActive(false);
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