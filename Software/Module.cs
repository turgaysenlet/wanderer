namespace Wandarer.Software
{
    public abstract class Module : Entity
    {
        public static List<Module> Modules { get; } = new List<Module>();
        public Module()
        {
            Modules.Add(this);
        }
        ~Module()
        {
            Modules.Remove(this);
        }
    }
}