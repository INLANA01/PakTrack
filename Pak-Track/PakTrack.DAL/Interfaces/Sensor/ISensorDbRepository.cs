using System.Collections.Generic;
using System.Threading.Tasks;

namespace PakTrack.DAL.Interfaces.Sensor
{
    public interface ISensorDbRepository<in T> where T:class
    {
        Task PushToDb(IEnumerable<T> sensorData, bool isFlash = false);
    }
}