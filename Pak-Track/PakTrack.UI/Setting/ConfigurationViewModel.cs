using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using PakTrack.Core.Base;
using PakTrack.DAL.Interfaces;
using Prism.Events;
using Prism.Regions;
using PakTrack.Utilities;
using Prism.Commands;
using PakTrack.DAL.Interfaces.Sensor;
using PakTrack.UI.Events;
using PakTrack.UI.Utilities;

namespace PakTrack.UI.Setting
{
    public class ConfigurationViewModel : PakTrackBindableBase
    {
        private readonly IEventAggregator _eventAggregator;
        private readonly ISensorReaderContext _sensorReaderContext;
        private double _temperatureThreshold = SensorConstants.DefaultTemperatureThreshold;
        private double _humidityThreshold = SensorConstants.DefaultHumidityThrehold;
        private double _vibrationThreshold = SensorConstants.DefaultVibrationThreshold;
        private double _shockThreshold = SensorConstants.DefaultShockThreshold;
        private double _pressureThreshold = SensorConstants.DefaultPressureThrehold;
        private double _lightThreshold = SensorConstants.DefaultLightThreshold;

        private double _minTemperatureThreshold = 0;
        private double _minHumidityThreshold = 0;
        private double _minVibrationThreshold = 0;
        private double _minShockThreshold = 0;
        private double _minPressureThreshold = 0;
        private double _minLightThreshold = 0;

        private int _beforeTemperatureThresholdTimeMin = SensorConstants.DefaultBeforeTemperatureThresholdTimeMin;
        private int _beforeTemperatureThresholdTimeSec = SensorConstants.DefaultBeforeTemperatureThresholdTimeSec;
        private int _afterTemperatureThresholdTimeMin = SensorConstants.DefaultAfterTemperatureThresholdTimeMin;
        private int _afterTemperatureThresholdTimeSec = SensorConstants.DefaultAfterTemperatureThresholdTimeSec;


        private int _beforeHumidityThresholdTimeMin = SensorConstants.DefaultBeforeHumidityThresholdTimeMin;
        private int _beforeHumidityThresholdTimeSec = SensorConstants.DefaultBeforeHumidityThresholdTimeSec;
        private int _afterHumidityThresholdTimeMin = SensorConstants.DefaultAfterHumidityThresholdTimeMin;
        private int _afterHumidityThresholdTimeSec = SensorConstants.DefaultAfterHumidityThresholdTimeSec;

        private int _afterVibrationThresholdTimeMin = SensorConstants.DefaultAfterVibrationThresholdTimeMin;
        private int _afterVibrationThresholdTimeSec = SensorConstants.DefaultAfterVibrationThresholdTimeSec;
        private int _beforeVibrationThresholdTime = 32400;

        private int _beforePressureThresholdTimeMin = SensorConstants.DefaultBeforePressureThresholdTimeMin;
        private int _beforePressureThresholdTimeSec = SensorConstants.DefaultBeforePressureThresholdTimeSec;
        private int _afterPressureThresholdTimeMin = SensorConstants.DefaultAfterPressureThresholdTimeMin;
        private int _afterPressureThresholdTimeSec = SensorConstants.DefaultAfterPressureThresholdTimeSec;

        private int _beforeLightThresholdTimeMin = SensorConstants.DefaultBeforeLightThresholdTimeMin;
        private int _beforeLightThresholdTimeSec = SensorConstants.DefaultBeforeLightThresholdTimeSec;
        private int _afterLightThresholdTimeMin = SensorConstants.DefaultAfterLightThresholdTimeMin;
        private int _afterLightThresholdTimeSec = SensorConstants.DefaultAfterLightThresholdTimeSec;

        private int _beforeVibrationThresholdTimeMin = 5;
        private int _beforeVibrationThresholdTimeSec = 0;

        private bool _isCustomSchedule = false;

        private bool _isCelcious = false;

        
        private DateTime _startTime = DateTime.Now;
        private DateTime _endTime = DateTime.Now;

