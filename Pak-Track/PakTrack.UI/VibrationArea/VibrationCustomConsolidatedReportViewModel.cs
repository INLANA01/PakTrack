using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PakTrack.Core.Base;
using PakTrack.DAL.Interfaces;
using PakTrack.Models;
using PakTrack.UI.Events;
using PakTrack.UI.Utilities;
using PakTrack.Utilities;
using Prism.Commands;
using Prism.Events;
using Prism.Regions;

namespace PakTrack.UI.VibrationArea
{
    class VibrationCustomConsolidatedReportViewModel : PakTrackBindableBase, INavigationAware
    {
        private IVibrationRepository VibrationRepository { get;  set; }
        private readonly IEventAggregator _eventAggregator;
        private readonly IVibrationConsolidatedReport _consolidatedReport;

        private bool _isGValue = true;

        public bool IsGValue
        {
            get { return _isGValue; }
            set
            {
                SetProperty(ref _isGValue, value);
                OnPropertyChanged("IsGValue");
            }
        }

        public bool IsTimeStamp
        {
            get { return !_isGValue; }
            set
            {
                SetProperty(ref _isGValue, !value);
                OnPropertyChanged("IsTimeStamp");
            }
        }


        private string _minXGValue;

        public string MinXGValue
        {
            get { return _minXGValue; }
            set { SetProperty(ref _minXGValue, value); }
        }

        private string _maxXGValue;

        public string MaxXGValue
        {
            get { return _maxXGValue; }
            set { SetProperty(ref _maxXGValue, value); }
        }

        private string _minYGValue;

        public string MinYGValue
        {
            get { return _minYGValue; }
            set { SetProperty(ref _minYGValue, value); }
        }

        private string _maxYGValue;

        public string MaxYGValue
        {
            get { return _maxYGValue; }
            set { SetProperty(ref _maxYGValue, value); }
        }

        private string _minZGValue;

        public string MinZGValue
        {
            get { return _minZGValue; }
            set { SetProperty(ref _minZGValue, value); }
        }

        private string _maxZGValue;

        public string MaxZGValue
        {
            get { return _maxZGValue; }
            set { SetProperty(ref _maxZGValue, value); }
        }


        private int _startDay;

        public int StartDay
        {
            get { return _startDay; }
            set { SetProperty(ref _startDay, value); }
        }

        private int _startMonth;

        public int StartMonth
        {
            get { return _startMonth; }
            set { SetProperty(ref _startMonth, value); }
        }

        private int _startYear;

        public int StartYear
        {
            get { return _startYear; }
            set { SetProperty(ref _startYear, value); }
        }

        private int _startMin;

        public int StartMin
        {
            get { return _startMin; }
            set { SetProperty(ref _startMin, value); }
        }

        private int _startHour;

        public int StartHour
        {
            get { return _startHour; }
            set { SetProperty(ref _startHour, value); }
        }


        private int _endDay;

        public int EndDay
        {
            get { return _endDay; }
            set { SetProperty(ref _endDay, value); }
        }

        private int _endMonth;

        public int EndMonth
        {
            get { return _endMonth; }
            set { SetProperty(ref _endMonth, value); }
        }

        private int _endYear;

        public int EndYear
        {
            get { return _endYear; }
            set { SetProperty(ref _endYear, value); }
        }

        private int _endMin;

        public int EndMin
        {
            get { return _endMin; }
            set { SetProperty(ref _endMin, value); }
        }

        private int _endHour;

        public int EndHour
        {
            get { return _endHour; }
            set { SetProperty(ref _endHour, value); }
        }

        private IEnumerable<Vibration> VibrationData { get; set; }

        public DelegateCommand CommandCancel { get; private set; }
        public DelegateCommand GenerateReportCommand { get; private set; }

        public DelegateCommand ApplyCustomFilterCommand { get; private set; }


        public VibrationCustomConsolidatedReportViewModel(IRegionManager regionManager,
            IEventAggregator eventAggregator, IVibrationRepository vibrationRepository,
            IVibrationConsolidatedReport consolidatedReport)
        {
            VibrationRepository = vibrationRepository;
            _eventAggregator = eventAggregator;
            _consolidatedReport = consolidatedReport;
            GenerateReportCommand = new DelegateCommand(OnGenerateReportCommand);
            CommandCancel = new DelegateCommand(OnCancelCommand);
            ApplyCustomFilterCommand = new DelegateCommand(OnFilterApply);
        }

