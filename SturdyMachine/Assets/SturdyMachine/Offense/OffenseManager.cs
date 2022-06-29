using UnityEngine;
using System.Collections.Generic;

using ICustomEditor.ScriptableObjectEditor;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace SturdyMachine.Offense.Manager 
{
    [CreateAssetMenu(fileName = "NewOffenseManager", menuName = "Offence/Manager/Offence", order = 52)]
    public class OffenseManager : ScriptableObjectICustomEditor
    {
        [SerializeField]
        List<Offense> _offense;

        [SerializeField]
        List<Offense> _stanceOffense;

        [SerializeField]
        Offense _damageHitOffense;

        Offense _currentOffense, _nextOffense;

        bool _isCooldownActivated;

        float _currentCooldownTime, _currentMaxCooldownTime;

        bool _isOffense, _isStanceOffense, _isRepelOffense;

        public Offense[] GetOffense => _offense.ToArray();
        public Offense[] GetStanceOffense => _stanceOffense.ToArray();

        public OffenseDirection GetCurrentOffenseDirection => _currentOffense.GetOffenseDirection;
        public bool GetIsCooldownActivated => _isCooldownActivated;
        public float GetCurrentCooldownTime => _currentCooldownTime;
        public float GetMaxCooldownTime => _currentMaxCooldownTime;

        public bool GetIsStanceOffense => _isStanceOffense;

        public float GetOffenseClipTime(OffenseDirection pOffenseDirection, OffenseType pOffenseType) 
        {
            //Stance
            if (pOffenseDirection == OffenseDirection.STANCE)
            {
                for (int i = 0; i < _stanceOffense.Count; ++i) 
                {
                    if (_stanceOffense[i].GetOffenseType == pOffenseType)
                        return _stanceOffense[i].GetClip.length;
                }
            }

            //Offense
            else 
            {
                for (int i = 0; i < _offense.Count; ++i) 
                {
                    if (_offense[i].GetOffenseDirection == pOffenseDirection) 
                    {
                        if (_offense[i].GetOffenseType == pOffenseType)
                            return _offense[i].GetClip.length + _offense[i].GetMaxCooldownTime;
                    }
                }
            }

            return 0f;
        }

        public bool GetIsStance() 
        {
            if (_currentOffense)
            {
                if (_currentOffense.GetOffenseDirection != OffenseDirection.STANCE)
                    return false;
            }

            return true;
        }

        bool GetIsCanceled(Animator pAnimator, bool pIsMonsterBot)
        {
            if (pIsMonsterBot) 
            {
                if (_nextOffense.GetOffenseType == OffenseType.DEFLECTION)
                    return true;
                else if (_isRepelOffense)
                    return true;
            }

            if (_nextOffense.GetOffenseType == OffenseType.SWEEP)
                return true;
            else if (_nextOffense.GetOffenseType == OffenseType.DAMAGEHIT)
                return true;

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

        public void SetAnimation(Animator pAnimator, OffenseDirection pOffenseDirection, OffenseType pOffenseType, bool pIsStance, bool pIsMonsterBot = false)
        {
            OffenseSetup(pAnimator, pOffenseDirection, pOffenseType, pIsMonsterBot);

            if (!_isCooldownActivated)
            {
                //Stance
                if (pIsStance)
                    StanceRebindSetup(pAnimator, pOffenseType);

                if (_nextOffense != null)
                {
                    if (_nextOffense != _currentOffense)
                    {
                        if (_currentOffense)
                        {
                            if (GetIsCanceled(pAnimator, pIsMonsterBot))
                            {
                                pAnimator.Play(_nextOffense.GetClip.name);

                                if (_nextOffense.GetIsCooldownAvailable)
                                {
                                    _isCooldownActivated = _nextOffense.GetIsCooldownAvailable;

                                    _currentMaxCooldownTime = _nextOffense.GetMaxCooldownTime;
                                }

                                if (pIsMonsterBot)
                                    _currentOffense = GetCurrentOffense(pOffenseDirection == OffenseDirection.STANCE ? GetStanceOffense : GetOffense, pOffenseDirection, pOffenseType);
                            
                                if (_isRepelOffense)
                                    _isRepelOffense = false;
                            }
                        }
                    }
                    //Repel
                    else if (pOffenseType == OffenseType.REPEL)
                    {
                        if (_nextOffense.GetRepelClip)
                            pAnimator.Play(_nextOffense.GetRepelClip.name);

                        if (!_isRepelOffense)
                            _isRepelOffense = true;
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

        public Offense GetNextOffense() { return _nextOffense; }

        Offense GetNextOffense(OffenseDirection pOffenseDirection, OffenseType pOffenseType, bool pIsMonsterBot)
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
                    if (_currentOffense)
                    {
                        if (pOffenseType != OffenseType.REPEL)
                        {
                            if (_offense[i].GetIsGoodOffense(pOffenseDirection, pOffenseType))
                            {
                                if (_nextOffense != _offense[i])
                                    return _offense[i];
                            }
                        }
                        else if (_offense[i].GetIsGoodOffense(_currentOffense.GetOffenseDirection, _currentOffense.GetOffenseType))
                        {
                            if (!pIsMonsterBot)
                            {
                                if (_nextOffense != _offense[i])
                                    return _offense[i];
                            }
                            else
                                return _offense[i];
                            
                        }
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

            //DamageHit
            if (pOffenseType == OffenseType.DAMAGEHIT)
            {
                if (_nextOffense != _damageHitOffense)
                    _nextOffense = _damageHitOffense;
            }

            return _nextOffense;
        }

        public Offense GetCurrentOffense() { return _currentOffense; }

        Offense GetCurrentOffense(Offense[] pOffense, AnimationClip pCurrentAnimationClip)
        {
            if (pOffense.Length != 0)
            {
                for (int i = 0; i < pOffense.Length; ++i)
                {
                    if (_isRepelOffense) 
                    {
                        if (pOffense[i].GetRepelClip) 
                        {
                            if (pOffense[i].GetRepelClip.name == pCurrentAnimationClip.name)
                                return pOffense[i];
                        }
                    }
                    else if (pOffense[i].GetClip.name == pCurrentAnimationClip.name)
                        return pOffense[i];
                }
            }

            //DamageHit
            if (pCurrentAnimationClip.name == _damageHitOffense.GetClip.name)
                return _damageHitOffense;

            return null;
        }

        Offense GetCurrentOffense(Offense[] pOffense, OffenseDirection pOffenseDirection, OffenseType pOffenseType)
        {
            if (pOffense.Length != 0)
            {
                for (int i = 0; i < pOffense.Length; ++i)
                {
                    if (pOffense[i].GetOffenseDirection == pOffenseDirection)
                    {
                        if (pOffense[i].GetOffenseType == pOffenseType)
                            return pOffense[i];
                    }
                }
            }

            return null;
        }

        void CurrentOffenseSetup(Animator pAnimator, ref Offense pCurrentOffense)
        {
            if (pCurrentOffense == null || pCurrentOffense.GetClip != pAnimator.GetCurrentAnimatorClipInfo(0)[0].clip)
            {
                if (pCurrentOffense != null)
                {
                    if (pAnimator.GetCurrentAnimatorClipInfo(0)[0].clip == pCurrentOffense.GetClip)
                    {
                        if (pCurrentOffense.GetRepelClip)
                        {
                            _currentOffense = GetCurrentOffense(GetOffense, pAnimator.GetCurrentAnimatorClipInfo(0)[0].clip);

                            return;
                        }
                    }
                }

                //Stance
                _currentOffense = GetCurrentOffense(GetStanceOffense, pAnimator.GetCurrentAnimatorClipInfo(0)[0].clip);

                //Offense
                if (_currentOffense == null)
                    _currentOffense = GetCurrentOffense(GetOffense, pAnimator.GetCurrentAnimatorClipInfo(0)[0].clip);

            }
        }

        void OffenseSetup(Animator pAnimator, OffenseDirection pOffenseDirection, OffenseType pOffenseType, bool pIsMonsterBot)
        {
            CurrentOffenseSetup(pAnimator, ref _currentOffense);

            if (GetNextOffense(pOffenseDirection, pOffenseType, pIsMonsterBot) != _nextOffense)
                _nextOffense = GetNextOffense(pOffenseDirection, pOffenseType, pIsMonsterBot);
            else if (_nextOffense == _currentOffense) 
            {
                if (!pIsMonsterBot) 
                {
                    if (pOffenseType != OffenseType.DAMAGEHIT)
                        _nextOffense = null;
                }
                    
            }
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

            EditorGUI.BeginChangeCheck();

            #region Information

            GUILayout.Label("Informations");

            EditorGUILayout.Space();

            EditorGUILayout.BeginHorizontal();

            //Add
            if (GUILayout.Button("+"))
            {
                if (_offense == null)
                    _offense = new List<Offense>();

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

            _damageHitOffense = EditorGUILayout.ObjectField(_damageHitOffense, typeof(Offense), true) as Offense;

            EditorGUILayout.Space();

            EditorGUIUtility.labelWidth = 100f;

            ++EditorGUI.indentLevel;

            EditorGUILayout.BeginVertical(GUI.skin.box);

            EditorGUILayout.BeginHorizontal(GUI.skin.box);

            //Offense
            EditorGUILayout.LabelField("Offense: ", _guiStyle);

            _isOffense = EditorGUILayout.Toggle(_isOffense);

            //StanceOffense
            EditorGUILayout.LabelField("Stance Offense: ", _guiStyle);

            _isStanceOffense = EditorGUILayout.Toggle(_isStanceOffense);

            EditorGUILayout.EndHorizontal();

            EditorGUILayout.EndVertical();

            //Offense
            if (_isOffense)
            {
                if (_offense != null)
                {
                    for (int i = 0; i < _offense.Count; ++i)
                    {
                        _offense[i] = EditorGUILayout.ObjectField(_offense[i], typeof(Offense), true) as Offense;

                        if (_offense[i])
                        {
                            _offense[i].CustomOnInspectorGUI();

                            EditorGUILayout.Space();

                            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
                        }
                    }
                }
            }

            //StanceOffense
            if (_isStanceOffense)
            {
                if (_stanceOffense != null)
                {
                    for (int i = 0; i < _stanceOffense.Count; ++i)
                    {
                        _stanceOffense[i] = EditorGUILayout.ObjectField(_stanceOffense[i], typeof(Offense), true) as Offense;

                        if (_stanceOffense[i])
                        {
                            _stanceOffense[i].CustomOnInspectorGUI();

                            EditorGUILayout.Space();

                            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
                        }
                    }
                }
            }

            EditorGUILayout.EndVertical();

            #endregion

            EditorGUILayout.EndVertical();

            if (EditorGUI.EndChangeCheck())
                AssetDatabase.SaveAssets();
        }

        public override void CustomOnEnable()
        {
            base.CustomOnEnable();

            
        }

        public override void CustomOnDisable()
        {
            base.CustomOnDisable();

            _currentOffense = null;
            _nextOffense = null;
        }

#endif
    }
}