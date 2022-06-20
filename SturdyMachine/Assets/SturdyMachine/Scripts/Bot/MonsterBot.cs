using System;
using System.Collections.Generic;

using UnityEngine;

using SturdyMachine.Features.Fight;

#if UNITY_EDITOR
using UnityEditor;
using NWH.VehiclePhysics2;
#endif

namespace SturdyMachine
{
    [Serializable]
    public partial class MonsterBot : Bot
    {
        [SerializeField]
        FightData[] _fightData;

        [SerializeField]
        float _currentOffenseTimer, _currentWaitingTimer, _currentTimer;

        [SerializeField]
        Vector3 _focusRange;

        bool _isStanceActivated;

        int _currentOffenseIndex;

        float _currentWaintingBegin, _currentWaintingEnd;

        bool _isCurrentOffenseIsPlayed;

        public Vector3 GetFocusRange => _focusRange;

        public virtual void Initialize(FightData[] pFightData)
        {
            Offense.Manager.OffenseManager offenseManager = Instantiate(_offenseManager);

            _offenseManager = null;

            _offenseManager = offenseManager;

            MonsterOffenseInit(pFightData);

            SetDefault(true);

            _offenseManager.SetAnimation(_animator, OffenseDirection.STANCE, OffenseType.DEFAULT, true);

            base.Initialize();
        }

        void MonsterOffenseInit(FightData[] pFightData) 
        {
            float timer = 0f;

            FightData monsterFightData = new FightData();

            List<FightData> monsterFightDataList = new List<FightData>();

            for (int i = 0; i < pFightData.Length; ++i)
            {
                if (pFightData[i].monsterBot == gameObject)
                {
                    //WaitingTimer
                    if (pFightData[i].isWaiting)
                    {
                        monsterFightData.waitingBegin = pFightData[i].waitingBegin;
                        monsterFightData.waitingEnd = pFightData[i].waitingEnd;
                    }

                    //Offense
                    monsterFightData.offenseDirection = pFightData[i].offenseDirection;
                    monsterFightData.offenseType = pFightData[i].offenseType;

                    if (monsterFightData.offenseDirection == OffenseDirection.STANCE)
                        monsterFightData.timer = pFightData[i].timer;

                    monsterFightDataList.Add(monsterFightData);
                }
                else
                    timer += _offenseManager.GetOffenseClipTime(pFightData[i].offenseDirection, pFightData[i].offenseType) + pFightData[i].timer;
            }

            _fightData = monsterFightDataList.ToArray();
        }

        bool IsWaitingTimer(ref float pCurrentWaitingValue, float pWaitingValue) 
        {
            if (pWaitingValue > 0)
            {
                if (pCurrentWaitingValue < pWaitingValue) 
                {
                    pCurrentWaitingValue += Time.deltaTime;

                    return true;
                }
            }

            return false;
        }

        public virtual void SetDefault(bool pIsOffenseIndex = false)
        {
            base.SetDefault();

            if (_currentWaintingBegin != 0)
                _currentWaintingBegin = 0;

            if (_currentWaintingEnd != 0)
                _currentWaintingEnd = 0;

            if (_currentTimer != 0)
                _currentTimer = 0;

            if (pIsOffenseIndex)
            {
                if (_currentOffenseTimer != -1)
                    _currentOffenseIndex = -1;
            }
        }

        public virtual void UpdateRemote() 
        {
            if (!GetIsActivated)
                return;

            if (_fightData.Length == 0)
                return;

            if (_offenseManager.GetCurrentOffense())
            {
                if (_offenseManager.GetCurrentOffense().GetOffenseDirection == OffenseDirection.STANCE)
                {
                    if (!_isStanceActivated)
                        _isStanceActivated = true;
                }

                if (!IsWaitingTimer(ref _currentWaintingBegin, _fightData[_currentOffenseIndex == -1 ? 0 : _currentOffenseIndex].waitingBegin))
                {
                    if (_currentOffenseIndex + 1 <= _fightData.Length - 1)
                    {
                        if (!_isCurrentOffenseIsPlayed)
                        {
                            ++_currentOffenseIndex;

                            if (_fightData[_currentOffenseIndex].offenseDirection == OffenseDirection.STANCE)
                            {
                                if (!_isStanceActivated)
                                    _isStanceActivated = true;
                            }
                            else if (_isStanceActivated)
                                _isStanceActivated = false;

                            _offenseManager.SetAnimation(_animator, _fightData[_currentOffenseIndex].offenseDirection, _fightData[_currentOffenseIndex].offenseType, _isStanceActivated, true);

                            _isCurrentOffenseIsPlayed = true;
                        }
                    }
                    
                    if (_isCurrentOffenseIsPlayed)
                    {
                        if (!IsWaitingTimer(ref _currentTimer, _fightData[_currentOffenseIndex].timer))
                        {
                            if (!IsWaitingTimer(ref _currentWaintingEnd, _fightData[_currentOffenseIndex].waitingEnd))
                            {
                                _isCurrentOffenseIsPlayed = false;

                                SetDefault();
                            }
                            else if (_offenseManager.GetCurrentOffense().GetOffenseType != OffenseType.DAMAGEHIT)
                            {
                                if (_offenseManager.GetCurrentOffense().GetOffenseDirection != OffenseDirection.DEFAULT)
                                {
                                    if (!_isStanceActivated)
                                        _isStanceActivated = true;

                                    _offenseManager.SetAnimation(_animator, OffenseDirection.STANCE, OffenseType.DEFAULT, _isStanceActivated, true);

                                    return;
                                }
                            }
                        }
                    }
                    else if (_currentOffenseIndex == _fightData.Length - 1)
                        SetDefault(true);
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

            drawer.ReorderableList("_fightData");

            drawer.EndSubsection();

            drawer.EndEditor(this);

            return true;
        }


        public override bool UseDefaultMargins()
        {
            return false;
        }
    }

#endif
}