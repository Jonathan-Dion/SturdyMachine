using UnityEngine;
using System;
using System.Collections.Generic;

using GameplayFeature.Manager;
using ICustomEditor.Class;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditorInternal;
#endif

namespace Feature.Focus.Manager
{
    public class FocusManager : UnityICustomEditor
    {
        Transform _currentFocus;
        Vector3[] _originalMonsterPosition;

        System.Random _random;

        int _currentMonsterBot, _lastMonsterBot;
        float _currentTimer;

        [SerializeField]
        protected List<GameObject> _monsterBot = new List<GameObject>();

        [SerializeField, Range(0f, 5f), Tooltip("Time in seconds before the next focus change")]
        protected float _maxTimer;

        public override void Awake() 
        {
            _random = new System.Random();
        }

        public override void Start() 
        {
            _originalMonsterPosition = new Vector3[_monsterBot.Count];

            if (_originalMonsterPosition.Length > 1)
            {
                for (int i = 0; i < _monsterBot.Count; ++i)
                    _originalMonsterPosition[i] = _monsterBot[i].transform.position;
            }

            if (_monsterBot.Count == 1)
                _currentFocus = _monsterBot[0].transform;
        }

        public virtual void Update()
        {
            if (_monsterBot.Count > 1)
            {
                _currentTimer += Time.deltaTime;

                if (_currentTimer >= _maxTimer)
                {
                    _lastMonsterBot = _currentMonsterBot;

                    while(_currentMonsterBot == _lastMonsterBot)
                        _currentMonsterBot = _random.Next(_monsterBot.Count);

                    _currentFocus = _monsterBot[_currentMonsterBot].transform;

                    _monsterBot[_currentMonsterBot].transform.position = Vector3.MoveTowards(_currentFocus.position, Main.GetInstance.GetSturdyMachine.position, 0.5f);

                    _monsterBot[_lastMonsterBot].transform.position = _originalMonsterPosition[_lastMonsterBot];

                    _currentTimer = 0f;
                }

                if (_currentFocus)
                {
                    Main.GetInstance.GetSturdyMachine.rotation = Quaternion.Slerp(Main.GetInstance.GetSturdyMachine.rotation, Quaternion.LookRotation(_currentFocus.position - Main.GetInstance.GetSturdyMachine.position), 0.07f);
                }
            }
        }

        public virtual void LateUpdate() { }

#if UNITY_EDITOR

        public override void CustomOnEnable()
        {
            base.CustomOnEnable();

            _reorderableListName.Add("_monsterBot");
        }

        public override void CustomOnInspectorGUI()
        {
            //MaxTimer
            _maxTimer = EditorGUILayout.FloatField("MaxTimer: ", _maxTimer, _guiStyle);

            EditorGUILayout.Space();
        }

#endif
    }

}