using System;

using UnityEngine;

using SturdyMachine.Offense;
using SturdyMachine.Component;
using UnityEngine.UIElements;


#if UNITY_EDITOR
using UnityEditor;
using NWH.VehiclePhysics2;
using NWH.NUI;
#endif

namespace SturdyMachine.ParticlesState {

    [Serializable]
    public struct ParticlesStateOffenseData {

        public OffenseType offenseType;
        public OffenseDirection offenseDirection;
    }

    [SerializeField]
    public struct ParticlesStateData {
    
        public ParticleSystem[] particleSystems;

        public ParticlesStateOffenseData[] particlesStateOffenseData;
    }

    public class ParticlesState : MonoBehaviour{

        #region Attributes

        [SerializeField]
        ParticlesStateData[] _particlesStateData;

        ParticleSystem[] _currentParticleSystem;

        #endregion

        #region Properties

        ParticleSystem[] GetCurrentParticlesSystemAccordingToOffense(OffenseType pOffenseType, OffenseDirection pOffenseDirection)
        {

            for (byte i = 0; i < _particlesStateData.Length; ++i)
            {

                for (byte j = 0; j < _particlesStateData[i].particlesStateOffenseData.Length; ++j)
                {

                    if (_particlesStateData[i].particlesStateOffenseData[j].offenseType != pOffenseType)
                        continue;

                    //Default
                    if (_particlesStateData[i].particlesStateOffenseData[j].offenseDirection == OffenseDirection.DEFAULT)
                        return _particlesStateData[i].particleSystems;

                    if (_particlesStateData[i].particlesStateOffenseData[j].offenseDirection != pOffenseDirection)
                        continue;

                    return _particlesStateData[i].particleSystems;
                }
            }

            return null;
        }

        #endregion

        #region Methods

        public ParticlesState(ParticlesStateData[] pParticlesStateData) {

            _particlesStateData = pParticlesStateData;
        }

        public void OnUpdate(OffenseType pOffenseType, OffenseDirection pOffenseDirection, bool pIsHitConfirmActivated) {

            if (_particlesStateData == null)
                return;

            _currentParticleSystem = GetCurrentParticlesSystemAccordingToOffense(pOffenseType, pOffenseDirection);

            if (_currentParticleSystem == null)
                return;

            for (byte i = 0; i < _currentParticleSystem.Length; ++i) {

                if (!pIsHitConfirmActivated) {

                    if (_currentParticleSystem[i].isPlaying)
                        continue;

                    _currentParticleSystem[i].Play();

                    continue;
                }

                if (_currentParticleSystem[i].isPaused)
                    continue;

                _currentParticleSystem[i].Pause();
            }
        }

        #endregion
    }

#if UNITY_EDITOR

    [CustomEditor(typeof(ParticlesState))]
    public class ParticlesStateDrawer : BaseComponentEditor{
        
        public override bool OnInspectorNUI()
        {
            if (!base.OnInspectorNUI())
                return false;

            drawer.ReorderableList("_particlesStateData");

            drawer.EndEditor(this);
            return true;
        }
    }

    [CustomPropertyDrawer(typeof(ParticlesStateOffenseData))]
    public partial class ParticlesStateOffenseDataDrawer : ComponentNUIPropertyDrawer
    {
        public override bool OnNUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (!base.OnNUI(position, property, label))
                return false;

            drawer.Field("offenseType", false, null, "Type: ");
            drawer.Field("offenseDirection", false, null, "Direction: ");

            drawer.EndProperty();
            return true;
        }
    }

    [CustomPropertyDrawer(typeof(ParticlesStateData))]
    public partial class ParticlesStateDataDrawer : ComponentNUIPropertyDrawer
    {
        public override bool OnNUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (!base.OnNUI(position, property, label))
                return false;

            drawer.ReorderableList("particleSystems");
            drawer.ReorderableList("particlesStateOffenseData");

            drawer.EndProperty();
            return true;
        }
    }

#endif
}