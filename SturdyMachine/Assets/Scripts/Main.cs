//using UnityEngine;

//#if UNITY_EDITOR
//using UnityEditor;
//#endif

//using GameplayFeature.Focus.Manager;

//namespace GameplayFeature.Manager
//{
//    public class Main : FeatureManager 
//    {
//        static Main _main;

//        public static Main GetInstance => _main;
//        public Transform GetCurrentFocus => _focusManager.GetCurrentFocus;

//        public override void Awake()
//        {
//            _main = this;

//            base.Awake();
//        }

//        public override void Start()
//        {
//            base.Start();
//        }

//        public override void FixedUpdate()
//        {
//            base.FixedUpdate();
//        }

//        public override void Update()
//        {
//            base.Update();
//        }

//        public override void LateUpdate()
//        {
//            base.LateUpdate();
//        }
//    }

//#if UNITY_EDITOR
//    [CustomEditor(typeof(Main))]
//    public class MainEditor : FeatureManagerEditor 
//    {
//        protected void OnEnable()
//        {
//            base.OnEnable();
//        }

//        public override void OnInspectorGUI()
//        {
//            base.OnInspectorGUI();
//        }
//    }

//#endif
//}