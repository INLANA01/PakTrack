using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using LiveCharts;
using LiveCharts.Configurations;
using LiveCharts.Defaults;
using LiveCharts.Wpf;
using PakTrack.BL.Models;
using PakTrack.Core.Base;
using PakTrack.Utilities;
using Prism.Regions;
using OxyPlot;
using OxyPlot.Axes;
using PakTrack.Core;
using PakTrack.UI.Utilities;
using FontWeights = OxyPlot.FontWeights;

//using OxyPlot.Series;

namespace PakTrack.UI.VibrationArea
{
    public class VibrationConsolidatedReportDetailsViewModel : PakTrackBindableBase, INavigationAware
    {
        private double _xAxisMinValue;


        private VibrationConsolidatedReport _reportDetails;
        private List<VibrationConsolidatedReport> _report50Details;

        private string _xAxisTitle;
        private string _yAxisTitle;
        private string _zAxisTitle;
        private string _vecAxisTitle;

        public String XAXisTitle
        {
            get { return _xAxisTitle; }
            set { SetProperty(ref _xAxisTitle, value); }
        }

        public String YAXisTitle
        {
            get { return _yAxisTitle; }
            set { SetProperty(ref _yAxisTitle, value); }
        }

        public String ZAXisTitle
        {
            get { return _zAxisTitle; }
            set { SetProperty(ref _zAxisTitle, value); }
        }


        public String VecAXisTitle
        {
            get { return _vecAxisTitle; }
            set { SetProperty(ref _vecAxisTitle, value); }
        }


        private string _xGrmsText;

        public string XGRMSText
        {
            get { return _xGrmsText; }
            set { SetProperty(ref _xGrmsText, value); }
        }


        private string _xGrmsText50;

        public string XGRMSText50
        {
            get { return _xGrmsText50; }
            set { SetProperty(ref _xGrmsText50, value); }
        }

        private string _xGrmsText95;

        public string XGRMSText95
        {
            get { return _xGrmsText95; }
            set { SetProperty(ref _xGrmsText95, value); }
        }

        private string _xGrmsText90;

        public string XGRMSText90
        {
            get { return _xGrmsText90; }
            set { SetProperty(ref _xGrmsText90, value); }
        }


        private string _yGrmsText;

        public string YGRMSText
        {
            get { return _yGrmsText; }
            set { SetProperty(ref _yGrmsText, value); }
        }


        private string _yGrmsText50;

        public string YGRMSText50
        {
            get { return _yGrmsText50; }
            set { SetProperty(ref _yGrmsText50, value); }
        }

        private string _yGrmsText95;

        public string YGRMSText95
        {
            get { return _yGrmsText95; }
            set { SetProperty(ref _yGrmsText95, value); }
        }

        private string _yGrmsText90;

        public string YGRMSText90
        {
            get { return _yGrmsText90; }
            set { SetProperty(ref _yGrmsText90, value); }
        }

        private string _zGrmsText;

        public string ZGRMSText
        {
            get { return _zGrmsText; }
            set { SetProperty(ref _zGrmsText, value); }
        }


        private string _zGrmsText50;

        public string ZGRMSText50
        {
            get { return _zGrmsText50; }
            set { SetProperty(ref _zGrmsText50, value); }
        }

        private string _zGrmsText95;

        public string ZGRMSText95
        {
            get { return _zGrmsText95; }
            set { SetProperty(ref _zGrmsText95, value); }
        }

        private string _zGrmsText90;

        public string ZGRMSText90
        {
            get { return _zGrmsText90; }
            set { SetProperty(ref _zGrmsText90, value); }
        }


        public VibrationConsolidatedReport ReportDetails
        {
            get { return _reportDetails; }
            set { SetProperty(ref _reportDetails, value); }
        }

        public List<VibrationConsolidatedReport> ReportDetails50Percent
        {
            get { return _report50Details; }
            set { SetProperty(ref _report50Details, value); }
        }
        private SeriesCollection _xAxisGraphData;

