using System;
using System.Collections.Generic;
using System.Linq;
using NoCompany.Interfaces;
using CodeContracts;
using NoCompany.DataAnalyzer.Properties;

namespace NoCompany.DataAnalyzer
{
    public class DataAnalyzer : IDataAnalyzer
    {
        public event EventHandler<string> DetectedDifferenceEvent;
       
        public void Analyze(IEnumerable<IChangeableData> receivedDataSet, IEnumerable<IChangeableData> presavedDataSet)
        {
            if (receivedDataSet == null || presavedDataSet == null)
                return;
            //To exclude shitty duplicates in receivedDataSet
            var receivedDataDictionary = receivedDataSet.ToDictionary();
            
            //To avoid double foreach
            var presavedDataDictionary = presavedDataSet.ToDictionary();

            foreach (var receivedData in receivedDataDictionary)
            {
                IChangeableData preservedData = null;
                
                if (presavedDataDictionary.TryGetValue(receivedData.Key, out preservedData))
                {
                    if (String.Equals(preservedData.Value, receivedData.Value.Value, StringComparison.OrdinalIgnoreCase) == false)
                    {
                        RaiseEventAboutDifferences(Resources.Event_ItemsAreNotEqual, 
                                                    preservedData.Value, 
                                                    receivedData.Value.Value);
                    }
                    else
                    {
                        //Analyze sub Data
                        Analyze(receivedData.Value.Childs, preservedData.Childs);
                    }

                    presavedDataDictionary.Remove(receivedData.Key);
                    continue;
                }
                RaiseEventAboutDifferences(Resources.Event_NewInfo, receivedData.Key, receivedData.Value.Value);
            }

            ReportAboutOutdatedItems(presavedDataDictionary);
        }

        private void ReportAboutOutdatedItems(IEnumerable<KeyValuePair<string,IChangeableData>> outdatedItems)
        {
            outdatedItems.All(x =>
            {
                RaiseEventAboutDifferences(Resources.Event_DataIsNotActual, x.Key, x.Value.Value);
                return true;
            });
        }

        private void RaiseEventAboutDifferences(string format, string arg0, string arg1)
        {
            if (DetectedDifferenceEvent != null)
            {
                string message = String.Format(format, arg0, arg1);
                DetectedDifferenceEvent(this, message);
            }
        }
    }
}
