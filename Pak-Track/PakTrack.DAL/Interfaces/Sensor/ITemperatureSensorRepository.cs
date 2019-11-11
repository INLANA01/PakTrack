using System.Collections.Generic;
using PakTrack.Models;
using PakTrack.Models.Sensor;

namespace PakTrack.DAL.Interfaces.Sensor
{
    public interface ITemperatureSensorRepository : ISensorRepository<Temperature>
    {
        IEnumerable<Temperature> ExtractTemperatureData(IEnumerable<Packet> packets);
    }
}