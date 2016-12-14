using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenesisTrialTest
{
    public class DataChangedHandler:EventArgs
    {
        public string Message { get; private set; }
        public DataCondition State { get; private set; }
        public DataChangedHandler(string message, DataCondition state)
        {
            Message = message;
            State = state;
        }

        public DataChangedHandler(string format, string arg0, string arg1, DataCondition state)
        {
            Message = String.Format(format, arg0, arg1);
            State = state;
        }
    }

    public enum DataCondition
    {
        NotFound,
        Modified,
        Deleted,
        Outdated,
        SomeThingNew
    }
}
