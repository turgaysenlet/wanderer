using Emgu.CV.Ocl;
using Emgu.CV.Reg;
using Khronos;
using OpenGL;
using System;
using System.Numerics;
using System.Text;
using Wandarer.Software.ImageProcessing;
using static OpenGL.Gl;

namespace Wanderer
{
    public partial class MainForm : Form
    {
        public RealSense RealSense { get; set; } = new RealSense();
        public Wandarer.Software.Mapping.Map Map { get; set; } = new Wandarer.Software.Mapping.Map(10, 10, 0.1f);
        public Config Config = new Config();
        private VertexArray _VertexArray;
        public MainForm()
        {
            InitializeComponent();
        }

        private void StartCameraButton_Click(object sender, EventArgs e)
        {
            RealSense.Start();
            pictureBox1.Image = RealSense.D435.ColorBitmap;
            pictureBox2.Image = RealSense.D435.DepthColorBitmap;
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            propertyGrid1.SelectedObject = Config;
            Config.Values = "-5,10,-15,80,0,0";
            Config.Ortho = false;
        }

        private void MapPictureBox_Paint(object sender, PaintEventArgs e)
        {
            Map.Draw(e.Graphics);
        }
        private Programs _Program;
        private void RenderControl_CreateGL320()
        {
            _Program = new Programs(_VertexSourceGL, _FragmentSourceGL);
        }
        private readonly string[] _VertexSourceGL = {
            "#version 150 compatibility\n",
            "uniform mat4 uMVP;\n",
            "in vec3 aPosition;\n",
            "in vec3 aColor;\n",
            "out vec3 vColor;\n",
            "void main() {\n",
            "	gl_Position = uMVP * vec4(aPosition, 1.0);\n",
            "	vColor = aColor;\n",
            "}\n"
        };

        private readonly string[] _FragmentSourceGL = {
            "#version 150 compatibility\n",
            "in vec3 vColor;\n",
            "void main() {\n",
            "	gl_FragColor = vec4(vColor, 1.0);\n",
            "}\n"
        };

        private void RenderControl_ContextCreated(object sender, GlControlEventArgs e)
        {
            GlControl glControl = (GlControl)sender;

            // GL Debugging
            if (Gl.CurrentExtensions != null && Gl.CurrentExtensions.DebugOutput_ARB)
            {
                //Gl.DebugMessageCallback(GLDebugProc, IntPtr.Zero);
                //Gl.DebugMessageControl(DebugSource.DontCare, DebugType.DontCare, DebugSeverity.DontCare, 0, null, true);
            }

            // Allocate resources and/or setup GL states
            switch (Gl.CurrentVersion.Api)
            {
                case KhronosVersion.ApiGl:
                    if (Gl.CurrentVersion >= Gl.Version_320)
                        RenderControl_CreateGL320();
                    break;
                case KhronosVersion.ApiGles2:
                    break;
            }

            // Uses multisampling, if available
            if (Gl.CurrentVersion != null && Gl.CurrentVersion.Api == KhronosVersion.ApiGl && glControl.MultisampleBits > 0)
            {
                Gl.Enable(EnableCap.Multisample);
            }
        }

        private static void GLDebugProc(DebugSource source, DebugType type, uint id, DebugSeverity severity, int length, IntPtr message, IntPtr userParam)
        {
            string strMessage;

            // Decode message string
            unsafe
            {
                strMessage = Encoding.ASCII.GetString((byte*)message.ToPointer(), length);
            }

            Console.WriteLine($"{source}, {type}, {severity}: {strMessage}");
        }


