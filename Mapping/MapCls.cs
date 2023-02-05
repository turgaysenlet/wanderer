using System.Drawing;
using System.Xml.Linq;
using SimplexNoise;

namespace Wanderer.Software.Mapping
{
    public class MapCls : EntityCls
    {
        public MapPointCls[,] MapData { get; private set; }
        public int XBoxes { get; private set; }
        public int YBoxes { get; private set; }
        public float XMeters { get; private set; }
        public float YMeters { get; private set; }
        public float ScaleBoxesPerMeter { get; private set; }
        public float DrawingScalePixelsPerMeter { get; set; } = 100.0f;
        public Pen Pen { get; set; } = new Pen(Color.Black, 0.1f);
        public Brush Brush { get; set; } = new SolidBrush(Color.Azure);
        public float CenterX { get; private set; }
        public float CenterY { get; private set; }
        public float ZScale { get; set; } = 0.002f;
        public float LocationX { get; set; }
        public float LocationY { get; set; }
        public Font font = new Font("Arial", 32);
        public Bitmap GenerateBitmap(int width, int height)
        {
            Bitmap bitmap = new Bitmap(width, height);
            Draw(Graphics.FromImage(bitmap));
            return bitmap;
        }     
        public MapCls(float xMeters, float yMeters, float scaleBoxesPerMeter)
        {            
            XMeters = xMeters;
            YMeters = yMeters;
            ScaleBoxesPerMeter = scaleBoxesPerMeter;
            XBoxes = (int)(xMeters / scaleBoxesPerMeter);
            YBoxes = (int)(yMeters / scaleBoxesPerMeter);
            Name = $"Map {XMeters}m x {YMeters}m";
            InitMap();
        }

        private void InitMap()
        {
            float[,] noise = Noise.Calc2D(XBoxes, YBoxes, ScaleBoxesPerMeter);
            MapData = new MapPointCls[XBoxes, YBoxes];
            for (int i = 0; i < XBoxes; i++)
            {
                for (int j = 0; j < YBoxes; j++)
                {
                    int n = (int)(noise[i, j]);
                    MapData[i, j] = new MapPointCls();
                    MapData[i, j].X = i * ScaleBoxesPerMeter;
                    MapData[i, j].Y = j * ScaleBoxesPerMeter;
                    MapData[i, j].Height = noise[i, j];
                    MapData[i, j].Color = Color.FromArgb(n, n, n);
                }
            }
        }

        public void Draw(Graphics graphic)
        {
            CenterX = XMeters / 2.0f;
            CenterY = XMeters / 2.0f;
            float locationXCentered = LocationX + CenterX;
            float locationYCentered = LocationY + CenterY;
            for (int indexX = 0; indexX < XBoxes; indexX++)
            {
                for (int indexY = 0; indexY < YBoxes; indexY++)
                {
                    float meterX = MapData[indexX, indexY].X;
                    float meterY = MapData[indexX, indexY].Y;
                    float pixelX = meterX * DrawingScalePixelsPerMeter;
                    float pixelY = meterY * DrawingScalePixelsPerMeter;
                    float w = 10f;
                    float h = 10f;
                    var color = MapData[indexX, indexY].Color;
                    if (InsideTheBox(0, 0, meterX, meterY))
                    {
                        color = Color.Green;
                    }
                    if (InsideTheBox(LocationX, LocationY, meterX, meterY))
                    {
                        color = Color.Red;
                    }
                    graphic.FillRectangle(new SolidBrush(color), new RectangleF(pixelX, graphic.VisibleClipBounds.Height - pixelY, w, h));
                    //graphic.FillRectangle(new SolidBrush(Color.Red), new RectangleF(x, graphic.VisibleClipBounds.Height - y, w, h));
                    graphic.DrawRectangle(Pen, pixelX, graphic.VisibleClipBounds.Height - pixelY, w, h);
                }
            }
            graphic.DrawString($"{LocationX.ToString("0.00")},{LocationY.ToString("0.00")}m", font, new SolidBrush(Color.Red),
                new Point((int)(locationXCentered * DrawingScalePixelsPerMeter), (int)(graphic.VisibleClipBounds.Height - locationYCentered * DrawingScalePixelsPerMeter)));
        }

        private bool InsideTheBox(float testX, float testY, float x, float y)
        {
            testX += CenterX;
            testY += CenterY;
            return testX >= (x - 1 * ScaleBoxesPerMeter) && testX < (x + 2 * ScaleBoxesPerMeter) && testY >= (y - 1 * ScaleBoxesPerMeter) && testY < (y + 1* ScaleBoxesPerMeter);
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
