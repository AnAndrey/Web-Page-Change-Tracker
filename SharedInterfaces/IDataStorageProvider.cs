using System.Collections.Generic;
using System.Threading;

namespace NoCompany.Interfaces
{
    /// <summary>
    ///     Allows an object to implement a DataStorageProvider, and represents a set of methods
    ///     that are used to data controling operations.
    /// </summary>
    public interface IDataStorageProvider:IDataProvider
    {
        /// <summary>
        /// Saves data in storage.
        /// </summary>
        void SaveData(IEnumerable<IChangeableData> data, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Clean data storage.
        /// </summary>
        void CleanStorage();
    }
}
