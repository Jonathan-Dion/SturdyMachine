using System;

using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
using NWH.NUI;
#endif

namespace SturdyMachine.Component
{
    /// <summary>
    /// All types of Bot
    /// </summary>
    public enum BotType { Default, SturdyBot, SkinnyBot }

    /// <summary>
    /// Manage the current state of this object
    /// </summary>
    [Serializable]
    public partial class SturdyComponent : BaseComponent
    {
        #region Attribut

        /// <summary>
        /// Current type of this Bot
        /// </summary>
        [SerializeField, Tooltip("Current type of this Bot")]
        protected BotType _botType;

        #endregion

        #region Get

        public BotType GetBotType => _botType;

        #endregion

    }

#if UNITY_EDITOR

    [CustomEditor(typeof(SturdyComponent))]
    public class SturdyComponentEditor : BaseComponentEditor
    {
        public override bool OnInspectorNUI()
        {
            if (!base.OnInspectorNUI())
                return false;

            base.OnInspectorNUI();

            drawer.Field("_botType");

            drawer.EndEditor(this);
            return true;
        }
    }

#endif
}