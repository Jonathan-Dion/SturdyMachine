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
    [Serializable]
    public class FocusManager : UnityICustomEditor
    {
        Transform _currentFocus;
        Vector3[] _originalMonsterPosition;

        System.Random _random;

        int _currentMonsterBot, _lastMonsterBot;
        float _currentTimer;

        [SerializeField]
        protected List<GameObject> _monsterBot = new List<GameObject>();

        [SerializeField, Range(0f, 5f)]
        protected float _maxTimer;

        public override void Awake() { }

        public override void Start() 
        {
            _originalMonsterPosition = new Vector3[_monsterBot.Count];

            if (_originalMonsterPosition.Length > 1)
            {
                for (int i = 0; i < _monsterBot.Count; ++i)
                    _originalMonsterPosition[i] = _monsterBot[i].transform.position;
            }
        }

        public virtual void Update()
        {
            if (_monsterBot.Count > 1)
            {
                _currentTimer += Time.deltaTime;

                if (_currentTimer >= _maxTimer)
                {
                    if (_monsterBot[_currentMonsterBot].transform.position != _originalMonsterPosition[_currentMonsterBot])
                        _monsterBot[_currentMonsterBot].transform.position = _originalMonsterPosition[_currentMonsterBot];

                    if (_currentMonsterBot == 0f)
                        _currentMonsterBot = _random.Next(_monsterBot.Count);
                    else if (_currentMonsterBot == _lastMonsterBot)
                    {
                        _currentMonsterBot = _random.Next(_monsterBot.Count);

                        while (_currentMonsterBot == _lastMonsterBot)
                            _currentMonsterBot = _random.Next(_monsterBot.Count);

                        _monsterBot[_currentMonsterBot].transform.position = Vector3.MoveTowards(_monsterBot[_currentMonsterBot].transform.position, Main.GetInstance.GetSturdyMachine.position, 1f);
                        _currentFocus = _monsterBot[_currentMonsterBot].transform;

                        _monsterBot[_currentMonsterBot].transform.position = Vector3.MoveTowards(_monsterBot[_currentMonsterBot].transform.position, Main.GetInstance.GetSturdyMachine.position, 1f);
                    }

                    _currentTimer = 0f;
                }
            }
            else
                _currentFocus = _monsterBot[_monsterBot.Count - 1].transform;

            //SturdyMachine focus
            if (Main.GetInstance.GetSturdyMachine.rotation != Quaternion.Slerp(Main.GetInstance.GetSturdyMachine.rotation, Quaternion.LookRotation(_currentFocus.position), 0.07f))
                Main.GetInstance.GetSturdyMachine.rotation = Quaternion.Slerp(Main.GetInstance.GetSturdyMachine.rotation, Quaternion.LookRotation(_currentFocus.position), 0.07f);
        }

        public virtual void LateUpdate() { }

#if UNITY_EDITOR

        public override void CustomOnEnable()
        {
            base.CustomOnEnable();

            _reorderableName.Add("_monsterBot");
        }

        public override void CustomOnInspectorGUI()
        {
            EditorGUILayout.BeginVertical(GUI.skin.box);

            EditorGUILayout.EndVertical();
        }

#endif
    }

}