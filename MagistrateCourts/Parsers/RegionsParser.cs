using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NoCompany.Interfaces;
using log4net;
using System.Reflection;
using HtmlAgilityPack;
using NoCompany.Data.Properties;

namespace NoCompany.Data.Parsers
{
    public class RegionsParser : DataParserHandlerBase
    {
        private static ILog logger = LogManager.GetLogger(typeof(RegionsParser));

        public RegionsParser() : base(new DistrictsParser(), new FailureHandler())
        {
        }

        public RegionsParser(IDataParserHandler successHandler, IDataParserHandler failureHandler)
            : base(successHandler, failureHandler)
        {
        }

        protected override List<IChangeableData> TryParce(string pageUrl)
        {
            logger.Debug(MethodBase.GetCurrentMethod().Name);

            const string magistratCourtsNodeXPath = "//a[@href and @title='Участки мировых судей']";
            const string href = "href";
            const string option = "option";

            logger.Debug(Resources.Trace_AllCurtRegionsLoad);
            KeepTracking();


            var list = new List<string>();
            HtmlDocument doc = LoadHtmlDocument(pageUrl, Encoding.UTF8);
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
            HtmlDocument allCourtRegionsDoc = LoadHtmlDocument(pageUrl + magistrateCourtsLink, Encoding.UTF8);
            if (allCourtRegionsDoc == null)
            {
                throw new HtmlRoutineException(Resources.Error_FailedToLoadRegions);
            }

            var allCourtRegionsNode = allCourtRegionsDoc.DocumentNode.SelectSingleNode("//select[@id='ms_subj']");

            return allCourtRegionsNode.Descendants(option).Skip(1)
                    .Select(n => new CourtRegion(n.InnerText, n.Attributes["value"].Value)).Where(x => x.Value == "30")
                    .Cast<IChangeableData>().ToList();
        }


    }
}
