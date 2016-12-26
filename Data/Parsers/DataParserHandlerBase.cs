using System;
using static System.String;
using System.Collections.Generic;
using NoCompany.Interfaces;
using log4net;
using HtmlAgilityPack;
using System.Text;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Concurrent;

namespace NoCompany.Data.Parsers
{
    public abstract class DataParserHandlerBase : ControlableExecutionBase, IDataParserHandler
    {
        private static ILog logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public override IViabilityObserver ViabilityObserver
        {
            get { return _viabilityObserver; }
            set
            {
                _viabilityObserver = value;
                if (Successor != null)
                    Successor.ViabilityObserver = _viabilityObserver;

                if (Failer != null)
                    Failer.ViabilityObserver =_viabilityObserver;
            }
        }
        
        private ParallelOptions _parallelOptions = null;
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
            logger.Debug($"Successor type - '{Successor}', Failer type is '{Failer}'.");
            ShouldStopOperating();
            IEnumerable<IChangeableData> data = TryParce(entryPoint);
            if (data == null && Failer != null)
            {
                data = Failer.Parce(entryPoint);
            }

            FillChildsInParallel(data, Successor);
            return data;
        }

        private void FillChildsInParallel(IEnumerable<IChangeableData> data, IDataParserHandler parser)
        {
            if (data == null || parser == null)
                return;

            var exceptions = new ConcurrentQueue<Exception>();
            Parallel.ForEach(data, ParallelOptions, item =>
            {
                try
                {
                    item.Childs = parser.Parce(item.Value);
                }
                catch (HtmlRoutineException ex)
                {
                    logger.Warn(ex.Message);
                }
                catch (StopOperatoinException stopEx)
                {
                    exceptions.Enqueue(stopEx);
                    throw new AggregateException(exceptions);
                }
                catch (Exception ex)
                {
                    exceptions.Enqueue(ex);
                }
            });

            if (exceptions.Any()) throw new AggregateException(exceptions);
        }

        protected virtual HtmlDocument LoadHtmlDocument(string url, Encoding encoding)
        {
            return HtmlDocumentLoader.LoadHtmlDocument(url, encoding);
        }

        public override void Cancel()
        {
            IsCancellationRequested = true;
            if(Successor != null)
                Successor.Cancel();

            if(Failer != null)
                Failer.Cancel();
        }

        protected virtual ParallelOptions ParallelOptions
        {
            get 
            {
                if(_parallelOptions == null)
                    _parallelOptions = new ParallelOptions() { MaxDegreeOfParallelism = MaxDegreeOfParallelism };
                return _parallelOptions;
            }
        }

        protected abstract IEnumerable<IChangeableData> TryParce(string entryPoint);
    }

}
