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

        bool _lastLookLeftState, _lastLookRightState;

        //Timer
        [SerializeField, Range(0f, 5f), Tooltip("Time in seconds before the next focus change")]
        float _maxTimer;

        [SerializeField]
        float _currentTimer;

        public Transform GetCurrentFocus => _currentFocus;

        public override FeatureModuleCategory GetFeatureModuleCategory()
        {
            return FeatureModuleCategory.Focus;
        }

        void LookSetup(MonsterBot[] pMonsterBot, SturdyBot pSturdyBot, Inputs.SturdyInputControl pSturdyInputControl) 
        {
            //MonsterLook
            MonsterBotLook(pMonsterBot, pSturdyBot.transform);

            //SturdyBot
            SturdyBotLook(pMonsterBot, pSturdyBot.transform, pSturdyInputControl.GetIsLeftFocusActivated, pSturdyInputControl.GetIsRightFocusActivated);
        }

        void MonsterBotLook(MonsterBot[] pMonsterBot, Transform pSturdyTransform) 
        {
            for (int i = 0; i < pMonsterBot.Length; ++i)
            {
                if (pMonsterBot[i].transform.rotation != Quaternion.Slerp(pMonsterBot[i].transform.rotation, Quaternion.LookRotation(pSturdyTransform.position - pMonsterBot[i].transform.position), 0.07f))
                    pMonsterBot[i].transform.rotation = Quaternion.Slerp(pMonsterBot[i].transform.rotation, Quaternion.LookRotation(pSturdyTransform.position - pMonsterBot[i].transform.position), 0.07f);
            }
        }

        void SturdyBotLook(MonsterBot[] pMonsterBot, Transform pSturdyTransform, bool pIsLookLeft, bool pIsLookRight) 
        {
            if (pMonsterBot.Length > 1)
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
                        if (_currentMonsterBotIndex < pMonsterBot.Length - 1)
                            ++_currentMonsterBotIndex;

                        _lastLookRightState = true;
                    }
                }
                else if (_lastLookRightState)
                    _lastLookRightState = false;

                pSturdyTransform.rotation = Quaternion.Slerp(pSturdyTransform.rotation, Quaternion.LookRotation(pMonsterBot[_currentMonsterBotIndex].transform.position - pSturdyTransform.position), 0.07f);
            }
        }

        public override void UpdateRemote(MonsterBot[] pMonsterBot, SturdyBot pSturdyBot, Inputs.SturdyInputControl pSturdyInputControl)
        {
            if (!GetIsActivated)
                return;

            LookSetup(pMonsterBot, pSturdyBot, pSturdyInputControl);

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