using LiteDB;
using PakTrack.Models;

namespace PakTrack.DAL.Interfaces
{
    public interface IPakTrackDbContext
    {
        LiteCollection<Configuration> Configurations { get; set; }
        LiteCollection<Humidity> Humidities { get; set; }
        LiteCollection<Light> Lights { get; set; }
        LiteCollection<Organization> Organizations { get; set; }
        LiteCollection<Pressure> Pressures { get; set; }
        LiteCollection<Shock> Shocks { get; set; }
        LiteCollection<Temperature> Temperatures { get; set; }
        LiteCollection<VibrationReport> VibrationReports { get; set; }
        LiteCollection<Vibration> Vibrations { get; set; }

        LiteCollection<BsonDocument> TestCollection { get; set; }
    }
}