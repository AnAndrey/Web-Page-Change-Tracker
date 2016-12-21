using System.Collections.Generic;

namespace NoCompany.Interfaces
{
    /// <summary>
    ///     Allows an object to implement a DataStorageProvider, and represents a set of methods
    ///     that are used to data controling operations.
    /// </summary>
    public interface IDataStorageProvider:IDataProvider
    {
        void SaveData(IEnumerable<IChangeableData> data);
        void CleanStorage();
    }
}
