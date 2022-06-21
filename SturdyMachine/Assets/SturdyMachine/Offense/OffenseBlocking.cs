﻿using System;
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

        public virtual void Initialize() 
        {
            //Min
            BlockingDataInitialize(blockingType == BlockingType.Second ? offense.GetClip.length : offense.GetClip.frameRate, blockingRange.x, ref blockingData.x);

            //Max
            BlockingDataInitialize(blockingType == BlockingType.Second ? offense.GetClip.length : offense.GetClip.frameRate, blockingRange.y, ref blockingData.y);
            
            _isInitialized = true;
        }

        public bool GetIsGoodOffenseBlocking(Offense pCurrentOffense) 
        {
            if (offense == pCurrentOffense)
                return true;

            return false;
        }

#if UNITY_EDITOR

        public virtual void CustomOnEnable()
        {
            if (offense == null)
                offense = new Offense();
        }

        public virtual void CustomOnDisable() { }

        public virtual void CustomOnDestroy() { }

        public void CustomOnInspectorGUI() { }

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
                        EditorGUI.BeginChangeCheck();

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

                        //SaveAsset
                        if (EditorGUI.EndChangeCheck())
                            AssetDatabase.SaveAssets();
                    }
                }

                EditorGUILayout.Space();
            }

            EditorGUILayout.EndVertical();
        }

        public virtual void CustomOnSceneGUI() { }

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

            pBlockingValue = EditorGUILayout.FloatField(Mathf.Clamp(pBlockingValue, 0f, pRangeValue * 0.90f), GUILayout.Width(100f));

            EditorGUILayout.EndHorizontal();
        }

        void BlockingDataInitialize(float pClipSize, float pBlockingRangeValue, ref float pBlockingDataValue) 
        {
            pBlockingDataValue = pBlockingRangeValue / pClipSize;
        }

#endif
    }

    [CreateAssetMenu(fileName = "NewOffenseBlockingConfig", menuName = "Offence/CustomBlocingOffense", order = 51)]
    public class OffenseBlocking : ScriptableObjectICustomEditor
    {
        [SerializeField]
        List<OffenseBlockingData> _offenseBlockingData;

        [SerializeField]
        Offense _deflectionOffense;

        public List<OffenseBlockingData> GetOffenseBlockingData => _offenseBlockingData;

#if UNITY_EDITOR
        bool _isInitialized;

        public bool GetIsInitialzed => _isInitialized;
#endif

        public bool GetIsHitting(Offense pCurrentOffense, Animator pMonsterBotAnimator) 
        {
            if (pMonsterBotAnimator.GetCurrentAnimatorClipInfo(0)[0].clip == pCurrentOffense.GetClip)
            {
                for (int i = 0; i < _offenseBlockingData.Count; ++i)
                {
                    if (pCurrentOffense == _offenseBlockingData[i].offense)
                    {
                        if (pMonsterBotAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime >= _offenseBlockingData[i].blockingRange.y)
                            return true;
                    }
                }
            }

            return false;
        }

        public bool GetIsBlocking(Offense pSturdyBotOffense, Offense pMonsterBotOffense, Animator pMonsterBotAnimator) 
        {
            if (pSturdyBotOffense == _deflectionOffense)
            {
                for (int i = 0; i < _offenseBlockingData.Count; ++i)
                {
                    if (pMonsterBotOffense == _offenseBlockingData[i].offense) 
                    {
                        if (Mathf.Clamp(pMonsterBotAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime, _offenseBlockingData[i].blockingRange.x, _offenseBlockingData[i].blockingRange.y) == pMonsterBotAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime)
                            return true;
                    }
                }
            }

            return false;
        }

#if UNITY_EDITOR

        public override void CustomOnEnable()
        {
            base.CustomOnEnable();

            if (_offenseBlockingData == null)
                _offenseBlockingData = new List<OffenseBlockingData>();

            _isInitialized = true;
        }

        public override void CustomOnInspectorGUI()
        {
            ShowDefaultValue();
        }

        public override void CustomOnDisable()
        {
            base.CustomOnDisable();

            _isInitialized = false;
        }

        void ShowDefaultValue() 
        {
            #region Information

            EditorGUILayout.BeginVertical();

            GUILayout.Label("Informations", _guiStyle);

            EditorGUILayout.Space();

            _deflectionOffense = EditorGUILayout.ObjectField(_deflectionOffense, typeof(Offense), true) as Offense;

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
                        AssetDatabase.SaveAssets();

                        AssetDatabase.Refresh();
                    }
                }
            }

            EditorGUILayout.EndVertical();

            #endregion

            if (_offenseBlockingData != null)
            {
                for (int i = 0; i < _offenseBlockingData.Count; ++i)
                {
                    if (_offenseBlockingData[i] != null)
                        _offenseBlockingData[i].CustomOnInspectorGUI(_guiStyle);
                }
            }
        }

#endif
    }
}