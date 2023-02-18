using Intel.RealSense;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
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
        public List<Tuple<DateTime, double[]>> Poses = new();
        // 500ms collection time, 2hours recording, 120*60*2 = 14400 entries.
        public TimeSpan PoseRecordingPeriod = TimeSpan.FromMilliseconds(500);
        private DateTime LastPoseRecordingTime = DateTime.Now;

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
            // Collect GC or otherwise WaitForFrames gets stuck after 16 frames.
            GC.Collect();
            try
            {
                var frames = Pipeline.WaitForFrames();
                PoseFrame = frames.PoseFrame;
                FrameNo++;
                //Console.WriteLine($"T264 frame captured: {FrameNo} - [{PoseFrame.PoseData.translation.x},{PoseFrame.PoseData.translation.y},{PoseFrame.PoseData.translation.z}]");
                DateTime now = DateTime.Now;
                if (LastPoseRecordingTime + PoseRecordingPeriod < now)
                {
                    LastPoseRecordingTime = now;
                    AddPose(now, PosePositionOrientationDegrees());
                }
            } catch (Exception ex)
            {
                Console.WriteLine($"T264 frame capture failed: {FrameNo} - {ex.Message}");
            }
        }

        private void AddPose(DateTime now, double[] pose)
        {
            Poses.Add(new Tuple<DateTime, double[]>(now, pose));
        }

        public double[] Position()
        {
            if (PoseFrame == null || PoseFrame.PoseData == null)
            {
                return new double[] { 0, 0, 0 };
            }
            /* Original T264 pose coordinate system
             * 1. Positive X direction is towards right imager
             * 2. Positive Y direction is upwards toward the top of the device
             * 3. Positive Z direction is inwards toward the back of the device
             * The center of tracking corresponds to the center location between the right and left monochrome imagers on the device.
             */

            /* Robot pose coordinate system
             * 1. Positive X direction is towards right imager, right of the robot
             * 2. Positive Y direction is outward toward the front of the device/robot
             * 3. Positive Z direction is  upwards toward the top of the device/robot
             * The center of tracking corresponds to the center location between the right and left monochrome imagers on the device.
             */
            var x = PoseFrame.PoseData.translation.x;
            var y = -PoseFrame.PoseData.translation.z;
            var z = PoseFrame.PoseData.translation.y;
            return new double[] { x, y, z};
        }
        public double[] OrientationDegrees()
        {
            if (PoseFrame == null || PoseFrame.PoseData == null)
            {
                return new double[] { 0, 0, 0, 0};
            }
            return QuaternionToEulerDegrees(new double[] { PoseFrame.PoseData.rotation.x, PoseFrame.PoseData.rotation.y, PoseFrame.PoseData.rotation.z, PoseFrame.PoseData.rotation.w});
        }
        public double[] PosePositionOrientationDegrees()
        {
            var position = Position();
            var eulerDegrees = OrientationDegrees();
            return new double[] { position[0], position[1], position[2], eulerDegrees[0], eulerDegrees[1], eulerDegrees[2] };
        }
        public static double[] QuaternionToEulerDegrees(double[] quaternion)
        {
            var yaw = RadianToDegree( Math.Atan2(2 * quaternion[1] * quaternion[3] - 2 * quaternion[0] * quaternion[2], 1 - 2 * quaternion[1] * quaternion[1] - 2 * quaternion[2] * quaternion[2]));
            var pitch = RadianToDegree(Math.Asin(2 * quaternion[0] * quaternion[1] + 2 * quaternion[2] * quaternion[3]));
            var roll = RadianToDegree(Math.Atan2(2 * quaternion[0] * quaternion[3] - 2 * quaternion[1] * quaternion[2], 1 - 2 * quaternion[0] * quaternion[0] - 2 * quaternion[2] * quaternion[2]));
            return new double[] { yaw, pitch, roll };
        }
        public static double RadianToDegree(double radians)
        {
            radians += 2 * Math.PI;
            radians %= 2 * Math.PI;
            radians += 2 * Math.PI;
            radians %= 2 * Math.PI;
            return (180.0 / Math.PI) * radians;
        }
        public static double DegreeToRadian(double degree)
        {
            degree += 360.0;
            degree %= 360.0;
            degree += 360.0;
            degree %= 360.0;
            return (Math.PI / 180.0) * degree;
        }
    }
}
