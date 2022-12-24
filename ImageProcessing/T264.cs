using Intel.RealSense;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wandarer.Software.ImageProcessing
{
    public class T264
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
        }
        public void Start()
        {
            if (!Started)
            {
                Context = new Context();
                DeviceList devices = Context.QueryDevices();
                Pipeline = new Pipeline(Context);
                Config = new Config();
                Config.EnableStream(Intel.RealSense.Stream.Pose, Format.SixDOF);
                PipelineProfile = Pipeline.Start(Config);
                Started = true;
            }
            GrabFrame();
        }

        public void GrabFrame()
        {
            var frames = Pipeline.WaitForFrames();
            PoseFrame = frames.PoseFrame;
        }
    }
}
