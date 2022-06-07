using System;
using UnityEngine;

namespace SturdyMachine
{
    [Serializable]
    public abstract partial class SturdyComponent 
    {
        protected bool _isInitialized, _isEnabled;

        protected Manager.Main _main;
        protected SturdyBot _sturdyBot;

        public bool GetIsInitialized => _isInitialized;
        public bool GetIsActivated => _isInitialized && _isEnabled;

        public virtual void Initialize(MonsterBot[] pMonsterBot, SturdyBot pSturdyBot) 
        {
            _isInitialized = true;
        }

        public virtual void Awake(Manager.Main pMain, SturdyBot pSturdyBot) 
        {
            _main = pMain;
            _sturdyBot = pSturdyBot;

            _isInitialized = false;
            _isEnabled = false;
        }

        public virtual void UpdateRemote(MonsterBot[] pMonsterBot, SturdyBot pSturdyBot, Inputs.SturdyInputControl pSturdyInputControl) 
        {
            
        }

        public abstract void FixedUpdate();

        public virtual void Enable(MonsterBot[] pMonsterBot, SturdyBot pSturdyBot) 
        {
            if (!_isInitialized) 
            {
                if (Application.isPlaying)
                    Initialize(pMonsterBot, pSturdyBot);
            }

            _isEnabled = true;
        }

        public virtual void Disable() 
        {
            _isEnabled = false;
        }

        public virtual void ToogleState(MonsterBot[] pMonsterBot, SturdyBot pSturdyBot) 
        {
            if (_isEnabled)
                Disable();
            else
                Enable(pMonsterBot, pSturdyBot);
        }
    }
}