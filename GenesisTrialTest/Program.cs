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


namespace GenesisTrialTest
{
    class Program
    {
        public static HtmlDocument LoadHtmlDocument(string url, Encoding encoding )
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

            HtmlChangeDetector rrrr = new HtmlChangeDetector();
            rrrr.ChangeHasDetectedEvent += Rrrr_ChangeHasDetectedEvent;
            rrrr.ErrorEvent += Rrrr_ErrorEvent;

            rrrr.FindChanges();
            Console.WriteLine("Press any key to exit...");
            Console.ReadLine();
            return;
            const string sudrf = "https://sudrf.ru";
            
            var list = new List<string>();
            HtmlDocument doc = LoadHtmlDocument(sudrf, Encoding.UTF8);
            

            var main = doc.DocumentNode.SelectNodes("//a[@href and @title='Участки мировых судей']");
            if (main == null)
                throw new Exception("redesing: Node is not found.");

            string href = String.Empty;
            foreach (HtmlNode link in main)
            {
                href = link.Attributes["href"].Value;
                list.Add(link.Attributes["title"].Value);
            }

            if (String.IsNullOrEmpty(href))
            {
                throw new Exception("redesing: href is not found.");
            }


            HtmlNode.ElementsFlags.Remove("option");
            HtmlDocument allCourtRegions = LoadHtmlDocument(sudrf+ href, Encoding.UTF8);




           var r = allCourtRegions.DocumentNode.SelectNodes("//select[@id='ms_subj']");



            var regions = r[0].Descendants("option").Skip(1)
                    .Select(n => new
                    {
                        Value = n.Attributes["value"].Value,
                        Text = n.InnerText
                    })
                    .ToList();
            foreach (var t in regions)
            {
                Console.WriteLine("'{0}' '{1}'", t.Value, t.Text);
            }
            //var courtRegions = from nodes in allCourtRegions.DocumentNode.SelectNodes("//select[@id='ms_subj' and @name='court_subj']").Cast<HtmlNode>()
            //               //from options in nodes.SelectNodes("option").Cast<HtmlNode>()
            //               from options in nodes.SelectNodes("option").Cast<HtmlNode>()
            //               select new { OptionNumber = "", Text = options.InnerText };

            LoadAddresses("http://3liv.orl.msudrf.ru/");

            LoadDistrics("28");
            return;
            //ConfigurationManager.ConnectionStrings["SomeConnectionString"].ConnectionString
            
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

        private static void LoadAddresses(string v)
        {
            string str = v + "modules.php?name=terr";
            HtmlDocument allCourtDistrcits = LoadHtmlDocument(str, Encoding.UTF8);
            var territoryItems = allCourtDistrcits.DocumentNode.SelectSingleNode("//div[@class='content']")
                                  .SelectNodes("div[@class='terr-item']");

            var addrs = from n in territoryItems
                        select new {location = n.SelectSingleNode("div[@class='left']").InnerText,
                                    addres = n.SelectSingleNode("div[@class='right']").InnerText.Trim(new char[]{ '\r', '\n' , '\t' })
                        };
            foreach(var addres in addrs)
            {
                Console.WriteLine("Addresses - '{0}', '{1}'", addres.addres, addres.location);

            }

        }

        private static void ParceDistrict(HtmlNode node)
        {
            string districtName = node.Element("a").InnerText;

            string districtNamed = node.SelectSingleNode(".//div[@class='courtInfoCont']//a").InnerText;

            Console.WriteLine("District name - '{0}', '{1}'", districtName, districtNamed);
        }
        private static void LoadDistrics(string districtID)
        {
            const string getDistrictFormatString = "https://sudrf.ru/index.php?id=300&act=go_ms_search&searchtype=ms&var=true&ms_type=ms&court_subj={0}&ms_city=&ms_street=";
            HtmlDocument allCourtDistrcits = LoadHtmlDocument(String.Format(getDistrictFormatString, districtID), Encoding.UTF8);
            var searchResultTbl = allCourtDistrcits.DocumentNode.SelectNodes("//table[@class='msSearchResultTbl']//tr//td");

            foreach (var cell in searchResultTbl)
            {
                ParceDistrict(cell);
            }                                    //.Select(n => n.Elements("tbody").
                                    //    Select(i=> i.Descendants("tr")) 
                                    // );


            //var regions = searchResultTbl[0].Descendants("tbody")
            //        .Select(n => new
            //        {
            //            //Value = n.Attributes["value"].Value,
            //            Text = n.InnerText
            //        })
            //        .ToList();

        }
    }

    public static class IEnumerableExtensions
    {
        public static DataTable AsDataTable<T>(this IEnumerable<T> data) where T: class
        {
            PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(typeof(T));
            var table = new DataTable();

            table.Columns.Add("data", typeof(string));
            foreach (T item in data)
            {
                DataRow row = table.NewRow();
                row["data"] = item;

                table.Rows.Add(row);
            }
            return table;
        }
    }
}
