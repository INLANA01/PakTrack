using System.Collections.Generic;
using LiteDB;

namespace PakTrack.Models
{
    public class SRSAxis
    {
        [BsonField("positive")]
        public ICollection<double> Positive { get; set; }
        [BsonField("negative")]
        public ICollection<double> Negative { get; set; }

        [BsonField("freq")]
        public ICollection<double> Frequency { get; set; }
    }
}