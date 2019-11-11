using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PakTrack.Core.Base;
using PakTrack.DAL.Interfaces;
using PakTrack.DTO;
using PakTrack.Models;
using PakTrack.UI.Events;
using PakTrack.Utilities;
using Prism.Events;
using Prism.Regions;

namespace PakTrack.UI.DashboardArea
{
    public class DashboardViewModel : PakTrackBindableBase, INavigationAware
    {
        private readonly IEventAggregator _eventAggregator;
        private readonly IHumidityRepository _humidityRepository;
        private readonly IVibrationRepository _vibrationRepository;
        private readonly IConfigurationRepository _configurationRepository;
        private readonly ITemperatureRepository _temperatureRepository;
        private readonly ILightRepository _lightRepository;
        private readonly IShockRepository _shock;
        private readonly IPressureRepository _pressureRepository;

        private long _vibrationEvents;
        public long VibrationEvents
        {
            get { return _vibrationEvents; }
            set { SetProperty(ref _vibrationEvents, value); }
        }

        public long TemperatureEvents
        {
            get { return _temperatureEvents; }
            set { SetProperty(ref _temperatureEvents, value); }
        }

        public long ShockEvents
        {
            get { return _shockEvents; }
            set { SetProperty(ref _shockEvents, value); }
        }

        public long PressureEvents
        {
            get { return _pressureEvents; }
            set { SetProperty(ref _pressureEvents, value); }
        }

        public long LightEvents
        {
            get { return _lightEvents; }
            set { SetProperty(ref _lightEvents, value); }
        }

        public long HumidityEvents
        {
            get { return _humidityEvents; }
            set { SetProperty(ref _humidityEvents, value); }
        }

        public string VibrationThreshold
        {
            get { return _vibrationThreshold; }
            set { SetProperty(ref _vibrationThreshold, value); }
        }

        public string TemperatureThreshold
        {
            get { return _temperatureThreshold; }
            set { SetProperty(ref _temperatureThreshold, value); }
        }

        public string HumidityThreshold
        {
            get { return _humidityThreshold; }
            set { SetProperty(ref _humidityThreshold, value); }
        }

        public string LightThreshold
        {
            get { return _lightThreshold; }
            set { SetProperty(ref _lightThreshold, value); }
        }

        public string PressureThreshold
        {
            get { return _pressureThreshold; }
            set { SetProperty(ref _pressureThreshold, value); }
        }

        public string ShockThreshold
        {
            get { return _shockThreshold; }
            set { SetProperty(ref _shockThreshold, value); }
        }

        public string VibrationStartTime
        {
            get { return _vibrationStartTime; }
            set { SetProperty(ref _vibrationStartTime, value); }
        }

        public string TemperatureStartTime
        {
            get { return _temperatureStartTime; }
            set { SetProperty(ref _temperatureStartTime, value); }
        }

        public string HumidityStartTime
        {
            get { return _humidityStartTime; }
            set { SetProperty(ref _humidityStartTime, value); }
        }

        public string PressureStartTime
        {
            get { return _pressureStartTime; }
            set { SetProperty(ref _pressureStartTime, value); }
        }

        public string LightStartTime
        {
            get { return _lightStartTime; }
            set { SetProperty(ref _lightStartTime, value); }
        }

        public string ShockStartTime
        {
            get { return _shockStartTime; }
            set { SetProperty(ref _shockStartTime, value); }
        }

        public string VibrationEndTime
        {
            get { return _vibrationEndTime; }
            set { SetProperty(ref _vibrationEndTime, value); }
        }

        public string TemperatureEndTime
        {
            get { return _temperatureEndTime; }
            set { SetProperty(ref _temperatureEndTime, value); }
        }

        public string ShockEndTime
        {
            get { return _shockEndTime; }
            set { SetProperty(ref _shockEndTime, value); }
        }

        public string PressureEndTime
        {
            get { return _pressureEndTime; }
            set { SetProperty(ref _pressureEndTime, value); }
        }

        public string LightEndTime
        {
            get { return _lightEndTime; }
            set { SetProperty(ref _lightEndTime, value); }
        }

        public string HumidityEndTime
        {
            get { return _humidityEndTime; }
            set { SetProperty(ref _humidityEndTime, value); }
        }

        public string DashboardHeader
        {
            get { return _dashboardHeader; }
            set { SetProperty(ref _dashboardHeader, value); }
        }

        private string _dashboardHeader;

        private long _temperatureEvents;
        private long _shockEvents;
        private long _pressureEvents;
        private long _lightEvents;
        private long _humidityEvents;

        private string _vibrationThreshold;
        private string _temperatureThreshold;
        private string _humidityThreshold;
        private string _lightThreshold;
        private string _pressureThreshold;
        private string _shockThreshold;

        private string _vibrationStartTime;
        private string _temperatureStartTime;
        private string _humidityStartTime;
        private string _lightStartTime;
        private string _pressureStartTime;
        private string _shockStartTime;

        private string _vibrationEndTime;
        private string _temperatureEndTime;
        private string _humidityEndTime;
        private string _lightEndTime;
        private string _pressureEndTime;
        private string _shockEndTime;

