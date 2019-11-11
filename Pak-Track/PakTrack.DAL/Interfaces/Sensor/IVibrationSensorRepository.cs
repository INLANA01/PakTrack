using System.Collections.Generic;
using PakTrack.Models;
using PakTrack.Models.Sensor;

namespace PakTrack.DAL.Interfaces.Sensor
{
    public interface IVibrationSensorRepository : ISensorRepository<Vibration>
    {
        IEnumerable<Vibration> ExtractVibrationData(IEnumerable<Packet> packets);
    }
}