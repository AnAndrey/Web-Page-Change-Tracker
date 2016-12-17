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
    public class SqlDataPreserver : IDataPreserver
    {
        public void CleanStorage()
        {
            using (CourtDBContext dbContext = new CourtDBContext())
            {
                dbContext.CourtLocations.RemoveRange(dbContext.CourtLocations);
                dbContext.CourtDistricts.RemoveRange(dbContext.CourtDistricts);
                dbContext.CourtRegions.RemoveRange(dbContext.CourtRegions);

                dbContext.SaveChanges();
            }
        }

        public void SaveData(IEnumerable<IChangeableData> data)
        {
            using (CourtDBContext dbContext = new CourtDBContext())
            {
                dbContext.CourtRegions.AddRange(data.Cast<CourtRegion>());
                dbContext.SaveChanges();
            }
        }
    }
}
