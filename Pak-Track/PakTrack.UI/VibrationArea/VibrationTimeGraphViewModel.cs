using System.Collections.Generic;
using System.Linq;
using System.Windows.Media;
using LiteDB;
using LiveCharts;
using LiveCharts.Defaults;
using LiveCharts.Wpf;
using PakTrack.Core.Base;
using PakTrack.DAL.Interfaces;
using PakTrack.Utilities;
using Prism.Commands;
using Prism.Regions;

namespace PakTrack.UI.VibrationArea
{
    public class VibrationTimeGraphViewModel : PakTrackBindableBase, INavigationAware
    {
        private readonly IRegionManager _regionManager;
        private IRegionNavigationService _navigationService;

        private readonly IVibrationRepository _vibrationRepository;

        private SeriesCollection _seriesCollection;

        private ChartValues<ObservablePoint> _xSeriesValue;

        private ChartValues<ObservablePoint> _ySeriesValue;

        private ChartValues<ObservablePoint> _zSeriesValue;

        private bool _isBusy;

        public DelegateCommand GoBackCommand { get; private set; }

        private bool _canGoBack;

        public bool CanGoBack
        {
            get { return _canGoBack; }
            set { SetProperty(ref _canGoBack, value); }
        }
        public VibrationTimeGraphViewModel(IRegionManager regionManager, IVibrationRepository vibrationRepository)
        {
            _regionManager = regionManager;
            _vibrationRepository = vibrationRepository;
            Title = "Vibration Time-Domain Graph";
            GoBackCommand = new DelegateCommand(GoBack)
                .ObservesCanExecute((o) => CanGoBack);
        }

        public SeriesCollection SeriesCollection
        {
            get { return _seriesCollection; }
            set { SetProperty(ref _seriesCollection, value); }
        }

      
        public bool IsBusy
        {
            get { return _isBusy; }
            set { SetProperty(ref _isBusy, value); }
        }

        public override void OnNavigatedTo(NavigationContext navigationContext)
        {
            IsBusy = true;
            _navigationService = navigationContext.NavigationService;
            CanGoBack = navigationContext.NavigationService.Journal.CanGoBack;
            var vibrationId = navigationContext.Parameters[PakTrackConstant.EventId] as ObjectId;
            var graphType = (GraphType) navigationContext.Parameters[PakTrackConstant.GraphType];

            GetInformationForTimeGraph(vibrationId);
            IsBusy = false;
        }

        private void GetInformationForTimeGraph(ObjectId vibrationId)
        {
            var vibration = _vibrationRepository.GetById(vibrationId);
            if (vibration == null)
                return;

            const double sampleTime = 0.000625;
            var timestamp = 1.0;

            var xValue = vibration.X.AsEnumerable().ToArray();
            var yValue = vibration.Y.AsEnumerable().ToArray();
            var zValue = vibration.Z.AsEnumerable().ToArray();

            var xAxis = new ChartValues<ObservablePoint>();
            var yAxis = new ChartValues<ObservablePoint>();
            var zAxis = new ChartValues<ObservablePoint>();

            var totalPoint = vibration.X.Count;

            var tempXAxisData = new List<ObservablePoint>();
            var tempYAxisData = new List<ObservablePoint>();
            var tempZAxisData = new List<ObservablePoint>();
            //Exclude 0
            for (var i = 1; i < totalPoint; i++)
            {

                    timestamp += sampleTime * 1000;

                tempXAxisData.Add(new ObservablePoint(timestamp, xValue[i]));
                tempYAxisData.Add(new ObservablePoint(timestamp, yValue[i]));
                tempZAxisData.Add(new ObservablePoint(timestamp, zValue[i]));
            }

            xAxis.AddRange(tempXAxisData);
            yAxis.AddRange(tempYAxisData);
            zAxis.AddRange(tempZAxisData);


            SeriesCollection = new SeriesCollection
            {
                new LineSeries
                {
                    Values = xAxis,
                    PointGeometrySize = 0,
                    StrokeThickness = 1,
                    Fill = Brushes.Transparent,
                    Title = "X",
                    LineSmoothness = 0.5,
                    Stroke = Brushes.DarkOrange
                },
                new LineSeries
                {
                    Values = yAxis,
                    PointGeometrySize = 0,
                    Title = "Y",
                    Fill = Brushes.Transparent,
                    LineSmoothness = 0.5,
                    Stroke = Brushes.Green
                },
                new LineSeries
                {
                    Values = zAxis,
                    PointGeometrySize = 0,
                    Title = "Z",
                    Fill = Brushes.Transparent,
                    LineSmoothness = 0.5,
                    Stroke = Brushes.DodgerBlue
                }
            };
            //Stroke = Brushes.ForestGreen   Stroke = Brushes.DodgerBlue   Stroke = Brushes.Tomato
        
        }

        public override void OnNavigatedFrom(NavigationContext navigationContext)
        {
            SeriesCollection = null;
        }

        private void GoBack()
        {
            if (_navigationService.Journal.CanGoBack)
            {
                _navigationService.Journal.GoBack();
            }
        }
    }
}