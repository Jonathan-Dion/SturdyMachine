using System;
using UnityEngine;
using SturdyMachine.Offense;
using SturdyMachine.Features.Fight;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace SturdyMachine 
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


        public override bool UseDefaultMargins()
        {
            return false;
        }
    }

#endif
}