using System.Collections.Generic;
using LiteDB;

namespace PakTrack.Models
{
    /// <summary>
    /// Class to represent a vibration event
    /// </summary>
    public class Vibration : ShockVibrationBase
    {

        [BsonField("psd_x")]
        public ICollection<double> PSDX { get; set; }

        [BsonField("psd_y")]
        public ICollection<double> PSDY { get; set; }

        [BsonField("psd_z")]
        public ICollection<double> PSDZ { get; set; }

        [BsonField("psd_vector")]
        public ICollection<double> PSDVector { get; set; }

        [BsonField("max_psd")]
        public ThreeAxisInformation MaximumPSD { get; set; }

        [BsonField("kurtosis")]
        public ThreeAxisInformation Kurtosis { get; set; }

        [BsonField("grms")]
        public ThreeAxisInformation GRMS { get; set; }

        [BsonField("rms")]
        public ThreeAxisInformation RMS { get; set; }


    }
}