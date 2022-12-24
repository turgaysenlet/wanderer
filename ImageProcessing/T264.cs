using Intel.RealSense;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Wandarer.Software.ImageProcessing
{
    public class T264 : Wandarer.Hardware.Device
    {
        public Pipeline Pipeline { get; set; }
        public Context Context { get; set; }
        public Device Device { get; set; }
        public Config Config { get; set; }
        public PipelineProfile PipelineProfile { get; set; }
        public PoseFrame PoseFrame { get; private set; }
        public bool Started { get; private set; }

        public T264()
        {
            Name = "T264 - Localization Camera";
        }
        public void Start()
        {
            if (!Started)
            {
                try
                {
                    Context = new Context();
                    DeviceList devices = Context.QueryDevices();
                    if (devices == null || devices.Count == 0)
                    {
                        State = DeviceStateEnu.NotFound;
                        return;
                    }
                    Pipeline = new Pipeline(Context);
                    Config = new Config();
                    Config.EnableStream(Intel.RealSense.Stream.Pose, Format.SixDOF);
                    PipelineProfile = Pipeline.Start(Config);
                    if (PipelineProfile.Device == null)
                    {
                        State = DeviceStateEnu.NotFound;
                    }
                    else
                    {
                        State = DeviceStateEnu.Found;
                    }
                    GrabFrame();
                    Started = true;
                }
                catch (Exception ex)
                {
                    Started = false;
                    State = DeviceStateEnu.Failed;
                }
            }
        }

        public PoseFrame GrabFrame()
        {
            var frames = Pipeline.WaitForFrames();
            PoseFrame = frames.PoseFrame;
            return PoseFrame;
        }
    }
}
