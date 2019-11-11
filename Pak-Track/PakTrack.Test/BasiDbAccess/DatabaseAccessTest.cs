using System.Configuration;
using System.IO;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PakTrack.DAL.Context;

namespace PakTrack.Test.BasiDbAccess
{
    [TestClass]
    public class DatabaseAccessTest
    {
        [TestMethod]
        public void TestHasAccessToDatabasePath()
        {
            //Arrange
            var path = Directory.GetCurrentDirectory();
            var dbLocation = ConfigurationManager.AppSettings["DbLocation"];
            var dbPath = Path.GetFullPath(Path.Combine(path, dbLocation));
            
            //Act
            var dbFileName = Path.GetFileNameWithoutExtension(dbPath);


            //Assert
            Assert.AreEqual(dbFileName, "PaktrackDb");
        }

        [TestMethod]
        public void TestCanGetLightObjectsFromLightCollection()
        {
            //Arrange
            var path = Directory.GetCurrentDirectory();
            var dbLocation = ConfigurationManager.AppSettings["DbLocation"];
            var dbPath = Path.GetFullPath(Path.Combine(path, dbLocation));

            var pakTrackDbContext = new PakTrackDbContext(dbPath);

            //Act
            var lightEvents = pakTrackDbContext.Lights.FindAll().ToList();

            //Assert
            Assert.IsTrue(lightEvents.Count > 0);

        }
    }
}
