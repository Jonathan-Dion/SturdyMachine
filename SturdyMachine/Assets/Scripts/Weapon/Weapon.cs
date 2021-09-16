﻿using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Equipment.Weapon
{
    public class Weapon : Equipment
    {
        [SerializeField]
        protected ParticleSystem _weaponTrail;

        public override void Awake()
        {
            base.Awake();
        }

        public override void Start()
        {
            base.Start();
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
        }

        public override void Update()
        {
            base.Update();
        }

        public override void CustomLateUpdate(OffenseDirection pOffenseDirection)
        {
            base.CustomLateUpdate(pOffenseDirection);

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
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(Weapon))]
    public class WeaponEditor : EquipmentEditor 
    {
        SerializedProperty _weaponTrail;

        protected void OnEnable()
        {
            base.OnEnable();

            _weaponTrail = serializedObject.FindProperty("_weaponTrail");
        }

        public override void OnInspectorGUI()
        {
            EditorGUILayout.BeginVertical(GUI.skin.box);

            GUILayout.Label("Weapon", _guiStyle);

            EditorGUILayout.Space();

            EditorGUILayout.ObjectField(_weaponTrail);

            serializedObject.ApplyModifiedProperties();

            EditorGUILayout.Space();

            base.OnInspectorGUI();

            serializedObject.ApplyModifiedProperties();

            EditorGUILayout.EndVertical();
        }
    }

#endif

}