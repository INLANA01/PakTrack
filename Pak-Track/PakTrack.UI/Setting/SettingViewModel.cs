using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;
using PakTrack.Core;
using PakTrack.Core.Base;
using PakTrack.DAL.Interfaces;
using PakTrack.DAL.Interfaces.Sensor;
using PakTrack.Models;
using PakTrack.Models.Sensor;
using PakTrack.UI.Events;
using PakTrack.Utilities;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using Prism.Regions;
using PakTrack.UI.Utilities;

namespace PakTrack.UI.Setting
{
    public class SettingViewModel : PakTrackBindableBase
    {
        private readonly IRegionManager _regionManager;
        private readonly IEventAggregator _eventAggregator;
        private readonly ISensorReaderContext _sensorReaderContext;
        private readonly IPressureSensorRepository _pressureSensorRepository;
        private readonly ILightSensorRepository _lightSensorRepository;
        private readonly ILightRepository _lightRepository;
        private readonly ITemperatureRepository _temperatureRepository;
        private readonly IHumidityRepository _humidityRepository;
        private readonly IPressureRepository _pressureRepository;
        private readonly IConfigurationRepository _configurationRepository;
        private readonly IVibrationRepository _vibrationRepository;
        private readonly IShockRepository _shockRepository;
        private readonly IVibrationConsolidatedReport _vibrationConsolidatedReport;
        private readonly IShockSensorRepository _shockSensorRepository;
        private readonly IFlashDataSensorRepository _flashDataSensorRepository;
        private readonly IVibrationSensorRepository _vibrationSensorRepository;
        private readonly IConfigurationSensorRepository _configurationSensorRepository;
        private readonly ITemperatureSensorRepository _temperatureSensorRepository;
        private readonly IHumiditySensorRepository _humiditySensorRepository;
        private bool _canRead;
        private bool _canExecuteConnectDisconnectCommand = true;
        private string _connectionStatus = "Not Connected";
        private string _readStatus = "NA";
        private string _connectDisconnectTitle = "Connect";
        private bool _isSensorConnected = false;
        private Timer statusTimer;
//        private Timer BatteryStatusTimer;

        public SettingViewModel(IRegionManager regionManager, IEventAggregator eventAggregator,
            ISensorReaderContext sensorReaderContext,
            IConfigurationSensorRepository configurationSensorRepository,
            ITemperatureSensorRepository temperatureSensorRepository,
            IHumiditySensorRepository humiditySensorRepository,
            IPressureSensorRepository pressureSensorRepository,
            ILightSensorRepository lightSensorRepository, ILightRepository lightRepository,
            ITemperatureRepository temperatureRepository,
            IHumidityRepository humidityRepository,
            IPressureRepository pressureRepository,
            IConfigurationRepository configurationRepository,
            IVibrationRepository vibrationRepository,
            IShockRepository shockRepository,
            IVibrationConsolidatedReport vibrationConsolidatedReport,
            IShockSensorRepository shockSensorRepository,
            IFlashDataSensorRepository flashDataSensorRepository,
            IVibrationSensorRepository vibrationSensorRepository)
        {
            _regionManager = regionManager;
            _eventAggregator = eventAggregator;
            _sensorReaderContext = sensorReaderContext;
            _pressureSensorRepository = pressureSensorRepository;
            _lightSensorRepository = lightSensorRepository;
            _lightRepository = lightRepository;
            _temperatureRepository = temperatureRepository;
            _humidityRepository = humidityRepository;
            _pressureRepository = pressureRepository;
            _configurationRepository = configurationRepository;
            _vibrationRepository = vibrationRepository;
            _shockRepository = shockRepository;
            _vibrationConsolidatedReport = vibrationConsolidatedReport;
            _shockSensorRepository = shockSensorRepository;
            _flashDataSensorRepository = flashDataSensorRepository;
            _vibrationSensorRepository = vibrationSensorRepository;
            _configurationSensorRepository = configurationSensorRepository;
            _temperatureSensorRepository = temperatureSensorRepository;
            _humiditySensorRepository = humiditySensorRepository;
            InitializeCommand();
            statusTimer = new Timer();
            statusTimer.Elapsed += StatusUpdate;
            statusTimer.Interval = 5000;
      

        }
        public DelegateCommand SensorConnectDisconnectCommand { get; private set; }
        public DelegateCommand SensorReadCommand { get; private set; }

        public DelegateCommand SensorConfigurationCommand { get; private set; }

        public override void Initialize()
        {
            if (_isSensorConnected)
            {
                BatteryStatusUpdate();
            }
        }

