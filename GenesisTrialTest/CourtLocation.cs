using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharedInterfaces;

namespace GenesisTrialTest
{
    public class CourtLocation : ChangeableData
    {
        public CourtLocation(string location, string address):base(location, address)
        {
            GroupName = this.GetType().Name;
        }
    }
}
