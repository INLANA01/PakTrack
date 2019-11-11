using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LiteDB;
using PakTrack.DAL.Base;
using PakTrack.DAL.Interfaces;
using PakTrack.Utilities;
using PakTrack.Models;

namespace PakTrack.DAL.Repositories
{
    public class VibrationConsolidatedReport : BaseRepository, IVibrationConsolidatedReport
    {

        public VibrationConsolidatedReport(IPakTrackDbContext dbContext) : base(dbContext)
        {
        }
        
        public VibrationReport GetById(ObjectId eventId)
        {
            return DbContext.VibrationReports.FindById(eventId);
        }

        public IEnumerable<VibrationReport> GetAll()
        {
            return DbContext.VibrationReports.FindAll();
        }

        public IEnumerable<VibrationReport> GetByTruckAndPackageId(string truckId, string packageId)
        {
            return DbContext.VibrationReports.Find(t => t.TruckId == truckId && t.PackageId == packageId);
        }

        public bool DeleteEvent(ObjectId eventId)
        {
            return DbContext.VibrationReports.Delete(eventId);
        }

        public async Task<bool> DeleteEvents(string truckId, string packageId)
        {
            var deletedEvents = await Task.Run(() => DbContext.VibrationReports.Delete(x => x.PackageId.Equals(packageId) && x.TruckId.Equals(truckId)));
            return deletedEvents > 0;
        }


        public IEnumerable<VibrationReport> GetRegularByTruckAndPackageId(string truckId, string packageId)
        {
            return DbContext.VibrationReports
                .Find( t => t.TruckId == truckId && t.PackageId == packageId && !t.IsCustomReport);
        }

        public IEnumerable<VibrationReport> GetCustomByTruckAndPackageId(string truckId, string packageId)
        {
            return DbContext.VibrationReports
               .Find(t => t.TruckId == truckId && t.PackageId == packageId && t.IsCustomReport);
        }

        public int GetEventCount(string truckId, string packageId)
        {
            return DbContext.VibrationReports.Count(t => t.TruckId == truckId && t.PackageId == packageId);
        }

        public IEnumerable<VibrationReport> GenerateReport(IEnumerable<Vibration> sensorData, bool isCustom = false)
        {

            var vibrationReports = new List<VibrationReport>();
            var vibrations = sensorData as IList<Vibration> ?? sensorData.ToList();
            if (!vibrations.Any())
                return vibrationReports;
            var psdXs = vibrations.Select(vibration => vibration.PSDX).ToList();
            var psdYs = vibrations.Select(vibration => vibration.PSDY).ToList();
            var psdZs = vibrations.Select(vibration => vibration.PSDZ).ToList();
            var psdVectors = vibrations.Select(vibration => vibration.PSDVector).ToList();
            if (psdXs.Any() && psdVectors.Any() && psdYs.Any() && psdZs.Any())
            {
                var consolidatedRmsXs = PsdGrms.ConsolidatedRms(psdXs);
                var consolidatedRmsYs = PsdGrms.ConsolidatedRms(psdYs);
                var consolidatedRmsZs = PsdGrms.ConsolidatedRms(psdZs);
                var consolidatedRmsVectors = PsdGrms.ConsolidatedRms(psdVectors);
                var numberOfEvents = vibrations.Count();
                var vibrationReport = new VibrationReport
                {
                    TruckId = vibrations[0].TruckId,
                    PackageId = vibrations[0].PackageId,
                    IsCustomReport = isCustom,
                    X = consolidatedRmsXs,
                    Y = consolidatedRmsYs,
                    Z = consolidatedRmsZs,
                    Vector = consolidatedRmsVectors,
                    NumberOfEvents = numberOfEvents,
                    GRMS = new ThreeAxisInformation
                    {
                        X = PsdGrms.GetGrms(PsdGrms.GetDataWithFrequency(consolidatedRmsXs)),
                        Y = PsdGrms.GetGrms(PsdGrms.GetDataWithFrequency(consolidatedRmsYs)),
                        Z = PsdGrms.GetGrms(PsdGrms.GetDataWithFrequency(consolidatedRmsZs)),
                        Vector = PsdGrms.GetGrms(PsdGrms.GetDataWithFrequency(consolidatedRmsVectors))
                    }
                };
                vibrationReports.Add(vibrationReport);
            }
            return vibrationReports;
        }


        public async  Task PushToDb(IEnumerable<VibrationReport> sensorData, bool isFlash= false)
        {
            var docs = sensorData as IList<VibrationReport> ?? sensorData.ToList();
            await Task.Run(() =>
            {
                if (docs.Any())
                {
                    DbContext.VibrationReports.Delete(x => x.Id.Equals(docs.First().Id));
                }
                DbContext.VibrationReports.Insert(docs);
            });
        }
    }
}
