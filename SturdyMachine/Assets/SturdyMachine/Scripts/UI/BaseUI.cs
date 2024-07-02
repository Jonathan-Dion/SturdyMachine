using System;

using UnityEngine;

using SturdyMachine.Component;

#if UNITY_EDITOR
using UnityEditor;
using NWH.NUI;
using NWH.VehiclePhysics2;
#endif

namespace SturdyMachine.UI {

    public enum BaseUIType { MainMenu, Battle, Gameplay }

    /// <summary>
    /// Represents configuration values ??regarding a ui slider element
    /// </summary>
    [Serializable, Tooltip("Represents configuration values ??regarding a ui slider element")]
    public struct UISliderData 
    {
        /// <summary>
        /// Represents the maximum value of this UI element
        /// </summary>
        [Tooltip("Represents the maximum value of this UI element")]
        public float maxValue;

        /// <summary>
        /// Represents the minimum and maximum positioning values
        /// X: Min
        /// Y: Max
        /// </summary>
        [Tooltip("Represents the minimum and maximum positioning values")]
        public Vector2 positionValues;

        /// <summary>
        /// Represents the element in the scene
        /// </summary>
        [Tooltip("Represents the element in the scene")]
        public RectTransform uiElement;
    }

    [Serializable]
    public partial class  BaseUI : BaseComponent
    {
        #region Attribut

        [SerializeField]
        protected BaseUIType baseUIType;

        #endregion

        #region Properties

        public BaseUIType GetBaseUIType => baseUIType;

        public float GetDistanceBetweenPositionValue(UISliderData pUISliderData) => pUISliderData.positionValues.x - pUISliderData.positionValues.y;

        Vector3 GetCurrentUISliderPosition(ref UISliderData pUISliderData, float pCurrentValue)
        {
            Vector3 currentUISliderPosition = pUISliderData.uiElement.localPosition;

            currentUISliderPosition.x += (pUISliderData.maxValue - pCurrentValue) / pUISliderData.maxValue * GetDistanceBetweenPositionValue(pUISliderData);

            return currentUISliderPosition;
        } 

        #endregion

        #region Method

        public virtual void Initialize(int pSturdyEnergyPointValue, int pMaxDeathblowValue) 
        {
            base.Initialize();
        }

        public virtual bool OnStart(BaseUIType pStartBaseUIType) 
        {
            if (!base.OnStart())
                return false;

            gameObject.SetActive(baseUIType == pStartBaseUIType);

            return true;
        }

        public virtual bool OnUpdate(bool pIsHitConfirmActivated, float pNextSturdyEnergyPointValue, float pNextDeathblowValue)
        {
            if (!base.OnUpdate())
                return false;

            return gameObject.activeSelf;
        }

        public virtual void OnEnabled(int pSturdyEnergyPointValue, int pMaxDeathblowValue) {

            if (!_isInitialized)
            {
                if (Application.isPlaying)
                {
                    base.OnEnabled();

                    Initialize(pSturdyEnergyPointValue, pMaxDeathblowValue);
                }

            }

            _isEnabled = true;
        }

        #endregion

    }

#if UNITY_EDITOR

    [CustomEditor(typeof(BaseUI))]
    public class BaseUIEditor : BaseComponentEditor
    {
        public override bool OnInspectorNUI()
        {
            if (!base.OnInspectorNUI())
                return false;

            drawer.Field("baseUIType", true, null, "UI Type: ");

            drawer.EndEditor(this);
            return true;
        }
    }

    [CustomPropertyDrawer(typeof(UISliderData))]
    public partial class UISliderDataDrawer : ComponentNUIPropertyDrawer
    {
        public override bool OnNUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (!base.OnNUI(position, property, label))
                return false;

            if (drawer.Field("uiElement", true, null, "Element: ").objectReferenceValue) 
            {
                drawer.Field("maxValue", false, "Units", "Max unit: ");
                drawer.Field("positionValues", true, null, "Positions: ");
            }

            drawer.EndProperty();
            return true;
        }
    }

#endif
}