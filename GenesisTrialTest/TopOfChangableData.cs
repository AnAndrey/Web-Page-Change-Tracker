using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharedInterfaces;
namespace GenesisTrialTest
{
    public class TopOfChangableData : ChangeableData
    {
        public TopOfChangableData() : base("root", "root")
        {
            base.GroupName = "root";
        }

    }
}
