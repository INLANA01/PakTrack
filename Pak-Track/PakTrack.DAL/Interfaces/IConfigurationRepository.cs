using System.Collections.Generic;
using System.Threading.Tasks;
using LiteDB;
using PakTrack.Models;

namespace PakTrack.DAL.Interfaces
{
    public interface IConfigurationRepository
    {
        Configuration GetById(ObjectId id);
        IEnumerable<Configuration> GetAll();

        IEnumerable<Configuration> GetByOrganizationId(string organizationId);

        IEnumerable<Configuration> GetByTruckAndPackageId(string truckId, string packageId);

        IEnumerable<string> GetTrucksByOrganizationId(string organizationId);

        IEnumerable<string> GetPackageByTruckId(string truckId);
        void PushToDb(IEnumerable<Configuration> saveConfigurations);

        void PushToDb(Configuration savConfiguration);

        IEnumerable<string> GetAllTrucks();

        void SetShockEvents(string truckId, string packageId, long events);

        void SetHumidityEvents(string truckId, string packageId, long events);

        void SetVibrationEvents(string truckId, string packageId, long events);

        void SetTemperatureEvents(string truckId, string packageId, long events);

        void SetLightEvents(string truckId, string packageId, long events);

        void SetPressureEvents(string truckId, string packageId, long events);

        Task<bool> DeleteEvents(string truckId, string packageId);
    }
}