using System;
using NoCompany.Interfaces;
using Microsoft.Practices.Unity;
using Microsoft.Practices.Unity.Configuration;
using System.Configuration;

namespace GenesisTrialTest
{
    class Program
    {
            // 1. Init storage connection (IStorageConnector)
            // 2. Init Reader, set parser (IDataFetcher)
            // 3. Init changes detector   (IChangeDetector)
            // 4. Init notifyer           (INotifyer)  
            // 5. Read ChangeableData
            // 6. Compare -> Store:Notify and Store
        static void Main(string[] args)
        {
            try
            {
                var container = new UnityContainer();
                container.LoadConfiguration();
                ChangesNotifierFacade noty = container.Resolve<ChangesNotifierFacade>();
                noty.FindAndNotify();
            }
            catch(Exception ex)
            {
                Console.WriteLine("Something goes wrong: \r\n {0}", ex);
            }

            Console.WriteLine();
            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
            return;
        }
        
    }
}