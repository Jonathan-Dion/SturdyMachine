using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Feature.Focus.Manager
{
    public abstract class FocusManager : Feature
    {
        Transform _sturdyMachine;
        Transform _currentFocus;
        Vector3[] _originalMonsterPosition;

        System.Random _random;

        int _currentMonsterBot, _lastMonsterBot;
        float _currentTimer;

        [SerializeField]
        protected Transform[] _monsterBot;

        [SerializeField, Range(0f, 5f)]
        protected float _maxTimer;

        public Transform GetCurrentFocus => _currentFocus;

        public override void Awake()
        {
            base.Awake();

            _random = new System.Random();
        }

        public virtual void Start(Transform pSturdyBot)
        {
            _sturdyMachine = pSturdyBot;

            _originalMonsterPosition = new Vector3[_monsterBot.Length];

            if (_originalMonsterPosition.Length > 1)
            {
                for (int i = 0; i < _monsterBot.Length; ++i)
                    _originalMonsterPosition[i] = _monsterBot[i].position;
            }
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
        }

        public override void Update()
        {
            base.Update();

            if (_monsterBot.Length > 1)
            {
                _currentTimer += Time.deltaTime;

                if (_currentTimer >= _maxTimer)
                {
                    if (_monsterBot[_currentMonsterBot].position != _originalMonsterPosition[_currentMonsterBot])
                        _monsterBot[_currentMonsterBot].position = _originalMonsterPosition[_currentMonsterBot];

                    if (_currentMonsterBot == 0f)
                        _currentMonsterBot = _random.Next(_monsterBot.Length);
                    else if (_currentMonsterBot == _lastMonsterBot)
                    {
                        _currentMonsterBot = _random.Next(_monsterBot.Length);

                        while (_currentMonsterBot == _lastMonsterBot)
                            _currentMonsterBot = _random.Next(_monsterBot.Length);

                        _currentFocus = _monsterBot[_currentMonsterBot];

                        _monsterBot[_currentMonsterBot].position = Vector3.MoveTowards(_monsterBot[_currentMonsterBot].position, _sturdyMachine.position, 1f);
                    }

                    _currentTimer = 0f;
                }
            }
        }

        public override void LateUpdate()
        {
            base.LateUpdate();
        }
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(FocusManager))]
    public class FocusManagerEditor : FeatureEditor 
    {
        SerializedProperty _monsterBot;
        SerializedProperty _maxTimer;

        bool _isMonsterFoldout;

        protected override void FeatureOnInspectorGUI()
        {
            base.FeatureOnInspectorGUI();

            EditorGUILayout.BeginVertical(GUI.skin.box);

            EditorGUILayout.BeginHorizontal();

            GUILayout.Label("Focus", _guiStyle);

            EditorGUILayout.IntField(_monsterBot.arraySize, "Size: ");

            serializedObject.ApplyModifiedProperties();

            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space();

            _isMonsterFoldout = EditorGUILayout.Foldout(_isMonsterFoldout, "Monster bot");

            if (_isMonsterFoldout)
            {
                for (int i = 0; i < _monsterBot.arraySize; ++i) 
                {
                    EditorGUILayout.ObjectField(_monsterBot.GetArrayElementAtIndex(i), typeof(Transform), new GUIContent($"{i}: "));

                    serializedObject.ApplyModifiedProperties();
                }
            }

            EditorGUILayout.EndVertical();
        }

        protected void OnEnable()
        {
            base.OnEnable();

            _monsterBot = serializedObject.FindProperty("_monsterBot");
            _maxTimer = serializedObject.FindProperty("_maxTimer");
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            FeatureOnInspectorGUI();
        }
    }

#endif

}