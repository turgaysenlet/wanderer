using System.Drawing;
using System.Linq;
using System.Xml.Linq;
using Emgu.CV.Structure;
using Emgu.CV;
using SimplexNoise;
using Wanderer.Software.Common;
using Intel.RealSense;

namespace Wanderer.Software.Mapping
{
    public class MapCls : EntityCls
    {
        public MapPointCls[,] MapData { get; private set; }
        public int XBoxes { get; private set; }
        public int YBoxes { get; private set; }
        public float XMeters { get; private set; }
        public float YMeters { get; private set; }
        public float ScaleMetersPerBox { get; private set; }
        public float DrawingScalePixelsPerMeter { get; set; } = 100.0f;
        public Pen Pen { get; set; } = new Pen(Color.Black, 0.1f);
        public Brush Brush { get; set; } = new SolidBrush(Color.Azure);
        public float CenterX { get; private set; }
        public float CenterY { get; private set; }
        public float ZScale { get; set; } = 0.002f;
        public float LocationX { get; set; }
        public float LocationY { get; set; }
        public Font font = new Font("Arial", 32);
        public List<Tuple<DateTime, double[]>> Poses = new();
        public Bitmap GenerateBitmap(int width, int height)
        {
            Bitmap bitmap = new Bitmap(width, height);
            Draw(Graphics.FromImage(bitmap));
            return bitmap;
        }     
        public MapCls(float xMeters, float yMeters, float scaleMetersPerBox)
        {            
            XMeters = xMeters;
            YMeters = yMeters;
            ScaleMetersPerBox = scaleMetersPerBox;
            XBoxes = (int)(xMeters / scaleMetersPerBox);
            YBoxes = (int)(yMeters / scaleMetersPerBox);
            Name = $"Map {XMeters}m x {YMeters}m";
            InitMap();
        }

        private void InitMap()
        {
            float[,] noise = Noise.Calc2D(XBoxes, YBoxes, ScaleMetersPerBox);
            MapData = new MapPointCls[XBoxes, YBoxes];
            for (int i = 0; i < XBoxes; i++)
            {
                for (int j = 0; j < YBoxes; j++)
                {
                    int n = (int)(noise[i, j]);
                    MapData[i, j] = new MapPointCls();
                    MapData[i, j].X = i * ScaleMetersPerBox;
                    MapData[i, j].Y = j * ScaleMetersPerBox;
                    MapData[i, j].Height = 0; //noise[i, j];
                    MapData[i, j].Color = Color.Pink; //Color.FromArgb(n, n, n);
                }
            }
            CenterX = XMeters / 2.0f;
            CenterY = YMeters / 2.0f;
        }

        public void Draw(Graphics graphic)
        {
            lock (this)
            {
                float locationXCentered = LocationX + CenterX;
                float locationYCentered = LocationY + CenterY;
                //float H = graphic.VisibleClipBounds.Height;
                float w = 10f;
                float h = 10f;
                for (int indexX = 0; indexX < XBoxes; indexX++)
                {
                    for (int indexY = 0; indexY < YBoxes; indexY++)
                    {
                        float meterX = MapData[indexX, indexY].X;
                        float meterY = MapData[indexX, indexY].Y;
                        float pixelX = meterX * DrawingScalePixelsPerMeter;
                        float pixelY = meterY * DrawingScalePixelsPerMeter;
                        var color = MapData[indexX, indexY].Color;
                        if (InsideTheBox(0, 0, meterX, meterY))
                        {
                            color = Color.Green;
                        }
                        if (InsideTheBox(LocationX, LocationY, meterX, meterY))
                        {
                            color = Color.FromArgb(255, 90, 0);
                        }
                        graphic.FillRectangle(new SolidBrush(color), new RectangleF(pixelX, pixelY, w, h));
                        //graphic.FillRectangle(new SolidBrush(Color.Red), new RectangleF(x, y, w, h));
                        graphic.DrawRectangle(Pen, pixelX,  pixelY, w, h);
                    }
                }
                //for (int k = 0; k < Poses.Count; k++)
                //{
                //    var pose = Poses[k];
                //    var pixelX = (CenterX + (float)pose.Item2[0]) * DrawingScalePixelsPerMeter;
                //    var pixelY = (CenterY + (float)pose.Item2[1]) * DrawingScalePixelsPerMeter;
                //    graphic.FillRectangle(new SolidBrush(Color.Red), new RectangleF(pixelX, pixelY, w, h));
                //}
                var points = Poses.Select(pose => new PointF((CenterX + (float)pose.Item2[0]) * DrawingScalePixelsPerMeter, (CenterY + (float)pose.Item2[1]) * DrawingScalePixelsPerMeter)).ToArray();
                graphic.DrawLines(Pens.Red, points);
                var p = new Pen(Color.FromArgb(128, Color.Blue), 2);
                graphic.DrawLine(p,
                    new PointF((CenterX + LocationX) * DrawingScalePixelsPerMeter, (CenterY + LocationY) * DrawingScalePixelsPerMeter),
                    new PointF((CenterX + LocationX - 2.9f) * DrawingScalePixelsPerMeter, (CenterY + LocationY + 3.4f) * DrawingScalePixelsPerMeter));
                graphic.DrawLine(p,
                    new PointF((CenterX + LocationX) * DrawingScalePixelsPerMeter, (CenterY + LocationY) * DrawingScalePixelsPerMeter),
                    new PointF((CenterX + LocationX + 2.9f) * DrawingScalePixelsPerMeter, (CenterY + LocationY + 3.4f) * DrawingScalePixelsPerMeter));
                graphic.DrawString($"{LocationX.ToString("0.00")},{LocationY.ToString("0.00")}m", font, new SolidBrush(Color.FromArgb(255, 90, 0)),
                    new Point((int)(locationXCentered * DrawingScalePixelsPerMeter), (int)(locationYCentered * DrawingScalePixelsPerMeter)));
            }
        }
        public void CalculateOccupancyMap(VertexRgb[] vertices)
        {
            for (int k = 0; k < vertices.Length; k++)
            {
                VertexRgb vertex = vertices[k];
                float x = vertex.x + LocationX;
                float y = vertex.y + LocationY;
                int X = (int)((x + CenterX) / ScaleMetersPerBox);
                int Y = (int)((y + CenterY) / ScaleMetersPerBox);
                if (X >= 0 && X < XBoxes && Y >= 0 && Y < YBoxes)
                {
                    var height = vertex.z * 100.0f;
                    if (height < 0)
                    {
                        height = 0;
                    }
                    else
                    {
                        if (height > 255.0f)
                        {
                            height = 255.0f;
                        }
                        if (height > 60)
                        {
                            MapData[X, Y].Color = Color.Yellow;
                        }
                        else
                        {
                            MapData[X, Y].Color = Color.FromArgb((int)height, (int)height, (int)height);
                        }
                        MapData[X, Y].Height = height;
                    }
                    //if (vertex.pointType == VertexRgb.PointTypeEnu.Obstacle)
                    //{
                    //    MapData[X, Y].Height = 1.0f;
                    //}
                    //else // if (!MapData[X, Y].Equals(red))
                    //{
                    //    MapData[X, Y].Height = 0.0f;
                    //}
                }
            }
        }
        private bool InsideTheBox(float testX, float testY, float x, float y)
        {
            testX += CenterX;
            testY += CenterY;
            return testX >= (x - 1 * ScaleMetersPerBox) && testX < (x + 1 * ScaleMetersPerBox) && testY >= (y - 1 * ScaleMetersPerBox) && testY < (y + 1 * ScaleMetersPerBox);
        }

