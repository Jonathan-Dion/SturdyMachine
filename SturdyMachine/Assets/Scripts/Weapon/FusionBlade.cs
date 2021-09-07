using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(BoxCollider))]
[RequireComponent(typeof(Rigidbody))]
[ExecuteInEditMode]
public class FusionBlade : MonoBehaviour
{
    FusionWeapon _fusionWeapon;
}

#if UNITY_EDITOR
[CustomEditor(typeof(FusionBlade))]
public class FusionBladeEditor : FusionWeaponEditor 
{
    void OnEnable()
    {
        
    }

    public override void OnInspectorGUI()
    {
        
    }
}

#endif