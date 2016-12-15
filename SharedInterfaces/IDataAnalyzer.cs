using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedInterfaces
{
    public interface IDataAnalyzer
    {
        event EventHandler<string> ErrorEvent;
        event EventHandler DetectedDifferenceEvent;
        void Analyze(IEnumerable<ChangeableData> a, IEnumerable<ChangeableData> b);
    }
}
