using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.Globalization;
using System.ComponentModel.Design.Serialization;

namespace Wanderer.Software.Mapping
{
    #region GPS
    [Serializable()]
    [ComVisible(true)]
    public class GpsInfoCls
    {
        /// <summary>
        /// Radians
        /// </summary>
        private double currentLatitude;
        /// <summary>
        /// Radians
        /// </summary>
        public double CurrentLatitude
        {
            get { return currentLatitude; }
            set { currentLatitude = value; }
        }
        /// <summary>
        /// Radians
        /// </summary>
        private double currentLongitude;
        /// <summary>
        /// Radians
        /// </summary>
        public double CurrentLongitude
        {
            get { return currentLongitude; }
            set { currentLongitude = value; }
        }
        /// <summary>
        /// Radians
        /// </summary>
        private double currentAltitude;
        /// <summary>
        /// Radians
        /// </summary>
        public double CurrentAltitude
        {
            get { return currentAltitude; }
            set { currentAltitude = value; }
        }
        /// <summary>
        /// String
        /// </summary>
        private string currentBearing;
        public string CurrentBearing
        {
            get { return currentBearing; }
            set { currentBearing = value; }
        }
        /// <summary>
        /// km/h
        /// </summary>
        private double currentSpeed;
        /// <summary>
        /// km/h
        /// </summary>
        public double CurrentSpeed
        {
            get { return currentSpeed; }
            set { currentSpeed = value; }
        }
        private DateTime currentTime;
        public DateTime CurrentTime
        {
            get { return currentTime; }
            set { currentTime = value; }
        }
        private float reliability = 0.0f;
        /// <summary>
        /// Reliability of GPS device as percent
        /// </summary>
        public float Reliability
        {
            get { return reliability; }
            set { reliability = value; }
        }
        private bool isConnected = false;
        public bool IsConnected
        {
            get { return isConnected; }
            set { isConnected = value; }
        }
        private double fixPrecision = 0;
        /// <summary>
        /// Fix precision in meters
        /// </summary>
        public double FixPrecision
        {
            get { return fixPrecision; }
            set { fixPrecision = value; }
        }
        private bool isFixed = false;
        public bool IsFixed
        {
            get { return isFixed; }
            set { isFixed = value; }
        }
        private bool isGps = false;
        public bool IsGps
        {
            get { return isGps; }
            set { isGps = value; }
        }
        private int numberOfSatellites = 0;
        public int NumberOfSatellites
        {
            get { return numberOfSatellites; }
            set { numberOfSatellites = value; }
        }
        public override string ToString()
        {
            string output = "";
            output += "Lat: " + this.currentLatitude.ToString("0.0000000");
            output += ", Long: " + this.currentLongitude.ToString("0.0000000");
            output += ", Alt: " + this.currentAltitude.ToString("0.000");
            if (isConnected == false)
            {
                output += " (Not Connected)";
            }
            else if (isFixed == false)
            {
                output += " (Not Fixed)";
            }
            else
            {
                output += " (Fix: " + this.fixPrecision.ToString("0.0") + "m)";
            }
            return output;
        }
    }
    #endregion

    #region Map
    [Serializable()]
    [ComVisible(true)]
    public class WayPointCls : ICloneable
    {
        private float pixelX = 0;
        public float PixelX
        {
            get { return pixelX; }
            set { pixelX = value; }
        }
        private float pixelY = 0;
        public float PixelY
        {
            get { return pixelY; }
            set { pixelY = value; }
        }
        public WayPointCls()
        {
        }
        public WayPointCls(float pixelX, float pixelY)
        {
            this.pixelX = pixelX;
            this.pixelY = pixelY;
        }
        #region ICloneable Members
        public object Clone()
        {
            return new WayPointCls(this.pixelX, this.pixelY);
        }

