using System;

using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
using NWH.VehiclePhysics2;
#endif

namespace SturdyMachine.Features.Audio {

    /// <summary>
    /// Module managing the fight sequence as well as the combat behavior of a MonsterBot
    /// </summary>
    [Serializable]
    public partial class AudioModule : FeatureModule{
        
        #region Attributes

        

        #endregion

        #region Properties

        public override FeatureModuleCategory GetFeatureModuleCategory() => FeatureModuleCategory.Audio;

        #endregion

        #region Methods



        #endregion
    }

#if UNITY_EDITOR

    [CustomPropertyDrawer(typeof(AudioModule))]
    public partial class AudioModuleDrawer : FeatureModuleDrawer{
        
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