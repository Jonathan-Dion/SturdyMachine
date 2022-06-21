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

        //MonsterBot
        Offense.Blocking.OffenseBlocking _monsterBotOffenseBlocking;

        //SturdyBot
        Offense.Blocking.OffenseBlocking _sturdyBotOffenseBlocking;

        int _currentMonsterBotIndex;

        public FightDataGroup[] GetFightDataGroup => _fightDataGroup;

        Offense.Blocking.OffenseBlocking GetMonsterBotOffenseBlocking(MonsterBot[] pMonsterBot)
        {
            for (int i = 0; i < pMonsterBot.Length; ++i)
            {
                if (pMonsterBot[i].GetOffenseManager.GetCurrentOffense().GetOffenseDirection != OffenseDirection.STANCE)
                {
                    if (pMonsterBot[i].GetOffenseManager.GetCurrentOffense().GetOffenseType != OffenseType.DEFAULT)
                    {
                        _currentMonsterBotIndex = i;

                        return _main.GetOffenseBlockingConfig.GetOffenseBlocking(pMonsterBot[i].GetOffenseManager.GetCurrentOffense());
                    }
                }
            }

            return null;
        }

        Offense.Blocking.OffenseBlocking GetSturdyBotOffenseBlocking(SturdyBot pSturdyBot)
        {
            if (pSturdyBot.GetOffenseManager.GetCurrentOffense())
            {
                if (pSturdyBot.GetOffenseManager.GetCurrentOffense().GetOffenseDirection != OffenseDirection.STANCE)
                    return _main.GetOffenseBlockingConfig.GetOffenseBlocking(_sturdyBot.GetOffenseManager.GetCurrentOffense());
            }

            return null;
        }

        public override void Initialize(MonsterBot[] pMonsterBot, SturdyBot pSturdyBot)
        {
            _focusModule = _main.GetComponent<Focus.FocusModuleWrapper>().GetFeatureModule() as Focus.FocusModule;

            base.Initialize(pMonsterBot, pSturdyBot);
        }

        void MonsterOffenseBlockingSetup(ref Offense.Blocking.OffenseBlocking pMonsterBotOffenseBlocking, MonsterBot[] pMonsterBot) 
        {
            //MonsterBot
            pMonsterBotOffenseBlocking = GetMonsterBotOffenseBlocking(pMonsterBot);


        }

        void FightSetup() 
        {
            
        }

        void BlockingSetup() 
        {
            
        }

        public override void UpdateRemote(MonsterBot[] pMonsterBot, SturdyBot pSturdyBot, Inputs.SturdyInputControl pSturdyInputControl)
        {
            if (!GetIsActivated)
                return;


            //SturdyBot
            _sturdyBotOffenseBlocking = GetSturdyBotOffenseBlocking(_sturdyBot);

            if (!_monsterBotOffenseBlocking) 
            {
                if (_isHitting)
                    _isHitting = false;
            }
            else
            {
                if (_isBlocking)
                    _isBlocking = false;

                if (_focusModule.GetCurrentFocus == pMonsterBot[_currentMonsterBotIndex].transform)
                {
                    if (_monsterBotOffenseBlocking.GetIsBlocking(pSturdyBot.GetOffenseManager.GetCurrentOffense(), pMonsterBot[_currentMonsterBotIndex].GetOffenseManager.GetCurrentOffense(), pMonsterBot[_currentMonsterBotIndex].GetAnimator))
                    {
                        if (!_isBlocking)
                            _isBlocking = true;
                    }
                }

                if (!_isBlocking)
                {
                    for (int i = 0; i < pMonsterBot.Length; ++i)
                    {
                        if (_monsterBotOffenseBlocking.GetIsHitting(pMonsterBot[i].GetOffenseManager.GetCurrentOffense(), pMonsterBot[i].GetAnimator))
                        {
                            if (!_isHitting)
                            {
                                _isHitting = true;

                                return;
                            }
                        }
                    }
                }
                else if (_isHitting)
                    _isHitting = false;
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
                drawer.ReorderableList("fightData");

            drawer.EndProperty();
            return true;
        }
    }

#endif
}