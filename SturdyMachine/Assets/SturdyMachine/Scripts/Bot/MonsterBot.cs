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
        FightOffenseSequence _fightOffenseSequence;

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
        /// Checks if all conditions allow verification of this bot's offense sequences
        /// </summary>
        /// <param name="pFightModule">FightModule</param>
        /// <returns>Returns the result of all conditions</returns>
        bool GetIfNeedCheckOffensePatern(FightModule pFightModule) {

            //Assigns the idle offense by default if no OffenseSequence is assigned for this bot
            if (_fightOffenseSequence.fightOffenseSequenceData.Length == 0) {

                base.OnUpdate(OffenseDirection.STANCE, OffenseType.DEFAULT, _offenseManager.GetIsStance(), pFightModule);

                return false;
            }

            //Assigns the offense that was assigned in the OffenseManager for this bot if no bot was selected in the FocusModule
            if (pFightModule.GetOffenseMonsterBotBlocking.instanciateID != -1)
            {
                if (!_isDeflectionActivated)
                    _isDeflectionActivated = true;

                if (_offenseManager.GetCurrentOffense())
                    base.OnUpdate(_offenseManager.GetCurrentOffense().GetOffenseDirection, _offenseManager.GetCurrentOffense().GetOffenseType, _offenseManager.GetIsStanceOffense, pFightModule);

                return true;
            }

            //Checking if no OffenseSequence is assigned for this bot
            if (pFightModule.GetFightOffenseSequence.Length == 0)
                return false;

            //Checks if the CurrentOffense has been assigned
            if (!_offenseManager.GetCurrentOffense()) {

                base.OnUpdate(OffenseDirection.STANCE, OffenseType.DEFAULT, _offenseManager.GetIsStance(), pFightModule);

                return false;
            }

            return true;
        }

        /// <summary>
        /// Checks if all conditions regarding fightDataIndex management must be carried out
        /// </summary>
        /// <returns>Returns the status of all conditions</returns>
        bool GetIsFightDataIndex() {

            if (_isDeflectionActivated)
                return true;

            if (_offenseManager.GetCurrentOffense().GetOffenseType == OffenseType.DAMAGEHIT) {
            
                if (_animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f)
                    return true;
            }

            return false;
        }

        /// <summary>
        /// Assigns the index of the next OffenseSequence based on conditions
        /// </summary>
        /// <returns>Returns the index of the next OffenseSequence</returns>
        int GetNextFightDataIndex() {

            int nextFightIndex = _currentFightDataIndex;

            if (_currentHittingCount + 1 > _fightOffenseSequence.fightOffenseSequenceData.Length - 1)
                return 0;

            ++nextFightIndex;

            //TODO: Add new blockingChance management with FightOffenseSequence and Offense

            return nextFightIndex;
        }

        /// <summary>
        /// Method to initialize all enemy bot components
        /// </summary>
        /// <param name="pOffenseSequencePatternsData">Table grouping all the OffenseSequence of this bot</param>
        public virtual void Initialize(FightOffenseSequence[] pFightOffenseSequence) {

            base.Initialize();

            _random = new System.Random();

            _currentHittingCount = -1;

            _offenseManager = GetInitOffenseManager();

            OffenseInit(pFightOffenseSequence);

            _offenseManager.SetAnimation(_animator, OffenseDirection.STANCE, OffenseType.DEFAULT, true);

        }

        public virtual bool OnUpdate(FightModule pFightModule) {

            if (!base.OnUpdate())
                return false;

            /*Sif (!GetIfNeedCheckOffensePatern(pFightModule))
                OnOffenseSequencePattern(pFightModule);*/

            return true;
        }

        /// <summary>
        /// Initialize all enemy bot offense sequences
        /// </summary>
        /// <param name="pOffenseSequencePatternsData">Table containing all enemy bot offenses</param>
        void OffenseInit(FightOffenseSequence[] pFightOffenseSequence) {
        
            List<FightOffenseSequence> offenseSequencePattern = new List<FightOffenseSequence>();

            //Iterate each offense sequence in the array
            for (int i = 0; i < pFightOffenseSequence.Length; ++i) {

                //Check if the enemy bot matches this one
                if (pFightOffenseSequence[i].ennemiBot != gameObject)
                    continue;

                //Assign the current ennemy gameobject bot
                _fightOffenseSequence.ennemiBot = gameObject;

                //Assign all SequenceOffense for this bot
                _fightOffenseSequence.fightOffenseSequenceData = pFightOffenseSequence[i].fightOffenseSequenceData;

            }

        }

        /// <summary>
        /// Method for managing this bot's OffenseSequence
        /// </summary>
        /// <param name="pFightModule">FightModule</param>
        void OnOffenseSequencePattern(FightModule pFightModule) {

            //Assigns the state variable of the Stance type offense if the CurrentOffense is of this type
            if (_offenseManager.GetCurrentOffense().GetOffenseDirection == OffenseDirection.STANCE)
            {
                if (!_isStanceActivated)
                    _isStanceActivated = true;
            }

            if (GetIsFightDataIndex()) {
            
            }
        }

        void OnFightDataIndex() {
        
            
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

            drawer.Property("_fightOffenseSequence");

            drawer.EndSubsection();

            drawer.BeginSubsection("Debug");

            drawer.Field("_currentHittingCount", false, null, "Hit count: ");

            drawer.Field("_currentFightDataIndex", false, null, "Fight block index: ");

            drawer.Field("_currentOffenseIndex", false, null, "Offenseindex: ");

            drawer.EndSubsection();

            drawer.EndEditor(this);

            return true;
        }
    }

#endif
}