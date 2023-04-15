using Emgu.CV.UI;
using Wanderer.Software.ImageProcessing;
namespace SmallTestUiApplication
{
    public partial class Form1 : Form
    {
        Kinect kinect = new Kinect();
        public Form1()
        {
            InitializeComponent();
            kinect.Start();
            kinect.Capture();
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            kinect.Stop();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            kinect.Capture();
            kinect.map.Save(@"C:\temp\map.jpg");
            imageBox1.Image = kinect.map;
            imageBox1.Refresh();
            GC.Collect();
        }
    }
}