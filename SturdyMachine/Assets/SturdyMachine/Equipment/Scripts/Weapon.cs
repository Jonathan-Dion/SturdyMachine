using System;

using UnityEngine;

#if UNITY_EDITOR
using NWH.NUI;
using UnityEditor;
#endif

namespace SturdyMachine.Equipment 
{
    public partial class Weapon : Equipment 
    {
        [SerializeField]
        ParticleSystem _weaponTrail;

        public override void Initialize()
        {
            base.Initialize();
        }

        public override void Awake()
        {
            base.Awake();
        }

        public override void Update()
        {
            base.Update();
        }

        public override void LateUpdateRemote(OffenseDirection pOffenseDirection)
        {
            if (_weaponTrail != null)
            {
                if (pOffenseDirection == OffenseDirection.STANCE)
                {
                    if (_weaponTrail.transform.gameObject.activeSelf)
                        _weaponTrail.transform.gameObject.SetActive(false);
                }
                else if (!_weaponTrail.transform.gameObject.activeSelf)
                    _weaponTrail.transform.gameObject.SetActive(true);
            }
        }

        public override void OnCollisionEnter(Collision pCollision)
        {
            base.OnCollisionEnter(pCollision);
        }

        public override void OnCollisionExit(Collision pCollision)
        {
            base.OnCollisionExit(pCollision);
        }

        public override void Enable()
        {
            base.Enable();
        }

        public override void Disable()
        {
            base.Disable();
        }

        public override void ToogleState()
        {
            base.ToogleState();
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