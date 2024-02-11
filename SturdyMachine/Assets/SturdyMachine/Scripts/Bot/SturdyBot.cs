using System;

using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace SturdyMachine.Bot
{
    [Serializable]
    public partial class SturdyBot : Bot 
    {

    }

#if UNITY_EDITOR

    [CustomEditor(typeof(SturdyBot))]
    public class SturdyBotEditor : BotEditor
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