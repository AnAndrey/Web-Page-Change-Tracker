using NoCompany.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NoCompany.DataAnalyzer
{
    internal static class ChageableDataExtensions
    {
        internal static Dictionary<string, IChangeableData> ToDictionary(this IEnumerable<IChangeableData> dataSet)
        {
            var dictionary = new Dictionary<string, IChangeableData>();
            foreach (IChangeableData item in dataSet)
            {
                IChangeableData tempValue = null;
                if (!dictionary.TryGetValue(item.Name, out tempValue))
                {
                    dictionary.Add(item.Name, item);
                }
            }

            return dictionary;
        }
    }
}
