using System;

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
    public partial class SturdyComponent
    {

        /// <summary>
        /// Variable representing its initialization state
        /// </summary>
        protected bool _isInitialized;

        /// <summary>
        /// Variable representing its state
        /// </summary>
        protected bool _isEnabled;

        /// <summary>
        /// Called when the present object has just been activated
        /// </summary>
        [NonSerialized, Tooltip("Called when the present object has just been activated")]
        public UnityEvent onAwake;

        /// <summary>
        /// Called when the present object has just been initialize
        /// </summary>
        [NonSerialized, Tooltip("Called when the present object has just been initialize")]
        public UnityEvent onStart;

        /// <summary>
        /// Called for each frame
        /// </summary>
        [NonSerialized, Tooltip("Called for each frame")]
        public UnityEvent onUpdate;

        /// <summary>
        /// Called the last frame after the Update frame
        /// </summary>
        [NonSerialized, Tooltip("Called the last frame after the Update frame")]
        public UnityEvent onLateUpdate;

        /// <summary>
        /// Called depending on physical fluidity
        /// </summary>
        [NonSerialized, Tooltip("Called depending on physical fluidity")]
        public UnityEvent onFixedUpdate;

        /// <summary>
        /// Called when the present object is woken up
        /// </summary>
        [NonSerialized, Tooltip("Called when the present object setActive true")]
        public UnityEvent onEnable;

        /// <summary>
        /// Called when the present object setActive false
        /// </summary>
        [NonSerialized, Tooltip("Called when the present object setActive false")]
        public UnityEvent onDisable;

        /// <summary>
        /// Called when the current object has just been destroyed
        /// </summary>
        [NonSerialized, Tooltip("Called when the current object has just been destroyed")]
        public UnityEvent onDestroy;

        /// <summary>
        /// Called when we want to reverse the activation state
        /// </summary>
        [NonSerialized, Tooltip("Called when we want to reverse the activation state")]
        public UnityEvent onToggleState;

        /// <summary>
        /// Return if the current object is totaltly initialized
        /// </summary>
        public bool GetIsActive => _isInitialized && _isEnabled;

        /// <summary>
        /// Initialize each UnityEvent
        /// </summary>
        public virtual void Initialize() {

            //Start
            UnityEventInit(ref onStart);

            //Update
            UnityEventInit(ref onUpdate);

            //LateUpdate
            UnityEventInit(ref onLateUpdate);

            //FixedUpdate
            UnityEventInit(ref onFixedUpdate);

            //Enable
            UnityEventInit(ref onEnable);

            //Disable
            UnityEventInit(ref onDisable);

            //Destroy
            UnityEventInit(ref onDestroy);

            //ToogleState
            UnityEventInit(ref onToggleState);
        }

        public virtual void Awake()
        {
            onAwake = new UnityEvent();

            onAwake.Invoke();
        }

        public virtual void Start()
        {
            onStart.Invoke();

            _isInitialized = true;
        }

        public virtual void Update()
        {

            if (!GetIsActive)
                return;

            onUpdate.Invoke();
        }

        public virtual void LateUpdate()
        {

            if (!GetIsActive)
                return;

            onLateUpdate.Invoke();
        }

        public virtual void FixedUpdate()
        {

            if (!GetIsActive)
                return;

            onFixedUpdate.Invoke();
        }

        public virtual void onEnabled()
        {

            onEnable.Invoke();

            _isEnabled = true;
        }

        public virtual void onDisabled()
        {

            onDisable.Invoke();

            _isEnabled = false;
        }

        public virtual void onDestroyed()
        {

            onDestroy.Invoke();
        }

        /// <summary>
        /// Initialize each UnityEvent
        /// </summary>
        void Initialize()
        {

            
        }

        /// <summary>
        /// Assign the default constructor on UnityEvent
        /// </summary>
        /// <param name="pUnityEvent">UnityEvent you want to initialize</param>
        void UnityEventInit(ref UnityEvent pUnityEvent)
        {

            pUnityEvent = new UnityEvent();
        }

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