        public SeriesCollection XAxisGraphData
        {
            get { return _xAxisGraphData; }
            set { SetProperty(ref _xAxisGraphData, value); }
        }

        private SeriesCollection _yAxisGraphData;

        public SeriesCollection YAxisGraphData
        {
            get { return _yAxisGraphData; }
            set { SetProperty(ref _yAxisGraphData, value); }
        }

        private SeriesCollection _zAxisGraphData;

        public SeriesCollection ZAxisGraphData
        {
            get { return _zAxisGraphData; }
            set { SetProperty(ref _zAxisGraphData, value); }
        }

        private SeriesCollection _vectorGraphData;

        public SeriesCollection VectorGraphData
        {
            get { return _vectorGraphData; }
            set { SetProperty(ref _vectorGraphData, value); }
        }

        public Func<double, string> Formatter { get; set; }


        public override void OnNavigatedTo(NavigationContext navigationContext)
        {
            ReportDetails =
                navigationContext.Parameters[PakTrackConstant.VibrationConsolidatedReportDetails] as
                    VibrationConsolidatedReport;
             ReportDetails50Percent = 
                navigationContext.Parameters[PakTrackConstant.VibrationConsolidatedReportNDetails] as
                List<VibrationConsolidatedReport>;
            //ProcessGraphs();
        }

        public override void OnNavigatedFrom(NavigationContext navigationContext)
        {
            ReportDetails = null;
            XAxisGraphData = null;
            YAxisGraphData = null;
            ZAxisGraphData = null;
            VectorGraphData = null;
           
            //OxyPlot
            XAxisPlotModel = null;
            YAxisPlotModel = null;
            ZAxisPlotModel = null;
            VectorPlotModel = null;
        }

        private void ProcessGraphs()
        {
            var xAxis = new ChartValues<ObservablePoint>();
            var yAxis = new ChartValues<ObservablePoint>();
            var zAxis = new ChartValues<ObservablePoint>();
            var vectorAxis = new ChartValues<ObservablePoint>();

            //Min values for log function
            _xAxisMinValue = ReportDetails.XAxis.Select(t => t.Value).ToArray().Min();
            var yAxisMinValue = ReportDetails.YAxis.Select(t => t.Value).ToArray().Min();


            var tempXAxisData = ReportDetails.XAxis
                .Select(t => new ObservablePoint(t.Frequency, t.Value));

            var tempYAxisData = ReportDetails.YAxis
                .Select(t => new ObservablePoint(t.Frequency, t.Value));

            var tempZAxisData = ReportDetails.ZAxis
                .Select(t => new ObservablePoint(t.Frequency, t.Value));

            var tempVectorData = ReportDetails.Vector
                .Select(t => new ObservablePoint(t.Frequency, t.Value));

            //Add temp data to series values
            xAxis.AddRange(tempXAxisData);
            yAxis.AddRange(tempYAxisData);
            zAxis.AddRange(tempZAxisData);
            vectorAxis.AddRange(tempVectorData);

            // XAxis = xAxis;
            XAxisGraphData = new SeriesCollection(Mappers.Xy<ObservablePoint>()
                .X(GetXAxis20Log)
                .Y(t => GetYAxis20Log1(t.Y, _xAxisMinValue)))
            {
                new LineSeries
                {
                    Values = xAxis,
                    PointGeometrySize = 10,
                    PointGeometry = DefaultGeometries.Circle,
                    StrokeThickness = 1,
                    Fill = Brushes.Transparent,
                    Title = "X-Axis",
                    LineSmoothness = 0.5
                }
            };

            //Y-Axis Serie
            YAxisGraphData = new SeriesCollection(Mappers.Xy<ObservablePoint>()
                .X(GetXAxis20Log)
                .Y(t => GetYAxis20Log1(t.Y, yAxisMinValue)))
            {
                new LineSeries
                {
                    Values = yAxis,
                    PointGeometrySize = 10,
                    PointGeometry = DefaultGeometries.Circle,
                    StrokeThickness = 1,
                    Fill = Brushes.Transparent,
                    Title = "Y-Axis",
                    LineSmoothness = 0.5
                }
            };

            //Z-Axis Serie
            ZAxisGraphData = new SeriesCollection(Mappers.Xy<ObservablePoint>()
                .X(GetXAxis20Log)
                .Y(t => GetYAxis20Log1(t.Y, yAxisMinValue)))
            {
                new LineSeries
                {
                    Values = zAxis,
                    PointGeometrySize = 10,
                    PointGeometry = DefaultGeometries.Circle,
                    StrokeThickness = 1,
                    Fill = Brushes.Transparent,
                    Title = "Z-Axis",
                    LineSmoothness = 0.5
                }
            };

            //Z-Axis Serie
            VectorGraphData = new SeriesCollection(Mappers.Xy<ObservablePoint>()
                .X(GetXAxis20Log)
                .Y(t => GetYAxis20Log1(t.Y, yAxisMinValue)))
            {
                new LineSeries
                {
                    Values = vectorAxis,
                    PointGeometrySize = 10,
                    PointGeometry = DefaultGeometries.Circle,
                    StrokeThickness = 1,
                    Fill = Brushes.Transparent,
                    Title = "Vector",
                    LineSmoothness = 0.5
                }
            };

            Formatter = value => (20*Math.Pow(10, value)).ToString("E2");
        }

