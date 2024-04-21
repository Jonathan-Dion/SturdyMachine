using System;

using UnityEngine;

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

    public class BattleUI : BaseUI 
    {
        #region Attribut

        [SerializeField, Tooltip("")]
        EnergyPointData _energyPointData;

        [SerializeField, Tooltip("")]
        DeathblowPointData _deathblowPointData;

        #endregion

        #region Properties



        #endregion

        #region Methods

        public virtual void Initialize(int pSturdyEnergyPointValue, int pMaxDeathblowValue)
        {
            EnergyPointInit(pSturdyEnergyPointValue);

            _deathblowPointData.deathblowUISliderPoint.maxValue = pMaxDeathblowValue;

            base.Initialize();
        }

        public virtual bool OnUpdate(float pNextSturdyEnergyPointValue, float pNextDeathblowValue) 
        {
            if (!base.OnUpdate())
                return false;

            EnergyPointUpdate(pNextSturdyEnergyPointValue);

            DeathblowPointUpdate(pNextDeathblowValue);

            return true;
        }

        void EnergyPointInit(int pSturdyEnergyPointValue) 
        {
            //Green
            _energyPointData.uiSliderGreen.maxValue = pSturdyEnergyPointValue;
            _energyPointData.currentValue = _energyPointData.uiSliderGreen.maxValue;
        }

        void EnergyPointUpdate(float pNextSturdyEnergyPointValue) 
        {
            if (pNextSturdyEnergyPointValue == 0)
                return;

            Vector3 currentUISliderPosition = _energyPointData.uiElement.localPosition;

            _energyPointData.currentValue -= pNextSturdyEnergyPointValue;

            currentUISliderPosition.x += (_energyPointData.uiSliderGreen.maxValue - _energyPointData.currentValue) / _energyPointData.uiSliderGreen.maxValue * GetDistanceBetweenPositionValue(_energyPointData.uiSliderGreen);
            _energyPointData.uiSliderGreen.uiElement.localPosition = currentUISliderPosition;
        }

        void DeathblowPointUpdate(float pNextSturdyEnergyPointValue)
        {
            if (pNextSturdyEnergyPointValue == 0)
                return;

            Vector3 currentUISliderPosition = _deathblowPointData.deathblowUISliderPoint.uiElement.localPosition;

            _deathblowPointData.currentValue += pNextSturdyEnergyPointValue;

            currentUISliderPosition.x = (_deathblowPointData.deathblowUISliderPoint.maxValue - _deathblowPointData.currentValue) / _deathblowPointData.deathblowUISliderPoint.maxValue * GetDistanceBetweenPositionValue(_energyPointData.uiSliderGreen);

            _deathblowPointData.deathblowUISliderPoint.uiElement.localPosition = currentUISliderPosition;
        }

        #endregion
    }

    [CustomEditor(typeof(BattleUI))]
    public class BattleUIEditor : BaseUIEditor
    {
        public override bool OnInspectorNUI()
        {
            if (!base.OnInspectorNUI())
                return false;

            drawer.Property("_energyPointData");
            drawer.Property("_deathblowPointData");
            
            drawer.EndEditor(this);

            return true;
        }
    }

    [CustomPropertyDrawer(typeof(EnergyPointData))]
    public partial class EnergyPointDataDrawer : ComponentNUIPropertyDrawer{
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
}