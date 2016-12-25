using System;
using static System.String;
using System.Collections.Generic;
using NoCompany.Interfaces;
using log4net;
using HtmlAgilityPack;
using System.Text;
using System.Linq;
using System.Threading.Tasks;

namespace NoCompany.Data.Parsers
{
    public abstract class DataParserHandlerBase : CancelableBase, IDataParserHandler
    {
        private static ILog logger = LogManager.GetLogger(typeof(DataParserHandlerBase));

        public event EventHandler ImStillAlive;

        public int MaxDegreeOfParallelism { get; set; } = 1;
        public HtmlDocumentLoader HtmlDocumentLoader { get; set; } = new HtmlDocumentLoader();
        public DataParserHandlerBase(IDataParserHandler successHandler, IDataParserHandler failureHandler)
        {
            Successor = successHandler;
            Failer = failureHandler;
        }

        public DataParserHandlerBase()
        {
        }
        public IDataParserHandler Failer{get;set;}

        public IDataParserHandler Successor { get; set; }

        public virtual IEnumerable<IChangeableData> Parce(string entryPoint)
        {
            ShouldStopOperating();
            List<IChangeableData> data = TryParce(entryPoint);
            if (data == null)
            {
                data = Failer.Parce(entryPoint).ToList();
            }
            if (data != null)
            {
                Parallel.ForEach(data, new ParallelOptions() { MaxDegreeOfParallelism = MaxDegreeOfParallelism }, item =>
                {
                    item.Childs = Successor.Parce(item.Value);
                });
            }
            return data;
        }

        protected virtual HtmlDocument LoadHtmlDocument(string url, Encoding encoding)
        {
            return HtmlDocumentLoader.LoadHtmlDocument(url, encoding);
        }
        protected virtual void KeepTracking()
        {
            if(ImStillAlive != null)
                ImStillAlive(this, EventArgs.Empty);
        }

        public override void Cancel()
        {
            IsCancellationRequested = true;
            if(Successor != null)
                Successor.Cancel();

            if(Failer != null)
                Failer.Cancel();
        }

        protected abstract List<IChangeableData> TryParce(string entryPoint);
    }

}
