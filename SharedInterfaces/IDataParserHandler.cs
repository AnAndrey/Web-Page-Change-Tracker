using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NoCompany.Interfaces
{
    public interface IDataParserHandler
    {
        IDataParserHandler Successor { get; set; }
        IDataParserHandler Failer { get; set; }

        IEnumerable<IChangeableData> Parce(string entryPoint);
    }
}
