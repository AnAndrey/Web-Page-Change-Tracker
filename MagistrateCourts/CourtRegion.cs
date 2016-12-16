using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharedInterfaces;

namespace MagistrateCourts
{
    public class CourtRegion: ChangeableData
    {

        public CourtRegion(string name, string number):base(name, number)
        {
            GroupName = this.GetType().Name;
        }
     }
}
