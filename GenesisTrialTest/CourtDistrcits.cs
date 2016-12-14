using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharedInterfaces;

namespace GenesisTrialTest
{
    public class CourtDistrcits : ChangeableData
    {
        public CourtDistrcits(string name, string site):base(name, site)
        {
             GroupName = this.GetType().Name;
        }
    }
}
