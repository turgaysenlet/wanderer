using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wanderer.Hardware.Motor
{
    [Serializable()]
    public class ServoPositionInfo
    {
        protected double acceleration = 0;
        public double Acceleration
        {
            get
            {
                return acceleration;
            }
            set
            {
                acceleration = value;
            }
        }
        protected double speed = 0;
        public double Speed
        {
            get
            {
                return speed;
            }
            set
            {
                speed = value;
            }
        }
        /// <summary>
        /// Fill this field to move the servo
        /// </summary>
        protected double target = 0;
        public double Target
        {
            get
            {
                return target;
            }
            set
            {
                target = value;
            }
        }
        /// <summary>
        /// Observe this field to see the servo position
        /// </summary>
        protected double position = 0;
        public double Position
        {
            get
            {
                return position;
            }
        }
        public ServoNoEnu ServoNo = ServoNoEnu.Unknown;

        // Explicit mutator for position field, which is actualy read-only
        public void ReportPosition(double p)
        {
            position = p;
        }
    }
}
