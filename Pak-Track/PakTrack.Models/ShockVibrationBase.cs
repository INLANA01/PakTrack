using System.Collections.Generic;
using LiteDB;

namespace PakTrack.Models
{
    /// <summary>
    /// Base class for shock and vibration
    /// </summary>
    public abstract class ShockVibrationBase : PakTrackBase
    {
        [BsonField("is_processed")]
        public bool IsProcessed { get; set; }
        [BsonField("x")]
        public ICollection<double> X { get; set; }

        [BsonField("y")]
        public ICollection<double> Y { get; set; }

        [BsonField("z")]
        public ICollection<double> Z { get; set; }

        [BsonField("vector")]
        public ICollection<double> Vector { get; set; }
        [BsonField("max_x")]
        public MaxIndex MaximumX { get; set; }

        [BsonField("max_y")]
        public MaxIndex MaximumY { get; set; }

        [BsonField("max_z")]
        public MaxIndex MaximumZ { get; set; }

        [BsonField("max_vector")]
        public MaxIndex MaximumVector { get; set; }
    }
}