using System;
using System.Collections.Generic;

using UnityEngine;

using ICustomEditor.ScriptableObjectEditor;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace SturdyMachine.Offense.Blocking
{
    public enum BlockingType { Second, FrameRate }

    [CreateAssetMenu(fileName = "NewOffenseBlockingConfig", menuName = "Offence/CustomBlocingOffense", order = 51)]
    public class OffenseBlocking : ScriptableObjectICustomEditor
    {
        public Offense _offense;

        public Vector2 _blockingRange;

        public BlockingType _blockingType;

#if UNITY_EDITOR

        public override void CustomOnInspectorGUI()
        {
            _offense = EditorGUILayout.ObjectField(_offense, typeof(Offense), true) as Offense;

            EditorGUILayout.Space();

            if (_offense)
            {
                if (_offense.GetClip)
                {
                    EditorGUILayout.BeginVertical(GUI.skin.window, GUILayout.Height(10f));

                    EditorGUILayout.BeginHorizontal();

                    EditorGUILayout.LabelField("Type: ", _guiStyle, GUILayout.Width(50f));

                    _blockingType = (BlockingType)EditorGUILayout.EnumPopup(_blockingType, _guiStyle);

                    EditorGUILayout.EndHorizontal();

                    //Second
                    if (_blockingType == BlockingType.Second)
                    {
                        ClipBlockingField($"{_offense.GetClip.length} seconds", new Vector2(Mathf.Clamp(_blockingRange.x, 0f, _offense.GetClip.length),
                                                                                            Mathf.Clamp(_blockingRange.y, 0f, _offense.GetClip.length)));


                    }

                    //Frame
                    else if (_blockingType == BlockingType.FrameRate)
                    {
                        ClipBlockingField($"{_offense.GetClip.frameRate} frames", new Vector2(Mathf.Clamp(_blockingRange.x, 0f, _offense.GetClip.frameRate),
                                                                                              Mathf.Clamp(_blockingRange.y, 0f, _offense.GetClip.frameRate)));
                    }
                }
            }
        }

        public override void CustomOnDisable()
        {
            base.CustomOnDisable();
        }

        void ClipBlockingField(string pLabelField, Vector2 pRangeValue) 
        {
            EditorGUILayout.LabelField(pLabelField);

            EditorGUILayout.Space();

            EditorGUILayout.EndVertical();
            EditorGUILayout.EndVertical();

            EditorGUILayout.Space();

            //Min
            RangeBlockingField("Min: ", ref _blockingRange.x);

            //Max
            RangeBlockingField("Max: ", ref _blockingRange.y);
        }

        void RangeBlockingField(string pLabelText, ref float pRangeValue) 
        {
            EditorGUILayout.BeginHorizontal();

            EditorGUILayout.LabelField(pLabelText, _guiStyle, GUILayout.Width(50f));

            pRangeValue = EditorGUILayout.FloatField(pRangeValue, _guiStyle);

            EditorGUILayout.EndHorizontal();
        }

#endif
    }
}