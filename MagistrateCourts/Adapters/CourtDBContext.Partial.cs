using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MagistrateCourts
{
    public partial class CourtDBContext
    {
        public CourtDBContext(string connectionString) : base(connectionString){ }
    }
}
