using Pololu.Usc;
using Wanderer.Hardware;
using Pololu.UsbWrapper;

namespace Wanderer.Hardware.Motor
{
    public class ServoController : Device
    {
        private Usc usbServoController = null;
        public Usc UsbServoController
        {
            get { return usbServoController; }
        }
        private string serialNumber = "00125365";
        public string SerialNumber
        {
            get { return serialNumber; }
        }
        private static ServoRange[] servoRanges = new ServoRange[6];
        public static ServoRange[] ServoRanges
        {
            get
            {
                return servoRanges;
            }
        }
        ~ServoController()
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
            return (usbServoController != null);
        }

        public void Disconnect()
        {
            TryToDisconnect();
            //UpdateTimer.Stop();
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
                        usbServoController = new Usc(d);
                    }
                    catch
                    {
                        usbServoController = null;
                    }
                    // Log("Connected to #" + serialNumber + ".");
                    return;
                }
            }
        }
        private void TryToDisconnect()
        {
            if (usbServoController == null)
            {
                // Log("Connecting stopped.");
                return;
            }

            try
            {
                // Log("Disconnecting...");
                usbServoController.disconnect();
            }
            catch (Exception e)
            {
                // Log(e);
                // Log("Failed to disconnect cleanly.");
            }
            finally
            {
                // do this no matter what
                usbServoController = null;
                // Log("Disconnected from #" + serialNumber + ".");
            }
        }
        /// <summary>
        /// Gets servo positions and status from the controller
        /// </summary>
        public ServoPositionInfo[] GetPosition()
        {
            ServoStatus[] servos;
            if (usbServoController != null)
            {
                usbServoController.getVariables(out servos);
                return GetServoAngleStatus(servos);
            }
            return null;
        }
        /// <summary>
        /// Gets servo positions and status from the controller.
        /// Also gets stack variables.
        /// </summary>
        public ServoPositionInfo[] GetPosition(out MaestroVariables variables, out short[] stack, out ushort[] callStack)
        {
            ServoStatus[] servos;
            usbServoController.getVariables(out variables, out stack, out callStack, out servos);
            return GetServoAngleStatus(servos);
        }
        private ServoPositionInfo[] GetServoAngleStatus(ServoStatus[] servos)
        {
            if (servos == null)
            {
                return new ServoPositionInfo[0];
            }
            ServoPositionInfo[] servoAngleStatus = new ServoPositionInfo[servos.Length];
            for (int i = 0; i < servos.Length; i++)
            {
                servoAngleStatus[i] = new ServoPositionInfo();
                servoAngleStatus[i].Acceleration = servos[i].acceleration;
                servoAngleStatus[i].ReportPosition(servos[i].position);
                servoAngleStatus[i].Speed = servos[i].speed;
                servoAngleStatus[i].Target = servos[i].target;
#warning short to double angle conversion needed here
            }

            return servoAngleStatus;
        }
        public bool SetServoPosition(ServoNoEnu servoNo, double angle, double speed, double acceleration)
        {
            if (usbServoController != null && usbServoController.servoCount > (byte)servoNo)
            {
                /*servoController.setAcceleration((byte)servoNo, (byte)acceleration);
                servoController.setSpeed((byte)servoNo, (ushort)speed);
                ushort angleValue = ServoAngleCls.FindServoAngleValue(servoNo, angle);
                servoController.setTarget((byte)servoNo, angleValue);
                 */

                try
                {
                    usbServoController.setAcceleration((byte)servoNo, (byte)acceleration);
                }
                catch (Exception)
                {
                }
                try
                {
                    usbServoController.setSpeed((byte)servoNo, (ushort)speed);
                }
                catch (Exception)
                {
                }
                try
                {
                    usbServoController.setTarget((byte)servoNo, (ushort)angle);
                }
                catch (Exception)
                {
                }
                return true;
            }
            return false;
        }
    }    
}
