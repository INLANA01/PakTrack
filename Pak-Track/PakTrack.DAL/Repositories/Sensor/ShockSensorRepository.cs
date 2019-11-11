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
    public class ShockSensorRepository:BaseSensorRepository, IShockSensorRepository
    { 
        public ShockSensorRepository(ISensorReaderContext sensorReader) : base(sensorReader) 
        {
        }

        public async Task<IEnumerable<Shock>> ReadSensorData()
        {
            var packets = await GetAllPacketsForSensor(SensorConstants.GET_SHOCK);
            return ExtractShockData(packets);
        }

        public IEnumerable<Shock> ExtractShockData(IEnumerable<Packet> packets)
        {
            var shocksDictionary = new Dictionary<long, Shock>();
            foreach (var packet in packets)
            {
                var timestamp = packet.GetTimeStampMilliSeconds();
                if (!shocksDictionary.ContainsKey(timestamp))
                {
                    var shock = new Shock
                    {
                        PackageId = PackageId,
                        TruckId = TruckId,
                        SensorId = SensorId,
                        Timestamp = packet.GetTimeStampMilliSeconds(),
                        IsAboveThreshold = packet.IsAboveThreshold(),
                        IsProcessed = true,
                        MaximumX = new MaxIndex(),
                        MaximumY = new MaxIndex(),
                        MaximumZ = new MaxIndex(),
                        MaximumVector = new MaxIndex(),
                        Average = new ThreeAxisInformation(),
                        X = new List<double>(),
                        Y = new List<double>(),
                        Z = new List<double>(),
                        SRS = new SRS(),
                        IsInstantaneous = packet.IsInstanteaous()
                        
                    };
                    shocksDictionary.Add(timestamp, shock);
                }
                var shockEvent = shocksDictionary[timestamp];
                if (packet.IsX())
                {
                    var shockX = packet.GetShock().ToList();
                    shockEvent.X = shockX;
                    var max = SensorUtils.GetAbsoluteMax(shockX);
                    shockEvent.MaximumX.Value = max.Item2;
                    shockEvent.MaximumX.Index = max.Item1;
                    shockEvent.Average.X = SensorUtils.GetAverage(shockX);
                    var srs = PsdGrms.SRS(shockX);
                    shockEvent.SRS.X = new SRSAxis
                    {
                        Frequency = srs.Item1,
                        Positive = srs.Item2,
                        Negative = srs.Item3
                    };
                }
                    
                else if (packet.IsY())
                {
                    var shockY = packet.GetShock().ToList();
                    shockEvent.Y = shockY;
                    var max = SensorUtils.GetAbsoluteMax(shockY);
                    shockEvent.MaximumY.Value = max.Item2;
                    shockEvent.MaximumY.Index = max.Item1;
                    shockEvent.Average.Y = SensorUtils.GetAverage(shockY);
                    var srs = PsdGrms.SRS(shockY);
                    shockEvent.SRS.Y = new SRSAxis
                    {
                        Frequency = srs.Item1,
                        Positive = srs.Item2,
                        Negative = srs.Item3
                    };
                }
                else if (packet.IsZ())
                {
                    var shockZ = packet.GetShock().ToList();
                    shockEvent.Z = shockZ;
                    var max = SensorUtils.GetAbsoluteMax(shockZ);
                    shockEvent.MaximumZ.Value = max.Item2;
                    shockEvent.MaximumZ.Index = max.Item1;
                    shockEvent.Average.Z = SensorUtils.GetAverage(shockZ);
                    var srs = PsdGrms.SRS(shockZ);
                    shockEvent.SRS.Z = new SRSAxis
                    {
                        Frequency = srs.Item1,
                        Positive = srs.Item2,
                        Negative = srs.Item3
                    };
                }
                if (shockEvent.X.Count > 0 &&
                    shockEvent.Y.Count > 0 &&
                    shockEvent.Z.Count > 0)
                {
                    var vector = SensorUtils.GetVector(shockEvent.X,
                        shockEvent.Y, shockEvent.Z).ToList();
                    shockEvent.Vector = vector;
                    var max = SensorUtils.GetAbsoluteMax(vector);
                    shockEvent.MaximumVector.Value = max.Item2;
                    shockEvent.MaximumVector.Index = max.Item1;
                    shockEvent.Average.Vector = SensorUtils.GetAverage(vector);
                    var maxIndex = max.Item1 + 1; //add 1 to get correct millisecond as array starts with 0
                    shockEvent.DropHeight = PsdGrms.GetDropHeight(maxIndex);
                    shockEvent.Orientation = GetDropOrientation(shockEvent);
                    shockEvent.GRMS = GetShockGrms(shockEvent.MaximumX.Value, shockEvent.MaximumY.Value,
                        shockEvent.MaximumZ.Value);

                }
            }
            return shocksDictionary.Values;
        }

        private List<int> GetDropOrientation(Shock shock)
        {
            var maxAxisValues = new Dictionary<string, double>
            {
                {"x", Math.Abs(shock.MaximumX.Value)},
                {"y", Math.Abs(shock.MaximumY.Value)},
                {"z", Math.Abs(shock.MaximumZ.Value)}
            };

            var sortedMaxAxisValues = maxAxisValues.OrderByDescending(x => x.Value);
            var maxAxis = sortedMaxAxisValues.ToList();
            var faces = new List<int>();
            var key = maxAxis.First().Key;
            var value = maxAxis.First().Value;
            faces.Add(GetFace(key, GetAxisValue(key, shock)));
            var maxValue = value;
            maxAxis.RemoveAt(0);
            var faceDetectionThreshold = 1.5;
            foreach (var keyValuePair in maxAxis)
            {
                var ratio = Math.Min(maxValue, keyValuePair.Value) > 0.0
                    ? Math.Max(maxValue, keyValuePair.Value) / Math.Min(maxValue, keyValuePair.Value)
                        : Math.Max(maxValue, keyValuePair.Value);
                if (Math.Abs(ratio) < faceDetectionThreshold)
                {
                    faces.Add(GetFace(keyValuePair.Key,GetAxisValue(key,shock)));
                }
            }
            return faces;
        }

        private int GetFace(string axis, double value)
        {
            var isNegative = !(value >= 0);
            if (axis.Equals("x"))
            {
                return isNegative ? 4 : 2;
            }
            if (axis.Equals("y"))
            {
                return isNegative ? 6 : 5;
            }
            return isNegative ? 1 : 3;//for z
        }

        private double GetAxisValue(string key, Shock shock)
        {
            if (key.Equals("x"))
                return shock.MaximumX.Value;
            if (key.Equals("y"))
                return shock.MaximumY.Value;
            if (key.Equals("z"))
                return shock.MaximumZ.Value;
            return shock.MaximumVector.Value;
        }

        private double GetShockGrms(double maxX, double maxY, double maxZ)
        {
            return Math.Sqrt((Math.Pow(maxX, 2) + Math.Pow(maxY, 2) + Math.Pow(maxZ, 2)) / 3);
        }
    }
}