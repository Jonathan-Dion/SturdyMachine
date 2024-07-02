using System;

using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
using NWH.NUI;
using NWH.VehiclePhysics2;
#endif

namespace SturdyMachine.UI {

    public class GameplayUI : BaseUI {

        #region Attributs

        [SerializeField]
        BattleUI _battleUI;

        #endregion

        #region Properties

        public BattleUI GetBattleUI => _battleUI;

        #endregion

        #region Methods

        public override void Initialize()
        {
            base.Initialize();

            _battleUI.Initialize(5, 5);
        }

        public override void OnAwake()
        {
            base.OnAwake();

            _battleUI.OnAwake();
        }

        public override bool OnStart(BaseUIType pStartBaseUIType)
        {
            _battleUI.OnStart(pStartBaseUIType);

            return true;
        }

        public override bool OnUpdate(bool pIsHitConfirmActivated, float pNextSturdyEnergyPointValue, float pNextDeathblowValue)
        {
            if (!base.OnUpdate())
                return false;

            _battleUI.OnUpdate(pIsHitConfirmActivated, pNextSturdyEnergyPointValue, pNextDeathblowValue);

            return true;
        }

        public override void OnEnabled()
        {
            _battleUI.OnEnabled();

            base.OnEnabled();
        }

        #endregion
    }

#if UNITY_EDITOR

    [CustomEditor(typeof(GameplayUI))]
    public class GameplayUIEditor : BaseUIEditor
    {
        public override bool OnInspectorNUI()
        {
            if (!base.OnInspectorNUI())
                return false;

            drawer.Field("_battleUI");

            drawer.EndEditor(this);

            return true;
        }
    }

#endif
}