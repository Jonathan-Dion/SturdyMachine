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
    public partial class SturdyModuleComponent
    {
        #region Attribut

        protected SturdyComponent _sturdyComponent;

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
        
        public SturdyComponent GetSturdyComponent => _sturdyComponent;
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
        public virtual void OnAwake(SturdyComponent pSturdyComponent) {

            _sturdyComponent = pSturdyComponent;

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
        /// Called for calculating physics calculations
        /// </summary>
        public virtual bool OnFixedUpdate() {
        
            if (GetIsActive)
                return false;

            return true;
        }

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

        /// <summary>
        /// Called to reverse activation state
        /// </summary>
        /// <param name="pState">Current component state</param>
        /// <param name="pNextState">Next component state</param>
        public virtual void ToogleState(ref bool pState, bool pNextState = true) {

            if (pState == pNextState)
                pState = pNextState;
        }

        #endregion

    }

#if UNITY_EDITOR

    [CustomEditor(typeof(SturdyModuleComponent))]
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