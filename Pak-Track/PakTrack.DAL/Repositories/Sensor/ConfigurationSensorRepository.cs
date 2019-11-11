using System.Collections.Generic;
using System.Threading.Tasks;
using PakTrack.DAL.Base;
using PakTrack.DAL.Context;
using PakTrack.DAL.Interfaces.Sensor;
using PakTrack.Models;
using PakTrack.Models.Sensor;
using PakTrack.Utilities;

namespace PakTrack.DAL.Repositories.Sensor
{
    public class ConfigurationSensorRepository : BaseSensorRepository, IConfigurationSensorRepository
    {
        public ConfigurationSensorRepository(ISensorReaderContext sensorReader) : base(sensorReader)
        {
        }

        public Configuration GetDeviceConfiguration()
        {
            var configPacket = GetConfiguration();
            return new Configuration
            {
                SensorId = this.SensorId,
                Timestamp = configPacket.GetTimeStampMilliSeconds(),
                TruckId = this.TruckId,
                PackageId = this.PackageId,
                Comments = this.Notes,
                UpdatedAt = configPacket.getDate(),
                Configs = new List<SensorConfiguration>(new[]
                {
                    toSensorConfiguration(configPacket)
                }),
                IsRealTime = false
            };
        }

        private SensorConfiguration toSensorConfiguration(Packet packet)
        {
            //Temerature
            var sensorConfiguration = new SensorConfiguration();
            var rawThreshold = (packet.getData(SensorConstants.TEMP_MAX_THRESHOLD_INDEX) & 0xff) | 
                (packet.getData(SensorConstants.TEMP_MAX_THRESHOLD_INDEX + 1) & 0xff) << 8;
            var time = (packet.getData(SensorConstants.TEMP_TIME_INDEX) & 0xff) | 
                (packet.getData(SensorConstants.TEMP_TIME_INDEX + 1) & 0xff) << 8;
            var overtime = (packet.getData(SensorConstants.TEMP_TIME_OVER_THRES_INDEX) & 0xff) | 
                (packet.getData(SensorConstants.TEMP_TIME_OVER_THRES_INDEX + 1) & 0xff) << 8;

            sensorConfiguration.TemperatureMaxThreshold = SensorUtils.GetTemperature(rawThreshold);
            sensorConfiguration.TemperatureTimePeriod = time;
            sensorConfiguration.TemperatureTimePeriodAfterThreshold = overtime;

            //Humidity
            rawThreshold = (packet.getData(SensorConstants.HUMD_MAX_THRES_INDEX) & 0xff) | 
                (packet.getData(SensorConstants.HUMD_MAX_THRES_INDEX + 1) & 0xff) << 8;
            time = (packet.getData(SensorConstants.HUMD_TIME_INDEX) & 0xff) | 
                (packet.getData(SensorConstants.HUMD_TIME_INDEX + 1) & 0xff) << 8;
            overtime = (packet.getData(SensorConstants.HUMD_TIME_OVER_THRES_INDEX) & 0xff) | 
                (packet.getData(SensorConstants.HUMD_TIME_OVER_THRES_INDEX + 1) & 0xff) << 8;

            sensorConfiguration.HumidityMaxThreshold = SensorUtils.GetHumidity(rawThreshold);
            sensorConfiguration.HumidityTimePeriod = time;
            sensorConfiguration.HumidityTimePeriodAfterThreshold = overtime;

            //Shock
            rawThreshold = (packet.getData(SensorConstants.SHOCK_MAX_THRESHOLD_INDEX) & 0xff) | 
                (packet.getData(SensorConstants.SHOCK_MAX_THRESHOLD_INDEX + 1) & 0xff) << 8;
            sensorConfiguration.ShockMaxThreshold = SensorUtils.GetShock(rawThreshold);

            //Vibration
            rawThreshold = (packet.getData(SensorConstants.VIB_MAX_THRESHOLD_INDEX) & 0xff) |
                (packet.getData(SensorConstants.VIB_MAX_THRESHOLD_INDEX + 1) & 0xff) << 8;
            time = (packet.getData(SensorConstants.VIB_TIME_INDEX) & 0xff) | 
                (packet.getData(SensorConstants.VIB_TIME_INDEX + 1) & 0xff) << 8;
            overtime = (packet.getData(SensorConstants.VIB_TIME_OVER_THRES_INDEX) & 0xff) | 
                (packet.getData(SensorConstants.VIB_TIME_OVER_THRES_INDEX + 1) & 0xff) << 8;

            sensorConfiguration.VibrationMaxThreshold = SensorUtils.GetVibrationThreshold(rawThreshold);
            sensorConfiguration.VibrationTimePeriod = time;
            sensorConfiguration.VibrationTimePeriodAfterThreshold = overtime;

            //Pressure
            rawThreshold = (packet.getData(SensorConstants.PRESSURE_HIGH_THRESHOLD_INDEX) & 0xff) | 
                (packet.getData(SensorConstants.PRESSURE_HIGH_THRESHOLD_INDEX + 1) & 0xff) << 8;
            time = (packet.getData(SensorConstants.PRESSURE_REPORT_RATE_INDEX) & 0xff) | 
                (packet.getData(SensorConstants.PRESSURE_REPORT_RATE_INDEX + 1) & 0xff) << 8;
            overtime = (packet.getData(SensorConstants.PRESSURE_OVER_THRESHOLD_REPORT_RATE_INDEX) & 0xff) | 
                (packet.getData(SensorConstants.PRESSURE_OVER_THRESHOLD_REPORT_RATE_INDEX + 1) & 0xff) << 8;

            sensorConfiguration.PressureMaxThreshold = SensorUtils.GetPressureThreshold(rawThreshold);
            sensorConfiguration.PressureTimePeriod = time;
            sensorConfiguration.PressureTimePeriodAfterThreshold = overtime;

            //Light
            rawThreshold = (packet.getData(SensorConstants.LIGHT_HIGH_THRESHOLD_INDEX) & 0xff) | 
                (packet.getData(SensorConstants.LIGHT_HIGH_THRESHOLD_INDEX + 1) & 0xff) << 8;
            time = (packet.getData(SensorConstants.LIGHT_REPORT_RATE_INDEX) & 0xff) | 
                (packet.getData(SensorConstants.LIGHT_REPORT_RATE_INDEX + 1) & 0xff) << 8;
            overtime = (packet.getData(SensorConstants.LIGHT_OVER_THRESHOLD_REPORT_RATE_INDEX) & 0xff) | 
                (packet.getData(SensorConstants.LIGHT_OVER_THRESHOLD_REPORT_RATE_INDEX + 1) & 0xff) << 8;

            sensorConfiguration.LightMaxThreshold = SensorUtils.GetClearLight(rawThreshold);
            sensorConfiguration.LightTimePeriod = time;
            sensorConfiguration.LightTimePeriodAfterThreshold = overtime;

            return sensorConfiguration;
        }
    }
}