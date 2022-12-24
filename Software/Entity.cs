using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wandarer.Software
{
    public class Entity
    {
        protected string name = "Unknown entity";
        public string Name
        {
            get { return name; }
            protected set { name = value; }
        }
    }
}