        private string _comments = string.Empty;
        private bool _canConfigure = true;

        public bool CanConfigure
        {
            get { return _canConfigure; }
            set
            {
                SetProperty(ref _canConfigure, value);
            }
        }

        public DelegateCommand ConfigureCommand { get; private set; }
        public DelegateCommand CancelConfigurationCommand { get; private set; }


        public double TemperatureThreshold
        {
            get { return _temperatureThreshold; }
            set { SetProperty(ref _temperatureThreshold, value); }
        }

        public double HumidityThreshold
        {
            get { return _humidityThreshold; }
            set { SetProperty(ref _humidityThreshold, value); }
        }

        public double VibrationThreshold
        {
            get { return _vibrationThreshold; }
            set { SetProperty(ref _vibrationThreshold ,value); }
        }

        public double ShockThreshold
        {
            get { return _shockThreshold; }
            set {SetProperty(ref _shockThreshold , value); }
        }

        public double PressureThreshold
        {
            get { return _pressureThreshold; }
            set { SetProperty(ref _pressureThreshold , value); }
        }

        public double LightThreshold
        {
            get { return _lightThreshold; }
            set { SetProperty(ref _lightThreshold , value); }
        }

        public int BeforeTemperatureThresholdTimeMin
        {
            get { return _beforeTemperatureThresholdTimeMin; }
            set { SetProperty( ref _beforeTemperatureThresholdTimeMin , value); }
        }

        public int BeforeTemperatureThresholdTimeSec
        {
            get { return _beforeTemperatureThresholdTimeSec; }
            set { SetProperty( ref _beforeTemperatureThresholdTimeSec, value); }
        }

        public int AfterTemperatureThresholdTimeMin
        {
            get { return _afterTemperatureThresholdTimeMin; }
            set { SetProperty( ref _afterTemperatureThresholdTimeMin, value); }
        }

        public int AfterTemperatureThresholdTimeSec
        {
            get { return _afterTemperatureThresholdTimeSec; }
            set { SetProperty(ref _afterTemperatureThresholdTimeSec , value); }
        }

        public int BeforeVibrationThresholdTimeMin
        {
            get { return _beforeVibrationThresholdTimeMin; }
            set { SetProperty(ref _beforeVibrationThresholdTimeMin, value); }
        }

        public int BeforeVibrationThresholdTimeSec
        {
            get { return _beforeVibrationThresholdTimeSec; }
            set { SetProperty(ref _beforeVibrationThresholdTimeSec, value); }
        }
        public int BeforeHumidityThresholdTimeMin
        {
            get { return _beforeHumidityThresholdTimeMin; }
            set { SetProperty(ref _beforeHumidityThresholdTimeMin, value); }
        }

        public int BeforeHumidityThresholdTimeSec
        {
            get { return _beforeHumidityThresholdTimeSec; }
            set { SetProperty(ref _beforeHumidityThresholdTimeSec, value); }
        }

        public int AfterHumidityThresholdTimeMin
        {
            get { return _afterHumidityThresholdTimeMin; }
            set { SetProperty(ref _afterHumidityThresholdTimeMin, value); }
        }

        public int AfterHumidityThresholdTimeSec
        {
            get { return _afterHumidityThresholdTimeSec; }
            set { SetProperty(ref _afterHumidityThresholdTimeSec , value); }
        }

        public int AfterVibrationThresholdTimeMin
        {
            get { return _afterVibrationThresholdTimeMin; }
            set { SetProperty(ref _afterVibrationThresholdTimeMin , value); }
        }

        public int AfterVibrationThresholdTimeSec
        {
            get { return _afterVibrationThresholdTimeSec; }
            set { SetProperty(ref _afterVibrationThresholdTimeSec, value); }
        }

        public int BeforePressureThresholdTimeMin
        {
            get { return _beforePressureThresholdTimeMin; }
            set { SetProperty(ref _beforePressureThresholdTimeMin , value); }
        }

