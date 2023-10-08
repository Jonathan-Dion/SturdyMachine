using System;
using System.Collections.Generic;

using UnityEngine;

using SturdyMachine.Offense;
using SturdyMachine.Offense.Blocking;
using SturdyMachine.Manager;

#if UNITY_EDITOR
using UnityEditor;
using NWH.VehiclePhysics2;
#endif

namespace SturdyMachine.Features.Fight 
{
    public enum FightPaternType { DEFAULT, RANDOM, SPECIFIC };

    /// <summary>
    /// Stores information about the assignment of future FightPatterns
    /// </summary>
    [Serializable]
    public struct PatternSequenceData {

        /// <summary>
        /// The sequence pattern offense type
        /// </summary>
        public FightPaternType patternType;

        /// <summary>
        /// Set the current PattrnSequence index when the pattern type is in Specific mode
        /// </summary>
        public int nextPatternIndex;

        /// <summary>
        /// Add this value on blocking chance when the bot receive a hit
        /// </summary>
        public float addBlockingChance;
    }

    /// <summary>
    /// Store the Offense data
    /// </summary>
    [Serializable]
    public struct OffensePatternData
    {
        /// <summary>
        /// The direction for this Offense on OffensePatern
        /// </summary>
        public OffenseDirection offenseDirection;

        /// <summary>
        /// The type for this Offense on OffencePattern
        /// </summary>
        public OffenseType offenseType;

        /// <summary>
        /// The waiting time before making the next attack if the OffenseDirection is in Stance mode
        /// </summary>
        public float stanceTimer;

        public bool isWaiting;


    }

    /// <summary>
    /// Store information about the Offenses the MonsterBot will execute based on the pattern
    /// </summary>
    [Serializable]
    public struct OffensePattern
    {
        /// <summary>
        /// Represents the present behavior of the MonsterBot in these attacks
        /// </summary>
        public OffenseBlockType offenseBlockingType;

        /// <summary>
        /// Array representing the Aoofenses that will be executed in the pattern
        /// </summary>
        public OffensePatternData[] offensePatternData;
    }

    /// <summary>
    /// Store all pattern information of each MonsterBot
    /// </summary>
    [Serializable]
    public struct FightPatternsData {

        /// <summary>
        /// The MonsterBot you want to configure a FightPattern
        /// </summary>
        public GameObject monsterBot;

        /// <summary>
        /// Array regrouping each PatternSequence information for the MonsterBot
        /// </summary>
        public PatternSequenceData[] patternSequenceData;

        /// <summary>
        /// Array grouping all the MonsterBot's OffenseSequence according to its type of fight PatternType
        /// </summary>
        public OffensePattern[] offensePatternData;
    }

    /// <summary>
    /// Store combat information for each bot based on their actions
    /// </summary>
    [Serializable]
    public struct OffenseFightBlocking {

        /// <summary>
        /// Array representing all bot OffenseBlocking during fights
        /// </summary>
        public OffenseBlocking[] offenseBlocking;

        /// <summary>
        /// If the defending bot receives a hit
        /// </summary>
        public bool isHitting;

        /// <summary>
        /// If the defending bot succeeded the blocking
        /// </summary>
        public bool isBlocking;

        /// <summary>
        /// Instantiation index of the MonsterBot who was instanced in the scene and who is in the fight
        /// </summary>
        public int instanciateID;

        /// <summary>
        /// State representing if the MonsterBot is lucky enough to be able to attempt to block the attack Offense
        /// </summary>
        public bool isHaveChanceToBlock;
    }

    /// <summary>
    /// Module managing the fight sequence as well as the combat behavior of a MonsterBot
    /// </summary>
    [Serializable]
    public partial class FightModule : FeatureModule
    {
        #region Attribut

        /// <summary>
        /// Array representing all the fight pattern of each MonsterBot
        /// </summary>
        [SerializeField, Tooltip("Array representing all the fight pattern of each MonsterBot")]
        FightPatternsData[] _fightPatternsData;

        /// <summary>
        /// Module allowing the management of MonsterBot selection in fights
        /// </summary>
        Focus.FocusModule _focusModule;

        /// <summary>
        /// The combat information for SturdyBot
        /// </summary>
        OffenseFightBlocking _offenseSturdyBotBlocking;

        /// <summary>
        /// The combat information for MonsterBot
        /// </summary>
        OffenseFightBlocking _offenseMonsterBotBlocking;

        System.Random _random;

        Main _main;

        #endregion

        #region Get

        public override FeatureModuleCategory GetFeatureModuleCategory() => FeatureModuleCategory.Fight;

        /// <summary>
        /// Returns the array of all MonsterBot combat patterns
        /// </summary>
        public FightPatternsData[] GetFightPatternsData => _fightPatternsData;

