using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

using ICustomEditor.Class;

namespace Equipment
{
    [RequireComponent(typeof(MeshRenderer))]
    [RequireComponent(typeof(BoxCollider))]
    [RequireComponent(typeof(Rigidbody))]
    public abstract class Equipment : UnityICustomEditor 
    {
        [SerializeField]
        protected MeshRenderer _meshRenderer;

        [SerializeField]
        protected BoxCollider _collider;

        [SerializeField]
        protected Rigidbody _rigidbody;

        [SerializeField]
        protected ParticleSystem _equipmentImpact;

        Vector3 _contactPosition;

        public override void Awake() 
        {
            _meshRenderer = GetComponent<MeshRenderer>();
            _collider = GetComponent<BoxCollider>();
            _rigidbody = GetComponent<Rigidbody>();
        }

        public override void Start() { }

        public virtual void Update() { }

        public virtual void CustomLateUpdate(OffenseDirection pOffenseDirection) { }

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

#if UNITY_EDITOR
        
        public override void CustomOnInspectorGUI()
        {
            EditorGUILayout.BeginVertical(GUI.skin.box);

            GUILayout.Label("Equipment", _guiStyle);

            EditorGUILayout.Space();

            EditorGUILayout.ObjectField(_equipmentImpact, typeof(ParticleSystem), true);

            EditorGUILayout.EndVertical();
        }

#endif
    }
}