using PakTrack.DAL.Interfaces.Sensor;
using PakTrack.Models;

namespace PakTrack.DAL.Interfaces
{
    public interface IVibrationRepository:IRepository<Vibration>, ISensorDbRepository<Vibration>
    {
        
    }
}