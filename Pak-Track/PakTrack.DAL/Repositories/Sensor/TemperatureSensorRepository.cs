using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PakTrack.DAL.Base;
using PakTrack.DAL.Context;
using PakTrack.DAL.Interfaces.Sensor;
using PakTrack.Models;
using PakTrack.Models.Sensor;
using PakTrack.Utilities;

namespace PakTrack.DAL.Repositories.Sensor
{
    public class TemperatureSensorRepository : BaseSensorRepository, ITemperatureSensorRepository
    {
        public TemperatureSensorRepository(ISensorReaderContext context): base(context)
        {
        }

        public async Task<IEnumerable<Temperature>> ReadSensorData()
        {
            var packets = await GetAllPacketsForSensor(SensorConstants.GET_TEMPERATURE);
            return ExtractTemperatureData(packets);
        }

        public IEnumerable<Temperature> ExtractTemperatureData(IEnumerable<Packet> packets)
        {
            var temps = packets.Select(p => new Temperature
            {
                PackageId = PackageId,
                TruckId = TruckId,
                SensorId = SensorId,
                Unit = SensorConstants.DEFAULT_TEMPERATURE_UNIT,
                Value = p.GetTemperature(),
                Timestamp = p.GetTimeStampMilliSeconds(),
                IsAboveThreshold = p.IsAboveThreshold()
            });

            return temps;

        }
    }
}
