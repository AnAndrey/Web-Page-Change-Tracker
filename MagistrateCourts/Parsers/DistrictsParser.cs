using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NoCompany.Interfaces;
using HtmlAgilityPack;
using NoCompany.Data.Properties;
using log4net;

namespace NoCompany.Data.Parsers
{
    public class DistrictsParser : DataParserHandlerBase
    {
        public static ILog logger = LogManager.GetLogger(typeof(DistrictsParser));
        private const string c_districtFormatString = "https://sudrf.ru/index.php?id=300&act=go_ms_search&searchtype=ms&var=true&ms_type=ms&court_subj={0}&ms_city=&ms_street=";

        public DistrictsParser(): base(new LocationsParser(), new FailureHandler())
        {
        }

        protected override List<IChangeableData> TryParce(string regionNumber)
        {
            KeepTracking(Resources.Trace_LoadDistrictsForRegion, regionNumber);
            string districtUrl = String.Format(c_districtFormatString, regionNumber);
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

        private HtmlDocument LoadHtmlDocument(string districtUrl, Encoding uTF8)
        {
            throw new NotImplementedException();
        }

        private void KeepTracking(object trace_LoadDistrictsForRegion, object regionNumber)
        {
            throw new NotImplementedException();
        }
    }
}
