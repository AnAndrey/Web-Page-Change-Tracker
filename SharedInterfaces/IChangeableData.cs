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

        IEnumerable<IChangeableData> Childs { get; set; }
    }
}
