using System;

using UnityEngine;

namespace SturdyMachine.Features.StateConfirm {

    [Serializable]
    public partial class StateConfirmModuleWrapper : FeatureModuleWrapper {

        [SerializeField]
        StateConfirmModule _module = new StateConfirmModule();

        public override FeatureModuleCategory GetFeatureModuleCategory => _module.GetFeatureModuleCategory();

        public override FeatureModule GetFeatureModule() => _module;

        public override void SetFeatureModule(FeatureModule pFeatureModule) => _module = pFeatureModule as StateConfirmModule;
    }
}