using System.Linq;
using LiteDB;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PakTrack.DAL.Interfaces;
using PakTrack.DAL.Repositories;
using PakTrack.Test.Base;

namespace PakTrack.Test.PressureTest
{
    [TestClass]
    public class PressureRepositoryTest: BaseTestClass
    {
        private readonly IPressureRepository _pressureRepository;

        public PressureRepositoryTest()
        {
            _pressureRepository = new PressureRepository(PakTrackDbContext);
        }

        [TestMethod]
        public void TestCanGetAllPressure()
        {
            //Arrange

            //Act
            var pressures = _pressureRepository.GetAll();

            //Assert
            Assert.IsTrue(pressures.Any());
        }

        [TestMethod]
        public void TestCanGetPressureByTruckAndPackageId()
        {
            //Arrange
            var truckId = "HW BACKUP 1";
            var packageId = "PACK1";

            //Act
            var pressures = _pressureRepository.GetByTruckAndPackageId(truckId, packageId);

            //Assert
            Assert.IsTrue(pressures.Any());
        }

        [TestMethod]
        public void TestCanGetPressureById()
        {
            //Arrange
            var pressureId = new ObjectId("579970f209690934ef6807eb");

            //Act
            var pressure = _pressureRepository.GetById(pressureId);

            //Assert
            Assert.IsNotNull(pressure);
        }
    }
}
