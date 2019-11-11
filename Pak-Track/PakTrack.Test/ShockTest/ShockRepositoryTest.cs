using System;
using System.Linq;
using LiteDB;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PakTrack.DAL.Interfaces;
using PakTrack.DAL.Repositories;
using PakTrack.Test.Base;

namespace PakTrack.Test.ShockTest
{
    [TestClass]
    public class ShockRepositoryTest: BaseTestClass
    {
        private readonly IShockRepository _shockRepository;

        public ShockRepositoryTest()
        {
            _shockRepository = new ShockRepository(PakTrackDbContext);
        }

        [TestMethod]
        public void TestCanGetAllShocks()
        {
            //Arrange

            //Act
            var shocks = _shockRepository.GetAll();

            //Assert
            Assert.IsTrue(shocks.Any());
        }

        [TestMethod]
        public void TestCanGetShocksByTruckAndPackageId()
        {
            //Arrange
            var truckId = "HW BACKUP 1";
            var packageId = "PACK1";

            //Act
            var shocks = _shockRepository.GetByTruckAndPackageId(truckId, packageId);

            //Assert
            Assert.IsTrue(shocks.Any());
        }

        [TestMethod]
        public void TestCanGetShockById()
        {
            //Arrange
            var shockId = new ObjectId("57997080b3486b23f5104335");

            //Act
            var shock = _shockRepository.GetById(shockId);

            //Assert
            Assert.IsNotNull(shock);
        }
    }
}