        /// <summary>
        /// Return the combat information of attackingBot
        /// </summary>
        public OffenseFightBlocking GetOffenseSturdyBotBlocking => _offenseSturdyBotBlocking;

        /// <summary>
        /// Return the combat information of defendingBot
        /// </summary>
        public OffenseFightBlocking GetOffenseMonsterBotBlocking => _offenseMonsterBotBlocking;

        bool GetIsBlockingState(ref OffenseFightBlocking pOffenseFightBlocking, OffenseBlocking pAttackerOffenseBlocking, Bot pAttackerBot, Bot pDefenderBot, Offense.Offense pMonsterDeflectionOffense = null)
        {
            if (!pOffenseFightBlocking.isBlocking)
            {
                if (pAttackerOffenseBlocking.GetIsBlocking(pMonsterDeflectionOffense ? pMonsterDeflectionOffense
                                                                                     : _main.GetSturdyBot.GetOffenseManager.GetCurrentOffense(),
                                                           pAttackerBot.GetOffenseManager.GetCurrentOffense(), pAttackerBot.GetAnimator))
                {
                    if (!pMonsterDeflectionOffense)
                        base.ToogleState(ref pOffenseFightBlocking.isBlocking, false);

                    return true;
                }
            }
            else
                return true;

            return false;
        }

        bool GetIsBlocking(ref OffenseFightBlocking pOffenseFightBlocking, Bot pAttackerBot, ref OffenseFightBlocking pAttackerFightBlocking, Bot pDefenderBot, bool pIsSturdyBot)
        {
            for (int i = 0; i < pAttackerFightBlocking.offenseBlocking.Length; ++i)
            {
                if (pIsSturdyBot)
                {
                    if (_focusModule.GetCurrentFocus == pAttackerBot.transform)
                        return BlockingSetup(ref pOffenseFightBlocking, pAttackerFightBlocking.offenseBlocking[i], pAttackerBot, pDefenderBot);
                }
                else if (!pOffenseFightBlocking.isBlocking)
                {
                    Offense.Offense deflectionOffense = pAttackerFightBlocking.offenseBlocking[0].GetDeflectionOffense;

                    if (BlockingSetup(ref pOffenseFightBlocking, pAttackerFightBlocking.offenseBlocking[i], pAttackerBot, pDefenderBot, deflectionOffense))
                    {
                        if (!pOffenseFightBlocking.isHaveChanceToBlock)
                        {
                            pOffenseFightBlocking.isHaveChanceToBlock = true;

                            if (_random.Next(1, 101) <= 100f * (pDefenderBot as MonsterBot).GetBlockingChance)
                            {
                                pDefenderBot.GetOffenseManager.SetAnimation(pDefenderBot.GetAnimator, deflectionOffense.GetOffenseDirection, deflectionOffense.GetOffenseType, pDefenderBot.GetOffenseManager.GetIsStanceOffense, true);

                                ToogleState(ref pOffenseFightBlocking.isBlocking, false);

                                return true;
                            }
                        }

                        pOffenseFightBlocking.isHitting = true;
                    }
                }
                else
                    return true;
            }

            return false;

        }

        bool GetIsHitting(ref OffenseFightBlocking pOffenseFightBlocking, OffenseFightBlocking pAttackerFightBlocking, Bot pAttackerBot)
        {
            for (int i = 0; i < pAttackerFightBlocking.offenseBlocking.Length; ++i)
            {
                if (pAttackerFightBlocking.offenseBlocking[i])
                {
                    if (pAttackerFightBlocking.offenseBlocking[i].GetIsHitting(pAttackerBot.GetOffenseManager.GetCurrentOffense(), pAttackerBot.GetAnimator))
                    {
                        base.ToogleState(ref pOffenseFightBlocking.isHitting, false);

                        return true;
                    }
                }
            }

            return false;
        }

        bool BlockingSetup(ref OffenseFightBlocking pOffenseFightBlocking, OffenseBlocking pAttackerOffenseBlocking, Bot pAttackerBot, Bot pDefenderBot, Offense.Offense pMonsterDeflectionOffense = null)
        {
            if (!pOffenseFightBlocking.isBlocking)
            {
                if (pAttackerOffenseBlocking.GetIsBlocking(pMonsterDeflectionOffense ? pMonsterDeflectionOffense
                                                                                     : _main.GetSturdyBot.GetOffenseManager.GetCurrentOffense(),
                                                           pAttackerBot.GetOffenseManager.GetCurrentOffense(), pAttackerBot.GetAnimator))
                {
                    if (!pMonsterDeflectionOffense)
                        base.ToogleState(ref pOffenseFightBlocking.isBlocking, false);

                    return true;
                }
            }
            else
                return true;

            return false;
        }

