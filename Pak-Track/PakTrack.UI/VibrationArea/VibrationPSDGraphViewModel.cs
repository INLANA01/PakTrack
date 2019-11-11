using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Media;
using LiteDB;
using LiveCharts;
using LiveCharts.Configurations;
using LiveCharts.Defaults;
using LiveCharts.Wpf;
using PakTrack.Core.Base;
using PakTrack.DAL.Interfaces;
using PakTrack.Utilities;
using Prism.Commands;
using Prism.Regions;

namespace PakTrack.UI.VibrationArea
{
    public class VibrationPSDGraphViewModel : PakTrackBindableBase, INavigationAware
    {
        private readonly IRegionManager _regionManager;
        private readonly IVibrationRepository _vibrationRepository;
        private IRegionNavigationService _navigationService;

        private SeriesCollection _seriesCollection;

        public DelegateCommand GoBackCommand { get; private set; }

        private bool _canGoBack;

        public bool CanGoBack
        {
            get { return _canGoBack; }
            set { SetProperty(ref _canGoBack, value); }
        }

        public VibrationPSDGraphViewModel(IRegionManager regionManager, IVibrationRepository vibrationRepository)
        {
            _regionManager = regionManager;
            _vibrationRepository = vibrationRepository;
            Title = "Vibration PSD Graph";
            GoBackCommand = new DelegateCommand(GoBack)
                .ObservesCanExecute((o) => CanGoBack);
        }


        public SeriesCollection SeriesCollection
        {
            get { return _seriesCollection; }
            set { SetProperty(ref _seriesCollection, value); }
        }

        public Func<double, string> Formatter { get; set; }


        public override void OnNavigatedTo(NavigationContext navigationContext)
        {
            _navigationService = navigationContext.NavigationService;
            var vibrationId = navigationContext.Parameters[PakTrackConstant.EventId] as ObjectId;
            CanGoBack = navigationContext.NavigationService.Journal.CanGoBack;
            var graphType = (GraphType) navigationContext.Parameters[PakTrackConstant.GraphType];

            GetInformationForPSDGraph(vibrationId);
        }

        private void GetInformationForPSDGraph(ObjectId vibrationId)
        {
            var vibration = _vibrationRepository.GetById(vibrationId);
            if (vibration == null)
                return;

            const double baseFrecuency = 1600.0;

            var xValue = vibration.PSDX.AsEnumerable().ToArray();
            var yValue = vibration.PSDY.AsEnumerable().ToArray();
            var zValue = vibration.PSDZ.AsEnumerable().ToArray();
//            var vector = vibration.PSDVector.AsEnumerable().ToArray();


            var xAxis = new ChartValues<ObservablePoint>();
            var yAxis = new ChartValues<ObservablePoint>();
            var zAxis = new ChartValues<ObservablePoint>();
            //            var vectorAxis = new ChartValues<ObservablePoint>();
            var totalPoint = vibration.X.Count;
//            var totalPoint = (vibration.X.Count/2) + 1; // For PSD only 1024 points

            var tempXAxisData = new List<ObservablePoint>();
            var tempYAxisData = new List<ObservablePoint>();
            var tempZAxisData = new List<ObservablePoint>();
//            var tempVectorAxis = new List<ObservablePoint>();

            for (var i = 0; i < 3; i++)
            {
                var frecuency = (baseFrecuency / totalPoint) * i;

                tempXAxisData.Add(new ObservablePoint(frecuency, 0));
                tempYAxisData.Add(new ObservablePoint(frecuency, 0));
                tempZAxisData.Add(new ObservablePoint(frecuency, 0));
//                tempVectorAxis.Add(new ObservablePoint(frecuency, 0));
            }


            for (var i = 3; i < 300; i++)
            {
                var frecuency = (baseFrecuency / totalPoint) * i;
                tempXAxisData.Add(new ObservablePoint(frecuency, xValue[i]));
                tempYAxisData.Add(new ObservablePoint(frecuency, yValue[i]));
                tempZAxisData.Add(new ObservablePoint(frecuency, zValue[i]));
//                tempVectorAxis.Add(new ObservablePoint(frecuency, vector[i]));
            }

            xAxis.AddRange(tempXAxisData); 
            yAxis.AddRange(tempYAxisData);
            zAxis.AddRange(tempZAxisData);
//            vectorAxis.AddRange(tempVectorAxis);


         //   var vectorColor = new SolidColorBrush(((Color)ColorConverter.ConvertFromString("#2ca02c")));

            SeriesCollection = new SeriesCollection(Mappers.Xy<ObservablePoint>()
                .X(point => point.X)
                .Y(point => point.Y))
            {
                new LineSeries
                {
                    Values = xAxis,
                    PointGeometrySize = 0,
                    StrokeThickness = 1,
                    Fill = Brushes.Transparent,
                    Title = "PSD X",
                    LineSmoothness = 0.5,
                    Stroke = Brushes.DarkOrange
                },
                new LineSeries
                {
                    Values = yAxis,
                    PointGeometrySize = 0,
                    Title = "PSD Y",
                    Fill = Brushes.Transparent,
                    LineSmoothness = 0.5,
                    Stroke = Brushes.DodgerBlue
                },
                new LineSeries
                {
                    Values = zAxis,
                    PointGeometrySize = 0,
                    Title = "PSD Z",
                    Fill = Brushes.Transparent,
                    LineSmoothness = 0.5,
                    Stroke = Brushes.Green
                }
            };

            Formatter = value => Math.Pow(10, value).ToString("N");
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