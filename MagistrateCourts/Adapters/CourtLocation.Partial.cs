using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharedInterfaces;

namespace MagistrateCourts
{
    public partial class CourtLocation : IChangeableData
    {
        public CourtLocation(string name, string address)
        {
            Name = name;
            Address = address;
        }

        public CourtLocation()
        {
        }
        public IEnumerable<IChangeableData> Childs
        {
            get
            {
                return null;
            }

            set
            {
                throw new NotImplementedException();
            }
        }

        public bool HasChilds
        {
            get
            {
                return false;
            }
        }

        public string Value
        {
            get
            {
                return Address;
            }
        }
    }
}
