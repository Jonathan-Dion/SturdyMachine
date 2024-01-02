using System;

using UnityEngine;

using SturdyMachine.Features.Focus;
using SturdyMachine.Features.Fight;

using SturdyMachine.Component;

using SturdyMachine.Offense;

using NWH.VehiclePhysics2;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace SturdyMachine.Features.HitConfirm {

    /// <summary>
    /// Identify the type of HitConfirm
    /// </summary>
    public enum HitConfirmType { None, Normal, Slow, Stop }

    [Serializable]
    public partial class HitConfirmModule : FeatureModule {

        #region Attribut



        #endregion

        #region Get

        public override FeatureModuleCategory GetFeatureModuleCategory() => FeatureModuleCategory.HitConfirm;

        #endregion

        #region Method

        

        #endregion
    }

#if UNITY_EDITOR

    [CustomPropertyDrawer(typeof(HitConfirmModule))]
    public partial class HitConfirmModuleDrawer : FeatureModuleDrawer
    {
        public override bool OnNUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (!base.OnNUI(position, property, label))
                return false;

            

            drawer.EndProperty();
            return true;
        }
    }

#endif
}