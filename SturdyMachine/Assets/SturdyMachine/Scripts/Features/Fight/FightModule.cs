using System;

using UnityEngine;

using SturdyMachine.Offense;
using SturdyMachine.Offense.Blocking;
using SturdyMachine.Features.Focus;
using SturdyMachine.Component;
using SturdyMachine.Inputs;
using SturdyMachine.Features.HitConfirm;

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
        FocusModule _focusModule;

        /// <summary>
        /// The combat information for SturdyBot
        /// </summary>
        OffenseFightBlocking _offenseSturdyBotBlocking;

        /// <summary>
        /// The combat information for MonsterBot
        /// </summary>
        OffenseFightBlocking _offenseEnnemyBotBlocking;

        System.Random _random;

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
        public OffenseFightBlocking GetOffenseMonsterBotBlocking => _offenseEnnemyBotBlocking;

        /// <summary>
        /// Checks if the defending bot enters its blocking sequence
        /// </summary>
        /// <param name="pOffenseFightBlockingDefender">Defendant's FightBlocking Offense</param>
        /// <param name="pAttackerOffenseBlocking">The attacker's OffenseBlocking.</param>
        /// <param name="pAttackerBot">The attacking bot</param>
        /// <param name="pDefenderBot">The Defendant Bot</param>
        /// <param name="pMonsterOffense">Opponent's Offense</param>
        /// <returns>Returns if the defending bot enters its blocking sequence</returns>
        bool GetIsBlockingState(BotData pAttackerBotData, OffenseBlocking pAttackerOffenseBlocking, ref OffenseFightBlocking pOffenseFightBlockingDefender, Offense.Offense pEnnemyBotOffense, bool pIsPlayer)
        {
            //Checks if the defending bot is in blocking mode
            if (!pOffenseFightBlockingDefender.isBlocking)
            {
                //Checks if the OffenseBlocking can block the attacking offense
                if (pAttackerOffenseBlocking.GetIsBlocking(pEnnemyBotOffense ? pEnnemyBotOffense
                                                                             : _sturdyBotData.offenseManager.GetCurrentOffense(),
                                                           pAttackerBotData.offenseManager.GetCurrentOffense(), pAttackerBotData.animator, pIsPlayer))
                {
                    //Disables the blocked state if the defending bot is the player
                    if (!pEnnemyBotOffense)
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
        bool GetIsBlocking(BotData pAttackerBotData, ref OffenseFightBlocking pAttackerFightBlocking, BotData pDefenderBotData, ref OffenseFightBlocking pDefendingOffenseFightBlocking, bool pIsSturdyBot)
        {
            //Iterates all OffenseBlockings from the attacking bot
            for (int i = 0; i < pAttackerFightBlocking.offenseBlocking.Length; ++i)
            {
                //Check if the current attacking bot is the player
                if (pIsSturdyBot)
                    return !GetIsPlayerBlockingSequence(pAttackerBotData, pAttackerFightBlocking.offenseBlocking[i], pDefenderBotData, ref pDefendingOffenseFightBlocking, pDefenderBotData.offenseManager.GetCurrentOffense(), pIsSturdyBot);
                
                return GetIsEnnemyBlockingSequence(pDefenderBotData, ref pDefendingOffenseFightBlocking, pAttackerBotData, pAttackerFightBlocking.offenseBlocking[0].GetDeflectionOffense, pAttackerFightBlocking.offenseBlocking[i], pIsSturdyBot);
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
        bool GetIsHitting(ref OffenseFightBlocking pDefenderFightBlocking, OffenseFightBlocking pAttackerFightBlocking, BotData pAttackerBotData)
        {
            //Iterates through the attacking bot's OffenseBlocking list.
            for (int i = 0; i < pAttackerFightBlocking.offenseBlocking.Length; ++i)
            {
                //Check if the current OffenseBlocking is valid
                if (!pAttackerFightBlocking.offenseBlocking[i])
                    continue;

                //Checks if the attacker's OffenseBlocking can hit the defending bot
                if (!pAttackerFightBlocking.offenseBlocking[i].GetIsHitting(pAttackerBotData.offenseManager.GetCurrentOffense(), pAttackerBotData.animator))
                    continue;

                return true;
            }

            return false;
        }

        /// <summary>
        /// Checks if the attacking bot's OffenseBlocking list has been reset
        /// </summary>
        /// <param name="pAttackerOffenseFightBlocking">OffenseFightBlocking of the attacking bot</param>
        /// <param name="pAttackerBot">The attacking bot</param>
        /// <param name="pIsSturdyBot">The Defendant Bot</param>
        /// <returns>Returns the status of the offensive bot's OffenseBlocking list reset</returns>
        bool GetIsOffenseBlocking(ref OffenseFightBlocking pAttackerOffenseFightBlocking, OffenseManager pOffenseManagerAttackerBot, bool pIsSturdyBot)
        {
            //Checks if the attacking bot's current Offense direction is Stance type and assigns the list if so
            if (!pOffenseManagerAttackerBot.GetIsStance())
                _offenseBlockingConfig.OffenseBlockingSetup(pOffenseManagerAttackerBot.GetCurrentOffense(), ref pAttackerOffenseFightBlocking.offenseBlocking, pIsSturdyBot);

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
        bool GetIsPlayerBlockingSequence(BotData pAttackerBotData, OffenseBlocking pAttackerOffenseBlocking, BotData pDefenderBotBot, ref OffenseFightBlocking pDefendingOffenseFightBlocking, Offense.Offense pDefenderBotOffense, bool pIsPlayer) {

            if (!pIsPlayer)
                return false;

            //Checks if the current focus is that of the attacking bot and returns the blocking status if this is the case.
            if (_focusModule.GetCurrentFocus == pDefenderBotBot.botObject.transform)
                return !GetIsBlockingState(pAttackerBotData, pAttackerOffenseBlocking, ref pDefendingOffenseFightBlocking, pDefenderBotOffense, pIsPlayer);

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
        bool GetIsEnnemyBlockingSequence(BotData pDefenderBotData, ref OffenseFightBlocking pDefendingOffenseFightBlocking, BotData pAttackerBotData, Offense.Offense pDeflectionOffense, OffenseBlocking pAttackerOffenseBlocking, bool pIsPlayer) {

            if (pDefendingOffenseFightBlocking.isBlocking)
                return true;

            //Checks if the defending bot can enter its blocking sequence
            if (!GetIsBlockingState(pAttackerBotData, pAttackerOffenseBlocking, ref pDefendingOffenseFightBlocking, pDefenderBotData.offenseManager.GetCurrentOffense(), pIsPlayer))
                return false;

            //Checks if the enemy bot has a chance to block the player's offense
            if (!pDefendingOffenseFightBlocking.isHaveChanceToBlock)
            {
                pDefendingOffenseFightBlocking.isHaveChanceToBlock = true;

                //Checks if the block percentage is within the range of the defending enemy bot
                if (!pIsPlayer) {

                    if (_random.Next(1, 101) <= 100f * _focusModule.GetCurrentEnnemyBotData.blockingChance)
                    {
                        //Assigns the correct blocking animation of the defending enemy bot based on the player's attacking offense
                        pDefenderBotData.offenseManager.SetAnimation(pDefenderBotData.animator, pDeflectionOffense.GetOffenseDirection, pDeflectionOffense.GetOffenseType, pDefenderBotData.offenseManager.GetIsStanceOffense, true);

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
        bool GetIsAttackingOffenseBlocking(OffenseManager pOffenseManagerAttackerBot, ref OffenseFightBlocking pAttackerFightBlocking, ref OffenseFightBlocking pDefenderFightBlocking, bool pIsPlayer) {

            if (!GetIsOffenseBlocking(ref pAttackerFightBlocking, pOffenseManagerAttackerBot, pIsPlayer)) {

                //Deactivate blocking state on FightBlocking of defending bot
                base.ToogleState(ref pDefenderFightBlocking.isBlocking, false);

                //Deactivate hitting state on FightBlocking of defending bot
                base.ToogleState(ref pDefenderFightBlocking.isHitting, false);

                //Check if the current bot is not the player
                if (!pIsPlayer)
                {
                    //Assign default instanciateID on enemmy OffenseBlocking
                    if (_offenseEnnemyBotBlocking.instanciateID != -1)
                        _offenseEnnemyBotBlocking.instanciateID = -1;

                    //Assign default isHaveChanceToBlock state on enemmy OffenseBlocking
                    if (_offenseEnnemyBotBlocking.isHaveChanceToBlock)
                        _offenseEnnemyBotBlocking.isHaveChanceToBlock = false;
                }

                return pIsPlayer;
            }

            return true;
        }

        /// <summary>
        /// Returns the state if the bot does a hitConfirm
        /// </summary>
        /// <param name="pOffensFightBlocking">Bot FightBlockingData</param>
        /// <returns>Returns if the bot activates hitConfirm</returns>
        bool GetIfBotHitConfirm(OffenseFightBlocking pOffensFightBlocking) {

            //Hitting
            if (pOffensFightBlocking.isHitting)
                return true;

            //Blocking
            if (pOffensFightBlocking.isBlocking)
                return true;

            return false;
        }

        /// <summary>
        /// Checks if one of the bot activates hitConfirm during combat
        /// </summary>
        /// <returns>Returns if one of the two bots activates hitconfirm in the fight</returns>
        public bool GetIsHitConfirm() {

            //EnnemyBot
            if (GetIfBotHitConfirm(_offenseEnnemyBotBlocking))
                return true;

            //SturdyBot
            if (GetIfBotHitConfirm(_offenseSturdyBotBlocking))
                return true;

            return false;
        }

        public Offense.Offense GetCurrentOffenseAttackerBot() {

            //EnnemyBot
            if (_offenseEnnemyBotBlocking.offenseBlocking.Length != 0)
                return _focusModule.GetCurrentEnnemyBotData.offenseManager.GetCurrentOffense();

            //Sturdy
            return _sturdyBotData.offenseManager.GetCurrentOffense();
        }

        public OffenseFightBlocking GetOffenseFightBlockingAttackerBot() {

            //EnnemyBot
            if (_offenseEnnemyBotBlocking.offenseBlocking.Length != 0)
                return _offenseEnnemyBotBlocking;

            //Sturdy
            return _offenseSturdyBotBlocking;
        }

        public HitConfirmOffense GetDefenderHitConfirmOffense() {

            //Sturdy
            if (_offenseSturdyBotBlocking.offenseBlocking.Length != 0)
                return _sturdyBotData.hitConfirmOffenseManager.GetConfirmOffense;

            //EnnemyBot
            return _focusModule.GetCurrentEnnemyBotData.hitConfirmOffenseManager.GetConfirmOffense;
        }

        #endregion

        #region Method

        public override void OnAwake(SturdyComponent pSturdyComponent, BotData[] pEnnemyBotData)
        {
            base.OnAwake(pSturdyComponent, pEnnemyBotData);

            _focusModule = pSturdyComponent.GetComponent<FocusModuleWrapper>().GetFocusModule;
        }

        public override void Initialize(BotData pStrudyBotData, SturdyInputControl pSturdyInputControl, OffenseBlockingConfig pOffenseBlockingConfig)
        {
            base.Initialize(pStrudyBotData, pSturdyInputControl, pOffenseBlockingConfig);

            _offenseEnnemyBotBlocking.offenseBlocking = new OffenseBlocking[0];

            if (_offenseEnnemyBotBlocking.instanciateID != -1)
                _offenseEnnemyBotBlocking.instanciateID = -1;

            _offenseSturdyBotBlocking.offenseBlocking = new OffenseBlocking[0];
        }

        public override bool OnUpdate()
        {
            if (!base.OnUpdate())
                return false;

            if (_fightOffenseSequence.Length != 0)
                _fightOffenseSequence = new FightOffenseSequence[0];

            base.ToogleState(ref _isHitConfirm, false);

            for (byte i = 0; i < _ennemyBotData.Length; ++i) {

                //MonsterBot to SturdyBot
                OffenseBlockingSetup(_ennemyBotData[i], ref _offenseEnnemyBotBlocking, _sturdyBotData, ref _offenseSturdyBotBlocking, false);

                //SturdyBot to MonsterBot
                OffenseBlockingSetup(_sturdyBotData, ref _offenseSturdyBotBlocking, _ennemyBotData[i], ref _offenseEnnemyBotBlocking, true);

                _ennemyBotData[i].hitConfirmOffenseManager.SetCurrentHitConfirmOffense(_ennemyBotData[i].offenseManager.GetCurrentOffense());
            }

            _sturdyBotData.hitConfirmOffenseManager.SetCurrentHitConfirmOffense(_sturdyBotData.offenseManager.GetCurrentOffense());

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
        void OffenseBlockingSetup(BotData pAttackerBotData, ref OffenseFightBlocking pAttackerFightBlocking, BotData pDefenderBotData, ref OffenseFightBlocking pDefenderFightBlocking, bool pIsPlayer)
        {
            //Checks if the attacking bot's OffenseBlocking list has been reset
            if (GetIsAttackingOffenseBlocking(pAttackerBotData.offenseManager, ref pAttackerFightBlocking, ref pDefenderFightBlocking, pIsPlayer)) {

                if (!GetIsBlocking(pAttackerBotData, ref pAttackerFightBlocking, pDefenderBotData, ref pDefenderFightBlocking, pIsPlayer))
                    base.ToogleState(ref pDefenderFightBlocking.isHitting, GetIsHitting(ref pDefenderFightBlocking, pAttackerFightBlocking, pAttackerBotData));

                if (!pIsPlayer)
                {
                    if (_offenseEnnemyBotBlocking.instanciateID != pDefenderBotData.botObject.transform.GetInstanceID())
                        _offenseEnnemyBotBlocking.instanciateID = pDefenderBotData.botObject.transform.GetInstanceID();
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