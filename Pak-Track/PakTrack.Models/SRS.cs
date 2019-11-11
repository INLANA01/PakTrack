using LiteDB;

namespace PakTrack.Models
{
    public class SRS
    {
        [BsonField("x")]
        public SRSAxis X { get; set; }

        [BsonField("y")]
        public SRSAxis Y { get; set; }

        [BsonField("z")]
        public SRSAxis Z { get; set; }
    }
}