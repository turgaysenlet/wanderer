using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Wanderer.Software
{
    public class EntityCls
    {
        public static List<EntityCls> Entities { get; } = new List<EntityCls>();
        public static int EntityCount { get; private set; } = 0;
        public int EntityNo { get; private set; } = 0;
        private static EntityCls AddEntity(EntityCls entity)
        {
            if (entity != null)
            {
                Entities.Add(entity);
                entity.EntityNo = EntityCount++;
            }
            return entity;
        }
       ~EntityCls()
        {
            Entities.Remove(this);
        }
        protected string name = "Unknown entity";
        public string Name
        {
            get { return name; }
            protected set { name = value; }
        }
        public EntityCls()
        {
            AddEntity(this);
            Name = $"Unknown {this.GetType().Name}";
        }
        public override string ToString()
        {
            if (GetType() == typeof(EntityCls))
            {
                return $"{this.GetType().Name} - {Name} - Entity {EntityNo}";
            }
            return $"{this.GetType().Name} - {Name}";
        }
    }
}
