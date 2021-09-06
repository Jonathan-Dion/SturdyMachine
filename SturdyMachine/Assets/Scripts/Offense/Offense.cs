using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

[CreateAssetMenu(fileName = "NewCustomAnimation", menuName = "Offence/CustomOffence", order = 1)]
public class Offense : ScriptableObject
{
    [SerializeField]
    protected AnimationClip _clip;

    [SerializeField]
    protected OffenseDirection _offenseDirection;

    [SerializeField]
    protected OffenseType _offenseType;

    [SerializeField]
    protected AnimationClip _repelClip;

    [SerializeField]
    bool _isCooldownAvailable;

    [SerializeField]
    float _maxCooldownTime;

    public OffenseDirection GetOffenseDirection => _offenseDirection;
    public OffenseType GetOffenseType => _offenseType;

    public AnimationClip GetClip => _clip;
    public AnimationClip GetRepelClip => _repelClip;
    public bool GetIsCooldownAvailable => _isCooldownAvailable;
    public float GetMaxCooldownTime => _maxCooldownTime;

    public bool GetIsGoodOffense(OffenseDirection pOffenseDirection, OffenseType pOffenseType) 
    {
        if (pOffenseDirection == _offenseDirection)
            if (pOffenseType == _offenseType)
                return true;

        return false;
    }
}

[CustomEditor(typeof(Offense))]
public class OffenseEditor : Editor 
{
    GUIStyle _guiStyle;

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        if (_guiStyle == null) 
        {
            _guiStyle = new GUIStyle();

            _guiStyle.fontStyle = FontStyle.BoldAndItalic;
        }

        #region Clip information

        EditorGUILayout.BeginVertical(GUI.skin.box);

        EditorGUILayout.PropertyField(serializedObject.FindProperty("_clip"), new GUIContent("Animation Clip: "));

        EditorGUILayout.Space();

        EditorGUILayout.PropertyField(serializedObject.FindProperty("_repelClip"), new GUIContent("Repel: "));

        EditorGUILayout.Space();

        EditorGUILayout.EndVertical();

        #endregion

        #region Offense informations

        #region Offense configuration

        EditorGUILayout.BeginVertical(GUI.skin.box);

        GUILayout.Label("Informations:", _guiStyle);

        EditorGUILayout.PropertyField(serializedObject.FindProperty("_offenseDirection"), new GUIContent("Direction: "));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("_offenseType"), new GUIContent("Type: "));

        EditorGUILayout.EndVertical();

        #endregion

        EditorGUILayout.Space();

        #region Cooldown configuration

        EditorGUILayout.BeginVertical(GUI.skin.box);

        GUILayout.Label("Cooldown", _guiStyle);

        EditorGUILayout.Space();

        ++EditorGUI.indentLevel;

        EditorGUILayout.BeginHorizontal();

        EditorGUILayout.PropertyField(serializedObject.FindProperty("_isCooldownAvailable"), new GUIContent("Available"));

        if (serializedObject.FindProperty("_isCooldownAvailable").boolValue == true)
            EditorGUILayout.PropertyField(serializedObject.FindProperty("_maxCooldownTime"), new GUIContent("Time: "));

        EditorGUILayout.EndHorizontal();

        EditorGUILayout.EndVertical();

        --EditorGUI.indentLevel;

        #endregion

        #endregion

        serializedObject.ApplyModifiedProperties();
    }
}