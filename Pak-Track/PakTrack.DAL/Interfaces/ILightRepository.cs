using PakTrack.DAL.Interfaces.Sensor;
using PakTrack.Models;

namespace PakTrack.DAL.Interfaces
{
    public interface ILightRepository:IRepository<Light>, ISensorDbRepository<Light>
    {
    }
}