using System.Collections.Generic;
using System.Threading.Tasks;
using PakTrack.DAL.Base;
using PakTrack.DAL.Interfaces.Sensor;
using PakTrack.Models.Sensor;
using PakTrack.Utilities;

namespace PakTrack.DAL.Repositories.Sensor
{
    public class FlashDataSensorRepository : BaseSensorRepository, IFlashDataSensorRepository
    {
        public FlashDataSensorRepository(ISensorReaderContext sensorReader) : base(sensorReader)
        {
        }

        public async Task<IEnumerable<Packet>> ReadSensorData()
        {
            var sensorType = SensorConstants.GET_FLASH;
            var packets = await GetAllPacketsForSensor(sensorType);
            return packets;
        }
    }
}