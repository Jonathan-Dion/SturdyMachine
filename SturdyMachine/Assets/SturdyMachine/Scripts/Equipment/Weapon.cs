using System;

using UnityEngine;

#if UNITY_EDITOR
using NWH.NUI;
using UnityEditor;
#endif

namespace SturdyMachine.Equipment 
{
    /// <summary>
    /// Weapon class
    /// </summary>
    public partial class Weapon : Equipment 
    {
        #region Attribut

        /// <summary>
        /// ParticleSystem component for create a trail on this equipment
        /// </summary>
        [SerializeField, Tooltip("ParticleSystem component for create a trail on this equipment")]
        ParticleSystem _weaponTrail;

        #endregion

        #region Method



        #endregion

    }

#if UNITY_EDITOR

    [CustomEditor(typeof(Weapon))]
    public class WeaponEditor : EquipmentEditor
    {
        public override bool OnInspectorNUI()
        {
            if (!base.OnInspectorNUI())
                return false;

            EditorGUI.BeginChangeCheck();

            drawer.BeginSubsection("Configuration");

            drawer.Field("_weaponTrail");

            drawer.EndSubsection();

            drawer.EndEditor(this);
            return true;
        }
    }

#endif

}