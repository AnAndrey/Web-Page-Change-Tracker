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
            var changes = r.GetData();

            string rootTableName = changes.First().GetType().Name;
            var presavedData = _dataFetcher.GetData();

            //GetStorageData
            AnalyzeDifferences(changes, presavedData);
            //return;
            // Clear All old data
            SqlDataPreserver preserve = new SqlDataPreserver(null);

            preserve.ClearTable(rootTableName);
            // Save new data
            preserve.Save(changes);
        }

        private void AnalyzeDifferences(IEnumerable<ChangeableData> receivedData, IEnumerable<ChangeableData> presavedData)
        {
            var presavedDataDictionary = presavedData.ToDictionary();

            foreach (ChangeableData data in receivedData)
            {
                string val = string.Empty;
                //if key is present
                if (presavedDataDictionary.TryGetValue(data.Name, out val))
                {
                    if (String.Equals(val, data.Value, StringComparison.OrdinalIgnoreCase) == false)
                    {
                        ReportAboutChages("Stored value - '{0}' and retrieved value - '{1}' are different", val, data.Value, DataCondition.Modified);
                    }
                    else
                    {
                        //Analyze sub Data
                        //AnalyzeDifferences(data.Childs, data.Childs.First().GetType().Name);
                    }
                    
                    

                    presavedDataDictionary.Remove(data.Value);
                    continue;
                }

                ReportAboutChages("Found new info! Name - '{0}' and value - '{1}'.", data.Name, data.Value, DataCondition.SomeThingNew);
            }

            ReportAboutOutdatedItems(presavedDataDictionary);
        }

        private void ReportAboutOutdatedItems(IEnumerable<KeyValuePair<string,string>> outdatedItems)
        {
            outdatedItems.All(x =>
            {
                ReportAboutChages("Not actual data: Name - '{0}' and value - '{1}'.", x.Key, x.Value, DataCondition.Outdated);
                return true;
            });
        }

        private void ReportAboutChages(string format, string name, string value, DataCondition state)
        {
            ChangeHasDetectedEvent(this, 
                new DataChangedHandler(format, name, value, state));
        }
    }

    internal static class ChageableDataExtensions
    {
        internal static Dictionary<string, string> ToDictionary(this IEnumerable<ChangeableData> dataSet)
        {
            Dictionary<string, string> dictionary = new Dictionary<string, string>();
            foreach (ChangeableData item in dataSet)
            {
                string tempValue = String.Empty;
                if (!dictionary.TryGetValue(item.Value, out tempValue))
                {
                    dictionary.Add(item.Value, item.Name);
                }
            }

            return dictionary;
        }
    }
}
