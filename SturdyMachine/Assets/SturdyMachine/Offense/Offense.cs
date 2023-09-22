using UnityEngine;

using SturdyMachine.Utilities;
using System;
using NWH.VehiclePhysics2;
using UnityEditor.Experimental.GraphView;

#if UNITY_EDITOR
using NWH.NUI;
using UnityEditor;
#endif

namespace SturdyMachine.Offense 
{
    /// <summary>
    /// Store all type of cooldown for this Offense
    /// </summary>
    [Serializable]
    public struct CooldownData {

        /// <summary>
        /// Cooldown timer when this are Attack type and is blocked with another offense.
        /// </summary>
        [Tooltip("Cooldown timer when this are Attack type and is blocked with another offense.")]
        public float blockingCooldown;

        /// <summary>
        /// Cooldown when this offense are attack type and is used
        /// </summary>
        [Tooltip("Cooldown when this offense are attack type and is used")]
        public float offenseCooldown;

        /// <summary>
        /// Cooldown when this offense is special offense and needs charging time after being used
        /// </summary>
        [Tooltip("Cooldown when this offense is special offense and needs charging time after being used")]
        public float maxCooldown;
    }

    /// <summary>
    /// Store basic Offense information
    /// </summary>
    [CreateAssetMenu(fileName = "NewCustomAnimation", menuName = "Offence/CustomOffence", order = 1)]
    public class Offense : ScriptableObject
    {
        #region Property

        /// <summary>
        /// Manage the current state of this object
        /// </summary>
        protected SturdyComponent _sturdyComponent;

        /// <summary>
        /// The animationClip for the current Offense
        /// </summary>
        [SerializeField, Tooltip("The animationClip for the current Offense")]
        protected AnimationClip _clip;

        /// <summary>
        /// The current direction of this offense.
        /// </summary>
        [SerializeField, Tooltip("The current direction of this offense.")]
        protected OffenseDirection _offenseDirection;

        /// <summary>
        /// The current offense tyoe of this offense
        /// </summary>
        [SerializeField, Tooltip("The current offense tyoe of this offense")]
        protected OffenseType _offenseType;

        /// <summary>
        /// The animationClip when this offense type are Deflection and when this offense block another offense
        /// </summary>
        [SerializeField, Tooltip("The animationClip when this offense type are Deflection and when this offense block another offense")]
        protected AnimationClip _repelClip;

        /// <summary>
        /// Store all type of cooldown for this Offense
        /// </summary>
        [SerializeField, Tooltip("Store all type of cooldown for this Offense")]
        protected CooldownData _cooldownData;

        [SerializeField]
        float _maxCooldownTime;

        #endregion

        #region Get

        public OffenseDirection GetOffenseDirection => _offenseDirection;
        public OffenseType GetOffenseType => _offenseType;

        public AnimationClip GetClip => _clip;
        public AnimationClip GetRepelClip => _repelClip;
        public bool GetIsCooldownAvailable => _maxCooldownTime > 0;
        public float GetMaxCooldownTime => _maxCooldownTime;

        public bool GetIsGoodOffense(OffenseDirection pOffenseDirection, OffenseType pOffenseType)
        {
            if (pOffenseDirection == _offenseDirection) 
            {
                //Repel
                if (pOffenseType == OffenseType.REPEL)
                {
                    if (_repelClip)
                        return true;
                }
                else if (pOffenseType == _offenseType)
                    return true;
            }

            return false;
        }

        #endregion

#if UNITY_EDITOR

        [CustomEditor(typeof(Offense))]
        public class OffenseEditor : NUIEditor
        {
            public override bool OnInspectorNUI()
            {
                if (!base.OnInspectorNUI())
                    return false;

                EditorGUI.BeginChangeCheck();

                drawer.BeginSubsection("Offense");

                ShowOffenseInformations();

                ShowClip();

                ShowCooldown();

                drawer.EndSubsection();

                drawer.EndEditor(this);
                return true;
            }

            void ShowClip() {

                drawer.BeginSubsection("Animation clip");

                drawer.Field("_clip");
                drawer.Field("_repelClip", true, null, "Repel: ");

                drawer.EndSubsection();
            }

            void ShowOffenseInformations() {

                drawer.BeginSubsection("Informations");

                drawer.Field("_offenseDirection", true, null, "Direction: ");
                drawer.Field("_offenseType", true, null, "Type: ");

                drawer.EndSubsection();
            }

            void ShowCooldown() {

                drawer.BeginSubsection("Cooldown");

                drawer.Field("_maxCooldownTime");

                drawer.Property("_cooldownData");

                drawer.EndSubsection();
            }
        }

        [CustomPropertyDrawer(typeof(CooldownData))]
        public partial class CooldownDataDrawer : ComponentNUIPropertyDrawer
        {
            public override bool OnNUI(Rect position, SerializedProperty property, GUIContent label)
            {
                if (!base.OnNUI(position, property, label))
                    return false;

                drawer.Field("blockingCooldown", true, "sec", "Blocking: ");
                drawer.Field("offenseCooldown", true, "sec", "Offense: ");
                drawer.Field("maxCooldown", true, "sec", "Max: ");

                drawer.EndProperty();
                return true;
            }
        }

#endif
    }
}