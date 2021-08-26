using UnityEngine;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;
#endif

[CreateAssetMenu(fileName = "NewOffenseManager", menuName = "Offence/Manager", order = 52)]
public class OffenseManager : ScriptableObject
{
    [SerializeField]
    Offense[] _offense;

    Offense _currentOffense, _nextOffense;

    bool _isCooldownActivated;

    float _currentCooldownTime, _currentMaxCooldownTime;

    public bool GetIsCooldownActivated => _isCooldownActivated;
    public float GetCurrentCooldownTime => _currentCooldownTime;
    public float GetMaxCooldownTime => _currentMaxCooldownTime;

    public AnimationClip GetIdleFightingStance() 
    {
        for (int i = 0; i < _offense.Length; ++i) 
        {
            if (_offense[i].GetOffenseDirection == OffenseDirection.STANCE)
                if (_offense[i].GetOffenseType == OffenseType.DEFAULT)
                    return _offense[i].GetClip;
        }

        return null;
    }

    public void StartRemote() 
    {
        for (int i = 0; i < _offense.Length; ++i) 
        {
            if (_offense[i].GetOffenseDirection == OffenseDirection.STANCE)
                _offense[i].GetClip.wrapMode = WrapMode.Loop;
        }

    }

    bool IsSpecialCanceled(AnimationClip pAnimationClip, int pIndex)
    {
        return false;
    }

    bool GetIsCanceled(AnimationClip pAnimationClip) 
    {
        //Standard
        if (_nextOffense != null)
        {
            if (IsStandardCanceled())
                return true;
        }

        for (int i = 0; i < _offense.Length; ++i) 
        {
            //Stance
            if (IsStanceCanceled(pAnimationClip, i))
                return true;
            //Special
            else if (IsSpecialCanceled(pAnimationClip, i))
                return true;
        }

        return false;
    }

    bool IsStanceCanceled(AnimationClip pAnimationClip, int pIndex) 
    {
        if (_offense[pIndex].GetOffenseDirection == OffenseDirection.STANCE) 
        {
            //Fighting
            if (_offense[pIndex].GetOffenseType == OffenseType.DEFAULT)
            {
                if (pAnimationClip == _offense[pIndex].GetClip)
                    if (_currentOffense == _offense[pIndex])
                        return true;
            }
            else if (pAnimationClip == _offense[pIndex].GetClip)
                return true;
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
        OffenseSetup(pAnimator, pOffenseDirection, pOffenseType, pIsStance);

        if (!_isCooldownActivated)
        {
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

    void OffenseSetup(Animator pAnimator, OffenseDirection pOffenseDirection, OffenseType pOffenseType, bool pIsStance) 
    {
        for (int i = 0; i < _offense.Length; ++i)
        {
            if (pOffenseDirection != OffenseDirection.DEFAULT)
            {
                if (_offense[i].GetIsGoodOffense(pOffenseDirection, pOffenseType))
                {
                    if (_nextOffense != _offense[i])
                        _nextOffense = _offense[i];

                    //Stance
                    if (pIsStance)
                    {
                        if (pAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.95f)
                            pAnimator.Rebind();
                    }

                    break;
                }
            }

            if (_currentOffense == null)
            {
                if (_offense[i].GetClip == pAnimator.GetCurrentAnimatorClipInfo(0)[0].clip)
                    _currentOffense = _offense[i];
            }
            else if (_currentOffense.GetClip != pAnimator.GetCurrentAnimatorClipInfo(0)[0].clip)
            {
                if (pAnimator.GetCurrentAnimatorClipInfo(0)[0].clip == _offense[i].GetClip)
                {
                    if (_currentOffense != _offense[i])
                        _currentOffense = _offense[i];

                    break;
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
    SerializedProperty _offense;

    GUIStyle _guiStyle;

    void OnEnable() 
    {
        _offense = serializedObject.FindProperty("_offense");

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

        EditorGUILayout.Space();

        EditorGUILayout.EndVertical();

        #endregion

        EditorGUILayout.EndVertical();

        serializedObject.ApplyModifiedProperties();
    }
}


#endif