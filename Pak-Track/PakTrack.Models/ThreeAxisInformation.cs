using LiteDB;

namespace PakTrack.Models
{
    /// <summary>
    /// This class is used to represent those properties that need to store information about x,y and z axis as well as the resulting vector.
    /// </summary>
    public class ThreeAxisInformation
    {
        [BsonField("x")]
        public double X { get; set; }

        [BsonField("y")]
        public double Y { get; set; }

        [BsonField("z")]
        public double Z { get; set; }

        [BsonField("vector")]
        public double Vector { get; set; }

    }
}