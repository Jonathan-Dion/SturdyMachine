using System;

using UnityEngine;
using SturdyMachine.Offense;
using SturdyMachine.Features;
using SturdyMachine.Features.TimeAND;


#if UNITY_EDITOR
using UnityEditor;
using NWH.VehiclePhysics2;
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

        Color GetTimeANDColor(TimeANDType pTimeANDType) {

            //Advantage
            if (pTimeANDType == TimeANDType.Advantage)
                return _timeANDData.timeAdvantageColor;

            //Disadvantage
            if (pTimeANDType == TimeANDType.Disadvantage)
                return _timeANDData.timeDisadvantageColor;

            return _timeANDData.timeNeutralColor;
        }

        #endregion

        #region Methods

        public virtual bool OnUpdate(OffenseDirection pOffenseDirection, OffenseType pOffenseType, bool pIsCooldownActivated, TimeANDType pTimeANDType,  
            bool pIsHitConfirmActivated, AnimationClipOffenseType pAnimationClipOffenseType = AnimationClipOffenseType.Full, bool pIsForceAudioClip = false)
        {
            if (!base.OnUpdate(pOffenseDirection, pOffenseType, pIsCooldownActivated, pIsHitConfirmActivated, pAnimationClipOffenseType))
                return false;

            if (_timeANDData.timeANDSkinnedMesh)
                _timeANDData.timeANDSkinnedMesh.material.SetColor("_EmissionColor", GetTimeANDColor(pTimeANDType));

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