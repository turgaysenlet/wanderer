using System;
using System.Collections.Generic;
using System.Text;
using GeoFramework;
using GeoFramework.Gps;
using GeoFramework.Gps.IO;
using Wanderer.Software.Mapping;

namespace Wanderer.Hardware
{
    public class GpsCls : Wanderer.Hardware.Device
    {
        GeoFramework.Gps.Nmea.NmeaInterpreter nmeaInterpreter1 = new GeoFramework.Gps.Nmea.NmeaInterpreter();

        public GpsCls()
        {
            Initialize(1, 9600);
        }

        public GpsCls(int _comPort, int _baudRate)
        {
            Initialize(_comPort, _baudRate);
        }
        public void Reinitialize()
        {
            Initialize(comPort, baudRate);
        }
        protected void Initialize(int _comPort, int _baudRate)
        {
            comPort = _comPort;
            baudRate = _baudRate;

            SerialDevice s = new SerialDevice(ComPortName, baudRate);
            try
            {
                if (s.IsOpen)
                {
                    s.Close();
                }
            }
            catch (Exception ex)
            {
                //LogCls.Report(LogElementTypeEnu._Error_, "Cannot close serial device! ComPortName: " + ComPortName + ", BaudRate: " + baudRate.ToString() + "!", ex);
            }
            try
            {
                s.Open();
            }
            catch (Exception ex)
            {
                //LogCls.Report(LogElementTypeEnu._Error_, "Cannot open serial device! ComPortName: " + ComPortName + ", BaudRate: " + baudRate.ToString() + "!", ex);
            }
            if (s.IsOpen)
            {
                try
                {
                    nmeaInterpreter1.Device = s;
                    nmeaInterpreter1.Start();
                }
                catch (Exception ex)
                {
                    //LogCls.Report(LogElementTypeEnu._Error_, "Cannot start NMEA Interpreter! ComPortName: " + ComPortName + ", BaudRate: " + baudRate.ToString() + "!", ex);
                    try
                    {
                        nmeaInterpreter1.Stop();
                    }
                    catch (Exception ex2)
                    {
                        //LogCls.Report(LogElementTypeEnu._Error_, "Cannot stop NMEA Interpreter! ComPortName: " + ComPortName + ", BaudRate: " + baudRate.ToString() + "!", ex2);
                    }
                }
            }
            else
            {
                //LogCls.Report(LogElementTypeEnu.Warning, "Serial device not open! ComPortName: " + ComPortName + ", BaudRate: " + baudRate.ToString() + "!");
                SerialDevice.ClearCache();
            }
            System.Threading.Thread.Sleep(500);
            GpsInfoCls info = GetGpsInfo();
            string[] ports = System.IO.Ports.SerialPort.GetPortNames();
            for (int i = 0; i < ports.Length && info.IsConnected == false; i++)
            {
                comPort = int.Parse(ports[i].Substring(3, ports[i].Length - 3));

                s = new SerialDevice(ComPortName, baudRate);
                try
                {
                    if (s.IsOpen)
                    {
                        s.Close();
                    }
                }
                catch (Exception ex)
                {
                    // LogCls.Report(LogElementTypeEnu._Error_, "Cannot close serial device! ComPortName: " + ComPortName + ", BaudRate: " + baudRate.ToString() + "!", ex);
                }
                try
                {                    
                    s.Open();                    
                }
                catch (Exception ex)
                {
                    // LogCls.Report(LogElementTypeEnu._Error_, "Cannot open serial device! ComPortName: " + ComPortName + ", BaudRate: " + baudRate.ToString() + "!", ex);
                }
                try
                {
                    if (s.IsOpen)
                    {
                        nmeaInterpreter1.Device = s;
                        nmeaInterpreter1.Start();
                    }
                }
                catch (Exception ex)
                {
                    // LogCls.Report(LogElementTypeEnu._Error_, "Cannot start NMEA Interpreter! ComPortName: " + ComPortName + ", BaudRate: " + baudRate.ToString() + "!", ex);
                    try
                    {
                        nmeaInterpreter1.Stop();
                    }
                    catch (Exception ex2)
                    {
                        // LogCls.Report(LogElementTypeEnu._Error_, "Cannot stop NMEA Interpreter! ComPortName: " + ComPortName + ", BaudRate: " + baudRate.ToString() + "!", ex2);
                    }
                }
            
                System.Threading.Thread.Sleep(500);
                info = GetGpsInfo();
                if (info.IsConnected == false && s.IsOpen)
                {
                    try
                    {
                        s.Close();
                    }
                    catch (Exception ex)
                    {
                        // LogCls.Report(LogElementTypeEnu._Error_, "Cannot close serial device! ComPortName: " + ComPortName + ", BaudRate: " + baudRate.ToString() + "!", ex);                        
                    }
                }
            }
        }