        public int BeforePressureThresholdTimeSec
        {
            get { return _beforePressureThresholdTimeSec; }
            set { SetProperty(ref _beforePressureThresholdTimeSec , value); }
        }

        public int AfterPressureThresholdTimeMin
        {
            get { return _afterPressureThresholdTimeMin; }
            set { SetProperty(ref _afterPressureThresholdTimeMin , value); }
        }

        public int AfterPressureThresholdTimeSec
        {
            get { return _afterPressureThresholdTimeSec; }
            set { SetProperty(ref _afterPressureThresholdTimeSec , value); }
        }

        public int BeforeLightThresholdTimeMin
        {
            get { return _beforeLightThresholdTimeMin; }
            set { SetProperty( ref _beforeLightThresholdTimeMin , value); }
        }

        public int BeforeLightThresholdTimeSec
        {
            get { return _beforeLightThresholdTimeSec; }
            set { SetProperty( ref _beforeLightThresholdTimeSec , value); }
        }

        public int AfterLightThresholdTimeMin
        {
            get { return _afterLightThresholdTimeMin; }
            set { SetProperty(ref _afterLightThresholdTimeMin , value); }
        }

        public int AfterLightThresholdTimeSec
        {
            get { return _afterLightThresholdTimeSec; }
            set { SetProperty( ref _afterLightThresholdTimeSec , value); }
        }

        public bool IsCustomSchedule
        {
            get { return _isCustomSchedule; }
            set { SetProperty(ref _isCustomSchedule, value); }
        }

        public DateTime StartTime
        {
            get { return _startTime; }
            set { SetProperty(ref _startTime, value); }
        }

        public DateTime EndTime
        {
            get { return _endTime; }
            set { SetProperty(ref _endTime, value); }
        }

        public bool IsCelcious
        {
            get { return _isCelcious; }
            set { SetProperty(ref _isCelcious, value); }
        }

        public bool IsFarhenit
        {
            get { return !_isCelcious; }
            set { SetProperty(ref _isCelcious, !value); }
        }

        public string Comments
        {
            get { return _comments; }
            set { SetProperty(ref _comments, value); }
        }

        public ConfigurationViewModel(IRegionManager regionManager, IConfigurationRepository configurationRepository,
            IEventAggregator eventAggregator, ISensorReaderContext sensorReaderContext)
        {
            _eventAggregator = eventAggregator;
            _sensorReaderContext = sensorReaderContext;
            InitializeCommand();
        }

        private void InitializeCommand()
        {
            ConfigureCommand = new DelegateCommand(OnConfigureCommand).ObservesCanExecute(o=> CanConfigure);
            CancelConfigurationCommand = new DelegateCommand(OnCancelConfigurationCommand).ObservesCanExecute(o=> CanConfigure);
        }

        private void OnCancelConfigurationCommand()
        {
            _eventAggregator.GetEvent<RemoteNavigationEvent>().Publish(PakTrackConstant.MenuOptionSetting);
        }

        private async void OnConfigureCommand()
        {
            CanConfigure = false;
            Console.WriteLine("Configure Button Pressed");
            _eventAggregator.GetEvent<StatusEvent>().Publish(new StatusInformation
            {
                Action = "Sensor Configuration",
                Status = "Started",
                IsCompleted = false
            });
            Console.WriteLine("Config values reading");
            var configPack = CreateConfigPacket();
            Console.WriteLine("Config values reading complete");
            var notes = GetNote();
            Console.WriteLine("Config started ");
            var result = await Task.Run(() => _sensorReaderContext.Configure(configPack, Encoding.ASCII.GetBytes(notes)));
            CanConfigure = true;
            if (result)
            {
                _eventAggregator.GetEvent<RemoteNavigationEvent>().Publish(PakTrackConstant.MenuOptionSetting);
                _eventAggregator.GetEvent<StatusEvent>().Publish(new StatusInformation
                {
                    Action = "Sensor Configuration",
                    Status = "Configured Successfully",
                    IsCompleted = true
                });
            }
            else
            {
                _eventAggregator.GetEvent<StatusEvent>().Publish(new StatusInformation
                {
                    Action = "Sensor Configuration",
                    Status = "Configured Failded Plsease configure again",
                    IsCompleted = true
                });

            }
           

        }

