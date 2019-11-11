using System.Linq;
using LiteDB;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PakTrack.DAL.Interfaces;
using PakTrack.Test.Base;
using PakTrack.DAL.Repositories;

namespace PakTrack.Test.ShellTest
{
    [TestClass]
    public class MainShellTest: BaseTestClass
    {
        private readonly IOrganizationRepository _organizationRepository;
        public MainShellTest()
        {
            _organizationRepository = new OrganizationRepository(PakTrackDbContext);
        }

        [TestMethod]
        public void TestCanGetOrganizationInformation()
        {
            //Arrange

            //Act
            var allOrganization = _organizationRepository.GetAll();

            //Assert
            Assert.IsTrue(allOrganization.Any());

        }

        [TestMethod]
        public void TestCanGetOrganizationByUserName()
        {
            //Arrange
            var userName = "root";

            //Act
            var information = _organizationRepository.GetByUserName(userName);
            var organizationId = information.Id.ToString();

            //Assert
            Assert.IsTrue(organizationId != "");

        }

        [TestMethod]
        public void TestCanGetOrganizationById()
        {
            //Arrange
            var organizationId = new ObjectId("57fb21694027ea0f105050ab");

            //Act
            var organization = _organizationRepository.GetById(organizationId);

            //Assert
            Assert.IsNotNull(organization);
        }

        [TestMethod]
        public void TestCanGetUserInformationByUser()
        {
            //Arrange
            var userName = "root";

            //Act
            var user = _organizationRepository.GetUserInformationByUsername(userName);

            //Assert
            Assert.IsNotNull(user);
        }

    }
}


