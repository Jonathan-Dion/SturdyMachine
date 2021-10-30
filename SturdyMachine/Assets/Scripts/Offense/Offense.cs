using UnityEngine;

using ICustomEditor.ScriptableObjectEditor;

#if UNITY_EDITOR
using UnityEditor;
#endif

[CreateAssetMenu(fileName = "NewCustomAnimation", menuName = "Offence/CustomOffence", order = 1)]
public class Offense : ScriptableObjectICustomEditor
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

#if UNITY_EDITOR
    public override void CustomOnInspectorGUI()
    {
        EditorGUILayout.BeginVertical(GUI.skin.box);

        GUILayout.Label("Offense", _guiStyle);

        EditorGUILayout.Space();

        #region Clip information

        EditorGUILayout.BeginVertical(GUI.skin.box);

        EditorGUILayout.ObjectField(_clip, typeof(AnimationClip), true);

        EditorGUILayout.Space();

        EditorGUILayout.ObjectField(_repelClip, typeof(AnimationClip), true);

        EditorGUILayout.Space();

        EditorGUILayout.EndVertical();

        #endregion

        #region Offense informations

        #region Offense configuration

        EditorGUILayout.BeginVertical(GUI.skin.box);

        GUILayout.Label("Informations:", _guiStyle);

        _offenseDirection = (OffenseDirection)EditorGUILayout.EnumPopup(_offenseDirection, "Offense Direction: ");
        _offenseType = (OffenseType)EditorGUILayout.EnumPopup(_offenseType, "Offense Type: ");

        EditorGUILayout.EndVertical();

        #endregion

        EditorGUILayout.Space();

        #region Cooldown configuration

        EditorGUILayout.BeginVertical(GUI.skin.box);

        GUILayout.Label("Cooldown", _guiStyle);

        EditorGUILayout.Space();

        ++EditorGUI.indentLevel;

        EditorGUILayout.BeginHorizontal();

        EditorGUILayout.Toggle(_isCooldownAvailable, "Cooldown available");

        if (_isCooldownAvailable)
            EditorGUILayout.FloatField(_maxCooldownTime, "Max cooldown Time: ");

        EditorGUILayout.EndHorizontal();

        EditorGUILayout.EndVertical();

        --EditorGUI.indentLevel;

        #endregion

        #endregion

        EditorGUILayout.EndVertical();
    }

#endif
}