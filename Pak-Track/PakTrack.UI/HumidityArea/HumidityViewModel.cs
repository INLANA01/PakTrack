using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using CsvHelper;
using LiteDB;
using Microsoft.Win32;
using OxyPlot;
using OxyPlot.Axes;
using PakTrack.Core;
using PakTrack.Core.Base;
using PakTrack.DAL.Interfaces;
using PakTrack.DTO;
using PakTrack.UI.Events;
using PakTrack.UI.Utilities;
using PakTrack.Utilities;
using Prism.Commands;
using Prism.Events;
using Prism.Regions;

namespace PakTrack.UI.HumidityArea
{
    public class HumidityViewModel : PakTrackBindableBase, INavigationAware
    {
        private readonly IHumidityRepository _humidityRepository;
        private readonly IEventAggregator _eventAggregator;
        public DelegateCommand HumidityToCsvCommand { get; private set; }

        public  DelegateCommand DeleteAllHumidityCommand { get; private set; }
        public DelegateCommand<ObjectId> DeleteHumidityCommand { get; private set; }

        private IEnumerable<HumidityDTO> _humidityData;

        public IEnumerable<HumidityDTO> HumidityData
        {
            get { return _humidityData; }
            set { SetProperty(ref _humidityData, value); }
        }

        private PlotModel _humidityPlotModel;

        public PlotModel HumidityPlotModel
        {
            get { return _humidityPlotModel; }
            set { SetProperty(ref _humidityPlotModel, value); }
        }

        public HumidityViewModel(IEventAggregator eventAggregator, IHumidityRepository humidityRepository)
        {
            _humidityRepository = humidityRepository;
            _eventAggregator = eventAggregator;
            DeleteHumidityCommand = new DelegateCommand<ObjectId>(DeleteHumidity);
            HumidityToCsvCommand = new DelegateCommand(HumidityToCsv);
            DeleteAllHumidityCommand = new DelegateCommand(DeleteEvents);
            eventAggregator.GetEvent<NavigationEvent>().Subscribe(OnTruckAndPackageChanged);
        }

        public override void Initialize()
        {
            HumidityData = _humidityRepository.GetByTruckAndPackageId(TruckId, PackageId);
            Title = "Humidity Data";
            PlotHumidityGraph();
        }

        private async void DeleteEvents()
        {
            if (await _humidityRepository.DeleteEvents(TruckId, PackageId))
            {
                _eventAggregator.GetEvent<RemoteNavigationEvent>().Publish(PakTrackConstant.MenuOptionDashboard);
            }

        }

        private void PlotHumidityGraph()
        {
            HumidityPlotModel = new PlotModel
            {
                Title = "Plot of Datetime vs Humidity",
                TitleFontWeight = FontWeights.Bold,
                TitleFontSize = 14
            };

            //Series
            var humidityLineSeries = GraphAxis.GetSensorTypeLineSeries("Humidity");
            humidityLineSeries.Points
                .AddRange(HumidityData.Select( t=> new DataPoint(DateTimeAxis.ToDouble(t.DateTime.ToLocalTime()), t.Value)));

            var humidityMaxThresholdLineSeries = GraphAxis.GetThresholdLineSeries();
            var humidityMaxThreshold = _humidityRepository.GetMaxThreshold(TruckId, PackageId, SensorMaxThreshold.Humidity);
            humidityMaxThresholdLineSeries.Points
                .AddRange(HumidityData.Select(t => new DataPoint(DateTimeAxis.ToDouble(t.DateTime.ToLocalTime()), humidityMaxThreshold)));

            //Add series to plotmodel
            HumidityPlotModel.Series.Add(humidityLineSeries);
            HumidityPlotModel.Series.Add(humidityMaxThresholdLineSeries);

            HumidityPlotModel.Axes.Add(GraphAxis.GetDatetimeAxis());
            HumidityPlotModel.Axes.Add(GraphAxis.GetLinealAxisForY("Humidity", "RH%"));
        }

        public override void OnNavigatedFrom(NavigationContext navigationContext)
        {
            HumidityPlotModel = null;
            HumidityData = null;
        }

        public void DeleteHumidity(ObjectId id)
        {
            _humidityRepository.DeleteEvent(id);
            HumidityData = _humidityRepository.GetByTruckAndPackageId(TruckId, PackageId);
        }

        private void HumidityToCsv()
        {
            var fileDialog = new SaveFileDialog
            {
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                Filter = "CSV files (*.csv)|*.csv|All files (*.*)|*.*",
                FileName = "HUMIDITY_DATA_" + TruckId + "_" + PackageId
            };
            if (fileDialog.ShowDialog() == true)
            {
                using (var writer = File.CreateText(fileDialog.FileName))
                {
                    var csvWritter = new CsvWriter(writer);
                    if (HumidityData.Any())
                    {
                        csvWritter.WriteField("Timestamp");
                        csvWritter.WriteField("Humidity (RH %)");
                        csvWritter.NextRecord();

                        foreach (var humidityDto in HumidityData)
                        {
                            csvWritter.WriteField(DataConverters.TimeStampConverter(humidityDto.Timestamp));
                            csvWritter.WriteField(humidityDto.Value);
                            csvWritter.NextRecord();
                        }
                    }
                    else
                    {
                        csvWritter.WriteField("No Data");
                    }
                }
            }
        }
    }
}
