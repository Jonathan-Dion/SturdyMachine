
using UnityEngine;

using NWH.VehiclePhysics2;
using System;
using SturdyMachine.Offense;

#if UNITY_EDITOR
using UnityEditor;
using NWH.NUI;
#endif

namespace SturdyMachine.Features.HitConfirm 
{
    /// <summary>
    /// Store all HitConfirmSubSequence for each Offense
    /// </summary>
    [CreateAssetMenu(fileName = "NewHitConfirmOffenseManager", menuName = "SturdyMachine/HitConfirm/Manager", order = 51)]
    public class HitConfirmOffenseManager : ScriptableObject {

        #region Attribut

        

        #endregion

        #region Get

        

        #endregion

        #region Method

        

        #endregion
    }

#if UNITY_EDITOR

    [CustomEditor(typeof(HitConfirmOffenseManager))]
    public class HitConfirmOffenseManagerEditor : NUIEditor
    {
        public override bool OnInspectorNUI()
        {
            if (!base.OnInspectorNUI())
                return false;

            drawer.BeginSubsection("Debug value");

            drawer.Field("_currentHitConfirmOffense");

            drawer.EndSubsection();

            EditorGUI.BeginChangeCheck();

            //Deflection
            DrawSpecificHitConfirmData("Deflection");

            //Evasion
            DrawSpecificHitConfirmData("Evasion");

            //Sweep
            DrawSpecificHitConfirmData("Sweep");

            //Strike
            DrawSpecificHitConfirmData("Strike");

            //Heavy
            DrawSpecificHitConfirmData("Heavy");

            //Deathblow
            DrawSpecificHitConfirmData("Deathblow");

            drawer.EndEditor(this);
            return true;
        }

        void DrawSpecificHitConfirmData(string pHitConfirmOffenseDataType) {

            drawer.BeginSubsection($"{pHitConfirmOffenseDataType}");

            drawer.Property($"_hitConfirmOffense{pHitConfirmOffenseDataType}Data");

            drawer.EndSubsection();
        }
    }

#endif
}