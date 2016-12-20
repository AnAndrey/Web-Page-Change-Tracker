using System;
using NoCompany.Data;
using NoCompany.Interfaces;
using NoCompany.Core;
using NoCompany.DataAnalyzer;


namespace NoCompany.Runner
{
    class Program
    {
        public static void EntryPoint()
        {
            // 1. Init storage connection (IStorageConnector)
            // 2. Init Reader, set parser (IDataFetcher)
            // 3. Init changes detector   (IChangeDetector)
            // 4. Init notifyer           (INotifyer)  
            // 5. Read ChangeableData
            // 6. Compare -> Store:Notify and Store
        }
        static void Main(string[] args)
        {
            try
            {
                
                ChangesNotifierFacade f = new ChangesNotifierFacade(new DataAnalyzer.DataAnalyzer(),
                                                                new HtmlCourtsInfoFetcher(),
                                                                new SqlDataStorageProvider(),
                                                                new EmailNotifier.EmailNotifier());

                f.FindAndNotify();
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