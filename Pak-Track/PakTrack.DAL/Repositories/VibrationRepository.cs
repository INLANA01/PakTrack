using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LiteDB;
using PakTrack.DAL.Base;
using PakTrack.DAL.Interfaces;
using PakTrack.Models;
using PakTrack.Utilities;

namespace PakTrack.DAL.Repositories
{
    public class VibrationRepository : BaseRepository, IVibrationRepository
    {

        public VibrationRepository(IPakTrackDbContext dbContext) : base(dbContext)
        {
        }
        public Vibration GetById(ObjectId eventId)
        {
            return DbContext.Vibrations.FindById(eventId);
        }

        public IEnumerable<Vibration> GetAll()
        {
            return DbContext.Vibrations.FindAll().Where(x => x.Timestamp > new DateTime(2011,1,1).Ticks);
        }

        public IEnumerable<Vibration> GetByTruckAndPackageId(string truckId, string packageId)
        {
            return DbContext.Vibrations.Find(t => t.TruckId == truckId && t.PackageId == packageId).Where(x => x.Timestamp > new DateTime(2011, 1, 1).Ticks);
        }

        public int GetEventCount(string truckId, string packageId)
        {
            return DbContext.Vibrations.Count(t => t.TruckId == truckId && t.PackageId == packageId && t.Timestamp > new DateTime(2011, 1, 1).Ticks);
        }

        public bool DeleteEvent(ObjectId eventId)
        {
            return DbContext.Vibrations.Delete(eventId);
        }

        public async Task<bool> DeleteEvents(string truckId, string packageId)
        {

            var deletedEvents = await Task.Run(()=>
                {
                    return DbContext.Vibrations.Delete(x => x.PackageId.Equals(packageId) && x.TruckId.Equals(truckId));
                });
            return deletedEvents > 0;
        }

    

        public async Task PushToDb(IEnumerable<Vibration> sensorData, bool isFlash= false)
        {
            var vibrations = sensorData as IList<Vibration> ?? sensorData.Where(data => data.Vector.Any()).ToList();
            var packet = vibrations.FirstOrDefault();
            await Task.Run(() =>
            {
                if (packet != null && !isFlash)
                {
                    DbContext.Vibrations.Delete(x => x.SensorId.Equals(packet.SensorId) &&
                                                     x.PackageId.Equals(packet.PackageId) &&
                                                     x.TruckId.Equals(packet.TruckId));
                }
                DbContext.Vibrations.Insert(vibrations.Where(v => (v.Vector != null && v.Vector.Any())));
            });
        }
    }
}