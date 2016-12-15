using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SharedInterfaces;
using HtmlAgilityPack;
using System.Net;
using System.IO;
using System.Threading.Tasks;

namespace GenesisTrialTest
{
    public class HtmlChangeableDataFetcher : IDataFetcher
    {
        protected virtual HtmlDocument LoadHtmlDocument(string url, Encoding encoding)
        {
            if (!Uri.IsWellFormedUriString(url, UriKind.Absolute))
            {
                Console.WriteLine("Invalid URL '{0}'.", url);
                return null;
            }
            
            WebClient webClient = new WebClient() { Encoding = encoding };
            for (int t = 0; t < 5; t++)
            {
                try
                {
                    using (Stream htmlStream = webClient.OpenRead(url))
                    {
                        var list = new List<string>();
                        HtmlDocument doc = new HtmlDocument();
                        doc.Load(htmlStream);

                        return doc;
                    }
                }
                catch (WebException wex)
                {
                    Console.WriteLine("Error - '{0}', attempt - '{1}'.", wex, t);
                    HttpWebResponse errorResponse = wex.Response as HttpWebResponse;
                    if (errorResponse != null && errorResponse.StatusCode == HttpStatusCode.NotFound)
                        break;
                }
            }
            return null;
        }

        private IEnumerable<ChangeableData> GetAllCurtRegions()
        {
            const string sudrf = "https://sudrf.ru";

            var list = new List<string>();
            HtmlDocument doc = LoadHtmlDocument(sudrf, Encoding.UTF8);
            if (doc == null)
            {
                Console.WriteLine("Can't load Regions");
                return null;
            }

            var magistratCourtsLinkNode = doc.DocumentNode.SelectSingleNode("//a[@href and @title='Участки мировых судей']");
            if (magistratCourtsLinkNode == null)
                throw new Exception("redesing: Node is not found.");

            string magistrateCourtsLink = magistratCourtsLinkNode.Attributes["href"].Value;

            if (String.IsNullOrEmpty(magistrateCourtsLink))
            {
                throw new Exception("redesing: href is not found.");
            }

            HtmlNode.ElementsFlags.Remove("option");
            HtmlDocument allCourtRegionsDoc = LoadHtmlDocument(sudrf + magistrateCourtsLink, Encoding.UTF8);
            if (allCourtRegionsDoc == null)
            {
                return null;
            }

            var allCourtRegionsNode = allCourtRegionsDoc.DocumentNode.SelectSingleNode("//select[@id='ms_subj']");

            return allCourtRegionsNode.Descendants("option").Skip(1)
                    .Select(n => new CourtRegion(n.InnerText, n.Attributes["value"].Value))
                    .Cast<ChangeableData>().ToList();
        }

        public IEnumerable<ChangeableData> GetData()
        {
            var regions = GetAllCurtRegions();
            if (regions == null)
            {
                return null;
            }
  
            Parallel.ForEach( regions, new ParallelOptions() { MaxDegreeOfParallelism = 20}, region =>
                    
                    {
                        if (region.Value == "30")
                        {
                            Console.WriteLine(region.Value);
                            region.Childs = GetAllCurtDistricts(region.Value);
                            if (region.Childs != null)
                            {
                                foreach (var district in region.Childs)
                                {
                                    district.Childs = GetAllLocations(district.Value);
                                }
                            }
                        }
                    });
            return regions;
        }

        private IEnumerable<ChangeableData> GetAllLocations(string site)
        {
            string str = site + "/modules.php?name=terr";
            HtmlDocument allCourtDistrcits = LoadHtmlDocument(str, Encoding.UTF8);
            if (allCourtDistrcits == null)
            {
                Console.WriteLine("Couldnt open site '{0}'", site);
                return null;
            }

            HtmlNode contentTable = allCourtDistrcits.DocumentNode.SelectSingleNode("//div[@class='content']");
            if (contentTable == null)
            {
                Console.WriteLine("Node is not found '{0}'", "//div[@class='content']");
                return null;                    
            }
            var territoryItems = contentTable.SelectNodes("div[@class='terr-item']");
            if (territoryItems == null)
            {
                Console.WriteLine("Node is not found '{0}'", "div[@class='terr-item']");
                return null;
            }
            return from n in territoryItems
                   select (ChangeableData)new CourtLocation(n.SelectSingleNode("div[@class='right']").InnerText.Trim(new char[] { '\r', '\n', '\t' }),
                                                            n.SelectSingleNode("div[@class='left']").InnerText);
        }

        private IEnumerable<ChangeableData> GetAllCurtDistricts(string regionNumber)
        {
            const string getDistrictFormatString = "https://sudrf.ru/index.php?id=300&act=go_ms_search&searchtype=ms&var=true&ms_type=ms&court_subj={0}&ms_city=&ms_street=";
            HtmlDocument allCourtDistrcits = LoadHtmlDocument(String.Format(getDistrictFormatString, regionNumber), Encoding.UTF8);
            if (allCourtDistrcits == null)
            {
                Console.WriteLine("Can't load districts for region '{0}'", regionNumber);

                return null;
            }
            var searchResultTbl = allCourtDistrcits.DocumentNode.SelectNodes("//table[@class='msSearchResultTbl']//tr//td");
            if (searchResultTbl == null)
            {
                Console.WriteLine("Node is not found '{0}' for region '{1}'", "//table[@class='msSearchResultTbl']//tr//td", regionNumber);
                return null;
            }

            return searchResultTbl.Select(n => new CourtDistrcits(n.Element("a").InnerText,
                                                                       n.SelectSingleNode(".//div[@class='courtInfoCont']//a").InnerText))
                                  .Cast<ChangeableData>()
                                  .ToList();
        }

    }
}