        #endregion
    }
    [Serializable()]
    [ComVisible(true)]
    public class WayPointPathCls : ICloneable
    {
        private List<WayPointCls> wayPoints = new List<WayPointCls>();
        public List<WayPointCls> WayPoints
        {
            get { return wayPoints; }
            set { wayPoints = value; }
        }
        public WayPointPathCls()
        {
        }
        public WayPointPathCls(WayPointCls[] wayPoints)
        {
            ClearWayPoints();
            AddWayPoints(wayPoints);
        }
        public WayPointPathCls(int[,] pointCoordinates)
        {
            ClearWayPoints();
            AddWayPoints(ConvertCoordinatesToWayPoints(pointCoordinates));
        }

        private void AddWayPoints(WayPointCls[] wayPoints)
        {
            this.wayPoints.AddRange(wayPoints);
        }

        private WayPointCls[] ConvertCoordinatesToWayPoints(int[,] pointCoordinates)
        {
            WayPointCls[] w = new WayPointCls[pointCoordinates.GetLength(0)];
            for (int i = 0; i < pointCoordinates.GetLength(0); i++)
            {
                w[i] = new WayPointCls(pointCoordinates[i, 0], pointCoordinates[i, 1]);
            }
            return w;
        }

        public void LoadWayPoints()
        {
        }

        public void ClearWayPoints()
        {
            wayPoints.Clear();
        }

        #region ICloneable Members
        public object Clone()
        {
            return new WayPointPathCls(this.wayPoints.ToArray());
        }
        #endregion
    }
    #endregion

    [Serializable()]
    [ComVisible(true)]
    public class Pose3DCls : ICloneable
    {
        private double x;
        private double y;
        private double z;
        public double X { get { return x; } set { x = value; } }
        public double Y { get { return y; } set { y = value; } }
        public double Z { get { return z; } set { z = value; } }
        private double yaw;
        private double pitch;
        private double roll;
        public double Yaw { get { return yaw; } set { yaw = value; } }
        public double Pitch { get { return pitch; } set { pitch = value; } }
        public double Roll { get { return roll; } set { roll = value; } }
        public override string ToString()
        {
            return "{X: " + X.ToString() + ", Y: " + Y.ToString() + ", Z: " + Z.ToString() + "}" + "{Yaw: " + yaw.ToString() + ", Pitch: " + pitch.ToString() + ", Roll: " + Roll.ToString() + "}";
        }
        public Pose3DCls(double x, double y, double z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
            this.yaw = 0;
            this.pitch = 0;
            this.roll = 0;
        }
        public Pose3DCls(double x, double y, double z, double yaw, double pitch, double roll)
        {
            this.x = x;
            this.y = y;
            this.z = z;
            this.yaw = yaw;
            this.pitch = pitch;
            this.roll = roll;
        }
        public Pose3DCls()
        {
        }

        #region ICloneable Members
        public object Clone()
        {
            return new Pose3DCls(x, y, z, yaw, pitch, roll);
        }
        #endregion
    }

    [Serializable()]
    [ComVisible(true)]
    [TypeConverter(typeof(PointConverter3DCls))]
    public struct Point3D
    {
        private float x;
        private float y;
        private float z;
        public float X { get { return x; } set { x = value; } }
        public float Y { get { return y; } set { y = value; } }
        public float Z { get { return z; } set { z = value; } }
        public override string ToString()
        {
            return "{X: " + X.ToString() + ", Y: " + Y.ToString() + ", Z: " + Z.ToString() + "}";
        }
        public Point3D(float x, float y, float z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }
    }
    public class PointConverter3DCls : TypeConverter
    {
        // Summary:
        //     Initializes a new instance of the System.Drawing.PointConverter class.
        public PointConverter3DCls()
        {
        }