        private void InitializeCommand()
        {
            SensorConnectDisconnectCommand = new DelegateCommand(OnConnectDisconnect).ObservesCanExecute(o => CanExecuteConnectDisconnectCommand);
            SensorReadCommand = new DelegateCommand(OnSensorReadCommand).ObservesCanExecute(o => CanRead);
            SensorConfigurationCommand = new DelegateCommand(OnSensorConfigurationCommand).ObservesCanExecute(o => CanRead);
        }

        private void OnSensorConfigurationCommand()
        {
           
            _eventAggregator.GetEvent<RemoteNavigationEvent>().Publish(PakTrackConstant.MenuOptionConfiguration);
        }
        public bool CanRead
        {
            get { return _canRead; }
            set { SetProperty(ref _canRead, value); }
        }


        public bool CanExecuteConnectDisconnectCommand
        {
            get { return _canExecuteConnectDisconnectCommand; }
            set { SetProperty(ref _canExecuteConnectDisconnectCommand, value); }
        }

        public string ConnectionStatus
        {
            get { return _connectionStatus; }
            set { SetProperty(ref _connectionStatus, value); }
        }
        public string ReadStatus
        {
            get { return _readStatus; }
            set { SetProperty(ref _readStatus, value); }
        }

        public string ConnectTileTitle
        {
            get { return _connectDisconnectTitle; }
            set { SetProperty(ref _connectDisconnectTitle, value); }
        }


        private void OnConnectDisconnect()
        {
            if (_isSensorConnected)
            {
                _sensorReaderContext.Disconneect();
                ConnectionStatus = "Disconnected";
                CanRead = false;
                TruckId = string.Empty;
                PackageId = string.Empty;
                _isSensorConnected = false;
                ConnectTileTitle = "Connect";
            }
            else
            {
                try
                 {
                    var port = SensorUtils.GetCorrectPort();
                    _sensorReaderContext.Connect(port);
                }
                catch (Exception ex)
                {
                    ConnectionStatus = "Check the Sensor and Try again!!";
                    return;
                }

                ConnectionStatus = "Connected";
                CanRead = true;
                BatteryStatusUpdate();
                TruckId = _sensorReaderContext.TruckId();
                PackageId = _sensorReaderContext.PackageId();
                IsFlashDataAvailable = _sensorReaderContext.IsFlashDataAvailable();
                _isSensorConnected = true;
                ConnectTileTitle = "Disconnect";
            }
            CanExecuteConnectDisconnectCommand = true;
        }

        private async void OnSensorReadCommand()
        {
            CanExecuteConnectDisconnectCommand = false;
            CanRead = false;
            var shockThresh = ReadConfiguration();
            try
            {
                statusTimer.Start();

                var vibrationDone = await ReadVibration();
                var shockDone = await ReadShock(shockThresh);
                var temperatureDone = await ReadTemerature();
                var humidityDone = await ReadHumidity();
                var lightDone = await ReadLight();
                var pressureDone = await ReadPressure();
                var flashDone = await ReadFlash();

                statusTimer.Stop();
                BatteryStatusUpdate();
                if (  vibrationDone && temperatureDone && shockDone && humidityDone && lightDone && pressureDone && flashDone)
                {
                    ReadStatus = "Read Complete";
                    var navigationInfo = new NavigationInformation
                    {
                        TruckId = this.TruckId,
                        PackageId = this.PackageId
                    };
                    _eventAggregator.GetEvent<TruckAddEvent>().Publish(navigationInfo);

                    _eventAggregator.GetEvent<StatusEvent>().Publish(new StatusInformation
                    {
                        Action = ReadStatus,
                        Status = "Done",
                        IsCompleted = true
                    });
                }
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine("Something went wrong" + ex.StackTrace);
                ReadStatus = "Something Went Wrong Please Read the data again";
            }
            CanExecuteConnectDisconnectCommand = true;
            CanRead = true;
        }

        private async Task<bool> ReadFlash()
        {
            if (_sensorReaderContext.IsFlashDataAvailable())
            {
                ReadStatus = "Reading Flash";
                var packets = await _flashDataSensorRepository.ReadSensorData();
                ReadStatus = "Flash Read Complete. Sorting Packets";
                var packetList = packets as IList<Packet> ?? packets.ToList();
                ManageTempPackets(packetList.Where(x => x.isTemperature()));
                ManageVibrationPackets(packetList.Where(p => p.isVibration()));
                ManageHmidityPackets(packetList.Where(p => p.isHumidty()));
                ManagePressurePackets(packetList.Where(p => p.isPressure()));
                ManageLightPackets(packetList.Where(p => p.isLight()));
                ManageShockPackets(packetList.Where(p => p.isShock()));
            }
            return true;
        }

