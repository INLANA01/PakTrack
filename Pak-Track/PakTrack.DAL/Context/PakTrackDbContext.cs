using LiteDB;
using PakTrack.DAL.Base;
using PakTrack.DAL.Interfaces;
using PakTrack.Models;

namespace PakTrack.DAL.Context
{
    public class PakTrackDbContext : LiteDatabase, IPakTrackDbContext
    {
        public PakTrackDbContext(string connectionString) : base(connectionString)
        {
            //Initialize collections
            
            Lights = GetCollection<Light>(PakTrackCollection.Light);
            Pressures = GetCollection<Pressure>(PakTrackCollection.Pressure);
            Temperatures = GetCollection<Temperature>(PakTrackCollection.Temperature);
            Humidities = GetCollection<Humidity>(PakTrackCollection.Humidity);
            Shocks = GetCollection<Shock>(PakTrackCollection.Shock);
            Vibrations = GetCollection<Vibration>(PakTrackCollection.Vibration);
            Organizations = GetCollection<Organization>(PakTrackCollection.Organization);
            VibrationReports = GetCollection<VibrationReport>(PakTrackCollection.VibrationReport);
            Organizations = GetCollection<Organization>(PakTrackCollection.Organization);
            Configurations = GetCollection<Configuration>(PakTrackCollection.Configuration);

            TestCollection = GetCollection<BsonDocument>(PakTrackCollection.Configuration);
        }


        public LiteCollection<Light> Lights { get; set; }

        public LiteCollection<Pressure> Pressures { get; set; }

        public LiteCollection<Temperature> Temperatures { get; set; }

        public LiteCollection<Humidity> Humidities { get; set; }

        public LiteCollection<Shock> Shocks { get; set; }

        public LiteCollection<Vibration> Vibrations { get; set; }

        public LiteCollection<BsonDocument> TestCollection { get; set; }

        public LiteCollection<Organization> Organizations { get; set; }

        public LiteCollection<VibrationReport> VibrationReports { get; set; }

        public LiteCollection<Configuration> Configurations { get; set; }


    }
}