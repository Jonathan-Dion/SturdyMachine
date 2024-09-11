using System;

using UnityEngine;

namespace SturdyMachine.Features.Audio
{
    [Serializable]
    public partial class AudioModuleWrapper : FeatureModuleWrapper
    {
        [SerializeField]
        AudioModule _module = new AudioModule();

        public override FeatureModuleCategory GetFeatureModuleCategory => _module.GetFeatureModuleCategory();

        public override FeatureModule GetFeatureModule() => _module;

        public AudioModule GetAudioModule => _module;

        public override void SetFeatureModule(FeatureModule pFeatureModule)
        {
            _module = pFeatureModule as AudioModule;
        }
    }
}
