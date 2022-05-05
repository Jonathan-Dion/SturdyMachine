using UnityEngine;

using ICustomEditor.Class;
using SturdyMachine.Offense.Manager;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Humanoid
{
    [RequireComponent(typeof(Animator))]
    public abstract class Humanoid : UnityICustomEditor 
    {
        protected Animator _animator;

        [SerializeField]
        protected OffenseManager _offenseManager;

        public override void Awake()
        {
            base.Awake();

            _animator = GetComponent<Animator>();
        }

        public override void Start()
        {
            base.Start();
        }

        public virtual void CustomUpdate(OffenseDirection pOffenseDirection, OffenseType pOffenseType, bool pIsStance) 
        {
            if (_offenseManager != null)
                _offenseManager.SetAnimation(_animator, pOffenseDirection, pOffenseType, pIsStance);
        }
        
        public virtual void CustomLateUpdate(OffenseDirection pOffenseDirection) { }

#if UNITY_EDITOR
        public override void CustomOnInspectorGUI()
        {
            EditorGUILayout.BeginVertical(GUI.skin.box);

            GUILayout.Label("Humanoid", _guiStyle);

            EditorGUILayout.Space();

            EditorGUILayout.ObjectField(_offenseManager, typeof(OffenseManager), true);

            EditorGUILayout.EndVertical();
        }

#endif

    }
}