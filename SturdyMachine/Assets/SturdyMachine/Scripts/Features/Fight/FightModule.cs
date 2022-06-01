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

        public OffenseDirection offenseDirection;
        public OffenseType offenseType;

        public float timer;
    }

    [Serializable]
    public partial class FightModule : FeatureModule 
    {
        [SerializeField]
        FightData[] _fightData;

        public FightData[] GetFightData => _fightData;

        public override void UpdateRemote(MonsterBot[] pMonsterBot, SturdyBot pSturdyBot, Inputs.SturdyInputControl pSturdyInputControl)
        {
            if (!GetIsActivated)
                return;
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
            drawer.Field("offenseDirection");
            drawer.Field("offenseType");
            drawer.Field("timer");

            drawer.EndProperty();
            return true;
        }
    }

#endif
}