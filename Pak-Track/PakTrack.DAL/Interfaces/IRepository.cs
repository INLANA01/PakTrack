using System.Collections.Generic;
using System.Threading.Tasks;
using LiteDB;
using PakTrack.Core;

namespace PakTrack.DAL.Interfaces
{
    public interface IRepository<TObject> where TObject:class
    {
        TObject GetById(ObjectId eventId);
        IEnumerable<TObject> GetAll();

        IEnumerable<TObject> GetByTruckAndPackageId(string truckId, string packageId);

        double GetMaxThreshold(string truckId, string packageId, SensorMaxThreshold sensorMaxThreshold);

        bool DeleteEvent(ObjectId eventId);

        int GetEventCount(string truckId, string packageId);

        Task<bool> DeleteEvents(string truckId, string packageId);


    }
}