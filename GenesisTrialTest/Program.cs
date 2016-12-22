using System;
using Microsoft.Practices.Unity;
using Microsoft.Practices.Unity.Configuration;

namespace GenesisTrialTest
{
    class Program
    {
        // 1. Init storage connection 
        // 2. Init Reader, set parser 
        // 3. Init changes detector   
        // 4. Init notifyer            
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
            catch (Exception ex)
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