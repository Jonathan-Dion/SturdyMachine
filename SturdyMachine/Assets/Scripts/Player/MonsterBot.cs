using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Humanoid.Bot.Monster
{
    public class MonsterBot : Bot 
    {
        public override void Awake()
        {
            base.Awake();
        }

        public override void Start()
        {
            base.Start();
        }

        public override void CustomUpdate(OffenseDirection pOffenseDirection, OffenseType pOffenseType, bool pIsStance = false)
        {
            base.CustomUpdate(pOffenseDirection, pOffenseType, pIsStance);
        }

        public override void CustomLateUpdate(OffenseDirection pOffenseDirection)
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

        public override void CustomOnEnable()
        {
            base.CustomOnEnable();
        }

        public override void CustomOnDisable()
        {
            base.CustomOnDisable();
        }

        public override void CustomOnInspectorGUI()
        {
            base.CustomOnInspectorGUI();
        }
#endif
    }
}