        private void SetDefaultG()
        {
            MinXGValue = "0.0";
            MaxXGValue = "4.0";
            MinYGValue = "0.0";
            MaxYGValue = "4.0";
            MinZGValue = "0.0";
            MaxZGValue = "4.0";
        }
      

        private void SetTimeDefaults()
        {
            var today = DateTime.Now;
            StartDay = today.Day;
            EndDay = today.Day;

            StartMonth = today.Month;
            EndMonth = today.Month;

            StartYear = today.Year;
            EndYear = today.Year;

            StartMin = today.Minute;
            EndMin = today.Minute;

            StartHour = today.Hour;
            EndHour = today.Hour;
        }

        private void OnCancelCommand()
        {
            _eventAggregator.GetEvent<RemoteNavigationEvent>()
                .Publish(PakTrackConstant.MenuOptionVibration);
        }


        private void OnFilterApply()
        {
            var filterInfo = new FilterInfo
            {
                X = new AxisInfo
                {
                    MinValue = double.Parse(MinXGValue),
                    MaxValue = double.Parse(MaxXGValue)
                },
                Y = new AxisInfo
                {
                    MinValue = double.Parse(MinYGValue),
                    MaxValue = double.Parse(MaxYGValue)
                },
                Z = new AxisInfo
                {
                    MinValue = double.Parse(MinZGValue),
                    MaxValue = double.Parse(MaxZGValue)
                },
                Start = new DateTime(StartYear, StartMonth, StartDay, StartHour, StartMin, 0),
                End = new DateTime(EndYear, EndMonth, EndDay, EndHour, EndMin, 0),
                IsTimeStamp = !IsGValue
            };
            _eventAggregator.GetEvent<RemoteNavigationEvent>()
                .Publish(PakTrackConstant.MenuOptionVibration);
            _eventAggregator.GetEvent<CustomReportFilterEvent>().Publish(filterInfo);
        }
        private void OnGenerateReportCommand()
        {
            IEnumerable<Vibration> customVibEvents;
            if (IsGValue)
            {
                var maxXGval = double.Parse(MaxXGValue);
                var maxYGval = double.Parse(MaxYGValue);
                var maxZGval = double.Parse(MaxZGValue);

                var minXGval = double.Parse(MinXGValue);
                var minYGval = double.Parse(MinYGValue);
                var minZGval = double.Parse(MinZGValue);
                customVibEvents = VibrationData.Where(x =>
                    x.MaximumX.Value >= minXGval && x.MaximumX.Value <= maxXGval ||
                    x.MaximumY.Value >= minYGval && x.MaximumY.Value <= maxYGval ||
                    x.MaximumZ.Value >= minZGval && x.MaximumZ.Value <= maxZGval);
            }
            else
            {
                var startTime = new DateTime(StartYear, StartMonth, StartDay, StartHour, StartMin, 0);
                var endTime = new DateTime(EndYear, EndMonth, EndDay, EndHour, EndMin, 0);
                customVibEvents =
                    VibrationData.Where(vib => vib.Timestamp > startTime.Ticks && vib.Timestamp < endTime.Ticks);
            }
            _eventAggregator.GetEvent<StatusEvent>().Publish(new StatusInformation
            {
                Action = "Report Generator",
                Status = "Report Generation Started",
                IsCompleted = true
            });
            Task.Run(() => GenerateReportTask(customVibEvents));
        }

        private void GenerateReportTask(IEnumerable<Vibration> data)
        {
            var sensorData = data as IList<Vibration> ?? data.ToList();
            if (sensorData.Any())
            {
                _consolidatedReport.PushToDb(_consolidatedReport.GenerateReport(sensorData, true));

                _eventAggregator.GetEvent<StatusEvent>().Publish(new StatusInformation
                {
                    Action = "Report Generator",
                    Status = "Report Generation Complete",
                    IsCompleted = true
                });
                OnCancelCommand();
            }
            else
            {
                _eventAggregator.GetEvent<StatusEvent>().Publish(new StatusInformation
                {
                    Action = "Report Generator",
                    Status = "No Vibration Data to Generate Report",
                    IsCompleted = true
                });
            }
        }

        public override void Initialize()
        {
            VibrationData = VibrationRepository.GetByTruckAndPackageId(TruckId, PackageId);
            SetTimeDefaults();
            SetDefaultG();
        }
    }
}