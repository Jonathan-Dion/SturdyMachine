using System;

using UnityEngine;

using SturdyMachine.Component;
using SturdyMachine.Offense;
using SturdyMachine.Audio;

#if UNITY_EDITOR
using UnityEditor;
using NWH.NUI;
using NWH.VehiclePhysics2;
#endif

namespace SturdyMachine.Equipment 
{
    [Serializable]
    public struct AudioOffenseEquipmentData {

        public OffenseType offenseType;
        public OffenseDirection offenseDirection;

        public AnimationClipOffenseType animationClipOffenseType;

        public AudioClip audioClip;

        public bool GetIfIsGoodAudioClip(OffenseType pCurrentOffenseType, OffenseDirection pCurrentOffenseDirection, AnimationClipOffenseType pAnimationClipOffenseType) {

            if (offenseType != pCurrentOffenseType)
                return false;

            if (offenseDirection != OffenseDirection.DEFAULT) {

                if (offenseDirection != pCurrentOffenseDirection)
                    return false;
            }

            if (animationClipOffenseType != pAnimationClipOffenseType)
                return false;

            return true;
        }
    }

    /// <summary>
    /// Base class for all equipment
    /// </summary>
    public abstract class Equipment : BaseComponent
    {
        /// <summary>
        /// Mesh component for this equipment
        /// </summary>
        [SerializeField, Tooltip("Mesh component for this equipment")]
        protected MeshRenderer _meshRenderer;

        /// <summary>
        /// BoxCollider component for this equipment
        /// </summary>
        [SerializeField, Tooltip("BoxCollider component for this equipment")]
        protected BoxCollider _boxCollider;

        /// <summary>
        /// Rigidbody component for this equipment
        /// </summary>
        [SerializeField, Tooltip("Rigidbody component for this equipment")]
        protected Rigidbody _rigidbody;

        [SerializeField]
        protected AudioOffenseEquipmentData[] _audioOffenseEquipmentData;

        [SerializeField]
        AudioOffenseMaster _audioOffenseMaster;

        [SerializeField]
        AudioSource _audioSource;

        AudioClip _currentAudioClip;

        AudioClip GetCurrentAudioOffenseWithAnimationClipOffenseType(OffenseType pCurrentOffenseType, OffenseDirection pCurrentOffenseDirection, AnimationClipOffenseType pAnimationClipOffenseType) {

            for (byte i = 0; i < _audioOffenseEquipmentData.Length; ++i) {

                if (!_audioOffenseEquipmentData[i].GetIfIsGoodAudioClip(pCurrentOffenseType, pCurrentOffenseDirection, pAnimationClipOffenseType))
                    continue;

                return _audioOffenseEquipmentData[i].audioClip;
            }

            return null;
        }

        public override void OnAwake()
        {
            base.OnAwake();

            _meshRenderer = GetComponent<MeshRenderer>();
            _boxCollider = GetComponent<BoxCollider>();
            _rigidbody = GetComponent<Rigidbody>();

            _audioOffenseMaster = new AudioOffenseMaster(_audioSource);
        }

        public virtual bool OnUpdate(OffenseType pCurrentOffenseType, OffenseDirection pCurrentOffenseDirection, AnimationClipOffenseType  pAnimationClipOffenseType)
        {
            if (!base.OnUpdate())
                return false;

            _audioOffenseMaster.UpdateAudio(pCurrentOffenseType, pCurrentOffenseDirection, pAnimationClipOffenseType, GetCurrentAudioOffenseWithAnimationClipOffenseType(pCurrentOffenseType, pCurrentOffenseDirection, pAnimationClipOffenseType));

            return true;
        }
    }

#if UNITY_EDITOR

    [CustomEditor(typeof(Equipment))]
    public class EquipmentEditor : NUIEditor
    {
        public override bool OnInspectorNUI()
        {
            if (!base.OnInspectorNUI())
                return false;

            drawer.BeginSubsection("Debug Value");

            drawer.Space();

            drawer.Field("_meshRenderer", false);
            drawer.Field("_boxCollider", false);
            drawer.Field("_rigidbody", false);

            drawer.EndSubsection();

            drawer.BeginSubsection("Configuration");

            drawer.Field("_audioSource");
            drawer.ReorderableList("_audioOffenseEquipmentData");

            drawer.EndSubsection();

            drawer.EndEditor(this);
            return true;
        }
    }

    [CustomPropertyDrawer(typeof(AudioOffenseEquipmentData))]
    public partial class AudioOffenseEquipmentDataDrawer : ComponentNUIPropertyDrawer
    {
        public override bool OnNUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (!base.OnNUI(position, property, label))
                return false;

            if (drawer.Field("offenseType").enumValueIndex != 0) {

                drawer.Field("offenseDirection");
                drawer.Field("animationClipOffenseType");
                drawer.Field("audioClip");
            }                       

            drawer.EndProperty();
            return true;
        }
    }

#endif

}