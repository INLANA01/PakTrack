using System.Collections.Generic;
using PakTrack.Models;
using PakTrack.DAL.Interfaces.Sensor;

namespace PakTrack.DAL.Interfaces
{
    public interface IVibrationConsolidatedReport : IRepository<VibrationReport>, ISensorDbRepository<VibrationReport>
    {
        IEnumerable<VibrationReport> GetRegularByTruckAndPackageId(string truckId, string packageId);
        IEnumerable<VibrationReport> GetCustomByTruckAndPackageId(string truckId, string packageId);
        IEnumerable<VibrationReport> GenerateReport(IEnumerable<Vibration> sensorData, bool isCustom = false);
    }
}