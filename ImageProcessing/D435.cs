using Intel.RealSense;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Wanderer.Software.ImageProcessing
{
    public class D435 : Wanderer.Hardware.Device
    {
        /// <summary>
        /// Grab frame when ColorBitmap or DepthColorBitmap is called
        /// </summary>
        public bool AutoGrabFrame { get; set; } = true;
        public TimeSpan AutoGrabFrameTtl { get; set; } = TimeSpan.FromMilliseconds(200);
        private DateTime AutoGrabFrameTime { get; set; } = DateTime.Now;
        public Pipeline Pipeline { get; set; }
        public Context Context { get; set; }
        public Device Device { get; set; }
        public Config Config { get; set; }
        public PipelineProfile PipelineProfile { get; set; }
        public VideoFrame ColorFrame { get; private set; }
        public DepthFrame DepthFrame { get; private set; }
        private Colorizer colorizer = new Colorizer();
        public bool Started { get; private set; }

        public Bitmap ColorBitmap
        {
            get
            {
                AutoGrab();
                return new Bitmap(ColorFrame.Width, ColorFrame.Height, ColorFrame.Stride, System.Drawing.Imaging.PixelFormat.Format24bppRgb, ColorFrame.Data);
            }
        }

        private void AutoGrab()
        {
            if (AutoGrabFrame)
            {
                if (DateTime.Now > AutoGrabFrameTime + AutoGrabFrameTtl)
                {
                    GrabFrame();
                }
            }
        }

        public Bitmap DepthColorBitmap
        {
            get
            {
                AutoGrab();
                VideoFrame depthColorFrame = colorizer.Process<VideoFrame>(DepthFrame);
                return new Bitmap(depthColorFrame.Width, depthColorFrame.Height, depthColorFrame.Stride, System.Drawing.Imaging.PixelFormat.Format24bppRgb, depthColorFrame.Data);
            }
        }

        public D435()
        {
            Name = "D435 - 3D Camera";
            DeviceType = DeviceTypeEnu.Sensor;
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
                    Config.EnableStream(Intel.RealSense.Stream.Color, 640, 480, Format.Bgr8, 30);
                    Config.EnableStream(Intel.RealSense.Stream.Depth, 640, 480, Format.Z16, 30);
                    PipelineProfile = Pipeline.Start(Config);
                    if (PipelineProfile.Device == null)
                    {
                        State = DeviceStateEnu.NotFound;
                    }
                    else
                    {
                        State = DeviceStateEnu.Found;
                    }
                    for (int i = 0; i < 10; i++)
                    {
                        GrabFrame();
                    }
                    State = DeviceStateEnu.Started;
                    Started = true;
                    GrabFrame();
                }
                catch (Exception ex)
                {
                    Started = false;
                    State = DeviceStateEnu.Failed;
                }
            }
        }

        public Tuple<VideoFrame, DepthFrame> GrabFrame()
        {
            try
            {
                var frames = Pipeline.WaitForFrames();
                AutoGrabFrameTime = DateTime.Now;
                ColorFrame = frames.ColorFrame;
                DepthFrame = frames.DepthFrame;
                return new Tuple<VideoFrame, DepthFrame>(ColorFrame, DepthFrame);
            }
            catch (Exception ex)
            {
                return new Tuple<VideoFrame, DepthFrame>(null, null);
            }
        }
    }
}
