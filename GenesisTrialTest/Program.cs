using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using HtmlAgilityPack;
using System.Data.SqlClient;
using System.Text;
using System.Net;
using System.IO;
using System.Linq;
using EmailNotifyer;
using SharedInterfaces;

namespace GenesisTrialTest
{
    class Program
    {
        public static HtmlDocument LoadHtmlDocument(string url, Encoding encoding)
        {
            if (!Uri.IsWellFormedUriString(url, UriKind.RelativeOrAbsolute))
            {
                throw new InvalidDataException(String.Format("Invalid URL '{0}'.", url));
            }
            Stream htmlStream = null;
            try
            {
                WebClient webClient = new WebClient() { Encoding = encoding };
                htmlStream = webClient.OpenRead(url);

                var list = new List<string>();
                HtmlDocument doc = new HtmlDocument();
                doc.Load(htmlStream);


                return doc;
            }
            finally
            {
                if (htmlStream != null)
                {
                    htmlStream.Close();
                }
            }
        }

        public static IEnumerable<int> CountFrom(int start)
        {
            start++;
            for (int i = start; i <= 20; i++)
                yield return i;
        }

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
            IDataFetcher firstSourceOfData = new HtmlChangeableDataFetcher();
            var receivedData = firstSourceOfData.GetData();

            string rootTableName = receivedData.First().GetType().Name;
            IDataFetcher secondSourceOfData = new SqlDataFetcher("CourtRegion");

            var presavedData = secondSourceOfData.GetData();

            //GetStorageData
            changeDetector.Analyze(receivedData, presavedData);
            //return;
            // Clear All old data
            SqlDataPreserver preserve = new SqlDataPreserver(null);

            int t = preserve.ClearTable(rootTableName);
            // Save new data
            preserve.Save(receivedData);

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