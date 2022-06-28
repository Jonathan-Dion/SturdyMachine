using System;
using System.Collections.Generic;

using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
using NWH.VehiclePhysics2;
#endif

namespace SturdyMachine.Features.Fight 
{
    public enum FightPaternType { DEFAULT, RANDOM, SPECIFIC };

    [Serializable]
    public struct FightPatern 
    {
        public FightPaternType paternType;

        public int paternIndex;

        public float blockingChanceAdd;
    }

    [Serializable]
    public struct FightBlocking 
    {
        public List<Offense.Blocking.OffenseBlocking> offenseBlocking;

        public bool isHitting, isBlocking;

        public int instanciateID;

        public bool isHaveChance;
    }

    [Serializable]
    public struct FightOffenseData
    {
        public bool isWaiting;

        public float waitingBegin, waitingEnd;

        public OffenseDirection offenseDirection;
        public OffenseType offenseType;

        public float timer;
    }

    [Serializable]
    public struct FightData 
    {
        public OffenseBlockType offenseBlockType;

        public FightOffenseData[] fightOffenseData;
    }

    [Serializable]
    public struct FightDataGroup 
    {
        public GameObject monsterBot;

        public List<FightPatern> fightPatern;

        public FightData[] fightData;
    }

    [Serializable]
    public partial class FightModule : FeatureModule
    {
        [SerializeField]
        FightDataGroup[] _fightDataGroup;

        Focus.FocusModule _focusModule;

        FightBlocking _monsterBotFightBlocking, _sturdyBotFightBlocking;

        System.Random _random;

        public FightDataGroup[] GetFightDataGroup => _fightDataGroup;

        public FightBlocking GetSturdyBotFightBlocking => _sturdyBotFightBlocking;
        public FightBlocking GetMonsterBotFightBlocking => _monsterBotFightBlocking;

        bool GetIsOffenseBlocking(ref FightBlocking pFightBlocking, Bot pAttackerBot, bool pIsSturdyBot)
        {
            if (!pAttackerBot.GetOffenseManager.GetIsStance())
                _main.GetOffenseBlockingConfig.OffenseBlockingSetup(pAttackerBot.GetOffenseManager.GetCurrentOffense(), pFightBlocking.offenseBlocking, pIsSturdyBot);
            else if (pFightBlocking.offenseBlocking.Count != 0)
                pFightBlocking.offenseBlocking.Clear();

            if (pFightBlocking.offenseBlocking.Count != 0)
                return true;

            return false;
        }

        void OffenseBlockingSetup(Bot pAttackerBot, Bot pDefenderBot, ref FightBlocking pAttackerFightBlocking, ref FightBlocking pFightBlocking, bool pIsSturdyBot = false) 
        {
            if (!GetIsOffenseBlocking(ref pAttackerFightBlocking, pAttackerBot, pIsSturdyBot)) 
            {
                ToogleState(ref pFightBlocking.isBlocking);
                ToogleState(ref pFightBlocking.isHitting);

                if (!pIsSturdyBot) 
                {
                    if (_monsterBotFightBlocking.instanciateID != -1)
                        _monsterBotFightBlocking.instanciateID = -1;

                    if (_monsterBotFightBlocking.isHaveChance)
                        _monsterBotFightBlocking.isHaveChance = false;
                }
            }
                
            else
            {
                if (!GetIsBlocking(ref pFightBlocking, pAttackerBot, ref pAttackerFightBlocking ,pDefenderBot, pIsSturdyBot))
                    GetIsHitting(ref pFightBlocking, pAttackerFightBlocking, pAttackerBot);
                else
                    ToogleState(ref pFightBlocking.isHitting, true);

                if (!pIsSturdyBot) 
                {
                    if (_monsterBotFightBlocking.instanciateID != pDefenderBot.transform.GetInstanceID())
                        _monsterBotFightBlocking.instanciateID = pDefenderBot.transform.GetInstanceID();
                }
            }
        }

        bool BlockingSetup(ref FightBlocking pFightBlocking, Offense.Blocking.OffenseBlocking pAttackerOffenseBlocking, Bot pAttackerBot, Bot pDefenderBot, Offense.Offense pMonsterDeflectionOffense = null) 
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
        }

