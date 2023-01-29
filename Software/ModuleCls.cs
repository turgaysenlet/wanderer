namespace Wanderer.Software
{
    public abstract class ModuleCls : EntityCls
    {
        public static List<ModuleCls> Modules { get; } = new List<ModuleCls>();
        public static int ModuleCount { get; private set; } = 0;
        public int ModuleNo { get; private set; } = 0;
        private static ModuleCls AddModule(ModuleCls module)
        {
            if (module != null)
            {
                Modules.Add(module);
                module.ModuleNo = ModuleCount++;
            }
            return module;
        }
        public ModuleCls()
        {
            AddModule(this);
        }
        ~ModuleCls()
        {
            Modules.Remove(this);
        }
        public override string ToString()
        {
            return $"{base.ToString()} - Module {ModuleNo}";
        }

        protected ModuleStateEnu state = ModuleStateEnu.Unknown;

        public ModuleStateEnu State
        {
            get { return state; }
            protected set { state = value; }
        }

        public enum ModuleStateEnu
        {
            Unknown,
            Created,
            Started,
            Stopped,
            Failed
        }
    }
}