using System.Collections.Generic;
using PakTrack.Models;
using PakTrack.Models.Sensor;

namespace PakTrack.DAL.Interfaces.Sensor
{
    public interface IHumiditySensorRepository: ISensorRepository<Humidity>
    {
        IEnumerable<Humidity> ExtractHumiditieData(IEnumerable<Packet> packets);
    }
}