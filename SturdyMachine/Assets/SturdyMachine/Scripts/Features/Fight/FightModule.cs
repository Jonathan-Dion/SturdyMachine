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
    /// <summary>
    /// Module managing the fight sequence as well as the combat behavior of a MonsterBot
    /// </summary>
    [Serializable]
    public partial class FightModule : FeatureModule
    {
        #region Attribut

        #endregion

        #region Get

        public override FeatureModuleCategory GetFeatureModuleCategory() => FeatureModuleCategory.Fight;

        #endregion

        #region Method



        #endregion
    }

#if UNITY_EDITOR

    [CustomPropertyDrawer(typeof(FightModule))]
    public partial class FightModuleDrawer : FeatureModuleDrawer{
        public override bool OnNUI(Rect position, SerializedProperty property, GUIContent label){
            if (!base.OnNUI(position, property, label))
                return false;

            drawer.EndProperty();
            return true;
        }
    }

#endif
}