namespace Wanderer.Software
{
    public abstract class Module : Entity
    {
        public static List<Module> Modules { get; } = new List<Module>();
        public static int ModuleCount { get; private set; } = 0;
        public int ModuleNo { get; private set; } = 0;
        private static Module AddModule(Module module)
        {
            if (module != null)
            {
                Modules.Add(module);
                module.ModuleNo = ModuleCount++;
            }
            return module;
        }
        public Module()
        {
            AddModule(this);
        }
        ~Module()
        {
            Modules.Remove(this);
        }
        public override string ToString()
        {
            return $"{base.ToString()} - Module {ModuleNo}";
        }
    }
}