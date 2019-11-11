using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PakTrack.DAL.Base;
using PakTrack.DAL.Interfaces.Sensor;
using PakTrack.Models;
using PakTrack.Models.Sensor;
using PakTrack.Utilities;

namespace PakTrack.DAL.Repositories.Sensor
{
    public class VibrationSensorRepository : BaseSensorRepository, IVibrationSensorRepository
    {
        public VibrationSensorRepository(ISensorReaderContext sensorReader) : base(sensorReader)
        {
        }

        public async Task<IEnumerable<Vibration>> ReadSensorData()
        {
            var packets = await GetAllPacketsForSensor(SensorConstants.GET_VIBRATION);
            return ExtractVibrationData(packets);
        }

        public IEnumerable<Vibration> ExtractVibrationData(IEnumerable<Packet> packets)
        {
            var vibrationsDictionary = new Dictionary<long, Vibration>();
            foreach (var packet in packets)
            {
               
                var timestamp = packet.getDate().Ticks;
                Vibration vibrationEvent;
                if (!vibrationsDictionary.ContainsKey(timestamp))
                {
                    vibrationEvent = new Vibration
                    {
                        PackageId = PackageId,
                        TruckId = TruckId,
                        SensorId = SensorId,
                        Timestamp = packet.getDate().Ticks,
                        IsAboveThreshold = packet.IsAboveThreshold(),
                        MaximumX = new MaxIndex(),
                        MaximumY = new MaxIndex(),
                        MaximumZ = new MaxIndex(),
                        RMS = new ThreeAxisInformation(),
                        MaximumPSD = new ThreeAxisInformation(),
                        GRMS = new ThreeAxisInformation(),
                        MaximumVector = new MaxIndex(),
                        Kurtosis = new ThreeAxisInformation(),
                        X = new List<double>(),
                        Y = new List<double>(),
                        Z = new List<double>(),
                        Vector = new List<double>(),
                        IsProcessed = true
                    };
                    vibrationsDictionary.Add(timestamp, vibrationEvent);
                }
                vibrationEvent = vibrationsDictionary[timestamp];
                if (packet.IsX())
                {
                    var vibrationX = packet.GetVibration().ToList();
                    vibrationEvent.X = vibrationX;
                    var max = SensorUtils.GetAbsoluteMax(vibrationX);
                    vibrationEvent.MaximumX.Value = max.Item2;
                    vibrationEvent.MaximumX.Index = max.Item1;
                    vibrationEvent.PSDX = PsdGrms.GetPsd(vibrationX);
                    vibrationEvent.RMS.X = PsdGrms.GetRms(vibrationX);
                    vibrationEvent.MaximumPSD.X = vibrationEvent.PSDX.Max();
                    vibrationEvent.GRMS.X =
                        PsdGrms.GetGrms(PsdGrms.GetDataWithFrequency(vibrationEvent.PSDX.ToList()));
                    vibrationEvent.Kurtosis.X = PsdGrms.Kurtosis(vibrationX);


                }
                else if (packet.IsY())
                {
                    var vibrationY = packet.GetVibration().ToList();
                    var max = SensorUtils.GetAbsoluteMax(vibrationY);
                    vibrationEvent.MaximumY.Value = max.Item2;
                    vibrationEvent.MaximumY.Index = max.Item1;
                    vibrationEvent.Y = vibrationY;
                    vibrationEvent.PSDY = PsdGrms.GetPsd(vibrationY);
                    vibrationEvent.RMS.Y = PsdGrms.GetRms(vibrationY);
                    vibrationEvent.MaximumPSD.Y = vibrationEvent.PSDY.Max();
                    vibrationEvent.GRMS.Y =
                        PsdGrms.GetGrms(PsdGrms.GetDataWithFrequency(vibrationEvent.PSDY.ToList()));
                    vibrationEvent.Kurtosis.Y = PsdGrms.Kurtosis(vibrationY);

                }
                else if (packet.IsZ())
                {
                    var vibrationZ = packet.GetVibration().ToList();
                    var max = SensorUtils.GetAbsoluteMax(vibrationZ);
                    vibrationEvent.MaximumZ.Value = max.Item2;
                    vibrationEvent.MaximumZ.Index = max.Item1;
                    vibrationEvent.Z = vibrationZ;
                    vibrationEvent.PSDZ = PsdGrms.GetPsd(vibrationZ);
                    vibrationEvent.MaximumPSD.Z = vibrationEvent.PSDZ.Max();
                    vibrationEvent.RMS.Z = PsdGrms.GetRms(vibrationZ);
                    vibrationEvent.GRMS.Z =
                        PsdGrms.GetGrms(PsdGrms.GetDataWithFrequency(vibrationEvent.PSDZ.ToList()));
                    vibrationEvent.Kurtosis.Z = PsdGrms.Kurtosis(vibrationZ);

                }
                if (vibrationEvent.X.Count > 0 &&
                    vibrationEvent.Y.Count > 0 &&
                    vibrationEvent.Z.Count > 0)
                {
                    var vector = SensorUtils.GetVector(vibrationEvent.X,
                        vibrationEvent.Y, vibrationEvent.Z).ToList();
                    var max = SensorUtils.GetAbsoluteMax(vector);
                    vibrationEvent.Vector = vector;
                    vibrationEvent.PSDVector = PsdGrms.GetPsd(vector);
                    vibrationEvent.MaximumVector.Value = max.Item2;
                    vibrationEvent.MaximumVector.Index = max.Item1;
                    vibrationEvent.RMS.Vector = PsdGrms.GetRms(vector);
                    vibrationEvent.MaximumPSD.Vector = vibrationEvent.PSDVector.Max();
                    vibrationEvent.GRMS.Vector =
                        PsdGrms.GetGrms(PsdGrms.GetDataWithFrequency(vibrationEvent.PSDVector.ToList()));
                }
            }
            return vibrationsDictionary.Values;
        }
    }
}