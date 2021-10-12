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

        public virtual void Update()
        {
            _focusManager.Update();
        }

        public virtual void LateUpdate()
        {
            _focusManager.LateUpdate();
        }

        public virtual void OnEnable()
        {
            _focusManager = GetComponent<FocusManager>();
        }

        public virtual void OnDisable()
        {
            _focusManager = null;
        }

#if UNITY_EDITOR

        public override void CustomOnEnable()
        {
            
        }

        public override void CustomOnInspectorGUI()
        {
            
        }

        public override void CustomOnDisable()
        {
            
        }

#endif
    }
}