        private byte[] CreateConfigPacket()
        {
            var configPacket = new List<byte>();
            configPacket.AddRange(GetGeneralConfig());
            configPacket.AddRange(GetTemperatureSettings());
            configPacket.AddRange(GetHumiditySettings());
            configPacket.AddRange(GetVibrationSetting());
            configPacket.AddRange(GetShockSettings());
            configPacket.AddRange(GetLightSettings());
            configPacket.AddRange(GetPressureSettings());
            return configPacket.ToArray();
        }

        private List<byte> GetGeneralConfig()
        {
            var generalSettings = new List<byte>(SensorConstants.GENERAL_SETTINGS_SIZE)
            {
                 SensorConstants.DEFAULT_RADIO_RESTART_DELAY & 0xff,
                 SensorConstants.DEFAULT_RADIO_MAX_RETRIES & 0xff,
                 SensorConstants.DEFAULT_RADIO_MAX_FAILURES & 0xff,
                 SensorConstants.DEFAULT_RADIO_PANID & 0xff,
                 SensorConstants.DEFAULT_RADIO_PANID >> 8,
                 SensorConstants.DEFAULT_CONFIG_TYPE & 0xff,
                 SensorConstants.DEFAULT_DATA_IN_FLASH & 0xff
            };
            return generalSettings;
        }

        private int GetTimeInSec(int min, int sec)
        {
            var value = sec;
            value += min * 60;
            return value;
        }

        private List<byte> GetTemperatureSettings()
        {
            //Time Period = Before time
            var size = IsCustomSchedule ? (SensorConstants.TEMPERATURE_SETTINGS_SIZE - 2) : SensorConstants.TEMPERATURE_SETTINGS_SIZE;
            var tempThresholdinF = IsCelcious ? UnitConversion.CelsiusToFahrenheit(TemperatureThreshold) : TemperatureThreshold;
            var temperatureSettings = new List<byte>(size)
            {
                (byte) (GetTimeInSec(BeforeTemperatureThresholdTimeMin, BeforeTemperatureThresholdTimeSec) & 0xff),
                (byte) (GetTimeInSec(BeforeTemperatureThresholdTimeMin, BeforeTemperatureThresholdTimeSec) >> 8),
                (byte) (GetTimeInSec(AfterTemperatureThresholdTimeMin, AfterTemperatureThresholdTimeSec) & 0xff),
                (byte) (GetTimeInSec(AfterTemperatureThresholdTimeMin, AfterTemperatureThresholdTimeSec) >> 8),
                (byte) (SensorUtils.GetTemperature(tempThresholdinF) & 0xff),
                (byte) (SensorUtils.GetTemperature(tempThresholdinF) >> 8),
                (byte) (SensorUtils.GetTemperature(_minTemperatureThreshold) & 0xff),
                (byte) (SensorUtils.GetTemperature(_minTemperatureThreshold) >> 8)
            };

            if (IsCustomSchedule) return temperatureSettings;
            temperatureSettings.Add(SensorConstants.DEFAULT_TEMP_OVER_THRESHOLD & 0xff);
            temperatureSettings.Add(SensorConstants.DEFAULT_TEMP_UNDER_THRESHOLD & 0xff);
            return temperatureSettings;
        }

