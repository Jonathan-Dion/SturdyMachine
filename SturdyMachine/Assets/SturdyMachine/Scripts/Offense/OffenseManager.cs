using UnityEngine;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;
using NWH.NUI;
#endif

namespace SturdyMachine.Offense 
{
    /// <summary>
    /// Configuration file that keeps track of all offenses of a bot
    /// </summary>
    [CreateAssetMenu(fileName = "NewOffenseManager", menuName = "Offence/Manager/Offence", order = 52)]
    public class OffenseManager : ScriptableObject
    {
        #region Attribut

        /// <summary>
        /// The ScriptableObject having the list of offenses that can be blocked
        /// </summary>
        [SerializeField, Tooltip("The ScriptableObject having the list of offenses that can be blocked")]
        OffenseCancelConfig _offenseCancelConfig;

        /// <summary>
        /// The attack offense list for this bot
        /// </summary>
        [SerializeField, Tooltip("The attack offense list for this bot")]
        Offense[] _offense;

        /// <summary>
        /// Stance type offense list for this bot
        /// </summary>
        [SerializeField, Tooltip("Stance type offense list for this bot")]
        Offense[] _stanceOffense;

        /// <summary>
        /// The offense that must be played when the bot receives damage
        /// </summary>
        [SerializeField, Tooltip("The offense that must be played when the bot receives damage")]
        Offense _damageHitOffense;

        /// <summary>
        /// The current Offense is playing
        /// </summary>
        [SerializeField, Tooltip("The current Offense is playing")]
        Offense _currentOffense;

        /// <summary>
        /// The next Offense to play
        /// </summary>
        [SerializeField, Tooltip("The next Offense to play")]
        Offense _nextOffense;

        /// <summary>
        /// Represents the state if there is a cooldown activated
        /// </summary>
        [SerializeField, Tooltip("Represents the state if there is a cooldown activated")]
        bool _isCooldownActivated;

        /// <summary>
        /// The current cooldown time
        /// </summary>
        [SerializeField, Tooltip("Current cooldown time")]
        float _currentCooldownTime;

        /// <summary>
        /// The current max cooldown time
        /// </summary>
        [SerializeField, Tooltip("")]
        float _currentMaxCooldownTime;

        /// <summary>
        /// If the offense currently playing is a Stance type
        /// </summary>
        [SerializeField, Tooltip("If the offense currently playing is a Stance type")]
        bool _isStanceOffense;

        /// <summary>
        /// If the offense currently playing is a Repel type
        /// </summary>
        [SerializeField, Tooltip("If the offense currently playing is a Stance type")]
        bool _isRepelOffense;

        #endregion

        #region Get

        /// <summary>
        /// Returns all Offenses that have been configured
        /// </summary>
        public Offense[] GetOffense => _offense;

        /// <summary>
        /// Returns all Stance Offenses that have been configured
        /// </summary>
        public Offense[] GetStanceOffense => _stanceOffense;

        /// <summary>
        /// Returns the direction of the Offense currently playing
        /// </summary>
        public OffenseDirection GetCurrentOffenseDirection => _currentOffense.GetOffenseDirection;

        /// <summary>
        /// Return if the Cooldown is Activated
        /// </summary>
        public bool GetIsCooldownActivated => _isCooldownActivated;

        /// <summary>
        /// Return the current cooldown time
        /// </summary>
        public float GetCurrentCooldownTime => _currentCooldownTime;

        /// <summary>
        /// Return the current max cooldown time
        /// </summary>
        public float GetMaxCooldownTime => _currentMaxCooldownTime;

        /// <summary>
        /// Return if the current offense type are Stance
        /// </summary>
        public bool GetIsStanceOffense => _isStanceOffense;

