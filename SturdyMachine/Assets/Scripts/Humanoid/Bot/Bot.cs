using UnityEngine;

using Equipment.Weapon;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Humanoid.Bot
{
    public abstract class Bot : Humanoid
    {
        [SerializeField]
        protected Weapon _fusionBlade;

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

        public override void CustomUpdate(OffenseDirection pOffenseDirection, OffenseType pOffenseType, bool pIsStance)
        {
            base.CustomUpdate( pOffenseDirection, pOffenseType, pIsStance);

            _fusionBlade.Update();

            //Focus
            if (GameplayFeature.Manager.Main.GetInstance.GetFeatureManager.GetFocusManager.GetCurrentFocus)
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(GameplayFeature.Manager.Main.GetInstance.GetFeatureManager.GetFocusManager.GetCurrentFocus.position - transform.position), 0.07f);
        }

        public override void CustomLateUpdate(OffenseDirection pOffenseDirection)
        {
            base.CustomLateUpdate(pOffenseDirection);

            _fusionBlade.CustomLateUpdate(_offenseManager.GetCurrentOffenseDirection);
        }

        public virtual void OnCollisionEnter(Collision pCollision) 
        {
            _fusionBlade.OnCollisionEnter(pCollision);
        }

        public virtual void OnColliserExit(Collision pCollision) 
        {
            _fusionBlade.OnCollisionExit(pCollision);
        }

#if UNITY_EDITOR

        public override void CustomOnInspectorGUI()
        {
            base.CustomOnInspectorGUI();

            EditorGUILayout.Space();

            EditorGUILayout.BeginVertical(GUI.skin.box);

            GUILayout.Label("Weapon", _guiStyle);

            EditorGUILayout.Space();

            EditorGUILayout.ObjectField(_fusionBlade, typeof(Weapon), true);

            EditorGUILayout.EndVertical();
        }

#endif
    }

}