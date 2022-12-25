using Device.Net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wanderer.Hardware.Input
{
    public partial interface IInputDevice
    {
        string DeviceName { get; }
        string VendorId { get;  }
        string ProductId { get;}
        string DevicePath { get; }
        bool IsConnected { get; }
        InputDeviceTypeEnu InputDeviceType { get;  }
    }
}
