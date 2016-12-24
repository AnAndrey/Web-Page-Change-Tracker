using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NoCompany.Interfaces;
using HtmlAgilityPack;
using static NoCompany.Data.Properties.Resources;
using log4net;

namespace NoCompany.Data.Parsers
{
    public class DistrictsParser : DataParserHandlerBase
    {
        private static ILog logger = LogManager.GetLogger(typeof(DistrictsParser));
        private const string c_districtFormatString = "https://sudrf.ru/index.php?id=300&act=go_ms_search&searchtype=ms&var=true&ms_type=ms&court_subj={0}&ms_city=&ms_street=";

        public DistrictsParser(): base(new LocationsParser(), new FailureHandler())
        {
        }

        public DistrictsParser(IDataParserHandler successHandler, IDataParserHandler failureHandler)
            : base(successHandler, failureHandler)
        {
        }

        protected override List<IChangeableData> TryParce(string regionNumber)
        {
            KeepTracking(Trace_LoadDistrictsForRegion, regionNumber);
            string districtUrl = String.Format(c_districtFormatString, regionNumber);
            HtmlDocument allCourtDistrcits = LoadHtmlDocument(districtUrl, Encoding.UTF8);
            if (allCourtDistrcits == null)
            {
                logger.ErrorFormat(Error_FailedToLoadDistricts, regionNumber, districtUrl);
                return null;
            }
            var searchResultTbl = allCourtDistrcits.DocumentNode.SelectNodes("//table[@class='msSearchResultTbl']//tr//td");
            if (searchResultTbl == null)
            {
                logger.ErrorFormat(Error_DistrictsTableFail, districtUrl);
                return null;
            }

            return searchResultTbl.Select(n => new CourtDistrict(n.Element("a").InnerText,
                                                                 n.SelectSingleNode(".//div[@class='courtInfoCont']//a").InnerText))
                                                                  .Cast<IChangeableData>()
                                                                  .ToList();
        }


    }
}
