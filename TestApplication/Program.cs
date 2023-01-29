using Wanderer.Hardware;
using Wanderer.Hardware.Input;
using Wanderer.Software.ImageProcessing;
using Wanderer.Software.Mapping;
using Wanderer.Software;
using Wanderer.Software.Speech;
using Wanderer.Software.Api;

namespace Wanderer.Software.TestApplication
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
            MapCls map = new MapCls(10, 10, 0.1f);
            var entities = EntityCls.Entities;
            var devices = Wanderer.Hardware.Device.Devices;
            var modules = ModuleCls.Modules;
            foreach ( var entity in entities )
            {
                Console.WriteLine(entity);
            }
            RobotApiServer robotApiServer = new RobotApiServer() ;
            robotApiServer.Start();
        }
    }
}