using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Controls;
using PakTrack.Core;
using PakTrack.DAL.Interfaces;
using PakTrack.UI.Events;
using PakTrack.UI.MenuArea;
using PakTrack.UI.Utilities;
using PakTrack.Utilities;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using Prism.Regions;

namespace PakTrack.UI.Shell
{
    public class ShellViewModel : BindableBase
    {
        private readonly IConfigurationRepository _configurationRepository;
        private readonly IEventAggregator _eventAggregator;
        private readonly ILightRepository _lightRepository;
        private readonly ITemperatureRepository _temperatureRepository;
        private readonly IHumidityRepository _humidityRepository;
        private readonly IPressureRepository _pressureRepository;
        private readonly IVibrationRepository _vibrationRepository;
        private readonly IVibrationConsolidatedReport _vibrationConsolidatedReport;
        private readonly IShockRepository _shockRepository;
        private readonly string _organizationId = "5375fee944ae42f6704f102f";
        private readonly IRegionManager _regionManager;
        private bool _isAnalysisMenuEnabled;
        private bool _isCanSelectPackage;
        private bool _isFirstLoaded;
        private Collection<string> _packages;
        private string _selectedPackage;
        private string _selectedTruck;
        private string _title = "PakTrack Application";
        private Collection<string> _trucks;
        private string _currentView;
        private bool _isNotConfigPage = true;
        private string _action;
        private string _status;
        private bool _isCompleted;
        private bool _isVibrationView;
        private bool _isShockView;
        private bool _canDelete;
        private string _batteryLevel;

        public string BatteryLevel
        {
            get { return _batteryLevel; }
            set { SetProperty(ref _batteryLevel, value); }
        }
        public string Action
        {
            get { return _action; }
            set { SetProperty(ref _action, value); }
        }

        public string Status
        {
            get { return _status; }
            set { SetProperty(ref _status, value); }
        }

        public bool IsIndeterminate
        {
            get { return _isCompleted; }
            set { SetProperty(ref _isCompleted, value); }
        }

        public bool CanDelete
        {
            get { return _canDelete; }
            set { SetProperty(ref _canDelete, value); }
        }

        public ShellViewModel(IRegionManager regionManager, IConfigurationRepository configurationRepository,
            IEventAggregator eventAggregator,
            ILightRepository lightRepository,
            ITemperatureRepository temperatureRepository,
            IHumidityRepository humidityRepository,
            IPressureRepository pressureRepository,
            IVibrationRepository vibrationRepository,
            IVibrationConsolidatedReport vibrationConsolidatedReport,
            IShockRepository shockRepository)
        {
            Trucks = new BindingList<string>();
            Packages = new BindingList<string>();
            _regionManager = regionManager;
            _configurationRepository = configurationRepository;
            _eventAggregator = eventAggregator;
            _lightRepository = lightRepository;
            _temperatureRepository = temperatureRepository;
            _humidityRepository = humidityRepository;
            _pressureRepository = pressureRepository;
            _vibrationRepository = vibrationRepository;
            _vibrationConsolidatedReport = vibrationConsolidatedReport;
            _shockRepository = shockRepository;
            InitializeCommand();
            _eventAggregator.GetEvent<TruckAddEvent>().Subscribe(NewTruckAdded);
            _eventAggregator.GetEvent<RemoteNavigationEvent>().Subscribe(NavigateToConfigurationPage);
            _eventAggregator.GetEvent<StatusEvent>().Subscribe(SetStatus);
            _eventAggregator.GetEvent<BatteryStatus>().Subscribe(BateryStatus);
        }

        private void BateryStatus(string status)
        {
            BatteryLevel = "Battery Level: " + status;
        }

        private void SetStatus(StatusInformation statusInfo)
        {
            Action = statusInfo.Action;
            Status = statusInfo.Status;
            IsIndeterminate = !statusInfo.IsCompleted;
        }

        private void NavigateToConfigurationPage(string url)
        {
            Navigate(url);
        }

        private void NewTruckAdded(NavigationInformation navigationInfo)
        {
            Trucks.Clear();
            GetTrucksByOrganization();
            SelectedTruck = null;
            SelectedPackage = null;
        }

