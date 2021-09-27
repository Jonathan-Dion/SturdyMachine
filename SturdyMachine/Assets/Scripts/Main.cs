using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

using Feature.Manager;

namespace GameplayFeature.Manager
{
    public class Main : FeatureManager
    {
        [SerializeField]
        Transform _sturdyMachine;

        static Main _main;

        public static Main GetInstance => _main;
        public Transform GetSturdyMachine => _sturdyMachine;
        public Transform GetCurrentFocus => _focusManager.GetCurrentFocus;

        public override void Awake()
        {
            _main = this;

            base.Awake();
        }

        public override void Start() 
        {
            base.Start();
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
        }

        public override void Update()
        {
            base.Update();
        }

        public override void LateUpdate()
        {
            base.LateUpdate();
        }

#if UNITY_EDITOR
        public override void CustomOnInspectorGUI()
        {
            base.CustomOnInspectorGUI();

            EditorGUILayout.BeginVertical(GUI.skin.box);

            EditorGUILayout.ObjectField(_sturdyMachine, typeof(Transform), true);

            EditorGUILayout.EndVertical();

            EditorGUILayout.Space();

        }

#endif
    }
}