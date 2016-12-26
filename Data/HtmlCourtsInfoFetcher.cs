using System;
using System.Collections.Generic;
using NoCompany.Interfaces;
using log4net;
using System.Reflection;
using CodeContracts;

namespace NoCompany.Data
{
    public class HtmlCourtsInfoFetcher : ControlableExecutionBase, IDataProvider
    {
        public static ILog logger = LogManager.GetLogger(typeof(HtmlCourtsInfoFetcher));
        private const string _sudRF = "https://sudrf.ru";

        public IDataParserHandler Parser { get; private set; }
        private Action CacelOperation { get; set; }

        public override IViabilityObserver ViabilityObserver
        {
            get
            {
                if (_viabilityObserver == null)
                    _viabilityObserver = new ViabilityObserver();
                return _viabilityObserver;
            }

            set
            {
                _viabilityObserver = value;
            }
        }

        public HtmlCourtsInfoFetcher(IDataParserHandler parser)
        {
            Requires.NotNull(parser, "parser");
            Parser = parser;

            Parser.ViabilityObserver = ViabilityObserver;
            
            CacelOperation = () => Parser.Cancel();
        }

        public IEnumerable<IChangeableData> GetData()
        {
            logger.Debug(MethodBase.GetCurrentMethod().Name);

            Parser.ViabilityObserver = ViabilityObserver;

            return Parser.Parce(_sudRF);
        }

        public override void Cancel()
        {
            base.Cancel();
            CacelOperation();
        }
    }
}
