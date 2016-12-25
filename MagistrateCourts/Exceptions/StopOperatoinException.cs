using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NoCompany.Data
{
    public class StopOperatoinException:Exception
    {
        public StopOperatoinException(string message) : base(message) { }
        public StopOperatoinException() { }
    }
}
