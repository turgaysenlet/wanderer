using Wandarer.Hardware;
using Wandarer.Hardware.Input;
using Wandarer.Software.ImageProcessing;
using Wandarer.Software.Mapping;
using Wandarer.Software;

namespace Wandarer.Software.TestApplication
{
    internal class Program
    {
        static void Main(string[] args)
        {
            InputManager inputManager = new InputManager();
            //Keyboard keyboard = new Keyboard();
            //Mouse mouse = new Mouse();
            RealSense realSense = new RealSense();
            realSense.Start();
            Map map = new Map(10, 10, 0.1f);
            var entities = Entity.Entities;
            var devices = Wandarer.Hardware.Device.Devices;
            var modules = Module.Modules;

            foreach ( var entity in entities )
            {
                Console.WriteLine(entity);
            }
        }
    }
}