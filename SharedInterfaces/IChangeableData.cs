using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedInterfaces
{
    
    public interface IChangeableData
    {
        string Value { get; }
        string Name { get; }

        bool HasChilds { get; }
        //{
        //    get { return Childs != null ? Childs.Any() : false; }
        //}

        IEnumerable<IChangeableData> Childs { get; set; }

        //public IChangeableData(string name, string value)
        //{
        //    Name = name;
        //    Value = value;
        //}
    }
}
