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
    public class LightSensorRepository : BaseSensorRepository, ILightSensorRepository
    {
        public LightSensorRepository(ISensorReaderContext sensorReader) : base(sensorReader)
        {
        }

        public async Task<IEnumerable<Light>> ReadSensorData()
        {
            var packets = await GetAllPacketsForSensor(SensorConstants.GET_LIGHT);
            return ExtractLightData(packets);
        }

        public IEnumerable<Light> ExtractLightData(IEnumerable<Packet> packets)
        {
            var temps = packets.Select(p => new Light
            {
                PackageId = PackageId,
                TruckId = TruckId,
                SensorId = SensorId,
                Unit = SensorConstants.DEFAULT_LIGHT_UNIT,
                Value = new LightValue
                {
                    R = p.GetR(),
                    G = p.GetG(),
                    B = p.GetB(),
                    ClearLight = p.GetClearLight(),
                    Illuminance = p.GetIlluminance()
                },
                Timestamp = p.GetTimeStampMilliSeconds(),
                IsAboveThreshold = p.IsAboveThreshold()
            });

            return temps;
        }
    }
}