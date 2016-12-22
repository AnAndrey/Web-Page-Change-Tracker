using System;
using System.Collections.Generic;
using System.Threading;

namespace NoCompany.Interfaces
{
    /// <summary>
    ///     Allows an object to implement a DataProvider, and represents a method
    ///     that are used to retrieve requirable information
    /// </summary>
    public interface IDataProvider
    {
        /// <summary>
        /// Retrieve set of data from provider.
        /// </summary>
        IEnumerable<IChangeableData> GetData(CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Event to distinguish time consumable operations and hangs.
        /// </summary>
        event EventHandler<string> ImStillAlive;
    }


}
