using Intel.RealSense;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wandarer.Software.ImageProcessing
{
    public class D435
    {
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
                return new Bitmap(ColorFrame.Width, ColorFrame.Height, ColorFrame.Stride, System.Drawing.Imaging.PixelFormat.Format24bppRgb, ColorFrame.Data);
            }
        }
        public Bitmap DepthColorBitmap
        {
            get
            {
                VideoFrame depthColorFrame = colorizer.Process<VideoFrame>(DepthFrame);
                return new Bitmap(depthColorFrame.Width, depthColorFrame.Height, depthColorFrame.Stride, System.Drawing.Imaging.PixelFormat.Format24bppRgb, depthColorFrame.Data);
            }
        }

        public D435()
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
                Config.EnableStream(Intel.RealSense.Stream.Color, 640, 480, Format.Bgr8, 30);
                Config.EnableStream(Intel.RealSense.Stream.Depth, 640, 480, Format.Z16, 30);
                PipelineProfile = Pipeline.Start(Config);
                for (int i = 0; i < 10; i++)
                {
                    GrabFrame();
                }
                Started = true;
            }
            GrabFrame();
        }

        public void GrabFrame()
        {
            var frames = Pipeline.WaitForFrames();
            ColorFrame = frames.ColorFrame;
            DepthFrame = frames.DepthFrame;
        }
    }
}
