using System;
using System.Collections.Generic;
using System.Linq;
using NoCompany.Interfaces;
using CodeContracts;
using GenesisTrialTest.Runner.Properties;
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

        public void FindAndNotify()
        {
            //Get Freash data
            var receivedData = ExternalSource.GetData();
            Assumes.True(receivedData != null && receivedData.Any(), Resources.Error_LoadExternalData) ;

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