        private List<byte> GetHumiditySettings()
        {
            //Time Period = Before time
            var size = IsCustomSchedule ? (SensorConstants.HUMIDITY_SETTINGS_SIZE - 2) : SensorConstants.HUMIDITY_SETTINGS_SIZE;
            var humiditySettings = new List<byte>(size)
            {
                (byte) (GetTimeInSec(BeforeHumidityThresholdTimeMin, BeforeHumidityThresholdTimeSec) & 0xff),
                (byte) (GetTimeInSec(BeforeHumidityThresholdTimeMin, BeforeHumidityThresholdTimeSec) >> 8),
                (byte) (GetTimeInSec(AfterHumidityThresholdTimeMin, AfterHumidityThresholdTimeSec) & 0xff),
                (byte) (GetTimeInSec(AfterHumidityThresholdTimeMin, AfterHumidityThresholdTimeSec) >> 8),
                (byte) (SensorUtils.GetHumidity(HumidityThreshold) & 0xff),
                (byte) (SensorUtils.GetHumidity(HumidityThreshold) >> 8),
                (byte) (SensorUtils.GetHumidity(_minHumidityThreshold) & 0xff),
                (byte) (SensorUtils.GetHumidity(_minHumidityThreshold) >> 8)
            };




            if (IsCustomSchedule) return humiditySettings;
            humiditySettings.Add(SensorConstants.DEFAULT_HUMD_OVER_THRESHOLD & 0xff);
            humiditySettings.Add(SensorConstants.DEFAULT_HUMD_UNDER_THRESHOLD & 0xff);
            return humiditySettings;
        }

        private List<byte> GetVibrationSetting()
        {
            //Time Period = Before time
            var size = IsCustomSchedule ? (SensorConstants.VIBRATION_SETTINGS_SIZE - 10) : SensorConstants.VIBRATION_SETTINGS_SIZE;
            var vibrationSettings = new List<byte>(size)
            {
                (byte) (_beforeVibrationThresholdTime & 0xff),
                (byte) (_beforeVibrationThresholdTime >> 8),
                (byte) (GetTimeInSec(AfterVibrationThresholdTimeMin, AfterVibrationThresholdTimeSec) & 0xff),
                (byte) (GetTimeInSec(AfterVibrationThresholdTimeMin, AfterVibrationThresholdTimeSec) >> 8),
                (byte) (SensorUtils.GetVibrationThreshold(VibrationThreshold) & 0xff),
                (byte) (SensorUtils.GetVibrationThreshold(VibrationThreshold) >> 8)
            };



            if (IsCustomSchedule) return vibrationSettings;
            vibrationSettings.Add(SensorConstants.DEFAULT_VIB_OVER_THRESHOLD & 0xff);
            vibrationSettings.Add(SensorConstants.DEFAULT_VIB_X_OFFSET & 0xff);
            vibrationSettings.Add(SensorConstants.DEFAULT_VIB_Y_OFFSET & 0xff);
            vibrationSettings.Add(SensorConstants.DEFAULT_VIB_Z_OFFSET & 0xff);
            vibrationSettings.Add(SensorConstants.DEFAULT_VIB_INACTIVITY_THRES & 0xff);
            vibrationSettings.Add(SensorConstants.DEFAULT_VIB_INACTIVITY_TIME & 0xff);
            vibrationSettings.Add(SensorConstants.DEFAULT_VIB_TAP_THRESHOLD & 0xff);
            vibrationSettings.Add(SensorConstants.DEFAULT_VIB_TAP_DURATION & 0xff);
            vibrationSettings.Add(SensorConstants.DEFAULT_VIB_TAP_LATENCY & 0xff);
            vibrationSettings.Add(SensorConstants.DEFAULT_VIB_TAP_WINDOW & 0xff);
            return vibrationSettings;
        }

        private List<byte> GetShockSettings()
        {
            //Time Period = Before time
            var size = IsCustomSchedule ? (SensorConstants.SHOCK_SETTINGS_SIZE - 3) : SensorConstants.SHOCK_SETTINGS_SIZE;
            var shockSettings = new List<byte>(size)
            {
                SensorConstants.DEFAULT_SHOCK_TIME & 0xff,
                SensorConstants.DEFAULT_SHOCK_TIME >> 8,
                SensorConstants.DEFAULT_SHOCK_OVER_THRESHOLD_TIME  & 0xff,
                SensorConstants.DEFAULT_SHOCK_OVER_THRESHOLD_TIME >> 8,
                (byte) (SensorUtils.GetShock(ShockThreshold) & 0xff),
                (byte) (SensorUtils.GetShock(ShockThreshold) >> 8)
            };

            if (IsCustomSchedule) return shockSettings;
            shockSettings.Add(SensorConstants.DEFAULT_SHOCK_FREE_FALL_THRESHOLD & 0xff);
            shockSettings.Add(SensorConstants.DEFAULT_SHOCK_FREE_FALL_TIME & 0xff);
            shockSettings.Add(SensorConstants.DEFAULT_SHOCK_ACTIVITY_THRESHOLD & 0xff);
            return shockSettings;
        }

