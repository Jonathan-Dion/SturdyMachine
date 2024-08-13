using System;

using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
using NWH.NUI;
#endif

namespace SturdyMachine.Component
{
    /// <summary>
    /// Manage the current state of this object
    /// </summary>
    [Serializable]
    public partial class ModuleComponent
    {
        #region Attributes

        protected BaseComponent _baseComponent;

        /// <summary>
        /// Variable representing its initialization state
        /// </summary>
        protected bool _isInitialized;

        /// <summary>
        /// Variable representing its state
        /// </summary>
        protected bool _isEnabled;

        #endregion

        #region Get

        /// <summary>
        /// Return if the current object is totaltly initialized
        /// </summary>
        public bool GetIsActive => _isInitialized && _isEnabled;
        
        public BaseComponent GetSturdyComponent => _baseComponent;

        #endregion

        #region Method

        /// <summary>
        /// Component initialization
        /// </summary>
        public virtual void Initialize() {
        
            _isInitialized = true;
        }

        /// <summary>
        /// Called when the component has just been instantiated
        /// </summary>
        public virtual void OnAwake(BaseComponent pBaseComponent) {

            _baseComponent = pBaseComponent;

            _isInitialized = false;
            
            _isEnabled = false;
        }

        /// <summary>
        /// Called after the component has instantiate
        /// </summary>
        /// <returns></returns>
        public virtual bool OnStart() => GetIsActive;

        /// <summary>
        /// Called every frames
        /// </summary>
        public virtual bool OnUpdate() => GetIsActive;

        /// <summary>
        /// Called every frame after OnUpdate
        /// </summary>
        public virtual bool OnLateUpdate() => GetIsActive;

        /// <summary>
        /// Called when activating the component
        /// </summary>
        public virtual void OnEnabled() {

            if (!_isInitialized) {

                if (Application.isPlaying)
                    Initialize();
            }

            _isEnabled = true;
        }

        /// <summary>
        /// Called when deactivating the component
        /// </summary>
        public virtual void OnDisabled() {
        
            _isEnabled = false;
        }

        #endregion

    }

#if UNITY_EDITOR

    [CustomEditor(typeof(ModuleComponent))]
    public class SturdyModuleComponentEditor : NUIEditor
    {
        public override bool OnInspectorNUI()
        {
            if (!base.OnInspectorNUI())
                return false;

            drawer.BeginSubsection("Debug Value");

            drawer.Field("_isInitialized", false, null, "Initialized: ");
            drawer.Field("_isEnabled", false, null, "Enabled: ");

            drawer.EndSubsection();

            drawer.EndEditor(this);
            return true;
        }
    }

#endif
}