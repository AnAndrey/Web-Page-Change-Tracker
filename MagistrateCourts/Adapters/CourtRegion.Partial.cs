using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NoCompany.Interfaces;

namespace NoCompany.Data
{
    public partial class CourtRegion : IChangeableData
    {
        public CourtRegion(string name, string number)
        {
            Name = name;
            Number = number;
        }
        public IEnumerable<IChangeableData> Childs
        {
            get
            {
                return CourtDistricts;
            }

            set
            {
                CourtDistricts = value.Cast<CourtDistrict>().ToList() ;
            }
        }

        public bool HasChilds
        {
            get
            {
                return CourtDistricts == null? false : CourtDistricts.Any();
            }
        }

        public string Value
        {
            get
            {
                return Number;
            }
        }
    }
}