        bool GetIsBlocking(ref FightBlocking pFightBlocking, Bot pAttackerBot, ref FightBlocking pAttackerFightBlocking, Bot pDefenderBot, bool pIsSturdyBot) 
        {
            for (int i = 0; i < pAttackerFightBlocking.offenseBlocking.Count; ++i)
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

        }

        bool GetIsHitting(ref FightBlocking pFightBlocking, FightBlocking pAttackerFightBlocking, Bot pAttackerBot) 
        {
            for (int i = 0; i < pAttackerFightBlocking.offenseBlocking.Count; ++i) 
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
        }

        public override void Initialize(MonsterBot[] pMonsterBot, SturdyBot pSturdyBot)
        {
            _focusModule = _main.GetComponent<Focus.FocusModuleWrapper>().GetFeatureModule() as Focus.FocusModule;

            _monsterBotFightBlocking.offenseBlocking = new List<Offense.Blocking.OffenseBlocking>();

            _sturdyBotFightBlocking.offenseBlocking = new List<Offense.Blocking.OffenseBlocking>();

            _random = new System.Random();

            if (_monsterBotFightBlocking.instanciateID != -1)
                _monsterBotFightBlocking.instanciateID = -1;

            base.Initialize(pMonsterBot, pSturdyBot);
        }

        public override void UpdateRemote(MonsterBot[] pMonsterBot, SturdyBot pSturdyBot, Inputs.SturdyInputControl pSturdyInputControl)
        {
            if (!GetIsActivated)
                return;

            for (int i = 0; i < pMonsterBot.Length; ++i) 
            {
                //MonsterBot to SturdyBot
                OffenseBlockingSetup(pMonsterBot[i], null, ref _monsterBotFightBlocking, ref _sturdyBotFightBlocking, true);

                //SturdyBot to MonsterBot
                OffenseBlockingSetup(_sturdyBot, pMonsterBot[i], ref _sturdyBotFightBlocking, ref _monsterBotFightBlocking);
            }
        }

        public override FeatureModuleCategory GetFeatureModuleCategory()
        {
            return FeatureModuleCategory.Fight;
        }

        public override void FixedUpdate() { }

        public override void CleanMemory()
        {
            _fightDataGroup = null;
        }
    }

#if UNITY_EDITOR

    [CustomPropertyDrawer(typeof(FightModule))]
    public partial class FightModuleDrawer : FeatureModuleDrawer
    {
        public override bool OnNUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (!base.OnNUI(position, property, label))
                return false;

            drawer.ReorderableList("_fightDataGroup");

            drawer.EndProperty();
            return true;
        }
    }

    [CustomPropertyDrawer(typeof(FightOffenseData))]
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
    }

    [CustomPropertyDrawer(typeof(FightData))]
    public partial class FightDataDrawer : ComponentNUIPropertyDrawer
    {
        public override bool OnNUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (!base.OnNUI(position, property, label))
                return false;

            if (drawer.Field("offenseBlockType", true, null, "Offense patern: ").enumValueIndex != 0)
                drawer.ReorderableList("fightOffenseData");

            drawer.EndProperty();
            return true;
        }
    }

    [CustomPropertyDrawer(typeof(FightDataGroup))]
    public partial class FightDataGroupDrawer : ComponentNUIPropertyDrawer
    {
        public override bool OnNUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (!base.OnNUI(position, property, label))
                return false;

            if (drawer.Field("monsterBot").objectReferenceValue != null) 
            {
                drawer.BeginSubsection("Patern");

                drawer.ReorderableList("fightPatern");

                drawer.EndSubsection();

                drawer.BeginSubsection("Fight Data");

                drawer.ReorderableList("fightData");

                drawer.EndSubsection();
            }

            drawer.EndProperty();
            return true;
        }
    }

    [CustomPropertyDrawer(typeof(FightPatern))]
    public partial class FightPaternDrawer : ComponentNUIPropertyDrawer
    {
        public override bool OnNUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (!base.OnNUI(position, property, label))
                return false;

            if (drawer.Field("paternType").enumValueIndex == 2)
                drawer.Field("paternIndex");

            drawer.Field("blockingChanceAdd");


            drawer.EndProperty();
            return true;
        }
    }


#endif
}