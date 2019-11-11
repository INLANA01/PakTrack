using System;
using System.Collections.Generic;
using LiteDB;

namespace PakTrack.Models
{
    public class Configuration
    {
        [BsonId]
        public ObjectId Id { get; set; }

        [BsonField("comments")]
        public string Comments { get; set; }

        [BsonField("is_realtime")]
        public bool IsRealTime { get; set; }

        [BsonField("organization_id")]
        public string OrganizationId { get; set; }

        [BsonField("package_id")]
        public string PackageId { get; set; }

        [BsonField("truck_id")]
        public string TruckId { get; set; }

        [BsonField("sensor_id")]
        public string SensorId { get; set; }

        [BsonField("timestamp")]
        public long Timestamp { get; set; }

        [BsonField("updated_at")]
        public DateTime? UpdatedAt { get; set; }

        [BsonField("configs")]
        public ICollection<SensorConfiguration> Configs { get; set; }

        [BsonField("humidity_events")]
        public long HumidityEvents { get; set; }

        [BsonField("vibration_events")]
        public long VibrationEvents { get; set; }

        [BsonField("pressure_events")]
        public long PressureEvents { get; set; }

        [BsonField("schock_events")]
        public long ShockEvents { get; set; }

        [BsonField("light_events")]
        public long LightEvents { get; set; }

        [BsonField("temperature_events")]
        public long TemperatureEvents { get; set; }

    }
}