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
        protected Weapon _fusionBlade = new Weapon();

        public Weapon GetFusionBlade { get; protected set; }

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

        public override void Update(OffenseDirection pOffenseDirection, OffenseType pOffenseType, bool pIsStance)
        {
            base.Update( pOffenseDirection, pOffenseType, pIsStance);

            _fusionBlade.Update();
        }

        public override void LateUpdate(OffenseDirection pOffenseDirection)
        {
            base.LateUpdate(pOffenseDirection);

            _fusionBlade.LateUpdate(pOffenseDirection);
        }

        public virtual void OnCollisionEnter(Transform pTransform, Collision pCollision) 
        {
            _fusionBlade.OnCollisionEnter(pTransform, pCollision);
        }

        public virtual void OnColliserExit(Transform pTransform, Collision pCollision) 
        {
            _fusionBlade.OnCollisionExit(pTransform, pCollision);
        }
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(Bot))]
    public class BotEditor : HumanoidEditor 
    {
        Bot _bot;

        Editor _fusionBladeEditor;

        protected void OnEnable()
        {
            base.OnEnable();

            _bot = target as Bot;

            _fusionBladeEditor = CreateEditor(_bot.GetFusionBlade);
        }

        public override void OnInspectorGUI()
        {
            EditorGUILayout.BeginVertical(GUI.skin.box);

            GUILayout.Label("Bot", _guiStyle);

            EditorGUILayout.Space();

            base.OnInspectorGUI();

            EditorGUILayout.Space();

            _fusionBladeEditor.OnInspectorGUI();

            EditorGUILayout.EndVertical();
        }
    }

#endif
}