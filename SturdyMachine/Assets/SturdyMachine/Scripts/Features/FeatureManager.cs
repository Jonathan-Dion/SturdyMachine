using System;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
using NWH.NUI;
using NWH.VehiclePhysics2;
#endif

namespace SturdyMachine.Features 
{
    [Serializable]
    public partial class FeatureManager : SturdyComponent 
    {
        [SerializeField]
        List<FeatureModule> _featureModule = new List<FeatureModule>();

        public List<FeatureModule> GetFeatureModules => _featureModule;

        public FeatureModule GetSpecificFeatureModule(FeatureModule.FeatureModuleCategory pFeatureModuleCategory) 
        {
            for (int i = 0; i < _featureModule.Count; ++i) 
            {
                if (_featureModule[i].GetFeatureModuleCategory() == pFeatureModuleCategory)
                    return _featureModule[i];
            }

            return null;
        } 

        public override void Awake(Manager.Main pMain, SturdyBot pSturdyBot)
        {
            base.Awake(pMain, pSturdyBot);

            ReloadFeatureModule(pMain);

            for (int i = 0; i < _featureModule.Count; ++i)
                _featureModule[i].Awake(pMain, pSturdyBot);
        }

        public override void Initialize(MonsterBot[] pMonsterBot, SturdyBot pSturdyBot)
        {
            for (int i = 0; i < _featureModule.Count; ++i)
                _featureModule[i].Initialize(pMonsterBot, pSturdyBot);

            base.Initialize(pMonsterBot, pSturdyBot);
        }

        public override void UpdateRemote(MonsterBot[] pMonsterBot, SturdyBot pSturdyBot, Inputs.SturdyInputControl pSturdyInputControl)
        {
            if (!GetIsActivated)
                return;

            for (int i = 0; i < _featureModule.Count; ++i)
            {
                if (_featureModule[i].GetIsActivated)
                    _featureModule[i].UpdateRemote(pMonsterBot, pSturdyBot, pSturdyInputControl);
            }
        }

        public override void Enable(MonsterBot[] pMonsterBot, SturdyBot pSturdyBot)
        {
            for (int i = 0; i < _featureModule.Count; ++i)
                _featureModule[i].Enable(pMonsterBot, pSturdyBot);

            base.Enable(pMonsterBot, pSturdyBot);
        }

        public override void Disable()
        {
            for (int i = 0; i < _featureModule.Count; ++i)
                _featureModule[i].Disable();

            base.Disable();
        }

        public FM AddFeatureManager<FMW, FM>()
            where FMW : FeatureModuleWrapper
            where FM : FeatureModule
        {
            FeatureModule featureModule = _main.gameObject.AddComponent<FMW>().GetFeatureModule();

            _featureModule.Add(featureModule);

            ReloadFeatureModule(_main);

            return featureModule as FM;
        }

        public FM GetFeatureModule<FM>() where FM : FeatureModule 
        {
            if (!_main)
                return null;

            return _featureModule.FirstOrDefault(module => module.GetType() == typeof(FM)) as FM;
        }

        public void RemoveFeatureModule<FMW>()
            where FMW : FeatureModuleWrapper
        {
            if (!_main)
                return;

            FeatureModuleWrapper featureModuleWrapper = _main.GetComponent<FMW>();

            _featureModule.Remove(featureModuleWrapper.GetFeatureModule());

            if (Application.isPlaying)
                UnityEngine.Object.Destroy(featureModuleWrapper);
            else
                UnityEngine.Object.DestroyImmediate(featureModuleWrapper);

            ReloadFeatureModule(_main);
        }

        public void ReloadFeatureModule(Manager.Main pMain) 
        {
            _featureModule.Clear();

            List<FeatureModuleWrapper> featureModuleWrapper = pMain.gameObject.GetComponents<FeatureModuleWrapper>()?.ToList();

            if (featureModuleWrapper.Count == 0 || featureModuleWrapper == null)
                return;

            for (int i = 0; i < featureModuleWrapper.Count; ++i)
                _featureModule.Add(featureModuleWrapper[i].GetFeatureModule());
        }

        public override void FixedUpdate() { }

        public override void CleanMemory()
        {
            for (int i = 0; i < _featureModule.Count; ++i)
                _featureModule[i].CleanMemory();
        }
    }

#if UNITY_EDITOR

    [CustomPropertyDrawer(typeof(FeatureManager))]
    public partial class FeatureManagerDrawer : ComponentNUIPropertyDrawer 
    {
        bool _reloadFeatureModuleFlag;

        public FeatureManagerDrawer() 
        {
            _reloadFeatureModuleFlag = true;
        }

        public override bool OnNUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (!base.OnNUI(position, property, label))
                return false;

            Manager.Main main = SerializedPropertyHelper.GetTargetObjectWithProperty(property) as Manager.Main;

            FeatureManager featureManager = SerializedPropertyHelper.GetTargetObjectOfProperty(property) as FeatureManager;

            if (featureManager == null)
                drawer.EndProperty();

            if (_reloadFeatureModuleFlag)
            {
                _reloadFeatureModuleFlag = false;

                featureManager.ReloadFeatureModule(main);
            }

            drawer.Space();

            FeatureModuleWrapper[] moduleWrappers = main.GetComponents<FeatureModuleWrapper>();

            if (moduleWrappers.Length == 0)
            {
                drawer.Info("Use 'Add Component' button to add a module. Modules will appear here as they are added.");
                drawer.EndProperty();

                return true;
            }

            drawer.Label("Module Categories:");

            FeatureModule.FeatureModuleCategory[] moduleCategories =
                featureManager.GetFeatureModules.Select(m => m.GetFeatureModuleCategory()).Distinct().OrderBy(x => x).ToArray();

            int categoryIndex =
                drawer.HorizontalToolbar("moduleCategories", moduleCategories.Select(m => m.ToString()).ToArray());

            if (categoryIndex < 0)
                categoryIndex = 0;

            if (categoryIndex > moduleCategories.Length) 
            {
                drawer.EndProperty();
                return true;
            }

            drawer.Space(3);

            FeatureModule.FeatureModuleCategory category = moduleCategories[categoryIndex];

            for (int i = 0; i < moduleWrappers.Length; ++i)
            {
                if (moduleWrappers[i] == null || moduleWrappers[i].GetFeatureModule() == null)
                    continue;

                if (moduleWrappers[i].GetFeatureModule().GetFeatureModuleCategory() != category)
                    continue;

                drawer.EmbeddedObjectEditor<NVP_NUIEditor>(moduleWrappers[i], drawer.positionRect);
            }

            drawer.EndProperty();
            return true;
        }
    }

#endif
}