using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
namespace NoCompany.Interfaces
{
    /// <summary>
    ///     Allows an object to implement a DataProvider, and represents a method
    ///     that are used to retrieve requirable information
    /// </summary>
    public interface IDataProvider
    {
        IEnumerable<IChangeableData> GetData();
        
    }


}
