using System;

using UnityEngine;

namespace SturdyMachine.Features.Fight
{
    [Serializable]
    public partial class FightModuleWrapper : FeatureModuleWrapper
    {
        [SerializeField]
        FightsModule _module = new FightsModule();

        public override FeatureModuleCategory GetFeatureModuleCategory => _module.GetFeatureModuleCategory();

        public override FeatureModule GetFeatureModule() => _module;

        public FightsModule GetFightModule => _module;

        public override void SetFeatureModule(FeatureModule pFeatureModule)
        {
            _module = pFeatureModule as FightsModule;
        }
    }
}
