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
    /// Store OffenseSequence configuration of the bot
    /// </summary>
    [Serializable, Tooltip("Store OffenseSequence configuration of the bot")]
    public struct FightOffenseSequenceData {

        public OffensePaternType offensePaternType;

        public FightPaternTypeData fightPaternTypeData;

        public OffenseSubSequenceData[] offenseSubSequenceData;

        public float addHitConfirm;

        public float addBlockingChance;
    }

    /// <summary>
    /// Store each enemy bot's OffenseSequence setup
    /// </summary>
    [Serializable, Tooltip("Store each enemy bot's OffenseSequence setup")]
    public struct FightOffenseSequence {

        public GameObject ennemiBot;

        public FightOffenseSequenceData[] fightOffenseSequenceData;
    }

    /// <summary>
    /// Store the Offense data
    /// </summary>
    [Serializable, Tooltip("Store the Offense data")]
    public struct OffenseSubSequenceData
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

        public BlockingChanceData blockingChanceData;
    }

    /// <summary>
    /// Structure to manage the increased chance of bot blocking when it is hit
    /// </summary>
    [Serializable, Tooltip("Structure to manage the increased chance of bot blocking when it is hit")]
    public struct BlockingChanceData {

        public bool isActivated;

        public float delay;
    }

    /// <summary>
    /// Sturture allowing you to save the configuration concerning the change of sequence when the enemy bot is hit
    /// </summary>
    [Serializable, Tooltip("Sturture allowing you to save the configuration concerning the change of sequence when the enemy bot is hit")]
    public struct FightPaternTypeData {

        public FightPaternType paternType;

        public int specificIndex;
    }

    /// <summary>
    /// Store combat information for each bot based on their actions
    /// </summary>
    [Serializable, Tooltip("Store combat information for each bot based on their actions")]
    public struct OffenseFightBlocking
    {
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
        /// Array allowing you to store all the offensive sequences of all the enemy bots in the scene
        /// </summary>
        [SerializeField, Tooltip("Array allowing you to store all the offensive sequences of all the enemy bots in the scene ")]
        FightOffenseSequence[] _fightOffenseSequence;

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

        bool _isHitConfirm;

        #endregion

        #region Get

        public override FeatureModuleCategory GetFeatureModuleCategory() => FeatureModuleCategory.Fight;

        /// <summary>
        /// Table returning the list of all OffenseSequences of this bot
        /// </summary>
        public FightOffenseSequence[] GetFightOffenseSequence => _fightOffenseSequence;

        /// <summary>
        /// Return the combat information of attackingBot
        /// </summary>
        public OffenseFightBlocking GetOffenseSturdyBotBlocking => _offenseSturdyBotBlocking;

        /// <summary>
        /// Return the combat information of defendingBot
        /// </summary>
        public OffenseFightBlocking GetOffenseMonsterBotBlocking => _offenseMonsterBotBlocking;

        /// <summary>
        /// Checks if the defending bot enters its blocking sequence
        /// </summary>
        /// <param name="pOffenseFightBlockingDefender">Defendant's FightBlocking Offense</param>
        /// <param name="pAttackerOffenseBlocking">The attacker's OffenseBlocking.</param>
        /// <param name="pAttackerBot">The attacking bot</param>
        /// <param name="pDefenderBot">The Defendant Bot</param>
        /// <param name="pMonsterOffense">Opponent's Offense</param>
        /// <returns>Returns if the defending bot enters its blocking sequence</returns>
        bool GetIsBlockingState(ref OffenseFightBlocking pOffenseFightBlockingDefender, OffenseBlocking pAttackerOffenseBlocking, Offense.Offense pAttackerCurrentOffense, Animator pAttackerAnimator, Offense.Offense pMonsterOffense = null)
        {
            //Checks if the defending bot is in blocking mode
            if (!pOffenseFightBlockingDefender.isBlocking)
            {
                //Checks if the OffenseBlocking can block the attacking offense
                if (pAttackerOffenseBlocking.GetIsBlocking(pMonsterOffense ? pMonsterOffense
                                                                           : _sturdyComponent.GetComponent<Main>().GetSturdyBot.GetOffenseManager.GetCurrentOffense(),
                                                           pAttackerCurrentOffense, pAttackerAnimator))
                {
                    //Disables the blocked state if the defending bot is the player
                    if (!pMonsterOffense)
                        base.ToogleState(ref pOffenseFightBlockingDefender.isBlocking, false);

                    return true;
                }
            }
            else
                return true;

            return false;
        }

        /// <summary>
        /// Checks if the defending bot successfully blocked its opponent's attack offense
        /// </summary>
        /// <param name="pDefendingOffenseFightBlocking">Defendant's FightBlocking Offense</param>
        /// <param name="pAttackerBot">The attacking bot</param>
        /// <param name="pAttackerFightBlocking">The attacker's OffenseBlocking</param>
        /// <param name="pDefenderBot">The Defendant Bot</param>
        /// <param name="pIsSturdyBot">If the defending bot is the player</param>
        /// <returns></returns>
        bool GetIsBlocking(ref OffenseFightBlocking pDefendingOffenseFightBlocking, Bot pAttackerBot, ref OffenseFightBlocking pAttackerFightBlocking, Bot pDefenderBot, bool pIsSturdyBot)
        {
            //Iterates all OffenseBlockings from the attacking bot
            for (int i = 0; i < pAttackerFightBlocking.offenseBlocking.Length; ++i)
            {
                //Check if the current attacking bot is the player
                if (!GetIsPlayerBlockingSequence(ref pDefendingOffenseFightBlocking, pAttackerBot, pAttackerFightBlocking.offenseBlocking[i], pDefenderBot, pIsSturdyBot)) {

                    //Checks if the defendant's OffenseFightBlocking is not in blocking mode
                    if (!GetIsEnnemyBlockingSequence(ref pDefendingOffenseFightBlocking, pDefenderBot, pAttackerFightBlocking.offenseBlocking[0].GetDeflectionOffense, pAttackerBot, pAttackerFightBlocking.offenseBlocking[i]))
                        return true;
                }
            }

            return false;

        }

        /// <summary>
        /// Checks if the defending bot should enter its hit sequence
        /// </summary>
        /// <param name="pDefenderFightBlocking">Defendant bot’s OffenseFightBlocking</param>
        /// <param name="pAttackerFightBlocking">OffenseFightBlocking of the attacking bot</param>
        /// <param name="pAttackerBot">The attacking bot</param>
        /// <returns>Returns the state of the defending bot's hit sequence</returns>
        bool GetIsHitting(ref OffenseFightBlocking pDefenderFightBlocking, OffenseFightBlocking pAttackerFightBlocking, Bot pAttackerBot)
        {
            //Iterates through the attacking bot's OffenseBlocking list.
            for (int i = 0; i < pAttackerFightBlocking.offenseBlocking.Length; ++i)
            {
                //Check if the current OffenseBlocking is valid
                if (!pAttackerFightBlocking.offenseBlocking[i])
                    continue;

                //Checks if the attacker's OffenseBlocking can hit the defending bot
                if (!pAttackerFightBlocking.offenseBlocking[i].GetIsHitting(pAttackerBot.GetOffenseManager.GetCurrentOffense(), pAttackerBot.GetAnimator))
                    continue;

                base.ToogleState(ref pDefenderFightBlocking.isHitting, false);

                return true;
            }

            base.ToogleState(ref pDefenderFightBlocking.isHitting, false);

            return false;
        }

        /// <summary>
        /// Checks if the attacking bot's OffenseBlocking list has been reset
        /// </summary>
        /// <param name="pAttackerOffenseFightBlocking">OffenseFightBlocking of the attacking bot</param>
        /// <param name="pAttackerBot">The attacking bot</param>
        /// <param name="pIsSturdyBot">The Defendant Bot</param>
        /// <returns>Returns the status of the offensive bot's OffenseBlocking list reset</returns>
        bool GetIsOffenseBlocking(ref OffenseFightBlocking pAttackerOffenseFightBlocking, Bot pAttackerBot, bool pIsSturdyBot)
        {
            //Checks if the attacking bot's current Offense direction is Stance type and assigns the list if so
            if (!pAttackerBot.GetOffenseManager.GetIsStance())
                _main.GetOffenseBlockingConfig.OffenseBlockingSetup(pAttackerBot.GetOffenseManager.GetCurrentOffense(), ref pAttackerOffenseFightBlocking.offenseBlocking, pIsSturdyBot);

            //Clears the attacking bot's OffenseBlocking list if the bot's current Offense is not of Stance type
            else if (pAttackerOffenseFightBlocking.offenseBlocking.Length != 0)
                pAttackerOffenseFightBlocking.offenseBlocking = new OffenseBlocking[0];

            //Checks if the OffenseBlocking list of the attacking bot is not empty
            if (pAttackerOffenseFightBlocking.offenseBlocking.Length != 0)
                return true;

            return false;
        }

        /// <summary>
        /// Checks the status of the player's blocking sequence
        /// </summary>
        /// <param name="pDefendingOffenseFightBlocking">Defendant bot’s OffenseFightBlocking</param>
        /// <param name="pAttackerBot">The attacking bot</param>
        /// <param name="pAttackerOffenseBlocking">OffenseFightBlocking of the attacking bot</param>
        /// <param name="pDefenderBot">The Defendant Bot</param>
        /// <param name="pIsPlayer">If the defending bot is the player</param>
        /// <returns>Returns the status of the player's blocking sequence</returns>
        bool GetIsPlayerBlockingSequence(ref OffenseFightBlocking pDefendingOffenseFightBlocking, Bot pAttackerBot, OffenseBlocking pAttackerOffenseBlocking, Bot pDefenderBot, bool pIsPlayer) {

            if (!pIsPlayer)
                return false;

            //Checks if the current focus is that of the attacking bot and returns the blocking status if this is the case.
            if (_focusModule.GetCurrentFocus == pAttackerBot.transform)
                return GetIsBlockingState(ref pDefendingOffenseFightBlocking, pAttackerOffenseBlocking, pAttackerBot.GetOffenseManager.GetCurrentOffense(), pAttackerBot.GetAnimator, pDefenderBot.GetOffenseManager.GetCurrentOffense());

            return false;
        }

        /// <summary>
        /// Checks the status of the ennemy bot blocking sequence
        /// </summary>
        /// <param name="pDefendingOffenseFightBlocking">Defendant bot’s OffenseFightBlocking</param>
        /// <param name="pAttackerBot">The attacking bot</param>
        /// <param name="pAttackerOffenseBlocking">OffenseFightBlocking of the attacking bot</param>
        /// <param name="pDefenderBot">The Defendant Bot</param>
        /// <returns>Returns the status of the ennemy bot blocking sequence</returns>
        bool GetIsEnnemyBlockingSequence(ref OffenseFightBlocking pDefendingOffenseFightBlocking, Bot pDefenderBot, Offense.Offense pDeflectionOffense, Bot pAttackerBot, OffenseBlocking pAttackerOffenseBlocking) {

            if (pDefendingOffenseFightBlocking.isBlocking)
                return false;

            //Checks if the defending bot can enter its blocking sequence
            if (!GetIsBlockingState(ref pDefendingOffenseFightBlocking, pAttackerOffenseBlocking, pAttackerBot.GetOffenseManager.GetCurrentOffense(), pAttackerBot.GetAnimator, pDefenderBot.GetOffenseManager.GetCurrentOffense()))
                return false;

            //Checks if the enemy bot has a chance to block the player's offense
            if (!pDefendingOffenseFightBlocking.isHaveChanceToBlock)
            {
                pDefendingOffenseFightBlocking.isHaveChanceToBlock = true;

                //Checks if the block percentage is within the range of the defending enemy bot
                if (pDefenderBot) {

                    if (_random.Next(1, 101) <= 100f * (pDefenderBot as MonsterBot).GetBlockingChance)
                    {
                        //Assigns the correct blocking animation of the defending enemy bot based on the player's attacking offense
                        pDefenderBot.GetOffenseManager.SetAnimation(pDefenderBot.GetAnimator, pDeflectionOffense.GetOffenseDirection, pDeflectionOffense.GetOffenseType, pDefenderBot.GetOffenseManager.GetIsStanceOffense, true);

                        //Disables the blocking sequence state of the defending enemy bot
                        ToogleState(ref pDefendingOffenseFightBlocking.isBlocking, true);

                        return true;
                    }
                }
            }

            //Enables hit sequence state
            pDefendingOffenseFightBlocking.isHitting = true;

            return false;
        }

        /// <summary>
        /// Checks if the attacking bot's OffenseBlocking has been reset
        /// </summary>
        /// <param name="pAttackerBot">The attacking bot</param>
        /// <param name="pAttackerFightBlocking">OffenseFightBlocking of the attacking bot</param>
        /// <param name="pDefenderFightBlocking">Defendant bot’s OffenseFightBlocking</param>
        /// <param name="pIsPlayer">If is the player</param>
        /// <returns>Returns the state if the attacking bot's OffenseBlocking list has been reset</returns>
        bool GetIsAttackingOffenseBlocking(Bot pAttackerBot, ref OffenseFightBlocking pAttackerFightBlocking, ref OffenseFightBlocking pDefenderFightBlocking, bool pIsPlayer) {

            if (!GetIsOffenseBlocking(ref pAttackerFightBlocking, pAttackerBot, pIsPlayer)) {

                //Deactivate blocking state on FightBlocking of defending bot
                base.ToogleState(ref pDefenderFightBlocking.isBlocking, false);

                //Deactivate hitting state on FightBlocking of defending bot
                base.ToogleState(ref pDefenderFightBlocking.isHitting, false);

                //Check if the current bot is the player
                if (!pIsPlayer)
                {
                    //Assign default instanciateID on enemmy OffenseBlocking
                    if (_offenseMonsterBotBlocking.instanciateID != -1)
                        _offenseMonsterBotBlocking.instanciateID = -1;

                    //Assign default isHaveChanceToBlock state on enemmy OffenseBlocking
                    if (_offenseMonsterBotBlocking.isHaveChanceToBlock)
                        _offenseMonsterBotBlocking.isHaveChanceToBlock = false;
                }

                return pIsPlayer;
            }

            return true;
        }

        public bool GetIsHitConfirm => _offenseMonsterBotBlocking.isHitting;

        #endregion

        #region Method

        public override void Initialize()
        {
            base.Initialize();

            _main = _sturdyComponent.GetComponent<Main>();

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

            if (_fightOffenseSequence.Length != 0)
                _fightOffenseSequence = new FightOffenseSequence[0];

            base.ToogleState(ref _isHitConfirm, false);

            for (int i = 0; i < _main.GetMonsterBot.Length; ++i) {

                //MonsterBot to SturdyBot
                OffenseBlockingSetup(_main.GetMonsterBot[i], ref _offenseMonsterBotBlocking, _main.GetSturdyBot, ref _offenseSturdyBotBlocking, true);

                //SturdyBot to MonsterBot
                OffenseBlockingSetup(_main.GetSturdyBot, ref _offenseSturdyBotBlocking, _main.GetMonsterBot[i], ref _offenseMonsterBotBlocking);
            }

            return true;
        }

        /// <summary>
        /// Manages player and enemy bot OffenseBlocking sequences
        /// </summary>
        /// <param name="pAttackerBot">The attacking bot</param>
        /// <param name="pDefenderBot">The Defendant Bot</param>
        /// <param name="pAttackerFightBlocking">OffenseFightBlocking of the attacking bot</param>
        /// <param name="pDefenderFightBlocking">Defendant bot’s OffenseFightBlocking</param>
        /// <param name="pIsPlayer">If is the player</param>
        void OffenseBlockingSetup(Bot pAttackerBot, ref OffenseFightBlocking pAttackerFightBlocking, Bot pDefenderBot, ref OffenseFightBlocking pDefenderFightBlocking, bool pIsPlayer = false)
        {
            //Checks if the attacking bot's OffenseBlocking list has been reset
            if (GetIsAttackingOffenseBlocking(pAttackerBot, ref pAttackerFightBlocking, ref pDefenderFightBlocking, pIsPlayer)) {

                if (!GetIsBlocking(ref pDefenderFightBlocking, pAttackerBot, ref pAttackerFightBlocking, pDefenderBot, pIsPlayer))
                    GetIsHitting(ref pDefenderFightBlocking, pAttackerFightBlocking, pAttackerBot);
                else
                    base.ToogleState(ref pDefenderFightBlocking.isHitting, true);

                if (!pIsPlayer)
                {
                    if (_offenseMonsterBotBlocking.instanciateID != pDefenderBot.transform.GetInstanceID())
                        _offenseMonsterBotBlocking.instanciateID = pDefenderBot.transform.GetInstanceID();
                }

                if (pDefenderFightBlocking.isBlocking || pDefenderFightBlocking.isHitting)
                    _isHitConfirm = true;
            }
        }

        #endregion
    }