        private double GetYAxis20Log1(double value, double yAxisMinValue)
        {
            return (20*Math.Log10(value/yAxisMinValue));
        }


        private static double GetXAxis20Log(ObservablePoint p)
        {
            return (20*Math.Log10(p.X));
        }


        private double GetYAxis20Log(ObservablePoint point)
        {
            return (20*Math.Log10(point.Y/_xAxisMinValue));
        }

        #region Plot Models

        private PlotModel _xAxisPlotModel;

        public PlotModel XAxisPlotModel
        {
            get { return _xAxisPlotModel; }
            set { SetProperty(ref _xAxisPlotModel, value); }
        }

        private PlotModel _yAxisPlotModel;

        public PlotModel YAxisPlotModel
        {
            get { return _yAxisPlotModel; }
            set { SetProperty(ref _yAxisPlotModel, value); }
        }

        private PlotModel _zAxisPlotModel;

        public PlotModel ZAxisPlotModel
        {
            get { return _zAxisPlotModel; }
            set { SetProperty(ref _zAxisPlotModel, value); }
        }

        private PlotModel _vectorPlotModel;

        public PlotModel VectorPlotModel
        {
            get { return _vectorPlotModel; }
            set { SetProperty(ref _vectorPlotModel, value); }
        }

        public LogarithmicAxis LogXAxis { get; set; }
        public LogarithmicAxis LogYAxis { get; set; }

        #endregion

