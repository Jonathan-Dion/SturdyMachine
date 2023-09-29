using System;
using System.Collections.Generic;

using UnityEngine;

using SturdyMachine.Offense;
using SturdyMachine.Offense.Blocking;

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
        public override FeatureModuleCategory GetFeatureModuleCategory()
        {
            return FeatureModuleCategory.Fight;
        }

        #region Property

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

        #endregion

        #region Get

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

        /*bool GetIsBlockingState(ref FightBlocking pFightBlocking, OffenseBlocking pAttackerOffenseBlocking, Bot pAttackerBot, Bot pDefenderBot, Offense.Offense pMonsterDeflectionOffense = null)
        {
            if (!pFightBlocking.isBlocking)
            {
                if (pAttackerOffenseBlocking.GetIsBlocking(pMonsterDeflectionOffense ? pMonsterDeflectionOffense
                                                                                     : _sturdyBot.GetOffenseManager.GetCurrentOffense(),
                                                           pAttackerBot.GetOffenseManager.GetCurrentOffense(), pAttackerBot.GetAnimator))
                {
                    if (!pMonsterDeflectionOffense)
                        ToogleState(ref pFightBlocking.isBlocking, false);

                    return true;
                }
            }
            else
                return true;

            return false;
        }*/

        /*bool GetIsBlocking(ref FightBlocking pFightBlocking, Bot pAttackerBot, ref FightBlocking pAttackerFightBlocking, Bot pDefenderBot, bool pIsSturdyBot)
        {
            for (int i = 0; i < pAttackerFightBlocking.offenseBlocking.Length; ++i)
            {
                if (pIsSturdyBot)
                {
                    if (_focusModule.GetCurrentFocus == pAttackerBot.transform)
                        return BlockingSetup(ref pFightBlocking, pAttackerFightBlocking.offenseBlocking[i], pAttackerBot, pDefenderBot);
                }
                else if (!pFightBlocking.isBlocking)
                {
                    Offense.Offense deflectionOffense = pAttackerFightBlocking.offenseBlocking[0].GetDeflectionOffense;

                    if (BlockingSetup(ref pFightBlocking, pAttackerFightBlocking.offenseBlocking[i], pAttackerBot, pDefenderBot, deflectionOffense))
                    {
                        if (!pFightBlocking.isHaveChance)
                        {
                            pFightBlocking.isHaveChance = true;

                            if (_random.Next(1, 101) <= 100f * (pDefenderBot as MonsterBot).GetBlockingChance)
                            {
                                pDefenderBot.GetOffenseManager.SetAnimation(pDefenderBot.GetAnimator, deflectionOffense.GetOffenseDirection, deflectionOffense.GetOffenseType, pDefenderBot.GetOffenseManager.GetIsStanceOffense, true);

                                ToogleState(ref pFightBlocking.isBlocking, false);

                                return true;
                            }
                        }

                        pFightBlocking.isHitting = true;
                    }
                }
                else
                    return true;
            }

            return false;

        }*/

        /*bool GetIsHitting(ref FightBlocking pFightBlocking, FightBlocking pAttackerFightBlocking, Bot pAttackerBot)
        {
            for (int i = 0; i < pAttackerFightBlocking.offenseBlocking.Length; ++i)
            {
                if (pAttackerFightBlocking.offenseBlocking[i])
                {
                    if (pAttackerFightBlocking.offenseBlocking[i].GetIsHitting(pAttackerBot.GetOffenseManager.GetCurrentOffense(), pAttackerBot.GetAnimator))
                    {
                        ToogleState(ref pFightBlocking.isHitting, false);

                        return true;
                    }
                }
            }

            return false;
        }*/

        #endregion
    }

#if UNITY_EDITOR


#endif
}