using System;
using System.Collections.Generic;

using UnityEngine;

using SturdyMachine.Features.Fight;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace SturdyMachine
{
    [Serializable]
    public partial class MonsterBot : Bot
    {
        [SerializeField]
        FightDataGroup _fightDataGroup;

        [SerializeField]
        float _currentOffenseTimer, _currentWaitingTimer, _currentTimer;

        [SerializeField]
        Vector3 _focusRange;

        bool _isStanceActivated;

        [SerializeField]
        int _currentOffenseIndex, _currentFightDataIndex;

        float _currentWaintingBegin, _currentWaintingEnd;

        bool _isCurrentOffenseIsPlayed;

        public Vector3 GetFocusRange => _focusRange;

        public virtual void Initialize(FightDataGroup[] pFightDataGroup)
        {
            Offense.Manager.OffenseManager offenseManager = Instantiate(_offenseManager);

            _offenseManager = null;

            _offenseManager = offenseManager;

            MonsterOffenseInit(pFightDataGroup);

            SetDefault(true);

            _offenseManager.SetAnimation(_animator, OffenseDirection.STANCE, OffenseType.DEFAULT, true);

            base.Initialize();
        }

        void MonsterOffenseInit(FightDataGroup[] pFightDataGroup) 
        {
            List<FightData> fightDataList = new List<FightData>();

            for (int i = 0; i < pFightDataGroup.Length; ++i) 
            {
                if (pFightDataGroup[i].monsterBot == gameObject)
                {
                    _fightDataGroup.monsterBot = gameObject;

                    for (int j = 0; j < pFightDataGroup[i].fightData.Length; ++j)
                    {
                        List<FightOffenseData> fightOffenseDataList = new List<FightOffenseData>();

                        FightData fightData = new FightData();

                        if (fightData.offenseBlockType != pFightDataGroup[i].fightData[j].offenseBlockType)
                            fightData.offenseBlockType = pFightDataGroup[i].fightData[j].offenseBlockType;

                        for (int k = 0; k < pFightDataGroup[i].fightData[j].fightOffenseData.Length; ++k)
                        {
                            FightOffenseData fightOffenseData = new FightOffenseData();

                            //WaitingTimer
                            if (pFightDataGroup[i].fightData[j].fightOffenseData[k].isWaiting)
                            {
                                fightOffenseData.waitingBegin = pFightDataGroup[i].fightData[j].fightOffenseData[k].waitingBegin;
                                fightOffenseData.waitingEnd = pFightDataGroup[i].fightData[j].fightOffenseData[k].waitingEnd;
                            }

                            //Offense
                            fightOffenseData.offenseDirection = pFightDataGroup[i].fightData[j].fightOffenseData[k].offenseDirection;
                            fightOffenseData.offenseType = pFightDataGroup[i].fightData[j].fightOffenseData[k].offenseType;

                            //Timer
                            if (fightOffenseData.offenseDirection == OffenseDirection.STANCE)
                                fightOffenseData.timer = pFightDataGroup[i].fightData[j].fightOffenseData[k].timer;

                            fightOffenseDataList.Add(fightOffenseData);
                        }

                        fightData.fightOffenseData = fightOffenseDataList.ToArray();

                        fightDataList.Add(fightData);
                    }
                }
            }

            _fightDataGroup.fightData = fightDataList.ToArray();
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

        bool IsWaitingEnd() 
        {
            if (!IsWaitingTimer(ref _currentWaintingEnd, _fightDataGroup.fightData[_currentFightDataIndex].fightOffenseData[_currentOffenseIndex].waitingEnd))
            {
                _isCurrentOffenseIsPlayed = false;

                SetDefault();
            }
            else if (_offenseManager.GetCurrentOffense().GetOffenseDirection != OffenseDirection.DEFAULT)
            {
                if (!_isStanceActivated)
                    _isStanceActivated = true;

                _offenseManager.SetAnimation(_animator, OffenseDirection.STANCE, OffenseType.DEFAULT, _isStanceActivated, true);

                return true;
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

            if (_fightDataGroup.Equals(null))
                return;

            if (_fightDataGroup.fightData == null)
                return;

            if (_offenseManager.GetCurrentOffense())
            {
                if (_offenseManager.GetCurrentOffense().GetOffenseDirection == OffenseDirection.STANCE)
                {
                    if (!_isStanceActivated)
                        _isStanceActivated = true;
                }

                if (!IsWaitingTimer(ref _currentWaintingBegin, _fightDataGroup.fightData[_currentFightDataIndex].fightOffenseData[_currentOffenseIndex == -1 ? 0 : _currentOffenseIndex].waitingBegin))
                {
                    if (_currentOffenseIndex + 1 <= _fightDataGroup.fightData[_currentFightDataIndex].fightOffenseData.Length - 1)
                    {
                        if (!_isCurrentOffenseIsPlayed)
                        {
                            ++_currentOffenseIndex;

                            if (_fightDataGroup.fightData[_currentFightDataIndex].fightOffenseData[_currentOffenseIndex].offenseDirection == OffenseDirection.STANCE)
                            {
                                if (!_isStanceActivated)
                                    _isStanceActivated = true;
                            }
                            else if (_isStanceActivated)
                                _isStanceActivated = false;

                            _offenseManager.SetAnimation(_animator, _fightDataGroup.fightData[_currentFightDataIndex].fightOffenseData[_currentOffenseIndex].offenseDirection, _fightDataGroup.fightData[_currentFightDataIndex].fightOffenseData[_currentOffenseIndex].offenseType, _isStanceActivated, true);

                            _isCurrentOffenseIsPlayed = true;
                        }
                    }

                    if (_isCurrentOffenseIsPlayed)
                    {
                        if (!IsWaitingTimer(ref _currentTimer, _fightDataGroup.fightData[_currentFightDataIndex].fightOffenseData[_currentOffenseIndex].timer))
                        {
                            if (_animator.GetCurrentAnimatorClipInfo(0)[0].clip == _offenseManager.GetCurrentOffense().GetClip)
                            {
                                if (_offenseManager.GetCurrentOffense().GetOffenseDirection != OffenseDirection.STANCE)
                                {
                                    if (_animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f)
                                    {
                                        if (IsWaitingEnd())
                                            return;
                                    }
                                }
                                else if (IsWaitingEnd())
                                    return;
                            }
                        }
                    }
                    else if (_currentOffenseIndex == _fightDataGroup.fightData[_currentFightDataIndex].fightOffenseData.Length - 1)
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

            drawer.Property("_fightDataGroup");

            drawer.EndSubsection();

            drawer.BeginSubsection("Debug");

            drawer.Field("_currentFightDataIndex", false, null, "Fight block index: ");
            drawer.Field("_currentOffenseIndex", false, null, "Offenseindex: ");

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