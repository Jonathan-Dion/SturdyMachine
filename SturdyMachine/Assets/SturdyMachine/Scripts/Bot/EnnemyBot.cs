using System;

using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
using NWH.NUI;
#endif

namespace SturdyMachine.Bot {

    [Serializable]
    public partial class  EnnemyBot : Bot
    {
        #region Attributes



        #endregion

        #region Properties



        #endregion

        #region Methods



        #endregion
    }

#if UNITY_EDITOR

    [CustomEditor(typeof(EnnemyBot))]
    public class EnnemyBotEditor : BotEditor
    {
        public override bool OnInspectorNUI()
        {
            if (!base.OnInspectorNUI())
                return false;

            return true;
        }
    }

#endif
}