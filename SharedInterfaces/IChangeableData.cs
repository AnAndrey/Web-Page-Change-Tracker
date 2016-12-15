using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedInterfaces
{
    
    public abstract class  ChangeableData
    {
        public virtual string GroupName { get; protected set; }
        public virtual string Value { get; }
        public virtual string Name { get; }

        public virtual bool HasChilds
        {
            get { return Childs != null ? Childs.Any() : false; }
        }

        public virtual IEnumerable<ChangeableData> Childs { get; set; }

        public ChangeableData(string name, string value)
        {
            Name = name;
            Value = value;
        }
    }
}
