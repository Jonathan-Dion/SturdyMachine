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

        Offense.Offense _currentOffense;

        bool _isStanceActivated;

        bool _isFirstOffense;

        int _currentOffenseIndex;

        public virtual void Initialize(Features.Fight.FightData[] pFightData)
        {
            Offense.Manager.OffenseManager offenseManager = Instantiate(_offenseManager);

            _offenseManager = null;

            _offenseManager = offenseManager;

            MonsterOffenseInit(pFightData);

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
                            monsterOffense.timerWaiting = timer;
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

            if (_offenseManager.GetCurrentOffense(_animator.GetCurrentAnimatorClipInfo(0)[0].clip) != null)
            {
                if (_currentOffense != _offenseManager.GetCurrentOffense(_animator.GetCurrentAnimatorClipInfo(0)[0].clip))
                    _currentOffense = _offenseManager.GetCurrentOffense(_animator.GetCurrentAnimatorClipInfo(0)[0].clip);
            }

            if (_currentOffense)
            {
                if (_monsterOffense.Length > 0)
                {
                    if (_currentOffense.GetOffenseDirection == OffenseDirection.STANCE)
                    {
                        if (!_isStanceActivated)
                            _isStanceActivated = true;

                        if (_currentOffense.GetOffenseType == OffenseType.DEFAULT) 
                        {
                            if (_monsterOffense[0].timerWaiting > 0) 
                            {
                                if (!_isFirstOffense)
                                {
                                    if (_maxWaitingTimer != _monsterOffense[0].timerWaiting)
                                        _maxWaitingTimer = _monsterOffense[0].timerWaiting;

                                    _isFirstOffense = true;
                                }
                            }
                            else if (_maxWaitingTimer != 0)
                            {
                                _currentWaitingTimer += Time.deltaTime;

                                if (_currentWaitingTimer >= _maxWaitingTimer)
                                {
                                    _offenseManager.SetAnimation(_animator, _monsterOffense[_currentOffenseIndex].offenseDirection, _monsterOffense[_currentOffenseIndex].offenseType, _isStanceActivated);

                                    _currentWaitingTimer = _maxWaitingTimer = 0;
                                }
                            }

                        }
                        else if (_currentOffenseTimer >= _maxOffenseTimer)
                        {
                            ++_currentOffenseIndex;

                            if (_currentOffenseIndex > _monsterOffense.Length - 1)
                                _currentOffenseIndex = 0;

                            _offenseManager.SetAnimation(_animator, _monsterOffense[_currentOffenseIndex].offenseDirection, _monsterOffense[_currentOffenseIndex].offenseType, _isStanceActivated);

                            _currentOffenseTimer = 0f;
                        }
                        else
                            _currentOffenseTimer += Time.deltaTime;

                    }

                    if (_currentOffense.GetOffenseType != OffenseType.DEFAULT)
                    {
                        if (_isStanceActivated)
                        {
                            _isStanceActivated = false;

                            if (_maxOffenseTimer != _monsterOffense[_currentOffenseIndex].timer)
                                _maxOffenseTimer = _monsterOffense[_currentOffenseIndex].timer;
                        }


                        if (_animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f)
                        {
                            if (_currentOffense.GetOffenseDirection != OffenseDirection.STANCE)
                                _offenseManager.SetAnimation(_animator, OffenseDirection.STANCE, OffenseType.DEFAULT, _isStanceActivated);
                            else
                                _offenseManager.SetAnimation(_animator, _monsterOffense[_currentOffenseIndex].offenseDirection, _monsterOffense[_currentOffenseIndex].offenseType, _isStanceActivated);
                        }
                    }
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

            drawer.ReorderableList("_monsterOffense");

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

            drawer.Field("offenseDirection");
            drawer.Field("offenseType");
            drawer.Field("timer");

            drawer.EndProperty();
            return true;
        }
    }

#endif
}