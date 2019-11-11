using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;

namespace PakTrack.DAL.Interfaces.Sensor
{
    public interface ISensorRepository<T> where T:class
    {
        Task<IEnumerable<T>> ReadSensorData();
    }
}
