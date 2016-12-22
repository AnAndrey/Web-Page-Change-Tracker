using System;
using System.Collections.Generic;
using System.Linq;
using NoCompany.Interfaces;
using CodeContracts;
using GenesisTrialTest.Properties;

namespace GenesisTrialTest
{

    internal class MyException : Exception
    {
        internal MyException(string message) : base(message)
        {

        }
    }
    public class ChangesNotifierFacade
    {
        private List<string> listOfChanges = new List<string>();
        private List<string> listOfErrors = new List<string>();
        private const int s_defaultTimeOut = 30 * 1000;
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

            OperationHangTimeOut = s_defaultTimeOut;
        }

        protected virtual void Notify()
        {
            if(listOfChanges.Any())
                Notificator.NotifyAbout(listOfChanges);
        }

        protected virtual void Analyzer_DetectedDifferenceEvent(object sender, string e)
        {
            if (!String.IsNullOrEmpty(e))
                listOfChanges.Add(e);
        }

        protected virtual IEnumerable<IChangeableData> GetExternalData()
        {
            using (HangWatcher watcher = new HangWatcher(OperationHangTimeOut))
            {
                ExternalSource.ImStillAlive += (o, e) =>
                {
                    watcher.PostPone(OperationHangTimeOut);
                    Console.WriteLine("NEw event - '{0}'.", e);
                };

                //Get Fresh data

                return ExternalSource.GetData(watcher.Token);
            }
        }

        /// <summary>
        /// Limits a time consumable operation in Milliseconds. 
        /// If time is out the OperationCanceledException will be raised.
        /// </summary>
        public int OperationHangTimeOut { get; set; }
        public void FindAndNotify()
        {
           
            var receivedData = GetExternalData();

            var presavedData = DataStorage.GetData();

            Assumes.True(receivedData != null && receivedData.Any(), Resources.Error_LoadExternalData) ;

            //Get old data

            if (presavedData != null)
            {
                Analyzer.Analyze(receivedData, presavedData);
                Notify();
            }

            // Clear All old data
            DataStorage.CleanStorage();
            // Save new data
            DataStorage.SaveData(receivedData);
        }

        private void ExternalSource_ImStillAlive(object sender, string e)
        {
            throw new NotImplementedException();
        }
    }
}
