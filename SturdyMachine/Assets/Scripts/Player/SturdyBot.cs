﻿using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Humanoid.Bot.Sturdy
{
    public class SturdyBot : Bot
    {
        public override void Awake()
        {
            base.Awake();
        }

        public override void Start()
        {
            base.Start();
        }

        public virtual void UpdateRemote(OffenseDirection pOffenseDirection, OffenseType pOffenseType, bool pIsStanceActivated, Transform pCurrentFocus) 
        {
            base.CustomUpdate(pOffenseDirection, pOffenseType, pIsStanceActivated);

            //Focus
            if (pCurrentFocus)
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(pCurrentFocus.position - transform.position), 0.07f);
        }

        public virtual void LateUpdateRemote(OffenseDirection pOffenseDirection) 
        {
            base.CustomLateUpdate(pOffenseDirection);
        }

        public override void OnCollisionEnter(Collision pCollision)
        {
            base.OnCollisionEnter(pCollision);
        }

        public override void OnColliserExit(Collision pCollision)
        {
            base.OnColliserExit(pCollision);
        }

#if UNITY_EDITOR

        public override void CustomOnInspectorGUI()
        {
            base.CustomOnInspectorGUI();
        }

#endif
    }
}