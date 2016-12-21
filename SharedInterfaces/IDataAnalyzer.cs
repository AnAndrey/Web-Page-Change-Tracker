using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NoCompany.Interfaces
{
    /// <summary>
    ///     Allows an object to implement a DataAnalyzer, and represents a method
    ///     that are used to analyze two data sets.
    /// </summary>
    public interface IDataAnalyzer
    {
        event EventHandler<string> DetectedDifferenceEvent;
        void Analyze(IEnumerable<IChangeableData> a, IEnumerable<IChangeableData> b);
    }
}
