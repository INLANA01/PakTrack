using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;

namespace PakTrack.UI.Utilities
{
    public static class GraphAxis
    {
        public static DateTimeAxis GetDatetimeAxis()
        {
            return new DateTimeAxis
            {
                Title = "Datetime",
                TitleFormatString = "MMM dd, yyyy",
                IntervalType = DateTimeIntervalType.Days,
                StringFormat = "MMM dd, yyyy",
                TitleFontWeight = FontWeights.Bold,
                IntervalLength = 75,
                MinorIntervalType = DateTimeIntervalType.Days,
                Position = AxisPosition.Bottom,
                MajorGridlineStyle = LineStyle.Dash,
                MinorGridlineStyle = LineStyle.Dot
            };
        }

        /// <summary>
        /// Get a lineal axis representation for the X-Axis
        /// </summary>
        /// <param name="title">Title of the axis</param>
        /// <param name="unit">Unif of measurement</param>
        /// <returns>LinealAxis</returns>
        public static LinearAxis GetLinealAxisForY(string title, string unit)
        {
            return new LinearAxis
            {
                Title = title,
                Unit = unit,
                TitleFontWeight = FontWeights.Bold,
                Position = AxisPosition.Left,
                MajorGridlineStyle = LineStyle.Dot
            };
        }

        /// <summary>
        /// Get the consolidated report logarithmic scale for the Y axis.
        /// </summary>
        /// <returns>LogarithmicAxis</returns>
        public static LogarithmicAxis GetConsolidatedReportLogarithmicAxisForY()
        {
            return new LogarithmicAxis
            {
                Title = "PSD G\xB2/Hz",
                Position = AxisPosition.Left,
                TitleFontWeight = FontWeights.Bold,
                MajorGridlineStyle = LineStyle.Solid,
                MinorGridlineStyle = LineStyle.Dash
            };
        }

        /// <summary>
        /// Get the consolidated report logarithmic scale for the X axis.
        /// </summary>
        /// <returns>LogarithmicAxis</returns>
        public static LogarithmicAxis GetConsolidatedReportLogarithmicAxisForX()
        {
            return new LogarithmicAxis
            {
                Title = "Frecuency (Hz)",
                Position = AxisPosition.Bottom,
                TitleFontWeight = FontWeights.Bold,
                FontSize = 18,
                Minimum = 0,
                MajorGridlineStyle = LineStyle.Solid,
                MinorGridlineStyle = LineStyle.Dash
            };
        }

        /// <summary>
        /// Generate a basic lineseries using blue as the stroke color
        /// </summary>
        /// <param name="sensorType">Name of the sensor</param>
        /// <returns></returns>
        public static LineSeries GetSensorTypeLineSeries(string sensorType)
        {
            return new LineSeries { Color = OxyColor.FromRgb(12, 37, 201), Title = sensorType };
        }

        /// <summary>
        /// Return a basic lines series with red as the stroke color
        /// </summary>
        /// <returns>LineSeries</returns>
        public static LineSeries GetThresholdLineSeries()
        {
            return new LineSeries {Color = OxyColor.FromRgb(226, 4, 41), Title = "Threshold"};
        }
    }
}