        public void PrepareGlArrays(out float[] arrayPosition, out float[] arrayColor)
        {
            arrayPosition = new float[3 * XBoxes * YBoxes * 3 * 2];
            arrayColor = new float[3 * XBoxes * YBoxes * 3 * 2];
            int t = 0;
            int q = 0;
            float w2 = 0.20f / 2.0f;
            float h2 = 0.20f / 2.0f;
            float z2 = 10f / 2.0f;
            for (int i = 0; i < XBoxes; i++)
            {
                for (int j = 0; j < YBoxes; j++)
                {
                    float x = MapData[i, j].X;
                    float y = MapData[i, j].Y;
                    float z = MapData[i, j].Height * ZScale; // MapData[i, j].Height;
                    //arrayPosition[3 * k + 0] = x - w2;
                    //arrayPosition[3 * k + 1] = y - h2;
                    //arrayPosition[3 * k + 2] = z;
                    //arrayPosition[3 * k + 3] = x + w2;
                    //arrayPosition[3 * k + 4] = y - h2;
                    //arrayPosition[3 * k + 5] = z;
                    //arrayPosition[3 * k + 6] = x + w2;
                    //arrayPosition[3 * k + 7] = y + h2;
                    //arrayPosition[3 * k + 8] = z;

                    //arrayPosition[3 * k + 9] = x + w2;
                    //arrayPosition[3 * k + 10] = y + h2;
                    //arrayPosition[3 * k + 11] = z;
                    //arrayPosition[3 * k + 12] = x - w2;
                    //arrayPosition[3 * k + 13] = y + h2;
                    //arrayPosition[3 * k + 14] = z;
                    //arrayPosition[3 * k + 15] = x - w2;
                    //arrayPosition[3 * k + 16] = y - h2;
                    //arrayPosition[3 * k + 17] = z;
                    arrayPosition[t++] = x - w2;
                    arrayPosition[t++] = y - h2;
                    arrayPosition[t++] = z;
                    arrayPosition[t++] = x + w2;
                    arrayPosition[t++] = y - h2;
                    arrayPosition[t++] = z + z2;
                    arrayPosition[t++] = x + w2;
                    arrayPosition[t++] = y + h2;
                    arrayPosition[t++] = z - z2;
                    arrayPosition[t++] = x + w2;
                    arrayPosition[t++] = y + h2;
                    arrayPosition[t++] = z;
                    arrayPosition[t++] = x - w2;
                    arrayPosition[t++] = y + h2;
                    arrayPosition[t++] = z + z2;
                    arrayPosition[t++] = x - w2;
                    arrayPosition[t++] = y - h2;
                    arrayPosition[t++] = z;

                    float r = MapData[i, j].Color.R / 255.0f;
                    float g = MapData[i, j].Color.G / 255.0f;
                    float b = MapData[i, j].Color.B / 255.0f;
                    arrayColor[q++] = r;
                    arrayColor[q++] = g;
                    arrayColor[q++] = b;
                    arrayColor[q++] = r;
                    arrayColor[q++] = g;
                    arrayColor[q++] = b;
                    arrayColor[q++] = r;
                    arrayColor[q++] = g;
                    arrayColor[q++] = b;
                    arrayColor[q++] = r;
                    arrayColor[q++] = g;
                    arrayColor[q++] = b;
                    arrayColor[q++] = r;
                    arrayColor[q++] = g;
                    arrayColor[q++] = b;
                    arrayColor[q++] = r;
                    arrayColor[q++] = g;
                    arrayColor[q++] = b;
                }
            }
        }
        public override string ToString()
        {
            return $"{base.ToString()} - Entity {EntityNo}";
        }
    }
}
