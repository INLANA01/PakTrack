using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using CsvHelper;
using LiteDB;
using LiveCharts;
using LiveCharts.Wpf;
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

namespace PakTrack.UI.ShockArea
{
    public class ShockViewModel : PakTrackBindableBase, INavigationAware
    {
        /**
         * Pagination Variables
         */
        int _pageIndex = 1;
        private int _numberOfRecPerPage = 15;

        private String _pagingLabel;
        private int _totalNumberOfShockEvents;

        private readonly IRegionManager _regionManager;
        private readonly IShockRepository _shockRepository;

        public SeriesCollection SeriesCollection { get; set; }
        public string[] Labels { get; set; }
        public Func<double, string> Formatter { get; set; }

        private IEnumerable<Shock> _shockData;

        public IEnumerable<Shock> ShockData
        {
            get { return _shockData; }
            set { SetProperty(ref _shockData, value); }
        }

        private bool _isBusy;

        public bool IsBusy
        {
            get { return _isBusy; }
            set { SetProperty(ref _isBusy, value); }
        }

        public DelegateCommand<ObjectId> DisplayTimeGraphCommand { get; private set; }
        public DelegateCommand<ObjectId> DisplaySRSGraphCommand { get; private set; }
        public DelegateCommand<ObjectId> DeleteShockEventCommand { get; private set; }
        public DelegateCommand<ObjectId> TimeCsvEventCommand { get; private set; }
        public DelegateCommand<ObjectId> SrsCsvEventCommand { get; private set; }
        public DelegateCommand ShockDataToCsvCommand { get; private set; }

        public DelegateCommand FirstCommand { get; private set; }
        public DelegateCommand LastCommand { get; private set; }
        public DelegateCommand NextCommand { get; private set; }
        public DelegateCommand PreviousCommand { get; private set; }

        public string PagingLabel
        {
            get { return _pagingLabel; }
            set { SetProperty(ref _pagingLabel, value); }
        }

        public ShockViewModel(IRegionManager regionManager, IEventAggregator eventAggregator, IShockRepository shockRepository)
        {
            _regionManager = regionManager;
            _shockRepository = shockRepository;
            eventAggregator.GetEvent<NavigationEvent>().Subscribe(OnTruckAndPackageChangedShock);
            DisplayTimeGraphCommand = new DelegateCommand<ObjectId>(DisplayTimeGraph);
            DisplaySRSGraphCommand = new DelegateCommand<ObjectId>(DisplaySRSGraph);
            DeleteShockEventCommand = new DelegateCommand<ObjectId>(DeleteShockEvent);
            TimeCsvEventCommand = new DelegateCommand<ObjectId>(TimeEventToCsv);
            SrsCsvEventCommand = new DelegateCommand<ObjectId>(SrsEventToCSV);
            ShockDataToCsvCommand = new DelegateCommand(ShockDataToCsv);
            FirstCommand = new DelegateCommand(First);
            LastCommand = new DelegateCommand(Last);
            NextCommand = new DelegateCommand(Next);
            PreviousCommand = new DelegateCommand(Previous);
            Title = "Shock Area View";
        }

        private void DisplaySRSGraph(ObjectId shockId)
        {
            var navigationParams = new NavigationParameters
            {
                {PakTrackConstant.EventId, shockId},
                {PakTrackConstant.GraphType, GraphType.ShockPSD}
            };
            _regionManager.RequestNavigate(ApplicationRegion.MainRegion, PakTrackConstant.ShockSRSGraph,
                navigationParams);
        }

        private void DisplayTimeGraph(ObjectId shockId)
        {
            var navigationParams = new NavigationParameters
            {
                {PakTrackConstant.EventId, shockId},
                {PakTrackConstant.GraphType, GraphType.ShockTime}
            };
            _regionManager.RequestNavigate(ApplicationRegion.MainRegion, PakTrackConstant.ShockTimeGraph,
                navigationParams);
        }

        private void OnTruckAndPackageChangedShock(NavigationInformation navigationInformation)
        {
            var regionInfo = _regionManager.Regions[ApplicationRegion.MainRegion];
            if (IsActiveView(regionInfo, typeof(ShockView).Name))
            {
                TruckId = navigationInformation.TruckId;
                PackageId = navigationInformation.PackageId;
            }
        }

