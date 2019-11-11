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
    public class LightRepository : BaseRepository, ILightRepository
    {
        public LightRepository(IPakTrackDbContext dbContext) : base(dbContext)
        {
        }

        public Light GetById(ObjectId eventId)
        {
            return DbContext.Lights.FindById(eventId);
        }

        public IEnumerable<Light> GetAll()
        {
            return DbContext.Lights.FindAll().Where(x => x.Timestamp > new DateTime(2011, 1, 1).Ticks);
        }

        public IEnumerable<Light> GetByTruckAndPackageId(string truckId, string packageId)
        {
            return DbContext.Lights.Find(t => t.TruckId == truckId && t.PackageId == packageId).Where(x => x.Timestamp > new DateTime(2011,1,1).Ticks);
        }

        public int GetEventCount(string truckId, string packageId)
        {
            return DbContext.Lights.Count(t => t.TruckId == truckId && t.PackageId == packageId && t.Timestamp > new DateTime(2011, 1, 1).Ticks);
        }


        public async Task<bool> DeleteEvents(string truckId, string packageId)
        {
            var deletedEvents = await Task.Run(() => DbContext.Lights.Delete(x => x.PackageId.Equals(packageId) && x.TruckId.Equals(truckId)));
            return deletedEvents > 0;
        }

      

        public bool DeleteEvent(ObjectId eventId)
        {
            return DbContext.Lights.Delete(eventId);
        }

        public async Task PushToDb(IEnumerable<Light> sensorData, bool isFlash=false)
        {
            var lights = sensorData as IList<Light> ?? sensorData.ToList();
            var packet = lights.FirstOrDefault();
            await Task.Run(() =>
            {
                if (packet != null && !isFlash)
                {
                    DbContext.Lights.Delete(x => x.SensorId.Equals(packet.SensorId) &&
                                                 x.PackageId.Equals(packet.PackageId) &&
                                                 x.TruckId.Equals(packet.TruckId));
                }
                DbContext.Lights.Insert(lights);
            });
        }

    }
}