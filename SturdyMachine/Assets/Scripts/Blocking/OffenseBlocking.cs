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

        public virtual void CustomOnInspectorGUI(string pCurrentAssetPath, string pNewAssetPath)
        {
            _offense = EditorGUILayout.ObjectField(_offense, typeof(Offense), true) as Offense;

            EditorGUILayout.Space();

            if (_offense)
            {
                if (_offense.GetClip)
                {
                    EditorGUILayout.BeginVertical(GUI.skin.window, GUILayout.Height(10f));

                    //Set blockingType
                    EditorGUILayout.BeginHorizontal();

                    EditorGUILayout.LabelField("Type: ", _guiStyle, GUILayout.Width(50f));

                    _blockingType = (BlockingType)EditorGUILayout.EnumPopup(_blockingType);

                    EditorGUILayout.EndHorizontal();

                    //Second
                    if (_blockingType == BlockingType.Second)
                        ClipBlockingField($"{_offense.GetClip.length} seconds", _offense.GetClip.length);

                    //Frame
                    else if (_blockingType == BlockingType.FrameRate)
                        ClipBlockingField($"{_offense.GetClip.frameRate} frames", _offense.GetClip.frameRate);

                    EditorGUILayout.Space();

                    if (GUILayout.Button("Save")) 
                    {
                        FileMoving(pCurrentAssetPath, pNewAssetPath);

                        AssetDatabase.Refresh();
                        AssetDatabase.SaveAssets();
                    }
                }
            }
        }

        public override void CustomOnDisable()
        {
            base.CustomOnDisable();
        }

        void ClipBlockingField(string pLabelField, float pRangeValue) 
        {
            EditorGUILayout.LabelField(pLabelField, _guiStyle);

            EditorGUILayout.EndVertical();

            EditorGUILayout.Space();

            EditorGUILayout.BeginVertical(GUI.skin.window, GUILayout.Height(10f));

            //Min
            RangeBlockingField("Min: ", pRangeValue, ref _blockingRange.x);

            //Max
            RangeBlockingField("Max: ", pRangeValue, ref _blockingRange.y);

            EditorGUILayout.EndVertical();
        }

        void RangeBlockingField(string pLabelText, float pRangeValue, ref float pBlockingValue) 
        {
            EditorGUILayout.BeginHorizontal(GUI.skin.box);

            EditorGUILayout.LabelField(pLabelText, _guiStyle, GUILayout.Width(40f));

            pBlockingValue = EditorGUILayout.FloatField(Mathf.Clamp(pBlockingValue, 0f, pRangeValue), GUILayout.Width(50f));

            EditorGUILayout.EndHorizontal();
        }

#endif
    }
}