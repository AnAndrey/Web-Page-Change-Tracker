using System.Collections.Generic;

namespace NoCompany.Interfaces
{
    public interface IDataStorageProvider:IDataFetcher
    {
        void SaveData(IEnumerable<IChangeableData> data);
        void CleanStorage();
    }
}
