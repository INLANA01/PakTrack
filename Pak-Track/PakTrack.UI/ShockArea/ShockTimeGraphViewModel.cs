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
    public class ShockTimeGraphViewModel : PakTrackBindableBase, INavigationAware
    {
        private readonly IShockRepository _shockRepository;
        private IRegionNavigationService _navigationService;

        public ObjectId ShockId { get; set; }

        private SeriesCollection _seriesCollection;

        public SeriesCollection SeriesCollection
        {
            get { return _seriesCollection; }
            set { SetProperty(ref _seriesCollection, value); }
        }

        public DelegateCommand GoBackCommand { get; private set; }

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

        public ShockTimeGraphViewModel(IShockRepository shockRepository)
        {
            _shockRepository = shockRepository;
            Title = "Shock Time-Domain Graph";
            GoBackCommand = new DelegateCommand(GoBack)
                .ObservesCanExecute((o)=>CanGoBack);
        }

        public override void OnNavigatedTo(NavigationContext navigationContext)
        {
            _navigationService = navigationContext.NavigationService;
            CanGoBack = navigationContext.NavigationService.Journal.CanGoBack;
            ShockId = navigationContext.Parameters[PakTrackConstant.EventId] as ObjectId;
            var graphType = (GraphType)navigationContext.Parameters[PakTrackConstant.GraphType];
        }

        public override void OnNavigatedFrom(NavigationContext navigationContext)
        {
            SeriesCollection = null;
        }

        public override void Initialize()
        {
            var shock = _shockRepository.GetById(ShockId);
            if (shock == null)
                return;

            const double sampleTime = 0.000625;
            var timestamp = 1.0;


            var xValue = shock.X.AsEnumerable().ToArray();
            var yValue = shock.Y.AsEnumerable().ToArray();
            var zValue = shock.Z.AsEnumerable().ToArray();

            var xAxis = new ChartValues<ObservablePoint>();
            var yAxis = new ChartValues<ObservablePoint>();
            var zAxis = new ChartValues<ObservablePoint>();

            var totalPoint = shock.X.Count;

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
                    Title = "Shock X",
                    LineSmoothness = 0.5,
                    Stroke = Brushes.DarkOrange

                },
                new LineSeries
                {
                    Values = yAxis,
                    PointGeometrySize = 0,
                    Title = "Shock Y",
                    Fill = Brushes.Transparent,
                    LineSmoothness = 0.5,
                    Stroke = Brushes.Green
                },
                new LineSeries
                {
                    Values = zAxis,
                    PointGeometrySize = 0,
                    Title = "Shock Z",
                    Fill = Brushes.Transparent,
                    LineSmoothness = 0.5,
                    Stroke = Brushes.DodgerBlue
                }
            };
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
