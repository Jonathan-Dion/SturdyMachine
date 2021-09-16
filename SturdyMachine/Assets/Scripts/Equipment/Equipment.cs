using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Equipment
{
    [RequireComponent(typeof(MeshRenderer))]
    [RequireComponent(typeof(BoxCollider))]
    [RequireComponent(typeof(Rigidbody))]
    public abstract class Equipment : MonoBehaviour 
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

        public virtual void Awake() 
        {
            _meshRenderer = GetComponent<MeshRenderer>();
            _collider = GetComponent<BoxCollider>();
            _rigidbody = GetComponent<Rigidbody>();
        }

        public virtual void Start() { }

        public virtual void FixedUpdate() { }

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
    }

#if UNITY_EDITOR
    
    [CustomEditor(typeof(Equipment), true)]
    public class EquipmentEditor : Editor 
    {
        protected GUIStyle _guiStyle;

        protected void OnEnable()
        {
            _guiStyle = new GUIStyle();

            _guiStyle.fontStyle = FontStyle.BoldAndItalic;
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.BeginVertical(GUI.skin.box);

            GUILayout.Label("Equipment", _guiStyle);

            EditorGUILayout.Space();

            EditorGUILayout.ObjectField(serializedObject.FindProperty("_equipmentImpact"));

            serializedObject.ApplyModifiedProperties();

            EditorGUILayout.EndVertical();
        }
    }

#endif
}