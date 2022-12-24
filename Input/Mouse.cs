using HidLibrary;
using Wandarer.Hardware;
namespace Wandarer.Hardware.Input
{
    public class Mouse : Device, IInputDevice
    {
        public static List<Mouse> Mice { get; } = new List<Mouse>();
        public static Mouse Create(HidLibrary.HidDevice device1, HidSharp.HidDevice device2)
        {
            if (!Mice.Where(a => a.vendorId.Equals(device1.Attributes.VendorHexId) && a.productId.Equals(device1.Attributes.ProductHexId)).Any())
            {
                Mouse mouse = new Mouse(device1, device2);
                Console.WriteLine($"Adding: {((HidSharp.HidDevice)device2).ProductName} - {device1.Attributes.ProductHexId}-{device1.Attributes.VendorHexId} - {device1.Description}, IsConnected: {device1.IsConnected}, IsOpen: {device1.IsOpen}");
                return AddMouse(mouse);
            }
            Console.WriteLine($"Skipping: {((HidSharp.HidDevice)device2).ProductName} - {device1.Attributes.ProductHexId}-{device1.Attributes.VendorHexId} - {device1.Description}, IsConnected: {device1.IsConnected}, IsOpen: {device1.IsOpen}");
            return null;
        }

        private static Mouse AddMouse(Mouse mouse)
        {
            Mice.Add(mouse);
            return mouse;
        }
        public Mouse(HidLibrary.HidDevice device1, HidSharp.HidDevice device2)
        {
            this.device1 = device1;
            this.device2 = device2;
            this.vendorId = device1.Attributes.VendorHexId;
            this.productId = device1.Attributes.ProductHexId;
            this.devicePath = device1.DevicePath;
            this.deviceName = device2.GetProductName();
            this.Name = deviceName;
            this.inputDeviceType = InputDeviceTypeEnu.Mouse;
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
