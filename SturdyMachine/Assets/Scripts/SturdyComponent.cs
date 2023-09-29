﻿using System;

using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;
using NWH.NUI;
#endif

namespace SturdyMachine.Utilities
{
    /// <summary>
    /// Manage the current state of this object
    /// </summary>
    [Serializable]
    public partial class SturdyComponent : MonoBehaviour
    {
        #region Attribut

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
        public virtual void OnAwake() {

            _isInitialized = false;

            _isEnabled = false;
        }

        /// <summary>
        /// Called every frames
        /// </summary>
        public virtual void OnUpdate() {

            if (GetIsActive)
                return;
        }

        /// <summary>
        /// Called for calculating physics calculations
        /// </summary>
        public virtual void OnFixedUpdate() {
        
            if (GetIsActive)
                return;
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

    [CustomEditor(typeof(SturdyComponent))]
    public class SturdyComponentEditor : NUIEditor
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