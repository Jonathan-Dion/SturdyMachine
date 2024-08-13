using System;

using UnityEngine;

namespace SturdyMachine.Features.Fight
{
    [Serializable]
    public partial class FightSequencerModuleWrapper : FeatureModuleWrapper
    {
        [SerializeField]
        FightSequencerModule _module = new FightSequencerModule();

        public override FeatureModuleCategory GetFeatureModuleCategory => _module.GetFeatureModuleCategory();

        public override FeatureModule GetFeatureModule() => _module;

        public FightSequencerModule GetFightModule => _module;

        public override void SetFeatureModule(FeatureModule pFeatureModule)
        {
            _module = pFeatureModule as FightSequencerModule;
        }
    }
}
