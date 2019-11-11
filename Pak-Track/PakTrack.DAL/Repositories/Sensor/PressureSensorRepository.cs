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
    public class PressureSensorRepository: BaseSensorRepository, IPressureSensorRepository
    {
        public PressureSensorRepository(ISensorReaderContext sensorReader) : base(sensorReader)
        {
        }

        public async Task<IEnumerable<Pressure>> ReadSensorData()
        {
            var packets = await GetAllPacketsForSensor(SensorConstants.GET_PRESSURE);
            return ExtractPressurekData(packets);

        }

        public IEnumerable<Pressure> ExtractPressurekData(IEnumerable<Packet> packets)
        {
            var temps = packets.Select(p => new Pressure
            {
                PackageId = PackageId,
                TruckId = TruckId,
                SensorId = SensorId,
                Unit = SensorConstants.DEFAULT_PRESSURE_UNIT,
                Value = p.GetPressure(),
                Timestamp = p.GetTimeStampMilliSeconds(),
                IsAboveThreshold = p.IsAboveThreshold()
            });

            return temps;
        }
    }
}