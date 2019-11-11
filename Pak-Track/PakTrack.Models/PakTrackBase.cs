using System;
using LiteDB;
using PakTrack.Models.Interfaces;

namespace PakTrack.Models
{
    /// <summary>
    /// This class contains all the common properties across all sensor events.
    /// </summary>
    public abstract class PakTrackBase : IPakTrackBase
    {
        [BsonId]
        public ObjectId Id { get; set; }

        [BsonField("sensor_id")]
        public string SensorId { get; set; }

        [BsonField("package_id")]
        public string PackageId { get; set; }

        [BsonField("truck_id")]
        public string TruckId { get; set; }

        [BsonField("timestamp")]
        public long Timestamp { get; set; }

        [BsonField("is_above_threshold")]
        public bool IsAboveThreshold { get; set; }

        [BsonField("updated_at")]
        public DateTime? UpdatedAd { get; set; }
    }
}