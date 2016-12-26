using System;
using System.Collections.Generic;
using System.Linq;
using NoCompany.Interfaces;
namespace NoCompany.Data
{
    public partial class CourtDistrict : IChangeableData
    {
        public CourtDistrict(string name, string webSite)
        {
            Name = name;
            WebSite = webSite;
        }
        public IEnumerable<IChangeableData> Childs
        {
            get
            {
                return CourtLocations;
            }

            set
            {
                CourtLocations = value.Cast<CourtLocation>().ToList();
            }
        }

        public bool HasChilds
        {
            get
            {
                return CourtLocations == null ? false : CourtLocations.Any();
            }
        }

        public string Value
        {
            get
            {
                return WebSite;
            }
        }
    }
}
