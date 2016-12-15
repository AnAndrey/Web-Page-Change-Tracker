using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharedInterfaces;
using System.IO;
using System.Data;

namespace GenesisTrialTest
{
    class HtmlChangeDetector : IChangeDetector
    {
        public event EventHandler ChangeHasDetectedEvent;
        public event EventHandler<string> ErrorEvent;

        IDataFetcher _dataFetcher = new SqlDataFetcher("CourtRegion");
        public void FindChanges()
        {
            //GetHmlData
            HtmlChangeableDataFetcher r = new HtmlChangeableDataFetcher();
            var receivedData = r.GetData();

            string rootTableName = receivedData.First().GetType().Name;
            var presavedData = _dataFetcher.GetData();

            //GetStorageData
            AnalyzeDifferences(receivedData, presavedData);
            //return;
            // Clear All old data
            SqlDataPreserver preserve = new SqlDataPreserver(null);

            int t = preserve.ClearTable(rootTableName);
            // Save new data
            preserve.Save(receivedData);
        }

        private void AnalyzeDifferences(IEnumerable<ChangeableData> receivedDataSet, IEnumerable<ChangeableData> presavedDataSet, ChangeableData parent = null)
        {
            if (receivedDataSet == null)
                return;

            if (presavedDataSet == null)
            {
                presavedDataSet.All(x =>
                {
                    ReportAboutChages("Found new info! Name '{0}' and value '{1}'.", x.Name, x.Value, DataCondition.SomeThingNew);
                    return true;
                });
                return;
            }

            //To exclude shitty duplicates in receivedDataSet
            var receivedDataDictionary = receivedDataSet.ToDictionary();
            
            var presavedDataDictionary = presavedDataSet.ToDictionary(parent);

            foreach (var receivedData in receivedDataDictionary)
            {
                ChangeableData preservedData = null;
                //if key is present
                if (presavedDataDictionary.TryGetValue(receivedData.Key, out preservedData))
                {
                    if (String.Equals(preservedData.Value, receivedData.Value.Value, StringComparison.OrdinalIgnoreCase) == false)
                    {
                        ReportAboutChages("Stored value '{0}' and retrieved value '{1}' are different", preservedData.Value, receivedData.Value.Value, DataCondition.Modified);
                    }
                    else
                    {
                        //Analyze sub Data
                        AnalyzeDifferences(receivedData.Value.Childs, preservedData.Childs, receivedData.Value);
                    }
                    presavedDataDictionary.Remove(receivedData.Key);
                    continue;
                }
                ReportAboutChages("Found new info! Name '{0}' and value '{1}'.", receivedData.Key, receivedData.Value.Value, DataCondition.SomeThingNew);
            }

            ReportAboutOutdatedItems(presavedDataDictionary);
        }

        private void ReportAboutOutdatedItems(IEnumerable<KeyValuePair<string,ChangeableData>> outdatedItems)
        {
            outdatedItems.All(x =>
            {
                ReportAboutChages("Not actual data: Name '{0}' and value '{1}'.", x.Key, x.Value.Value, DataCondition.Outdated);
                return true;
            });
        }

        private void ReportAboutChages(string format, string name, string value, DataCondition state)
        {
            if(ChangeHasDetectedEvent != null)
                ChangeHasDetectedEvent(this, new DataChangedEventArgs(format, name, value, state));
        }
    }

    internal static class ChageableDataExtensions
    {
        internal static Dictionary<string, ChangeableData> ToDictionary(this IEnumerable<ChangeableData> dataSet, ChangeableData parent = null)
        {
            var dictionary = new Dictionary<string, ChangeableData>();
            foreach (ChangeableData item in dataSet)
            {
                ChangeableData tempValue = null;
                if (!dictionary.TryGetValue(item.Name, out tempValue))
                {
                    dictionary.Add(item.Name, item);
                }
                else
                {
                    if(parent == null)
                        Console.WriteLine("Ignore duplicates: name '{0}', value '{1}'.", item.Name, item.Value);
                    else 
                        if (String.IsNullOrEmpty(item.Name) ==false)
                        Console.WriteLine("Ignore duplicates: name '{0}', value '{1}', parent name '{2}', parent value '{3}'.", item.Name, item.Value, parent.Name, parent.Value);
                }
            }

            return dictionary;
        }
    }
}
