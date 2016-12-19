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
            InsertAllRegions(data);
        }

        private void InsertAllRegions(IEnumerable<IChangeableData> regionsRaw)
        {
            Dictionary<string, CourtRegion> regionDictionary = null;
            using (CourtDBContext dbContext = new CourtDBContext())
            {
                dbContext.BulkInsert(regionsRaw.Cast<CourtRegion>());
                regionDictionary = dbContext.CourtRegions.ToDictionary(x => x.Name);
            }
            InsertAllDistricts(regionDictionary, regionsRaw);
        }


        private void InsertAllDistricts(Dictionary<string, CourtRegion> regionsSaved, IEnumerable<IChangeableData> regionsRaw)
        {
            List<CourtDistrict> districtsWithParentId = SetParentIdToDistricts(regionsSaved, regionsRaw);

            Dictionary<string, CourtDistrict> savedDistricts = null;
            using (CourtDBContext dbContext = new CourtDBContext())
            {
                dbContext.BulkInsert(districtsWithParentId);
                savedDistricts = dbContext.CourtDistricts.ToDictionary(x => x.Name);
            }
            InsertLocationsByDistrict(savedDistricts, districtsWithParentId);
        }

        private List<CourtDistrict> SetParentIdToDistricts(Dictionary<string, CourtRegion> regionsSaved, IEnumerable<IChangeableData> regionsRaw)
        {
            List<CourtDistrict> districtsWithParentId = new List<CourtDistrict>();
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

                    districtsWithParentId.AddRange(districtsRaw);
                }
            }

            return districtsWithParentId;
        }

        private void InsertLocationsByDistrict(Dictionary<string, CourtDistrict> districtsSaved, IEnumerable<IChangeableData> districtsRaw)
        {
            List<CourtLocation> locationsToSave = SetParentIdToLocations(districtsSaved, districtsRaw);

            using (CourtDBContext dbContext = new CourtDBContext())
                dbContext.BulkInsert(locationsToSave);
        }

        private List<CourtLocation> SetParentIdToLocations(Dictionary<string, CourtDistrict> districtsSaved, IEnumerable<IChangeableData> districtsRaw)
        {
            List<CourtLocation> locationsToSave = new List<CourtLocation>();
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

                    locationsToSave.AddRange(locations);
                }
            }

            return locationsToSave;
        }

    }
}
