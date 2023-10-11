using System;
using System.Collections.Generic;

using UnityEngine;

using SturdyMachine.Offense;
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
        FightPatternsData _fightPatternData;

        [SerializeField]
        float _currentOffenseTimer, _currentWaitingTimer, _currentTimer;

        [SerializeField]
        Vector3 _focusRange;

        [SerializeField]
        int _currentOffenseIndex, _currentFightDataIndex, _currentHittingCount;

        [SerializeField, Range(0f, 1f)]
        float _blockingChance;

        System.Random _random;

        bool _isStanceActivated;

        float _currentWaintingBegin, _currentWaintingEnd;

        bool _isCurrentOffenseIsPlayed;

        bool _isDeflectionActivated;

        public float GetBlockingChance => _blockingChance;

        public Vector3 GetFocusRange => _focusRange;

        OffenseManager GetInitOffenseManager() => Instantiate(_offenseManager);

        /// <summary>
        /// MonsterBot initialization
        /// </summary>
        /// <param name="pFightPatternData">All pattern for this MonsterBot</param>
        public virtual void Initialize(FightPatternsData[] pFightPatternData) {

            _random = new System.Random();

            _currentHittingCount = -1;

            _offenseManager = GetInitOffenseManager();

            OffenseInit(pFightPatternData);

            _offenseManager.SetAnimation(_animator, OffenseDirection.STANCE, OffenseType.DEFAULT, true);

            base.Initialize();

        }        

        public override bool OnUpdate()
        {
            if (!base.OnUpdate())
                return false;


            return true;
        }

        void OffenseInit(FightPatternsData[] pFightPatternData) {
        
            List<OffensePattern> offensePaternList = new List<OffensePattern>();

            for (int i = 0; i < pFightPatternData.Length; ++i) {

                if (pFightPatternData[i].monsterBot != gameObject)
                    continue;

                _fightPatternData.monsterBot = gameObject;
                _fightPatternData.patternSequenceData = pFightPatternData[i].patternSequenceData;

                OffensePattern offensePatern = new OffensePattern();

                for (int j = 0; j < pFightPatternData[i].offensePatternData.Length; ++j) {
                
                    List<OffensePatternData> offensePatternDataList = new List<OffensePatternData>();

                    offensePatern = new OffensePattern();

                    if (offensePatern.offenseBlockingType != pFightPatternData[i].offensePatternData[j].offenseBlockingType)
                        offensePatern.offenseBlockingType = pFightPatternData[i].offensePatternData[j].offenseBlockingType;

                    for (int k = 0; k < pFightPatternData[i].offensePatternData[j].offensePatternData.Length; ++k) {
                    
                        OffensePatternData offensePatternData = new OffensePatternData();

                        //Offense
                        if (offensePatternData.offenseDirection == pFightPatternData[i].offensePatternData[j].offensePatternData[k].offenseDirection)
                            offensePatternData.offenseType = pFightPatternData[i].offensePatternData[j].offensePatternData[k].offenseType;

                        //Stance
                        if (offensePatternData.offenseDirection == OffenseDirection.STANCE)
                            offensePatternData.stanceTimer = pFightPatternData[i].offensePatternData[j].offensePatternData[k].stanceTimer;

                        offensePatternDataList.Add(offensePatternData);
                    }

                    offensePatern.offensePatternData = offensePatternDataList.ToArray();

                    offensePaternList.Add(offensePatern);
                }
            }

            _fightPatternData.offensePatternData = offensePaternList.ToArray();

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
            drawer.Field("_blockingChance");

            drawer.BeginSubsection("Offense");

            drawer.Property("_fightDataGroup");

            drawer.EndSubsection();

            drawer.BeginSubsection("Debug");

            drawer.Field("_currentHittingCount", false, null, "Hit count: ");

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