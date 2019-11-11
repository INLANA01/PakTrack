using LiteDB;

namespace PakTrack.Models
{
    /// <summary>
    /// Class to represent the Maximum value, the max for a given axis store both the index and the value.
    /// </summary>
    public class MaxIndex
    {
        [BsonField("index")]
        public int Index { get; set; }

        [BsonField("value")]
        public double Value { get; set; }
    }
}