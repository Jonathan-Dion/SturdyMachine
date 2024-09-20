using UnityEngine;

using SturdyMachine.Settings.NADTimeSettings;
using SturdyMachine.Settings;

namespace SturdyMachine.Features.NADTime {

    public static class NADTime {

        #region Attributes

        static NADTimeType _currentNADTimeType;

        static float _currentCooldownTime;

        static bool _isCooldownActivated;

        static float _currentAnimationClipLenght;

        #endregion

        #region Properties

        static float GetCooldownTimer(NADTimeType pCurrentNADTimeType, float pAnimationClipLenght) {

            return pAnimationClipLenght * GameSettings.GetGameSettings().GetGameplaySettings.GetNADTimeSettings.GetCurrentNADTimeMultiplicator(pCurrentNADTimeType);
        }

        public static bool GetIsCooldownActivated(float pCurrentAnimationClipLenght) {

            if (_currentAnimationClipLenght != pCurrentAnimationClipLenght) {
            
                _currentAnimationClipLenght = pCurrentAnimationClipLenght;

                _currentCooldownTime = 0f;
            }

            _currentCooldownTime += Time.deltaTime;

            _isCooldownActivated = _currentCooldownTime < GetCooldownTimer(_currentNADTimeType, pCurrentAnimationClipLenght);

            if (!_isCooldownActivated)
                _currentCooldownTime = 0;

            return _isCooldownActivated;
        }

        static public NADTimeType GetCurrentNADTimeType => _currentNADTimeType;

        #endregion

        #region Methods

        public static void Update(bool pIsPlayerInAttackMode, StateConfirmMode pPlayerStateConfirmMode, StateConfirmMode pEnemyStateConfirmMode, bool pIsGoodOffenseDirection) {

            //AttackMode
            if (pIsPlayerInAttackMode){

                //Blocked
                if (pEnemyStateConfirmMode == StateConfirmMode.Blocking)
                    _currentNADTimeType = NADTimeType.Neutral;

                //Hit success
                else if (pEnemyStateConfirmMode == StateConfirmMode.Hitting)
                    _currentNADTimeType = NADTimeType.Advantage;
            }

            //DefendingMode
            else if (!pIsGoodOffenseDirection) {

                _currentNADTimeType = NADTimeType.Disadvantage;
            }

            if (pPlayerStateConfirmMode == StateConfirmMode.Hitting)
                _currentNADTimeType = NADTimeType.Neutral;

            if (!_isCooldownActivated) {

                if (_currentNADTimeType != NADTimeType.Neutral)
                    _currentNADTimeType = NADTimeType.Neutral;
            }
        }

        #endregion
    }
}