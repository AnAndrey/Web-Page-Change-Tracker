using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NoCompany.Data
{
    public class HtmlRoutineException:Exception
    {
        private string _string;
        public HtmlRoutineException(string message) : base(message)
        {
            
        }

        public HtmlRoutineException(string format, params object[] args)
        {
            _string = String.Format(format, args);
        }

        public override string Message { get { return _string; }  }

    }
}
