using System;
using System.Collections.Generic;
using System.Collections.Specialized;
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
    public class DropHeightHistogramViewModel : PakTrackBindableBase, INavigationAware
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
        private bool _isDropHeight= false;

        public SeriesCollection SeriesCollection
        {
            get { return _seriesCollection; }
            set { SetProperty(ref _seriesCollection, value); }
        }

        public bool IsDropHeight
        {
            get { return _isDropHeight; }
            set { SetProperty(ref _isDropHeight, value); }
        }
        public string[] Labels { get; set; }

        public Func<double, string> Formatter { get; set; }

        public DropHeightHistogramViewModel(IRegionManager regionManager, IEventAggregator eventAggregator,
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
            var histogramValues = GetHistogramValues(shockData.Where(x => !x.IsInstantaneous));
            if (!histogramValues.Any())
            {
                IsDropHeight = false;
                return;
            }
            IsDropHeight = true;
            var orderedHistValues = histogramValues.OrderBy(value => float.Parse(value.Key));
            SeriesCollection = new SeriesCollection
            {
                new StackedColumnSeries()
                {
                    Title = "Drop Count vs Drop Height",
                    Values = new ChartValues<int>(orderedHistValues.Select(x=>x.Value)),
                    ScalesYAt = 0
                    
                },

                new LineSeries
                {
                    Title = "Cumulative Percentage Distribution",
                    Values = new ChartValues<double>(GetDistributionValues(orderedHistValues)),
                    ScalesYAt = 1
                }
            };

            Labels = orderedHistValues.Select(x=>x.Key).ToArray();
        }

        public override void OnNavigatedFrom(NavigationContext navigationContext)
        {
            ShockData = null;
        }

        protected override void OnTruckAndPackageChanged(NavigationInformation navigationInformation)
        {
            var regionInfo = _regionManager.Regions[ApplicationRegion.MainRegion];
            if (IsActiveView(regionInfo, typeof(DropHeightHistogramView).Name))
            {
                TruckId = navigationInformation.TruckId;
                PackageId = navigationInformation.PackageId;
            }
        }

        private Dictionary<string, int> GetHistogramValues(IEnumerable<Shock> shocks)
        {
            var dropHeightDict = new Dictionary<string, int>();
            if (ShockData.Any())
            {
                foreach (var shock in shocks)
                {
                    var dropHeight = shock.DropHeight.ToString("##0.00");
                    if (dropHeightDict.ContainsKey(dropHeight))
                    {
                        dropHeightDict[dropHeight] += 1;
                    }
                    else
                    {
                        dropHeightDict.Add(dropHeight, 1);
                    }
                }
            }
            return dropHeightDict;
        }

        private List<double> GetDistributionValues(IOrderedEnumerable<KeyValuePair<string, int>> histDictionary)
        {
            var distValues = new List<double>();
            var histValues = histDictionary.Select(x => x.Value).ToList();
            double sum = 0;
            for (var i = 0; i < histValues.Count; i++)
            {
                sum += histValues.ElementAt(i);
                var yCoord = Convert.ToDouble(sum) / histValues.Sum() * 100;
                distValues.Add(yCoord);
            }
            return distValues;
        }
    }
}

