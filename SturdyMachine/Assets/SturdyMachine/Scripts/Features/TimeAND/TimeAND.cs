using UnityEngine;

namespace SturdyMachine.Features.TimeAND {

    public enum TimeANDType { Neutral, Advantage, Disadvantage }

    public struct TimeANDData {

        public float neutralMultiplicator;
        public float advantageMultiplicator;
        public float disadvantageMultiplicator;
    }

    public static class TimeAND {

        #region Attributes

        static TimeANDType _currentTimeANDType;

        static TimeANDData _timeANDData;

        static float _currentCooldownTime;

        static bool _isCooldownActivated;

        static float _currentAnimationClipLenght;

        #endregion

        #region Properties

        static float GetCooldownTimer(TimeANDType pTimeANDType, float pAnimationClipLenght) {
        
            //Advantage
            if (pTimeANDType == TimeANDType.Advantage)
                return pAnimationClipLenght * _timeANDData.advantageMultiplicator;

            //Disadvantage
            if (pTimeANDType == TimeANDType.Disadvantage)
                return pAnimationClipLenght * _timeANDData.disadvantageMultiplicator;

            //Neutral
            return pAnimationClipLenght * _timeANDData.neutralMultiplicator;
        }

        public static bool GetIsCooldownActivated(float pCurrentAnimationClipLenght) {

            if (_currentAnimationClipLenght != pCurrentAnimationClipLenght) {
            
                _currentAnimationClipLenght = pCurrentAnimationClipLenght;

                _currentCooldownTime = 0f;
            }

            _currentCooldownTime += Time.deltaTime;

            _isCooldownActivated = _currentCooldownTime < GetCooldownTimer(_currentTimeANDType, pCurrentAnimationClipLenght);

            if (!_isCooldownActivated)
                _currentCooldownTime = 0;

            return _isCooldownActivated;
        }

        public static TimeANDType GetCurrentTimeANDType => _currentTimeANDType;

        #endregion

        #region Methods

        public static void Initialize(float pNeutralMultiplicator, float pAdvantageMultiplicator, float pDisadvantageMultiplicator) {
        
            _timeANDData = new TimeANDData();

            _timeANDData.neutralMultiplicator = pNeutralMultiplicator;
            _timeANDData.advantageMultiplicator = pAdvantageMultiplicator;
            _timeANDData.disadvantageMultiplicator = pDisadvantageMultiplicator;
        }

        public static void Update(bool pIsPlayerInAttackMode, StateConfirmMode pPlayerStateConfirmMode, StateConfirmMode pEnemyStateConfirmMode, bool pIsGoodOffenseDirection) {

            //AttackMode
            if (pIsPlayerInAttackMode){

                //Blocked
                if (pEnemyStateConfirmMode == StateConfirmMode.Blocking)
                    _currentTimeANDType = TimeANDType.Neutral;

                //Hit success
                else if (pEnemyStateConfirmMode == StateConfirmMode.Hitting)
                    _currentTimeANDType = TimeANDType.Advantage;
            }

            //DefendingMode
            else if (!pIsGoodOffenseDirection) {

                _currentTimeANDType = TimeANDType.Disadvantage;
            }

            if (pPlayerStateConfirmMode == StateConfirmMode.Hitting)
                _currentTimeANDType = TimeANDType.Neutral;

            if (!_isCooldownActivated) {

                if (_currentTimeANDType != TimeANDType.Neutral)
                    _currentTimeANDType = TimeANDType.Neutral;
            }
        }

        #endregion
    }
}