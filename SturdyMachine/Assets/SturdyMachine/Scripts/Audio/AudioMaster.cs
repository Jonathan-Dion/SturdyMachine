using System;

using UnityEngine;
using SturdyMachine.Offense;


#if UNITY_EDITOR
using UnityEditor;
using NWH.NUI;
#endif

namespace SturdyMachine.Audio {

    public class AudioOffenseMaster{

        #region Attributes

        AudioSource _currentAudioSource;

        OffenseType _currentAudioOffenseType;

        OffenseDirection _currentAudioOffenseDirection;

        #endregion

        #region Properties

        public AudioOffenseMaster(AudioSource pAudioSource) {
        
            _currentAudioSource = pAudioSource;
        }

        #endregion

        #region Methods

        public void UpdateAudio(OffenseType pNextOffenseType, OffenseDirection pNextOffenseDirection, AnimationClipOffenseType pAnimationClipOffenseType, AudioClip pNextAudioOffenseClip) {

            if (_currentAudioOffenseType == pNextOffenseType){

                if (_currentAudioOffenseDirection == pNextOffenseDirection)
                    return;
            }

            _currentAudioSource.Stop();

            _currentAudioSource.clip = pNextAudioOffenseClip;

            if (_currentAudioSource.clip)
                _currentAudioSource.Play();

            _currentAudioOffenseType = pNextOffenseType;
            _currentAudioOffenseDirection = pNextOffenseDirection;
        }

        #endregion
    }

#if UNITY_EDITOR

    [CustomEditor(typeof(AudioOffenseMaster))]
    public class AudioOffenseMasterEditor : NUIEditor{

        public override bool OnInspectorNUI()
        {
            if (!base.OnInspectorNUI())
                return false;

            EditorGUI.BeginChangeCheck();

            

            drawer.EndEditor(this);
            return true;
        }
    }

#endif

}