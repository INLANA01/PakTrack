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

namespace PakTrack.UI.PressureArea
{
    public class PressureViewModel : PakTrackBindableBase, INavigationAware
    {
        private readonly IPressureRepository _pressureRepository;
        private readonly IEventAggregator _eventAggregator;

        private IEnumerable<PressureDTO> _pressureData;

        public DelegateCommand PressureToCsvCommand { get; private set; }

        public DelegateCommand DeleteAllEventsCommand { get; private set; }

        public DelegateCommand<ObjectId> DeletePressureCommand { get; private set; }
        public IEnumerable<PressureDTO> PressureData
        {
            get { return _pressureData; }
            set { SetProperty(ref _pressureData, value); }
        }

        private PlotModel _pressurePlotModel;

        public PlotModel PressurePlotModel
        {
            get { return _pressurePlotModel; }
            set { SetProperty(ref _pressurePlotModel, value); }
        }

        public PressureViewModel(IEventAggregator eventAggregator, IPressureRepository pressureRepository)
        {
            _pressureRepository = pressureRepository;
            _eventAggregator = eventAggregator;
            DeleteAllEventsCommand = new DelegateCommand(DeleteEvents);
            DeletePressureCommand = new DelegateCommand<ObjectId>(DeletePressure);
            PressureToCsvCommand = new DelegateCommand(PressureToCsv);
            eventAggregator.GetEvent<NavigationEvent>().Subscribe(OnTruckAndPackageChanged);
        }

        public override void Initialize()
        {
            PressureData = _pressureRepository.GetByTruckAndPackageId(TruckId, PackageId);
            Title = "Pressure Data";
            PlotPressureGraph();
        }

        private async void DeleteEvents()
        {
            if (await _pressureRepository.DeleteEvents(TruckId, PackageId))
            {
                _eventAggregator.GetEvent<RemoteNavigationEvent>().Publish(PakTrackConstant.MenuOptionDashboard);
            }

        }

        public override void OnNavigatedFrom(NavigationContext navigationContext)
        {
            PressureData = null;
            PressurePlotModel = null;
        }

        public void DeletePressure(ObjectId id)
        {
            _pressureRepository.DeleteEvent(id);
            PressureData = _pressureRepository.GetByTruckAndPackageId(TruckId, PackageId);
        }
        private void PlotPressureGraph()
        {
            PressurePlotModel = new PlotModel
            {
                Title = "Plot of Datetime vs Pressure",
                TitleFontWeight = FontWeights.Bold,
                TitleFontSize = 14
            };

            //Series
            var pressureLineSeries = GraphAxis.GetSensorTypeLineSeries("Pressure");
            pressureLineSeries.Points
                .AddRange(PressureData.Select(t => new DataPoint(DateTimeAxis.ToDouble(t.DateTime), t.Value)));

            //Pressure threshold line series
            var pressureThresholdLineSerie = GraphAxis.GetThresholdLineSeries();
            var pressureMaxThreshold = _pressureRepository.GetMaxThreshold(TruckId, PackageId, SensorMaxThreshold.Pressure);
            pressureMaxThreshold = pressureMaxThreshold / 1000;
            pressureThresholdLineSerie.Points
                .AddRange(PressureData.Select(t => new DataPoint(DateTimeAxis.ToDouble(t.DateTime), pressureMaxThreshold)));

            //Add series to plotmodel
            PressurePlotModel.Series.Add(pressureLineSeries);
            PressurePlotModel.Series.Add(pressureThresholdLineSerie);

            PressurePlotModel.Axes.Add(GraphAxis.GetDatetimeAxis());
            PressurePlotModel.Axes.Add(GraphAxis.GetLinealAxisForY("Pressure", "KPascal"));
        }

        private void PressureToCsv()
        {
            var fileDialog = new SaveFileDialog
            {
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                Filter = "CSV files (*.csv)|*.csv|All files (*.*)|*.*",
                FileName = "PRESSURE_DATA_" + TruckId + "_" + PackageId
            };
            if (fileDialog.ShowDialog() == true)
            {
                using (var writer = File.CreateText(fileDialog.FileName))
                {
                    var csvWritter = new CsvWriter(writer);
                    if (PressureData.Any())
                    {
                        csvWritter.WriteField("Timestamp");
                        csvWritter.WriteField("Pressure (KPa)");
                        csvWritter.NextRecord();

                        foreach (var pressureDto in PressureData)
                        {
                            csvWritter.WriteField(DataConverters.TimeStampConverter(pressureDto.Timestamp));
                            csvWritter.WriteField(pressureDto.Value);
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
