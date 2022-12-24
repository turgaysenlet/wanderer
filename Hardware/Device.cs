using Wandarer.Software;

namespace Wandarer.Hardware
{
    public abstract class Device : Entity
    {
        public static List<Device> Devices { get; } = new List<Device>();
        public static int DeviceCount { get; private set; } = 0;
        public int DeviceNo { get; private set; } = 0;
        
        public Device()
        {
            AddDevice(this);
        }
        private static Device AddDevice(Device device)
        {
            if (device != null)
            {
                Devices.Add(device);
                device.DeviceNo = DeviceCount++;
            }
            return device;
        }

        ~Device()
        {
            Devices.Remove(this);
        }
        protected DeviceStateEnu state = DeviceStateEnu.Unknown;

        public DeviceStateEnu State
        {
            get { return state; }
            protected set { state = value; }
        }

        protected DeviceTypeEnu deviceType = DeviceTypeEnu.Unknown;

        public DeviceTypeEnu DeviceType
        {
            get { return deviceType; }
            protected set { deviceType = value; }
        }
        public enum DeviceStateEnu
        {
            Unknown,
            NotFound,
            Found,
            Started,
            Stopped,
            Failed
        }
        public enum DeviceTypeEnu
        {
            Unknown,
            Power,
            Sensor,
            Input,
            Network,
            Interface,
            Actuator,
            System,
        }
        public override string ToString()
        {
            return $"{base.ToString()} - {State} - Device {DeviceNo}";
        }
    }
}