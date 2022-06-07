using System;

using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace SturdyMachine.Features.Focus 
{
    [Serializable]
    public partial class FocusModule : FeatureModule 
    {
        [SerializeField]
        Transform _currentFocus;

        [SerializeField]
        int _currentMonsterBotIndex;

        Transform _sturdyTransform;

        Transform[] _monsterTransform;

        bool _lastLookLeftState, _lastLookRightState;

        //Timer
        [SerializeField, Range(0f, 5f), Tooltip("Time in seconds before the next focus change")]
        float _maxTimer;

        [SerializeField]
        float _currentTimer;

        public int GetCurrentMonsterBotIndex => _currentMonsterBotIndex;
        public Transform GetCurrentFocus => _currentFocus;

        public override FeatureModuleCategory GetFeatureModuleCategory()
        {
            return FeatureModuleCategory.Focus;
        }

        public override void Initialize(MonsterBot[] pMonsterBot, SturdyBot pSturdyBot)
        {
            _sturdyTransform = pSturdyBot.transform;

            _monsterTransform = new Transform[pMonsterBot.Length];

            for (int i = 0; i < pMonsterBot.Length; ++i)
                _monsterTransform[i] = pMonsterBot[i].transform;

            _currentMonsterBotIndex = UnityEngine.Random.Range(0, _monsterTransform.Length - 1);

            base.Initialize(pMonsterBot, pSturdyBot);
        }

        void LookSetup(MonsterBot[] pMonsterBot, Inputs.SturdyInputControl pSturdyInputControl) 
        {
            //MonsterLook
            MonsterBotLook();

            //SturdyBot
            SturdyBotLook(pMonsterBot, pSturdyInputControl.GetIsLeftFocusActivated, pSturdyInputControl.GetIsRightFocusActivated);
        }

        void MonsterBotLook() 
        {
            for (int i = 0; i < _monsterTransform.Length; ++i)
            {
                if (_monsterTransform[i] != null) 
                {
                    if (_monsterTransform[i].rotation != Quaternion.Slerp(_monsterTransform[i].rotation, Quaternion.LookRotation(_sturdyTransform.position - _monsterTransform[i].position), 0.07f))
                        _monsterTransform[i].rotation = Quaternion.Slerp(_monsterTransform[i].rotation, Quaternion.LookRotation(_sturdyTransform.position - _monsterTransform[i].position), 0.07f);
                }
            }
        }

        void SturdyBotLook(MonsterBot[] pMonsterBot,bool pIsLookLeft, bool pIsLookRight) 
        {
            if (_monsterTransform.Length > 1)
            {
                //LookLeft
                if (pIsLookLeft)
                {
                    if (!_lastLookLeftState)
                    {
                        if (_currentMonsterBotIndex > 0)
                            --_currentMonsterBotIndex;

                        _lastLookLeftState = true;
                    }
                }
                else if (_lastLookLeftState)
                    _lastLookLeftState = false;

                //LookRight
                else if (pIsLookRight)
                {
                    if (!_lastLookRightState)
                    {
                        if (_currentMonsterBotIndex < _monsterTransform.Length - 1)
                            ++_currentMonsterBotIndex;

                        _lastLookRightState = true;
                    }
                }
                else if (_lastLookRightState)
                    _lastLookRightState = false;
            }

            if (_currentFocus != _monsterTransform[_currentMonsterBotIndex])
                _currentFocus = _monsterTransform[_currentMonsterBotIndex];

            _sturdyTransform.rotation = Quaternion.Slerp(_sturdyTransform.rotation, Quaternion.LookRotation(_monsterTransform[_currentMonsterBotIndex].transform.position - _sturdyTransform.position), 0.07f);

            _sturdyTransform.position = Vector3.Lerp(_sturdyTransform.position, _monsterTransform[_currentMonsterBotIndex].position - pMonsterBot[_currentMonsterBotIndex].GetFocusRange, 0.5f);
        }

        public override void UpdateRemote(MonsterBot[] pMonsterBot, SturdyBot pSturdyBot, Inputs.SturdyInputControl pSturdyInputControl)
        {
            if (!GetIsActivated)
                return;

            LookSetup(pMonsterBot, pSturdyInputControl);

        }

        public override void FixedUpdate()
        {
            
        }
    }

#if UNITY_EDITOR

    [CustomPropertyDrawer(typeof(FocusModule))]
    public partial class FocusModuleDrawer : FeatureModuleDrawer 
    {
        public override bool OnNUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (!base.OnNUI(position, property, label))
                return false;

            drawer.BeginSubsection("Debug value");

            //drawer.Field("_currentFocus", false);
            drawer.Field("_currentMonsterBotIndex", false);

            drawer.EndSubsection();

            drawer.EndProperty();
            return true;
        }
    }

#endif

}