﻿using UnityEngine;

using Equipment.Weapon;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Humanoid.Bot
{
    public abstract class Bot : Humanoid
    {
        [SerializeField]
        protected Weapon _fusionBlade = new Weapon();

        public override void Awake()
        {
            base.Awake();

            _fusionBlade.Awake();
        }

        public override void Start()
        {
            base.Start();

            _fusionBlade.Start();
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();

            _fusionBlade.FixedUpdate();
        }

        public override void CustomUpdate(OffenseDirection pOffenseDirection, OffenseType pOffenseType, bool pIsStance)
        {
            base.CustomUpdate( pOffenseDirection, pOffenseType, pIsStance);

            _fusionBlade.Update();
        }

        public override void CustomLateUpdate(OffenseDirection pOffenseDirection)
        {
            base.CustomLateUpdate(pOffenseDirection);

            _fusionBlade.CustomLateUpdate(pOffenseDirection);
        }

        public virtual void OnCollisionEnter(Collision pCollision) 
        {
            _fusionBlade.OnCollisionEnter(pCollision);
        }

        public virtual void OnColliserExit(Collision pCollision) 
        {
            _fusionBlade.OnCollisionExit(pCollision);
        }
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(Bot))]
    public class BotEditor : HumanoidEditor 
    {
        SerializedProperty _fusionBlade;

        protected void OnEnable()
        {
            base.OnEnable();

            _fusionBlade = serializedObject.FindProperty("_fusionBlade");
        }

        public override void OnInspectorGUI()
        {
            EditorGUILayout.BeginVertical(GUI.skin.box);

            GUILayout.Label("Bot", _guiStyle);

            EditorGUILayout.Space();

            base.OnInspectorGUI();

            serializedObject.ApplyModifiedProperties();

            EditorGUILayout.Space();

            EditorGUILayout.BeginVertical(GUI.skin.box);

            GUILayout.Label("Weapon", _guiStyle);

            EditorGUILayout.Space();

            EditorGUILayout.ObjectField(_fusionBlade);

            serializedObject.ApplyModifiedProperties();

            EditorGUILayout.EndVertical();

            EditorGUILayout.EndVertical();
        }
    }

#endif
}