        private async void ManageTempPackets(IEnumerable<Packet> packets)
        {
            var tempData = _temperatureSensorRepository.ExtractTemperatureData(packets);
            await _temperatureRepository.PushToDb(tempData, true);
        }

        private async void ManageShockPackets(IEnumerable<Packet> packets)
        {
            var shockData = _shockSensorRepository.ExtractShockData(packets);
            await _shockRepository.PushToDb(shockData, true);
        }

        private async void ManageLightPackets(IEnumerable<Packet> packets)
        {
            var lightData = _lightSensorRepository.ExtractLightData(packets);
            await _lightRepository.PushToDb(lightData, true);
        }

        private async void ManageVibrationPackets(IEnumerable<Packet> packets)
        {
            var vibrationData = _vibrationSensorRepository.ExtractVibrationData(packets);
            await _vibrationRepository.PushToDb(vibrationData, true);
        }

        private async void ManageHmidityPackets(IEnumerable<Packet> packets)
        {
            var humidityData = _humiditySensorRepository.ExtractHumiditieData(packets);
            await _humidityRepository.PushToDb(humidityData, true);
        }

        private async void ManagePressurePackets(IEnumerable<Packet> packets)
        {
            var pressureData = _pressureSensorRepository.ExtractPressurekData(packets);
            await _pressureRepository.PushToDb(pressureData, true);
        }
        private async Task<bool> ReadTemerature()
        {
            ReadStatus = "Reading Temperature";
            var temperatureData = await _temperatureSensorRepository.ReadSensorData();
            var sensorData = temperatureData as IList<Temperature> ?? temperatureData.ToList();
            await _temperatureRepository.PushToDb(sensorData);
            return true;
        }

        private async Task<bool> ReadHumidity()
        {
            ReadStatus = "Reading Humidity";
            var humidityData = await _humiditySensorRepository.ReadSensorData();
            var sensorData = humidityData as IList<Humidity> ?? humidityData.ToList();
            await _humidityRepository.PushToDb(sensorData);
            return true;
        }

        private async Task<bool> ReadLight()
        {
            ReadStatus = "Reading Light";
            var lightData = await _lightSensorRepository.ReadSensorData();
            var sensorData = lightData as IList<Light> ?? lightData.ToList();
            await _lightRepository.PushToDb(sensorData);
            return true;
        }

        private async Task<bool> ReadPressure()
        {
            ReadStatus = "Reading Pressure";
            var pressureData = await _pressureSensorRepository.ReadSensorData();
            var sensorData = pressureData as IList<Pressure> ?? pressureData.ToList();
            await _pressureRepository.PushToDb(sensorData);
            return true;
        }

        private double ReadConfiguration()
        {
            ReadStatus = "Reading Configuration";
            var configurationData = _configurationSensorRepository.GetDeviceConfiguration();
            double shockThreshold = 0.0;
            foreach (var config in configurationData.Configs)
            {
                shockThreshold = Math.Round(config.ShockMaxThreshold, 2);
            }

            Task.Run(() =>_configurationRepository.PushToDb(configurationData));
            return shockThreshold;
        }

        private async Task<bool> ReadVibration()
        {
            ReadStatus = "Reading Vibration";
            var vibrationData = await _vibrationSensorRepository.ReadSensorData();
            var enumerable = vibrationData as IList<Vibration> ?? vibrationData.ToList();
            await _vibrationRepository.PushToDb(enumerable);
            return true;
        }

        private async Task<bool> ReadShock(double thresh)
        {
            ReadStatus = "Reading Shock";
            var shockData = await _shockSensorRepository.ReadSensorData();
            var sensorData = shockData as IList<Shock> ?? shockData.ToList();
            var filteredShock = sensorData.Where(evt => Math.Abs(evt.MaximumX.Value) > thresh ||
                                                       Math.Abs(evt.MaximumY.Value) > thresh ||
                                                       Math.Abs(evt.MaximumZ.Value) > thresh);
            await _shockRepository.PushToDb(filteredShock);
            return true;
        }

        private void StatusUpdate(Object source, ElapsedEventArgs e)
        {
            _eventAggregator.GetEvent<StatusEvent>().Publish(new StatusInformation
            {
                Action = ReadStatus,
                Status = _sensorReaderContext.GetTotalReadPacketsLength() + "/" + _sensorReaderContext.GetTotalPacketsLength(),
                IsCompleted = false
            });
        }

        private void BatteryStatusUpdate()
        {
            var level = _sensorReaderContext.GetBatteryLevel();
            _eventAggregator.GetEvent<BatteryStatus>().Publish(level + "%");
        }
    }
}
