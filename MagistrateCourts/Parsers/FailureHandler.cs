using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NoCompany.Interfaces;

namespace NoCompany.Data.Parsers
{
    public class FailureHandler : IDataParserHandler
    {
        public IDataParserHandler Failer
        {
            get
            {
                throw new NotImplementedException();
            }

            set
            {
                throw new NotImplementedException();
            }
        }

        public IDataParserHandler Successor
        {
            get
            {
                throw new NotImplementedException();
            }

            set
            {
                throw new NotImplementedException();
            }
        }

        public IEnumerable<IChangeableData> Parce(string entryPoint)
        {
            throw new HtmlRoutineException("Have no idea how to parse an entry point - '{0}'. Try to add appropriate parser.", entryPoint);
        }
    }
}
