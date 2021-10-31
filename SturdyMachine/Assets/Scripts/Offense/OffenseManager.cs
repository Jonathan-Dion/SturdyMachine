using UnityEngine;
using System.Collections.Generic;

using ICustomEditor.ScriptableObjectEditor;

#if UNITY_EDITOR
using UnityEditor;
#endif

[CreateAssetMenu(fileName = "NewOffenseManager", menuName = "Offence/Manager", order = 52)]
public class OffenseManager : ScriptableObjectICustomEditor
{
    [SerializeField]
    List<Offense> _offense;

    [SerializeField]
    List<Offense> _stanceOffense;

    Offense _currentOffense, _nextOffense;

    bool _isCooldownActivated;

    float _currentCooldownTime, _currentMaxCooldownTime;

    public Offense[] GetOffense => _offense.ToArray();
    public Offense[] GetStanceOffense => _stanceOffense.ToArray();

    public OffenseDirection GetCurrentOffenseDirection => _currentOffense.GetOffenseDirection;
    public bool GetIsCooldownActivated => _isCooldownActivated;
    public float GetCurrentCooldownTime => _currentCooldownTime;
    public float GetMaxCooldownTime => _currentMaxCooldownTime;

    bool GetIsCanceled(Animator pAnimator) 
    {
        //Standard
        if (_nextOffense != null)
        {
            if (IsStandardCanceled())
                return true;
        }

        //Stance
        if (IsStanceCanceled(pAnimator))
            return true;

        return false;
    }

    bool IsStanceCanceled(Animator pAnimator) 
    {
        if (_stanceOffense != null || GetStanceOffense.Length != 0) 
        {
            for (int i = 0; i < GetStanceOffense.Length; ++i) 
            {
                //CurrentOffense == Stance
                if (_currentOffense.GetOffenseDirection == OffenseDirection.STANCE)
                {
                    //CurrentOffense == DefaultStance
                    if (_currentOffense.GetOffenseType == OffenseType.DEFAULT)
                        return true;

                    else if (_nextOffense.GetOffenseType == _stanceOffense[i].GetOffenseType)
                        return true;
                }

                //NextOffense == Stance
                else if (_nextOffense.GetOffenseDirection == OffenseDirection.STANCE)
                {
                    if (pAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f)
                        return true;
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
                    if (GetIsCanceled(pAnimator))
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
            for (int i = 0; i < GetOffense.Length; ++i) 
            {
                if (_offense[i].GetIsGoodOffense(pOffenseDirection, pOffenseType))
                {
                    if (_nextOffense != _offense[i])
                        return _offense[i];
                }
            }
        }

        //Stance
        else if (GetStanceOffense.Length != 0)
        {
            for (int i = 0; i < GetStanceOffense.Length; ++i)
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
                    }
                }
            }
        }

        return _nextOffense;
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
            _currentOffense = GetCurrentOffense(GetStanceOffense, pAnimator.GetCurrentAnimatorClipInfo(0)[0].clip);

            //Offense
            if (_currentOffense == null)
                _currentOffense = GetCurrentOffense(GetOffense, pAnimator.GetCurrentAnimatorClipInfo(0)[0].clip);
        }
    }

    void OffenseSetup(Animator pAnimator, OffenseDirection pOffenseDirection, OffenseType pOffenseType) 
    {
        CurrentOffenseSetup(pAnimator, ref _currentOffense);

        if (GetNextOffense(pOffenseDirection, pOffenseType) != _nextOffense)
            _nextOffense = GetNextOffense(pOffenseDirection, pOffenseType);
        else if (_nextOffense == _currentOffense)
            _nextOffense = null;
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
                            pAnimator.Play(_currentOffense.GetClip.name);
                    }
                }
            }
        }
    }

#if UNITY_EDITOR
    public override void CustomOnInspectorGUI()
    {
        EditorGUILayout.BeginVertical(GUI.skin.box);

        #region Information

        GUILayout.Label("Informations");

        EditorGUILayout.Space();

        EditorGUILayout.BeginHorizontal();

        //Add
        if (GUILayout.Button("+"))
        {
            _offense.Add(null);
        }

        //Remove
        if (GUILayout.Button("-"))
        {
            if (GetOffense.Length > 0)
                _offense.RemoveAt(GetOffense.Length - 1);
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

        for (int i = 0; i < GetOffense.Length; ++i)
            EditorGUILayout.ObjectField(_offense[i], typeof(Offense), true);

        --EditorGUI.indentLevel;

        EditorGUILayout.EndVertical();

        EditorGUILayout.Space();

        EditorGUILayout.BeginVertical(GUI.skin.box);

        ++EditorGUI.indentLevel;

        for (int i = 0; i < GetStanceOffense.Length; ++i)
            EditorGUILayout.ObjectField(_stanceOffense[i], typeof(Offense), true);

        --EditorGUI.indentLevel;

        EditorGUILayout.EndVertical();

        #endregion

        EditorGUILayout.EndVertical();
    }

    public override void CustomOnDisable()
    {
        base.CustomOnDisable();

        _currentOffense = null;
        _nextOffense = null;
    }

#endif
}