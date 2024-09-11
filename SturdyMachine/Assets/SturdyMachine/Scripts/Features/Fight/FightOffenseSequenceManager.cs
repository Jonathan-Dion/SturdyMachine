using System;

using UnityEngine;
using SturdyMachine.Component;

#if UNITY_EDITOR
using UnityEditor;
using NWH.NUI;
#endif

namespace SturdyMachine.Features.Fight.Sequence {

    [CreateAssetMenu(fileName = "FightOffeneSequenceManager", menuName = "SturdyMachine/Features/Sequences/Manager", order = 2)]
    [DisallowMultipleComponent]
    public class FightOffenseSequenceManager : ScriptableObject {

        #region Attribut

        [SerializeField]
        FightOffenseSequence[] _fightOffenseSequence;

        #endregion

        #region Get

        public FightOffenseSequence GetFightOffenseSequence(BotType pBotType) {

            for (byte i = 0; i < _fightOffenseSequence.Length; ++i) {

                if (_fightOffenseSequence[i].GetBotType != pBotType)
                    continue;

                return _fightOffenseSequence[i];
            }

            return null;
        }

        #endregion
    }


#if UNITY_EDITOR

    [CustomEditor(typeof(FightOffenseSequenceManager))]
    public class FightOffenseSequenceManagerEditor : NUIEditor
    {
        public override bool OnInspectorNUI()
        {
            if (!base.OnInspectorNUI())
                return false;

            EditorGUI.BeginChangeCheck();

            drawer.ReorderableList("_fightOffenseSequence");

            drawer.EndEditor(this);
            return true;
        }
    }

#endif
}