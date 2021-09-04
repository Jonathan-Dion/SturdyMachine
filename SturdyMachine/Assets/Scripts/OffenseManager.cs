﻿using UnityEngine;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;
#endif

[CreateAssetMenu(fileName = "NewOffenseManager", menuName = "Offence/Manager", order = 52)]
public class OffenseManager : ScriptableObject
{
    [SerializeField]
    Offense[] _offense;

    [SerializeField]
    Offense[] _stanceOffense;

    Offense _currentOffense, _nextOffense;

    bool _isCooldownActivated;

    float _currentCooldownTime, _currentMaxCooldownTime;

    public bool GetIsCooldownActivated => _isCooldownActivated;
    public float GetCurrentCooldownTime => _currentCooldownTime;
    public float GetMaxCooldownTime => _currentMaxCooldownTime;

    bool GetIsCanceled(AnimationClip pAnimationClip) 
    {
        //Standard
        if (_nextOffense != null)
        {
            if (IsStandardCanceled())
                return true;
        }

        //Stance
        if (IsStanceCanceled(pAnimationClip))
            return true;

        return false;
    }

    bool IsStanceCanceled(AnimationClip pAnimationClip) 
    {
        if (_stanceOffense != null || _stanceOffense.Length != 0) 
        {
            for (int i = 0; i < _stanceOffense.Length; ++i) 
            {
                //Default idle Stance
                if (_stanceOffense[i].GetOffenseType == OffenseType.DEFAULT) 
                {
                    if (pAnimationClip == _stanceOffense[i].GetClip) 
                    {
                        if (_currentOffense == _stanceOffense[i])
                            return true;
                    }
                }
            }
        }

        return false;
    }

    bool IsStandardCanceled() 
    {
        for (int i = 0; i < OffenseCancelConfig.GetInstance.GetOffenseCancel.Length; ++i)
        {
            if (_nextOffense.GetOffenseType == OffenseCancelConfig.GetInstance.GetOffenseCancel[i].type)
            {
                //Default direction
                if (OffenseCancelConfig.GetInstance.GetOffenseCancel[i].direction == OffenseDirection.DEFAULT)
                {
                    for (int j = 0; j < OffenseCancelConfig.GetInstance.GetOffenseCancel[i].standardCancelDirection.Count; ++j)
                    {
                        if (OffenseCancelConfig.GetInstance.GetOffenseCancel[i].standardCancelDirection[j] == OffenseDirection.DEFAULT)
                        {
                            if (_currentOffense.GetOffenseType == OffenseCancelConfig.GetInstance.GetOffenseCancel[i].standardCancelType[j])
                            {
                                return true;
                            }
                        }
                        else if (_currentOffense.GetOffenseDirection == OffenseCancelConfig.GetInstance.GetOffenseCancel[i].standardCancelDirection[j])
                            return true;
                    }
                }
            }
        }

        return false;
    }

    public void SetAnimation(Animator pAnimator, OffenseDirection pOffenseDirection, OffenseType pOffenseType, bool pIsStance)
    {
        OffenseSetup(pAnimator, pOffenseDirection, pOffenseType);

        if (!_isCooldownActivated)
        {
            //Stance
            if (pIsStance)
                StanceRebindSetup(pAnimator, pOffenseType);

            if (_nextOffense != null)
            {
                if (_nextOffense != _currentOffense)
                {
                    if (GetIsCanceled(pAnimator.GetCurrentAnimatorClipInfo(0)[0].clip))
                    {
                        pAnimator.Play(_nextOffense.GetClip.name);

                        if (_nextOffense.GetIsCooldownAvailable)
                        {
                            _isCooldownActivated = _nextOffense.GetIsCooldownAvailable;

                            _currentMaxCooldownTime = _nextOffense.GetMaxCooldownTime;
                        }
                    }
                }
            }
        }
        else if (!GetIsCooldown())
            _isCooldownActivated = false;
    }

    bool GetIsCooldown()
    {
        if (_currentCooldownTime >= _currentMaxCooldownTime)
        {
            _currentCooldownTime = 0f;
            _currentMaxCooldownTime = _currentCooldownTime;

            return false;
        }

        _currentCooldownTime += Time.deltaTime;

        return true;
    }

