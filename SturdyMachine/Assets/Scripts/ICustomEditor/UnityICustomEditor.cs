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
        protected List<string> _propertyNameList;

        protected GUIStyle _guiStyle;

        public virtual void Awake() { }
        public virtual void Start() { }

        public string[] GetPropertyNameList => _propertyNameList.ToArray();

        //Editor
        public virtual void CustomOnEnable() 
        {
            _guiStyle = new GUIStyle();
            _guiStyle.fontStyle = FontStyle.BoldAndItalic;

            if (_propertyNameList == null)
                _propertyNameList = new List<string>();
        }

        public virtual void CustomOnDisable() { }

        public virtual void CustomOnDestroy() { }

        public virtual void CustomOnInspectorGUI() 
        {
            Editor.CreateEditor(this).OnInspectorGUI();
        }

        public virtual void CustomOnSceneGUI() { }

        public virtual void ReorderableListCreator(string pPropertyNane) 
        {
        
        }

        ReorderableList CustomReorderableList(string pPropertyData) 
        {
            return new ReorderableList(pPropertyData, );
        }

        public virtual void DrawListItems(Rect pRect, int pIndex, bool pIsActive, bool pIsFocused) { }

        public virtual void DrawHeader(Rect pRect) { }
    }
}