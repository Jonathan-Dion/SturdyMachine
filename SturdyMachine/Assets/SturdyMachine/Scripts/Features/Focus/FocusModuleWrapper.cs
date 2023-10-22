using SturdyMachine.Features.Fight;
using System;

using UnityEngine;

namespace SturdyMachine.Features.Focus
{
    [Serializable]
    public partial class FocusModuleWrapper : FeatureModuleWrapper 
    {
        [SerializeField]
        FocusModule _module = new FocusModule();

        public override FeatureModule.FeatureModuleCategory GetFeatureModuleCategory => _module.GetFeatureModuleCategory();

        public override FeatureModule GetFeatureModule() => _module;

        public FocusModule GetFocusModule => _module;

        public override void SetFeatureModule(FeatureModule pFeatureModule)
        {
            _module = pFeatureModule as FocusModule;
        }
    }
}