using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class FusionWeapon : Weapon
{
    [SerializeField]
    protected ParticleSystem _weaponTrail;

    Vector3 _contactPosition;

    public override MeshRenderer GetMeshRenderer => _meshRenderer;
    public override BoxCollider GetBoxCollider => _collider;
    public override Rigidbody GetRigidbody => _rigidbody;
    public override ParticleSystem GetWeaponImpact => _weaponImpact;

    public FusionWeapon() { }

    public FusionWeapon(MeshRenderer pMeshRenderer, BoxCollider pBoxCollider, Rigidbody pRigidbody, ParticleSystem pWeaponTrail, ParticleSystem pWeaponImpact) : base(pMeshRenderer, pBoxCollider, pRigidbody, pWeaponImpact)
    {
        _weaponTrail = pWeaponTrail;
    }

    public override void Awake()
    {

    }

    public override void Start()
    {
        
    }

    public override void FixedUpdate()
    {
        
    }

    public override void Update()
    {
        
    }

    public override void LateUpdate(OffenseDirection pOffenseDirection)
    {
        if (_weaponTrail != null)
        {
            if (pOffenseDirection == OffenseDirection.STANCE)
            {
                if (_weaponTrail.transform.gameObject.activeSelf)
                    _weaponTrail.transform.gameObject.SetActive(false);
            }
            else if (!_weaponTrail.transform.gameObject.activeSelf)
                _weaponTrail.transform.gameObject.SetActive(true);
        }
    }

    public override void OnCollisionEnter(Transform pTransform, Collision pCollision)
    {
        if (_contactPosition != pCollision.GetContact(0).point)
        {
            _contactPosition = pTransform.InverseTransformPoint(pCollision.transform.position);

            _weaponTrail.transform.localPosition = _contactPosition;

            if (!_weaponTrail.transform.gameObject.activeSelf)
            {
                _weaponTrail.transform.gameObject.SetActive(true);
                _weaponTrail.Play();
            }
        }
    }

    public override void OnCollisionExit(Transform pTransform, Collision pCollision)
    {
        if (_weaponTrail.transform.localPosition != Vector3.zero)
        {
            _weaponTrail.transform.localPosition = Vector3.zero;

            _contactPosition = _weaponTrail.transform.localPosition;

            if (_weaponTrail.transform.gameObject.activeSelf)
                _weaponTrail.transform.gameObject.SetActive(false);
        }
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(WeaponEditor))]
public class FusionWeaponEditor : Editor
{
    WeaponEditor _weaponEditor;

    GUIStyle _guiStyle;

    void OnEnable()
    {
        _weaponEditor = target as WeaponEditor;

        //Style
        if (_guiStyle == null)
        {
            _guiStyle = new GUIStyle();

            _guiStyle.fontStyle = FontStyle.BoldAndItalic;
        }
    }

    public override void OnInspectorGUI()
    {
        _weaponEditor.OnInspectorGUI();

        EditorGUILayout.Space();

        EditorGUILayout.BeginVertical(GUI.skin.box);

        GUILayout.Label("FusionWeapon", _guiStyle);

        EditorGUILayout.Space();

        EditorGUILayout.PropertyField(serializedObject.FindProperty("_weaponTrail"), new GUIContent("Trail: "));

        EditorGUILayout.EndVertical();
    }
}
#endif