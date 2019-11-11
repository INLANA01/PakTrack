using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PakTrack.DAL.Interfaces;
using PakTrack.DAL.Repositories;
using PakTrack.Test.Base;

namespace PakTrack.Test.LightTest
{
    [TestClass]
    public class LightRepositoryTest:BaseTestClass
    {
        private readonly ILightRepository _lightRepository;
        public LightRepositoryTest()
        {
            _lightRepository = new LightRepository(PakTrackDbContext);
        }

        [TestMethod]
        public void TestCanInitializeLightRepository()
        {
            //Arrange

            //Assert
            Assert.IsNotNull(_lightRepository);
        }

        [TestMethod]
        public void TestCanLightEventsFromDatabase()
        {
            //Arrange

            //Act
            var lightEvents = _lightRepository.GetAll();

            //Assert
            Assert.IsTrue(lightEvents.Any());
        }
    }
}
