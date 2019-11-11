using System;
using System.Collections.Generic;
using System.Linq;
using LiveCharts;
using LiveCharts.Wpf;
using PakTrack.Core;
using PakTrack.Core.Base;
using PakTrack.DAL.Interfaces;
using PakTrack.Models;
using Prism.Events;
using Prism.Regions;
using PakTrack.UI.Events;

namespace PakTrack.UI.ShockArea
{
    public class OrientationHistogramViewModel: PakTrackBindableBase, INavigationAware
    {
        private readonly IRegionManager _regionManager;
        private readonly IShockRepository _shockRepository;

        private IEnumerable<Shock> _shockData;

        public IEnumerable<Shock> ShockData
        {
            get { return _shockData; }
            set { SetProperty(ref _shockData, value); }
        }

        private SeriesCollection _seriesCollection;

        public SeriesCollection SeriesCollection
        {
            get { return _seriesCollection; }
            set { SetProperty(ref _seriesCollection, value); }
        }

        public string[] Labels { get; set; }

        public Func<double, string> Formatter { get; set; }

        public OrientationHistogramViewModel(IRegionManager regionManager, IEventAggregator eventAggregator,
            IShockRepository shockRepository)
        {
            _regionManager = regionManager;
            _shockRepository = shockRepository;
            eventAggregator.GetEvent<NavigationEvent>().Subscribe(OnTruckAndPackageChanged);
        }


        public override void Initialize()
        {
            ShockData = _shockRepository.GetByTruckAndPackageId(TruckId, PackageId);
            var shockData = ShockData as IList<Shock> ?? ShockData.ToList();
            var histogramValues = GetHistogramValues(shockData);
            SeriesCollection = new SeriesCollection
            {
                new StackedColumnSeries()
                {
                    Title = "Instantaneous shock vs Shock Orientation",
                    Values = new ChartValues<int>(histogramValues.Values.Select(x => x[0])),
                    ScalesYAt = 0
                },
                new StackedColumnSeries()
                {
                    Title = "Free Fall Drop Count vs Drop Orientation",
                    Values = new ChartValues<int>(histogramValues.Values.Select(x => x[1])),
                    ScalesYAt = 0
                },
                new LineSeries
                {
                    Title = "Cumulative Percentage Distribution",
                    Values = new ChartValues<double>(GetDistributionValues(histogramValues)),
                    ScalesYAt = 1
                }
            };

            Labels = histogramValues.Keys.ToArray();

            Formatter = value => value.ToString("N");
        }

        public override void OnNavigatedFrom(NavigationContext navigationContext)
        {
            ShockData = null;
        }

        protected override void OnTruckAndPackageChanged(NavigationInformation navigationInformation)
        {
            var regionInfo = _regionManager.Regions[ApplicationRegion.MainRegion];
            if (IsActiveView(regionInfo, typeof(OrientationHistogramView).Name))
            {
                TruckId = navigationInformation.TruckId;
                PackageId = navigationInformation.PackageId;
            }
        }

        private Dictionary<string, int[]> GetHistogramValues(IEnumerable<Shock> shocks)
        {
            var orientationDict = new Dictionary<string,int[]>();
            if (ShockData.Any())
            {
                foreach (var shock in shocks)
                {
                    var orientation = shock.Orientation != null ? string.Join("-", shock.Orientation.ToArray()) : "";
                    if (orientationDict.ContainsKey(orientation))
                    {
                        if (shock.IsInstantaneous)
                        {
                            orientationDict[orientation][0] += 1;
                            orientationDict[orientation][1] += 0;

                        }
                        else
                        {
                            orientationDict[orientation][1] += 1;
                            orientationDict[orientation][1] += 0;
                        }
                    }
                    else
                    {
                        if(shock.IsInstantaneous)
                            orientationDict[orientation]=new[] {1,0};
                        else
                            orientationDict[orientation]=new[] {0,1};
                    }
                }
            }
            return orientationDict;
        }

        private List<double> GetDistributionValues(Dictionary<string, int[]> histDictionary)
        {
            var distValues = new List<double>();
            double sum = 0;
            for (var i = 0; i < histDictionary.Count; i++)
            {
                sum += histDictionary.Values.ElementAt(i)[0] + histDictionary.Values.ElementAt(i)[1];
                var yCoord = Convert.ToDouble(sum) / (histDictionary.Values.Select(x=>x[0]).Sum() + histDictionary.Values.Select(x => x[1]).Sum()) * 100;
                distValues.Add(yCoord);
            }
            return distValues;
        }
    }
}