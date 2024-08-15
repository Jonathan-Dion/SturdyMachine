using System;

using UnityEngine;
using SturdyMachine.Offense;
using NWH.VehiclePhysics2;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace SturdyMachine.Bot
{
    [Serializable]
    struct TimeANDData {

        public SkinnedMeshRenderer timeANDSkinnedMesh;

        [ColorUsage(true, true)]
        public Color timeDisadvantageColor;

        [ColorUsage(true, true)]
        public Color timeNeutralColor;

        [ColorUsage(true, true)]
        public Color timeAdvantageColor;
    }

    [Serializable]
    public partial class SturdyBot : Bot 
    {
        #region Attributes

        [SerializeField]
        TimeANDData _timeANDData;

        #endregion

        #region Properties

        Color GetTimeANDColor(CooldownType pCooldownType) {

            //Advantage
            if (pCooldownType == CooldownType.ADVANTAGE)
                return _timeANDData.timeAdvantageColor;

            //Disadvantage
            if (pCooldownType == CooldownType.DISADVANTAGE)
                return _timeANDData.timeDisadvantageColor;

            return _timeANDData.timeNeutralColor;
        }

        #endregion

        #region Methods

        public override bool OnUpdate(OffenseDirection pOffenseDirection, OffenseType pOffenseType, CooldownType pCurrentCooldownType, AnimationClipOffenseType pAnimationClipOffenseType = AnimationClipOffenseType.Full)
        {
            if (!base.OnUpdate(pOffenseDirection, pOffenseType, pCurrentCooldownType, pAnimationClipOffenseType))
                return false;

            if (_timeANDData.timeANDSkinnedMesh)
                _timeANDData.timeANDSkinnedMesh.material.SetColor("_EmissionColor", GetTimeANDColor(pCurrentCooldownType));

            return true;
        }

        #endregion
    }

#if UNITY_EDITOR

    [CustomEditor(typeof(SturdyBot))]
    public class SturdyBotEditor : BotEditor
    {
        public override bool OnInspectorNUI()
        {
            if (!base.OnInspectorNUI())
                return false;

            drawer.Field("_timeANDData");
            
            return true;
        }
    }

    [CustomPropertyDrawer(typeof(TimeANDData))]
    public partial class TimeANDDataDrawer : ComponentNUIPropertyDrawer
    {
        public override bool OnNUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (!base.OnNUI(position, property, label))
                return false;

            drawer.Field("timeANDSkinnedMesh");

            drawer.BeginSubsection("Color");

            drawer.Field("timeDisadvantageColor", true, null, "Disadvantage: ");
            drawer.Field("timeNeutralColor", true, null, "Neutral: ");
            drawer.Field("timeAdvantageColor", true, null, "Advantage: ");

            drawer.EndSubsection();


            drawer.EndProperty();
            return true;
        }
    }

#endif
}