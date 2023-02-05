using Emgu.CV.Structure;
using Intel.RealSense.Math;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wanderer.Software.Common
{
    /**
   * Vertex class that also support recording RGB color, pixel coordinates and point type.
   * This is a class and not a struct like Vertex, so it has pros and cons.
   * Pro: It is always passed by reference, so its fields can be filled incrementally over multiple methods.
   * Con: It takes very long to do verticesAll[k] = new Vertex(); (100ms) compared to vertices[k] = new Vertex (10ms).
   * Pro: The slowness in array assignement is more than compansated since arrays are passed and reused, and not copied all the time.
   * */
    public class VertexRgb
    {
        public float x;
        public float y;
        public float z;
        public byte r;
        public byte g;
        public byte b;
        public Color color;
        public Rgb rgb;
        public float X;
        public float Y;
        public int index;
        public Vertex originalVertex;
        public PointTypeEnu pointType = PointTypeEnu.Unclassified;
        public VertexRgb() : this(0, 0, 0, 0, 255, 255, 255, 0, 0)
        {
        }
        public VertexRgb(int index, float x, float y, float z) : this(index, x, y, z, 255, 255, 255, 0, 0)
        {
        }
        public VertexRgb(int index, float x, float y, float z, byte r, byte g, byte b) : this(index, x, y, z, r, g, b, 0, 0)
        {
        }

        public VertexRgb(int index, float x, float y, float z, byte r, byte g, byte b, float X, float Y)
        {
            this.index = index;
            this.x = x;
            this.y = y;
            this.z = z;
            this.r = r;
            this.g = g;
            this.b = b;
            RecalculateColor();
            this.X = X;
            this.Y = Y;
            originalVertex = new Vertex();
        }
        public void RecalculateColor()
        {
            color = Color.FromArgb(r, g, b);
            rgb = new Rgb(r, g, b);
        }

        public static VertexRgb FromVertex(int index, Vertex vertex)
        {
            return new VertexRgb(index, vertex.x, vertex.y, vertex.z);
        }

        public Vertex ToVertex()
        {
            Vertex vertex = new Vertex();
            vertex.x = x;
            vertex.y = y;
            vertex.z = z;
            return vertex;
        }

        public override string ToString()
        {
            if (X == 0 && Y == 0)
            {
                return "(" + index + ") " + "[" + x.ToString("0.00") + ", " + y.ToString("0.00") + ", " + z.ToString("0.00") + "] (" + (int)r + ", " + (int)g + ", " + (int)b + ")";
            }
            else
            {
                return "(" + index + ") " + "[" + x.ToString("0.00") + ", " + y.ToString("0.00") + ", " + z.ToString("0.00") + "] (" + (int)r + ", " + (int)g + ", " + (int)b + ") [" + (int)X + ", " + (int)Y + "]";
            }
        }

        internal VertexRgb Clone()
        {
            VertexRgb clone = new VertexRgb(index, x, y, z, r, g, b, X, Y);
            clone.originalVertex = originalVertex;
            return clone;
        }

        public enum PointTypeEnu
        {
            // Uninitialized
            Unclassified = 1,
            // Initialized, but not yet classified
            Initialized = 2,
            // Initial plane point, based on its z corrdinate only
            PlaneInitial = 3,
            // Better plane point, that fits the plane equation
            PlaneInlier = 4,
            // Best plane point, that fits the plane equation and fits the average color of the inlier points
            PlaneColorMatched = 5,
            // Obstcacle that is above the plane by some distance (10cm) and tall up to some more distance (50cm)
            Obstacle = 6,
            // Non-obstacle, non-plane. Either between obstacle and plane (10cm region) or above the obstacle, e.g. ceiling, doorway, etc.
            Other = 7
        }
    }
}
