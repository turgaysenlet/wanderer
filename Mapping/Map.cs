using System.Drawing;
using SimplexNoise;

namespace Wandarer.Software.Mapping
{
    public class Map
    {
        public MapPoint[,] MapData { get; private set; }
        public int XBoxes { get; private set; }
        public int YBoxes { get; private set; }
        public float XMeters { get; private set; }
        public float YMeters { get; private set; }
        public float Scale { get; private set; }
        public Pen Pen { get; set; } = new Pen(Color.Black, 0.1f);
        public Brush Brush { get; set; } = new SolidBrush(Color.Azure);
        public float CenterX { get; private set; }
        public float CenterY { get; private set; }
        public float ZScale { get; set; } = 0.002f;

        public Map(float xMeters, float yMeters, float scale)
        {
            XMeters = xMeters;
            YMeters = yMeters;
            Scale = scale;
            XBoxes = (int)(xMeters / scale);
            YBoxes = (int)(yMeters / scale);
            InitMap();
        }

        private void InitMap()
        {
            float[,] noise = Noise.Calc2D(XBoxes, YBoxes, Scale);
            MapData = new MapPoint[XBoxes, YBoxes];
            for (int i = 0; i < XBoxes; i++)
            {
                for (int j = 0; j < YBoxes; j++)
                {
                    int n = (int)(noise[i, j]);
                    MapData[i, j] = new MapPoint();
                    MapData[i, j].X = i * Scale;
                    MapData[i, j].Y = j * Scale;
                    MapData[i, j].Height = noise[i, j];
                    if (i == 5)
                    {
                        MapData[i, j].Color = Color.Red;
                    }
                    else
                    {
                        if (j == 5)
                        {
                            MapData[i, j].Color = Color.Green;
                        }
                        else
                        {
                            MapData[i, j].Color = Color.FromArgb(n, n, n);
                        }
                    }
                }
            }
        }

        public void Draw(Graphics graphic)
        {
            for (int i = 0; i < XBoxes; i++)
            {
                for (int j = 0; j < YBoxes; j++)
                {
                    float x = MapData[i, j].X * 100;
                    float y = MapData[i, j].Y * 100;
                    float w = 10f;
                    float h = 10f;
                    graphic.FillRectangle(new SolidBrush(MapData[i, j].Color), new RectangleF(x, graphic.ClipBounds.Height - y, w, h));
                    graphic.DrawRectangle(Pen, x, graphic.ClipBounds.Height - y, w, h);
                }
            }
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
    }
}
