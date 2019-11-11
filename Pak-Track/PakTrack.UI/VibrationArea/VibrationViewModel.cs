using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CsvHelper;
using LiteDB;
using Microsoft.Win32;
using PakTrack.Core;
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
    public class VibrationViewModel : PakTrackBindableBase, INavigationAware
    {
        private readonly IRegionManager _regionManager;
        private readonly IEventAggregator _eventAggregator;
        private readonly IVibrationConsolidatedReport _consolidatedReport;
        private readonly IVibrationRepository _vibrationRepository;
        private ICollection<Vibration> CustomVibrationData;
        private bool _customDataAdded;
        private bool _isBusy;
        private string _reportGenerator;
        private string _filterBtnContent = "Apply Filter";
        private bool _isFilterApplied = false;

        private FilterInfo _filter;
        /**
        * Pagination Variables
        */
        int _pageIndex = 1;
        private int _numberOfRecPerPage = 15;
        private String _pagingLabel;
        private int _totalNumberOfVibrationEvents;

        private IEnumerable<Vibration> _vibrationData;
        public DelegateCommand FirstCommand { get; private set; }
        public DelegateCommand LastCommand { get; private set; }
        public DelegateCommand NextCommand { get; private set; }
        public DelegateCommand PreviousCommand { get; private set; }

        public DelegateCommand GenerateReportCommand { get; private set; }
        public DelegateCommand DeleteAllEventsCommand { get; private set; }

        public DelegateCommand FilterCommand { get; private set; }
        public bool IsCustomDataAdded
        {
            get { return _customDataAdded; }
            set { SetProperty(ref _customDataAdded, value); }
        }

        public bool IsNotFilterApplied
        {
            get { return !_isFilterApplied; }
            set { SetProperty(ref _isFilterApplied, value); }
        }

        public string PagingLabel
        {
            get { return _pagingLabel; }
            set { SetProperty(ref _pagingLabel, value); }
        }

        public string ReportGeneratorName
        {
            get { return _reportGenerator; }
            set { SetProperty(ref _reportGenerator, value); }
        }

        public string FilterBtnTitle
        {
            get { return _filterBtnContent; }
            set { SetProperty(ref _filterBtnContent, value); }
        }

        public VibrationViewModel(IVibrationRepository vibrationRepository, IRegionManager regionManager,
            IEventAggregator eventAggregator, IVibrationConsolidatedReport consolidatedReport)
        {
            IsCustomDataAdded = false;
            ReportGeneratorName = "Generate Report";
            _vibrationRepository = vibrationRepository;
            _regionManager = regionManager;
            _eventAggregator = eventAggregator;
            _consolidatedReport = consolidatedReport;
            DisplayTimeChartCommand = new DelegateCommand<ObjectId>(DisplayTimeGraph);
            DisplayPsdChartCommand = new DelegateCommand<ObjectId>(DisplayPSDGraph);
            DeleteVibrationEventCommand = new DelegateCommand<ObjectId>(DeleteVibrationEvent);
            TimeEventToCsvCommand = new DelegateCommand<ObjectId>(TimeEventToCsv);
            PsdEventToCsvCommand = new DelegateCommand<ObjectId>(PsdEventToCsv);
            VibrationToCsvCommand = new DelegateCommand(VibrationDataToCsv);
            AddToCustomReportCommand = new DelegateCommand<Vibration>(AddEventToCustomReportList).ObservesCanExecute(o => IsNotFilterApplied);
            CustomVibrationData = new List<Vibration>();
            DeleteAllEventsCommand = new DelegateCommand(DeleteEvents);
            eventAggregator.GetEvent<NavigationEvent>().Subscribe(OnTruckAndPackageChanged);
            GenerateReportCommand = new DelegateCommand(GenerateReport);
            FilterCommand = new DelegateCommand(OnFilterCommand);
            eventAggregator.GetEvent<CustomReportFilterEvent>().Subscribe(FilterApplied);
            FirstCommand = new DelegateCommand(First);
            LastCommand = new DelegateCommand(Last);
            NextCommand = new DelegateCommand(Next);
            PreviousCommand = new DelegateCommand(Previous);
        }

        public IEnumerable<Vibration> VibrationData
        {
            get { return _vibrationData; }
            set { SetProperty(ref _vibrationData, value); }
        }

        public bool IsBusy
        {
            get { return _isBusy; }
            set { SetProperty(ref _isBusy, value); }
        }

        public DelegateCommand<ObjectId> DisplayTimeChartCommand { get; private set; }
        public DelegateCommand<ObjectId> DisplayPsdChartCommand { get; private set; }
        public DelegateCommand<ObjectId> DeleteVibrationEventCommand { get; private set; }
        public DelegateCommand<ObjectId> TimeEventToCsvCommand { get; private set; }
        public DelegateCommand<ObjectId> PsdEventToCsvCommand { get; private set; }
        public DelegateCommand VibrationToCsvCommand { get; private set; }

        public DelegateCommand<Vibration> AddToCustomReportCommand { get; private set; }

        protected override void OnTruckAndPackageChanged(NavigationInformation navigationInformation)
        {
            var regionInfo = _regionManager.Regions[ApplicationRegion.MainRegion];
            if (IsActiveView(regionInfo, typeof(VibrationView).Name))
            {
                TruckId = navigationInformation.TruckId;
                PackageId = navigationInformation.PackageId;
            }
        }

        private void GenerateReport()
        {
            _eventAggregator.GetEvent<StatusEvent>().Publish(new StatusInformation
            {
                Action = "Report Generator",
                Status = "Report Generation Started",
                IsCompleted = true
            });
            if (IsCustomDataAdded)
            {
                Task.Run(() => GenerateReportTask(CustomVibrationData, true));
            }
            else
            {
                var data = GetVibration();
                var vibrations = data as IList<Vibration> ?? data.ToList();
                if(vibrations.Any())
                    Task.Run(() => GenerateReportTask(vibrations, _isFilterApplied));
            }
           
        }

        private void GenerateReportTask(ICollection<Vibration> vibrations, bool isCustom = false)
        {
            if (vibrations.Any())
            {
                _consolidatedReport.PushToDb(_consolidatedReport.GenerateReport(vibrations, isCustom));

                _eventAggregator.GetEvent<StatusEvent>().Publish(new StatusInformation
                {
                    Action = "Report Generator",
                    Status = "Report Generation Complete",
                    IsCompleted = true
                });
                if (isCustom)
                {
                    vibrations.Clear();
                    IsCustomDataAdded = false;
                    ReportGeneratorName = "Generate Report";
                }

                _regionManager.RequestNavigate(ApplicationRegion.MainRegion, PakTrackConstant.MenuOptionVibrationConsolidatedReport);
            }
            else
            {
                _eventAggregator.GetEvent<StatusEvent>().Publish(new StatusInformation
                {
                    Action = "Report Generator",
                    Status = "No Vibration Data to Generate Report",
                    IsCompleted = true
                });
            }
        }

        private void ResetFilter()
        {
            _isFilterApplied = false;
            FilterBtnTitle = "Apply Filter";
            Initialize();
        }

        private void OnFilterCommand()
        {
            if (_isFilterApplied)
                ResetFilter();
            else
                _eventAggregator.GetEvent<RemoteNavigationEvent>().Publish(PakTrackConstant.MenuOptionVibrationCustomConsolidatedReport);
        }

        private IEnumerable<Vibration> GetVibration()
        {
            if (_isFilterApplied)
            {
                if(_filter.IsTimeStamp)
                    return _vibrationRepository.GetByTruckAndPackageId(TruckId, PackageId)
                        .Where(vib => vib.Timestamp > _filter.Start.Ticks && vib.Timestamp < _filter.End.Ticks);
                return _vibrationRepository.GetByTruckAndPackageId(TruckId, PackageId).Where(x =>
                    x.MaximumX.Value >= _filter.X.MinValue && x.MaximumX.Value <= _filter.X.MaxValue ||
                    x.MaximumY.Value >= _filter.Y.MinValue && x.MaximumY.Value <= _filter.Y.MaxValue ||
                    x.MaximumZ.Value >= _filter.Z.MinValue && x.MaximumZ.Value <= _filter.Z.MaxValue);
            }
            return _vibrationRepository.GetByTruckAndPackageId(TruckId, PackageId);
        }

        private async void DeleteEvents()
        {
            var vibDelete = await _vibrationRepository.DeleteEvents(TruckId, PackageId);
            var reportDelete = await _consolidatedReport.DeleteEvents(TruckId, PackageId);
            if (vibDelete || reportDelete)
            {
                _eventAggregator.GetEvent<RemoteNavigationEvent>().Publish(PakTrackConstant.MenuOptionDashboard);
            }

        }
        private void DisplayPSDGraph(ObjectId eventId)
        {
            var navigationParams = new NavigationParameters
            {
                {PakTrackConstant.EventId, eventId},
                {PakTrackConstant.GraphType, GraphType.VibrationPSD}
            };
            _regionManager.RequestNavigate(ApplicationRegion.MainRegion, PakTrackConstant.VibrationPsdGraph,
                navigationParams);
        }

        private void DisplayTimeGraph(ObjectId eventId)
        {
            var navigationParams = new NavigationParameters
            {
                {PakTrackConstant.EventId, eventId},
                {PakTrackConstant.GraphType, GraphType.VibrationTime}
            };
            _regionManager.RequestNavigate(ApplicationRegion.MainRegion, PakTrackConstant.VibrationTimeGraph,
                navigationParams);
        }

        private void DeleteVibrationEvent(ObjectId eventId)
        {
            _vibrationRepository.DeleteEvent(eventId);
            Initialize();
        }

        public override void OnNavigatedFrom(NavigationContext navigationContext)
        {
            VibrationData = null;
        }

        private void AddEventToCustomReportList(Vibration vib)
        {
            if (CustomVibrationData.Contains(vib))
            {
                CustomVibrationData.Remove(vib);
                IsCustomDataAdded = CustomVibrationData.Any();
                ReportGeneratorName = IsCustomDataAdded ? "Generate Custom Report (" + CustomVibrationData.Count + ") Events" : "Generate Report";
            }
            else
            {
                IsCustomDataAdded = true;
                CustomVibrationData.Add(vib);
                ReportGeneratorName = "Generate Custom Report (" + CustomVibrationData.Count + ") Events";
            }
        }

        private void TimeEventToCsv(ObjectId eventId)
        {
            var fileDialog = new SaveFileDialog
            {
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                Filter = "CSV files (*.csv)|*.csv|All files (*.*)|*.*",
                FileName = "VIBRATION_DATA_" + TruckId + "_" + PackageId + "_Time_Event"
            };
            if (fileDialog.ShowDialog() == true)
            {
                using (var writer = File.CreateText(fileDialog.FileName))
                {
                    var csvWritter = new CsvWriter(writer);

                    var vibration = _vibrationRepository.GetById(eventId);

                    if (vibration == null)
                    {
                        csvWritter.WriteField("No Data");
                        return;
                    }

                    csvWritter.WriteField("Time");
                    csvWritter.WriteField("X");
                    csvWritter.WriteField("Y");
                    csvWritter.WriteField("Z");
                    csvWritter.NextRecord();

               
                    const double sampleTime = 0.000625;
                    var timestamp = 1.0;

                    var xValue = vibration.X.AsEnumerable().ToArray();
                    var yValue = vibration.Y.AsEnumerable().ToArray();
                    var zValue = vibration.Z.AsEnumerable().ToArray();

                    var totalPoint = vibration.X.Count;

                    for (var i = 1; i < totalPoint; i++)
                    {
                        timestamp += sampleTime * 1000;
                        csvWritter.WriteField(timestamp);
                        csvWritter.WriteField(xValue[i]);
                        csvWritter.WriteField(yValue[i]);
                        csvWritter.WriteField(zValue[i]);
                        csvWritter.NextRecord();

                    }
                }
            }
        }

        private void FilterApplied(FilterInfo info)
        {
            _isFilterApplied = true;
            FilterBtnTitle = "Remove Filter";
            _filter = info;
            Initialize();
        }

        private void PsdEventToCsv(ObjectId eventId)
        {
            var fileDialog = new SaveFileDialog
            {
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                Filter = "CSV files (*.csv)|*.csv|All files (*.*)|*.*",
                FileName = "VIBRATION_DATA_" + TruckId + "_" + PackageId + "_PSD_Event"
            };
            if (fileDialog.ShowDialog() == true)
            {
                using (var writer = File.CreateText(fileDialog.FileName))
                {
                    var csvWritter = new CsvWriter(writer);

                    var vibration = _vibrationRepository.GetById(eventId);

                    if (vibration == null)
                    {
                        csvWritter.WriteField("No Data");
                        return;
                    }

                    csvWritter.WriteField("Frequency");
                    csvWritter.WriteField("X");
                    csvWritter.WriteField("Y");
                    csvWritter.WriteField("Z");
                    csvWritter.NextRecord();

                    const double baseFrecuency = 1600.0;

                    var xValue = vibration.PSDX.AsEnumerable().ToArray();
                    var yValue = vibration.PSDY.AsEnumerable().ToArray();
                    var zValue = vibration.PSDZ.AsEnumerable().ToArray();

                    var totalPoint = vibration.X.Count;

                    for (var i = 1; i < totalPoint; i++)
                    {
                        var frecuency = (baseFrecuency / totalPoint) * i;
                        csvWritter.WriteField(frecuency);
                        csvWritter.WriteField(xValue[i]);
                        csvWritter.WriteField(yValue[i]);
                        csvWritter.WriteField(zValue[i]);
                        csvWritter.NextRecord();

                    }
                }
            }
        }

        private void VibrationDataToCsv()
        {
            var data = _vibrationRepository.GetByTruckAndPackageId(TruckId, PackageId);
            var fileDialog = new SaveFileDialog
            {
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                Filter = "CSV files (*.csv)|*.csv|All files (*.*)|*.*",
                FileName = "VIB_DATA_" + TruckId + "_" + PackageId
            };
            if (fileDialog.ShowDialog() == true)
            {
                using (var writer = File.CreateText(fileDialog.FileName))
                {
                    var csvWritter = new CsvWriter(writer);

                    csvWritter.WriteField("Timestamp");

                    csvWritter.WriteField("X");
                    csvWritter.WriteField("Y");
                    csvWritter.WriteField("Z");

                    csvWritter.WriteField("MAX X");
                    csvWritter.WriteField("MAX Y");
                    csvWritter.WriteField("MAX Z");


                    csvWritter.WriteField("RMS X");
                    csvWritter.WriteField("RMS Y");
                    csvWritter.WriteField("RMS Z");

                    csvWritter.NextRecord();

                    foreach (var vibration in data)
                    {
                        csvWritter.WriteField(DataConverters.TimeStampConverter(vibration.Timestamp));

                        csvWritter.WriteField(string.Join(" ", vibration.X));
                        csvWritter.WriteField(string.Join(" ", vibration.Y));
                        csvWritter.WriteField(string.Join(" ", vibration.Z));

                        csvWritter.WriteField(vibration.MaximumX.Value);
                        csvWritter.WriteField(vibration.MaximumY.Value);
                        csvWritter.WriteField(vibration.MaximumZ.Value);

                        csvWritter.WriteField(vibration.RMS.X);
                        csvWritter.WriteField(vibration.RMS.Y);
                        csvWritter.WriteField(vibration.RMS.Z);

                        csvWritter.NextRecord();
                    }
                }
            }
        }

        public override void Initialize()
        {
            IsBusy = true;
            _totalNumberOfVibrationEvents = GetVibration().Count();
            _pageIndex += 1;
            Previous();
            IsBusy = false;
        }

        private void First()
        {
            Navigate(PagingMode.First);
        }

        private void Next()
        {
            Navigate(PagingMode.Next);
        }

        private void Last()
        {
            Navigate(PagingMode.Last);
        }

        private void Previous()
        {
            Navigate(PagingMode.Previous);
        }

        private void Navigate(PagingMode mode)
        {
            int count;
            switch (mode)
            {
                case PagingMode.Next:
                    {
                        if (_totalNumberOfVibrationEvents > _pageIndex * _numberOfRecPerPage)
                        {
                            var data = GetVibration()
                                .Skip(_pageIndex * _numberOfRecPerPage).Take(_numberOfRecPerPage);
                            var vibrationData = data as IList<Vibration> ?? data.ToList();
                            if (vibrationData.Any())
                            {
                                VibrationData = vibrationData;
                                count = _pageIndex * _numberOfRecPerPage + VibrationData.Count();
                                _pageIndex++;
                                PagingLabel = count + " Of " + _totalNumberOfVibrationEvents;
                            }
                        }
                        break;
                    }
                case PagingMode.Previous:
                    {
                        if (_pageIndex > 1)
                        {
                            _pageIndex -= 1;
                            if (_pageIndex == 1)
                            {
                                VibrationData = GetVibration()
                                    .Take(_numberOfRecPerPage);
                                count = VibrationData.Count();
                            }
                            else
                            {
                                VibrationData = GetVibration()
                                    .Skip((_pageIndex -1) * _numberOfRecPerPage).Take(_numberOfRecPerPage);
                                var vibCount = VibrationData.Count();
                                count = vibCount == _numberOfRecPerPage ? _pageIndex * _numberOfRecPerPage : (_pageIndex -1) * _numberOfRecPerPage + vibCount;

                            }
                            PagingLabel = count + " Of " + _totalNumberOfVibrationEvents;
                        }
                        break;
                    }
                case PagingMode.First:
                    {
                        _pageIndex = 2;
                        Navigate(PagingMode.Previous);
                        break;
                    }
                case PagingMode.Last:
                    {
                        _pageIndex = _totalNumberOfVibrationEvents / _numberOfRecPerPage;
                        Navigate(PagingMode.Next);
                        break;
                    }
            }

        }

    }
}