    Offense GetNextOffense(OffenseDirection pOffenseDirection, OffenseType pOffenseType) 
    {
        if (_nextOffense != null) 
        {
            if (_currentOffense != null) 
            {
                if (_nextOffense == _currentOffense.GetClip)
                    return null;
            }
        }

        //Offense
        if (pOffenseDirection != OffenseDirection.STANCE)
        {
            for (int i = 0; i < _offense.Length; ++i)
            {
                if (_offense[i].GetIsGoodOffense(pOffenseDirection, pOffenseType))
                {
                    if (_nextOffense != _offense[i])
                        return _offense[i];
                }
            }
        }

        //Stance
        else if (_stanceOffense.Length != 0)
        {
            for (int i = 0; i < _stanceOffense.Length; ++i)
            {
                if (_stanceOffense[i].GetIsGoodOffense(pOffenseDirection, pOffenseType)) 
                {
                    if (_currentOffense != null)
                    {
                        if (_stanceOffense[i].GetClip != _currentOffense.GetClip)
                        {
                            if (_nextOffense != _stanceOffense[i])
                                return _stanceOffense[i];
                        }
                        else
                            return null;
                    }
                }
            }
        }

        return null;
    }

    Offense GetCurrentOffense(Offense[] pOffense, AnimationClip pCurrentAnimationClip) 
    {
        if (pOffense.Length != 0)
        {
            for (int i = 0; i < pOffense.Length; ++i) 
            {
                if (pOffense[i].GetClip.name == pCurrentAnimationClip.name)
                    return pOffense[i];
            }
        }

        return null;
    }

    void CurrentOffenseSetup(Animator pAnimator, ref Offense pCurrentOffense) 
    {
        if (pCurrentOffense == null || pCurrentOffense.GetClip != pAnimator.GetCurrentAnimatorClipInfo(0)[0].clip) 
        {
            //Stance
            _currentOffense = GetCurrentOffense(_stanceOffense, pAnimator.GetCurrentAnimatorClipInfo(0)[0].clip);

            //Offense
            if (_currentOffense == null)
                _currentOffense = GetCurrentOffense(_offense, pAnimator.GetCurrentAnimatorClipInfo(0)[0].clip);
        }
    }

    void OffenseSetup(Animator pAnimator, OffenseDirection pOffenseDirection, OffenseType pOffenseType) 
    {
        _nextOffense = GetNextOffense(pOffenseDirection, pOffenseType);

        CurrentOffenseSetup(pAnimator, ref _currentOffense);
    }

    void StanceRebindSetup(Animator pAnimator, OffenseType pOffenseType) 
    {
        if (pOffenseType != OffenseType.DEFAULT)
        {
            if (_currentOffense != null)
            {
                if (_currentOffense.GetOffenseType != OffenseType.DEFAULT)
                {
                    if (_currentOffense.GetOffenseDirection == OffenseDirection.STANCE)
                    {
                        if (pAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.75f)
                            pAnimator.Rebind();
                    }
                }
            }
        }
    }

    void OnDisable()
    {
        _currentOffense = null;
        _nextOffense = null;
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(OffenseManager))]
public class OffenseManagerEditor : Editor 
{
    SerializedProperty _offense, _stanceOffense;

    GUIStyle _guiStyle;

    void OnEnable() 
    {
        _offense = serializedObject.FindProperty("_offense");
        _stanceOffense = serializedObject.FindProperty("_stanceOffense");

        if (_guiStyle == null)
            _guiStyle = new GUIStyle();

        if (_guiStyle.fontStyle != FontStyle.BoldAndItalic)
            _guiStyle.fontStyle = FontStyle.BoldAndItalic;
    }

    public override void OnInspectorGUI() 
    {
        serializedObject.Update();

        EditorGUILayout.BeginVertical(GUI.skin.box);

        #region Information

        GUILayout.Label("Informations");

        EditorGUILayout.Space();

        EditorGUILayout.BeginHorizontal();

        //Add
        if (GUILayout.Button("+")) 
        {
            ++_offense.arraySize;

            _offense.GetArrayElementAtIndex(_offense.arraySize - 1).objectReferenceValue = null;
        }

        //Remove
        if (GUILayout.Button("-"))
        {
            if (_offense.arraySize > 0)
                --_offense.arraySize;
        }

        EditorGUILayout.EndHorizontal();

        #endregion

        EditorGUILayout.Space();

        #region Configuration

        EditorGUILayout.BeginVertical(GUI.skin.box);

        GUILayout.Label("Configuration", _guiStyle);

        EditorGUILayout.Space();

        EditorGUIUtility.labelWidth = 100f;

        ++EditorGUI.indentLevel;

        EditorGUILayout.PropertyField(_offense, new GUIContent("Offense"));

        --EditorGUI.indentLevel;

        EditorGUILayout.EndVertical();

        EditorGUILayout.Space();

        EditorGUILayout.BeginVertical(GUI.skin.box);

        ++EditorGUI.indentLevel;

        EditorGUILayout.PropertyField(_stanceOffense, new GUIContent("Stance"));

        --EditorGUI.indentLevel;

        EditorGUILayout.EndVertical();

        #endregion

        EditorGUILayout.EndVertical();

        serializedObject.ApplyModifiedProperties();
    }
}

#endif