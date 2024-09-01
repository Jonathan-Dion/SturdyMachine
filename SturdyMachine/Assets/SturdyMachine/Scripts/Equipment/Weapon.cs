using System;

#if UNITY_EDITOR
using UnityEditor;
using NWH.NUI;
#endif

namespace SturdyMachine.Equipment {

    [Serializable]
    public partial class Weapon : Equipment {
    
        
    }

#if UNITY_EDITOR

    [CustomEditor(typeof(Weapon))]
    public class WeapontEditor : EquipmentEditor
    {
        public override bool OnInspectorNUI()
        {
            if (!base.OnInspectorNUI())
                return false;

            return true;
        }
    }

#endif
}