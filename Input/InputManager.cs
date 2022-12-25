using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wanderer.Software;

namespace Wanderer.Hardware.Input
{
    public class InputManager : Module
    {
        public static List<IInputDevice> InputDevices { get; } = new List<IInputDevice>();
        public InputManager()
        {
            var list1 = HidSharp.DeviceList.Local.GetAllDevices();
            var list2 = HidLibrary.HidDevices.Enumerate();
            foreach (var device1 in list2)
            {
                var l = list1.FirstOrDefault();
                var device2 = (HidSharp.HidDevice)list1.Where(a => a.DevicePath.Equals(device1.DevicePath) && (a.GetType() == typeof(HidSharp.HidDevice) || a.GetType().Name == "WinHidDevice")).FirstOrDefault();
                if (device2 != null)
                {
                    CreateInputDevice(device1, device2);
                    //Console.WriteLine($"{((HidSharp.HidDevice)device2).ProductName} - {device1.Attributes.ProductHexId}-{device1.Attributes.VendorHexId} - {device1.Description}, IsConnected: {device1.IsConnected}, IsOpen: {device1.IsOpen}");
                }
            }
        }

        private void CreateInputDevice(HidLibrary.HidDevice? device1, HidSharp.HidDevice? device2)
        {
            if (device1 != null && device2 != null)
            {
                if (device1.Description.ToLower().Contains("game controller"))
                {
                    var joystick = Joystick.Create(device1, device2);
                    AddInputDevice(joystick);
                }
                else if (device1.Description.ToLower().Contains("keyboard"))
                {
                    var keyboard = Keyboard.Create(device1, device2);
                    AddInputDevice(keyboard);
                }
                else if (device1.Description.ToLower().Contains("mouse"))
                {
                    var mouse = Mouse.Create(device1, device2);
                    AddInputDevice(mouse);
                }
            }
        }
        private void AddInputDevice(IInputDevice inputDevice)
        {
            if (inputDevice != null)
            {
                InputDevices.Add(inputDevice);
            }
        }
    }
}
