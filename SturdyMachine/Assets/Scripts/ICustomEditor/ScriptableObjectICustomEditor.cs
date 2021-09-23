using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace ICustomEditor.ScriptableObjectEditor
{
    public abstract class ScriptableObjectICustomEditor : ScriptableObject, ICustomEditor 
    {
        protected GUIStyle _guiStyle;

        public virtual void CustomOnEnable() 
        {
            _guiStyle = new GUIStyle();

            _guiStyle.fontStyle = FontStyle.BoldAndItalic;
        }

        public virtual void CustomOnDisable() { }
        public virtual void CustomOnDestroy() { }

        public virtual void CustomOnInspectorGUI() 
        {
            Editor.CreateEditor(this).DrawDefaultInspector();
        }
        public virtual void CustomOnSceneGUI() { }
    }
}