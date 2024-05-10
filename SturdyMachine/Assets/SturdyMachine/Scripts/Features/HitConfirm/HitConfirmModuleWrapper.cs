using SturdyMachine.Features.Focus;
using System;

using UnityEngine;

namespace SturdyMachine.Features.HitConfirm {

    [Serializable]
    public partial class HitConfirmModuleWrapper : FeatureModuleWrapper {

        [SerializeField]
        HitConfirmModule _module = new HitConfirmModule();

        public override FeatureModuleCategory GetFeatureModuleCategory => _module.GetFeatureModuleCategory();

        public override FeatureModule GetFeatureModule() => _module;

        public override void SetFeatureModule(FeatureModule pFeatureModule)
        {
            _module = pFeatureModule as HitConfirmModule;
        }
    }
}