        private void DeleteShockEvent(ObjectId eventId)
        {
            _shockRepository.DeleteEvent(eventId);
            Initialize();
        }


        private void SrsEventToCSV(ObjectId eventId)
        {
            var fileDialog = new SaveFileDialog
            {
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                Filter = "CSV files (*.csv)|*.csv|All files (*.*)|*.*",
                FileName = TruckId + "_" + PackageId + "_SRS_Event"
            };
            if (fileDialog.ShowDialog() == true)
            {
                using (var writer = File.CreateText(fileDialog.FileName))
                {
                    var csvWritter = new CsvWriter(writer);
                    var shock = _shockRepository.GetById(eventId);
                    if (shock == null)
                    {
                        csvWritter.WriteField("No data ");
                        return;
                    }

                    csvWritter.WriteField("Frequency");
                    csvWritter.WriteField("X Positive");
                    csvWritter.WriteField("X Negative");

                    csvWritter.WriteField("Y Positive");
                    csvWritter.WriteField("Y Negative");

                    csvWritter.WriteField("Z Positive");
                    csvWritter.WriteField("Z Negative");
                    csvWritter.NextRecord();

                    //SRS contains array of Positive, Negative and Frecuencie values
                    var frecuencies = shock.SRS.X.Frequency.ToArray();

                    //Posiives values
                    var xPositiveValues = shock.SRS.X.Positive.ToArray();
                    var yPositiveValues = shock.SRS.Y.Positive.ToArray();
                    var zPositiveValues = shock.SRS.Z.Positive.ToArray();

                    //Negatives
                    var xNegativeValues = shock.SRS.X.Negative.ToArray();
                    var yNegativeValues = shock.SRS.Y.Negative.ToArray();
                    var zNegativeValues = shock.SRS.Z.Negative.ToArray();

                    //Events count
                    var eventCount = frecuencies.Length;

                    for (var i = 1; i < eventCount; i++)
                    {
                        csvWritter.WriteField(frecuencies[i]);
                        csvWritter.WriteField(xPositiveValues[i]);
                        csvWritter.WriteField(xNegativeValues[i]);

                        csvWritter.WriteField(yPositiveValues[i]);
                        csvWritter.WriteField(yNegativeValues[i]);

                        csvWritter.WriteField(zPositiveValues[i]);
                        csvWritter.WriteField(zNegativeValues[i]);

                        csvWritter.NextRecord();

                    }
                }
            }
        }

        private void DisplayGrpah()
        {
            SeriesCollection = new SeriesCollection
            {
                new StackedColumnSeries
                {
                    Values = new ChartValues<double>(ShockData.Select(x => x.MaximumX.Value)),
                    StackMode = StackMode.Values, 
                    DataLabels = true
                },
                new StackedColumnSeries
                {
                    Values = new ChartValues<double> (ShockData.Select(x => x.MaximumY.Value)),
                    StackMode = StackMode.Values,
                    DataLabels = true
                },
                new StackedColumnSeries
                {
                    Values = new ChartValues<double> ( ShockData.Select(x => x.MaximumZ.Value)),
                    StackMode = StackMode.Values,
                    DataLabels = true
                }
            };


            Labels = ShockData.Select(x=> DataConverters.TimeStampConverter(x.Timestamp)).ToArray();
          

        }

