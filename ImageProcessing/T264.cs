using Intel.RealSense;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Wanderer.Software.ImageProcessing
{
    public class T264 : Wanderer.Hardware.Device
    {
        private Timer timer;

        public Pipeline Pipeline { get; set; }
        public Context Context { get; set; }
        public Device Device { get; set; }
        public Config Config { get; set; }
        public PipelineProfile PipelineProfile { get; set; }
        public PoseFrame PoseFrame { get; private set; }
        public bool Started { get; private set; }
        public long FrameNo { get; private set; } = 0;

        public T264()
        {
            Name = "T264 - Localization Camera";
            DeviceType = DeviceTypeEnu.Sensor;
        }
        public void Start()
        {
            if (!Started)
            {
                try
                {
                    Console.WriteLine($"Starting T264 - Initial");
                    Context = new Context();
                    DeviceList devices = Context.QueryDevices();
                    if (devices == null || devices.Count == 0)
                    {
                        Console.WriteLine($"Starting T264 - No devices found");
                        State = DeviceStateEnu.NotFound;
                        return;
                    }
                    Console.WriteLine($"Starting T264 - Found devices: {devices.Count}");
                    Pipeline = new Pipeline(Context);
                    Config = new Config();
                    Config.EnableStream(Intel.RealSense.Stream.Pose, Format.SixDOF);
                    Console.WriteLine($"Starting T264 - Config created");
                    PipelineProfile = Pipeline.Start(Config);
                    Console.WriteLine($"Starting T264 - Pipeline started");
                    if (PipelineProfile.Device == null)
                    {
                        State = DeviceStateEnu.NotFound;
                        Console.WriteLine($"Starting T264 - Pipeline started - no device found");
                    }
                    else
                    {
                        State = DeviceStateEnu.Found;
                        Console.WriteLine($"Starting T264 - Pipeline started - device found");
                    }
                    GrabFrame();
                    State = DeviceStateEnu.Started;
                    Started = true;
                }
                catch (Exception ex)
                {
                    Started = false;
                    State = DeviceStateEnu.Failed;
                    Console.WriteLine($"Starting T264 - Failed - {ex.Message}");                    
                }
                timer = new Timer(new TimerCallback(TimerElapsed), null, 0, 100);
            }
        }
        private void TimerElapsed(Object stateInfo)
        {
            GrabFrame();
        }
        public void GrabFrame()
        {
            var frames = Pipeline.WaitForFrames();
            PoseFrame = frames.PoseFrame;
            FrameNo++;
            Console.WriteLine($"D435 frame captured: {FrameNo} - [{PoseFrame.PoseData.translation.x},{PoseFrame.PoseData.translation.y},{PoseFrame.PoseData.translation.z}]");
        }
    }
}
