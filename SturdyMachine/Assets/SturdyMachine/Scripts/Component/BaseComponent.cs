using System;

using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
using NWH.NUI;
#endif

namespace SturdyMachine.Component {

    /// <summary>
    /// All types of Bot
    /// </summary>
    public enum BotType { None, SturdyBot, SkinnyBot }

    [Serializable]
    public partial class  BaseComponent : MonoBehaviour
    {
        #region Attribut

        /// <summary>
        /// Variable representing its initialization state
        /// </summary>
        [SerializeField, Tooltip("Variable representing its initialization state")]
        protected bool _isInitialized;

        /// <summary>
        /// Variable representing its state
        /// </summary>
        [SerializeField, Tooltip("Variable representing its state")]
        protected bool _isEnabled;

        #endregion

        #region Properties

        /// <summary>
        /// Return if the current object is totaltly initialized
        /// </summary>
        public bool GetIsActive => _isInitialized && _isEnabled;

        #endregion

        #region Method

        /// <summary>
        /// Component initialization
        /// </summary>
        public virtual void Initialize()
        {

            _isInitialized = true;
        }

        /// <summary>
        /// Called when the component has just been instantiated
        /// </summary>
        public virtual void OnAwake()
        {

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
        public virtual bool OnFixedUpdate() => GetIsActive;

        /// <summary>
        /// Called when activating the component
        /// </summary>
        public virtual void OnEnabled()
        {

            if (!_isInitialized)
            {

                if (Application.isPlaying)
                    Initialize();
            }

            _isEnabled = true;
        }

        /// <summary>
        /// Called when deactivating the component
        /// </summary>
        public virtual void OnDisabled()
        {

            _isEnabled = false;
        }
        #endregion
    }

#if UNITY_EDITOR

    [CustomEditor(typeof(BaseComponent))]
    public class BaseComponentEditor : NUIEditor
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