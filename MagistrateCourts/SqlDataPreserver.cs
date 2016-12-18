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
            }
        }

        public void SaveData(IEnumerable<IChangeableData> data)
        {
            Dictionary<string, CourtRegion> regionDictionary = null;
            using (CourtDBContext dbContext = new CourtDBContext())
            {
                dbContext.BulkInsert(data.Cast<CourtRegion>());
                regionDictionary = dbContext.CourtRegions.ToDictionary(x => x.Name);
            }
            InsertDistrictsByRegion(regionDictionary, data);
        }

        private void InsertDistrictsByRegion(Dictionary<string, CourtRegion> regionsSaved, IEnumerable<IChangeableData> regionsRaw)
        {
            foreach (var itemRaw in regionsRaw)
            {
                CourtRegion region = null;
                if (itemRaw.Childs != null && regionsSaved.TryGetValue(itemRaw.Name, out region))
                {
                    var districtsRaw = itemRaw.Childs.Select(x =>
                    {
                        CourtDistrict district = (CourtDistrict)x;
                        district.RegionId = region.Id;
                        return district;
                    });

                    Dictionary<string, CourtDistrict> savedDistricts = null;
                    using (CourtDBContext dbContext = new CourtDBContext())
                    {
                        dbContext.BulkInsert(districtsRaw);
                        savedDistricts = dbContext.CourtDistricts.Where(x => x.RegionId == region.Id)
                                                                 .ToDictionary(x=>x.Name);
                    }
                    InsertLocationsByDistrict(savedDistricts, districtsRaw);
                }
            }
        }

        private void InsertLocationsByDistrict(Dictionary<string, CourtDistrict> districtsSaved, IEnumerable<IChangeableData> districtsRaw)
        {
            foreach (var itemRaw in districtsRaw)
            {
                CourtDistrict district = null;
                if (itemRaw.Childs != null && districtsSaved.TryGetValue(itemRaw.Name, out district))
                {
                    var locations = itemRaw.Childs.Select(x =>
                    {
                        CourtLocation location = (CourtLocation)x;
                        location.DistrictId = district.Id;
                        return location;
                    });

                    using (CourtDBContext dbContext = new CourtDBContext())
                        dbContext.BulkInsert(locations);
                }
            }

        }
    }
}
