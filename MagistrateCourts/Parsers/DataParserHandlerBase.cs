using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NoCompany.Interfaces;

namespace NoCompany.Data.Parsers
{
    public abstract class DataParserHandlerBase : IDataParserHandler
    {

        public DataParserHandlerBase(IDataParserHandler locationsParser, IDataParserHandler failureHandler)
        {
            Successor = locationsParser;
            Failer = failureHandler;
        }

        DataParserHandlerBase()
        {

        }
        public IDataParserHandler Failer{get;set;}

        public IDataParserHandler Successor { get; set; }

        public virtual IEnumerable<IChangeableData> Parce(string entryPoint)
        {
            var data = TryParce(entryPoint);
            if (data == null)
            {
                Failer.Parce(entryPoint);
            }
            else
            {
                foreach (var item in data)
                {
                    item.Childs = Successor.Parce(item.Value);
                }
            };
            return data;
        }

        protected abstract List<IChangeableData> TryParce(string entryPoint);
    }
}
