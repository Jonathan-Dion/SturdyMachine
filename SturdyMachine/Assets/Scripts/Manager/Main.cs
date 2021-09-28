using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

using ICustomEditor.Class;
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

        void Awake()
        {
           _main = this;
        }

        void Start() 
        {
            
        }

        void Update()
        {
            
        }

        void LateUpdate()
        {
            
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