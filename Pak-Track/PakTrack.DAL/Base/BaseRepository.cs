using System.Linq;
using PakTrack.Core;
using PakTrack.DAL.Interfaces;

namespace PakTrack.DAL.Base
{
    public class BaseRepository
    {
        protected readonly IPakTrackDbContext DbContext;

        public BaseRepository(IPakTrackDbContext dbContext)
        {
            DbContext = dbContext;
        }

        public double GetMaxThreshold(string truckId, string packageId, SensorMaxThreshold sensorMaxThreshold)
        {
            var configuration = DbContext.Configurations
                .Find(t => t.TruckId == truckId && t.PackageId == packageId).FirstOrDefault();

            if (configuration != null)
            {
                var sensorConfig = configuration.Configs.FirstOrDefault();
                //  return configuration.Configs.FirstOrDefault().HumidityMaxThreshold;
                if (sensorConfig == null)
                    return 0.0;

                switch (sensorMaxThreshold)
                {
                    case SensorMaxThreshold.Humidity:
                        return sensorConfig.HumidityMaxThreshold;
                    case SensorMaxThreshold.Temperature:
                        return sensorConfig.TemperatureMaxThreshold;
                    case SensorMaxThreshold.Light:
                        return sensorConfig.LightMaxThreshold;
                    case SensorMaxThreshold.Pressure:
                        return sensorConfig.PressureMaxThreshold;
                    case SensorMaxThreshold.Shock:
                        return sensorConfig.ShockMaxThreshold;
                    case SensorMaxThreshold.Vibration:
                        return sensorConfig.VibrationMaxThreshold;
                }
            }
            return 0.0;
        }
    }
}