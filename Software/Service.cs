using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wanderer.Software
{
    public class Service
    {
        public static List<Service> Services { get; } = new List<Service>();
        public static int ServiceCount { get; private set; } = 0;
        public int ServiceNo { get; private set; } = 0;
        public string Pattern { get; set; } = "";
        private static Service AddService(Service service)
        {
            if (service != null)
            {
                Services.Add(service);
                service.ServiceNo = ServiceCount++;
            }
            return service;
        }
        public Service(string pattern)
        {
            Pattern = pattern;
            AddService(this);
        }
        ~Service()
        {
            Services.Remove(this);
        }
        public override string ToString()
        {
            return $"{base.ToString()} - Service {ServiceNo}";
        }

        protected ServicestateEnu state = ServicestateEnu.Unknown;

        public ServicestateEnu State
        {
            get { return state; }
            protected set { state = value; }
        }

        public enum ServicestateEnu
        {
            Unknown,
            Created,
            Started,
            Stopped,
            Failed
        }
    }
}
