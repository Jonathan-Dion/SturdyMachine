using System;
using System.Collections.Generic;

using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
using NWH.VehiclePhysics2;
#endif

namespace SturdyMachine
{
    [Serializable]
    public struct MonsterOffense 
    {
        public float timerWaiting;

        public OffenseDirection offenseDirection;
        public OffenseType offenseType;

        public float timer;
    }

    [Serializable]
    public partial class MonsterBot : Bot
    {
        [SerializeField]
        MonsterOffense[] _monsterOffense;

        [SerializeField]
        float _currentOffenseTimer, _currentWaitingTimer, _maxOffenseTimer, _maxWaitingTimer;

        [SerializeField]
        Vector3 _focusRange;

        bool _isStanceActivated;

        bool _isFirstOffense;

        int _currentOffenseIndex;

        public Vector3 GetFocusRange => _focusRange;

        public virtual void Initialize(Features.Fight.FightData[] pFightData)
        {
            Offense.Manager.OffenseManager offenseManager = Instantiate(_offenseManager);

            _offenseManager = null;

            _offenseManager = offenseManager;

            MonsterOffenseInit(pFightData);

            _currentOffenseIndex = -1;

            _offenseManager.SetAnimation(_animator, OffenseDirection.STANCE, OffenseType.DEFAULT, true);

            base.Initialize();
        }

        void MonsterOffenseInit(Features.Fight.FightData[] pFightData) 
        {
            float timer = 0f;
            MonsterOffense monsterOffense = new MonsterOffense();
            List<MonsterOffense> monsterOffenseList = new List<MonsterOffense>();

            for (int i = 0; i < pFightData.Length; ++i)
            {
                if (pFightData[i].monsterBot == gameObject)
                {
                    if (monsterOffenseList.Count == 0)
                    {
                        if (i > 0)
                            monsterOffense.timerWaiting = pFightData[i].waitingTimer;
                    }

                    monsterOffense.offenseDirection = pFightData[i].offenseDirection;
                    monsterOffense.offenseType = pFightData[i].offenseType;
                    monsterOffense.timer = pFightData[i].timer;

                    monsterOffenseList.Add(monsterOffense);
                }
                else
                    timer += _offenseManager.GetOffenseClipTime(pFightData[i].offenseDirection, pFightData[i].offenseType) + pFightData[i].timer;
            }

            _monsterOffense = monsterOffenseList.ToArray();
        }

        public virtual void UpdateRemote() 
        {
            if (!GetIsActivated)
                return;

            if (_offenseManager.GetCurrentOffense())
            {
                if (_offenseManager.GetCurrentOffense().GetOffenseDirection == OffenseDirection.STANCE)
                {
                    if (!_isStanceActivated)
                        _isStanceActivated = true;

                    //Idle Fightning
                    if (_offenseManager.GetCurrentOffense().GetOffenseType == OffenseType.DEFAULT)
                    {
                        if (!_isFirstOffense)
                        {
                            if (_monsterOffense[0].timerWaiting > 0)
                            {
                                if (_maxWaitingTimer != _monsterOffense[0].timerWaiting)
                                    _maxWaitingTimer = _monsterOffense[0].timerWaiting;
                            }

                            _isFirstOffense = true;
                        }
                        else if (_maxWaitingTimer > 0)
                        {
                            _currentWaitingTimer += Time.deltaTime;

                            if (_currentWaitingTimer >= _maxWaitingTimer)
                                _currentWaitingTimer = _maxWaitingTimer = 0;
                        }
                    }
                }

                if (_maxWaitingTimer == 0)
                {
                    if (_currentOffenseIndex + 1 <= _monsterOffense.Length - 1)
                    {
                        if (_currentOffenseTimer >= _maxOffenseTimer)
                        {
                            ++_currentOffenseIndex;

                            if (_monsterOffense[_currentOffenseIndex].offenseDirection == OffenseDirection.STANCE)
                            {
                                if (!_isStanceActivated)
                                    _isStanceActivated = true;
                            }
                            else if (_isStanceActivated)
                                _isStanceActivated = false;

                            if (_maxOffenseTimer != _monsterOffense[_currentOffenseIndex].timer)
                                _maxOffenseTimer = _monsterOffense[_currentOffenseIndex].timer;

                            _currentOffenseTimer = 0f;
                        }
                        else
                            _currentOffenseTimer += Time.deltaTime;
                    }
                    else if (_currentOffenseIndex == _monsterOffense.Length - 1)
                    {
                        if (_offenseManager.GetCurrentOffense().GetOffenseDirection == OffenseDirection.STANCE) 
                        {
                            if (_offenseManager.GetCurrentOffense().GetOffenseType == OffenseType.DEFAULT) 
                            {
                                if (_currentOffenseIndex != 0)
                                    _currentOffenseIndex = 0;

                                return;
                            }
                        }

                        if (_animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f)
                            _offenseManager.SetAnimation(_animator, OffenseDirection.STANCE, OffenseType.DEFAULT, _isStanceActivated, true);

                        return;
                    }

                    _offenseManager.SetAnimation(_animator, _monsterOffense[_currentOffenseIndex].offenseDirection, _monsterOffense[_currentOffenseIndex].offenseType, _isStanceActivated, true);

                }
            }
        }

        public override void LateUpdateRemote(OffenseDirection pOffenseDirection)
        {
            base.LateUpdateRemote(pOffenseDirection);
        }
    }

#if UNITY_EDITOR

    [CustomEditor(typeof(MonsterBot))]
    public class MonsterBotEditor : BotEditor
    {
        public override bool OnInspectorNUI()
        {
            if (!base.OnInspectorNUI())
                return false;

            EditorGUI.BeginChangeCheck();

            drawer.Field("_focusRange");

            drawer.BeginSubsection("Offense");

            drawer.ReorderableList("_monsterOffense");

            drawer.EndSubsection();

            drawer.EndEditor(this);

            return true;
        }


        public override bool UseDefaultMargins()
        {
            return false;
        }
    }

    [CustomPropertyDrawer(typeof(MonsterOffense))]
    public partial class MonsterOffenseDrawer : ComponentNUIPropertyDrawer
    {
        public override bool OnNUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (!base.OnNUI(position, property, label))
                return false;

            drawer.Field("timerWaiting");
            drawer.Field("offenseDirection");
            drawer.Field("offenseType");
            drawer.Field("timer");

            drawer.EndProperty();
            return true;
        }
    }

#endif
}