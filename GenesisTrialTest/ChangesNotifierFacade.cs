using System;
using System.Collections.Generic;
using System.Linq;
using NoCompany.Interfaces;

namespace GenesisTrialTest
{
    public class ChangesNotifierFacade
    {
        private List<string> listOfChanges = new List<string>();
        private List<string> listOfErrors = new List<string>();

        public IDataAnalyzer Analyzer { get; private set; }
        public IDataFetcher ExternalSource { get; private set; }
        public IDataStorageProvider DataStorage { get; private set; }
        
        public INotificationManager Notificator { get; private set; }

        public ChangesNotifierFacade(IDataAnalyzer analyzer, 
                                   IDataFetcher externalSource, 
                                   IDataStorageProvider dataStorage, 
                                   INotificationManager notificator)
        {
            Analyzer = analyzer;
            Analyzer.DetectedDifferenceEvent += Analyzer_DetectedDifferenceEvent;
            Analyzer.ErrorEvent += Analyzer_ErrorEvent;

            ExternalSource = externalSource;
            DataStorage = dataStorage;
            Notificator = notificator;
        }

        protected virtual void Notify()
        {
            if(listOfChanges.Any())
                Notificator.NotifyAbout(listOfChanges);
        }

        protected virtual void Analyzer_ErrorEvent(object sender, string e)
        {
            if(!String.IsNullOrEmpty(e))
                listOfErrors.Add(e);
        }

        protected virtual void Analyzer_DetectedDifferenceEvent(object sender, string e)
        {
            if (!String.IsNullOrEmpty(e))
                listOfChanges.Add(e);
        }

        public void FindAndNotify()
        {
            //Get Freash data
            var receivedData = ExternalSource.GetData();

            //Get old data
            var presavedData = DataStorage.GetData();

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

    }
}
