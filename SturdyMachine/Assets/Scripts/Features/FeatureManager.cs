using UnityEngine;

using ICustomEditor.Class;
using Feature.Focus.Manager;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Feature.Manager 
{
    public class FeatureManager : UnityICustomEditor 
    {
        FocusManager _focusManager;

        public FocusManager GetFocusManager => _focusManager;

        public override void Awake() 
        {
            _focusManager = GetComponent<FocusManager>();
        }

        public override void Start() 
        {
            
        }

        public virtual void CustomUpdate(Vector3 pSturdyPosition) 
        {
            _focusManager.CustomUpdate(pSturdyPosition);
        }

        public virtual void LateUpdate()
        {
            
        }

#if UNITY_EDITOR

        public override void CustomOnInspectorGUI()
        {

        }

        public override void CustomOnEnable()
        {
            base.CustomOnEnable();

           //FocusManager
           _focusManager = gameObject.GetComponent<FocusManager>() ? gameObject.GetComponent<FocusManager>() 
                                                                   : gameObject.AddComponent<FocusManager>();

            _focusManager.CustomOnEnable();
        }

        public override void CustomOnDisable()
        {
            //FocusManager
            _focusManager.CustomOnDisable();
        }

#endif
    }
}