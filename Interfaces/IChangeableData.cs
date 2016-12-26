using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NoCompany.Interfaces
{
    /// <summary>
    ///     Allows an object to implement a ChangeableData, and represents a cantainer
    ///     for a hierarchical data with three structure.
    /// </summary>
    public interface IChangeableData
    {
        string Value { get; }
        string Name { get; }

        bool HasChilds { get; }

        IEnumerable<IChangeableData> Childs { get; set; }
    }
}
