using System.Collections.Generic;
using PakTrack.BL.Models;
using PakTrack.Models;

namespace PakTrack.BL.Interfaces
{
    public interface IReportManager
    {
        IEnumerable<VibrationReportSummary> GetReportSummary(IEnumerable<VibrationReport> reports);
        VibrationConsolidatedReport GetConsolidatedReport(VibrationReport report);
    }
}