        public DashboardViewModel(IEventAggregator eventAggregator, IHumidityRepository humidityRepository,
            IVibrationRepository vibrationRepository, IConfigurationRepository configurationRepository,
            ITemperatureRepository temperatureRepository, ILightRepository lightRepository,
            IShockRepository shock,IPressureRepository pressureRepository)
        {
            _eventAggregator = eventAggregator;
            _humidityRepository = humidityRepository;
            _vibrationRepository = vibrationRepository;
            _configurationRepository = configurationRepository;
            _temperatureRepository = temperatureRepository;
            _lightRepository = lightRepository;
            _shock = shock;
            _pressureRepository = pressureRepository;
            eventAggregator.GetEvent<NavigationEvent>().Subscribe(OnTruckAndPackageChanged);

        }

        public override void Initialize()
        {
            DashboardHeader = "Dashboard Loading...";
            Task.Run(() => SetupDashboard()) ;
        }

        private void SetupDashboard()
        {
            var config = _configurationRepository.GetByTruckAndPackageId(TruckId, PackageId).FirstOrDefault();
            var vibration = _vibrationRepository.GetByTruckAndPackageId(TruckId, PackageId);
            var humidity = _humidityRepository.GetByTruckAndPackageId(TruckId, PackageId);
            var light = _lightRepository.GetByTruckAndPackageId(TruckId, PackageId);
            var pressure = _pressureRepository.GetByTruckAndPackageId(TruckId, PackageId);
            var shock = _shock.GetByTruckAndPackageId(TruckId, PackageId);
            var temperature = _temperatureRepository.GetByTruckAndPackageId(TruckId, PackageId);
            if (config != null)
            {

                var vibrations = vibration as IList<Vibration> ?? vibration.ToList();
                VibrationEvents = vibrations.Count;
                if (VibrationEvents > 0)
                {
                    VibrationStartTime = new DateTime(vibrations.First().Timestamp).ToString("g");
                    VibrationEndTime = new DateTime(vibrations.Last().Timestamp).ToString("g");
                }
                var temperatureDtos = temperature as IList<TemperatureDTO> ?? temperature.ToList();
                TemperatureEvents = temperatureDtos.Count;
                if (TemperatureEvents > 0)
                {
                    TemperatureStartTime = temperatureDtos.First().DateTime.ToString("g");
                    TemperatureEndTime = temperatureDtos.Last().DateTime.ToString("g");
                }
                var lights = light as IList<Light> ?? light.ToList();
                LightEvents = lights.Count;
                if (LightEvents > 0)
                {
                    LightStartTime = new DateTime(lights.First().Timestamp).ToString("g");
                    LightEndTime = new DateTime(lights.Last().Timestamp).ToString("g");
                }
                var pressureDtos = pressure as IList<PressureDTO> ?? pressure.ToList();
                PressureEvents = pressureDtos.Count;
                if (PressureEvents > 0)
                {
                    PressureStartTime = pressureDtos.First().DateTime.ToString("g");
                    PressureEndTime = pressureDtos.Last().DateTime.ToString("g");
                }

                var shocks = shock as IList<Shock> ?? shock.ToList();
                ShockEvents = shocks.Count;
                if (ShockEvents > 0)
                {
                    ShockStartTime = new DateTime(shocks.First().Timestamp).ToString("g");
                    ShockEndTime = new DateTime(shocks.Last().Timestamp).ToString("g");
                }

                var humidityDtos = humidity as IList<HumidityDTO> ?? humidity.ToList();
                HumidityEvents = humidityDtos.Count;
                if (HumidityEvents > 0)
                {
                    HumidityStartTime = humidityDtos.First().DateTime.ToString("g");
                    HumidityEndTime = humidityDtos.Last().DateTime.ToString("g");
                }
                foreach (var sensorConfiguration in config.Configs)
                {
                    TemperatureThreshold = Math.Round(sensorConfiguration.TemperatureMaxThreshold, 2) + " " +
                                           SensorConstants.DEFAULT_TEMPERATURE_UNIT;
                    HumidityThreshold = Math.Round(sensorConfiguration.HumidityMaxThreshold, 2) + " " +
                                        SensorConstants.DEFAULT_HUMIDITY_UNIT;
                    LightThreshold = Math.Round(sensorConfiguration.LightMaxThreshold, 2) + " " +
                                     SensorConstants.DEFAULT_LIGHT_UNIT;
                    VibrationThreshold = Math.Round(sensorConfiguration.VibrationMaxThreshold, 2) + " " +
                                         SensorConstants.DEFAULT_VIBRATION_UNIT;
                    PressureThreshold = Math.Round(sensorConfiguration.PressureMaxThreshold / 1000, 2) + " " +
                                        SensorConstants.DEFAULT_PRESSURE_UNIT;
                    ShockThreshold = Math.Round(sensorConfiguration.ShockMaxThreshold, 2) + " " +
                                     SensorConstants.DEFAULT_VIBRATION_UNIT;
                }
                DashboardHeader = "Dashboard";
            }
        }
    }
}