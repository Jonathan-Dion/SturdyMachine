using UnityEngine;

using ICustomEditor.Class;
using Feature.Focus.Manager;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Feature.Manager 
{
    public class FeatureManager : UnityICustomEditor 
    {
        [SerializeField]
        protected FocusManager _focusManager = new FocusManager();

        public FocusManager GetFocusManager => _focusManager;

        public virtual void Awake()
        {
            _focusManager.Awake();
        }

        public virtual void Start() 
        {
            _focusManager.Start();
        }

        public virtual void FixedUpdate()
        {
            _focusManager.FixedUpdate();
        }

        public virtual void Update()
        {
            _focusManager.Update();
        }

        public virtual void LateUpdate()
        {
            _focusManager.LateUpdate();
        }

#if UNITY_EDITOR
        public override void CustomOnInspectorGUI()
        {
            base.CustomOnInspectorGUI();

            EditorGUILayout.BeginVertical(GUI.skin.box);

            _focusManager.CustomOnInspectorGUI();

            EditorGUILayout.EndVertical();
        }

#endif
    }

}