        /// <summary>
        /// Return the clip time depending on the offense
        /// </summary>
        /// <param name="pOffenseDirection">Offense direction</param>
        /// <param name="pOffenseType">Ofense type</param>
        /// <returns></returns>
        public float GetOffenseClipTime(OffenseDirection pOffenseDirection, OffenseType pOffenseType)
        {
            //If th current offense direction is the Stance
            if (pOffenseDirection == OffenseDirection.STANCE)
            {
                //Iterates through the stance type offense list until it finds the type matching the direction sent as a
                //parameter
                for (int i = 0; i < _stanceOffense.Length; ++i)
                {
                    if (_stanceOffense[i].GetOffenseType == pOffenseType)
                        return _stanceOffense[i].GetClip.length;
                }
            }

            //If it is anything other the Stance
            else
            {
                //Iterates through the offense type offense list until it finds the type matching the direction sent as a
                //parameter
                for (int i = 0; i < _offense.Length; ++i)
                {
                    if (_offense[i].GetOffenseDirection == pOffenseDirection)
                    {
                        //Returns the clip's time and maximum cooldown of the offense in question
                        if (_offense[i].GetOffenseType == pOffenseType)
                            return _offense[i].GetClip.length + _offense[i].GetMaxCooldownTime;
                    }
                }
            }

            return 0f;
        }

        /// <summary>
        /// Checks if the direction of the present offense is in the Stance category
        /// </summary>
        /// <returns>Return true if the currentOffense direction are Stance</returns>
        public bool GetIsStance()
        {
            if (_currentOffense)
            {
                if (_currentOffense.GetOffenseDirection != OffenseDirection.STANCE)
                    return false;
            }

            return true;
        }

        /// <summary>
        /// Return the current offense who is playing
        /// </summary>
        /// <returns></returns>
        public Offense GetCurrentOffense() { return _currentOffense; }

        /// <summary>
        /// Return the next offense that you want to play
        /// </summary>
        /// <returns></returns>
        public Offense GetNextOffense() { return _nextOffense; }

        /// <summary>
        /// Checks if the opponent's offense can be canceled
        /// </summary>
        /// <param name="pAnimator">The animator of the bot being attacked</param>
        /// <param name="pIsMonsterBot">If the bot in question is an AI</param>
        /// <returns>Return true if the current offense can be canceled</returns>
        bool GetIsCanceled(Animator pAnimator, bool pIsMonsterBot)
        {
            //The offense can be blocked if the attacked is an ai bot, the offense is Deflection type or it is in
            //repel mode
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

            //Check if it's a standard offense
            if (_nextOffense != null)
            {
                if (IsStandardCanceled())
                    return true;
            }

            //Check if it's a Stance offense
            if (IsStanceCanceled(pAnimator))
                return true;

            return false;
        }

