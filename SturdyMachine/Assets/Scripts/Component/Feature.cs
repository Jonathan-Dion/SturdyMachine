using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Feature 
{
    [DisallowMultipleComponent]
    public abstract class Feature : MonoBehaviour 
    {
        public virtual void Awake() { }
        public virtual void Start() { }
        public virtual void FixedUpdate() { }
        public virtual void Update() { }
        public virtual void LateUpdate() { }
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(Feature), true)]
    public class FeatureEditor : Editor 
    {
        protected GUIStyle _guiStyle;

        protected virtual void FeatureOnInspectorGUI() 
        {
        
        }

        protected void OnEnable()
        {
            _guiStyle = new GUIStyle();

            _guiStyle.fontStyle = FontStyle.BoldAndItalic;
        }

        protected void OnDisable()
        {
            
        }

        public override void OnInspectorGUI()
        {
            FeatureOnInspectorGUI();
        }
    }
#endif
}