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

namespace PakTrack.UI.TemperatureArea
{
    public class TemperatureViewModel : PakTrackBindableBase, INavigationAware
    {
        private readonly IRegionManager _regionManager;
        private readonly ITemperatureRepository _temperatureRepository;
        private readonly IEventAggregator _eventAggregator;



        private IEnumerable<TemperatureDTO> _temperatureData;

        public IEnumerable<TemperatureDTO> TemperatureData
        {
            get { return _temperatureData; }
            set { SetProperty(ref _temperatureData, value); }
        }

        private string _unit;

        public DelegateCommand TemperatureToCsvCommand { get; private set; }
        public DelegateCommand DeleteAllEventsCommand { get; private set; }


        public DelegateCommand<ObjectId> DeleteTemperatureCommand { get; private set; }


        public string Unit
        {
            get { return _unit; }
            set { SetProperty(ref _unit, value); }
        }

        private PlotModel _temperaturePlotModel;

        public PlotModel TemperaturePlotModel
        {
            get { return _temperaturePlotModel; }
            set { SetProperty(ref _temperaturePlotModel, value); }
        }


        public TemperatureViewModel(IEventAggregator eventAggregator, IRegionManager regionManager, ITemperatureRepository temperatureRepository)
        {
            _eventAggregator = eventAggregator;
            _regionManager = regionManager;
            _temperatureRepository = temperatureRepository;
            TemperatureToCsvCommand = new DelegateCommand(TemperatureToCsv);
            DeleteAllEventsCommand = new DelegateCommand(DeleteEvents);
            DeleteTemperatureCommand = new DelegateCommand<ObjectId>(DeleteTemperatureEvent);
            eventAggregator.GetEvent<NavigationEvent>().Subscribe(OnTruckAndPackageChanged);
        }

        private void DeleteTemperatureEvent(ObjectId eventId)
        {
            _temperatureRepository.DeleteEvent(eventId);
            TemperatureData = _temperatureRepository.GetByTruckAndPackageId(TruckId, PackageId);
        }

        private async void DeleteEvents()
        {
            if (await _temperatureRepository.DeleteEvents(TruckId, PackageId))
            {
                _eventAggregator.GetEvent<RemoteNavigationEvent>().Publish(PakTrackConstant.MenuOptionDashboard);
            }

        }

        public override void Initialize()
        {
            Title = "Temperature Data (Fahrenheit)";
            TemperatureData = _temperatureRepository.GetByTruckAndPackageId(TruckId, PackageId);
            PlotTemperatureGraph();
        }


        public override void OnNavigatedFrom(NavigationContext navigationContext)
        {
            TemperatureData = null;
            TemperaturePlotModel = null;
           
        }

        private void PlotTemperatureGraph()
        {
            TemperaturePlotModel = new PlotModel
            {
                Title = "Plot of Datetiime vs Temperature",
                TitleFontWeight = FontWeights.Bold,
                TitleFontSize = 14
            };

            //Temperature line series
            var temperatureLineSeries = GraphAxis.GetSensorTypeLineSeries("Temperature");
            temperatureLineSeries.Points
                .AddRange(TemperatureData.Select(t=> new DataPoint(DateTimeAxis.ToDouble(t.DateTime), t.Value)));

            //Temperature threshold line series
            var temperatureThresholdLineSerie = GraphAxis.GetThresholdLineSeries();
            var temperatureMaxThreshold = _temperatureRepository.GetMaxThreshold(TruckId, PackageId, SensorMaxThreshold.Temperature);
            temperatureThresholdLineSerie.Points
                .AddRange(TemperatureData.Select(t=> new DataPoint(DateTimeAxis.ToDouble(t.DateTime), temperatureMaxThreshold)));

            //Temperature threshold line series
            TemperaturePlotModel.Series.Add(temperatureLineSeries);
            TemperaturePlotModel.Series.Add(temperatureThresholdLineSerie);
            
            //Append the axis scales
            TemperaturePlotModel.Axes.Add(GraphAxis.GetDatetimeAxis());
            TemperaturePlotModel.Axes.Add(GraphAxis.GetLinealAxisForY("Temperature", "Fahrenheit"));

        }

        private void TemperatureToCsv()
        {
            var fileDialog = new SaveFileDialog
            {
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                Filter = "CSV files (*.csv)|*.csv|All files (*.*)|*.*",
                FileName = "TEMPERATURE_DATA_"+TruckId + "_" + PackageId 
            };
            if (fileDialog.ShowDialog() == true)
            {
                using (var writer = File.CreateText(fileDialog.FileName))
                {
                    var csvWritter = new CsvWriter(writer);
                    if (TemperatureData.Any())
                    {
                        csvWritter.WriteField("Timestamp");
                        csvWritter.WriteField("Temperature (F)");
                        csvWritter.NextRecord();

                        foreach (var temperature in TemperatureData)
                        {
                            csvWritter.WriteField(DataConverters.TimeStampConverter(temperature.Timestamp));
                            csvWritter.WriteField(temperature.FahrenheitValue);
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
