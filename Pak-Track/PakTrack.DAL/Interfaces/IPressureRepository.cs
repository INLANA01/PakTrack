using PakTrack.DAL.Interfaces.Sensor;
using PakTrack.DTO;
using PakTrack.Models;

namespace PakTrack.DAL.Interfaces
{
    public interface IPressureRepository : IRepository<PressureDTO>, ISensorDbRepository<Pressure>
    {
    }
}