using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wanderer.Hardware.Motor
{
    public class ServoRange
    {
        // Min (Value), Max (Value), Center (Value), MinLimit (Angle, >=-180), MaxLimit (Angle, <=180), CenterStart (Value, Motor, <=Center), CenterEnd (Value, Motor, >=Center), IgnitionValue (Value, Motor)        
        private ServoNoEnu servoNo;
        public ServoNoEnu ServoNo
        {
            get { return servoNo; }
            set { servoNo = value; }
        }

        private ushort minValue;
        public ushort MinValue
        {
            get { return minValue; }
            set { minValue = value; }
        }

        private ushort maxValue;
        public ushort MaxValue
        {
            get { return maxValue; }
            set { maxValue = value; }
        }

        private ushort centerValue;
        public ushort CenterValue
        {
            get { return centerValue; }
            set { centerValue = value; }
        }

        private double minLimitAngle;
        public double MinLimitAngle
        {
            get { return minLimitAngle; }
            set { minLimitAngle = value; }
        }

        private double maxLimitAngle;
        public double MaxLimitAngle
        {
            get { return maxLimitAngle; }
            set { maxLimitAngle = value; }
        }

        private ushort centerLowValue;
        public ushort CenterLowValue
        {
            get { return centerLowValue; }
            set { centerLowValue = value; }
        }

        private ushort centerHighValue;
        public ushort CenterHighValue
        {
            get { return centerHighValue; }
            set { centerHighValue = value; }
        }

        private ushort ignitionStartValue;
        public ushort IgnitionStartValue
        {
            get { return ignitionStartValue; }
            set { ignitionStartValue = value; }
        }
    }
}
