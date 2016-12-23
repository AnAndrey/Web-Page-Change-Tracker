using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NoCompany.Interfaces;
using NoCompany.Data.Properties;
using HtmlAgilityPack;
using log4net;

namespace NoCompany.Data.Parsers
{
    public class LocationsParser : DataParserHandlerBase
    {
        public static ILog logger = LogManager.GetLogger(typeof(LocationsParser));

        public LocationsParser():base (null, new FailureHandler())
        {

        }
        public override IEnumerable<IChangeableData> Parce(string entryPoint)
        {
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
                logger.WarnFormat("Trying to use next ensuing parser - '{0}'.", Failer.GetType().Name);
                data = Failer.Parce(entryPoint);
            }

            return data;
        }

        protected override List<IChangeableData> TryParce(string pageUrl)
        {
            KeepTracking(Resources.Trace_LoadLocations, pageUrl);

            string locationsUrl = pageUrl + "/modules.php?name=terr";
            HtmlDocument allLocations = LoadHtmlDocument(locationsUrl, Encoding.UTF8);
            if (allLocations == null)
            {
                logger.ErrorFormat(Resources.Error_FailedToLocationsPage, pageUrl);
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
            var data = from n in territoryItems
                   select (IChangeableData)new CourtLocation(n.SelectSingleNode("div[@class='right']").InnerText.Trim(new char[] { '\r', '\n', '\t' }),
                                                            n.SelectSingleNode("div[@class='left']").InnerText);
            return data.ToList();
        }

        private HtmlDocument LoadHtmlDocument(string locationsUrl, Encoding uTF8)
        {
            throw new NotImplementedException();
        }

        private void KeepTracking(object trace_LoadLocations, string pageUrl)
        {
            throw new NotImplementedException();
        }
    }
}
