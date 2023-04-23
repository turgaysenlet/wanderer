using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Emgu.CV.Structure;
using Emgu.CV;
using Microsoft.Azure.Kinect.Sensor;
using Emgu.CV.CvEnum;

namespace Wanderer.Software.ImageProcessing
{
    public class Kinect : Wanderer.Hardware.Device
    {
        private Device device;
        private Transformation transformation;
        public Image<Gray, byte> map = null;
        public void Start()
        {
            DeviceType = DeviceTypeEnu.Sensor;
            Name = "K4A";
            State = DeviceStateEnu.Unknown;
            device = Device.Open();
            DeviceConfiguration config = new DeviceConfiguration();
            config.ColorFormat = Microsoft.Azure.Kinect.Sensor.ImageFormat.ColorBGRA32;
            config.ColorResolution = ColorResolution.R720p;
            config.DepthMode = DepthMode.WFOV_2x2Binned;
            config.CameraFPS = FPS.FPS5;
            config.SynchronizedImagesOnly = true;
            config.WiredSyncMode = WiredSyncMode.Standalone;
            device.StartCameras(config);
            State = DeviceStateEnu.Started;
            transformation = device.GetCalibration().CreateTransformation();            
        }

        public void Stop()
        {
            device.StopCameras();
            State = DeviceStateEnu.Stopped;
            device.Dispose();
            State = DeviceStateEnu.Unknown;
        }

        public void Capture()
        {
            using (Capture capture = device.GetCapture())
            {
                //Image colorImage = capture.Color;
                Image depthImage = capture.Depth;

                //int colorWidth = colorImage.WidthPixels;
                //int colorHeight = colorImage.HeightPixels;
                //byte[] colorData = colorImage.Memory.ToArray();
                byte[] depthData = depthImage.Memory.ToArray();
                //using (Image<Bgra, Byte> colorEmguImage = new Image<Bgra, Byte>((int)colorImage.WidthPixels, (int)colorImage.HeightPixels))
                {
                    //colorEmguImage.Bytes = colorData;
                    //colorEmguImage.Save(@"c:\temp\color.png");
                }
                using (Image<Gray, Int16> depthEmguImage = new Image<Gray, Int16>((int)depthImage.WidthPixels, (int)depthImage.HeightPixels))
                {
                    depthEmguImage.Bytes = depthData;
                    Image<Gray, float> depthEmguImageFloat = new Image<Gray, float>((int)depthImage.WidthPixels, (int)depthImage.HeightPixels);
                    //depthEmguImage.Save(@"c:\temp\depth_uint16.tif");
                    //depthEmguImage.Save(@"c:\temp\depth_uint16.png");
                    depthEmguImageFloat.ConvertFrom(depthEmguImage);
                    //depthEmguImageFloat.Save(@"c:\temp\depth_float.tif");
                    //depthEmguImageFloat.Save(@"c:\temp\depth_float.png");
                    var depthEmguImageFloatScaled = depthEmguImage.ConvertScale<float>(1 / 1024.0, 0);
                    //depthEmguImageFloatScaled.Save(@"c:\temp\depth_float_scaled.tif");
                    //depthEmguImageFloatScaled.Save(@"c:\temp\depth_float_scaled.png");
                }
                using (var pointCloudImage = transformation.DepthImageToPointCloud(depthImage))
                {
                    XYZMm[] pointCloud = PointCloudImageToPointCloud(pointCloudImage);                    
                    short cameraHeightMillimeter = 150;
                    short sliceHeightMillimeter = (short)(cameraHeightMillimeter / 2);
                    short mapWidthMillimeter = 1_000;
                    short mapLengthMillimeter = 1_000;
                    PointCloudToMap(ref map, mapWidthMillimeter, mapLengthMillimeter, pointCloud, sliceHeightMillimeter);
                }
            }
        }

        private void PointCloudToMap(ref Image<Gray, byte> map, short mapWidthMillimeter, short mapLengthMillimeter, XYZMm[] pointCloud, short sliceHeightMillimeter)
        {
            short bottom = -200;
            short top = 50;
            //if (map == null)
            {
                map = new Image<Gray, byte>(mapWidthMillimeter, mapLengthMillimeter);
            }
            var centerX = mapWidthMillimeter / 2;
            var centerY = mapLengthMillimeter / 10;
            unchecked
            {
            for (int i = 0; i < pointCloud.Length; i++)
            {
                var point = pointCloud[i];
                if (//point.y < sliceHeightMillimeter &&
                         point.x + centerX < mapWidthMillimeter &&
                         point.z + centerY < mapLengthMillimeter &&
                         point.x + centerX > 0 &&
                         point.z + centerY > 0)
                {
                        if (point.y < top && point.y > bottom)
                            map[(point.x + centerX) , (point.z + centerY) ] = new Gray(255);
                        /*if (map[point.x + centerX, point.z + centerY].Intensity > point.y)
                    {
                            map[point.x + centerX, point.z + centerY] = new Gray(point.y);
                        }*/
                    }
                }
            }
        }

        public static XYZMm[] PointCloudImageToPointCloud(Image depthImage)
        {
            byte[] byteArray = depthImage.Memory.ToArray();
            int structSize = 2 * 3;
            int numStructs = byteArray.Length / structSize;
            XYZMm[] structArray = new XYZMm[numStructs];
            unsafe
            {
                fixed (XYZMm* pStruct = structArray)
                {
                    fixed (byte* pBytes = byteArray)
                    {
                        byte* pByte = pBytes;
                        XYZMm* pMyStruct = pStruct;

                        for (int i = 0; i < numStructs; i++)
                        {
                            *pMyStruct = *((XYZMm*)pByte);

                            pByte += structSize;
                            pMyStruct++;
                        }
                    }
                }
            }
            return structArray;
        }
    }
    public struct XYZMm
    {
        public Int16 x;
        public Int16 y;
        public Int16 z;
        public override string ToString()
        {
            return $"({x},{y},{z})";
        }
    }
}
