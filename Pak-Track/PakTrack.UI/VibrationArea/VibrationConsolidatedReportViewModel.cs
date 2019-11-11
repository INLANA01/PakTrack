using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using CsvHelper;
using LiteDB;
using Microsoft.Win32;
using PakTrack.BL.Interfaces;
using PakTrack.BL.Models;
using PakTrack.Core.Base;
using PakTrack.DAL.Interfaces;
using PakTrack.Models;
using PakTrack.UI.Events;
using PakTrack.UI.Utilities;
using PakTrack.Utilities;
using Prism.Commands;
using Prism.Events;
using Prism.Regions;

namespace PakTrack.UI.VibrationArea
{
    public class VibrationConsolidatedReportViewModel : PakTrackBindableBase, INavigationAware
    {
        #region Private members
        private readonly IEventAggregator _eventAggregator;
        private readonly IVibrationConsolidatedReport _consolidatedReport;
        private readonly IReportManager _reportManager;
        private readonly IRegionManager _regionManager;
        private readonly IVibrationRepository _vibrationRepository;
        private readonly IVibrationConsolidatedReport _vibrationConsolidatedReport;
        

        private IEnumerable<VibrationReportSummary> _customReports;

        private IEnumerable<VibrationReportSummary> _regularReports;

        private bool _isReportNotGenerated;

        #endregion

        #region Class constructor

        /// <param name="consolidatedReport"></param>
        /// <param name="reportManager"></param>
        /// <param name="regionManager"></param>
        /// <param name="vibrationRepository"></param>
        /// <param name="vibrationConsolidatedReport"></param>
        /// <param name="eventAggregator"></param>
        public VibrationConsolidatedReportViewModel(IVibrationConsolidatedReport consolidatedReport,
            IReportManager reportManager, IRegionManager regionManager, 
            IVibrationRepository vibrationRepository, 
            IEventAggregator eventAggregator)
        {
            _eventAggregator = eventAggregator;
            _consolidatedReport = consolidatedReport;
            _reportManager = reportManager;
            _regionManager = regionManager;
            _vibrationRepository = vibrationRepository;
            ShowReportDetailsCommand = new DelegateCommand<ObjectId>(ShowReportDetails);
//            GeneraRegularReportCommand = new DelegateCommand(GenerateRegularReport);
            CustomReportCommand = new DelegateCommand(NavigateCustomReportPage);
            DeleteVibrationReportEventCommand = new DelegateCommand<ObjectId>(OnDeleteReport);
            ReportCsvCommand = new DelegateCommand<ObjectId>(ReportToCsv);
        }

        #endregion

        #region Public properties

        public IEnumerable<VibrationReportSummary> RegularReports
        {
            get { return _regularReports; }
            set { SetProperty(ref _regularReports, value); }
        }

        public IEnumerable<VibrationReportSummary> CustomReports
        {
            get { return _customReports; }
            set { SetProperty(ref _customReports, value); }
        }

        public bool IsReportNotGenerated
        {
            get { return _isReportNotGenerated; }
            set { SetProperty(ref _isReportNotGenerated, value); }
        }

        #region Commands

        public DelegateCommand<ObjectId> ShowReportDetailsCommand { get; private set; }

        public DelegateCommand<ObjectId> DeleteVibrationReportEventCommand { get; private set; }

        public DelegateCommand GeneraRegularReportCommand { get; private set; }

        public DelegateCommand CustomReportCommand { get; private set; }
        public DelegateCommand<ObjectId> ReportCsvCommand { get; private set; }
        #endregion

        #endregion


        #region Public methods
        /// <summary>
        ///     Load available reports. This method is invoked once the view is loaded
        /// </summary>
        public override void Initialize()
        {
            RegularReports = _reportManager.GetReportSummary(_consolidatedReport.GetRegularByTruckAndPackageId(TruckId, PackageId));
            CustomReports = _reportManager.GetReportSummary(_consolidatedReport.GetCustomByTruckAndPackageId(TruckId, PackageId));
            VibrationData = _vibrationRepository.GetByTruckAndPackageId(TruckId, PackageId);
            IsReportNotGenerated = !RegularReports.Any();
        }

        public IEnumerable<Vibration> VibrationData { get; set; }

        #endregion

        #region Private methods

