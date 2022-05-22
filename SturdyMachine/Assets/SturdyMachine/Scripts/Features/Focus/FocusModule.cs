using System;
using SystemRandom = System.Random;

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

        SystemRandom _random;

        //MonsterBot
        [SerializeField]
        Vector3[] _originalMonsterBotPosition;

        [SerializeField]
        GameObject[] _monsterBot;

        int _currentMonsterBotIndex, _lastMonsterBotIndex;

        //Timer
        [SerializeField, Range(0f, 5f), Tooltip("Time in seconds before the next focus change")]
        float _maxTimer;

        [SerializeField]
        float _currentTimer;

        public Transform GetCurrentFocus => _currentFocus;
        public GameObject[] GetMonsterBot => _monsterBot;

        public override FeatureModuleCategory GetFeatureModuleCategory()
        {
            return FeatureModuleCategory.Focus;
        }

        public override void Awake(GameObject pGameObject)
        {
            _random = new SystemRandom();

            base.Awake(pGameObject);
        }

        public override void Initialize()
        {
            OriginalMonsterInit();

            base.Initialize();
        }

        public virtual void UpdateFocus(Vector3 pSturdyPosition)
        {
            if (!GetIsActivated)
                return;

            if (_monsterBot.Length > 1)
            {
                _currentTimer += Time.deltaTime;

                if (_currentTimer >= _maxTimer)
                {
                    if (_lastMonsterBotIndex != _currentMonsterBotIndex)
                        _lastMonsterBotIndex = _currentMonsterBotIndex;

                    while (_currentMonsterBotIndex == _lastMonsterBotIndex)
                        _currentMonsterBotIndex = _random.Next(_monsterBot.Length);

                    _currentFocus = _monsterBot[_currentMonsterBotIndex].transform;

                    _monsterBot[_currentMonsterBotIndex].transform.position = Vector3.MoveTowards(_currentFocus.position, pSturdyPosition, 0.5f);

                    _monsterBot[_lastMonsterBotIndex].transform.position = _originalMonsterBotPosition[_lastMonsterBotIndex];

                    _currentTimer = 0f;
                }
            }
        }

        public override void FixedUpdate()
        {
            
        }

        public override void Enable()
        {
            base.Enable();
        }

        public override void Disable()
        {
            base.Disable();
        }

        void OriginalMonsterInit() 
        {
            _originalMonsterBotPosition = new Vector3[_monsterBot.Length];

            for (int i = 0; i < _originalMonsterBotPosition.Length; ++i)
                _originalMonsterBotPosition[i] = _monsterBot[i].transform.position;

            if (_monsterBot.Length == 1)
                _currentFocus = _monsterBot[0].transform;
        }

        public override void Update() 
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

            drawer.Field("_currentFocus", false);

            drawer.EndSubsection();

            #region MonsterBot

            drawer.BeginSubsection("MonsterBot");

            GUI.enabled = false;

            drawer.ReorderableList("_originalMonsterBotPosition");

            GUI.enabled = true;

            drawer.Space();

            drawer.ReorderableList("_monsterBot");

            drawer.EndSubsection();

            #endregion

            #region Timer

            drawer.BeginSubsection("Timer");

            drawer.Field("_currentTimer", false);

            drawer.Field("_maxTimer");

            drawer.EndSubsection();

            #endregion

            drawer.EndProperty();
            return true;
        }
    }

#endif

}