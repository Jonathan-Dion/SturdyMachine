using UnityEditor;

using ICustomEditor.ScriptableObjectEditor;

[CustomEditor(typeof(ScriptableObjectICustomEditor), true)]
public class ScriptableObjectEditor : Editor 
{
    void OnEnable()
    {
        ScriptableObjectICustomEditor t_scriptableObjectICustomEditor = target as ScriptableObjectICustomEditor;

        if (t_scriptableObjectICustomEditor != null)
            t_scriptableObjectICustomEditor.CustomOnEnable();
    }

    void OnDisable()
    {
        ScriptableObjectICustomEditor t_scriptableObjectICustomEditor = target as ScriptableObjectICustomEditor;

        if (t_scriptableObjectICustomEditor != null)
            t_scriptableObjectICustomEditor.CustomOnDisable();
    }

    void OnDestroy()
    {
        ScriptableObjectICustomEditor t_scriptableObjectICustomEditor = target as ScriptableObjectICustomEditor;

        if (t_scriptableObjectICustomEditor != null)
            t_scriptableObjectICustomEditor.CustomOnDestroy();
    }

    public override void OnInspectorGUI()
    {
        ScriptableObjectICustomEditor t_scriptableObjectICustomEditor = target as ScriptableObjectICustomEditor;

        EditorGUI.BeginChangeCheck();

        Undo.RecordObject(t_scriptableObjectICustomEditor, "ScriptableObjectICustomEditor");

        t_scriptableObjectICustomEditor.CustomOnInspectorGUI();

        if (EditorGUI.EndChangeCheck())
            EditorUtility.SetDirty(t_scriptableObjectICustomEditor);

        if (UnityEngine.Event.current.type == UnityEngine.EventType.ValidateCommand)
        {
            if (UnityEngine.Event.current.commandName == "UndoRedoPerformed")
                Repaint();
        }
    }

    public void OnSceneGUI()
    {
        ScriptableObjectICustomEditor t_scriptableObjectICustomEditor = target as ScriptableObjectICustomEditor;

        EditorGUI.BeginChangeCheck();

        t_scriptableObjectICustomEditor.CustomOnSceneGUI();

        if (EditorGUI.EndChangeCheck())
            EditorUtility.SetDirty(t_scriptableObjectICustomEditor);
    }
}