using System;

using UnityEngine;
using SturdyMachine.Features.Focus;
using SturdyMachine.Component;
using SturdyMachine.Features.Fight;
using SturdyMachine.Manager;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace SturdyMachine.Features.HitConfirm {

    [Serializable]
    public partial class HitConfirmModule : FeatureModule {

        #region Attribut

        [SerializeField]
        AudioSource _audioSource;

        [SerializeField]
        AudioClip _audioClip;

        [SerializeField]
        float _basePitch;

        [SerializeField]
        float _pitchMultiplicator;

        Main _main;

        bool _isAlreadyPlayed;

        #endregion

        #region Get

        public override FeatureModuleCategory GetFeatureModuleCategory()
        {
            return FeatureModuleCategory.HitConfirm;
        }

        #endregion

        #region Method

        public override void Initialize()
        {
            base.Initialize();

            _audioSource.clip = _audioClip;

            _audioSource.pitch = _basePitch;
        }

        public override void OnAwake(SturdyComponent pSturdyComponent)
        {
            base.OnAwake(pSturdyComponent);

            _main = pSturdyComponent.GetComponent<Main>();
        }

        public override bool OnUpdate()
        {
            if (!base.OnUpdate())
                return false;

            PlayHitConfirm();

            return true;
        }

        void PlayHitConfirm() {

            if (!_main.GetFeatureManager.GetFeatureModule<FightModule>().GetIsHitConfirm) {

                _isAlreadyPlayed = false;

                return;
            }
                
            if (!_isAlreadyPlayed) {

                _audioSource.Play();

                _audioSource.pitch += _main.GetMonsterBot[_main.GetFeatureManager.GetFeatureModule<FocusModule>().GetCurrentMonsterBotIndex].GetHitConfirmValue;

                _isAlreadyPlayed = true;
            }
        }

        #endregion
    }

#if UNITY_EDITOR

    [CustomPropertyDrawer(typeof(HitConfirmModule))]
    public partial class HitConfirmModuleDrawer : FeatureModuleDrawer
    {
        public override bool OnNUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (!base.OnNUI(position, property, label))
                return false;

            drawer.Field("_pitchMultiplicator", false);

            if (drawer.Field("_audioSource").objectReferenceValue) {

                if (drawer.Field("_audioClip").objectReferenceValue)
                    drawer.Field("_basePitch");
            }

            drawer.EndProperty();
            return true;
        }
    }

#endif
}