using HidLibrary;
using Wanderer.Hardware;
namespace Wanderer.Hardware.Input
{
    public class Joystick : Device, IInputDevice
    {
        public static List<Joystick> Joysticks { get; } = new List<Joystick>();
        public static Joystick Create(HidLibrary.HidDevice device1, HidSharp.HidDevice device2)
        {
            if (!Joysticks.Where(a => a.vendorId.Equals(device1.Attributes.VendorHexId) && a.productId.Equals(device1.Attributes.ProductHexId)).Any())
            {
                Joystick joystick = new Joystick(device1, device2);
                return AddJoystick(joystick);
            }
            return null;
        }

        private static Joystick AddJoystick(Joystick joystick)
        {
            Joysticks.Add(joystick);
            return joystick;
        }
        public Joystick( HidLibrary.HidDevice device1, HidSharp.HidDevice device2) {
            this.device1 = device1;
            this.device2 = device2;
            this.vendorId = device1.Attributes.VendorHexId;
            this.productId = device1.Attributes.ProductHexId;
            this.devicePath = device1.DevicePath;
            this.deviceName = device2.GetProductName();
            this.Name = deviceName;
            this.inputDeviceType = InputDeviceTypeEnu.Joystick;
            this.deviceType = DeviceTypeEnu.Input;
            State = device1 == null ? DeviceStateEnu.NotFound : device1.IsConnected ? DeviceStateEnu.Found : DeviceStateEnu.NotFound;
        }
        private string vendorId;
        private string productId;
        private string devicePath;
        private string deviceName;
        private InputDeviceTypeEnu inputDeviceType;
        
        private HidLibrary.HidDevice device1;
        private HidSharp.Device device2;
        string IInputDevice.VendorId {
            get { return vendorId; }
        }
        string IInputDevice.ProductId {
            get { return productId; }
        }
        string IInputDevice.DevicePath
        {
            get { return devicePath; }
        }
        bool IInputDevice.IsConnected
        {
            get { return device1 == null ? false : device1.IsConnected; }
        }
        InputDeviceTypeEnu IInputDevice.InputDeviceType { get { return inputDeviceType; } }

        public string DeviceName => deviceName;
    }
}