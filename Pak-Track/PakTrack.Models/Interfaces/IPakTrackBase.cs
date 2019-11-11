using System;
using LiteDB;

namespace PakTrack.Models.Interfaces
{
    public interface IPakTrackBase
    {
        ObjectId Id { get; set; }
        bool IsAboveThreshold { get; set; }
        string PackageId { get; set; }
        string SensorId { get; set; }
        long Timestamp { get; set; }
        string TruckId { get; set; }
        DateTime? UpdatedAd { get; set; }
    }
}