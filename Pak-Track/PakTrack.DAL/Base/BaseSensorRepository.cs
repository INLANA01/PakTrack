using System.Collections.Generic;
using System.Threading.Tasks;
using PakTrack.DAL.Interfaces.Sensor;
using PakTrack.Models.Sensor;

namespace PakTrack.DAL.Base
{
    public class BaseSensorRepository
    {
        protected readonly ISensorReaderContext SensorReader;

        protected BaseSensorRepository(ISensorReaderContext sensorReader)
        {
            SensorReader = sensorReader;
        }

        protected string TruckId => SensorReader.TruckId();
        protected string PackageId => SensorReader.PackageId();
        protected string SensorId => SensorReader.SensorId();
        protected string Notes => SensorReader.Notes();

        protected async Task<List<Packet>> GetAllPacketsForSensor(byte[] sensorType)
        {
            return await SensorReader.ReadSdCardData(sensorType);
        }

        protected Packet GetConfiguration()
        {
            return SensorReader.GetConfiguration();
        }

        public int GetBatteryLevel()
        {
            return SensorReader.GetBatteryLevel();
        }

        public string GetNote()
        {
            return SensorReader.GetNote();
        }

        public string GetBoardId()
        {
            return SensorReader.GetBoardId();
        }

    }
}