        public DelegateCommand DeleteAllEventsCommand { get; private set; }
        public DelegateCommand<string> NavigateCommand { get; private set; }
        public DelegateCommand<string> NavigateUncnditionalCommand { get; private set; }
        public DelegateCommand<string> ActivateApplicationAreaCommand { get; private set; }
        public DelegateCommand OnSelectedTruckChangedCommand { get; private set; }
        public DelegateCommand OnSelectedPackageChangedCommand { get; private set; }

        public string Title
        {
            get { return _title; }
            set { SetProperty(ref _title, value); }
        }

        public Collection<string> Trucks
        {
            get { return _trucks; }
            set
            {
                SetProperty(ref _trucks, value); 
            }
        }

        public string SelectedTruck
        {
            get { return _selectedTruck; }
            set
            {
                SetProperty(ref _selectedTruck, value);
                OnPropertyChanged("SelectedTruck");
            }
        }

        public Collection<string> Packages
        {
            get { return _packages; }
            set { SetProperty(ref _packages, value); }
        }

        public string SelectedPackage
        {
            get { return _selectedPackage; }
            set
            {
                SetProperty(ref _selectedPackage, value);
                OnPropertyChanged("SelectedPackage");
            }
        }

        public bool IsAnalysisMenuEnabled
        {
            get { return _isAnalysisMenuEnabled; }
            set { SetProperty(ref _isAnalysisMenuEnabled, value); }
        }

        public bool IsCanSelectPackage
        {
            get { return _isCanSelectPackage; }
            set { SetProperty(ref _isCanSelectPackage, value); }
        }

        public bool IsNotConfigPage
        {
            get { return _isNotConfigPage; }
            set { SetProperty(ref _isNotConfigPage, value); }
        }

        public bool IsVibrationView
        {
            get { return _isVibrationView; }
            set { SetProperty(ref _isVibrationView, value); }
        }

        public bool IsShockView
        {
            get { return _isShockView; }
            set { SetProperty(ref _isShockView, value); }
        }

        /// <summary>
        ///     This method is invoked from the window using the Loaded Event.
        ///     The loaded event is part of the Microsoft Interactivity assembly
        /// </summary>
        public void Initialize()
        {
            //Register Area
            _regionManager.RegisterViewWithRegion(ApplicationRegion.SideBarMenu, typeof(AnalysisMenu));
            _regionManager.RegisterViewWithRegion(ApplicationRegion.HeaderMenu, typeof(HeaderMenu));
            _regionManager.RegisterViewWithRegion(ApplicationRegion.MainRegion, typeof(MainMenuView));
            GetTrucksByOrganization();
        }

        /// <summary>
        ///     Initializes the commands
        /// </summary>
        private void InitializeCommand()
        {
            NavigateCommand = new DelegateCommand<string>(Navigate)
                .ObservesCanExecute(o => IsAnalysisMenuEnabled);

            NavigateUncnditionalCommand = new DelegateCommand<string>(Navigate);

            ActivateApplicationAreaCommand = new DelegateCommand<string>(ActivateApplicationArea);
            OnSelectedTruckChangedCommand = new DelegateCommand(OnSelectedTruckChanged);

            OnSelectedPackageChangedCommand = new DelegateCommand(OnSelectedPackageChanged)
                .ObservesCanExecute(o => IsCanSelectPackage)
                .ObservesProperty(() => SelectedTruck);

            DeleteAllEventsCommand = new DelegateCommand(DeleteAllEvents);
        }

        /// <summary>
        ///     Activate a given area of the application based on the main menu selection
        /// </summary>
        /// <param name="appArea"></param>
        private void ActivateApplicationArea(string appArea)
        {
            //throw new System.NotImplementedException();
        }

