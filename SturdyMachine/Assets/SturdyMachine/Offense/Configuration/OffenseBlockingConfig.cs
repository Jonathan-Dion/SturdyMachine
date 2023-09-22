using System.Collections.Generic;

using UnityEngine;

using SturdyMachine.Utilities;

#if UNITY_EDITOR
using NWH.NUI;
using UnityEditor;
#endif

namespace SturdyMachine.Offense.Blocking
{
    /// <summary>
    /// Store all OffenseBlocking
    /// </summary>
    public class OffenseBlockingConfig : ScriptableObject {

        #region Property

        /// <summary>
        /// Array of OffenseBlocking
        /// </summary>
        [SerializeField, Tooltip("Array of OffenseBlocking")]
        OffenseBlocking[] _offenseBlocking;

        #endregion

        #region Get

        /// <summary>
        /// Return array of alls offense of blocking type
        /// </summary>
        public OffenseBlocking[] GetOffenseBlocking => _offenseBlocking;

        #endregion

        /*/// <summary>
        /// Initialize OffenseBlocking in term of currentOffense
        /// </summary>
        /// <param name="pCurrentOffense">Current Offense of the attacker</param>
        /// <param name="pOffenseBlocking">OffenseBlocking array for defender</param>
        /// <param name="pIsSturdyBot">If the defender are player</param>
        public void OffenseBlockingSetup(Offense pCurrentOffense, OffenseBlocking[] pOffenseBlocking, bool pIsSturdyBot) {
            List<OffenseBlocking> offenseBlockings = new List<OffenseBlocking>();

            for (int i = 0; i < _offenseBlocking.Length; ++i) {

                for (int j = 0; j < _offenseBlocking[i].GetOffenseBlockingData.Count; ++j) {

                    //Check if the currentOffense is in OffenseBlocking array
                    if (!_offenseBlocking[i].GetOffenseBlockingData[j].GetIsGoodOffenseBlocking(pCurrentOffense))
                        continue;

                    //Check if the OffenseBlocking of attacker according with defender
                    if (pOffenseBlocking[i] == _offenseBlocking[i])
                        continue;

                    //Define a good OffenseBlocking
                    if (_offenseBlocking[i].name.Contains("Evasion")) { 
                        
                        if (pIsSturdyBot)
                            offenseBlockings.Add(_offenseBlocking[i]);

                        continue;
                    }

                    offenseBlockings.Add(_offenseBlocking[i]);

                }
            }

            pOffenseBlocking = offenseBlockings.ToArray();
        }*/

#if UNITY_EDITOR

        [CustomEditor(typeof(OffenseBlockingConfig))]
        [CanEditMultipleObjects]
        public class OffenseBlockingConfigEditor : NUIEditor
        {
            public override bool OnInspectorNUI()
            {
                if (!base.OnInspectorNUI())
                    return false;

                EditorGUI.BeginChangeCheck();

                drawer.ReorderableList("_offenseBlocking");

                drawer.EndEditor(this);
                return true;
            }
        }

#endif
    }
}