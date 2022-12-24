using Wandarer.Software;

namespace Wandarer.Hardware
{
    public abstract class Device : Entity
    {
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
            Stopped
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
    }
}