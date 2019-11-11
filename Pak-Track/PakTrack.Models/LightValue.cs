using LiteDB;

namespace PakTrack.Models
{
    /// <summary>
    /// Class to represent a light event
    /// </summary>
    public class LightValue
    {
        [BsonField("r")]
        public double R { get; set; }

        [BsonField("g")]
        public double G { get; set; }

        [BsonField("b")]
        public double B { get; set; }

        [BsonField("c")]
        public double ClearLight { get; set; }

        [BsonField("illuminance")]
        public double Illuminance { get; set; }

    }
}