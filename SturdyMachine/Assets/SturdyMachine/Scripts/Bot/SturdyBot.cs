﻿using System;

using UnityEngine;
using SturdyMachine.Offense;
using SturdyMachine.Features;
using SturdyMachine.Features.NADTime;
using SturdyMachine.Settings.GameplaySettings.NADTimeSettings;
using SturdyMachine.Settings;



#if UNITY_EDITOR
using UnityEditor;
using NWH.VehiclePhysics2;
#endif

namespace SturdyMachine.Bot
{
    [Serializable]
    public partial class SturdyBot : Bot 
    {
        #region Attributes

        [SerializeField]
        SkinnedMeshRenderer[] _nadTimeSkinnedMesh;

        #endregion

        #region Methods

        public virtual bool OnUpdate(OffenseDirection pOffenseDirection, OffenseType pOffenseType, bool pIsCooldownActivated, NADTimeType pNADTimeType,  
            bool pIsHitConfirmActivated, AnimationClipOffenseType pAnimationClipOffenseType = AnimationClipOffenseType.Full, bool pIsForceAudioClip = false)
        {
            if (!base.OnUpdate(pOffenseDirection, pOffenseType, pIsCooldownActivated, pIsHitConfirmActivated, pAnimationClipOffenseType))
                return false;

            if (_nadTimeSkinnedMesh.Length > 0) {

                for (byte i = 0; i < _nadTimeSkinnedMesh.Length; ++i)
                {
                    if (!_nadTimeSkinnedMesh[i])
                        continue;

                    _nadTimeSkinnedMesh[i].material.SetColor("_EmissionColor", GameSettings.GetGameSettings().GetGameplaySettings.GetNADTimeSettings.GetCurrentNADTimeMeshColor(pNADTimeType));

                }
            }

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

            drawer.ReorderableList("_nadTimeSkinnedMesh");

            return true;
        }
    }

#endif
}