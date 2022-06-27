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

        bool _isShowAdvanced;

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

#if UNITY_EDITOR
        public override void CustomOnInspectorGUI()
        {
            EditorGUILayout.BeginVertical(GUI.skin.box);

            GUILayout.Label("Offense", _guiStyle);

            EditorGUILayout.BeginHorizontal(GUI.skin.box);

            EditorGUILayout.LabelField("Show Advanced: ", _guiStyle);

            _isShowAdvanced = EditorGUILayout.Toggle(_isShowAdvanced);

            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space();

            if (_isShowAdvanced)
            {
                EditorGUI.BeginChangeCheck();

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

                EditorGUILayout.LabelField("Informations: ", _guiStyle);

                EditorGUILayout.BeginHorizontal();

                _offenseDirection = (OffenseDirection)EditorGUILayout.EnumPopup(_offenseDirection);
                _offenseType = (OffenseType)EditorGUILayout.EnumPopup(_offenseType);

                EditorGUILayout.EndHorizontal();

                EditorGUILayout.EndVertical();

                #endregion

                EditorGUILayout.Space();

                #region Cooldown configuration

                EditorGUILayout.BeginVertical(GUI.skin.box);

                GUILayout.Label("Cooldown", _guiStyle);

                EditorGUILayout.EndVertical();

                _maxCooldownTime = EditorGUILayout.FloatField(_maxCooldownTime);

                #endregion

                #endregion

                if (EditorGUI.EndChangeCheck())
                    AssetDatabase.SaveAssets();
            }

            EditorGUILayout.EndVertical();
        }

        public override void CustomOnEnable()
        {
            base.CustomOnEnable();
        }

#endif
    }
}