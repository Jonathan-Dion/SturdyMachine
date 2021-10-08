using UnityEditor;
using UnityEditorInternal;
using System.Collections.Generic;

using ICustomEditor.Class;

[CustomEditor(typeof(UnityICustomEditor), true)]
public class UnityCustomEditor : Editor 
{
    List<SerializedProperty> _serializedPropertiesList;

    void OnEnable()
    {
        UnityICustomEditor t_unityICustomEditor = target as UnityICustomEditor;

        if (t_unityICustomEditor != null) 
        {
            t_unityICustomEditor.CustomOnEnable();

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
        UnityICustomEditor t_unityICustomEditor = target as UnityICustomEditor;

        //if (serializedObject.FindProperty("_propertyNameList").arraySize != 0)
        //{
            //if (serializedObject.FindProperty("_propertyList") == null)
            //{
            //    serializedObject.FindProperty("_propertyList").objectReferenceValue = 
            //}

                

            //if (serializedObject.FindProperty("_propertyList").arraySize != t_unityICustomEditor.GetPropertyNameList.Length)
            //    serializedObject.FindProperty("_propertyList").arraySize = t_unityICustomEditor.GetPropertyNameList.Length;

            //for (int i = 0; i < t_unityICustomEditor.GetPropertyNameList.Length; ++i)
            //    serializedObject.FindProperty("_propertyList").GetArrayElementAtIndex(i).objectReferenceValue = serializedObject.FindProperty(t_unityICustomEditor.GetPropertyNameList[i]).objectReferenceValue;
        //}

        EditorGUI.BeginChangeCheck();

        Undo.RecordObject(t_unityICustomEditor, "UnityICustomEditor");

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