using NWH.VehiclePhysics2;
using SturdyMachine.Offense;
using System;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
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