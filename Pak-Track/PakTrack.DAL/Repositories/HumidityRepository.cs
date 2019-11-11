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
    public class HumidityRepository : BaseRepository, IHumidityRepository
    {

        public HumidityRepository(IPakTrackDbContext dbContext) : base(dbContext)
        {

        }

        public HumidityDTO GetById(ObjectId eventId)
        {
            var humidity = DbContext.Humidities.FindById(eventId);
            if (humidity != null)
            {
                return new HumidityDTO
                {
                    Id = humidity.Id,
                    Timestamp = humidity.Timestamp,
                    Value = humidity.Value,
                    IsAboveThreshold = humidity.IsAboveThreshold
                };
            }
            return null;
        }

        public IEnumerable<HumidityDTO> GetAll()
        {
            return DbContext.Humidities.FindAll().Select(GetHumidityDTO()).Where(x =>  x.DateTime.Year > 2011);
        }

        public IEnumerable<HumidityDTO> GetByTruckAndPackageId(string truckId, string packageId)
        {
            return DbContext.Humidities.Find(t => t.TruckId == truckId && t.PackageId == packageId)
                .Select(GetHumidityDTO()).Where(x => x.DateTime.Year > 2011);
        }

        public int GetEventCount(string truckId, string packageId)
        {
            return DbContext.Humidities.Count(t => t.TruckId == truckId && t.PackageId == packageId && t.Timestamp  > new DateTime(2011, 1, 1).Ticks);
        }

        public bool DeleteEvent(ObjectId eventId)
        {
            return DbContext.Humidities.Delete(eventId);
        }

        private static Func<Humidity, HumidityDTO> GetHumidityDTO()
        {
            return p => new HumidityDTO
            {
                Id = p.Id,
                Value = p.Value,
                IsAboveThreshold = p.IsAboveThreshold,
                Timestamp = p.Timestamp
            };
        }

 
        public async Task PushToDb(IEnumerable<Humidity> sensorData, bool isFlash=false)
        {
            var humidities = sensorData as IList<Humidity> ?? sensorData.ToList();
            var packet = humidities.FirstOrDefault();
            await Task.Run(() =>
            {
                if (packet != null && !isFlash)
                {
                    DbContext.Humidities.Delete(x => x.SensorId.Equals(packet.SensorId) &&
                                                     x.PackageId.Equals(packet.PackageId) &&
                                                     x.TruckId.Equals(packet.TruckId));
                }
                DbContext.Humidities.Insert(humidities);
            });

        }

        public async Task<bool> DeleteEvents(string truckId, string packageId)
        {
            var deletedEvents = await Task.Run(()=>  DbContext.Humidities.Delete(x => x.PackageId.Equals(packageId) && x.TruckId.Equals(truckId)));
            return deletedEvents > 0;
        }


    }
}