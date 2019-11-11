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
    public class ShockRepository : BaseRepository, IShockRepository
    {
        public ShockRepository(IPakTrackDbContext dbContext) : base(dbContext)
        {
        }
        public Shock GetById(ObjectId eventId)
        {
            return DbContext.Shocks.FindById(eventId);
        }

        public IEnumerable<Shock> GetAll()
        {
            return DbContext.Shocks.FindAll().Where(x => x.Timestamp > new DateTime(2011, 1, 1).Ticks);
        }

        public IEnumerable<Shock> GetByTruckAndPackageId(string truckId, string packageId)
        {
            return DbContext.Shocks.Find(t => t.TruckId == truckId && t.PackageId == packageId).Where(x => x.Timestamp > new DateTime(2011, 1, 1).Ticks);
        }

        public int GetEventCount(string truckId, string packageId)
        {
            return DbContext.Shocks.Count(t => t.TruckId == truckId && t.PackageId == packageId && t.Timestamp > new DateTime(2011, 1, 1).Ticks);
        }
        public bool DeleteEvent(ObjectId eventId)
        {
            return DbContext.Shocks.Delete(eventId);
        }

        public async Task<bool> DeleteEvents(string truckId, string packageId)
        {
            var deletedEvents = await Task.Run(()=> DbContext.Shocks.Delete(x => x.PackageId.Equals(packageId) && x.TruckId.Equals(truckId)));
            return deletedEvents > 0;
        }


        public async Task PushToDb(IEnumerable<Shock> sensorData, bool isFlash = false)
        {
            var shocks = sensorData as IList<Shock> ?? sensorData.Where(data => data.Vector.Any()).ToList();
            var packet = shocks.FirstOrDefault();
            await Task.Run(() =>
            {
                if (packet != null && !isFlash)
                {
                    DbContext.Shocks.Delete(x => x.SensorId.Equals(packet.SensorId) &&
                                                 x.PackageId.Equals(packet.PackageId) &&
                                                 x.TruckId.Equals(packet.TruckId));
                }
                DbContext.Shocks.Insert(shocks);
            });

        }

    }
}