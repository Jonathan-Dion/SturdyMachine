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
        /// <summary>
        /// ParticleSystem component for create a trail on this equipment
        /// </summary>
        [SerializeField, Tooltip("ParticleSystem component for create a trail on this equipment")]
        ParticleSystem _weaponTrail;

        public override bool LateUpdateRemote(bool pNextState = true)
        {
            if (!base.OnUpdate())
                return false;

            if (!_weaponTrail)
                return true;

            if (_weaponTrail.transform.gameObject.activeSelf == pNextState)
                return true;

            _weaponTrail.transform.gameObject.SetActive(pNextState);

            return true;
        }

        public override void OnCollisionEnter(Collision pCollision)
        {
            base.OnCollisionEnter(pCollision);
        }

        public override void OnCollisionExit(Collision pCollision)
        {
            base.OnCollisionExit(pCollision);
        }
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