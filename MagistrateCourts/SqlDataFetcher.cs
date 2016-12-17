using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharedInterfaces;
using System.Data.SqlClient;
using System.Data;

namespace MagistrateCourts
{
    public class SqlDataFetcher : IDataFetcher
    {
        public IEnumerable<IChangeableData> GetData()
        {
            IEnumerable<IChangeableData> data = null;
            using (CourtDBContext dbContext = new CourtDBContext())
            {
                data = dbContext.CourtRegions.Include("CourtDistricts").Include("CourtDistricts.CourtLocations").ToList();
            }

            return data;
        }
    }
}

