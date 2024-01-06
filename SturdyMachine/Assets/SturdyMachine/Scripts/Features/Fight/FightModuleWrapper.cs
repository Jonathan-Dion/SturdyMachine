using System;

using UnityEngine;

namespace SturdyMachine.Features.Fight
{
    [Serializable]
    public partial class FightModuleWrapper : FeatureModuleWrapper
    {
        [SerializeField]
        FightModule _module = new FightModule();

        public override FeatureModuleCategory GetFeatureModuleCategory => _module.GetFeatureModuleCategory();

        public override FeatureModule GetFeatureModule() => _module;

        public FightModule GetFightModule => _module;

        public override void SetFeatureModule(FeatureModule pFeatureModule)
        {
            _module = pFeatureModule as FightModule;
        }
    }
}
