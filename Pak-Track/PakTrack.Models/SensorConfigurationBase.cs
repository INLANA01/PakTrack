using LiteDB;

namespace PakTrack.Models
{
    public abstract class SensorConfigurationBase : ISensorConfigurationBase
    {
        [BsonField("maxthreshold")]
        public double MaxThreshold { get; set; }

        [BsonField("minthreshold")]
        public double MinThreshold { get; set; }

        [BsonField("timeperiod")]
        public int TimePeriod { get; set; }

        [BsonField("timeperiod_after_threshold")]
        public int TimePeriodAfterThreshold { get; set; }
    }
}