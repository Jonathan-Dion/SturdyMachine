using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Humanoid
{
    [RequireComponent(typeof(Animator))]
    [RequireComponent(typeof(OffenseManager))]
    public abstract class Humanoid : MonoBehaviour 
    {
        [SerializeField]
        protected Animator _animator;

        [SerializeField]
        protected OffenseManager _offenseManager;

        public OffenseManager GetOffenseManager { get; protected set; }

        public virtual void Awake() 
        {
            _animator = GetComponent<Animator>();
            _offenseManager = GetComponent<OffenseManager>();
        }

        public virtual void Start() { }

        public virtual void FixedUpdate() { }

        public virtual void Update(OffenseDirection pOffenseDirection, OffenseType pOffenseType, bool pIsStance) 
        {
            _offenseManager.SetAnimation(_animator, pOffenseDirection, pOffenseType, pIsStance);
        }
        
        public virtual void LateUpdate(OffenseDirection pOffenseDirection) { }
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(Humanoid), true)]
    public class HumanoidEditor : Editor 
    {
        protected GUIStyle _guiStyle;

        Humanoid _humanoid;

        Editor _offenseManagerEditor;

        protected void OnEnable()
        {
            _guiStyle = new GUIStyle();

            _humanoid = target as Humanoid;

            _offenseManagerEditor = CreateEditor(_humanoid.GetOffenseManager);

            _guiStyle.fontStyle = FontStyle.BoldAndItalic;
        }

        public override void OnInspectorGUI()
        {
            EditorGUILayout.BeginVertical(GUI.skin.box);

            GUILayout.Label("Humanoid", _guiStyle);

            EditorGUILayout.Space();

            _offenseManagerEditor.OnInspectorGUI();

            EditorGUILayout.EndVertical();
        }
    }

#endif
}