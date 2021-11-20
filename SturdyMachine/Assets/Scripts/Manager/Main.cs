using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

using ICustomEditor.Class;
using Feature.Manager;

namespace GameplayFeature.Manager
{
    [RequireComponent(typeof(FeatureManager))]
    public class Main : UnityICustomEditor
    {
        [SerializeField]
        Transform _sturdyMachine;

        [SerializeField]
        FeatureManager _featureManager;

        static Main _main;

        public static Main GetInstance => _main;
        public FeatureManager GetFeatureManager { get { return _featureManager; } }
        public Transform GetSturdyMachine => _sturdyMachine;

        public override void Awake()
        {
            _main = this;

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

        public override void CustomOnEnable()
        {
            _featureManager = GetComponent<FeatureManager>();
        }

        public override void CustomOnInspectorGUI()
        {
            GUI.enabled = false;

            _featureManager = (FeatureManager)EditorGUILayout.ObjectField("FeatureManager", _featureManager, typeof(FeatureManager), true);

            GUI.enabled = true;

            EditorGUILayout.BeginVertical(GUI.skin.box);

            _sturdyMachine = (Transform)EditorGUILayout.ObjectField("SturdyMachine:", _sturdyMachine, typeof(Transform), true);

            EditorGUILayout.EndVertical();

            EditorGUILayout.Space();

        }

#endif
    }
}