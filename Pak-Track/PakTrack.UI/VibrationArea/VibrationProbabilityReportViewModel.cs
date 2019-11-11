using System;
using System.Collections.Generic;
using System.Linq;
using LiteDB;
using OxyPlot;
using OxyPlot.Series;
using PakTrack.Core;
using PakTrack.Core.Base;
using PakTrack.DAL.Interfaces;
using PakTrack.Models;
using PakTrack.UI.Events;
using PakTrack.Utilities;
using Prism.Commands;
using Prism.Events;
using Prism.Regions;
using OxyPlot.Axes;

namespace PakTrack.UI.VibrationArea
{
    public class VibrationProbabilityReportViewModel: PakTrackBindableBase, INavigationAware
    {
        private readonly IRegionManager _regionManager;
        private readonly IVibrationRepository _vibrationRepository;
        private HistTools _histHelper = new HistTools();
        private bool _isBusy;

        private IEnumerable<Vibration> _vibrationData;
        private PlotModel _modelX;

        public PlotModel ModelX
        {
            get { return _modelX; }
            set { SetProperty(ref _modelX, value); }
        }

        private PlotModel _modelY;

        public PlotModel ModelY
        {
            get { return _modelY; }
            set { SetProperty(ref _modelY, value); }
        }

        private PlotModel _modelZ;

