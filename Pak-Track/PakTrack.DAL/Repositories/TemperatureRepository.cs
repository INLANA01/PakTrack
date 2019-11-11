using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LiteDB;
using PakTrack.DAL.Base;
using PakTrack.DAL.Interfaces;
using PakTrack.DTO;
using PakTrack.Models;
using PakTrack.Utilities;

namespace PakTrack.DAL.Repositories
{
    /// <summary>
    /// Handle temperature events.
    /// Caveat: We assume the temperature raw data unit is Fahrenheit
    /// </summary>
    public class TemperatureRepository : BaseRepository, ITemperatureRepository
    {
        public TemperatureRepository(IPakTrackDbContext dbContext) : base (dbContext)
        {
        }
        public TemperatureDTO GetById(ObjectId eventId)
        {
            var temperature = DbContext.Temperatures.FindById(eventId);
            if (temperature != null)
            {
                return  new TemperatureDTO
                {
                    Id = temperature.Id,
                    IsAboveThreshold = temperature.IsAboveThreshold,
                    Timestamp = temperature.Timestamp,
                    Value = temperature.Value,
                    FahrenheitValue = temperature.Value,
                    Unit = temperature.Unit
                };
            }
            return null;
        }

        public IEnumerable<TemperatureDTO> GetAll()
        {
            return DbContext.Temperatures.FindAll().Select(TemperatureDto).Where(x => x.DateTime.Year > 2011);
        }

        public IEnumerable<TemperatureDTO> GetByTruckAndPackageId(string truckId, string packageId)
        {
            return DbContext.Temperatures.Find(t => t.TruckId == truckId && t.PackageId == packageId)
                .Select(TemperatureDto).Where(x => x.DateTime.Year > 2011);
        }

        public int GetEventCount(string truckId, string packageId)
        {
            return DbContext.Temperatures.Count(t => t.TruckId == truckId && t.PackageId == packageId && t.Timestamp > new DateTime(2011, 1, 1).Ticks);
        }

        public async Task<bool> DeleteEvents(string truckId, string packageId)
        {
            var deletedEvents = await Task.Run(()=> DbContext.Temperatures.Delete(x => x.PackageId.Equals(packageId) && x.TruckId.Equals(truckId)));
            return deletedEvents > 0;
        }


        public bool DeleteEvent(ObjectId eventId)
        {
            return DbContext.Temperatures.Delete(eventId);
        }

        private static Func<Temperature, TemperatureDTO> TemperatureDto
        {
            get
            {
                return p => new TemperatureDTO
                {
                    Id = p.Id,
                    Timestamp = p.Timestamp,
                    IsAboveThreshold = p.IsAboveThreshold,
                    Value = p.Value,
                    Unit = p.Unit,
                    FahrenheitValue = p.Value
                };
            }
        }

        public async Task PushToDb(IEnumerable<Temperature> sensorData, bool isFlash = false)
        {


            var temperature = sensorData as IList<Temperature> ?? sensorData.ToList();
            var packet = temperature.FirstOrDefault();
            await Task.Run(() =>
            {
                if (packet != null && !isFlash)
                {
                    DbContext.Temperatures.Delete(x => x.SensorId.Equals(packet.SensorId) &&
                                                       x.PackageId.Equals(packet.PackageId) &&
                                                       x.TruckId.Equals(packet.TruckId));
                }
                DbContext.Temperatures.Insert(temperature);
            });
        }
    }
}