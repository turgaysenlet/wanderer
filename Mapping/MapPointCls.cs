using System.Drawing;

namespace Wanderer.Software.Mapping
{
    public struct MapPointCls
    {
        public float X;
        public float Y;
        public float Height;
        public float Occupancy;
        public Color Color;
        public override string ToString()
        {
            return $"[{X},{Y},{Height}] - [{Color.R},{Color.G},{Color.B}]";
        }
    }
}