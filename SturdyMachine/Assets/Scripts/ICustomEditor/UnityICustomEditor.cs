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
        [SerializeField]
        protected List<ReorderableList> _reorderableList = new List<ReorderableList>();

        [SerializeField]
        protected List<string> _reorderableListID = new List<string>();


        [SerializeField]
        protected List<string> _reorderableName = new List<string>();

        protected GUIStyle _guiStyle;

        public virtual void Awake() { }
        public virtual void Start() { }

        public ReorderableList[] GetReorderableLists => _reorderableList.ToArray();
        public string[] GetReorderableListID => _reorderableListID.ToArray();
        public string[] GetReorderableName => _reorderableListID.ToArray();

        //Editor

        public virtual void ReorderableListOnEnable(SerializedObject pSerializedObject)
        {
            if (_reorderableName.Count != 0)
            {
                for (int i = 0; i < _reorderableName.Count; ++i)
                {
                    if (_reorderableListID.Count == 0)
                        _reorderableListID.Add(_reorderableName[i] + pSerializedObject.targetObject.GetInstanceID());

                    if (_reorderableList.Count == 0)
                        _reorderableList.Add(null);

                    _reorderableList[i] = ReorderableInitialization(_reorderableName[i], pSerializedObject);
                }
            }
        }

        public virtual void CustomOnEnable() 
        {
            _guiStyle = new GUIStyle();
            _guiStyle.fontStyle = FontStyle.BoldAndItalic;
        }

        public virtual void CustomOnDisable() 
        {
            _reorderableName.Clear();
        }

        public virtual void CustomOnDestroy() { }

        public virtual void ReorderableListOnInspectorGUI(SerializedObject pSerializedObject)
        {
            for (int i = 0; i < _reorderableList.Count; ++i)
                _reorderableList[i].DoLayoutList();
        }

        public virtual void CustomOnInspectorGUI() 
        {
            Editor.CreateEditor(this).OnInspectorGUI();
        }

        public virtual void CustomOnSceneGUI() { }

        ReorderableList GetReorderableListValue(string pReorderableListID)
        {
            for (int i = 0; i < _reorderableListID.Count; ++i)
            {
                if (_reorderableListID[i] == pReorderableListID)
                    return _reorderableList[i];
            }

            return null;
        }

        ReorderableList ReorderableInitialization(string pReorderableName, SerializedObject pSerializedObject) 
        {
            ReorderableList reorderableList = GetReorderableListValue(pReorderableName + $"{pSerializedObject.targetObject.GetInstanceID()}");

            if (reorderableList == null)
                reorderableList = new ReorderableList(pSerializedObject, pSerializedObject.FindProperty(pReorderableName), true, true, true, true);

            reorderableList.serializedProperty = pSerializedObject.FindProperty(pReorderableName);
            reorderableList.displayAdd = true;
            reorderableList.displayRemove = true;

            reorderableList.drawHeaderCallback = pRect =>
            {
                EditorGUI.LabelField(pRect, new GUIContent(pSerializedObject.FindProperty(pReorderableName).displayName));
            };

            reorderableList.drawElementCallback = (pRect, pIndex, pActive, pFocused) =>
            {
                EditorGUI.PropertyField(pRect, pSerializedObject.FindProperty(pReorderableName).GetArrayElementAtIndex(pIndex), true);
            };

            reorderableList.elementHeightCallback = pIndex =>
            {
                return EditorGUI.GetPropertyHeight(pSerializedObject.FindProperty(pReorderableName).GetArrayElementAtIndex(pIndex), true) + 1;
            };

            return reorderableList;
        }
    }
}