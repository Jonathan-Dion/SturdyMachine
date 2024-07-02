using System;

using UnityEngine;
using TMPro;
using UnityEngine.UI;

#if UNITY_EDITOR
using UnityEditor;
using NWH.NUI;
using NWH.VehiclePhysics2;
#endif

namespace SturdyMachine.UI {

    /// <summary>
    /// Represents all the sub-elements that make up the Sturdy EnergyPoints section
    /// </summary>
    [Serializable, Tooltip("Represents all the sub-elements that make up the Sturdy EnergyPoints section")]
    public struct EnergyPointData {

        /// <summary>
        /// Represents the element of EnergyPointData
        /// </summary>
        [Tooltip("Represents the element of EnergyPointData")]
        public Transform uiElement;

        public float currentValue;

        /// <summary>
        /// 
        /// </summary>
        [Tooltip("")]
        public UISliderData uiSliderGreen;

        /// <summary>
        /// 
        /// </summary>
        [Tooltip("")]
        public UISliderData uiSliderLightGreen;

        /// <summary>
        /// 
        /// </summary>
        [Tooltip("")]
        public UISliderData uiSliderPurple;
    }

    [Serializable, Tooltip("")]
    public struct DeathblowPointData 
    {
        public UISliderData deathblowUISliderPoint;

        public float currentValue;
    }

    [Serializable]
    public struct GameResultData {

        public TextMeshProUGUI tmpTxtGameResult;

        public Button btnReset;

        public Button btnQuit;

        public Animator gameResultAnimator;
        public AnimationClip gameResultClip;

        public string winMessage;
        public string loseMessage;
    }

    public class BattleUI : BaseUI 
    {
        #region Attribut

        [SerializeField]
        GameResultData _gameResultData;

        [SerializeField, Tooltip("")]
        EnergyPointData _energyPointData;

        [SerializeField, Tooltip("")]
        DeathblowPointData _deathblowPointData;

        [SerializeField]
        bool _isDamageApplied;

        #endregion

        #region Properties

        bool GetIsWinGame => _deathblowPointData.currentValue == _deathblowPointData.deathblowUISliderPoint.maxValue;

        
        bool GetIsLoseGame => _energyPointData.currentValue == 0;

        public bool GetIsEndGame() {
        
            //Win
            if (GetIsWinGame)
                return true;

            //Lose
            return GetIsLoseGame;
        }

        public Button GetResetButton => _gameResultData.btnReset;

        #endregion

        #region Methods

        public override void Initialize(int pSturdyEnergyPointValue, int pMaxDeathblowValue)
        {
            EnergyPointInit(pSturdyEnergyPointValue);

            DeathblowPointInit(pMaxDeathblowValue);

            GameResultInit();

            base.Initialize();
        }

        public override bool OnUpdate(bool pIsHitConfirmActivated, float pNextSturdyEnergyPointValue, float pNextDeathblowValue) 
        {
            if (!base.OnUpdate())
                return false;

            if (!pIsHitConfirmActivated) 
            {
                if (_isDamageApplied)
                    _isDamageApplied = false;

                return true;
            }
                

            EnergyPointUpdate(pNextSturdyEnergyPointValue);

            DeathblowPointUpdate(pNextDeathblowValue);

            if (GetIsEndGame())
                EndGameInit();

            return true;
        }

        void EnergyPointInit(int pSturdyEnergyPointValue) 
        {
            //Green
            _energyPointData.uiSliderGreen.maxValue = pSturdyEnergyPointValue;
            _energyPointData.currentValue = _energyPointData.uiSliderGreen.maxValue;
        }

        void DeathblowPointInit(int pMaxDeathblowValue) {

            _deathblowPointData.deathblowUISliderPoint.maxValue = pMaxDeathblowValue;
            _deathblowPointData.currentValue = 0;

            Vector3 currentUISliderPosition = _deathblowPointData.deathblowUISliderPoint.uiElement.localPosition;
            currentUISliderPosition.x = _deathblowPointData.deathblowUISliderPoint.positionValues.x;

            _deathblowPointData.deathblowUISliderPoint.uiElement.localPosition = currentUISliderPosition;
        }