        // Summary:
        //     Determines if this converter can convert an object in the given source type
        //     to the native type of the converter.
        //
        // Parameters:
        //   context:
        //     A formatter context. This object can be used to get additional information
        //     about the environment this converter is being called from. This may be null,
        //     so you should always check. Also, properties on the context object may also
        //     return null.
        //
        //   sourceType:
        //     The type you want to convert from.
        //
        // Returns:
        //     true if this object can perform the conversion; otherwise, false.
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType) { return ((sourceType == typeof(string)) || base.CanConvertFrom(context, sourceType)); }        //
                                                                                                                                                                                             // Summary:
                                                                                                                                                                                             //     Gets a value indicating whether this converter can convert an object to the
                                                                                                                                                                                             //     given destination type using the context.
                                                                                                                                                                                             //
                                                                                                                                                                                             // Parameters:
                                                                                                                                                                                             //   context:
                                                                                                                                                                                             //     An System.ComponentModel.ITypeDescriptorContext object that provides a format
                                                                                                                                                                                             //     context.
                                                                                                                                                                                             //
                                                                                                                                                                                             //   destinationType:
                                                                                                                                                                                             //     A System.Type object that represents the type you want to convert to.
                                                                                                                                                                                             //
                                                                                                                                                                                             // Returns:
                                                                                                                                                                                             //     true if this converter can perform the conversion; otherwise, false.
        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            return ((destinationType == typeof(InstanceDescriptor)) || base.CanConvertTo(context, destinationType));
        }
        //
        // Summary:
        //     Converts the specified object to a System.Drawing.Point3D object.
        //
        // Parameters:
        //   context:
        //     A formatter context. This object can be used to get additional information
        //     about the environment this converter is being called from. This may be null,
        //     so you should always check. Also, properties on the context object may also
        //     return null.
        //
        //   culture:
        //     An object that contains culture specific information, such as the language,
        //     calendar, and cultural conventions associated with a specific culture. It
        //     is based on the RFC 1766 standard.
        //
        //   value:
        //     The object to convert.
        //
        // Returns:
        //     The converted object.
        //
        // Exceptions:
        //   System.NotSupportedException:
        //     The conversion cannot be completed.
        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            string str = value as string;
            if (str == null)
            {
                return base.ConvertFrom(context, culture, value);
            }
            string str2 = str.Trim();
            if (str2.Length == 0)
            {
                return null;
            }
            if (culture == null)
            {
                culture = CultureInfo.CurrentCulture;
            }
            char ch = culture.TextInfo.ListSeparator[0];
            string[] strArray = str2.Split(new char[] { ch });
            float[] numArray = new float[strArray.Length];
            TypeConverter converter = TypeDescriptor.GetConverter(typeof(float));
            for (int i = 0; i < numArray.Length; i++)
            {
                numArray[i] = (float)converter.ConvertFromString(context, culture, strArray[i]);
            }
            if (numArray.Length != 3)
            {
                return new Point3D();//throw new ArgumentException(SR.GetString("TextParseFailedFormat", new object[] { str2, "x, y, z" }));
            }
            return new Point3D(numArray[0], numArray[1], numArray[2]);
        }
        //
        // Summary:
        //     Converts the specified object to the specified type.
        //
        // Parameters:
        //   context:
        //     A formatter context. This object can be used to get additional information
        //     about the environment this converter is being called from. This may be null,
        //     so you should always check. Also, properties on the context object may also
        //     return null.
        //
        //   culture:
        //     An object that contains culture specific information, such as the language,
        //     calendar, and cultural conventions associated with a specific culture. It
        //     is based on the RFC 1766 standard.
        //
        //   value:
        //     The object to convert.
        //
        //   destinationType:
        //     The type to convert the object to.
        //
        // Returns:
        //     The converted object.
        //
        // Exceptions:
        //   System.NotSupportedException:
        //     The conversion cannot be completed.
        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if (destinationType == null)
            {
                throw new ArgumentNullException("destinationType");
            }
            if (value is Point3D)
            {
                if (destinationType == typeof(string))
                {
                    Point3D point = (Point3D)value;
                    if (culture == null)
                    {
                        culture = CultureInfo.CurrentCulture;
                    }
                    string separator = culture.TextInfo.ListSeparator + " ";
                    TypeConverter converter = TypeDescriptor.GetConverter(typeof(float));
                    string[] strArray = new string[3];
                    int num = 0;
                    strArray[num++] = point.X.ToString("0.000");// converter.ConvertToString(context, culture, point.X);
                    strArray[num++] = point.Y.ToString("0.000");// converter.ConvertToString(context, culture, point.Y);
                    strArray[num++] = point.Z.ToString("0.000");// converter.ConvertToString(context, culture, point.Z);
                    return string.Join(separator, strArray);
                }
                if (destinationType == typeof(InstanceDescriptor))
                {
                    Point3D point2 = (Point3D)value;
                    System.Reflection.ConstructorInfo constructor = typeof(Point3D).GetConstructor(new Type[] { typeof(float), typeof(float), typeof(float) });
                    if (constructor != null)
                    {
                        return new InstanceDescriptor(constructor, new object[] { point2.X, point2.Y, point2.Z });
                    }
                }
            }
            return base.ConvertTo(context, culture, value, destinationType);
        }
        //
        // Summary:
        //     Creates an instance of this type given a set of property values for the object.
        //
        // Parameters:
        //   context:
        //     A type descriptor through which additional context can be provided.
        //
        //   propertyValues:
        //     A dictionary of new property values. The dictionary contains a series of
        //     name-value pairs, one for each property returned from System.Drawing.PointConverter.GetProperties(System.ComponentModel.ITypeDescriptorContext,System.Object,System.Attribute[]).
        //
        // Returns:
        //     The newly created object, or null if the object could not be created. The
        //     default implementation returns null.
        public override object CreateInstance(ITypeDescriptorContext context, IDictionary propertyValues)
        {
            if (propertyValues == null)
            {
                throw new ArgumentNullException("propertyValues");
            }
            object obj2 = propertyValues["X"];
            object obj3 = propertyValues["Y"];
            object obj4 = propertyValues["Z"];
            if (((obj2 == null) || (obj3 == null)) || (!(obj2 is float) || !(obj3 is float) || !(obj4 is float)))
            {
                return new Point3D();//throw new ArgumentException(SR.GetString("PropertyValueInvalidEntry"));
            }
            return new Point3D((float)obj2, (float)obj3, (float)obj4);
        }
        //
        // Summary:
        //     Determines if changing a value on this object should require a call to System.Drawing.PointConverter.CreateInstance(System.ComponentModel.ITypeDescriptorContext,System.Collections.IDictionary)
        //     to create a new value.
        //
        // Parameters:
        //   context:
        //     A System.ComponentModel.TypeDescriptor through which additional context can
        //     be provided.
        //
        // Returns:
        //     true if the System.Drawing.PointConverter.CreateInstance(System.ComponentModel.ITypeDescriptorContext,System.Collections.IDictionary)
        //     method should be called when a change is made to one or more properties of
        //     this object; otherwise, false.
        public override bool GetCreateInstanceSupported(ITypeDescriptorContext context)
        {
            return true;
        }
        //
        // Summary:
        //     Retrieves the set of properties for this type. By default, a type does not
        //     return any properties.
        //
        // Parameters:
        //   context:
        //     A type descriptor through which additional context can be provided.
        //
        //   value:
        //     The value of the object to get the properties for.
        //
        //   attributes:
        //     An array of System.Attribute objects that describe the properties.
        //
        // Returns:
        //     The set of properties that are exposed for this data type. If no properties
        //     are exposed, this method might return null. The default implementation always
        //     returns null.
        public override PropertyDescriptorCollection GetProperties(ITypeDescriptorContext context, object value, Attribute[] attributes)
        {
            return TypeDescriptor.GetProperties(typeof(Point3D), attributes).Sort(new string[] { "X", "Y", "Z" });
        }
        //
        // Summary:
        //     Determines if this object supports properties. By default, this is false.
        //
        // Parameters:
        //   context:
        //     A System.ComponentModel.TypeDescriptor through which additional context can
        //     be provided.
        //
        // Returns:
        //     true if System.Drawing.PointConverter.GetProperties(System.ComponentModel.ITypeDescriptorContext,System.Object,System.Attribute[])
        //     should be called to find the properties of this object; otherwise, false.
        public override bool GetPropertiesSupported(ITypeDescriptorContext context)
        {
            return true;
        }
    }
}