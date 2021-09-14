using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class FusionBlade : FusionWeapon
{

}

#if UNITY_EDITOR
[CustomEditor(typeof(FusionWeaponEditor))]
public class FusionBladeEditor : Editor 
{
    FusionWeaponEditor _fusionWeaponEditor;

    GUIStyle _guiStyle;

    void OnEnable()
    {
        _fusionWeaponEditor = target as FusionWeaponEditor;

        //Style
        if (_guiStyle == null)
        {
            _guiStyle = new GUIStyle();

            _guiStyle.fontStyle = FontStyle.BoldAndItalic;
        }
    }

    public override void OnInspectorGUI()
    {
        _fusionWeaponEditor.OnInspectorGUI();

        EditorGUILayout.Space();

        EditorGUILayout.BeginVertical(GUI.skin.box);

        GUILayout.Label("FusionBlade", _guiStyle);

        EditorGUILayout.Space();

        EditorGUILayout.PropertyField(serializedObject.FindProperty("_weaponImpact"), new GUIContent("Impact: "));

        EditorGUILayout.EndVertical();
    }
}

#endif