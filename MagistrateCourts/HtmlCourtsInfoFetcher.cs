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

        public IDataParserHandler Parser { get; private set; }
        public event EventHandler ImStillAlive;
       
        public HtmlCourtsInfoFetcher(IDataParserHandler parser)
        {
            Parser = parser;
            Parser.ImStillAlive += Parser_ImStillAlive;     
        }

        private void Parser_ImStillAlive(object sender, EventArgs e)
        {
            ImStillAlive(sender, e);
        }

        private void KeepTracking(string format, params object[] arg )
        {
            logger.DebugFormat(format, arg);
            if(ImStillAlive != null)
                ImStillAlive(this, new EventArgs());
        }

        public IEnumerable<IChangeableData> GetData()
        {
            logger.Debug(MethodBase.GetCurrentMethod().Name);

            const string sudRF = "https://sudrf.ru";

            return Parser.Parce(sudRF);
        }
    }
}