        public void PlotLogarithmicGraph()
        {

            //Plot Models
            XAxisPlotModel = new PlotModel {Title = "X-Axis plot of G\xB2/Hz vs Frequency (Hz)" };
            YAxisPlotModel = new PlotModel { Title = "Y-Axis plot of G\xB2/Hz vs Frequency (Hz)" };
            ZAxisPlotModel = new PlotModel { Title = "Z-Axis plot of G\xB2/Hz vs Frequency (Hz)" };
            
//            VectorPlotModel = new PlotModel { Title = "Vector plot of G\xB2/Hz vs Frequency (Hz)" };

            #region Line Series data points

            //X
            var xAxisLineSeries = new OxyPlot.Series.LineSeries { Color = OxyColor.FromRgb(255, 255, 0), Title = "100% PSD"};
            xAxisLineSeries.Points.AddRange(GetDataPointsByAxis(PakTrackAxis.X, ReportDetails));
            XAxisPlotModel.Series.Add(xAxisLineSeries);

            var xAxisLineSeries50 = new OxyPlot.Series.LineSeries { Color = OxyColor.FromRgb(255, 0, 0) , Title = "50% PSD" };
            xAxisLineSeries50.Points.AddRange(GetDataPointsByAxis(PakTrackAxis.X, ReportDetails50Percent[0]));
            XAxisPlotModel.Series.Add(xAxisLineSeries50);

            var xAxisLineSeries95 = new OxyPlot.Series.LineSeries { Color = OxyColor.FromRgb(0, 255, 0) , Title = "95% PSD" };
            xAxisLineSeries95.Points.AddRange(GetDataPointsByAxis(PakTrackAxis.X, ReportDetails50Percent[1]));
            XAxisPlotModel.Series.Add(xAxisLineSeries95);

            var xAxisLineSeries90 = new OxyPlot.Series.LineSeries { Color = OxyColor.FromRgb(0, 0, 255) , Title = "90% PSD" };
            xAxisLineSeries90.Points.AddRange(GetDataPointsByAxis(PakTrackAxis.X, ReportDetails50Percent[2]));
            XAxisPlotModel.Series.Add(xAxisLineSeries90);

            //Y
            var yAxisLineSeries = new OxyPlot.Series.LineSeries { Color = OxyColor.FromRgb(255, 255, 0), Title = "100% PSD" };
            yAxisLineSeries.Points.AddRange(GetDataPointsByAxis(PakTrackAxis.Y, ReportDetails));
            YAxisPlotModel.Series.Add(yAxisLineSeries);

            var yAxisLineSeries50 = new OxyPlot.Series.LineSeries { Color = OxyColor.FromRgb(255, 0, 0), Title = "50% PSD" };
            yAxisLineSeries50.Points.AddRange(GetDataPointsByAxis(PakTrackAxis.Y, ReportDetails50Percent[0]));
            YAxisPlotModel.Series.Add(yAxisLineSeries50);

            var yAxisLineSeries95 = new OxyPlot.Series.LineSeries { Color = OxyColor.FromRgb(0, 255, 0), Title = "95% PSD" };
            yAxisLineSeries95.Points.AddRange(GetDataPointsByAxis(PakTrackAxis.Y, ReportDetails50Percent[1]));
            YAxisPlotModel.Series.Add(yAxisLineSeries95);

            var yAxisLineSeries90 = new OxyPlot.Series.LineSeries { Color = OxyColor.FromRgb(0, 0, 255), Title = "90% PSD" };
            yAxisLineSeries90.Points.AddRange(GetDataPointsByAxis(PakTrackAxis.Y, ReportDetails50Percent[2]));
            YAxisPlotModel.Series.Add(yAxisLineSeries90);

            //Z
            var zAxisLineSeries = new OxyPlot.Series.LineSeries { Color = OxyColor.FromRgb(255, 255, 0), Title = "100% PSD" };
            zAxisLineSeries.Points.AddRange(GetDataPointsByAxis(PakTrackAxis.Z, ReportDetails));
            ZAxisPlotModel.Series.Add(zAxisLineSeries);

            var zAxisLineSeries50 = new OxyPlot.Series.LineSeries { Color = OxyColor.FromRgb(255, 0, 0), Title = "50% PSD" };
            zAxisLineSeries50.Points.AddRange(GetDataPointsByAxis(PakTrackAxis.Z, ReportDetails50Percent[0]));
            ZAxisPlotModel.Series.Add(zAxisLineSeries50);

            var zAxisLineSeries95 = new OxyPlot.Series.LineSeries { Color = OxyColor.FromRgb(0, 255, 0), Title = "95% PSD" };
            zAxisLineSeries95.Points.AddRange(GetDataPointsByAxis(PakTrackAxis.Z, ReportDetails50Percent[1]));
            ZAxisPlotModel.Series.Add(zAxisLineSeries95);

            var zAxisLineSeries90 = new OxyPlot.Series.LineSeries { Color = OxyColor.FromRgb(0, 0, 255), Title = "90% PSD" };
            zAxisLineSeries90.Points.AddRange(GetDataPointsByAxis(PakTrackAxis.Z, ReportDetails50Percent[2]));
            ZAxisPlotModel.Series.Add(zAxisLineSeries90);

            //Vector
//            var vectorLineSeries = new OxyPlot.Series.LineSeries { Color = OxyColor.FromRgb(255, 255, 0), Title = "100% PSD" };
//            vectorLineSeries.Points.AddRange(GetDataPointsByAxis(PakTrackAxis.Vector, ReportDetails));
//            VectorPlotModel.Series.Add(vectorLineSeries);
//
//            var vectorLineSeries50 = new OxyPlot.Series.LineSeries { Color = OxyColor.FromRgb(255, 0, 0), Title = "50% PSD" };
//            vectorLineSeries50.Points.AddRange(GetDataPointsByAxis(PakTrackAxis.Vector, ReportDetails50Percent[0]));
//            VectorPlotModel.Series.Add(vectorLineSeries50);
//
//            var vectorLineSeries95 = new OxyPlot.Series.LineSeries { Color = OxyColor.FromRgb(0, 255, 0), Title = "95% PSD" };
//            vectorLineSeries95.Points.AddRange(GetDataPointsByAxis(PakTrackAxis.Vector, ReportDetails50Percent[1]));
//            VectorPlotModel.Series.Add(vectorLineSeries95);
//
//            var vectorLineSeries90 = new OxyPlot.Series.LineSeries { Color = OxyColor.FromRgb(0, 0, 255), Title = "90% PSD" };
//            vectorLineSeries90.Points.AddRange(GetDataPointsByAxis(PakTrackAxis.Vector, ReportDetails50Percent[2]));
//            VectorPlotModel.Series.Add(vectorLineSeries90);
            #endregion

            //X
            XAxisPlotModel.Axes.Add(GraphAxis.GetConsolidatedReportLogarithmicAxisForX());
            XAxisPlotModel.Axes.Add(GraphAxis.GetConsolidatedReportLogarithmicAxisForY());
            //Y
            YAxisPlotModel.Axes.Add(GraphAxis.GetConsolidatedReportLogarithmicAxisForX());
            YAxisPlotModel.Axes.Add(GraphAxis.GetConsolidatedReportLogarithmicAxisForY());
            //Z
            ZAxisPlotModel.Axes.Add(GraphAxis.GetConsolidatedReportLogarithmicAxisForX());
            ZAxisPlotModel.Axes.Add(GraphAxis.GetConsolidatedReportLogarithmicAxisForY());
            //Vector
//            VectorPlotModel.Axes.Add(GraphAxis.GetConsolidatedReportLogarithmicAxisForX());
//            VectorPlotModel.Axes.Add(GraphAxis.GetConsolidatedReportLogarithmicAxisForY());



        }