        private List<byte> GetPressureSettings()
        {
            var size = IsCustomSchedule ? (SensorConstants.PRESSURE_SETTINGS_SIZE - 2) : SensorConstants.PRESSURE_SETTINGS_SIZE;
            var pressureSettings = new List<byte>(size)
            {
                (byte) (GetTimeInSec(BeforePressureThresholdTimeMin, BeforePressureThresholdTimeSec) & 0xff),
                (byte) (GetTimeInSec(BeforePressureThresholdTimeMin, BeforePressureThresholdTimeSec) >> 8),
                (byte) (GetTimeInSec(AfterPressureThresholdTimeMin, AfterPressureThresholdTimeSec) & 0xff),
                (byte) (GetTimeInSec(AfterPressureThresholdTimeMin, AfterPressureThresholdTimeSec) >> 8),
                (byte) (SensorUtils.GetPressure(PressureThreshold) & 0xff),
                (byte) (SensorUtils.GetPressure(PressureThreshold) >> 8),
                 SensorConstants.DEFAULT_PRESSURE_CHANGE & 0xff,
                 SensorConstants.DEFAULT_PRESSURE_CHANGE >> 8
            };

            if (IsCustomSchedule) return pressureSettings;
            pressureSettings.Add(SensorConstants.DEFAULT_PRESSURE_OVER_SAMPLE_RATE & 0xff);
            pressureSettings.Add(SensorConstants.DEFAULT_PRESSURE_OVER_THRESHOLD & 0xff);
            return pressureSettings;
        }

        private List<byte> GetLightSettings()
        {
            var size = IsCustomSchedule ? (SensorConstants.LIGHT_SETTINGS_SIZE - 4) : SensorConstants.LIGHT_SETTINGS_SIZE;
            var lightSettings = new List<byte>(size)
            {
                (byte) (GetTimeInSec(BeforeLightThresholdTimeMin, BeforeLightThresholdTimeSec) & 0xff),
                (byte) (GetTimeInSec(BeforeLightThresholdTimeMin, BeforeLightThresholdTimeSec) >> 8),
                (byte) (GetTimeInSec(AfterLightThresholdTimeMin, AfterLightThresholdTimeSec) & 0xff),
                (byte) (GetTimeInSec(AfterLightThresholdTimeMin, AfterLightThresholdTimeSec) >> 8),
                (byte) (SensorUtils.GetClearLight(LightThreshold) & 0xff),
                (byte) (SensorUtils.GetClearLight(LightThreshold) >> 8),
                (byte) (SensorUtils.GetClearLight(_minLightThreshold) & 0xff),
                (byte) (SensorUtils.GetClearLight(_minLightThreshold) >> 8)
            };

            if (IsCustomSchedule) return lightSettings;
            lightSettings.Add(SensorConstants.DEFAULT_LIGHT_WAIT_TIME & 0xff);
            lightSettings.Add(SensorConstants.DEFAULT_LIGHT_PRESISTENCE & 0xff);
            lightSettings.Add(SensorConstants.DEFAULT_LIGHT_OVER_THRESHOLD_INDEX & 0xff);
            lightSettings.Add(SensorConstants.DEFAULT_LIGHT_UNDER_THRESHOLD_INDEX & 0xff);
            return lightSettings;
        }

        private string GetNote()
        {
            return TruckId + "-" + PackageId + "-" + Comments;
        }
    }
}