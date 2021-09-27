using UnityEngine;

using ICustomEditor.Class;

using GameplayFeature.Manager;

namespace Feature.Focus
{
    public abstract class Focus : UnityICustomEditor 
    {
        Vector3 _currentFocus;

        public virtual void Awake() { }

        public virtual void Start() { }

        public virtual void FixedUpdate() { }

        public virtual void Update(Transform pTransform) 
        {
            if (_currentFocus != (Main.GetInstance.GetCurrentFocus.position - transform.position))
            {
                _currentFocus = Main.GetInstance.GetCurrentFocus.transform.position - transform.position;

                _currentFocus.y = 0f;
            }

            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(_currentFocus), 0.07f);
        }

        public virtual void LateUpdate() { }
    }
}