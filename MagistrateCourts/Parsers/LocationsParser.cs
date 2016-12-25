using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NoCompany.Interfaces;
using static NoCompany.Data.Properties.Resources;
using HtmlAgilityPack;
using log4net;
using System.Threading;

namespace NoCompany.Data.Parsers
{
    public class LocationsParser : DataParserHandlerBase
    {
        private static ILog logger = LogManager.GetLogger(typeof(LocationsParser));

        public LocationsParser() 
        {
            Failer = new FailureHandler();
        }

        public LocationsParser(IDataParserHandler successHandler, IDataParserHandler failureHandler)
            : base(successHandler, failureHandler)
        {
        }
        public override IEnumerable<IChangeableData> Parce(string entryPoint)
        {
            ShouldStopOperating();
            IEnumerable<IChangeableData> data = null;
            try
            {
                data = TryParce(entryPoint);
            }
            catch (HtmlRoutineException ex)
            {
                logger.ErrorFormat(ex.Message);
            }

            if (data == null)
            {
                logger.WarnFormat(Warn_NextParser, Failer.GetType().Name);
                data = Failer.Parce(entryPoint);
            }

            return data;
        }

        protected override List<IChangeableData> TryParce(string pageUrl)
        {
            ShouldStopOperating();
            logger.DebugFormat(Trace_LoadLocations, pageUrl);
            KeepTracking();
            string locationsUrl = pageUrl + "/modules.php?name=terr";
            HtmlDocument allLocations = LoadHtmlDocument(locationsUrl, Encoding.UTF8);
            if (allLocations == null)
            {
                logger.ErrorFormat(Error_FailedToLocationsPage, pageUrl);
                return null;
            }

            HtmlNode contentTable = allLocations.DocumentNode.SelectSingleNode("//div[@class='content']");
            if (contentTable == null)
            {
                logger.ErrorFormat(Error_LocationsTableFail, locationsUrl);
                return null;
            }
            var territoryItems = contentTable.SelectNodes("div[@class='terr-item']");
            if (territoryItems == null)
            {
                logger.ErrorFormat(Error_LocationsTerritoryFail, locationsUrl);
                return null;
            }
            var data = from n in territoryItems
                   select (IChangeableData)new CourtLocation(n.SelectSingleNode("div[@class='right']").InnerText.Trim(new char[] { '\r', '\n', '\t' }),
                                                            n.SelectSingleNode("div[@class='left']").InnerText);
            return data.ToList();
        }
    }
}
