using UnityEngine;

namespace GameplayFeature.Focus
{
    public abstract class Focus : GameplayFeature 
    {
        Vector3 _currentFocus;

        public override void Awake()
        {
            base.Awake();
        }

        public override void Start()
        {
            base.Start();
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
        }

        public virtual void Update(Transform pCurrentFocus) 
        {
            base.Update();

            if (_currentFocus != (Main.GetInstance.GetCurrentFocus.transform.position - transform.position))
            {
                _currentFocus = Main.GetInstance.GetCurrentFocus.transform.position - transform.position;

                _currentFocus.y = 0f;
            }

            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(_currentFocus), 0.07f);
        }

        public override void LateUpdate()
        {
            base.LateUpdate();
        }
    }
}