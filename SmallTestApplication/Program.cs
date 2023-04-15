// See https://aka.ms/new-console-template for more information
using Emgu.CV.UI;
using Wanderer.Software.ImageProcessing;
Kinect kinect = new Kinect();
kinect.Start();
kinect.Capture();
kinect.map.Save(@"c:\temp\map.png");
kinect.Stop();