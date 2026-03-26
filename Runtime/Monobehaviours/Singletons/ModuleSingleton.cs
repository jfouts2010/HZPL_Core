using Models.Module;

namespace Monobehaviours.Singletons
{
    public class ModuleSingleton: Singleton<ModuleSingleton>
    {
        public ModuleData ModuleData => TestModule.GetTestModule();
    }
}