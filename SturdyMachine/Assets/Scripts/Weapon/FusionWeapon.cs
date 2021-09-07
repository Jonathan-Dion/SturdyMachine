using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

class FusionWeapon : Weapon
{
    [SerializeField]
    protected ParticleSystem _particleSystem;

    public override MeshRenderer GetMeshRenderer => _meshRenderer;
    public override BoxCollider GetBoxCollider => _collider;
    public override Rigidbody GetRigidbody => _rigidbody;

    public FusionWeapon() { }

    public FusionWeapon(MeshRenderer pMeshRenderer, BoxCollider pBoxCollider, Rigidbody pRigidbody, ParticleSystem pParticleSystem) : base(pMeshRenderer, pBoxCollider, pRigidbody)
    {
        _particleSystem = pParticleSystem;
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
        if (_particleSystem != null)
        {
            if (pOffenseDirection == OffenseDirection.STANCE)
            {
                if (_particleSystem.transform.gameObject.activeSelf)
                    _particleSystem.transform.gameObject.SetActive(false);
            }
            else if (!_particleSystem.transform.gameObject.activeSelf)
                _particleSystem.transform.gameObject.SetActive(true);
        }
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(FusionWeapon))]
public class FusionWeaponEditor : WeaponEditor
{
    GUIStyle _guiStyle;

    void OnEnable()
    {
        //Style
        if (_guiStyle == null)
        {
            _guiStyle = new GUIStyle();

            _guiStyle.fontStyle = FontStyle.BoldAndItalic;
        }
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        EditorGUILayout.Space();

        EditorGUILayout.BeginVertical(GUI.skin.box);

        GUILayout.Label("FusionWeapon", _guiStyle);

        EditorGUILayout.Space();

        EditorGUILayout.PropertyField(serializedObject.FindProperty("_particleSystem"), new GUIContent("Particle System: "));

        EditorGUILayout.EndVertical();
    }
}
#endif