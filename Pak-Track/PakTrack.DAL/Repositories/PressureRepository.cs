using System;
using System.Collections.Generic;
using LiteDB;
using PakTrack.DAL.Interfaces;
using PakTrack.DTO;
using PakTrack.Models;
using System.Linq;
using System.Threading.Tasks;
using PakTrack.DAL.Base;
using PakTrack.Utilities;

namespace PakTrack.DAL.Repositories
{
    public class PressureRepository : BaseRepository, IPressureRepository
    {
        public PressureRepository(IPakTrackDbContext dbContext) : base(dbContext)
        {
        }
        
        public PressureDTO GetById(ObjectId eventId)
        {
            var pressure = DbContext.Pressures.FindById(eventId);
            if (pressure != null)
            {
                return  new PressureDTO
                {
                    Id = pressure.Id,
                    Timestamp = pressure.Timestamp,
                    Value = pressure.Value,
                    IsAboveThreshold = pressure.IsAboveThreshold
                };
            }
            return null;
        }

        public async Task<bool> DeleteEvents(string truckId, string packageId)
        {
            var deletedEvents = await Task.Run(() =>
                DbContext.Pressures.Delete(x => x.PackageId.Equals(packageId) && x.TruckId.Equals(truckId)));
            return deletedEvents > 0;
        }

      

        public IEnumerable<PressureDTO> GetAll()
        {
            return DbContext.Pressures.FindAll().Select(GetPressureDTO()).Where(x => x.DateTime.Year > 2011);
        }

        public IEnumerable<PressureDTO> GetByTruckAndPackageId(string truckId, string packageId)
        {
            return DbContext.Pressures.Find(t => t.TruckId == truckId && t.PackageId == packageId)
                .Select(GetPressureDTO()).Where(x => x.DateTime.Year > 2011);
        }

        public int GetEventCount(string truckId, string packageId)
        {
            return DbContext.Pressures.Count(t => t.TruckId == truckId && t.PackageId == packageId && t.Timestamp > new DateTime(2011, 1, 1).Ticks);
        }

        public bool DeleteEvent(ObjectId eventId)
        {
            return DbContext.Pressures.Delete(eventId);
        }

        private static Func<Pressure, PressureDTO> GetPressureDTO()
        {
            return p => new PressureDTO
            {
                Id = p.Id,
                Timestamp = p.Timestamp,
                Value = p.Value,
                IsAboveThreshold = p.IsAboveThreshold
            };
        }

        public async Task PushToDb(IEnumerable<Pressure> sensorData, bool isFlash = false)
        {
            var pressures = sensorData as IList<Pressure> ?? sensorData.ToList();
            var packet = pressures.FirstOrDefault();
            await Task.Run(() =>
            {
                if (packet != null && !isFlash)
                {
                    DbContext.Pressures.Delete(x => x.SensorId.Equals(packet.SensorId) &&
                                                    x.PackageId.Equals(packet.PackageId) &&
                                                    x.TruckId.Equals(packet.TruckId));
                }
                DbContext.Pressures.Insert(pressures);
            });
        }
    }
}