using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

abstract class Weapon
{
    [SerializeField]
    protected MeshRenderer _meshRenderer;

    [SerializeField]
    protected BoxCollider _collider;

    [SerializeField]
    protected Rigidbody _rigidbody;

    public abstract MeshRenderer GetMeshRenderer { get; }
    public abstract BoxCollider GetBoxCollider { get; }
    public abstract Rigidbody GetRigidbody { get; }

    public Weapon() { }
    
    public Weapon(MeshRenderer pMeshRenderer, BoxCollider pBoxCollider, Rigidbody pRigidbody) 
    {
        _meshRenderer = pMeshRenderer;
        _collider = pBoxCollider;
        _rigidbody = pRigidbody;
    }

    public virtual void Awake() { }
    public virtual void Start() { }
    public virtual void FixedUpdate() { }
    public virtual void Update() { }
    public virtual void LateUpdate(OffenseDirection pOffenseDirection) { }
}

#if UNITY_EDITOR
[CustomEditor(typeof(Weapon))]
public class WeaponEditor : Editor
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
        serializedObject.Update();

        EditorGUILayout.BeginVertical(GUI.skin.box);

        GUILayout.Label("Weapon", _guiStyle);

        EditorGUILayout.Space();

        EditorGUILayout.PropertyField(serializedObject.FindProperty("_meshRenderer"), new GUIContent("Mesh Renderer: "));

        EditorGUILayout.Space();

        EditorGUILayout.PropertyField(serializedObject.FindProperty("_collider"), new GUIContent("Box Collider: "));

        EditorGUILayout.Space();

        EditorGUILayout.PropertyField(serializedObject.FindProperty("_rigidbody"), new GUIContent("Rigidbody: "));

        EditorGUILayout.EndVertical();
    }
}
#endif