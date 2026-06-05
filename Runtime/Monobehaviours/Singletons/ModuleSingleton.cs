using System;
using System.Collections.Generic;
using System.Linq;
using Models.Module;

namespace Monobehaviours.Singletons
{
    public class ModuleSingleton: Singleton<ModuleSingleton>
    {
        public const string StandaloneModuleId = "standalone";

        private static readonly IReadOnlyList<ModuleDefinition> Modules = new List<ModuleDefinition>
        {
            TestModule.GetTestModule()
        };

        private ModuleDefinition _activeModule;

        public ModuleDefinition ActiveModule => _activeModule ??= GetDefaultModule();

        public IReadOnlyList<ModuleDefinition> GetAll()
        {
            return Modules;
        }

        public bool TryGetById(string moduleId, out ModuleDefinition module)
        {
            module = null;
            if (string.IsNullOrWhiteSpace(moduleId))
                return false;

            module = Modules.FirstOrDefault(candidate =>
                string.Equals(candidate.Id, moduleId.Trim(), StringComparison.OrdinalIgnoreCase));
            return module != null;
        }

        public void SetActive(ModuleDefinition module)
        {
            _activeModule = module ?? throw new ArgumentNullException(nameof(module));
        }

        public bool TrySetActive(string moduleId)
        {
            if (!TryGetById(moduleId, out var module))
                return false;

            SetActive(module);
            return true;
        }

        public void ResetToDefault()
        {
            _activeModule = GetDefaultModule();
        }

        private ModuleDefinition GetDefaultModule()
        {
            if (TryGetById(StandaloneModuleId, out var module))
                return module;

            if (Modules.Count > 0)
                return Modules[0];

            throw new InvalidOperationException("No modules are registered.");
        }
    }
}
