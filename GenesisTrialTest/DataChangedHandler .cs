using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenesisTrialTest
{
    public class DataChangedEventArgs:EventArgs
    {
        public string Message { get; private set; }
        public DataCondition State { get; private set; }
        public DataChangedEventArgs(string message, DataCondition state)
        {
            Message = message;
            State = state;
        }

        public DataChangedEventArgs(string format, string arg0, string arg1, DataCondition state)
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
