using System.Collections.Generic;
using PakTrack.Core.Base;
using PakTrack.DAL.Interfaces;
using Prism.Regions;
using PakTrack.Models;
using PakTrack.UI.Events;
using Prism.Commands;
using Prism.Events;
using System;
using System.IO;
using System.Linq;
using CsvHelper;
using LiteDB;
using Microsoft.Win32;
using OxyPlot;
using OxyPlot.Axes;
using PakTrack.Core;
using PakTrack.UI.Utilities;
using PakTrack.Utilities;

namespace PakTrack.UI.LightArea
{
    public class LightViewModel : PakTrackBindableBase, INavigationAware
    {
        private readonly ILightRepository _lightRepository;
        private readonly IEventAggregator _eventAggregator;
        private readonly IRegionManager _regionManager;

        private IEnumerable<Light> _lightData;
        public DelegateCommand LightToCsvCommand { get; private set; }

        public DelegateCommand<ObjectId> DeleteLightCommand { get; private set; }

        public DelegateCommand DeleteAllLisghtEventSCommand { get; private set; }
 
        private PlotModel _lightPlotModel;

        public PlotModel LightPlotModel
        {
            get { return _lightPlotModel; }
            set { SetProperty(ref _lightPlotModel, value); }
        }

        public IEnumerable<Light> LightData
        {
            get { return _lightData; }
            set { SetProperty(ref _lightData, value); }
        }

        public LightViewModel(ILightRepository lightRepository, IEventAggregator eventAggregator, IRegionManager regionManager)
        {
            _lightRepository = lightRepository;
            _eventAggregator = eventAggregator;
            _regionManager = regionManager;
            DeleteLightCommand = new DelegateCommand<ObjectId>(DeleteLight);
            DeleteAllLisghtEventSCommand = new DelegateCommand(DeleteEvents);
            LightToCsvCommand = new DelegateCommand(LightToCsv);
            eventAggregator.GetEvent<NavigationEvent>().Subscribe(OnTruckAndPackageChanged);
        }

        private async void DeleteEvents()
        {
            if (await _lightRepository.DeleteEvents(TruckId, PackageId))
            {
                _eventAggregator.GetEvent<RemoteNavigationEvent>().Publish(PakTrackConstant.MenuOptionDashboard);
            }

        }
        public void DeleteLight(ObjectId id)
        {
            _lightRepository.DeleteEvent(id);
            LightData = _lightRepository.GetByTruckAndPackageId(TruckId, PackageId);

        }
        private void LightToCsv()
        {
            var fileDialog = new SaveFileDialog
            {
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                Filter = "CSV files (*.csv)|*.csv|All files (*.*)|*.*",
                FileName = "LIGHT_DATA_" + TruckId + "_" + PackageId
            };
            if (fileDialog.ShowDialog() == true)
            {
                using (var writer = File.CreateText(fileDialog.FileName))
                {
                    var csvWritter = new CsvWriter(writer);
                    if (LightData.Any())
                    {
                        csvWritter.WriteField("Timestamp");
                        csvWritter.WriteField("R");
                        csvWritter.WriteField("G");
                        csvWritter.WriteField("B");
                        csvWritter.WriteField("Clear Light");
                        csvWritter.WriteField("Illuminance (Lux)");
                        csvWritter.NextRecord();

                        foreach (var light in LightData)
                        {
                            csvWritter.WriteField(DataConverters.TimeStampConverter(light.Timestamp));

                            csvWritter.WriteField(light.Value.R);
                            csvWritter.WriteField(light.Value.G);
                            csvWritter.WriteField(light.Value.B);

                            csvWritter.WriteField(light.Value.ClearLight);

                            csvWritter.WriteField(light.Value.Illuminance);
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

        public override void Initialize()
        {
            LightData = _lightRepository.GetByTruckAndPackageId(TruckId, PackageId);
            Title = "Light Data";
            PlotLightGraph();
        }

        private void PlotLightGraph()
        {
            LightPlotModel = new PlotModel
            {
                Title = "Plot of Datetime vs Clear Light [Lux]",
                TitleFontWeight = FontWeights.Bold,
                TitleFontSize = 14
            };

            //Series
            var humidityLineSeries = GraphAxis.GetSensorTypeLineSeries("Light");
            humidityLineSeries.Points
                .AddRange(LightData.Select(t => new DataPoint(DateTimeAxis.ToDouble(new DateTime(t.Timestamp).ToLocalTime()), t.Value.ClearLight)));

            var humidityMaxThresholdLineSeries = GraphAxis.GetThresholdLineSeries();
            var humidityMaxThreshold = _lightRepository.GetMaxThreshold(TruckId, PackageId, SensorMaxThreshold.Light);
            humidityMaxThresholdLineSeries.Points
                .AddRange(LightData.Select(t => new DataPoint(DateTimeAxis.ToDouble(new DateTime(t.Timestamp).ToLocalTime()), humidityMaxThreshold)));

            //Add series to plotmodel
            LightPlotModel.Series.Add(humidityLineSeries);
            LightPlotModel.Series.Add(humidityMaxThresholdLineSeries);

            LightPlotModel.Axes.Add(GraphAxis.GetDatetimeAxis());
            LightPlotModel.Axes.Add(GraphAxis.GetLinealAxisForY("Clear Light", "Lux"));
        }

        public override void OnNavigatedFrom(NavigationContext navigationContext)
        {
            LightPlotModel = null;
            LightData = null;
        }


    }
}
