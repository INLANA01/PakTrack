using PakTrack.DAL.Interfaces.Sensor;
using PakTrack.Models;

namespace PakTrack.DAL.Interfaces
{
    public interface IShockRepository : IRepository<Shock>, ISensorDbRepository<Shock>
    {
    }
}