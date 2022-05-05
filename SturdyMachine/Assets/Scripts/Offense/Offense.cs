using UnityEngine;

using ICustomEditor.ScriptableObjectEditor;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace SturdyMachine.Offense 
{
    [CreateAssetMenu(fileName = "NewCustomAnimation", menuName = "Offence/CustomOffence", order = 1)]
    public class Offense : ScriptableObjectICustomEditor
    {
        [SerializeField]
        protected AnimationClip _clip;

        [SerializeField]
        protected OffenseDirection _offenseDirection;

        [SerializeField]
        protected OffenseType _offenseType;

        [SerializeField]
        protected AnimationClip _repelClip;

        [SerializeField]
        float _maxCooldownTime;

        public OffenseDirection GetOffenseDirection => _offenseDirection;
        public OffenseType GetOffenseType => _offenseType;

        public AnimationClip GetClip => _clip;
        public AnimationClip GetRepelClip => _repelClip;
        public bool GetIsCooldownAvailable => _maxCooldownTime > 0;
        public float GetMaxCooldownTime => _maxCooldownTime;

        public bool GetIsGoodOffense(OffenseDirection pOffenseDirection, OffenseType pOffenseType)
        {
            if (pOffenseDirection == _offenseDirection)
                if (pOffenseType == _offenseType)
                    return true;

            return false;
        }

#if UNITY_EDITOR
        public override void CustomOnInspectorGUI()
        {
            EditorGUILayout.BeginVertical(GUI.skin.box);

            GUILayout.Label("Offense", _guiStyle);

            EditorGUILayout.Space();

            #region Clip information

            EditorGUILayout.BeginVertical(GUI.skin.box);

            _clip = EditorGUILayout.ObjectField(_clip, typeof(AnimationClip), true) as AnimationClip;

            EditorGUILayout.Space();

            _repelClip = EditorGUILayout.ObjectField(_repelClip, typeof(AnimationClip), true) as AnimationClip;

            EditorGUILayout.Space();

            EditorGUILayout.EndVertical();

            #endregion

            #region Offense informations

            #region Offense configuration

            EditorGUILayout.BeginVertical(GUI.skin.box);

            GUILayout.Label("Informations:", _guiStyle);

            _offenseDirection = (OffenseDirection)EditorGUILayout.EnumPopup(_offenseDirection, "Offense Direction: ");
            _offenseType = (OffenseType)EditorGUILayout.EnumPopup(_offenseType, "Offense Type: ");

            EditorGUILayout.EndVertical();

            #endregion

            EditorGUILayout.Space();

            #region Cooldown configuration

            EditorGUILayout.BeginVertical(GUI.skin.box);

            GUILayout.Label("Cooldown", _guiStyle);

            EditorGUILayout.EndVertical();

            _maxCooldownTime = EditorGUILayout.FloatField(_maxCooldownTime, "Max cooldown Time: ");

            #endregion

            #endregion

            EditorGUILayout.EndVertical();
        }

        public override void CustomOnEnable()
        {
            base.CustomOnEnable();
        }

#endif
    }
}