#if UNITY_EDITOR

    [CustomPropertyDrawer(typeof(FightModule))]
    public partial class FightModuleDrawer : FeatureModuleDrawer{
        public override bool OnNUI(Rect position, SerializedProperty property, GUIContent label){
            if (!base.OnNUI(position, property, label))
                return false;

            drawer.ReorderableList("_fightOffenseSequence");

            drawer.EndProperty();
            return true;
        }
    }

    [CustomPropertyDrawer(typeof(FightOffenseSequence))]
    public partial class FightOffenseSequenceDrawer : ComponentNUIPropertyDrawer
    {
        public override bool OnNUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (!base.OnNUI(position, property, label))
                return false;

            if (drawer.Field("ennemiBot").objectReferenceValue)
                drawer.ReorderableList("fightOffenseSequenceData");

            drawer.EndProperty();
            return true;
        }
    }

    [CustomPropertyDrawer(typeof(FightOffenseSequenceData))]
    public partial class FightOffenseSequenceDataDrawer : ComponentNUIPropertyDrawer
    {
        public override bool OnNUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (!base.OnNUI(position, property, label))
                return false;

            if (drawer.Field("offensePaternType").enumValueIndex != 0) {

                drawer.Field("addHitConfirm");
                drawer.Field("addBlockingChance");

                drawer.Property("fightPaternTypeData");

                drawer.ReorderableList("offenseSubSequenceData");

            }

            drawer.EndProperty();
            return true;
        }
    }

    [CustomPropertyDrawer(typeof(OffenseSubSequenceData))]
    public partial class OffenseSubSequenceDataDrawer : ComponentNUIPropertyDrawer{
        public override bool OnNUI(Rect position, SerializedProperty property, GUIContent label){
            if (!base.OnNUI(position, property, label))
                return false;

            //StanceTimer
            if (drawer.Field("offenseDirection", true, null, "Direction: ").enumValueIndex == 4)
                drawer.Field("stanceTimer");

            drawer.Field("offenseType", true, null, "Type: ");

            drawer.EndProperty();
            return true;
        }
    }

    [CustomPropertyDrawer(typeof(FightPaternTypeData))]
    public partial class FightPaternTypeDataDrawer : ComponentNUIPropertyDrawer
    {
        public override bool OnNUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (!base.OnNUI(position, property, label))
                return false;

            if (drawer.Field("paternType").enumValueIndex != 0)
                drawer.Field("specificIndex");

            drawer.EndProperty();
            return true;
        }
    }

    [CustomPropertyDrawer(typeof(BlockingChanceData))]
    public partial class BlockingChanceDataDrawer : ComponentNUIPropertyDrawer{
        public override bool OnNUI(Rect position, SerializedProperty property, GUIContent label){
            if (!base.OnNUI(position, property, label))
                return false;

            if (drawer.Field("isActivated").boolValue)
                drawer.Field("delay");

            drawer.EndProperty();
            return true;
        }
    }

#endif
}