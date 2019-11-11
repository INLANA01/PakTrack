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
    public class ConsolidatedReportRepositoryTest: BaseTestClass
    {
        private readonly IVibrationConsolidatedReport _consolidatedReport;

        public ConsolidatedReportRepositoryTest()
        {
            _consolidatedReport = new VibrationConsolidatedReport(PakTrackDbContext);
        }

        [TestMethod]
        public void TestCanGetAllReports()
        {
            //Arrange

            //Act
            var reports = _consolidatedReport.GetAll().ToList();

            //Assert
            Assert.IsTrue(reports.Any());
        }

        [TestMethod]
        public void TestCanGetRegularReportsByTruckAndPackageId()
        {
            //Arrange
            var truckId = "HW BACKUP 1";
            var packageId = "PACK1";

            //Act
            var regularReports = _consolidatedReport.GetRegularByTruckAndPackageId(truckId, packageId);

            //Assert
            Assert.IsTrue(regularReports.Any());
        }

        [TestMethod]
        public void TestCanGetCustomReportsByTruckAndPackageId()
        {
            //Arrange
            var truckId = "HW BACKUP 1";
            var packageId = "PACK1";

            //Act
            var customReports = _consolidatedReport.GetCustomByTruckAndPackageId(truckId, packageId);

            //Assert
            Assert.IsTrue(customReports.Any());
        }

        [TestMethod]
        public void TestCanGetReportById()
        {
            //Arrange
            var reportId = new ObjectId("57b1059a7579ae0823560c28");

            //Act
            var report = _consolidatedReport.GetById(reportId);

            //Assert
            Assert.IsNotNull(report);
        }


    }
}
