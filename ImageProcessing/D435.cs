using Intel.RealSense;
using Intel.RealSense.Math;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Wanderer.Software.Common;

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
        private PointCloud pc = new PointCloud();

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
        private VertexRgb[] vertices = new VertexRgb[0];
        public VertexRgb[] Vertices { get { return vertices; } }

        public D435()
        {
            Name = "D435 - 3D Camera";
            DeviceType = DeviceTypeEnu.Sensor;
        }
        Timer timer;
        public void Start()
        {
            // Simplest working example:
            // var pipe = new Pipeline();
            // pipe.Start();
            // int frameNo = 0;
            // while (true)
            // {
            //    using (var frames = pipe.WaitForFrames())
            //    using (var depth = frames.DepthFrame)
            //    {
            //        Console.WriteLine("The camera is pointing at an object " +
            //            depth.GetDistance(depth.Width / 2, depth.Height / 2).ToString("0.00") + " meters away\t");
            //        Console.WriteLine(frameNo++);
            //        Console.SetCursorPosition(0, 0);
            //    }
            // }

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
        Thread thread;

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
                        lock (this)
                        {
                            // If you don't collect here, WaitForFrames gets stuck after 16 frames
                            GC.Collect();
                            ColorFrame = frames.ColorFrame;
                            DepthFrame = frames.DepthFrame;
                            if (FrameNo % 10 == 0)
                            {
                                CalculatePointCloud(pc, DepthFrame, ref vertices);
                            }
                            FrameNo++;
                            Console.WriteLine($"D435 frame captured: {FrameNo}");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }

        public static void CalculatePointCloud(PointCloud pc, DepthFrame depthFrame, ref VertexRgb[] verticesAll)
        {
            using (Points p = pc.Calculate(depthFrame))
            using (var points = pc.Process(depthFrame).As<Points>())
            {
                if (verticesAll == null || points.Count != verticesAll.Length)
                {
                    verticesAll = new VertexRgb[points.Count];
                }
                var verticesAllList = new List<VertexRgb>();
                Vertex[] vertices = new Vertex[points.Count];
                // CopyVertices is extensible, any of these will do:
                // var vertices = new float[points.Count * 3];
                // var vertices = new Intel.RealSense.Math.Vertex[points.Count];
                // var vertices = new UnityEngine.Vector3[points.Count];
                // var vertices = new System.Numerics.Vector3[points.Count]; // SIMD
                // var vertices = new GlmSharp.vec3[points.Count];
                //  var vertices = new byte[points.Count * 3 * sizeof(float)];
                points.CopyVertices(vertices);
                for (int k = 0; k < points.Count; k++)
                {
                    Vertex vertex = vertices[k];
                    // Point on or behind the camera should be pushed to in front of the camera.
                    if (vertex.z <= 0)
                    {
                        vertex.z = 10;
                    }
                    // Vertex convention from this point on:
                    // x left (negative), right (positive) of the camera
                    // y away (positive), closer/behind (negative) to the camera
                    // z above/sky (positive), below/floor (negative) to the camera
                    // Pixel coordinates will remain as is, X: image left-right, Y: image up-down (similar to -vertex.z)
                    // This either needs to be reverted when doing projection, or the matrices should be changed
                    // No optimization works, this takes 100ms. Only making it a struct makes it 10ms, but that has other problems.
                    verticesAll[k] = new VertexRgb(k, vertex.x, vertex.z, -vertex.y);
                    verticesAll[k].originalVertex = vertex;
                    verticesAll[k].pointType = VertexRgb.PointTypeEnu.Initialized;
                }
                //int parts = 640;
                //int partSize = points.Count / parts;
                //ParallelOptions parallelOptions = new ParallelOptions();
                //parallelOptions.MaxDegreeOfParallelism = 20;
                //var vvv= new VertexRgb();// new VertexRgb(k, vertex.x, vertex.z, -vertex.y);
                //var parallel = Parallel.For(0, parts, parallelOptions, (iter) =>
                //{
                //    for (int k = iter * partSize; k < (iter + 1) * partSize; k++)
                //    {
                //        Vertex vertex = vertices[k];
                //        vvv.Clone();
                //        //verticesAll[k] = vvv.Clone();// new VertexRgb();// new VertexRgb(k, vertex.x, vertex.z, -vertex.y);
                //        //Interlocked.Exchange(ref verticesAll[k], new VertexRgb(k, vertex.x, vertex.z, -vertex.y));
                //    }
                //});
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
            return DepthFrame.GetDistance(i, j);
            //unsafe
            //{                
            //    ushort* depth_data = (ushort*)DepthFrame.Data.ToPointer();
            //    ushort d = (ushort)depth_data[DepthFrame.Width * j + i];
            //    return (float)d;
            //}
        }
    }
}