        /// <summary>
        /// Handle the show report details command
        /// </summary>
        /// <param name="reportId">Id of the report</param>
        private void ShowReportDetails(ObjectId reportId)
        {
            var vibrationReport = _consolidatedReport.GetById(reportId);
            
            //Get a formatted consolidated report basically peak values of the signal and the corresponding signal
            var consolidatedReport = _reportManager.GetConsolidatedReport(vibrationReport);
            consolidatedReport.GRMS = vibrationReport.GRMS;
            var report50Percenttest = GenerateCustomConsolidatedReport2(50, reportId);
            //-------------RFC-------------------//
            var report50Percent = GenerateCustomConsolidatedReport2(50, reportId);
            report50Percent.Id = reportId;
            var report95Percent = GenerateCustomConsolidatedReport2(95, reportId);
            var report90Percent = GenerateCustomConsolidatedReport2(90, reportId);
            //---------------RFC------------------//
            //---------------RFC-----------orignal code-------//
            //var report50Percent = GenerateCustomConsolidatedReport(50);
            //report50Percent.Id = reportId;
            //var report95Percent = GenerateCustomConsolidatedReport(95);
            //var report90Percent = GenerateCustomConsolidatedReport(90);
            //---------------RFC-----------orignal code-------//
            // 100 - 95 -90 - 80 -50 
            var consolidated50Percent = _reportManager.GetConsolidatedReport(report50Percent);
            var consolidated95Percent = _reportManager.GetConsolidatedReport(report95Percent);
            var consolidated90Percent = _reportManager.GetConsolidatedReport(report90Percent); ;

            List<VibrationConsolidatedReport> consolidatedReports =
                new List<VibrationConsolidatedReport>
                {
                    consolidated50Percent,
                    consolidated90Percent,
                    consolidated95Percent
                };

            var navigationParams = new NavigationParameters
            {
                {PakTrackConstant.VibrationConsolidatedReportDetails, consolidatedReport},
                {PakTrackConstant.VibrationConsolidatedReportNDetails, consolidatedReports }
            };
            _regionManager.RequestNavigate(ApplicationRegion.MainRegion, PakTrackConstant.VibrationConsolidatedReportDetails, navigationParams);
        }

     
        #endregion

        private void OnDeleteReport(ObjectId id)
        {
            _consolidatedReport.DeleteEvent(id);
            Initialize();
        }



        private void NavigateCustomReportPage()
        {
            _eventAggregator.GetEvent<RemoteNavigationEvent>().Publish(PakTrackConstant.MenuOptionVibrationCustomConsolidatedReport);
        }

        //-----------RFC----------50-90-95--------------//
        private VibrationReport GenerateCustomConsolidatedReport2(int dataPercentage, ObjectId reportId)
        {
            var vibrationReport = _consolidatedReport.GetById(reportId);
            var eventNumber = vibrationReport.NumberOfEvents;
            //Get a formatted consolidated report basically peak values of the signal and the corresponding signal
            var consolidatedReport = _reportManager.GetConsolidatedReport(vibrationReport);

            var totalEvents = VibrationData.Count();
            var eventsBasedOnPercentage = eventNumber * (float)dataPercentage / 100;

            var vibrationDataForVector = VibrationData.OrderBy(v => v.GRMS.Vector).Take((int)eventsBasedOnPercentage);
            var consolidatedReport2 = _consolidatedReport.GenerateReport(vibrationDataForVector, true).First();
            return consolidatedReport2;
        }
        //-----------RFC----------50-90-95--------------//

        private VibrationReport GenerateCustomConsolidatedReport(int dataPercentage)
        {
            var totalEvents = VibrationData.Count();
            var eventsBasedOnPercentage = totalEvents * (float)dataPercentage/100;

            var vibrationDataForVector = VibrationData.OrderBy(v => v.GRMS.Vector).Take((int)eventsBasedOnPercentage);
            var consolidatedReport = _consolidatedReport.GenerateReport(vibrationDataForVector, true).First();
            return consolidatedReport;
        }

        private void ReportToCsv(ObjectId reportId)
        {
            var vibrationReport = _consolidatedReport.GetById(reportId);

            //Get a formatted consolidated report basically peak values of the signal and the corresponding signal
            var data = _reportManager.GetConsolidatedReport(vibrationReport);
            var fileDialog = new SaveFileDialog
            {
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                Filter = "CSV files (*.csv)|*.csv|All files (*.*)|*.*",
                FileName = "REPORT_DATA_" + TruckId + "_" + PackageId + "_" + data.Id.CreationTime.ToLocalTime().ToString("MMddyyyyhhmmsstt")
            };
            if (fileDialog.ShowDialog() == true)
            {
                using (var writer = File.CreateText(fileDialog.FileName))
                {
                    var csvWritter = new CsvWriter(writer);

                    csvWritter.WriteField("Axis");

                    csvWritter.WriteField("Frequency");
                    csvWritter.WriteField("Value");

                    csvWritter.NextRecord();

                    foreach (var reportValue in data.XAxis)
                    {
                        csvWritter.WriteField("X-Axis");
                        csvWritter.WriteField(string.Join(" ", reportValue.Frequency));
                        csvWritter.WriteField(string.Join(" ", reportValue.Value));
                        csvWritter.NextRecord();
                    }

                    foreach (var reportValue in data.YAxis)
                    {
                        csvWritter.WriteField("Y-Axis");
                        csvWritter.WriteField(string.Join(" ", reportValue.Frequency));
                        csvWritter.WriteField(string.Join(" ", reportValue.Value));
                        csvWritter.NextRecord();
                    }

                    foreach (var reportValue in data.ZAxis)
                    {
                        csvWritter.WriteField("Z-Axis");
                        csvWritter.WriteField(string.Join(" ", reportValue.Frequency));
                        csvWritter.WriteField(string.Join(" ", reportValue.Value));
                        csvWritter.NextRecord();
                    }
                }
            }
        }
    }
}