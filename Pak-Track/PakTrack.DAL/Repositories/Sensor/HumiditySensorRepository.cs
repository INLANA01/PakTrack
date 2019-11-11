using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PakTrack.DAL.Base;
using PakTrack.DAL.Context;
using PakTrack.DAL.Interfaces.Sensor;
using PakTrack.Models;
using PakTrack.Models.Sensor;
using PakTrack.Utilities;

namespace PakTrack.DAL.Repositories.Sensor
{
    public class HumiditySensorRepository : BaseSensorRepository, IHumiditySensorRepository
    {
        public HumiditySensorRepository(ISensorReaderContext context): base(context)
        {
        }

        public async Task<IEnumerable<Humidity>> ReadSensorData()
        {
            var sensorType = SensorConstants.GET_HUMIDITY;
            var packets = await GetAllPacketsForSensor(sensorType);
            return ExtractHumiditieData(packets);
        }

        public IEnumerable<Humidity> ExtractHumiditieData(IEnumerable<Packet> packets)
        {
            var humidity = packets.Select(p => new Humidity
            {
                PackageId = this.PackageId,
                TruckId = this.TruckId,
                SensorId = this.SensorId,
                Unit = SensorConstants.DEFAULT_HUMIDITY_UNIT,
                Value = p.GetHumidity(),
                Timestamp = p.GetTimeStampMilliSeconds(),
                IsAboveThreshold = p.IsAboveThreshold()
            });

            return humidity;
        }
    }
}
