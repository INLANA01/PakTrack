using System.Collections.Generic;
using PakTrack.Models;
using PakTrack.Models.Sensor;

namespace PakTrack.DAL.Interfaces.Sensor
{
    public interface ILightSensorRepository : ISensorRepository<Light>
    {
        IEnumerable<Light> ExtractLightData(IEnumerable<Packet> packets);
    }
}