using LiteDB;

namespace PakTrack.BL.Models
{
    public class VibrationReportSummary
    {
        public ObjectId Id { get; set; }
        public string CreationDate { get; set; }
        public int NumberOfEvents { get; set; }
    }
}