using System.Collections.Generic;
using PakTrack.Models;
using PakTrack.Models.Sensor;

namespace PakTrack.DAL.Interfaces.Sensor
{
    public interface IShockSensorRepository:ISensorRepository<Shock>
    {
        IEnumerable<Shock> ExtractShockData(IEnumerable<Packet> packets);
    }
}
