using System;
using UnityEngine;

namespace SturdyMachine
{
    [Serializable]
    public abstract partial class SturdyComponent 
    {
        protected bool _isInitialized, _isEnabled;

        protected GameObject _main;

        public bool GetIsInitialized => _isInitialized;
        public bool GetIsActivated => _isInitialized && _isEnabled;

        public virtual void Initialize(MonsterBot[] pMonsterBot) 
        {
            _isInitialized = true;
        }

        public virtual void Awake(GameObject pGameObject) 
        {
            this._main = pGameObject;

            _isInitialized = false;
            _isEnabled = false;
        }

        public virtual void UpdateRemote(MonsterBot[] pMonsterBot, SturdyBot pSturdyBot, Inputs.SturdyInputControl pSturdyInputControl) 
        {
            
        }

        public abstract void FixedUpdate();

        public virtual void Enable(MonsterBot[] pMonsterBot) 
        {
            if (!_isInitialized) 
            {
                if (Application.isPlaying)
                    Initialize(pMonsterBot);
            }

            _isEnabled = true;
        }

        public virtual void Disable() 
        {
            _isEnabled = false;
        }

        public virtual void ToogleState(MonsterBot[] pMonsterBot) 
        {
            if (_isEnabled)
                Disable();
            else
                Enable(pMonsterBot);
        }
    }
}