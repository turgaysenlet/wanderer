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
            Map map = new Map(10, 10, 0.1f);
            var entities = Entity.Entities;
            var devices = Wanderer.Hardware.Device.Devices;
            var modules = Module.Modules;
            foreach (var entity in entities)
            {
                Console.WriteLine(entity);
            }
            RobotApiServer robotApiServer = new RobotApiServer();
            //realSense.Start();
            realSense.T264.Start();
            realSense.T264.GrabFrame();
            robotApiServer.Start(realSense.D435);

        }
    }
}