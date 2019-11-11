using LiteDB;

namespace PakTrack.Models
{
    /// <summary>
    /// Class to represent light sensor event
    /// </summary>
    public class Light : PakTrackBase
    {

        [BsonField("unit")]
        public string Unit { get; set; }

        [BsonField("value")]
        public LightValue Value { get; set; }

    }
}