        private void TimeEventToCsv(ObjectId eventId)
        {
            var fileDialog = new SaveFileDialog
            {
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                Filter = "CSV files (*.csv)|*.csv|All files (*.*)|*.*",
                FileName = TruckId+"_"+PackageId+"_Time_Event"
            };
            if (fileDialog.ShowDialog() == true)
            {
                using (var writer = File.CreateText(fileDialog.FileName))
                {
                    var csvWritter = new CsvWriter(writer);
                    var shockEvent = _shockRepository.GetById(eventId);
                    if (shockEvent == null)
                    {
                        csvWritter.WriteField("No data ");
                        return;
                    }

                    csvWritter.WriteField("Time");
                    csvWritter.WriteField("X");
                    csvWritter.WriteField("Y");
                    csvWritter.WriteField("Z");
                    csvWritter.NextRecord();
                    const double sampleTime = 0.000625;
                    var timestamp = 1.0;
                    var xValue = shockEvent.X.AsEnumerable().ToArray();
                    var yValue = shockEvent.Y.AsEnumerable().ToArray();
                    var zValue = shockEvent.Z.AsEnumerable().ToArray();

                    var totalPoint = shockEvent.X.Count;

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
        public override void Initialize()
        {
            IsBusy = true;
            _totalNumberOfShockEvents = _shockRepository.GetEventCount(TruckId, PackageId);
            _pageIndex += 1;
            Previous();
//            DisplayGrpah();
            IsBusy = false;
        }

        public override void OnNavigatedFrom(NavigationContext navigationContext)
        {
            ShockData = null;
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
                    if (_totalNumberOfShockEvents > _pageIndex * _numberOfRecPerPage)
                    {
                        var data = _shockRepository.GetByTruckAndPackageId(TruckId, PackageId)
                            .Skip(_pageIndex * _numberOfRecPerPage).Take(_numberOfRecPerPage);
                        var shockData = data as IList<Shock> ?? data.ToList();
                        if (shockData.Any())
                        {
                            ShockData = shockData ;
                            count = _pageIndex * _numberOfRecPerPage + ShockData.Count();
                            _pageIndex++;
                            PagingLabel = count + " Of " + _totalNumberOfShockEvents;
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
                            ShockData = _shockRepository.GetByTruckAndPackageId(TruckId, PackageId)
                                .Take(_numberOfRecPerPage);
                            count = ShockData.Count();
                        }
                        else
                        {
                            ShockData = _shockRepository.GetByTruckAndPackageId(TruckId, PackageId)
                                .Skip(_pageIndex * _numberOfRecPerPage).Take(_numberOfRecPerPage);
                            var shockData = ShockData as IList<Shock> ?? ShockData.ToList();
                            count = shockData.Count() == _numberOfRecPerPage ? _pageIndex * _numberOfRecPerPage  :(_pageIndex - 1) * _numberOfRecPerPage + shockData.Count;

                         }
                        PagingLabel = count + " Of " + _totalNumberOfShockEvents;
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
                    _pageIndex = _totalNumberOfShockEvents / _numberOfRecPerPage;
                    Navigate(PagingMode.Next);
                    break;
                }
            }

        }

        private void ShockDataToCsv()
        {
            var allShockData = _shockRepository.GetByTruckAndPackageId(TruckId, PackageId);
            var fileDialog = new SaveFileDialog
            {
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                Filter = "CSV files (*.csv)|*.csv|All files (*.*)|*.*",
                FileName = "SHOCK_DATA_"+TruckId + "_" + PackageId
            };
            if (fileDialog.ShowDialog() == true)
            {
                using (var writer = File.CreateText(fileDialog.FileName))
                {
                    var csvWritter = new CsvWriter(writer);

                    csvWritter.WriteField("Timestamp");
                    csvWritter.WriteField("IsInstantenious");
                    csvWritter.WriteField("Orientation");
                    csvWritter.WriteField("Drop Height");

                    csvWritter.WriteField("X");
                    csvWritter.WriteField("Y");
                    csvWritter.WriteField("Z");

                    csvWritter.WriteField("MAX X");
                    csvWritter.WriteField("MAX Y");
                    csvWritter.WriteField("MAX Z");

                    csvWritter.WriteField("G-RMS");

                    csvWritter.NextRecord();

                    foreach (var shock in allShockData)
                    {
                        csvWritter.WriteField(DataConverters.TimeStampConverter(shock.Timestamp));
                        csvWritter.WriteField(shock.IsInstantaneous);
                        csvWritter.WriteField(string.Join(" ", shock.Orientation));
                        csvWritter.WriteField((shock.IsInstantaneous ? 0 : shock.DropHeight));

                        csvWritter.WriteField(string.Join(" ", shock.X));
                        csvWritter.WriteField(string.Join(" ", shock.Y));
                        csvWritter.WriteField(string.Join(" ", shock.Z));

                        csvWritter.WriteField(shock.MaximumX.Value);
                        csvWritter.WriteField(shock.MaximumY.Value);
                        csvWritter.WriteField(shock.MaximumZ.Value);

                        csvWritter.WriteField(shock.GRMS);

                        csvWritter.NextRecord();
                    }

                }
            }
        }
    }
}
