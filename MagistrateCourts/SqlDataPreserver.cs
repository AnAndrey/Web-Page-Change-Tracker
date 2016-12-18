using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharedInterfaces;
using System.Data.SqlClient;
using System.Data;
using Z.EntityFramework.Extensions;

namespace MagistrateCourts
{
    public class SqlDataPreserver : IDataPreserver
    {
        public void CleanStorage()
        {
            using (CourtDBContext dbContext = new CourtDBContext())
            {
                dbContext.BulkDelete(dbContext.CourtRegions);
                //dbContext.CourtRegions.RemoveRange(null);
                //dbContext.SaveChanges();
            }
        }

        public void SaveData(IEnumerable<IChangeableData> data)
        {

            using (CourtDBContext dbContext = new CourtDBContext())
            {
                
                dbContext.BulkInsert(data.Cast<CourtRegion>());
                InsertDistrictsByRegion(dbContext, data);
            }
        }

        private void InsertDistrictsByRegion(CourtDBContext dbContext, IEnumerable<IChangeableData> data)
        {
            var regionDictionary = dbContext.CourtRegions.ToDictionary(x => x.Name);
            foreach (var item in data)
            {
                CourtRegion region = null;
                if (item.Childs != null && regionDictionary.TryGetValue(item.Name, out region))
                {
                    var districts = item.Childs.Select<IChangeableData, CourtDistrict>(x =>
                    {
                        CourtDistrict district = (CourtDistrict)x;
                        district.RegionId = region.Id;
                        return district;
                    });
                    dbContext.BulkInsert(districts);

                    var savedDistricts = dbContext.CourtDistricts.Where(x => x.RegionId == region.Id);
                    InsertLocationsByDistrict(dbContext, savedDistricts, districts);

                }
            }
        }

        private void InsertLocationsByDistrict(CourtDBContext dbContext, IQueryable<CourtDistrict> districts, IEnumerable<IChangeableData> data)
        {
            var districtDictionary = districts.ToDictionary(x => x.Name);

            foreach (var item in data)
            {
                CourtDistrict district = null;
                if (item.Childs != null && districtDictionary.TryGetValue(item.Name, out district))
                {
                    var locations = item.Childs.Select<IChangeableData, CourtLocation>(x =>
                    {
                        CourtLocation location = (CourtLocation)x;
                        location.DistrictId = district.Id;
                        return location;
                    });

                    dbContext.BulkInsert(locations);
                }
            }

        }
    }
}
