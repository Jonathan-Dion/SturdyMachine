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

    [Serializable]
    public class OffenseBlockingData : ICustomEditor.ICustomEditor
    {
        public Offense offense;
        public Vector2 blockingRange, blockingData;
        public BlockingType blockingType;

        bool _isAdvancedSettings;

        bool _isInitialized;

        public bool GetIsInitialized => _isInitialized;

#if UNITY_EDITOR

        public virtual void Initialize() 
        {
            //Min
            blockingData.x /= blockingRange.x;

            //Max
            blockingData.y /= blockingRange.y;

            _isInitialized = true;
        }

        public virtual void CustomOnEnable()
        {
            if (offense == null)
                offense = new Offense();
        }

        public virtual void CustomOnDisable()
        {
            
        }

        public virtual void CustomOnDestroy()
        {
            
        }

        public void CustomOnInspectorGUI()
        {
            
        }

        public virtual void CustomOnInspectorGUI(GUIStyle pGuiStyle)
        {
            EditorGUILayout.BeginVertical(GUI.skin.window, GUILayout.Height(10f));

            offense = EditorGUILayout.ObjectField(offense, typeof(Offense), true) as Offense;

            EditorGUILayout.Space();

            EditorGUILayout.BeginHorizontal();

            EditorGUILayout.LabelField("Advanced settings: ", pGuiStyle);

            _isAdvancedSettings = EditorGUILayout.Toggle(_isAdvancedSettings);

            EditorGUILayout.EndHorizontal();

            if (_isAdvancedSettings)
            {
                if (offense)
                {
                    if (offense.GetClip)
                    {
                        //Set blockingType
                        EditorGUILayout.BeginHorizontal();

                        EditorGUILayout.LabelField("Type: ", pGuiStyle, GUILayout.Width(50f));

                        blockingType = (BlockingType)EditorGUILayout.EnumPopup(blockingType);

                        EditorGUILayout.EndHorizontal();

                        //Second
                        if (blockingType == BlockingType.Second)
                            ClipBlockingField($"{offense.GetClip.length} seconds", offense.GetClip.length, pGuiStyle);

                        //Frame
                        else if (blockingType == BlockingType.FrameRate)
                            ClipBlockingField($"{offense.GetClip.frameRate} frames", offense.GetClip.frameRate, pGuiStyle);
                    }
                }

                EditorGUILayout.Space();
            }

            EditorGUILayout.EndVertical();
        }

        public virtual void CustomOnSceneGUI()
        {
            
        }

        void ClipBlockingField(string pLabelField, float pRangeValue, GUIStyle pGuiStyle)
        {
            EditorGUILayout.LabelField(pLabelField, pGuiStyle);

            EditorGUILayout.Space();

            EditorGUILayout.BeginVertical(GUI.skin.window, GUILayout.Height(10f));

            //Min
            RangeBlockingField("Min: ", pRangeValue, ref blockingRange.x, pGuiStyle);

            //Max
            RangeBlockingField("Max: ", pRangeValue, ref blockingRange.y, pGuiStyle);

            EditorGUILayout.EndVertical();
        }

        void RangeBlockingField(string pLabelText, float pRangeValue, ref float pBlockingValue, GUIStyle pGuiStyle)
        {
            EditorGUILayout.BeginHorizontal(GUI.skin.box);

            EditorGUILayout.LabelField(pLabelText, pGuiStyle, GUILayout.Width(40f));

            pBlockingValue = EditorGUILayout.FloatField(Mathf.Clamp(pBlockingValue, 0f, pRangeValue), GUILayout.Width(50f));

            EditorGUILayout.EndHorizontal();
        }


#endif
    }

    [CreateAssetMenu(fileName = "NewOffenseBlockingConfig", menuName = "Offence/CustomBlocingOffense", order = 51)]
    public class OffenseBlocking : ScriptableObjectICustomEditor
    {
        [SerializeField]
        List<OffenseBlockingData> _offenseBlockingData;

        public bool GetIsBlocking(Offense pOffense, float pNormalizedTime) 
        {
            for (int i = 0; i < _offenseBlockingData.Count; ++i) 
            {
                if (_offenseBlockingData[i].offense == pOffense)
                {
                    if (Mathf.Clamp(pNormalizedTime, _offenseBlockingData[i].blockingData.x, _offenseBlockingData[i].blockingData.y) == pNormalizedTime)
                        return true;

                    break;
                }
            }

            return false;
        }


#if UNITY_EDITOR

        public override void CustomOnEnable()
        {
            if (_offenseBlockingData == null)
                _offenseBlockingData = new List<OffenseBlockingData>();
        }

        public virtual void CustomOnInspectorGUI(string pCurrentAssetPath, string pNewAssetPath)
        {
            ShowDefaultValue(pCurrentAssetPath, pNewAssetPath);
        }

        public override void CustomOnDisable()
        {
            base.CustomOnDisable();
        }

        void ShowDefaultValue(string pCurrentAssetPath, string pNewAssetPath) 
        {
            #region Information

            EditorGUILayout.BeginVertical();

            GUILayout.Label("Informations", _guiStyle);

            EditorGUILayout.Space();

            EditorGUILayout.BeginHorizontal(GUI.skin.box);

            //Add
            if (GUILayout.Button("+"))
            {
                if (_offenseBlockingData == null)
                    _offenseBlockingData = new List<OffenseBlockingData>();

                _offenseBlockingData.Add(new OffenseBlockingData());

                AssetDatabase.Refresh();
            }

            //Remove
            if (GUILayout.Button("-"))
            {
                if (_offenseBlockingData != null)
                {
                    if (_offenseBlockingData.Count > 0)
                        _offenseBlockingData.RemoveAt(_offenseBlockingData.Count - 1);
                }

                AssetDatabase.Refresh();
            }

            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space();

            if (_offenseBlockingData != null)
            {
                if (_offenseBlockingData.Count != 0)
                {
                    if (GUILayout.Button("Save"))
                    {
                        FileMoving(pCurrentAssetPath, pNewAssetPath);

                        AssetDatabase.Refresh();
                        AssetDatabase.SaveAssets();
                    }
                }
            }

            EditorGUILayout.EndVertical();

            #endregion

            for (int i = 0; i < _offenseBlockingData.Count; ++i)
            {
                if (_offenseBlockingData[i] != null)
                    _offenseBlockingData[i].CustomOnInspectorGUI(_guiStyle);
            }
        }

#endif
    }
}