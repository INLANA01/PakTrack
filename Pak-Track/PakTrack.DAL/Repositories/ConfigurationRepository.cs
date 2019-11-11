using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LiteDB;
using PakTrack.DAL.Interfaces;
using PakTrack.Models;

namespace PakTrack.DAL.Repositories
{
    public class ConfigurationRepository :IConfigurationRepository
    {
        private readonly IPakTrackDbContext _dbContext;

        public ConfigurationRepository(IPakTrackDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public Configuration GetById(ObjectId id)
        {
            return _dbContext.Configurations.FindById(id);
        }

        public IEnumerable<Configuration> GetAll()
        {
            return _dbContext.Configurations.FindAll();
        }

        public IEnumerable<Configuration> GetByOrganizationId(string organizationId)
        {
            return _dbContext.Configurations.Find(t=>t.OrganizationId == organizationId);
        }

        public IEnumerable<Configuration> GetByTruckAndPackageId(string truckId, string packageId)
        {
            return _dbContext.Configurations.Find(t => t.TruckId == truckId && t.PackageId == packageId);
        }

        public IEnumerable<string> GetTrucksByOrganizationId(string organizationId)
        {
            var trucks = new List<string>();
            var organizations = _dbContext.Configurations.Find(t => t.OrganizationId == organizationId);
            if (organizations == null) return trucks;
            trucks.AddRange(organizations.Select(t => t.TruckId));

            return trucks;
        }

        public IEnumerable<string> GetPackageByTruckId(string truckId)
        {
            var packages = new List<string>();
            var organizations = _dbContext.Configurations.Find(t => t.TruckId == truckId);
            if (organizations == null) return packages;
            packages.AddRange(organizations.Select(t=>t.PackageId));

            return packages;
        }

        public void PushToDb(IEnumerable<Configuration> saveConfigurations)
        {
            _dbContext.Configurations.Insert(saveConfigurations);
        }

        public void PushToDb(Configuration saveConfiguration)
        {
            if (saveConfiguration != null)
            {
                _dbContext.Configurations.Delete(x => x.SensorId.Equals(saveConfiguration.SensorId) &&
                x.PackageId.Equals(saveConfiguration.PackageId) && x.TruckId.Equals(saveConfiguration.TruckId));
            }
            _dbContext.Configurations.Insert(saveConfiguration);
        }

        public IEnumerable<string> GetAllTrucks()
        {
            var trucks = new List<string>();
            var confugurations = _dbContext.Configurations.FindAll();
            if (confugurations == null) return trucks;
            trucks.AddRange(confugurations.Select(t => t.TruckId));
            return trucks;
        }

        public void SetHumidityEvents(string truckId, string packageId, long events)
        {
            var config = _dbContext.Configurations.FindOne(t => t.TruckId == truckId && t.PackageId == packageId);
            config.HumidityEvents = events;
            _dbContext.Configurations.Update(config);

        }


        public void SetVibrationEvents(string truckId, string packageId, long events)
        {
            var config = _dbContext.Configurations.FindOne(t => t.TruckId == truckId && t.PackageId == packageId);
            config.VibrationEvents = events;
            _dbContext.Configurations.Update(config);

        }
        public void SetShockEvents(string truckId, string packageId, long events)
        {
            var config = _dbContext.Configurations.FindOne(t => t.TruckId == truckId && t.PackageId == packageId);
            config.ShockEvents = events;
            _dbContext.Configurations.Update(config);

        }
        public void SetTemperatureEvents(string truckId, string packageId, long events)
        {
            var config = _dbContext.Configurations.FindOne(t => t.TruckId == truckId && t.PackageId == packageId);
            config.TemperatureEvents = events;
            _dbContext.Configurations.Update(config);

        }
        public void SetLightEvents(string truckId, string packageId, long events)
        {
            var config = _dbContext.Configurations.FindOne(t => t.TruckId == truckId && t.PackageId == packageId);
            config.LightEvents = events;
            _dbContext.Configurations.Update(config);

        }
        public void SetPressureEvents(string truckId, string packageId, long events)
        {
            var config = _dbContext.Configurations.FindOne(t => t.TruckId == truckId && t.PackageId == packageId);
            config.PressureEvents = events;
            _dbContext.Configurations.Update(config);

        }

        public async Task<bool>  DeleteEvents(string truckId, string packageId)
        {
            var result = await Task.Run(() => _dbContext.Configurations.Delete(config =>
                config.TruckId.Equals(truckId) && config.PackageId.Equals(packageId)));
            return result > 0;
        }
    }
}