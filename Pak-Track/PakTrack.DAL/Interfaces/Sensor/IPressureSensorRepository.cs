using System.Collections.Generic;
using PakTrack.Models;
using PakTrack.Models.Sensor;

namespace PakTrack.DAL.Interfaces.Sensor
{
    public interface IPressureSensorRepository:ISensorRepository<Pressure>
    {
        IEnumerable<Pressure> ExtractPressurekData(IEnumerable<Packet> packets);
    }
}