using UnityEngine;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditorInternal;
#endif

namespace ICustomEditor.Class
{
    public abstract class UnityICustomEditor : MonoBehaviour, ICustomEditor 
    {
        protected List<ReorderableList> _reorderableList = new List<ReorderableList>();

        protected List<string> _reorderableListName = new List<string>();

        protected GUIStyle _guiStyle;

        public virtual void Awake() { }
        public virtual void Start() { }

        public ReorderableList[] GetReorderableLists => _reorderableList.ToArray();

        //Editor

        public virtual void ReorderableListOnEnable(SerializedObject pSerializedObject) 
        {
            if (_reorderableListName.Count == 0)
                return;

            for (int i = 0; i < _reorderableListName.Count; ++i)
                _reorderableList.Add(SetReorderableList(pSerializedObject.FindProperty(_reorderableListName[i])));

        }

        ReorderableList SetReorderableList(SerializedProperty pSerializedProperty)
        {
            ReorderableList reorderableList = null;

            reorderableList = new ReorderableList(pSerializedProperty.serializedObject, pSerializedProperty, true, true, true, true);

            reorderableList.serializedProperty = pSerializedProperty;
            reorderableList.displayAdd = true;
            reorderableList.displayRemove = true;

            reorderableList.drawHeaderCallback = pRect =>
            {
                EditorGUI.LabelField(pRect, new GUIContent(pSerializedProperty.displayName));
            };

            reorderableList.drawElementCallback = (pRect, pIndex, pActive, pFocused) =>
            {
                EditorGUI.PropertyField(pRect, pSerializedProperty.GetArrayElementAtIndex(pIndex), true);
            };

            reorderableList.elementHeightCallback = pIndex =>
            {
                return EditorGUI.GetPropertyHeight(pSerializedProperty.GetArrayElementAtIndex(pIndex), true) + 1;
            };

            reorderableList.onAddCallback = (ReorderableList pList) => 
            {
                ++pList.serializedProperty.arraySize;
            };

            return reorderableList;
        }

        public virtual void CustomOnEnable() 
        {
            _guiStyle = new GUIStyle();
            _guiStyle.fontStyle = FontStyle.BoldAndItalic;
        }

        public virtual void CustomOnDisable() 
        {
            _guiStyle = null;

            _reorderableList = new List<ReorderableList>();
            _reorderableListName = new List<string>();
        }

        public virtual void CustomOnDestroy() { }

        public virtual void ReorderableListOnInspectorGUI(SerializedObject pSerializedObject)
        {
            if (_reorderableList.Count == 0)
                return;

            pSerializedObject.Update();

            for (int i = 0; i < _reorderableListName.Count; ++i)
                _reorderableList[i].DoLayoutList();

            pSerializedObject.ApplyModifiedProperties();
        }

        public virtual void CustomOnInspectorGUI() 
        {
            Editor.CreateEditor(this).OnInspectorGUI();
        }

        public virtual void CustomOnSceneGUI() { }
    }
}