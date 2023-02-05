using Pololu.UsbWrapper;
using Pololu.Usc;
using SharedTypes.Control;

namespace Control.Motor.ServoMotor
{
    public class ServoMotorControllerCls
    {
        private Usc servoController = null;
        public Usc ServoController
        {
            get { return servoController; }
        }

        public ServoMotorControllerCls()
        {
            //UpdateTimer.Tick += new EventHandler(UpdateTimer_Tick);

        }

        ~ServoMotorControllerCls()
        {
            Disconnect();       
        }

        public bool Connect()
        { 
            TryToReconnect();
            //UpdateTimer.Start();

            //UscSettings s = servoController.getUscSettings();
            //s.servosAvailable = 63;
            //servoController.setUscSettings(s,false);
            return (servoController != null);
        }

        public void Disconnect()
        {
            TryToDisconnect();
            //UpdateTimer.Stop();
        }

        //private string serialNumber = "00004219";
        private string serialNumber = "00125365";
        public string SerialNumber
        {
            get { return serialNumber; }
        }

        /// <summary>
        /// Connects to the device if it is found in the device list.
        /// </summary>
        private void TryToReconnect()
        {
            foreach (DeviceListItem d in Usc.getConnectedDevices())
            {
                if (d.serialNumber == serialNumber)
                {
                    try
                    {
                        servoController = new Usc(d);
                    }
                    catch
                    {
                        servoController = null;
                    }
                    // Log("Connected to #" + serialNumber + ".");

                    return;
                }
            }
        }

        private void TryToDisconnect()
        {
            if (servoController == null)
            {
                // Log("Connecting stopped.");
                return;
            }

            try
            {
                // Log("Disconnecting...");
                servoController.disconnect();
            }
            catch (Exception e)
            {
                // Log(e);
                // Log("Failed to disconnect cleanly.");
            }
            finally
            {
                // do this no matter what
                servoController = null;
                // Log("Disconnected from #" + serialNumber + ".");
            }
        }

        /// <summary>
        /// Gets servo positions and status from the controller
        /// </summary>
        public ServoPositionInfoCls[] GetPosition()
        {
            ServoStatus[] servos;
            if (servoController != null)
            {
                servoController.getVariables(out servos);
                return GetServoAngleStatus(servos);
            }
            return null;
        }

        /// <summary>
        /// Gets servo positions and status from the controller.
        /// Also gets stack variables.
        /// </summary>
        public ServoPositionInfoCls[] GetPosition(out MaestroVariables variables, out short[] stack, out ushort[] callStack)
        {
            ServoStatus[] servos;
            servoController.getVariables(out variables, out stack, out callStack, out servos);

            return GetServoAngleStatus(servos);
        }
        private ServoPositionInfoCls[] GetServoAngleStatus(ServoStatus[] servos)
        {
            if (servos == null)
            {
                return new ServoPositionInfoCls[0];
            }

            ServoPositionInfoCls[] servoAngleStatus = new ServoPositionInfoCls[servos.Length];
            for (int i = 0; i < servos.Length; i++)
            {
                servoAngleStatus[i] = new ServoPositionInfoCls();
                servoAngleStatus[i].Acceleration = servos[i].acceleration;
                servoAngleStatus[i].ReportPosition(servos[i].position);
                servoAngleStatus[i].Speed = servos[i].speed;
                servoAngleStatus[i].Target = servos[i].target;
#warning short to double angle conversion needed here
            }

            return servoAngleStatus;
        }

        #region updating

        /// <summary>
        /// This function will be called once every 100 ms to do an update.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void UpdateTimer_Tick(object sender, EventArgs e)
        {
            //Not used
            if (servoController == null)
            {
                // Display a message in the position box

                // Try connecting to a device.
                try
                {
                    //TryToReconnect();
                }
                catch (Exception e2)
                {
                    // Log(e2);
                    // Log("Failed connecting to #" + serialNumber + ".");
                    //servoController = null;
                }
            }
            else
            {
                // Update the GUI and the device.
                try
                {
                    //GetPosition();
                }
                catch (Exception e2)
                {
                    // If any exception occurs, log it, set usc to null, and keep trying..
                    // Log(e2);
                    // Log("Disconnected from #" + serialNumber + ".");
                    servoController = null;
                }
            }
        }

        #endregion

        public bool SetServoPosition(ServoNoEnu servoNo, double angle, double speed, double acceleration)
        {
            if (servoController != null && servoController.servoCount > (byte)servoNo)
            {
                /*servoController.setAcceleration((byte)servoNo, (byte)acceleration);
                servoController.setSpeed((byte)servoNo, (ushort)speed);
                ushort angleValue = ServoAngleCls.FindServoAngleValue(servoNo, angle);
                servoController.setTarget((byte)servoNo, angleValue);
                 */

                try
                {
                    servoController.setAcceleration((byte)servoNo, (byte)acceleration);
                }
                catch (Exception)
                {
                }
                try
                {
                    servoController.setSpeed((byte)servoNo, (ushort)speed);
                }
                catch (Exception)
                {
                }
                try
                {
                    servoController.setTarget((byte)servoNo, (ushort)angle);
                }
                catch (Exception)
                {
                }

                return true;
            }
            return false;
        }

        private static ServoRangeCls[] servoRanges = new ServoRangeCls[6];
        public static ServoRangeCls[] ServoRanges
        {
            get
            {
                return servoRanges;
            }
        }
    }
    public class ServoRangeCls
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