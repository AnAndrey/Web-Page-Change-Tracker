using System;
using System.Collections.Generic;
using System.Linq;
using NoCompany.Interfaces;
using CodeContracts;
using GenesisTrialTest.Properties;
using log4net;
using System.Reflection;

namespace GenesisTrialTest
{
    public class ChangesNotifierFacade
    {
        public static ILog logger = LogManager.GetLogger(typeof(ChangesNotifierFacade)); 

        private List<string> listOfChanges = new List<string>();
        private List<string> listOfErrors = new List<string>();
        private int _defaultTimeOut = 30 * 1000;
        public IDataAnalyzer Analyzer { get; private set; }
        public IDataProvider ExternalSource { get; private set; }
        public IDataStorageProvider DataStorage { get; private set; }
        
        public INotificationManager Notificator { get; private set; }

        public ChangesNotifierFacade(IDataAnalyzer analyzer, 
                                   IDataProvider externalSource, 
                                   IDataStorageProvider dataStorage, 
                                   INotificationManager notificator)
        {
            Requires.NotNull(analyzer, "analyzer");
            Requires.NotNull(externalSource, "externalSource");
            Requires.NotNull(dataStorage, "dataStorage");
            Requires.NotNull(notificator, "notificator");

            Analyzer = analyzer;
            Analyzer.DetectedDifferenceEvent += Analyzer_DetectedDifferenceEvent;

            ExternalSource = externalSource;
            DataStorage = dataStorage;
            Notificator = notificator;
        }

        public void FindAndNotify()
        {
            logger.Debug(MethodBase.GetCurrentMethod().Name);

            var receivedData = GetExternalData();

            var presavedData = DataStorage.GetData();

            Assumes.True(receivedData != null && receivedData.Any(), Resources.Error_LoadExternalData);
            try
            {
                if (presavedData != null)
                {
                    Analyzer.Analyze(receivedData, presavedData);
                }
                else
                {
                    logger.InfoFormat(Resources.Info_NoPreservedData);

                }
            }
            catch (System.Net.Mail.SmtpException ex)
            {
                logger.Error(ex);
            }
            finally
            {
                PostActions(receivedData);
            }
        }

        protected void PostActions(IEnumerable<IChangeableData> receivedData)
        {
            logger.InfoFormat(Resources.Info_CountOfChanges, listOfChanges.Count);

            if (listOfChanges.Any())
            {
                Notificator.NotifyAbout(listOfChanges);
                DataStorage.CleanStorage();
                DataStorage.SaveData(receivedData);
            }
        }

        protected virtual void Notify()
        {
            logger.InfoFormat(Resources.Info_CountOfChanges, listOfChanges.Count);

            if (listOfChanges.Any())
            {
                logger.Debug(MethodBase.GetCurrentMethod().Name);
                Notificator.NotifyAbout(listOfChanges);
            }
        }

        protected virtual void Analyzer_DetectedDifferenceEvent(object sender, string e)
        {
            if (!String.IsNullOrEmpty(e))
            {
                logger.Debug(e);
                listOfChanges.Add(e);
            }
        }

        protected virtual IEnumerable<IChangeableData> GetExternalData()
        {
            logger.Debug(MethodBase.GetCurrentMethod().Name);
            logger.InfoFormat(Resources.Info_TimeOut, OperationHangTimeOut);

            using (HangWatcher watcher = new HangWatcher(OperationHangTimeOut))
            {
                watcher.Token.Register(() => ExternalSource.Cancel());
                ExternalSource.ViabilityObserver.SomeBodyStillAlive += (o, e) => watcher.PostPone(OperationHangTimeOut);
                
                return ExternalSource.GetData();
            }
        }

        /// <summary>
        /// Limits a time consumable operation in Milliseconds. 
        /// If time is out the OperationCanceledException will be raised.
        /// </summary>
        public int OperationHangTimeOut
        {
            get { return _defaultTimeOut; }
            set
            {
                _defaultTimeOut = value;
                logger.InfoFormat(Resources.Info_TimeOutChange, value);

            }
        }
    }
}
