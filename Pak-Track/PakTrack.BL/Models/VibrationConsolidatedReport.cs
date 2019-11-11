using System.Collections.Generic;
using System.Security.AccessControl;
using LiteDB;
using PakTrack.Models;

namespace PakTrack.BL.Models
{
    /// <summary>
    /// This class is used to handle vibration consolidated report. 
    /// Each axis is represented by an array of report's value
    /// </summary>
    public class VibrationConsolidatedReport
    {
        public ObjectId Id { get; set; }
        public IEnumerable<ReportValue> XAxis { get; set; }
        public IEnumerable<ReportValue> YAxis { get; set; }
        public IEnumerable<ReportValue> ZAxis { get; set; }
        public IEnumerable<ReportValue> Vector { get; set; }

        public ThreeAxisInformation GRMS { get; set; }
    }
}