        private void RenderControl_Render(object sender, GlControlEventArgs e)
        {
            // Common GL commands
            Control senderControl = (Control)sender;

            Gl.Viewport(0, 0, senderControl.ClientSize.Width, senderControl.ClientSize.Height);
            Gl.Clear(ClearBufferMask.ColorBufferBit);
            if (Gl.CurrentVersion.Api == KhronosVersion.ApiGl && Gl.CurrentVersion >= Gl.Version_320)
            {
                RenderControl_RenderGL320();
            }
        }
        private void RenderControl_RenderGL320()
        {
            /// <summary>
            /// Vertex position array.
            /// </summary>
            float[] _ArrayPosition = new float[] {
                0.0f, 0.0f, 0.0f,
                1.0f, 0.0f, 0.0f,
                1.0f, 1.0f, 0.0f,

                1.0f, 1.0f, 0.0f,
                0.0f, 1.0f, 0.0f,
                0.0f, 0.0f, 0.0f,
            };

            /// <summary>
            /// Vertex color array.
            /// </summary>
            float[] _ArrayColor = new float[] {
                1.0f, 0.0f, 0.0f,
                0.0f, 1.0f, 0.0f,
                0.0f, 0.0f, 1.0f,

                1.0f, 0.0f, 0.0f,
                0.0f, 1.0f, 0.0f,
                0.0f, 0.0f, 1.0f,
            };
            Map.PrepareGlArrays(out _ArrayPosition, out _ArrayColor);
            if (_VertexArray != null)
            {
                _VertexArray.Dispose();
                _VertexArray = null;
            }
            _VertexArray = new VertexArray(_Program, _ArrayPosition, _ArrayColor);
            // Compute the model-view-projection on CPU
            Matrix4x4f projection;
            if (Config.Ortho)
            {
                projection = Matrix4x4f.Translated(Config.X, Config.Y, Config.Z) * Matrix4x4f.Ortho2D(-20.0f, +20.0f, -20.0f, +20.0f) * Matrix4x4f.RotatedZ(Config.AngleZ) * Matrix4x4f.RotatedY(Config.AngleY) * Matrix4x4f.RotatedX(Config.AngleX);
            }
            else
            {
                projection = Matrix4x4f.Translated(Config.X, Config.Y, Config.Z) * Matrix4x4f.Perspective(90.0f, 1.0f, 0.1f, 100.0f) * Matrix4x4f.RotatedZ(Config.AngleZ) * Matrix4x4f.RotatedY(Config.AngleY) * Matrix4x4f.RotatedX(Config.AngleX);
            }
            Matrix4x4f modelview = Matrix4x4f.Identity; //Matrix4x4f.Translated(x, y, z) * Matrix4x4f.RotatedZ(_Angle);

            // Select the program for drawing
            Gl.UseProgram(_Program.ProgramName);
            // Set uniform state
            Gl.UniformMatrix4f(_Program.LocationMVP, 1, false, projection * modelview);
            // Use the vertex array
            Gl.BindVertexArray(_VertexArray.ArrayName);
            // Draw triangle
            // Note: vertex attributes are streamed from GPU memory
            Gl.DrawArrays(PrimitiveType.Triangles, 0, _ArrayPosition.Length / 3);
        }

        private float GetAspectRatio()
        {
            return (float)RenderControl.Width / (float)RenderControl.Height;
        }

        private void MapPictureBox_Resize(object sender, EventArgs e)
        {
            MapPictureBox.Refresh();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            RenderControl.Refresh();
            MapPictureBox.Refresh();
        }
    }
    public class Config
    {
        private float angleX;
        private float angleY;
        private float angleZ;
        public float X { get; set; } = -5;
        public float Y { get; set; } = -5;
        public float Z { get; set; } = 0;
        public float AngleX
        {
            get => angleX; set { angleX = value; }
        }
        public float AngleY
        {
            get => angleY; set
            {
                angleY = value;
            }
        }
        public float AngleZ
        {
            get => angleZ; set
            {
                angleZ = value;
            }
        }
        public bool Ortho { get; set; } = true;
        public string Values
        {
            get { return $"{X},{Y},{Z},{AngleX},{AngleY},{AngleZ}"; }
            set
            {
                try
                {
                    var vals = value.Split(",");
                    X = float.Parse(vals[0]);
                    Y = float.Parse(vals[1]);
                    Z = float.Parse(vals[2]);
                    AngleX = float.Parse(vals[3]);
                    AngleY = float.Parse(vals[4]);
                    AngleZ = float.Parse(vals[5]);
                }
                catch
                {
                }
            }
        }
    }
}
