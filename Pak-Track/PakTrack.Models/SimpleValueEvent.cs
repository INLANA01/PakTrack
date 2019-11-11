using LiteDB;

namespace PakTrack.Models
{
    /// <summary>
    /// This abstract class it used for those sensor events that the value is a simple double value.
    /// </summary>
    public abstract class SimpleValueEvent : PakTrackBase
    {
        [BsonField("unit")]
        public string Unit { get; set; }

        [BsonField("value")]
        public double Value { get; set; }
    }
}