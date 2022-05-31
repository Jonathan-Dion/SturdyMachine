using System;

using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
using NWH.VehiclePhysics2;
#endif

namespace SturdyMachine
{
    [Serializable]
    public struct MonsterOffense 
    {
        public OffenseDirection offenseDirection;
        public OffenseType offenseType;

        public float timer;
    }

    [Serializable]
    public partial class MonsterBot : Bot
    {
        [SerializeField]
        MonsterOffense[] _monsterOffense;

        [SerializeField]
        float _currentOffenseTimer, _maxOffenseTimer;

        Offense.Offense _currentOffense;

        bool _isStanceActivated;

        int _currentOffenseIndex;

        public override void Initialize()
        {
            base.Initialize();

            Offense.Manager.OffenseManager offenseManager = Instantiate(_offenseManager);

            _offenseManager = null;

            _offenseManager = offenseManager;
        }

        public virtual void UpdateRemote() 
        {
            if (!GetIsActivated)
                return;

            if (_offenseManager.GetCurrentOffense(_animator.GetCurrentAnimatorClipInfo(0)[0].clip) != null)
            {
                if (_currentOffense != _offenseManager.GetCurrentOffense(_animator.GetCurrentAnimatorClipInfo(0)[0].clip))
                    _currentOffense = _offenseManager.GetCurrentOffense(_animator.GetCurrentAnimatorClipInfo(0)[0].clip);
            }

            if (_currentOffense)
            {
                if (_currentOffense.GetOffenseDirection == OffenseDirection.STANCE)
                {
                    if (!_isStanceActivated)
                        _isStanceActivated = true;

                    if (_currentOffenseTimer >= _maxOffenseTimer)
                    {
                        ++_currentOffenseIndex;

                        if (_currentOffenseIndex > _monsterOffense.Length - 1)
                            _currentOffenseIndex = 0;

                        _offenseManager.SetAnimation(_animator, _monsterOffense[_currentOffenseIndex].offenseDirection, _monsterOffense[_currentOffenseIndex].offenseType, false);

                        _currentOffenseTimer = 0f;
                    }
                    else
                        _currentOffenseTimer += Time.deltaTime;
                }

                if (_currentOffense.GetOffenseType != OffenseType.DEFAULT)
                {
                    if (_isStanceActivated)
                    {
                        _isStanceActivated = false;

                        if (_maxOffenseTimer != _monsterOffense[0].timer)
                            _maxOffenseTimer = _monsterOffense[0].timer;
                    }

                    if (_animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f)
                        _offenseManager.SetAnimation(_animator, OffenseDirection.STANCE, OffenseType.DEFAULT, false);
                }
            }
        }

        public override void LateUpdateRemote(OffenseDirection pOffenseDirection)
        {
            base.LateUpdateRemote(pOffenseDirection);
        }
    }

#if UNITY_EDITOR

    [CustomEditor(typeof(MonsterBot))]
    public class MonsterBotEditor : BotEditor
    {
        public override bool OnInspectorNUI()
        {
            if (!base.OnInspectorNUI())
                return false;

            EditorGUI.BeginChangeCheck();

            drawer.ReorderableList("_monsterOffense");

            drawer.EndEditor(this);

            return true;
        }


        public override bool UseDefaultMargins()
        {
            return false;
        }
    }

    [CustomPropertyDrawer(typeof(MonsterOffense))]
    public partial class MonsterOffenseDrawer : ComponentNUIPropertyDrawer
    {
        public override bool OnNUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (!base.OnNUI(position, property, label))
                return false;

            drawer.Field("offenseDirection");
            drawer.Field("offenseType");
            drawer.Field("timer");

            drawer.EndProperty();
            return true;
        }
    }

#endif
}