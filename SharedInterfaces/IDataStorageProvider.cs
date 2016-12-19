using System.Collections.Generic;

namespace SharedInterfaces
{
    public interface IDataStorageProvider:IDataFetcher
    {
        void SaveData(IEnumerable<IChangeableData> data);
        void CleanStorage();
    }
}
