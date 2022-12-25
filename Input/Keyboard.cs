using Hid.Net;
using HidSharp;
using DeviceNet = Device.Net;
namespace Wanderer.Hardware.Input
{
    public class Keyboard : Wanderer.Hardware.Device, IInputDevice
    {
            public static List<Keyboard> Keyboards { get; } = new List<Keyboard>();
        public static Keyboard Create(HidLibrary.HidDevice device1, HidSharp.HidDevice device2)
        {
            if (!Keyboards.Where(a => a.vendorId.Equals(device1.Attributes.VendorHexId) && a.productId.Equals(device1.Attributes.ProductHexId)).Any())
            {
                Keyboard keyboard = new Keyboard(device1, device2);
                return AddKeyboard(keyboard);
            }
            return null;
        }

        private static Keyboard AddKeyboard(Keyboard keyboard)
        {
            Keyboards.Add(keyboard);
            return keyboard;
        }

        protected Keyboard(HidLibrary.HidDevice device1, HidSharp.HidDevice device2)
        {
            this.device1 = device1;
            this.device2 = device2;
            this.vendorId = device1.Attributes.VendorHexId;
            this.productId = device1.Attributes.ProductHexId;
            this.devicePath = device1.DevicePath;
            this.deviceName = device2.GetProductName();
            this.Name = deviceName;
            this.inputDeviceType = InputDeviceTypeEnu.Keyboard;
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
        string IInputDevice.VendorId
        {
            get { return vendorId; }
        }
        string IInputDevice.ProductId
        {
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
