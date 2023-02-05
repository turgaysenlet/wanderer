using Intel.RealSense;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Text.RegularExpressions;
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
        private Thread FrameCaptureThread;
        private CancellationTokenSource cts = new CancellationTokenSource();

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
                    if (DepthFrame == null || ColorFrame == null)
                    {
                        GrabFrame();
                    }
                }
            }
        }

        public Bitmap DepthColorBitmap
        {
            get
            {
                try
                {
                    AutoGrab();
                    if (colorizer != null)
                    {
                        VideoFrame depthColorFrame = colorizer.Process<VideoFrame>(DepthFrame);
                        return new Bitmap(depthColorFrame.Width, depthColorFrame.Height, depthColorFrame.Stride, System.Drawing.Imaging.PixelFormat.Format24bppRgb, depthColorFrame.Data);
                    }
                }
                catch
                {
                }
                return new Bitmap(DepthFrame.Width, DepthFrame.Height);
            }
        }

        public long FrameNo { get; private set; } = 0;

        public D435()
        {
            Name = "D435 - 3D Camera";
            DeviceType = DeviceTypeEnu.Sensor;
        }
        Timer timer;
        public void Start()
        {
            if (!Started)
            {
                try
                {
                    Console.WriteLine($"Starting D435 - Initial");
                    Context = new Context();
                    DeviceList devices = Context.QueryDevices();
                    if (devices == null || devices.Count == 0)
                    {
                        Console.WriteLine($"Starting D435 - No devices found");
                        State = DeviceStateEnu.NotFound;
                        return;
                    }
                    Console.WriteLine($"Starting D435 - Found devices: {devices.Count}");
                    Pipeline = new Pipeline(Context);
                    Config = new Config();
                    Config.EnableStream(Intel.RealSense.Stream.Color, 640, 480, Format.Bgr8, 15);
                    Config.EnableStream(Intel.RealSense.Stream.Depth, 640, 480, Format.Z16, 15);
                    Console.WriteLine($"Starting D435 - Config created");
                    PipelineProfile = Pipeline.Start(Config);
                    Console.WriteLine($"Starting D435 - Pipeline started");
                    if (PipelineProfile.Device == null)
                    {
                        State = DeviceStateEnu.NotFound;
                        Console.WriteLine($"Starting D435 - Pipeline started - no device found");
                    }
                    else
                    {
                        State = DeviceStateEnu.Found;
                        Console.WriteLine($"Starting D435 - Pipeline started - device found");
                    }
                    for (int i = 0; i < 2; i++)
                    {
                        GrabFrame();
                    }
                    State = DeviceStateEnu.Started;
                    Started = true;
                    GrabFrame();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Starting D435 - Failed - {ex.Message}");
                    Started = false;
                    State = DeviceStateEnu.Failed;
                }
                Console.WriteLine($"Starting D435 - Pipeline started - starting frame capture loop");
                timer = new Timer(new TimerCallback(TimerElapsed), null, 0, 100);
            }
        }
        private void TimerElapsed(Object stateInfo)
        {
            GrabFrame();
        }
        private void GrabFrame()
        {
            try
            {
                if (Pipeline != null)
                {
                    using (var frames = Pipeline.WaitForFrames())
                    {
                        AutoGrabFrameTime = DateTime.Now;
                        //lock (this)
                        {
                            ColorFrame = frames.ColorFrame;
                            DepthFrame = frames.DepthFrame;
                            FrameNo++;
                            Console.WriteLine($"D435 frame captured: {FrameNo}");
                        }
                        //return new Tuple<VideoFrame, DepthFrame>(ColorFrame, DepthFrame);
                    }
                }
                //return new Tuple<VideoFrame, DepthFrame>(null, null);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                //return new Tuple<VideoFrame, DepthFrame>(null, null);
            }
        }

        public float Distance(int i = -1, int j = -1)
        {
            AutoGrab();
            if (i < 0)
            {
                i = DepthFrame.Width / 2;
            }
            if (j < 0)
            {
                j = DepthFrame.Height / 2;
            }
            unsafe
            {
                ushort* depth_data = (ushort*)DepthFrame.Data.ToPointer();
                ushort d = (ushort)depth_data[DepthFrame.Width * j + i];
                return (float)d;
            }
        }
    }
}
