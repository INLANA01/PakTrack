using System;
using System.Configuration;
using System.IO;
using Microsoft.Practices.Unity;
using Prism.Unity;
using System.Windows;
using PakTrack.BL;
using PakTrack.BL.Interfaces;
using PakTrack.DAL.Context;
using PakTrack.DAL.Interfaces;
using PakTrack.DAL.Interfaces.Sensor;
using PakTrack.DAL.Repositories;
using PakTrack.DAL.Repositories.Sensor;
using PakTrack.UI.DashboardArea;
using PakTrack.UI.HumidityArea;
using PakTrack.UI.LightArea;
using PakTrack.UI.PressureArea;
using PakTrack.UI.ShockArea;
using PakTrack.UI.TemperatureArea;
using PakTrack.UI.VibrationArea;
using PakTrack.Utilities;

namespace PakTrack.UI
{
    class Bootstrapper : UnityBootstrapper
    {

        protected override DependencyObject CreateShell()
        {
            return Container.Resolve<Shell.Shell>();
        }

        protected override void InitializeShell()
        {
            Application.Current.MainWindow.Show();
        }

        protected override void ConfigureContainer()
        {
            base.ConfigureContainer();
            //Navigation settings
           // Container.RegisterTypeForNavigation<type>("key");
            RegisterDependency();
            SetupNavigation();
        }

        /// <summary>
        /// Register all navigation route
        /// </summary>
        private void SetupNavigation()
        {
            //DashboardView 
            Container.RegisterTypeForNavigation<DashboardView>(PakTrackConstant.MenuOptionDashboard);
            //VibrationView
            Container.RegisterTypeForNavigation<VibrationView>(PakTrackConstant.MenuOptionVibration);

            //Vibration Time Graph
            Container.RegisterTypeForNavigation<VibrationTimeGraphView>(PakTrackConstant.VibrationTimeGraph);
            //Vibration PSD Graph
            Container.RegisterTypeForNavigation<VibrationPSDGraphView>(PakTrackConstant.VibrationPsdGraph);
            //Consolidated report
            Container.RegisterTypeForNavigation<VibrationConsolidatedReportView>(
                PakTrackConstant.MenuOptionVibrationConsolidatedReport);

            Container.RegisterTypeForNavigation<VibrationConsolidatedReportDetailsView>(
                PakTrackConstant.VibrationConsolidatedReportDetails);

            Container.RegisterTypeForNavigation<VibrationCustomConsolidatedReportView>(PakTrackConstant
                .MenuOptionVibrationCustomConsolidatedReport);


            //LightView
            Container.RegisterTypeForNavigation<LightView>(PakTrackConstant.MenuOptionLight);
            //Setting
            Container.RegisterTypeForNavigation<Setting.Setting>(PakTrackConstant.MenuOptionSetting);
            Container.RegisterTypeForNavigation<Setting.Configuration>(PakTrackConstant.MenuOptionConfiguration);


            //Shock
            Container.RegisterTypeForNavigation<ShockView>(PakTrackConstant.MenuOptionShock);
            ////----------------RFC-shock-histo-------------//
            ////Vibration Probability Histogram 
            Container.RegisterTypeForNavigation<ShockProbabilityReportView>(PakTrackConstant.MenuOptionShockProbabilityReport);
            ////----------------RFC-shock-histo-------------//
            //Shock Time-Domain Graph
            Container.RegisterTypeForNavigation<ShockTimeGraphView>(PakTrackConstant.ShockTimeGraph);

            //Shock SRS Grpah
            Container.RegisterTypeForNavigation<ShockSRSGraphView>(PakTrackConstant.ShockSRSGraph);

            //Pressure 
            Container.RegisterTypeForNavigation<PressureView>(PakTrackConstant.MenuOptionPressure);

            //Humidity
            Container.RegisterTypeForNavigation<HumidityView>(PakTrackConstant.MenuOptionHumidity);

            //Temperature
            Container.RegisterTypeForNavigation<TemperatureView>(PakTrackConstant.MenuOptionTemperature);

            //Orientation Histogram 
            Container.RegisterTypeForNavigation<OrientationHistogramView>(PakTrackConstant
                .MenuOptionOrientationHistogram);

            //Vibration Probability Histogram 
            Container.RegisterTypeForNavigation<VibrationProbabilityReportView>(PakTrackConstant
                .MenuOptionVibrationProbabilityReport);
            
            //Drop Height Histogram
            Container.RegisterTypeForNavigation<DropHeightHistogramView>(
                PakTrackConstant.MenuOptionsDropHeightHistogram);
        }


        /// <summary>
        /// Register depedency injection
        /// </summary>
        private void RegisterDependency()
        {
            var connectionString = GetConnectionString();
            //DataConext
            Container.RegisterType<IPakTrackDbContext, PakTrackDbContext>(new InjectionConstructor(connectionString));

            //////////////////////////Repositories Binding //////////////////////////

            //LightView
            Container.RegisterType<ILightRepository, LightRepository>();

            //VibrationView
            Container.RegisterType<IVibrationRepository, VibrationRepository>();
            //Vibration Consolidated Report
            Container.RegisterType<IVibrationConsolidatedReport, VibrationConsolidatedReport>();

            //ConfigurationView
            Container.RegisterType<IConfigurationRepository, ConfigurationRepository>();

            //Shock
            Container.RegisterType<IShockRepository, ShockRepository>();

            //Report Manager
            Container.RegisterType<IReportManager, ReportManager>();

            //Pressure 
            Container.RegisterType<IPressureRepository, PressureRepository>();

            //Humidity
            Container.RegisterType<IHumidityRepository, HumidityRepository>();

            //Temperature
            Container.RegisterType<ITemperatureRepository, TemperatureRepository>();

            /***************************Sensor Dependencies**********************************/

            Container.RegisterType<ISensorReaderContext, SensorReaderContext>(new ContainerControlledLifetimeManager());

            //Temperature Sensor
            Container.RegisterType<ITemperatureSensorRepository, TemperatureSensorRepository>();

            //Humidity sensor 
            Container.RegisterType<IHumiditySensorRepository, HumiditySensorRepository>();

            //Sensor Configuration reader
            Container.RegisterType<IConfigurationSensorRepository, ConfigurationSensorRepository>();

            //Pressure Sensor 
            Container.RegisterType<IPressureSensorRepository, PressureSensorRepository>();

            //Light Sensor 
            Container.RegisterType<ILightSensorRepository, LightSensorRepository>();

            //Vibration Sensor
            Container.RegisterType<IVibrationSensorRepository, VibrationSensorRepository>();

            //Shock Sensor
            Container.RegisterType<IShockSensorRepository, ShockSensorRepository>();
            //Flash Data
            Container.RegisterType<IFlashDataSensorRepository, FlashDataSensorRepository>();
        }

        /// <summary>
        /// Get the database location from the App.Config file then construct a connection string
        /// using the current directory combina with the database location
        /// </summary>
        /// <returns>string</returns>
        private string GetConnectionString()
        {
            var path = Directory.GetCurrentDirectory();
            var currrentPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            var dbLocation = currrentPath +"\\PakTrack\\"+ ConfigurationManager.AppSettings["DbLocation"];
            var dbPath = Path.GetFullPath(Path.Combine(path, dbLocation));
            return dbPath;
        }
    }
}