        public PlotModel ModelZ
        {
            get { return _modelZ; }
            set { SetProperty(ref _modelZ, value); }
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

        public PlotController HoverController { get; private set; }

        public VibrationProbabilityReportViewModel(IVibrationRepository vibrationRepository, IRegionManager regionManager,
            IEventAggregator eventAggregator)
        {
            _vibrationRepository = vibrationRepository;
            _regionManager = regionManager;
            
            eventAggregator.GetEvent<NavigationEvent>().Subscribe(OnTruckAndPackageChanged);
            HoverController = new PlotController();
            HoverController.UnbindMouseDown(OxyMouseButton.Left);
            HoverController.BindMouseEnter(PlotCommands.HoverSnapTrack);
        }

        private void OnTruckAndPackageChanged(NavigationInformation navigationInformation)
        {
            var regionInfo = _regionManager.Regions[ApplicationRegion.MainRegion];
            if (IsActiveView(regionInfo, typeof(VibrationView).Name))
            {
                //Title = "Event executed" + navigationInformation.TruckId;
                TruckId = navigationInformation.TruckId;
                PackageId = navigationInformation.PackageId;
                //  Initialize();
            }
        }

        public override void OnNavigatedFrom(NavigationContext navigationContext)
        {
            VibrationData = null;
            ModelX.Axes.Clear();
            ModelY.Axes.Clear();
            ModelZ.Axes.Clear();
            ModelX.Series.Clear();
            ModelY.Series.Clear();
            ModelZ.Series.Clear();
            ModelX = null;
            ModelY = null;
            ModelZ = null;
        }

        public override void Initialize()
        {
            IsBusy = true;
            VibrationData = _vibrationRepository.GetByTruckAndPackageId(TruckId, PackageId);
            PlotGraph();
            IsBusy = false;
        }

        /// <summary>
        /// Compute Bins and HistCount
        /// </summary>
        /// <param name="grms"></param>
        /// <returns>Tuple that contains bins and hist count</returns>
        private Tuple<List<double>, List<int>> GetHistogramPoints(List<double> grms)
        {
            List<double> bins;
            var histCount = new List<int>();
            var binWidth = _histHelper.ScottsBinWidth(grms, out bins);
            for (var i = 0; i < bins.Count; i++)
            {
                var lower = bins[i];
                var higher = lower + binWidth;
                var countForBin = grms.Count(grm => grm >= lower && grm < higher);
                histCount.Add(countForBin);
            }
            bins.Add(bins[bins.Count-1] + binWidth);
            return new Tuple<List<double>, List<int>>(bins, histCount);
        }

        private void PlotGraph()
        {
           
            PlotXAxisGraph();
            PlotYAxisGraph();
            PlotZAxisGraph();
        }

        private void PlotXAxisGraph()
        {
            var grmsX = new List<double>();
            if (VibrationData.Any())
            {
                grmsX.AddRange(VibrationData.Select(vibration => vibration.GRMS.X));
                var histPoints = GetHistogramPoints(grmsX);
                var binxX = histPoints.Item1;
                var histCountX = histPoints.Item2;

                ModelX = new PlotModel { Title = "GRMS Histogram Plot X-Axis"  };
                var rs = RectangleBarSeries(bins:binxX,histCount:histCountX);
                var xAxis = new LinearAxis { Title = "Grms", Position = AxisPosition.Bottom, Key = "X", IsZoomEnabled = false, IsPanEnabled = false};
                var yAxis = new LinearAxis { Title = "Number of Events", Position = AxisPosition.Left, Minimum = 0, IsZoomEnabled = false, Key="C", IsPanEnabled = false};
                ModelX.Axes.Add(xAxis);
                ModelX.Axes.Add(yAxis);
                rs.YAxisKey = "C";
                rs.XAxisKey = "X";

                var ls = PercentageLineSeries(binxX,histCountX);
                var percentageAxis = new LinearAxis { Title = "Cumulative Percentage", Position = AxisPosition.Right, Minimum = 0, Maximum = 101, Key = "PA" , IsZoomEnabled = false, IsPanEnabled = false};
                ModelX.Axes.Add(percentageAxis);
                ls.YAxisKey = "PA";
                ls.XAxisKey = "X";
                ModelX.Series.Add(rs);
                ModelX.Series.Add(ls);
            }

        }

        private void PlotYAxisGraph()
        {
            var grmsY = new List<double>();
            if (VibrationData.Any())
            {
                grmsY.AddRange(VibrationData.Select(vibration => vibration.GRMS.Y));
                var histPoints = GetHistogramPoints(grmsY);
                var binxY = histPoints.Item1;
                var histCountY = histPoints.Item2;

                ModelY = new PlotModel { Title = "GRMS Histogram Plot Y-Axis" };
                var rs = RectangleBarSeries(bins: binxY, histCount: histCountY);
                var xAxis = new LinearAxis { Title = "Grms", Position = AxisPosition.Bottom, Key = "X", IsZoomEnabled = false, IsPanEnabled = false};
                var yAxis = new LinearAxis { Title = "Number of Events", Position = AxisPosition.Left, Minimum = 0, IsZoomEnabled = false, Key = "C" , IsPanEnabled = false};
                rs.YAxisKey = "C";
                rs.XAxisKey = "X";
                ModelY.Axes.Add(xAxis);
                ModelY.Axes.Add(yAxis);

                var ls = PercentageLineSeries(binxY, histCountY);
                var percentageAxis = new LinearAxis { Title = "Cumulative Percentage", Position = AxisPosition.Right, Minimum = 0, Maximum = 101, Key = "PA", IsZoomEnabled = false, IsPanEnabled = false};
                ModelY.Axes.Add(percentageAxis);
                ls.YAxisKey = "PA";
                ls.XAxisKey = "X";
                ModelY.Series.Add(rs);
                ModelY.Series.Add(ls);
            }
        }

        private void PlotZAxisGraph()
        {
            var grmsZ = new List<double>();
            if (VibrationData.Any())
            {
                grmsZ.AddRange(VibrationData.Select(vibration => vibration.GRMS.Z));
                var histPoints = GetHistogramPoints(grmsZ);
                var binxZ = histPoints.Item1;
                var histCountZ = histPoints.Item2;

                ModelZ = new PlotModel { Title = "GRMS Histogram Plot Z-Axis" };
                var rs = RectangleBarSeries(bins: binxZ, histCount: histCountZ);
                var xAxis = new LinearAxis { Title = "Grms", Position = AxisPosition.Bottom, Key = "X", IsZoomEnabled = false, IsPanEnabled = false};
                var yAxis = new LinearAxis { Title = "Number of Events", Position = AxisPosition.Left, Minimum = 0, IsZoomEnabled = false, Key = "C", IsPanEnabled = false};
                ModelZ.Axes.Add(xAxis);
                ModelZ.Axes.Add(yAxis);
                rs.YAxisKey = "C";
                rs.XAxisKey = "X";

                var ls = PercentageLineSeries(binxZ, histCountZ);
                var percentageAxis = new LinearAxis { Title = "Cumulative Percentage", Position = AxisPosition.Right, Minimum = 0, Maximum = 101, Key = "PA" , IsZoomEnabled = false, IsPanEnabled = false};
                ModelZ.Axes.Add(percentageAxis);
                ls.YAxisKey = "PA";
                ls.XAxisKey = "X";
                ModelZ.Series.Add(rs);
                ModelZ.Series.Add(ls);
            }
        }

        private LineSeries PercentageLineSeries(List<double> bins, List<int> histCount)
        {
            var l = new LineSeries();
            double mySum = 0;
            for (var i = 0; i < histCount.Count; i++)
            {
                var xCoord = (bins[i] + bins[i + 1]) / 2;
                mySum += histCount[i];
                var yCoord = Convert.ToDouble(mySum) / histCount.Sum() * 100;
                l.Points.Add(new DataPoint(xCoord, yCoord));
            }

            return l;
        }

        public RectangleBarSeries RectangleBarSeries(List<double> bins, List<int> histCount)
        {
            
            var s1 = new RectangleBarSeries();
            for (var i = 0; i < histCount.Count; i++)
            {
                
                s1.Items.Add(new RectangleBarItem { X0 = bins[i], X1 = bins[i + 1], Y0 = 0, Y1 = histCount[i]});
                
            }
            return s1;
        }
    }
}