        /// <summary>
        /// Checking if the Stance type offense can be canceled
        /// </summary>
        /// <param name="pAnimator">The animator of the attacked</param>
        /// <returns>Return true if the current if the current offence are Stance offense type and if the current clip time 
        /// are finished</returns>
        bool IsStanceCanceled(Animator pAnimator)
        {
            //Checking if offense stance is in use or if there are offense stances configured for the bot
            if (_stanceOffense != null || GetStanceOffense.Length != 0)
            {
                //Iterates through the list of stance type offenses
                for (int i = 0; i < GetStanceOffense.Length; ++i)
                {
                    //Checking if the current Offense and Stance Type
                    if (_currentOffense.GetOffenseDirection == OffenseDirection.STANCE)
                    {
                        if (_currentOffense.GetOffenseType == OffenseType.DEFAULT)
                            return true;

                        else if (_nextOffense.GetOffenseType == _stanceOffense[i].GetOffenseType)
                            return true;
                    }

                    //Checking if the next Offense and Stance Type
                    else if (_nextOffense.GetOffenseDirection == OffenseDirection.STANCE)
                    {
                        if (pAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f)
                            return true;
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// Checking if the standard type offense can be canceled
        /// </summary>
        /// <returns>Returns true if all conditions have been met</returns>
        bool IsStandardCanceled()
        {
            for (int i = 0; i < _offenseCancelConfig.GetOffenseCancelDataGroup.Length; ++i)
            {
                if (_nextOffense.GetOffenseType != _offenseCancelConfig.GetOffenseCancelDataGroup[i].offenseType)
                    continue;

                if (_offenseCancelConfig.GetOffenseCancelDataGroup[i].offenseDirection != OffenseDirection.DEFAULT)
                    continue;

                for (int j = 0; j < _offenseCancelConfig.GetOffenseCancelDataGroup[i].standardOffenseCancel.Length; ++j)
                {
                    if (_offenseCancelConfig.GetOffenseCancelDataGroup[i].standardOffenseCancel[j].offenseDirection != OffenseDirection.DEFAULT)
                        continue;

                    if (_currentOffense.GetOffenseType != _offenseCancelConfig.GetOffenseCancelDataGroup[i].standardOffenseCancel[j].offenseType)
                        continue;

                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Checks if the current offense is in its cooldown phase.
        /// </summary>
        /// <returns>Return true if the cooldown timer has exceeded its maximum time.</returns>
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

        /// <summary>
        /// Allows the assignment of the next offense
        /// </summary>
        /// <param name="pOffenseDirection">The direction of the next desired offense</param>
        /// <param name="pOffenseType">The type of the next desired offense</param>
        /// <param name="pIsMonsterBot">If the bot is an AI</param>
        /// <returns>Returns the next offense </returns>
        Offense GetNextOffense(OffenseDirection pOffenseDirection, OffenseType pOffenseType, bool pIsMonsterBot)
        {
            //Returns null if the next offense is already assigned
            if (_nextOffense != null)
            {
                if (_currentOffense != null)
                {
                    if (_nextOffense == _currentOffense.GetClip)
                        return null;
                }
            }

            //Returns an offense of type offense
            if (pOffenseDirection != OffenseDirection.STANCE)
            {
                //Iterates through the list of offenses that are configured for this bot
                for (int i = 0; i < GetOffense.Length; ++i)
                {
                    if (_currentOffense)
                    {
                        //Checks if the offense is not a Repel type
                        if (pOffenseType != OffenseType.REPEL)
                        {
                            if (_offense[i].GetIsGoodOffense(pOffenseDirection, pOffenseType))
                            {
                                if (_nextOffense != _offense[i])
                                    return _offense[i];
                            }
                        }

                        //Checks if the desired offense is present in the configured list for the bot
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

            //Iterates through the Stance type offense list if it matches the desired one
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

            //Assigns damage offense if the bot should receive damage
            if (pOffenseType == OffenseType.DAMAGEHIT)
            {
                if (_nextOffense != _damageHitOffense)
                    _nextOffense = _damageHitOffense;
            }

            return _nextOffense;
        }

        /// <summary>
        /// Allows the assignment of the current offense
        /// </summary>
        /// <param name="pOffense">The list of offenses configured for this bot</param>
        /// <param name="pCurrentAnimationClip">The animationClip that the bot is currently playing.</param>
        /// <returns>Returns the current offense</returns>
        Offense GetCurrentOffense(Offense[] pOffense, AnimationClip pCurrentAnimationClip)
        {
            if (pOffense.Length != 0)
            {
                //Iterates through the list of offenses that have been configured for this bot
                for (int i = 0; i < pOffense.Length; ++i)
                {
                    //Returns the offense of type repel if the name of the animationClip is the same.
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

            //Returns the damage received offense if the name of the clip matches the name of the damage.
            if (pCurrentAnimationClip.name == _damageHitOffense.GetClip.name)
                return _damageHitOffense;

            return null;
        }

        /// <summary>
        /// Allows the attribution of the offense in court when you want a specific type of offense
        /// </summary>
        /// <param name="pOffense">The list of offenses configured for this bot</param>
        /// <param name="pOffenseDirection">The direction of the desired offense</param>
        /// <param name="pOffenseType">The type of offense desired</param>
        /// <returns>Return a specific type of offense</returns>
        Offense GetCurrentOffense(Offense[] pOffense, OffenseDirection pOffenseDirection, OffenseType pOffenseType)
        {
            if (pOffense.Length != 0)
            {
                //Iterates in the offense list that has been configured for this bot according to the values chosen as a parameter
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

        #endregion

        /// <summary>
        /// Assigns the right animation depending on the actions made and the combat situation
        /// </summary>
        /// <param name="pAnimator">The bot animator</param>
        /// <param name="pOffenseDirection">The direction of offense the bot wants to use</param>
        /// <param name="pOffenseType">The type of offense the bot wants to use</param>
        /// <param name="pIsStance">If the offense the bot wants to use is a Stance type</param>
        /// <param name="pIsMonsterBot">If the present bot is an AI bot</param>
        public void SetAnimation(Animator pAnimator, OffenseDirection pOffenseDirection, OffenseType pOffenseType, bool pIsStance, bool pIsMonsterBot = false)
        {
            //Assigns current offense based on battle situation
            OffenseSetup(pAnimator, pOffenseDirection, pOffenseType, pIsMonsterBot);

            if (_isCooldownActivated) {

                if (!GetIsCooldown())
                    _isCooldownActivated = false;

                return;
            }

            //Restarts the offense if it is a stance type and the animation is almost over
            if (pIsStance)
                StanceRebindSetup(pAnimator, pOffenseType);

            //Play next offense if conditions permit
            if (!_nextOffense)
                return;

            //Assign the current offense if it is not the same as the previous one.
            if (_nextOffense != _currentOffense) {

                if (!_currentOffense)
                    return;

                //Checks if the bot's current offense can be canceled
                if (!GetIsCanceled(pAnimator, pIsMonsterBot))
                    return;

                pAnimator.Play(_nextOffense.GetClip.name);

                //Assign cooldown values
                if (_nextOffense.GetIsCooldownAvailable)
                {
                    _isCooldownActivated = _nextOffense.GetIsCooldownAvailable;

                    _currentMaxCooldownTime = _nextOffense.GetMaxCooldownTime;
                }

                //Assigns present offense if present bot is an AI
                if (pIsMonsterBot)
                    _currentOffense = GetCurrentOffense(pOffenseDirection == OffenseDirection.STANCE ? GetStanceOffense : GetOffense, pOffenseDirection, pOffenseType);

                if (_isRepelOffense)
                    _isRepelOffense = false;

                return;
            }

            //Play DamageHit Offense
            if (_nextOffense.GetOffenseType == OffenseType.DAMAGEHIT) {

                pAnimator.Play(_nextOffense.GetClip.name);

                return;
            }

            /*//Play the repel offense
            else if (pOffenseType == OffenseType.REPEL)
            {
                if (_nextOffense.GetRepelClip)
                    pAnimator.Play(_nextOffense.GetRepelClip.name);

                if (!_isRepelOffense)
                    _isRepelOffense = true;
            }*/
        }

        /// <summary>
        /// Assign the offense to the Offense variable as a parameter
        /// </summary>
        /// <param name="pAnimator">The bot animator</param>
        /// <param name="pCurrentOffense">The variable indicating the offense that is currently used</param>
        void CurrentOffenseSetup(Animator pAnimator, ref Offense pCurrentOffense)
        {
            //Checks if the current offense has not yet been assigned or if it is already playing
            if (pCurrentOffense == null || pCurrentOffense.GetClip != pAnimator.GetCurrentAnimatorClipInfo(0)[0].clip)
            {
                //Assigns a new Offense if it is an Offense type
                if (pCurrentOffense != null)
                {
                    if (pAnimator.GetCurrentAnimatorClipInfo(0)[0].clip == pCurrentOffense.GetClip)
                    {
                        //Checks if the offense has a repel clip
                        if (pCurrentOffense.GetRepelClip)
                        {
                            _currentOffense = GetCurrentOffense(GetOffense, pAnimator.GetCurrentAnimatorClipInfo(0)[0].clip);

                            return;
                        }
                    }
                }

                //Assigns a new offense if it is a Stance type
                _currentOffense = GetCurrentOffense(GetStanceOffense, pAnimator.GetCurrentAnimatorClipInfo(0)[0].clip);

                //Assign an offense
                if (_currentOffense == null)
                    _currentOffense = GetCurrentOffense(GetOffense, pAnimator.GetCurrentAnimatorClipInfo(0)[0].clip);

            }
        }

        /// <summary>
        /// Assigns current offense based on battle situation
        /// </summary>
        /// <param name="pAnimator">The bot animator</param>
        /// <param name="pOffenseDirection">The direction of offense the bot wants to use</param>
        /// <param name="pOffenseType">The type of offense the bot wants to use</param>
        /// <param name="pIsMonsterBot">If the bot is an AI bot</param>
        void OffenseSetup(Animator pAnimator, OffenseDirection pOffenseDirection, OffenseType pOffenseType, bool pIsMonsterBot)
        {
            //Assign current offense
            CurrentOffenseSetup(pAnimator, ref _currentOffense);

            //If the conditions allow it, the next offense is assigned
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

        /// <summary>
        /// Allows the Stance type offense to be restarted when it is nearly complete
        /// </summary>
        /// <param name="pAnimator">The bot animator</param>
        /// <param name="pOffenseType">The type of current offense</param>
        void StanceRebindSetup(Animator pAnimator, OffenseType pOffenseType)
        {
            //Checking if the offense type is Stance type
            if (pOffenseType == OffenseType.DEFAULT)
                return;

            //Checking if the current offense is assigned
            if (!_currentOffense)
                return;

            //Checking if the current offense type is Stance type
            if (_currentOffense.GetOffenseType == OffenseType.DEFAULT)
                return;

            //Checking if current offense direction is Stance type
            if (_currentOffense.GetOffenseDirection != OffenseDirection.STANCE)
                return;

            //Checking if the time of the offense type offense animation is rendered at 3/4 in its time
            if (pAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime < 0.75f)
                return;

            pAnimator.Play(_currentOffense.GetClip.name);
        }

#if UNITY_EDITOR

        [CustomEditor(typeof(OffenseManager))]
        public class OffenseManagerEditor : NUIEditor
        {
            public override bool OnInspectorNUI()
            {
                if (!base.OnInspectorNUI())
                    return false;

                EditorGUI.BeginChangeCheck();

                #region Debug Value

                drawer.BeginSubsection("Debug values");

                #region Offense

                drawer.BeginSubsection("Offense");

                drawer.Field("_currentOffense", false, null, "Current: ");
                drawer.Field("_nextOffense", false, null, "Next: ");

                drawer.Space();

                drawer.Field("_isStanceOffense", false, null, "Stance: ");
                drawer.Field("_isRepelOffense", false, null, "Repel: ");

                drawer.EndSubsection();

                #endregion

                #region Cooldown

                drawer.BeginSubsection("Cooldown");

                if (drawer.Field("_isCooldownActivated").boolValue) 
                {
                    drawer.Field("_currentCooldownTime", false, null, "Current: ");
                    drawer.Field("_currentMaxCooldownTime", false, null, "MaxTime: ");
                }

                drawer.EndSubsection();

                #endregion

                drawer.EndSubsection();

                #endregion

                #region Offense

                //CanceConfig
                drawer.Field("_offenseCancelConfig", true, null, "Cancel: ");

                //Attack Offense list
                drawer.ReorderableList("_offense");

                //Stance Offense list
                drawer.ReorderableList("_stanceOffense");

                //Damage Offense
                drawer.Field("_damageHitOffense", true, null, "Damage: ");

                #endregion

                drawer.EndEditor(this);
                return true;
            }
        }

#endif
    }
}