        bool GetIsOffenseBlocking(ref OffenseFightBlocking pOffenseFightBlocking, Bot pAttackerBot, bool pIsSturdyBot)
        {
            /*if (!pAttackerBot.GetOffenseManager.GetIsStance())
                _main.GetOffenseBlockingConfig.OffenseBlockingSetup(pAttackerBot.GetOffenseManager.GetCurrentOffense(), pOffenseFightBlocking.offenseBlocking, pIsSturdyBot);
            
            else*/ if (pOffenseFightBlocking.offenseBlocking.Length != 0)
                pOffenseFightBlocking.offenseBlocking = new OffenseBlocking[0];
            
            if (pOffenseFightBlocking.offenseBlocking.Length != 0)
                return true;

            return false;
        }

        #endregion

        #region Method

        public override void Initialize()
        {
            base.Initialize();

            _main = GetComponent<Main>();

            _focusModule = _main.GetComponent<Focus.FocusModuleWrapper>().GetFeatureModule() as Focus.FocusModule;

            _random = new System.Random();

            _offenseMonsterBotBlocking.offenseBlocking = new OffenseBlocking[0];

            if (_offenseMonsterBotBlocking.instanciateID != -1)
                _offenseMonsterBotBlocking.instanciateID = -1;

            _offenseSturdyBotBlocking.offenseBlocking = new OffenseBlocking[0];
        }

        public override bool OnUpdate()
        {
            if (!base.OnUpdate())
                return false;

            for (int i = 0; i < _main.GetMonsterBot.Length; ++i) {

                //MonsterBot to SturdyBot
                OffenseBlockingSetup(_main.GetMonsterBot[i], null, ref _offenseMonsterBotBlocking, ref _offenseSturdyBotBlocking, true);

                //SturdyBot to MonsterBot
                OffenseBlockingSetup(_main.GetSturdyBot, _main.GetMonsterBot[i], ref _offenseSturdyBotBlocking, ref _offenseMonsterBotBlocking);
            }

            return true;
        }

        void OffenseBlockingSetup(Bot pAttackerBot, Bot pDefenderBot, ref OffenseFightBlocking pAttackerFightBlocking, ref OffenseFightBlocking pFightBlocking, bool pIsSturdyBot = false)
        {
            if (!GetIsOffenseBlocking(ref pAttackerFightBlocking, pAttackerBot, pIsSturdyBot))
            {
                base.ToogleState(ref pFightBlocking.isBlocking);
                base.ToogleState(ref pFightBlocking.isHitting);

                if (!pIsSturdyBot)
                {
                    if (_offenseMonsterBotBlocking.instanciateID != -1)
                        _offenseMonsterBotBlocking.instanciateID = -1;

                    if (_offenseMonsterBotBlocking.isHaveChanceToBlock)
                        _offenseMonsterBotBlocking.isHaveChanceToBlock = false;
                }
            }

            else
            {
                if (!GetIsBlocking(ref pFightBlocking, pAttackerBot, ref pAttackerFightBlocking, pDefenderBot, pIsSturdyBot))
                    GetIsHitting(ref pFightBlocking, pAttackerFightBlocking, pAttackerBot);
                else
                    base.ToogleState(ref pFightBlocking.isHitting, true);

                if (!pIsSturdyBot)
                {
                    if (_offenseMonsterBotBlocking.instanciateID != pDefenderBot.transform.GetInstanceID())
                        _offenseMonsterBotBlocking.instanciateID = pDefenderBot.transform.GetInstanceID();
                }
            }
        }

        #endregion
    }

#if UNITY_EDITOR

    [CustomPropertyDrawer(typeof(FightModule))]
    public partial class FightModuleDrawer : FeatureModuleDrawer
    {
        public override bool OnNUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (!base.OnNUI(position, property, label))
                return false;

            drawer.ReorderableList("_fightPatternsData");

            drawer.EndProperty();
            return true;
        }
    }

    /*[CustomPropertyDrawer(typeof(FightOffenseData))]
    public partial class FightOffenseDataDrawer : ComponentNUIPropertyDrawer
    {
        public override bool OnNUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (!base.OnNUI(position, property, label))
                return false;

            drawer.BeginSubsection("Waiting Timer");

            if (drawer.Field("isWaiting").boolValue)
            {
                drawer.Field("waitingBegin", true, "Sec", "Begin: ");
                drawer.Field("waitingEnd", true, "Sec", "End: ");
            }

            drawer.EndSubsection();

            if (drawer.Field("offenseDirection").enumValueIndex == 4)
                drawer.Field("timer");

            drawer.Field("offenseType");

            drawer.EndProperty();
            return true;
        }
    }*/

#endif
}