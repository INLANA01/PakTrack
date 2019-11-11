using System.Collections.Generic;
using LiteDB;

namespace PakTrack.Models
{
    public class VibrationReport
    {
        [BsonId]
        public ObjectId Id { get; set; }

        [BsonField("truck_id")]
        public string TruckId { get; set; }

        [BsonField("package_id")]
        public string PackageId { get; set; }

        [BsonField("is_custom_report")]
        public bool IsCustomReport { get; set; }

        [BsonField("x")]
        public ICollection<double> X { get; set; }

        [BsonField("y")]
        public ICollection<double> Y { get; set; }

        [BsonField("z")]
        public ICollection<double> Z { get; set; }

        [BsonField("vector")]
        public ICollection<double> Vector { get; set; }

        [BsonField("no_of_event")]
        public int NumberOfEvents { get; set; }

        /// <summary>
        /// This property represents the 3 axis and the vector for the GRMS
        /// </summary>
        [BsonField("grms")]
        public ThreeAxisInformation GRMS { get; set; }

    }
}