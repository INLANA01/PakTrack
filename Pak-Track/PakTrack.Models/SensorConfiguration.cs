using System.Collections.Generic;
using LiteDB;

namespace PakTrack.Models
{
    //#region configuration and user information

    public class SensorConfiguration
    {
        //public ICollection<ISensorConfigurationBase> SensorConfig { get; set; }

        //Temperature
        [BsonField("temperature_maxthreshold")]
        public double TemperatureMaxThreshold { get; set; }
        [BsonField("temperature_minthreshold")]
        public double TemperatureMinThreshold { get; set; }
        [BsonField("temperature_timeperiod")]
        public int TemperatureTimePeriod { get; set; }
        [BsonField("temperature_timeperiod_after_threshold")]
        public int TemperatureTimePeriodAfterThreshold { get; set; }

        //Humidity
        [BsonField("humidity_maxthreshold")]
        public double HumidityMaxThreshold { get; set; }
        [BsonField("humidity_minthreshold")]
        public double HumidityMinThreshold { get; set; }
        [BsonField("humidity_timeperiod")]
        public int HumidityTimePeriod { get; set; }
        [BsonField("humidity_timeperiod_after_threshold")]
        public int HumidityTimePeriodAfterThreshold { get; set; }

        //Vibration
        [BsonField("vibration_maxthreshold")]
        public double VibrationMaxThreshold { get; set; }
        [BsonField("vibration_minthreshold")]
        public double VibrationMinThreshold { get; set; }
        [BsonField("vibration_timeperiod")]
        public int VibrationTimePeriod { get; set; }
        [BsonField("vibration_timeperiod_after_threshold")]
        public int VibrationTimePeriodAfterThreshold { get; set; }

        //Shock
        [BsonField("shock_maxthreshold")]
        public double ShockMaxThreshold { get; set; }
        [BsonField("shock_minthreshold")]
        public double ShockMinThreshold { get; set; }
        [BsonField("shock_timeperiod")]
        public int ShockTimePeriod { get; set; }
        [BsonField("shock_timeperiod_after_threshold")]
        public int ShockTimePeriodAfterThreshold { get; set; }

        //Pressure
        [BsonField("pressure_maxthreshold")]
        public double PressureMaxThreshold { get; set; }
        [BsonField("pressure_minthreshold")]
        public double PressureMinThreshold { get; set; }
        [BsonField("pressure_timeperiod")]
        public int PressureTimePeriod { get; set; }
        [BsonField("pressure_timeperiod_after_threshold")]
        public int PressureTimePeriodAfterThreshold { get; set; }

        //Light
        [BsonField("light_maxthreshold")]
        public double LightMaxThreshold { get; set; }
        [BsonField("light_minthreshold")]
        public double LightMinThreshold { get; set; }
        [BsonField("light_timeperiod")]
        public int LightTimePeriod { get; set; }
        [BsonField("light_timeperiod_after_threshold")]
        public int LightTimePeriodAfterThreshold { get; set; }
    }


    //#endregion

}

