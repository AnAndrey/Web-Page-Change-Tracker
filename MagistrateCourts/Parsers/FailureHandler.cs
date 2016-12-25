using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NoCompany.Interfaces;

namespace NoCompany.Data.Parsers
{
    public class FailureHandler : DataParserHandlerBase
    {
        public override IEnumerable<IChangeableData> Parce(string entryPoint)
        {
            return GiveUp(entryPoint);
        }

        protected override List<IChangeableData> TryParce(string entryPoint)
        {
            return GiveUp(entryPoint);
        }

        protected List<IChangeableData> GiveUp(string entryPoint)
        {
            throw new HtmlRoutineException("Have no idea how to parse an entry point - '{0}'. Try to add appropriate parser.", entryPoint);
        }
    }
}
