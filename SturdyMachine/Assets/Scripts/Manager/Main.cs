using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

using ICustomEditor.Class;
using Feature.Manager;

namespace GameplayFeature.Manager
{
    [DisallowMultipleComponent, RequireComponent(typeof(FeatureManager))]
    public class Main : UnityICustomEditor
    {
        [SerializeField]
        Transform _sturdyMachine;

        FeatureManager _featureManager;

        static Main _main;

        public static Main GetInstance => _main;
        public Transform GetSturdyMachine => _sturdyMachine;

        public override void Awake()
        {
            _main = this;

            _featureManager = GetComponent<FeatureManager>();

            _featureManager.Awake();
        }

        public override void Start() 
        {
            _featureManager.Start();
        }

        void Update()
        {
            _featureManager.Update();
        }

        void LateUpdate()
        {
            _featureManager.LateUpdate();
        }

#if UNITY_EDITOR

        public override void CustomOnInspectorGUI()
        {
            EditorGUILayout.BeginVertical(GUI.skin.box);

            EditorGUILayout.ObjectField("SturdyMachine:", _sturdyMachine, typeof(Transform), true);

            EditorGUILayout.EndVertical();

            EditorGUILayout.Space();

        }

#endif
    }
}