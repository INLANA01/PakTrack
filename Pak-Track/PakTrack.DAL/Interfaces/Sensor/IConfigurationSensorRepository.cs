using PakTrack.Models;

namespace PakTrack.DAL.Interfaces.Sensor
{
    public interface IConfigurationSensorRepository
    {
        Configuration GetDeviceConfiguration();
    }
}