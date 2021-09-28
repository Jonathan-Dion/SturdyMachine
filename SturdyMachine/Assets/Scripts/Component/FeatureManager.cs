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
        FocusManager _focusManager;

        public FocusManager GetFocusManager => _focusManager;

        public override void Awake()
        {
            _focusManager.Awake();
        }

        public override void Start() 
        {
            _focusManager.Start();
        }

        void Update()
        {
            _focusManager.Update();
        }

        void LateUpdate()
        {
            _focusManager.LateUpdate();
        }

#if UNITY_EDITOR

        public override void CustomOnInspectorGUI()
        {
            EditorGUILayout.BeginVertical(GUI.skin.box);

            GUILayout.Label("Focus", _guiStyle);

            EditorGUILayout.Space();

            EditorGUILayout.ObjectField(_focusManager, typeof(FocusManager), true);

            EditorGUILayout.EndVertical();
        }

#endif
    }
}