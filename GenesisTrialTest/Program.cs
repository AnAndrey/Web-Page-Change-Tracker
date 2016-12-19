using System;
using System.Collections.Generic;
using System.ComponentModel;
using SimpleDataAnalyzer;
using System.Data.SqlClient;
using System.Text;
using System.Net;
using MagistrateCourts;
using System.Linq;
using EmailNotifyer;
using SharedInterfaces;

namespace GenesisTrialTest
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
                EmailNotifierFacade f = new EmailNotifierFacade(new DataAnalyzer(),
                                                                new HtmlCourtsInfoFetcher(),
                                                                new SqlDataStorageProvider(),
                                                                null);

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