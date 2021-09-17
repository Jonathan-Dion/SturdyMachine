using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace GameplayFeature
{
    [DisallowMultipleComponent]
    public abstract class GameplayFeature : MonoBehaviour
    {
        public virtual void Awake() { }
        public virtual void Start() { }
        public virtual void FixedUpdate() { }
        public virtual void Update() { }
        public virtual void LateUpdate() { }
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(GameplayFeature), true)]
    public class GameplayFeatureEditor : Editor 
    {
        protected GUIStyle _guiStyle;

        protected void OnEnable()
        {
            _guiStyle = new GUIStyle();

            _guiStyle.fontStyle = FontStyle.BoldAndItalic;
        }

        public override void OnInspectorGUI()
        {
            
        }
    }

#endif
}