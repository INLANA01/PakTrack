using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using PakTrack.Core;
using PakTrack.Models.Sensor;

namespace PakTrack.DAL.Interfaces.Sensor
{
    public interface ISensorReaderContext
    {
//       
        string Notes();
        string TruckId();
        string PackageId();
        string SensorId();
        void Connect(string port);
        void Disconneect();
        byte[] ReadFramedPacket(bool readSdCard, BinaryReader input = null);
        Task<List<Packet>> ReadSdCardData(byte[] sensorType);
        Packet GetConfiguration();
        int GetBatteryLevel();
        string GetNote();
        string GetBoardId();
        bool IsFlashDataAvailable();
        bool Configure(byte[] configPacket, byte[] note);
        int GetTotalPacketsLength();
        int GetTotalReadPacketsLength();
    }
}