using UnityEngine;

using ICustomEditor.Class;
using Feature.Focus.Manager;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Feature.Manager 
{
    [RequireComponent(typeof(FocusManager))]
    public class FeatureManager : UnityICustomEditor 
    {
        [SerializeField]
        protected FocusManager _focusManager;

        public FocusManager GetFocusManager => _focusManager;

        public override void Awake()
        {
            _focusManager.Awake();
        }

        public override void Start() 
        {
            _focusManager.Start();
        }

        public virtual void CustomUpdate(Vector3 pSturdyPosition) 
        {
            _focusManager.CustomUpdate(pSturdyPosition);
        }

        public virtual void LateUpdate()
        {
            _focusManager.LateUpdate();
        }

#if UNITY_EDITOR

        public override void CustomOnEnable()
        {
            _focusManager = GetComponent<FocusManager>();
        }

        public override void CustomOnInspectorGUI()
        {
            GUI.enabled = false;

            _focusManager = (FocusManager)EditorGUILayout.ObjectField("FocusManager", _focusManager, typeof(FocusManager), true);

            GUI.enabled = true;
        }

#endif
    }
}