        void EnergyPointUpdate(float pNextSturdyEnergyPointValue) 
        {
            if (pNextSturdyEnergyPointValue == 0)
                return;

            if (_isDamageApplied)
                return;

            Vector3 currentUISliderPosition = _energyPointData.uiSliderGreen.uiElement.localPosition;

            _energyPointData.currentValue -= pNextSturdyEnergyPointValue;

            currentUISliderPosition.x = -(_energyPointData.uiSliderGreen.maxValue - _energyPointData.currentValue) / _energyPointData.uiSliderGreen.maxValue * GetDistanceBetweenPositionValue(_energyPointData.uiSliderGreen);
            _energyPointData.uiSliderGreen.uiElement.localPosition = currentUISliderPosition;
        
            _isDamageApplied = true;
        }

        void DeathblowPointUpdate(float pNextSturdyEnergyPointValue)
        {
            if (pNextSturdyEnergyPointValue == 0)
                return;

            if (_isDamageApplied)
                return;

            Vector3 currentUISliderPosition = _deathblowPointData.deathblowUISliderPoint.uiElement.localPosition;

            _deathblowPointData.currentValue += pNextSturdyEnergyPointValue;

            if (_deathblowPointData.currentValue > _deathblowPointData.deathblowUISliderPoint.maxValue)
                _deathblowPointData.currentValue = _deathblowPointData.deathblowUISliderPoint.maxValue;

            currentUISliderPosition.x = (_deathblowPointData.deathblowUISliderPoint.maxValue - _deathblowPointData.currentValue) / _deathblowPointData.deathblowUISliderPoint.maxValue * GetDistanceBetweenPositionValue(_deathblowPointData.deathblowUISliderPoint);

            _deathblowPointData.deathblowUISliderPoint.uiElement.localPosition = currentUISliderPosition;

            _isDamageApplied = true;
        }

        void GameResultInit() {

            _gameResultData.gameResultAnimator.gameObject.SetActive(false);

            _gameResultData.tmpTxtGameResult.text = "";
        }

        void EndGameInit() {

            _gameResultData.tmpTxtGameResult.text = GetIsWinGame ? _gameResultData.winMessage : _gameResultData.loseMessage;

            _gameResultData.gameResultAnimator.gameObject.SetActive(true);
        }

        #endregion
    }

#if UNITY_EDITOR

    [CustomEditor(typeof(BattleUI))]
    public class BattleUIEditor : BaseUIEditor
    {
        public override bool OnInspectorNUI()
        {
            if (!base.OnInspectorNUI())
                return false;

            drawer.Field("_isDamageApplied", false, "Units", "Damage Applied: ");

            drawer.BeginSubsection("Configuration");

            drawer.Property("_gameResultData");

            drawer.EndSubsection();


            drawer.Property("_energyPointData");
            drawer.Property("_deathblowPointData");
            
            drawer.EndEditor(this);

            return true;
        }
    }

    [CustomPropertyDrawer(typeof(EnergyPointData))]
    public partial class EnergyPointDataDrawer : ComponentNUIPropertyDrawer
    {
        public override bool OnNUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (!base.OnNUI(position, property, label))
                return false;

            drawer.Field("currentValue", false, "Units", "Current value: ");

            if (drawer.Field("uiElement", true, null, "EnergyPoint element: ").objectReferenceValue) 
            {
                drawer.Property("uiSliderGreen");
                drawer.Property("uiSliderLightGreen");
                drawer.Property("uiSliderPurple");
            }

            drawer.EndProperty();

            return true;
        }
    }

    [CustomPropertyDrawer(typeof(DeathblowPointData))]
    public partial class DeathblowPointDataDrawer : ComponentNUIPropertyDrawer
    {
        public override bool OnNUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (!base.OnNUI(position, property, label))
                return false;

            drawer.Field("currentValue", false, "Units", "Current value: ");
            drawer.Property("deathblowUISliderPoint");

            drawer.EndProperty();

            return true;
        }
    }

    [CustomPropertyDrawer(typeof(GameResultData))]
    public partial class GameResultDataDrawer : ComponentNUIPropertyDrawer
    {
        public override bool OnNUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (!base.OnNUI(position, property, label))
                return false;

            drawer.Field("tmpTxtGameResult", true, null, "Game Result txt: ");
            drawer.Field("gameResultAnimator", true, null, "Animator: ");
            drawer.Field("gameResultClip", true, null, "Clip: ");

            drawer.BeginSubsection("Button");

            drawer.Field("btnReset", true, null, "Reset: ");
            drawer.Field("btnQuit", true, null, "Quit: ");

            drawer.EndSubsection();

            drawer.BeginSubsection("Message");

            drawer.Field("winMessage", true, null, "Win: ");
            drawer.Field("loseMessage", true, null, "Lose: ");

            drawer.EndSubsection();

            drawer.EndProperty();

            return true;
        }
    }

#endif
}