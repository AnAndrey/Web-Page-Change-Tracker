using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NoCompany.Interfaces
{
    public interface IDataAnalyzer
    {
        event EventHandler<string> ErrorEvent;
        event EventHandler<string> DetectedDifferenceEvent;
        void Analyze(IEnumerable<IChangeableData> a, IEnumerable<IChangeableData> b);
    }
}
