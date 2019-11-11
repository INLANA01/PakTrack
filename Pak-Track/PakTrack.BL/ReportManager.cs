using System;
using System.Collections.Generic;
using System.Linq;
using PakTrack.BL.Interfaces;
using PakTrack.BL.Models;
using PakTrack.Models;

namespace PakTrack.BL
{
    public class ReportManager : IReportManager
    {
        /// <summary>
        ///     Get the report summary information. It handles the date formatting.
        /// </summary>
        /// <param name="reports">Report data</param>
        /// <returns>IEnumerable<VibrationReportSummary></returns>
        public IEnumerable<VibrationReportSummary> GetReportSummary(IEnumerable<VibrationReport> reports)
        {
            return reports.Select(report => new VibrationReportSummary
                {
                    Id = report.Id,
                    CreationDate = report.Id.CreationTime.ToLocalTime().ToString("MMM dd, yyyy hh:mm:ss tt"),
                    NumberOfEvents = report.NumberOfEvents
                })
                .ToList();
        }

        /// <summary>
        /// Build the consolidated report from the provided raw consolidated data
        /// </summary>
        /// <param name="report">Raw consolidated data</param>
        /// <returns>VibrationConsolidatedReport</returns>
        public VibrationConsolidatedReport GetConsolidatedReport(VibrationReport report)
        {
            var consolidatedReport = new VibrationConsolidatedReport
            {
                Id = report.Id,
                XAxis = GetValueWithFrequency(report.X.ToArray()),
                YAxis = GetValueWithFrequency(report.Y.ToArray()),
                ZAxis = GetValueWithFrequency(report.Z.ToArray()),
                Vector = GetValueWithFrequency(report.Vector.ToArray()),
                GRMS = report.GRMS
               
            };

            return consolidatedReport;
        }

        /// <summary>
        /// Get key value pair for frecuency and accelaration. The starting point provided by Prof. Changfeng is as follow:
        /// 1 HZ ==> 0.000058 (g/Hz²)
        /// </summary>
        /// <param name="axisValues">Array of values</param>
        /// <returns>IEnumerable<ReportValue></returns>
        private static IEnumerable<ReportValue> GetValueWithFrequency(IReadOnlyList<double> axisValues)
        {
            var tempReportValues = new List<ReportValue>();
            var reportValues = new List<ReportValue>();

            var baseFrequency = 1600.0/axisValues.Count;
            var previousFrequency = 0.0;

            //First 300 values for testing
            for (var i = 0; i < 300; i++)
            {
                var currentFrecuency = baseFrequency * i;

                var currentValue = axisValues[i];
                tempReportValues.Add(new ReportValue
                {
                    Frequency = Math.Round(currentFrecuency, 4),
                    Value = Math.Round(currentValue, 8)
                });                
            }

            var orderedTempReportValues = tempReportValues.OrderByDescending(t => t.Value);
            var eventCounts = 0;

            double maxValue = double.MaxValue;

//            reportValues.Add(new ReportValue
//            {
//                Frequency = 1,
//                Value = 0.000058
//            });

            foreach (var reportValue in orderedTempReportValues)
            {
                var currentFrequency = reportValue.Frequency;
                if (currentFrequency <= previousFrequency || currentFrequency < 1)
                    continue;
                //Taking about 18 peaks values
//                if (eventCounts >= 20)
//                    break;

                if (reportValue.Value < maxValue)
                {
                    maxValue = reportValue.Value;
                }
                reportValues.Add(reportValue);
                previousFrequency = currentFrequency;
                eventCounts++;
            }

            return reportValues;
        }
    }
}