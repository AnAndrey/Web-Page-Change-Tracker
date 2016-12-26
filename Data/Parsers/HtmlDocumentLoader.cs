using System;
using System.Collections.Generic;
using System.Text;
using HtmlAgilityPack;
using static NoCompany.Data.Properties.Resources;
using log4net;
using System.Net;
using System.IO;
using static CodeContracts.Requires;

namespace NoCompany.Data.Parsers
{ 
    public class HtmlDocumentLoader
    {
        private static ILog logger = LogManager.GetLogger(typeof(HtmlDocumentLoader));
        private int _retryCount = 3;
        public int RetryCount
        {
            get{ return _retryCount; }
            set
            {
                ValidState(value > default(int));
                _retryCount = value;
            }
        }
        public virtual HtmlDocument LoadHtmlDocument(string url, Encoding encoding)
        {
            logger.DebugFormat(Trace_HtmlLoad, url);
            if (!Uri.IsWellFormedUriString(url, UriKind.Absolute))
            {
                logger.FatalFormat(Error_InValidUrl, url);
                return null;
            }

            WebClient webClient = new WebClient() { Encoding = encoding };
            for (int t = 0; t < RetryCount; t++)
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
                        logger.WarnFormat(Error_SkipPage, url);
                        break;
                    }

                    logger.ErrorFormat(Error_FailedLoadPageRetry, wex, url, t, RetryCount);
                }
            }
            return null;
        }
    }
}