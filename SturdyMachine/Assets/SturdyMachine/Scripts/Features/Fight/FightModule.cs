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
    public struct FightData 
    {
        public GameObject monsterBot;

        public float waitingTimer;

        public OffenseDirection offenseDirection;
        public OffenseType offenseType;

        public float timer;
    }

    [Serializable]
    public partial class FightModule : FeatureModule 
    {
        [SerializeField]
        FightData[] _fightData;

        Focus.FocusModule _focusModule;

        Offense.Blocking.OffenseBlocking _monsterBotOffenseBlocking, _sturdyBotOffenseBlocking;

        int _currentMonsterBotIndex;

        bool _isHitting, _isBlocking;

        public bool GetIsHitting => _isHitting;
        public bool GetIsBlocking => _isBlocking;

        public FightData[] GetFightData => _fightData;

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

        public override void UpdateRemote(MonsterBot[] pMonsterBot, SturdyBot pSturdyBot, Inputs.SturdyInputControl pSturdyInputControl)
        {
            if (!GetIsActivated)
                return;

            //MonsterBot
            _monsterBotOffenseBlocking = GetMonsterBotOffenseBlocking(pMonsterBot);

            //SturdyBot
            _sturdyBotOffenseBlocking = GetSturdyBotOffenseBlocking(_sturdyBot);

            if (_monsterBotOffenseBlocking)
            {
                _isBlocking = _monsterBotOffenseBlocking.GetIsBlocking(pSturdyBot.GetOffenseManager.GetCurrentOffense(), pMonsterBot[_currentMonsterBotIndex].GetOffenseManager.GetCurrentOffense(), pMonsterBot[_currentMonsterBotIndex].GetAnimator);

                if (!_isBlocking)
                    _isHitting = _monsterBotOffenseBlocking.GetIsHitting(pMonsterBot[_currentMonsterBotIndex].GetOffenseManager.GetCurrentOffense(), pMonsterBot[_currentMonsterBotIndex].GetAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime);
                else if (_isHitting)
                    _isHitting = false;



            }
        }

        public override FeatureModuleCategory GetFeatureModuleCategory()
        {
            return FeatureModuleCategory.Fight;
        }

        public override void FixedUpdate() { }
    }

#if UNITY_EDITOR

    [CustomPropertyDrawer(typeof(FightModule))]
    public partial class FightModuleDrawer : FeatureModuleDrawer
    {
        public override bool OnNUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (!base.OnNUI(position, property, label))
                return false;

            drawer.ReorderableList("_fightData");

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

            drawer.Field("monsterBot");
            drawer.Field("waitingTimer");
            drawer.Field("offenseDirection");
            drawer.Field("offenseType");
            drawer.Field("timer");

            drawer.EndProperty();
            return true;
        }
    }

#endif
}