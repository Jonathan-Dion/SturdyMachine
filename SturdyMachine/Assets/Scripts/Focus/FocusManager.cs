using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace GameplayFeature.Focus.Manager
{
    public abstract class FocusManager : GameplayFeature 
    {
        Transform _sturdyPosition;

        [SerializeField]
        GameObject[] _monsterBot;

        Transform _currentFocus;
        Vector3[] _originalMonsterPosition;
        int _currentMonsterBot, _lastMonsterBot;

        System.Random _random;
        
        public Transform GetCurrentFocus => _currentFocus;

        public override void Awake()
        {
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

    }

#if UNITY_EDITOR
    [CustomEditor(typeof(FocusManager))]
    public class FocusManagerEditor : GameplayFeatureEditor 
    {
        SerializedProperty _monsterBot;

        bool _isMonsterBotFoldout;

        protected void OnEnable()
        {
            base.OnEnable();

            _monsterBot = serializedObject.FindProperty("_monsterBot");
        }

        public override void OnInspectorGUI()
        {
            EditorGUILayout.BeginVertical(GUI.skin.box);

            GUILayout.Label("Focus", _guiStyle);

            EditorGUILayout.Space();

            EditorGUILayout.BeginVertical(GUI.skin.box);

            EditorGUILayout.BeginHorizontal();

            _isMonsterBotFoldout = EditorGUILayout.Foldout(_isMonsterBotFoldout, "MonsterBot");

            EditorGUILayout.IntField(_monsterBot.arraySize, "Size: ");

            serializedObject.ApplyModifiedProperties();

            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space();

            EditorGUILayout.BeginVertical(GUI.skin.box);

            if (_isMonsterBotFoldout)
            {
                for (int i = 0; i < _monsterBot.arraySize; ++i) 
                {
                    EditorGUILayout.BeginHorizontal();

                    EditorGUILayout.ObjectField(_monsterBot.GetArrayElementAtIndex(i), typeof(Transform), new GUIContent($"{i}: "));

                    EditorGUILayout.EndVertical();
                }
            }

            EditorGUILayout.EndVertical();

            EditorGUILayout.EndVertical();

            base.OnInspectorGUI();

            EditorGUILayout.EndVertical();
        }
    }


#endif
}