        private int comPort = 1;
        public int ComPort
        {
            get { return comPort; }
        }

        public string ComPortName
        {
            get { return "COM" + comPort.ToString(); }
        }

        private int baudRate = 4800;
        public int BaudRate
        {
            get { return baudRate; }
        }

        private Position3D lastPosition3D = new Position3D(new Distance(0, DistanceUnit.Meters), new Position(new Latitude(0), new Longitude(0)));
        public Position3D LastPosition3D
        {
            get { return lastPosition3D; }
            private set
            {
                lastPosition3D = value;
                lastPosition = new Position(lastPosition3D.Latitude, lastPosition3D.Longitude);
            }
        }

        private Position lastPosition = new Position(new Position(new Latitude(0), new Longitude(0)));
        public Position LastPosition
        {
            get { return lastPosition; }
            private set
            {
                lastPosition = value;
#warning using old altitude when new is not available
                lastPosition3D = new Position3D(lastPosition3D.Altitude, lastPosition3D.Latitude, lastPosition3D.Longitude);
            }
        }

        public Position3D GetPosition3D()
        {
            Position p = GeoFramework.Gps.IO.Devices.Position;

            Distance h = GeoFramework.Gps.IO.Devices.Altitude;
            return new Position3D(h, p);
        }

        public Position GetPosition()
        {
            return GeoFramework.Gps.IO.Devices.Position;
        }

        public Azimuth GetBearing()
        {
            return GeoFramework.Gps.IO.Devices.Bearing;
        }

        public DateTime GetTime()
        {
            return GeoFramework.Gps.IO.Devices.DateTime;
        }

        public Speed GetSpeed()
        {
            return GeoFramework.Gps.IO.Devices.Speed;
        }

        public List<Satellite> GetSatellites()
        {

            try
            {
                return GeoFramework.Gps.IO.Devices.Satellites;
            }
            catch (Exception ex)
            {
                // LogCls.Report(LogElementTypeEnu.Warning, "Cannot return list of satellites!", ex);
                return new List<Satellite>();
            }
        }

        private bool isFixed = false;
        public bool IsFixed
        {
            get { return isFixed; }
        }
        private double fixPrecision = 0;
        public double FixPrecision
        {
            get { return fixPrecision; }
        }

        public GpsInfoCls GetGpsInfo()
        {
            GpsInfoCls info = new GpsInfoCls();
            Position3D pos3 = GetPosition3D();
            info.CurrentAltitude = pos3.Altitude.ToMeters().Value;
            info.CurrentLatitude = pos3.Latitude.DecimalDegrees;
            info.CurrentLongitude = pos3.Longitude.DecimalDegrees;
            info.CurrentSpeed = GetSpeed().ToKilometersPerHour().Value;
            info.CurrentTime = GetTime();
            info.CurrentBearing = GetBearing().Direction.ToString();
            
            if (GeoFramework.Gps.IO.Devices.GpsDevices != null && GeoFramework.Gps.IO.Devices.GpsDevices.Count > 0)
            {                
                info.IsConnected = GeoFramework.Gps.IO.Devices.GpsDevices[0].IsOpen;
                info.IsGps = GeoFramework.Gps.IO.Devices.GpsDevices[0].IsGpsDevice;
                info.Reliability = GeoFramework.Gps.IO.Devices.GpsDevices[0].Reliability.Value;
                if (nmeaInterpreter1 != null)
                {
                    fixPrecision= nmeaInterpreter1.FixPrecisionEstimate.ToMeters().Value;
                    info.FixPrecision = fixPrecision;
                    isFixed = (info.FixPrecision > 0);
                    info.IsFixed = isFixed;
                }
                else
                {
                    info.FixPrecision = 0;
                    info.FixPrecision = fixPrecision;
                    isFixed = false;
                    info.IsFixed = isFixed;
                }
                List<Satellite> list = GetSatellites();
                if (list == null || list.Count == 0)
                {
                    info.NumberOfSatellites = 0;// new Satellite[0];
                }
                else
                {
                    info.NumberOfSatellites = list.Count;
                }
            }
            else
            {
                info.IsConnected = false;
                info.IsGps = false;
                info.IsFixed = false;
                info.FixPrecision = 0;
                info.NumberOfSatellites = 0;// new Satellite[0];
            }
            return info;
        }
    }
}
