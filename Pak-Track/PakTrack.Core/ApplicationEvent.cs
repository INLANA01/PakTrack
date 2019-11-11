using PakTrack.Models.Sensor;

namespace PakTrack.Core
{
    public class ApplicationEvent
    {
        public delegate void NotifyPacketCreated(Packet pack);

        public delegate void SaveSensorInfo<in T>(T info);

        public delegate void DataReadComplete();

    }
}