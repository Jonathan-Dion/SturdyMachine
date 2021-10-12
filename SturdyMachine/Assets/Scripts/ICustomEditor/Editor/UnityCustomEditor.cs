using UnityEditor;
using UnityEditorInternal;
using System.Collections.Generic;

using ICustomEditor.Class;

[CustomEditor(typeof(UnityICustomEditor), true)]
public class UnityCustomEditor : Editor
{
    void OnEnable()
    {
        UnityICustomEditor t_unityICustomEditor = target as UnityICustomEditor;

        if (t_unityICustomEditor != null)
        {
            t_unityICustomEditor.CustomOnEnable();

            //UnityEngine.Debug.Log(serializedObject.FindProperty("_reorderableName").GetArrayElementAtIndex(0));

            t_unityICustomEditor.ReorderableListOnEnable(new SerializedObject(serializedObject.FindProperty("_reorderableName").serializedObject.targetObject));
        }
    }

    void OnDisable()
    {
        UnityICustomEditor t_unityICustomEditor = target as UnityICustomEditor;

        if (t_unityICustomEditor != null)
            t_unityICustomEditor.CustomOnDisable();
    }

    void OnDestroy()
    {
        UnityICustomEditor t_unityICustomEditor = target as UnityICustomEditor;

        if (t_unityICustomEditor != null)
            t_unityICustomEditor.CustomOnDestroy();
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        UnityICustomEditor t_unityICustomEditor = target as UnityICustomEditor;

        EditorGUI.BeginChangeCheck();

        Undo.RecordObject(t_unityICustomEditor, "UnityICustomEditor");

        t_unityICustomEditor.ReorderableListOnInspectorGUI(new SerializedObject(serializedObject.FindProperty("_reorderableName").serializedObject.targetObject));

        serializedObject.ApplyModifiedProperties();

        t_unityICustomEditor.CustomOnInspectorGUI();

        if (EditorGUI.EndChangeCheck())
            EditorUtility.SetDirty(t_unityICustomEditor);

        if (UnityEngine.Event.current.type == UnityEngine.EventType.ValidateCommand)
        {
            if (UnityEngine.Event.current.commandName == "UndoRedoPerformed")
                Repaint();
        }
    }

    public void OnSceneGUI()
    {
        UnityICustomEditor t_unityICustomEditor = target as UnityICustomEditor;

        EditorGUI.BeginChangeCheck();

        t_unityICustomEditor.CustomOnSceneGUI();

        if (EditorGUI.EndChangeCheck())
            EditorUtility.SetDirty(t_unityICustomEditor);
    }
}