using System;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace SturdyMachine 
{
    [Serializable]
    public partial class SturdyBot : Bot 
    {
        

        public override void OnCollisionEnter(Collision pCollision)
        {
            base.OnCollisionEnter(pCollision);
        }

        public override void OnCollisionExit(Collision pCollision)
        {
            base.OnCollisionExit(pCollision);
        }
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