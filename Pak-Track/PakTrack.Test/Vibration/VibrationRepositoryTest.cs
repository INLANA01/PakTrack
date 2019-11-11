using System;
using System.Linq;
using LiteDB;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PakTrack.DAL.Interfaces;
using PakTrack.DAL.Repositories;
using PakTrack.Test.Base;

namespace PakTrack.Test.Vibration
{
    [TestClass]
    public class VibrationRepositoryTest:BaseTestClass
    {
        private readonly IVibrationRepository _vibrationRepository;
        public VibrationRepositoryTest()
        {
            _vibrationRepository = new VibrationRepository(PakTrackDbContext);
        }
        [TestMethod]
        public void TestCanGetAllVibrations()
        {
            //Arrange

            //Act 
            var vibrations = _vibrationRepository.GetAll().ToList();

            //Assert
            Assert.IsTrue(vibrations.Any());
        }

        [TestMethod]
        public void TestCanGetVibrationById()
        {
            //Arrange
            var vibrationId = new ObjectId("5799701fb3486b23f51042d5");

            //Act
            var vibration = _vibrationRepository.GetById(vibrationId);

            //Assert
            Assert.IsNotNull(vibration);
        }

        [TestMethod]
        public void TestCanGetVibrationsByTruckAndPackageId()
        {
            //Arrange
            var truckId = "HW BACKUP 1";
            var packageId = "PACK1";

            //Act
            var vibrations = _vibrationRepository.GetByTruckAndPackageId(truckId, packageId);

            //Assert
            Assert.IsTrue(vibrations.Any());
        }
    }
}
