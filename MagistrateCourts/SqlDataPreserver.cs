using System.Collections.Generic;
using System.Linq;
using NoCompany.Interfaces;
using System.Data;
using CodeContracts;
using System;
using System.Threading;
using static NoCompany.Data.Properties.Resources;
using log4net;

namespace NoCompany.Data
{
    public class SqlDataStorageProvider : IDataStorageProvider
    {
        public static ILog logger = LogManager.GetLogger(typeof(SqlDataStorageProvider));

        public event EventHandler ImStillAlive;

        public IEnumerable<IChangeableData> GetData()
        {
            KeepTracking(Trace_AllCurtRegionsLoad);

                        IEnumerable<IChangeableData> data = null;
            using (CourtDBContext dbContext = new CourtDBContext())
            {
                data = dbContext.CourtRegions.Include("CourtDistricts")
                                             .Include("CourtDistricts.CourtLocations")
                                             .ToList();
            }

            return data;
        }
        public void CleanStorage()
        {
            KeepTracking(Trace_StorageCleaning);

            using (CourtDBContext dbContext = new CourtDBContext())
            {
                dbContext.BulkDelete(dbContext.CourtRegions);
            }
        }

        public void SaveData(IEnumerable<IChangeableData> data)
        {
            Requires.NotNullOrEmpty(data, "data");
            InsertAllRegions(data);
        }

        private void InsertAllRegions(IEnumerable<IChangeableData> regionsRaw)
        {
            KeepTracking(Trace_SaveRegions);

            Dictionary<string, CourtRegion> regionDictionary = null;
            CourtDBContext dbContext = new CourtDBContext();
            using (dbContext)
            {
                dbContext.BulkInsert(regionsRaw.Cast<CourtRegion>());
                regionDictionary = dbContext.CourtRegions.ToDictionary(x => x.Name);
            }
            InsertAllDistricts(regionDictionary, regionsRaw);
        }


        private void InsertAllDistricts(Dictionary<string, CourtRegion> regionsSaved, IEnumerable<IChangeableData> regionsRaw)
        {
            KeepTracking(Trace_SaveDistricts);

            List<CourtDistrict> districtsWithParentId = SetParentIdToDistricts(regionsSaved, regionsRaw);

            Dictionary<string, CourtDistrict> savedDistricts = null;
            using (CourtDBContext dbContext = new CourtDBContext())
            {
                dbContext.BulkInsert(districtsWithParentId);
                savedDistricts = dbContext.CourtDistricts.ToDictionary(x => x.Name);
            }
            InsertAllLocations(savedDistricts, districtsWithParentId);
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

        private void InsertAllLocations(Dictionary<string, CourtDistrict> districtsSaved, IEnumerable<IChangeableData> districtsRaw)
        {
            KeepTracking(Trace_SaveLocations);

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

        private void KeepTracking(string format, params object[] arg)
        {
            logger.DebugFormat(format, arg);
            if(ImStillAlive != null)
                ImStillAlive(this, new EventArgs());
        }

    }
}
