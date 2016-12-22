using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NoCompany.Interfaces;
using HtmlAgilityPack;
using System.Net;
using System.IO;
using System.Threading.Tasks;
using NoCompany.Data.Properties;
using System.Threading;
using log4net;
using System.Reflection;

namespace NoCompany.Data
{
    public class HtmlCourtsInfoFetcher : IDataProvider
    {
        public static ILog logger = LogManager.GetLogger(typeof(HtmlCourtsInfoFetcher));


        private readonly int c_retryCount = 5;
        private readonly int c_maxDegreeOfParallelism = 20;

        public event EventHandler ImStillAlive;

        public HtmlCourtsInfoFetcher() { }
        
        public HtmlCourtsInfoFetcher(int retryCount, int maxDegreeOfParallelism)
        {
            c_retryCount = retryCount;
            c_maxDegreeOfParallelism = maxDegreeOfParallelism;
        }
        protected virtual HtmlDocument LoadHtmlDocument(string url, Encoding encoding)
        {
            logger.Debug(MethodBase.GetCurrentMethod().Name);

            KeepTracking(Resources.Trace_HtmlLoad, url);
            if (!Uri.IsWellFormedUriString(url, UriKind.Absolute))
            {
                logger.FatalFormat(Resources.Error_InValidUrl, url);
                return null;
            }
            
            WebClient webClient = new WebClient() { Encoding = encoding };
            for (int t = 0; t < c_retryCount; t++)
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

                    HttpWebResponse errorResponse = wex.Response as HttpWebResponse;
                    if (errorResponse != null && errorResponse.StatusCode == HttpStatusCode.NotFound)
                    {
                        logger.WarnFormat(Resources.Error_SkipPage, url);
                        break;
                    }

                    logger.ErrorFormat(Resources.Error_FailedLoadPageRetry, wex, url, t, c_retryCount);
                }
            }
            return null;
        }

        private IEnumerable<IChangeableData> GetAllCurtRegions()
        {
            logger.Debug(MethodBase.GetCurrentMethod().Name);

            const string sudrf = "https://sudrf.ru";
            const string magistratCourtsNodeXPath = "//a[@href and @title='Участки мировых судей']";
            const string href = "href";
            const string option = "option";

            KeepTracking(Resources.Trace_AllCurtRegionsLoad);


            var list = new List<string>();
            HtmlDocument doc = LoadHtmlDocument(sudrf, Encoding.UTF8);
            if (doc == null)
            {
                throw new HtmlRoutineException(Resources.Error_FailedToLoadMainPage);
            }

            var magistratCourtsLinkNode = doc.DocumentNode.SelectSingleNode(magistratCourtsNodeXPath);
            if (magistratCourtsLinkNode == null)
            {
                throw new HtmlRoutineException(Resources.Error_NodeIsNotFound, magistratCourtsNodeXPath);
            }

            string magistrateCourtsLink = magistratCourtsLinkNode.Attributes[href].Value;

            if (String.IsNullOrEmpty(magistrateCourtsLink))
            {
                throw new HtmlRoutineException(Resources.Error_AttributeIsNotFound, href, magistratCourtsNodeXPath);
            }

            HtmlNode.ElementsFlags.Remove(option);
            HtmlDocument allCourtRegionsDoc = LoadHtmlDocument(sudrf + magistrateCourtsLink, Encoding.UTF8);
            if (allCourtRegionsDoc == null)
            {
                throw new HtmlRoutineException(Resources.Error_FailedToLoadRegions);
            }

            var allCourtRegionsNode = allCourtRegionsDoc.DocumentNode.SelectSingleNode("//select[@id='ms_subj']");

            return allCourtRegionsNode.Descendants(option).Skip(1)
                    .Select(n => new CourtRegion(n.InnerText, n.Attributes["value"].Value))
                    .Cast<IChangeableData>().ToList();
        }

        private void KeepTracking(string format, params object[] arg )
        {
            logger.DebugFormat(format, arg);
            if(ImStillAlive != null)
                ImStillAlive(this, new EventArgs());
        }

        public IEnumerable<IChangeableData> GetData(CancellationToken cancellationToken)
        {
            logger.Debug(MethodBase.GetCurrentMethod().Name);

            cancellationToken.ThrowIfCancellationRequested();

            var regions = GetAllCurtRegions();

            cancellationToken.ThrowIfCancellationRequested();
            Parallel.ForEach(regions, new ParallelOptions() { MaxDegreeOfParallelism = c_maxDegreeOfParallelism }, region =>
                    {
                        if (region.Value == "30")
                        {
                            Console.WriteLine(region.Value);
                            region.Childs = GetAllCurtDistricts(region.Value);
                            cancellationToken.ThrowIfCancellationRequested();
                            if (region.Childs != null)
                            {
                                foreach (var district in region.Childs)
                                {
                                    district.Childs = GetAllLocations(district.Value);
                                    cancellationToken.ThrowIfCancellationRequested();
                                }
                            }
                        }
                    });

            return regions;
        }

        private IEnumerable<IChangeableData> GetAllLocations(string site)
        {
            KeepTracking(Resources.Trace_LoadLocations, site);

            string locationsUrl = site + "/modules.php?name=terr";
            HtmlDocument allLocations = LoadHtmlDocument(locationsUrl, Encoding.UTF8);
            if (allLocations == null)
            {
                logger.ErrorFormat(Resources.Error_FailedToLocationsPage, site);
                return null;
            }

            HtmlNode contentTable = allLocations.DocumentNode.SelectSingleNode("//div[@class='content']");
            if (contentTable == null)
            {
                logger.ErrorFormat(Resources.Error_LocationsTableFail, locationsUrl);
                return null;                    
            }
            var territoryItems = contentTable.SelectNodes("div[@class='terr-item']");
            if (territoryItems == null)
            {
                logger.ErrorFormat(Resources.Error_LocationsTerritoryFail, locationsUrl);
                return null;
            }
            return from n in territoryItems
                   select (IChangeableData)new CourtLocation(n.SelectSingleNode("div[@class='right']").InnerText.Trim(new char[] { '\r', '\n', '\t' }),
                                                            n.SelectSingleNode("div[@class='left']").InnerText);
        }

        private IEnumerable<IChangeableData> GetAllCurtDistricts(string regionNumber)
        {
            KeepTracking(Resources.Trace_LoadDistrictsForRegion, regionNumber);
            const string getDistrictFormatString = "https://sudrf.ru/index.php?id=300&act=go_ms_search&searchtype=ms&var=true&ms_type=ms&court_subj={0}&ms_city=&ms_street=";
            string districtUrl = String.Format(getDistrictFormatString, regionNumber);
            HtmlDocument allCourtDistrcits = LoadHtmlDocument(districtUrl, Encoding.UTF8);
            if (allCourtDistrcits == null)
            {
                logger.ErrorFormat(Resources.Error_FailedToLoadDistricts, regionNumber, districtUrl);
                return null;
            }
            var searchResultTbl = allCourtDistrcits.DocumentNode.SelectNodes("//table[@class='msSearchResultTbl']//tr//td");
            if (searchResultTbl == null)
            {
                logger.ErrorFormat(Resources.Error_DistrictsTableFail, districtUrl);
                return null;
            }

            return searchResultTbl.Select(n => new CourtDistrict(n.Element("a").InnerText,
                                                                 n.SelectSingleNode(".//div[@class='courtInfoCont']//a").InnerText))
                                                                  .Cast<IChangeableData>()
                                                                  .ToList();
        }

    }
}
