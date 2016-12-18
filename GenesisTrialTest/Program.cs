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
            IDataAnalyzer changeDetector = new DataAnalyzer();
            changeDetector.DetectedDifferenceEvent += Rrrr_ChangeHasDetectedEvent;
            changeDetector.ErrorEvent += Rrrr_ErrorEvent;
            
            //GetHmlData
            IDataFetcher firstSourceOfData = new HtmlCourtsInfoFetcher();
            var receivedData = firstSourceOfData.GetData();

            IDataFetcher secondSourceOfData = new SqlDataFetcher();

            var presavedData = secondSourceOfData.GetData();

            changeDetector.Analyze(receivedData, presavedData);

            SqlDataPreserver preserve = new SqlDataPreserver();

            // Clear All old data
            preserve.CleanStorage();
            // Save new data
            preserve.SaveData(receivedData);

            Console.WriteLine("Press any key to exit...");
            Console.ReadLine();
            return;
        }

        private static void Rrrr_ErrorEvent(object sender, string e)
        {
            Console.WriteLine("Error ocured - '{0}'.", e);
        }

        private static void Rrrr_ChangeHasDetectedEvent(object sender, EventArgs e)
        {
            DataChangedEventArgs args = e as DataChangedEventArgs;
            if (args != null)
            {
                Console.WriteLine("Message - '{0}', state - '{1}'", args.Message, args.State);
            }
        }

    }
}