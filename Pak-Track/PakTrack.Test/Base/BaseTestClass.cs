using System.Configuration;
using System.IO;
using PakTrack.DAL.Context;
using PakTrack.DAL.Interfaces;

namespace PakTrack.Test.Base
{
    public abstract class BaseTestClass
    {
        public string ConnectionString { get { return GetConnectionString(); } }
        public IPakTrackDbContext PakTrackDbContext { get; set; }
        /// <summary>
        /// Get the database location from the App.Config file then construct a connection string
        /// using the current directory combina with the database location
        /// </summary>
        /// <returns>string</returns>
        private string GetConnectionString()
        {
            var path = Directory.GetCurrentDirectory();
            var dbLocation = ConfigurationManager.AppSettings["DbLocation"];
            var dbPath = Path.GetFullPath(Path.Combine(path, dbLocation));
            return dbPath;
        }

        protected BaseTestClass()
        {
            PakTrackDbContext = new PakTrackDbContext(ConnectionString);
        }
    }
}