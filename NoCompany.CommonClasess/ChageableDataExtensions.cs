using NoCompany.Interfaces;
using System;
using System.Collections.Generic;

namespace NoCompany.CommonClasess
{
    public static class ChageableDataExtensions
    {
        public static Dictionary<string, T> ToDictionary<T>(this IEnumerable<T> dataSet, Action<T> actOnDuplicates = null) where T : IChangeableData
        {
            var nonDuplicated = new Dictionary<string, T>();
            foreach (T item in dataSet)
            {
                T tempValue = default(T);
                if (!nonDuplicated.TryGetValue(item.Name, out tempValue))
                {
                    nonDuplicated.Add(item.Name, item);
                }
                else if (actOnDuplicates != null)
                {
                    actOnDuplicates(tempValue);
                }
            }

            return nonDuplicated;
        }
    }
}
