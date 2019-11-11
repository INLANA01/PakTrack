using Prism.Commands;
using Prism.Mvvm;
using System;
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
using Prism.Regions;

namespace PakTrack.UI.ShockArea
{
    public class ShockSRSGraphViewModel : PakTrackBindableBase, INavigationAware
    {
        private readonly IShockRepository _shockRepository;

        private IRegionNavigationService _navigationService;

        private ObjectId _shockId;

        public ObjectId ShockId
        {
            get { return _shockId; }
            set { SetProperty(ref _shockId, value); }
        }

        private SeriesCollection _seriesCollection;

        public SeriesCollection SeriesCollection
        {
            get { return _seriesCollection; }
            set { SetProperty(ref _seriesCollection, value); }
        }

        private bool _canGoBack;

        public bool CanGoBack
        {
            get { return _canGoBack; }
            set { SetProperty(ref _canGoBack, value); }
        }

        private bool _isBusy;

        public bool IsBusy
        {
            get { return _isBusy; }
            set { SetProperty(ref _isBusy, value); }
        }

        public DelegateCommand GoBackCommand { get; private set; }


        public ShockSRSGraphViewModel(IShockRepository shockRepository)
        {
            _shockRepository = shockRepository;
            Title = "Shock SRS Graph";
            IsBusy = true;
            GoBackCommand = new DelegateCommand(GoBack)
                .ObservesCanExecute((o) => CanGoBack);
        }


        public override void OnNavigatedTo(NavigationContext navigationContext)
        {
            _navigationService = navigationContext.NavigationService;
            CanGoBack = navigationContext.NavigationService.Journal.CanGoBack;
            ShockId = navigationContext.Parameters[PakTrackConstant.EventId] as ObjectId;
            var graphType = (GraphType) navigationContext.Parameters[PakTrackConstant.GraphType];
        }

        public override void OnNavigatedFrom(NavigationContext navigationContext)
        {
            SeriesCollection = null;
        }

        /// <summary>
        /// 
        /// </summary>
        public override void Initialize()
        {
            var shock = _shockRepository.GetById(ShockId);

            if(shock.SRS==null|| shock.SRS.X==null)return;

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

            var xAxisPositive = new ChartValues<ObservablePoint>();
            var yAxisPositive = new ChartValues<ObservablePoint>();
            var zAxisPositive = new ChartValues<ObservablePoint>();

            var xAxisNegative = new ChartValues<ObservablePoint>();
            var yAxisNegative = new ChartValues<ObservablePoint>();
            var zAxisNegative = new ChartValues<ObservablePoint>();

            var tempXAxisPositiveData = new List<ObservablePoint>();
            var tempYAxisPositiveData = new List<ObservablePoint>();
            var tempZAxisPositiveData = new List<ObservablePoint>();

            var tempXAxisNegativeData = new List<ObservablePoint>();
            var tempYAxisNegativeData = new List<ObservablePoint>();
            var tempZAxisNegativeData = new List<ObservablePoint>();

            for (var i = 0; i < eventCount; i++)
            {
                //Positive values
                tempXAxisPositiveData.Add(new ObservablePoint(frecuencies[i], xPositiveValues[i]));
                tempYAxisPositiveData.Add(new ObservablePoint(frecuencies[i], yPositiveValues[i]));
                tempZAxisPositiveData.Add(new ObservablePoint(frecuencies[i], zPositiveValues[i]));

                //Negative values
                tempXAxisNegativeData.Add(new ObservablePoint(frecuencies[i], xNegativeValues[i]));
                tempYAxisNegativeData.Add(new ObservablePoint(frecuencies[i], yNegativeValues[i]));
                tempZAxisNegativeData.Add(new ObservablePoint(frecuencies[i], zNegativeValues[i]));
            }

            //Positive
            xAxisPositive.AddRange(tempXAxisPositiveData);
            yAxisPositive.AddRange(tempYAxisPositiveData);
            zAxisPositive.AddRange(tempZAxisPositiveData);

            //Negative
            xAxisNegative.AddRange(tempXAxisNegativeData);
            yAxisNegative.AddRange(tempYAxisNegativeData);
            zAxisNegative.AddRange(tempZAxisNegativeData);

            SeriesCollection = new SeriesCollection
            {
                new LineSeries
                {
                    Values = xAxisPositive,
                    PointGeometrySize = 0,
                    StrokeThickness = 1,
                    Fill = Brushes.Transparent,
                    Title = "Positive X",
                    LineSmoothness = 0.5,
                    Stroke = Brushes.DarkOrange

                },
                new LineSeries
                {
                    Values = yAxisPositive,
                    PointGeometrySize = 0,
                    Title = "Positive Y",
                    Fill = Brushes.Transparent,
                    LineSmoothness = 0.5,
                    Stroke = Brushes.Green
                },
                new LineSeries
                {
                    Values = zAxisPositive,
                    PointGeometrySize = 0,
                    Title = "Positive Z",
                    Fill = Brushes.Transparent,
                    LineSmoothness = 0.5,
                    Stroke = Brushes.DodgerBlue
                },

                //Negative values
                              new LineSeries
                {
                    Values = xAxisNegative,
                    PointGeometrySize = 0,
                    StrokeThickness = 1,
                    Fill = Brushes.Transparent,
                    Title = "Negative X",
                    LineSmoothness = 0.5,
                    Stroke = Brushes.DarkRed

                },
                new LineSeries
                {
                    Values = yAxisNegative,
                    PointGeometrySize = 0,
                    Title = "Negative Y",
                    Fill = Brushes.Transparent,
                    LineSmoothness = 0.5,
                    Stroke = Brushes.DarkViolet
                },
                new LineSeries
                {
                    Values = zAxisNegative,
                    PointGeometrySize = 0,
                    Title = "Negative Z",
                    Fill = Brushes.Transparent,
                    LineSmoothness = 0.5,
                    Stroke = Brushes.DeepPink
                }
            };

            IsBusy = false;
        }

        /// <summary>
        /// Navigate back to the previous page
        /// </summary>
        private void GoBack()
        {
            if (_navigationService.Journal.CanGoBack)
            {
                _navigationService.Journal.GoBack();
            }
        }
    }
}