        private void LabelGrms()
        {

            XGRMSText = "Grms: " + Math.Round(ReportDetails.GRMS.X, 4);
            YGRMSText = "Grms: " + Math.Round(ReportDetails.GRMS.Y, 4);
            ZGRMSText = "Grms: " + Math.Round(ReportDetails.GRMS.Z, 4);

            XGRMSText50 = "Grms (50%): " + (ReportDetails50Percent[0] != null ?
                              Math.Round(ReportDetails50Percent[0].GRMS.X, 4).ToString(CultureInfo.InvariantCulture) : "");
            YGRMSText50 = "Grms (50%): " + (ReportDetails50Percent[0] != null ?
                              Math.Round(ReportDetails50Percent[0].GRMS.Y, 4).ToString(CultureInfo.InvariantCulture) : "");
            ZGRMSText50 = "Grms (50%): " + (ReportDetails50Percent[0] != null ?
                              Math.Round(ReportDetails50Percent[0].GRMS.Z, 4).ToString(CultureInfo.InvariantCulture) : "");

            XGRMSText95 = "Grms (95%): " + (ReportDetails50Percent[1] != null ?
                              Math.Round(ReportDetails50Percent[1].GRMS.X, 4).ToString(CultureInfo.InvariantCulture) : "");
            YGRMSText95 = "Grms (95%): " + (ReportDetails50Percent[1] != null ?
                              Math.Round(ReportDetails50Percent[1].GRMS.Y, 4).ToString(CultureInfo.InvariantCulture) : "");
            ZGRMSText95 = "Grms (95%): " + (ReportDetails50Percent[1] != null ?
                              Math.Round(ReportDetails50Percent[1].GRMS.Z, 4).ToString(CultureInfo.InvariantCulture) : "");


            XGRMSText90 = "Grms (90%): " + (ReportDetails50Percent[2] != null ?
                              Math.Round(ReportDetails50Percent[2].GRMS.X, 4).ToString(CultureInfo.InvariantCulture) : "");
            YGRMSText90 = "Grms (90%): " + (ReportDetails50Percent[2] != null ?
                              Math.Round(ReportDetails50Percent[2].GRMS.Y, 4).ToString(CultureInfo.InvariantCulture) : "");
            ZGRMSText90 = "Grms (90%): " + (ReportDetails50Percent[2] != null ?
                              Math.Round(ReportDetails50Percent[2].GRMS.Z, 4).ToString(CultureInfo.InvariantCulture) : "");
        }        /// <summary>
        /// Get the datapoints for a given axis
        /// </summary>
        /// <param name="axis">Axis to get the points</param>
        /// <returns>List<DataPoint></returns>
        private List<DataPoint> GetDataPointsByAxis(PakTrackAxis axis, VibrationConsolidatedReport sourceConsolidatedReport)
        {
            var dataPoints = new List<DataPoint>();

            switch (axis)
            {
                case PakTrackAxis.X:
                    //X-Axis data
                    dataPoints.Add(new DataPoint(1, sourceConsolidatedReport.XAxis.Select(t => t.Value).ToArray().Min()));
                    dataPoints.AddRange(sourceConsolidatedReport.XAxis
                        .Select(t => new DataPoint(t.Frequency, t.Value)));
                    break;
                case PakTrackAxis.Y:
                    //Y-Axis data
                    dataPoints.Add(new DataPoint(1, sourceConsolidatedReport.YAxis.Select(t => t.Value).ToArray().Min()));
                    dataPoints.AddRange(sourceConsolidatedReport.YAxis
                        .Select(t => new DataPoint(t.Frequency, t.Value)));
                    break;
                case PakTrackAxis.Z:
                    //Z-Axis data
                    dataPoints.Add(new DataPoint(1, sourceConsolidatedReport.ZAxis.Select(t => t.Value).ToArray().Min()));
                    dataPoints.AddRange(sourceConsolidatedReport.ZAxis
                        .Select(t => new DataPoint(t.Frequency, t.Value)));
                    break;
                case PakTrackAxis.Vector:
                    //Vector data
                    dataPoints.Add(new DataPoint(1, sourceConsolidatedReport.Vector.Select(t => t.Value).ToArray().Min()));
                    dataPoints.AddRange(sourceConsolidatedReport.Vector
                        .Select(t => new DataPoint(t.Frequency, t.Value)));
                    break;
            }

            return dataPoints;
        }

        public override void Initialize()
        {
            XAXisTitle = "X-Axis";
            YAXisTitle = "Y-Axis";
            ZAXisTitle = "Z-Axis";
//            VecAXisTitle = "Vector";
            PlotLogarithmicGraph();
            LabelGrms();
        }

    }
}