using System;

namespace PakTrack.Utilities
{
    public static class PakTrackConstant
    {
        public const string EventId = "eventId";
        public const string Setting = "setting";
        public const string PackageId = "packageId";
        public const string TruckId = "truckId";

        //Related to the vibration area
        public const string VibrationTimeGraph = "vibrationTimeChart";
        public const string VibrationPsdGraph = "vibrationPsdChart";
        public const string GraphType = "graphType";

        //Related to the shock area
        public const string ShockTimeGraph = "shockTimeGraph";
        public const string ShockSRSGraph = "shockSRSGraph";
       
        //Menu
        public const string MenuOptionDashboard = "dashboard";
        public const string MenuOptionLight = "light";
        public const string MenuOptionTemperature = "temperature";
        public const string MenuOptionHumidity = "humidity";
        public const string MenuOptionPressure = "pressure";
        public const string MenuOptionShock = "shock";
        public const string MenuOptionVibration = "vibration";
        public const string MenuOptionSetting = "setting";
        public const string MenuOptionConfiguration = "config";
        public const string MenuOptionOrientationHistogram = "orientation";

        public const string MenuOptionsDropHeightHistogram = "dropHeight";
        //Menu sub-options
        public const string MenuOptionVibrationProbabilityReport = "vibrationProbabilityReport";
        public const string MenuOptionVibrationConsolidatedReport = "vibrationConsolidatedReport";
        public const string VibrationConsolidatedReportDetails = "VibrationConsolidatedReportDetails";
        public const string VibrationConsolidatedReportNDetails = "VibrationConsolidatedReportNDetails";

        public const string MenuOptionVibrationCustomConsolidatedReport = "VibrationCustomConsolidatedReport";
        ///////----------------RFC-shock-histo MenuOptionShockProbabilityReport-------------///
        public const string MenuOptionShockProbabilityReport = "shockProbabilityReport";
        ///////----------------RFC-shock-histo-------------///
        public const string MenuOptionShockOrientation = "shockOrientation";

        //Main Menu area
        public const string MainMenuOptionDataAnalysis = "DataAnalysis";

        //Sensor Options
        public const string SensorConnect = "connect";
        public const string SensorRead = "read";


        //DB save batch size
        private const int DefaultDBBatchSize = 200;

        public const int DBBatchSizeHeavyData = 50;

        public const int DBBatchSizeLiteData = DefaultDBBatchSize;

    }
}