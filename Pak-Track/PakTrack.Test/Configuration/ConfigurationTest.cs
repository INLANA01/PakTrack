using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PakTrack.DAL.Context;
using PakTrack.DAL.Interfaces;
using PakTrack.DAL.Repositories;
using PakTrack.Test.Base;

namespace PakTrack.Test.Configuration
{
    [TestClass]
    public class ConfigurationTest:BaseTestClass
    {
        private readonly IConfigurationRepository _configurationRepository;

        public ConfigurationTest()
        {
            _configurationRepository = new ConfigurationRepository(PakTrackDbContext);
        }

        [TestMethod]
        public void TestCanGetAllConfiguration()
        {
            //Arrange

            //Act
            var organizations = _configurationRepository.GetAll().ToList();

            ////Assert
            Assert.IsTrue(organizations.Any());
        }

        [TestMethod]
        public void TestCanGetConfigurationByOrganizationId()
        {
            //Arrange
            var organizationId = "5375fee944ae42f6704f102f"; //Used for development

            //Act
            var configurations = _configurationRepository.GetByOrganizationId(organizationId);

            //Assert
            Assert.IsTrue(configurations.Any());

        }

        [TestMethod]
        public void TestCanGetConfigurationsByTruckAndPackageId()
        {
            //Arrange

            //Act
            var configurations = _configurationRepository.GetByTruckAndPackageId("HW BACKUP 1", "PACK1");

            //Assert
            Assert.IsNotNull(configurations);
        }

        [TestMethod]
        public void TestCanGetTrucksByOrganizationId()
        {
            //Arrange
            var organizationId = "5375fee944ae42f6704f102f"; //Used for development

            //Act
            var trucks = _configurationRepository.GetTrucksByOrganizationId(organizationId);

            //Assert
            Assert.IsTrue(trucks.Any());

        }

        [TestMethod]
        public void TestCanGetPackagesByTruckId()
        {
            //Arrange

            //Act
            var packages = _configurationRepository.GetPackageByTruckId("HW BACKUP 1");

            //Assert
            Assert.IsNotNull(packages);

        }
    }
}
