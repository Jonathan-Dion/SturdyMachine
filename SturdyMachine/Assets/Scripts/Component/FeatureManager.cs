using UnityEngine;

using Feature.Focus.Manager;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Feature.Manager 
{
    public class FeatureManager : Feature 
    {
        public FocusManager focusManager;

        public FocusManager GetFocusManager => focusManager;

        public override void Awake()
        {
            base.Awake();

            focusManager.Awake();
        }

        //public virtual void Start(Transform pSturdyBot)
        //{
        //    base.Start();

        //    focusManager.Start(pSturdyBot);
        //}

        public override void FixedUpdate()
        {
            base.FixedUpdate();

            focusManager.FixedUpdate();
        }

        public override void Update()
        {
            base.Update();

            focusManager.Update();
        }

        public override void LateUpdate()
        {
            base.LateUpdate();

            focusManager.LateUpdate();
        }
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(FeatureManager))]
    public class FeatureManagerEditor : FeatureEditor 
    {
        FeatureManager _featureManager;
        
        Editor _focusManagerEditor;

        protected void OnEnable()
        {
            base.OnEnable();

            _featureManager = target as FeatureManager;

            //FocusManager
            _featureManager.focusManager = EditorGUILayout.ObjectField("focusManager", _featureManager.focusManager, typeof(FocusManager), true) as FocusManager;

            _focusManagerEditor = CreateEditor(_featureManager.focusManager);
        }

        protected override void FeatureOnInspectorGUI()
        {
            base.FeatureOnInspectorGUI();
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            EditorGUILayout.BeginVertical(GUI.skin.box);

            _focusManagerEditor.OnInspectorGUI();

            EditorGUILayout.EndVertical();
        }
    }

#endif

}