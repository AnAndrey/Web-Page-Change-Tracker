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
using NoCompany.Data.Parsers;

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

            const string sudRF = "https://sudrf.ru";

            IDataParserHandler dapataParser = new RegionsParser();
            return dapataParser.Parce(sudRF);

            //logger.Debug(MethodBase.GetCurrentMethod().Name);

            //cancellationToken.ThrowIfCancellationRequested();

            //var regions = GetAllCurtRegions();

            //cancellationToken.ThrowIfCancellationRequested();
            //Parallel.ForEach(regions, new ParallelOptions() { MaxDegreeOfParallelism = c_maxDegreeOfParallelism }, region =>
            //        {
            //            if (region.Value == "30")
            //            {
            //                Console.WriteLine(region.Value);
            //               // region.Childs = GetAllCurtDistricts(region.Value);
            //                cancellationToken.ThrowIfCancellationRequested();
            //                if (region.Childs != null)
            //                {
            //                    foreach (var district in region.Childs)
            //                    {
            //                  //      district.Childs = GetAllLocations(district.Value);
            //                        cancellationToken.ThrowIfCancellationRequested();
            //                    }
            //                }
            //            }
            //        });

            //return regions;
        }

        



    }
}
