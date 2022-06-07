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
        public override void Initialize()
        {
            base.Initialize();
        }

        public override void Awake()
        {
            base.Awake();
        }

        public virtual void UpdateRemote(OffenseDirection pOffenseDirection, OffenseType pOffenseType, bool pIsStanceActivated, Features.Fight.FightModule pFightModule)
        {
            base.UpdateRemote(pOffenseDirection, pOffenseType, pIsStanceActivated);

            if (pFightModule.GetIsHitting)
                _offenseManager.SetAnimation(_animator, OffenseDirection.DEFAULT, OffenseType.DAMAGEHIT, pIsStanceActivated);
            else if (pFightModule.GetIsBlocking)
                _offenseManager.SetAnimation(_animator, OffenseDirection.DEFAULT, OffenseType.REPEL, pIsStanceActivated);

        }

        public override void LateUpdateRemote(OffenseDirection pOffenseDirection)
        {
            base.LateUpdateRemote(pOffenseDirection);
        }

        public override void Enable()
        {
            base.Enable();
        }

        public override void Disable()
        {
            base.Disable();
        }

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