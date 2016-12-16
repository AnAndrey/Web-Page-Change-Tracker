using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharedInterfaces;
using System.IO;
using System.Data;

namespace SimpleDataAnalyzer
{
    public class DataAnalyzer : IDataAnalyzer
    {
        public event EventHandler DetectedDifferenceEvent;
        public event EventHandler<string> ErrorEvent;
       
        public void Analyze(IEnumerable<ChangeableData> receivedDataSet, IEnumerable<ChangeableData> presavedDataSet)
        {
            if (receivedDataSet == null || presavedDataSet == null)
                return;

            //To exclude shitty duplicates in receivedDataSet
            var receivedDataDictionary = receivedDataSet.ToDictionary();
            
            //To avoid double foreach
            var presavedDataDictionary = presavedDataSet.ToDictionary();

            foreach (var receivedData in receivedDataDictionary)
            {
                ChangeableData preservedData = null;
                
                if (presavedDataDictionary.TryGetValue(receivedData.Key, out preservedData))
                {
                    if (String.Equals(preservedData.Value, receivedData.Value.Value, StringComparison.OrdinalIgnoreCase) == false)
                    {
                        RaiseEventAboutDifferences("Stored value '{0}' and retrieved value '{1}' are different", 
                                                    preservedData.Value, 
                                                    receivedData.Value.Value,
                                                    DataCondition.Modified);
                    }
                    else
                    {
                        //Analyze sub Data
                        Analyze(receivedData.Value.Childs, preservedData.Childs);
                    }

                    presavedDataDictionary.Remove(receivedData.Key);
                    continue;
                }
                RaiseEventAboutDifferences("Found new info! Name '{0}' and value '{1}'.", 
                                            receivedData.Key, 
                                            receivedData.Value.Value, 
                                            DataCondition.SomeThingNew);
            }

            ReportAboutOutdatedItems(presavedDataDictionary);
        }

        private void ReportAboutOutdatedItems(IEnumerable<KeyValuePair<string,ChangeableData>> outdatedItems)
        {
            outdatedItems.All(x =>
            {
                RaiseEventAboutDifferences("Not actual data: Name '{0}' and value '{1}'.", 
                                            x.Key, 
                                            x.Value.Value, 
                                            DataCondition.Outdated);
                return true;
            });
        }

        private void RaiseEventAboutDifferences(string format, string arg0, string arg1, DataCondition state)
        {
            if(DetectedDifferenceEvent != null)
                DetectedDifferenceEvent(this, new DataChangedEventArgs(format, arg0, arg1, state));
        }
    }

    internal static class ChageableDataExtensions
    {
        internal static Dictionary<string, ChangeableData> ToDictionary(this IEnumerable<ChangeableData> dataSet)
        {
            var dictionary = new Dictionary<string, ChangeableData>();
            foreach (ChangeableData item in dataSet)
            {
                ChangeableData tempValue = null;
                if (!dictionary.TryGetValue(item.Name, out tempValue))
                {
                    dictionary.Add(item.Name, item);
                }
            }

            return dictionary;
        }
    }
}
