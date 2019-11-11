using System.Collections.Generic;
using LiteDB;

namespace PakTrack.Models
{
    /// <summary>
    /// Class to represent a shock event
    /// </summary>
    public class Shock : ShockVibrationBase
    {

        [BsonField("drop_height")]
        public double DropHeight { get; set; }

        [BsonField("g_rms")]
        public double GRMS { get; set; }

        [BsonField("is_instantaneous")]
        public bool IsInstantaneous { get; set; }

        [BsonField("average")]
        public ThreeAxisInformation Average { get; set; }

        [BsonField("srs")]
        public SRS SRS { get; set; }

        [BsonField("orientation")]
        public ICollection<int> Orientation { get; set; }
    }
}