using System;
using System.Collections.Generic;

using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
using NWH.VehiclePhysics2;
#endif

namespace SturdyMachine.Features.Fight 
{
    [Serializable]
    public struct FightBlocking 
    {
        public List<Offense.Blocking.OffenseBlocking> offenseBlocking;

        public bool isHitting, isBlocking;

        public int instanciateID;
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

        public FightData[] fightData;
    }

    [Serializable]
    public partial class FightModule : FeatureModule
    {
        [SerializeField]
        FightDataGroup[] _fightDataGroup;

        Focus.FocusModule _focusModule;

        FightBlocking _monsterBotFightBlocking, _sturdyBotFightBlocking;

        public FightDataGroup[] GetFightDataGroup => _fightDataGroup;

        public FightBlocking GetSturdyBotFightBlocking => _sturdyBotFightBlocking;
        public FightBlocking GetMonsterBotFightBlocking => _monsterBotFightBlocking;

        bool GetIsOffenseBlocking(ref FightBlocking pFightBlocking, Bot pAttackerBot)
        {
            if (!pAttackerBot.GetOffenseManager.GetIsDefaultStance())
                _main.GetOffenseBlockingConfig.OffenseBlockingSetup(pAttackerBot.GetOffenseManager.GetCurrentOffense(), pFightBlocking.offenseBlocking);
            else if (pFightBlocking.offenseBlocking.Count != 0)
                pFightBlocking.offenseBlocking.Clear();

            if (pFightBlocking.offenseBlocking.Count != 0)
                return true;

            return false;
        }

        void OffenseBlockingSetup(Bot pAttackerBot, ref FightBlocking pAttackerFightBlocking, ref FightBlocking pFightBlocking, bool pIsSturdyBot = false) 
        {
            if (!GetIsOffenseBlocking(ref pAttackerFightBlocking, pAttackerBot)) 
            {
                ToogleState(ref pFightBlocking.isBlocking);
                ToogleState(ref pFightBlocking.isHitting);
            }
                
            else
            {
                if (!GetIsBlocking(ref pFightBlocking, pAttackerBot, pIsSturdyBot))
                    GetIsHitting(ref pFightBlocking, pAttackerFightBlocking, pAttackerBot);
                else
                    ToogleState(ref pFightBlocking.isHitting, true);
            }
        }

        bool GetIsBlocking(ref FightBlocking pFightBlocking, Bot pAttackerBot, bool pIsSturdyBot = false) 
        {
            if (pIsSturdyBot)
            {
                if (_focusModule.GetCurrentFocus == pAttackerBot.transform)
                {
                    if (!pFightBlocking.isBlocking) 
                    {
                        for (int i = 0; i < _monsterBotFightBlocking.offenseBlocking.Count; ++i)
                        {
                            if (_monsterBotFightBlocking.offenseBlocking[i].GetIsBlocking(_sturdyBot.GetOffenseManager.GetCurrentOffense(), pAttackerBot.GetOffenseManager.GetCurrentOffense(), pAttackerBot.GetAnimator))
                            {
                                ToogleState(ref pFightBlocking.isBlocking, false);

                                return true;
                            }
                        }
                    }
                    else
                        return true;
                }
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

            base.Initialize(pMonsterBot, pSturdyBot);
        }

        public override void UpdateRemote(MonsterBot[] pMonsterBot, SturdyBot pSturdyBot, Inputs.SturdyInputControl pSturdyInputControl)
        {
            if (!GetIsActivated)
                return;

            for (int i = 0; i < pMonsterBot.Length; ++i)
                OffenseBlockingSetup(pMonsterBot[i], ref _monsterBotFightBlocking, ref _sturdyBotFightBlocking, true);
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
                drawer.ReorderableList("fightData");

            drawer.EndProperty();
            return true;
        }
    }

#endif
}