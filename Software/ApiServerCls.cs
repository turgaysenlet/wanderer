using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wanderer.Software
{
    public class ApiServerCls : ModuleCls
    {
        public List<Service> Services { get; } = new List<Service>();
        protected Service AddService(Service service)
        {
            Services.Add(service);
            return service;
        }
        protected void RemoveService(Service service)
        {
            Services.Remove(service);
        }       
    }
}