        /// <summary>
        ///     Navigate to a given view
        /// </summary>
        /// <param name="url">name of the view</param>
        private void Navigate(string url)
        {
            CanDelete = false;
            IsVibrationView = false;
            IsShockView = false;
            if (url == PakTrackConstant.MenuOptionVibration ||
                url == PakTrackConstant.MenuOptionVibrationConsolidatedReport ||
                url == PakTrackConstant.MenuOptionVibrationProbabilityReport)
                IsVibrationView = true;
            if (url == PakTrackConstant.MenuOptionShock || 
                url == PakTrackConstant.MenuOptionOrientationHistogram || 
                url == PakTrackConstant.MenuOptionsDropHeightHistogram
                ///////----------------RFC-shock-histo-------------///
                || url == PakTrackConstant.MenuOptionShockProbabilityReport
                /////-----------RFC-shock-histo-------------------///
                )
                IsShockView = true;
            IsNotConfigPage = true;
            if (url == PakTrackConstant.MenuOptionConfiguration)
                IsNotConfigPage = false;
            if (url == PakTrackConstant.MenuOptionDashboard)
                CanDelete = true;
            var navigationParams = new NavigationParameters
            {
                {PakTrackConstant.TruckId, SelectedTruck},
                {PakTrackConstant.PackageId, SelectedPackage}
            };
            if (_currentView != PakTrackConstant.MenuOptionDashboard && _currentView == url) return; //Do not renavigate to same view, it messes up the state
            _regionManager.RequestNavigate(ApplicationRegion.MainRegion, url, navigationParams);
            _currentView = url;
            _eventAggregator.GetEvent<StatusEvent>().Publish(new StatusInformation
            {
                Action = string.Empty,
                Status = string.Empty,
                IsCompleted = true
            });
        }

        public async void DeleteAllEvents()
        {

            if (SelectedPackage != null && SelectedTruck != null)
            {
                var lisght = await _lightRepository.DeleteEvents(SelectedTruck, SelectedPackage);
                var vib = await _vibrationRepository.DeleteEvents(SelectedTruck, SelectedPackage);
                var shock=await _shockRepository.DeleteEvents(SelectedTruck, SelectedPackage);
                var humid =await _humidityRepository.DeleteEvents(SelectedTruck, SelectedPackage);
                var pressure =await _pressureRepository.DeleteEvents(SelectedTruck, SelectedPackage);
                var temp =await _temperatureRepository.DeleteEvents(SelectedTruck, SelectedPackage);
                var config =await _configurationRepository.DeleteEvents(SelectedTruck, SelectedPackage);
                var report = await _vibrationConsolidatedReport.DeleteEvents(SelectedTruck, SelectedPackage);

                Trucks.Clear();
                GetTrucksByOrganization();
                SelectedTruck = string.Empty;
                SelectedPackage = string.Empty;

                Navigate(PakTrackConstant.MenuOptionSetting);
            }
        }

        /// <summary>
        ///     Get the trucks for a given organization and set the first one as the selected option
        /// </summary>
        private void GetTrucksByOrganization()
        {
            foreach (var truck in _configurationRepository.GetAllTrucks())
            {
                if(truck != null && !Trucks.Contains(truck))
                    Trucks.Add(truck);
            }
        }

        /// <summary>
        ///     When the package is selected we enable navigation
        /// </summary>
        private void OnSelectedPackageChanged()
        {
            IsAnalysisMenuEnabled = true;
            if (!_isFirstLoaded)
            {
                _isFirstLoaded = true;
            }
            else
            {
                var regionInfo = _regionManager.Regions[ApplicationRegion.MainRegion];
                var navigationEntry = regionInfo.NavigationService.Journal.CurrentEntry;

                if (navigationEntry == null || SelectedPackage == null || SelectedTruck == null)
                    return;
                var navigationInfo = new NavigationInformation
                {
                    TruckId = SelectedTruck,
                    PackageId = SelectedPackage
                };
                _eventAggregator.GetEvent<NavigationEvent>().Publish(navigationInfo);
            }
            Navigate(PakTrackConstant.MenuOptionDashboard);

        }

        /// <summary>
        ///     On selected truck we get the packages and display them to the user.
        ///     Also it enables
        /// </summary>
        private void OnSelectedTruckChanged()
        {
            IsAnalysisMenuEnabled = false;
            SelectedPackage = string.Empty;
            Packages.Clear();
            Packages = new BindingList<string>(_configurationRepository.GetPackageByTruckId(SelectedTruck).ToList());
            IsCanSelectPackage = true;
        }
    }
}