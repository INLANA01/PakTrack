using PakTrack.DAL.Interfaces.Sensor;
using PakTrack.DTO;
using PakTrack.Models;

namespace PakTrack.DAL.Interfaces
{
    public interface ITemperatureRepository : IRepository<TemperatureDTO>, ISensorDbRepository<Temperature>
    {
    }
}