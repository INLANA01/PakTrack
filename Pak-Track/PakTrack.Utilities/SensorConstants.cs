using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PakTrack.Utilities
{
    public static class SensorConstants
    {
        public const bool DEBUG = true;

        public const String SEPERATOR = "-";
        public const String NO_ID = "NO_ID";
        public static readonly int[] FRAME_SYNC = { 170, 255, 85 };
        public const int SYNC_BYTE = 126;
        public const int ESCAPE_BYTE = 125;
        public const int P_ACK = 67;
        public const int P_REGISTRATION = 153;
        public const int P_SERVICE_REQUEST = 255;
        public const int P_SERVICE_RESPONSE = 0;
        public const int P_SERVICE_REPORT_RATE = 254;
        public const int P_SERVICE_UPDATE_THRESHOLD = 253;
        public const int P_BLACKBOX_REQUEST = 208;
        public const int P_BLACKBOX_RESPONSE = 209;
        public const int P_TIME_SYNC = 160;
        public const int P_UPDATE = 1;
        public const int P_UPDATE_THRESHOLD = 2;
        public const int P_STREAM_PACKET = 210;
        public const int P_UNKNOWN = 255;
        public const int MTU = 1100;
        public const int ACK_TIMEOUT = 1000; // in milliseconds
        public const int P_CONFIG_SIZE = 85;


        // Report Rate Types
        public const int NORMAL_REPORT_RATE = 0;
        public const int AFTER_THRESHOLD_REPORT_RATE = 1;

        // Threshold Types
        public const int HIGH_THRESHOLD = 0;
        public const int LOW_THRESHOLD = 1;


        //Black Box serial Commands
        public static readonly byte[] GET_BOARD_ID = { 2, 00 };
        public static readonly byte[] GET_ALL_CONFIG = { 50, 00 };
        public static readonly byte[] GET_TEMPERATURE = { 17, 01 };
        public static readonly byte[] GET_HUMIDITY = { 17, 02 };
        public static readonly byte[] GET_VIBRATION = { 17, 03 };
        public static readonly byte[] GET_SHOCK = { 17, 04 };
        public static readonly byte[] GET_NOTE = { 17, 05 };
        public static readonly byte[] GET_LOG = { 17, 06 };
        public static readonly byte[] GET_LIGHT = { 17, 07 };
        public static readonly byte[] GET_PRESSURE = { 17, 8 };
        public static readonly byte[] GET_FLASH = { 33, 00 };

        public static readonly byte[] SET_ALL_CONFIG = { 52, 00 };
        public static readonly byte[] SAVE_ALL_CONFIG = { 53, 00 };
        public static readonly byte[] SAVE_COMMENT = { 19, 00 };

        public static readonly byte[] RESET_BOARD = { 1, 0 };
        public static readonly byte[] RESET_CONFIG = { 54, 0 };
        public static readonly byte[] RESET_RADIO = { 81, 0 };

        public static readonly byte[] FORMAT_SD_CARD = { 18, 00 };
        public static readonly byte[] FORMAT_FLASH = { 34, 00 };

        public static readonly byte[] GET_BATTERY_LEVEL = { 96, 00 };

        public static readonly byte[] RUN_ACCELEROMETER_CALIBRATION = { 65, 00 };

        public static readonly byte[] START_MEASUREMENT = { 112, 00 };
        public static readonly byte[] END_MEASUREMENT = { 113, 00 };

        public static readonly byte[] RETURN_TO_SCHEDULE = { 114, 00 };
        public static readonly byte[] DELETE_SCHEDULE = { 115, 00 };

        public static readonly byte[] SCHEDULE_SERVICE = { 1 };
        public static readonly byte[] CONFIG_SERVICE = { 2 };
        /**
         * Scheduling Configuration Stream Packet index
         */
        public const int TIME_DATA_SIZE = 4;
        public const int STREAM_START_TIME_INDEX = 0;
        public const int STREAM_END_TIME_INDEX = 4;
        public const int STREAM_SWITCH_TIME_INDEX = 8;

        public const int STREAM_TEMP_NORMAL_REPORT_RATE_INDEX = 12;
        public const int STREAM_TEMP_OVER_THRESHOLD_REPORT_RATE_INDEX = 14;
        public const int STREAM_TEMP_MAX_THRESHOLD_INDEX = 16;
        public const int STREAM_TEMP_MIN_THRESHOLD_INDEX = 18;

        public const int STREAM_HUMIDITY_NORMAL_REPORT_RATE_INDEX = 20;
        public const int STREAM_HUMIDITY_OVER_THRESHOLD_REPORT_RATE_INDEX = 22;
        public const int STREAM_HUMIDITY_MAX_THRESHOLD_INDEX = 24;
        public const int STREAM_HUMIDITY_MIN_THRESHOLD_INDEX = 26;

        public const int STREAM_VIB_NORMAL_REPORT_RATE_INDEX = 28;
        public const int STREAM_VIB_OVER_THRESHOLD_REPORT_RATE_INDEX = 30;
        public const int STREAM_VIB_MAX_THRESHOLD_INDEX = 32;

        public const int STREAM_SHOCK_MAX_THRESHOLD_INDEX = 34;

        //Genereal Config Settings 
        public const int GENERAL_SETTINGS_SIZE = 7;
        public const int RADIO_RESTART_DELAY_INDEX = 0;
        public const int RADIO_MAX_RETRIES_INDEX = 1;
        public const int RADIO_MAX_FAILURES_INDEX = 2;
        public const int RADIO_PANID_INDEX = 3;
        public const int CONFIG_TYPE_INDEX = 5;
        public const int DATA_IN_FLASH_INDEX = 6;

        //General Default Config Values
        public const int DEFAULT_RADIO_RESTART_DELAY = 8;
        public const int DEFAULT_RADIO_MAX_RETRIES = 3;
        public const int DEFAULT_RADIO_MAX_FAILURES = 3;
        public const int DEFAULT_RADIO_PANID = 65535;
        public const int DEFAULT_CONFIG_TYPE = 0;
        public const int DEFAULT_DATA_IN_FLASH = 0;

        //Default misc config
        public const int CONFIG_VIB_SHOCk_BOTH_INDEX = 94;
        public const int DEFAULT_CONFIG_VIB_SHOCk_BOTH = 1;


        /**
         *  Black Box Configuration Packet index
         */
        //Temperature Index
        public const int TEMPERATURE_SETTINGS_SIZE = 10;
        public const int TEMP_TIME_INDEX = 7;
        public const int TEMP_TIME_OVER_THRES_INDEX = TEMP_TIME_INDEX + 2; //9
        public const int TEMP_MAX_THRESHOLD_INDEX = TEMP_TIME_OVER_THRES_INDEX + 2;//11
        public const int TEMP_MIN_THRESHOLD_INDEX = TEMP_MAX_THRESHOLD_INDEX + 2;//13
        public const int TEMP_OVER_THRESHOLD_INDEX = TEMP_MIN_THRESHOLD_INDEX + 2;//15
        public const int TEMP_UNDER_THRESHOLD_INDEX = TEMP_MIN_THRESHOLD_INDEX + 1; //16

        //Temperature default values
        public const int DEFAULT_TEMP_TIME = 10;
        public const int DEFAULT_TEMP_TIME_OVER_THRES = 5;
        public const int DEFAULT_TEMP_MAX_THRES = 27419;
        public const int DEFAULT_TEMP_MIN_THRES = 17473;
        public const int DEFAULT_TEMP_OVER_THRESHOLD = 0;
        public const int DEFAULT_TEMP_UNDER_THRESHOLD = 0;

        /*
        public const int TEMP_UNITS = 8;
        public const int TEMP_UNIT_MAXIMUM = 9;
        public const int TEMP_UNIT_MINIMUM = 11;
        public const int DEFAULT_TEMP_UNITS = 0;
        public const int DEFAULT_TEMP_UNIT_MAXIMUM = 4320;
        public const int DEFAULT_TEMP_UNIT_MINIMUM = 320;
        */
        //Humidity Index
        public const int HUMIDITY_SETTINGS_SIZE = 10;
        public const int HUMD_TIME_INDEX = 17;
        public const int HUMD_TIME_OVER_THRES_INDEX = HUMD_TIME_INDEX + 2;//19
        public const int HUMD_MAX_THRES_INDEX = HUMD_TIME_OVER_THRES_INDEX + 2;//21
        public const int HUMD_MIN_THRES_INDEX = HUMD_MAX_THRES_INDEX + 2;//23
        public const int HUMD_OVER_THRESHOLD_INDEX = HUMD_MIN_THRES_INDEX + 2;//25
        public const int HUMD_UNDER_THRESHOLD_INDEX = HUMD_OVER_THRESHOLD_INDEX + 1; //26

        //Humidity default values
        public const int DEFAULT_HUMD_TIME = 10;
        public const int DEFAULT_HUMD_TIME_OVER_THRES = 5;
        public const int DEFAULT_HUMD_MAX_THRES = 45088;
        public const int DEFAULT_HUMD_MIN_THRES = 5767;
        public const int DEFAULT_HUMD_OVER_THRESHOLD = 0;
        public const int DEFAULT_HUMD_UNDER_THRESHOLD = 0;

        /*
        public const int HUMD_UNITS = 21;
        public const int HUMD_UNIT_MAXIMUM = 22;
        public const int HUMD_UNIT_MINIMUM = 24;
        public const int DEFAULT_HUMD_UNITS = 1;
        public const int DEFAULT_HUMD_UNIT_MAXIMUM = 1984;
        public const int DEFAULT_HUMD_UNIT_MINIMUM = 384;
        */
        //Vibration Config Index
        public const int VIBRATION_SETTINGS_SIZE = 17;
        public const int VIB_TIME_INDEX = 27;
        public const int VIB_TIME_OVER_THRES_INDEX = VIB_TIME_INDEX + 2;//29
        public const int VIB_MAX_THRESHOLD_INDEX = VIB_TIME_OVER_THRES_INDEX + 2;//31
        public const int VIB_OVER_THRESHOLD_INDEX = VIB_MAX_THRESHOLD_INDEX + 2;//33
        public const int VIB_X_OFFSET_INDEX = VIB_OVER_THRESHOLD_INDEX + 1;//34
        public const int VIB_Y_OFFSET_INDEX = VIB_X_OFFSET_INDEX + 1;//35
        public const int VIB_Z_OFFSET_INDEX = VIB_Y_OFFSET_INDEX + 1;//36
        public const int VIB_INACTIVITY_THRESHOLD_INDEX = VIB_Z_OFFSET_INDEX + 1;//37
        public const int VIB_INACTIVITY_TIME = VIB_INACTIVITY_THRESHOLD_INDEX + 1;//38
        public const int VIB_TAP_THRESHOLD = VIB_INACTIVITY_TIME + 1;//39
        public const int VIB_TAP_DURATION = VIB_TAP_THRESHOLD + 1;//40
        public const int VIB_TAP_LATENCY = VIB_TAP_DURATION + 1;//41
        public const int VIB_TAP_WINDOW = VIB_TAP_LATENCY + 1;//42

        //Vibration default values
        public const int DEFAULT_VIB_TIME = 120;
        public const int DEFAULT_VIB_TIME_OVER_THRES = 60;
        public const int DEFAULT_VIB_MAX_THRESHOLD = 64;
        public const int DEFAULT_VIB_OVER_THRESHOLD = 0;
        public const int DEFAULT_VIB_X_OFFSET = 0;
        public const int DEFAULT_VIB_Y_OFFSET = 0;
        public const int DEFAULT_VIB_Z_OFFSET = 0;
        public const int DEFAULT_VIB_INACTIVITY_THRES = 5;
        public const int DEFAULT_VIB_INACTIVITY_TIME = 2;
        public const int DEFAULT_VIB_TAP_THRESHOLD = 24;//0.5g
        public const int DEFAULT_VIB_TAP_DURATION = 255;//159.375 ms
        public const int DEFAULT_VIB_TAP_LATENCY = 12;//15
        public const int DEFAULT_VIB_TAP_WINDOW = 255;//318.75

        /*
        public const int VIB_X_UNIT_INDEX = 68;
        public const int VIB_Y_UNIT_INDEX = 69;
        public const int VIB_Z_UNIT_INDEX = 70;
        public const int VIB_X_MAXIMUM_INDEX = 71;
        public const int VIB_Y_MAXIMUM_INDEX = 73;
        public const int VIB_Z_MAXIMUM_INDEX = 75;
        public const int VIB_X_MINIMUM_INDEX = 77;
        public const int VIB_Y_MINIMUM_INDEX = 79;
        public const int VIB_Z_MINIMUM_INDEX = 81;
       */

        //Shock Config Index
        public const int SHOCK_SETTINGS_SIZE = 9;
        public const int SHOCK_TIME_INDEX = 43;
        public const int SHOCK_OVER_THRESHOLD_TIME_INDEX = SHOCK_TIME_INDEX + 2;//45
        public const int SHOCK_MAX_THRESHOLD_INDEX = SHOCK_OVER_THRESHOLD_TIME_INDEX + 2;//47
        public const int SHOCK_FREE_FALL_THRESHOLD_INDEX = SHOCK_MAX_THRESHOLD_INDEX + 2;//49
        public const int SHOCK_FREE_FALL_TIME_INDEX = SHOCK_FREE_FALL_THRESHOLD_INDEX + 1;//50
        public const int SHOCK_ACTIVITY_THRESHOLD_INDEX = SHOCK_FREE_FALL_THRESHOLD_INDEX + 1;//51

        //Shock default values
        public const int DEFAULT_SHOCK_TIME = 0;
        public const int DEFAULT_SHOCK_OVER_THRESHOLD_TIME = 0;//45
        public const int DEFAULT_SHOCK_MAX_THRESHOLD = 10;//47
        public const int DEFAULT_SHOCK_FREE_FALL_THRESHOLD = 7;
        public const int DEFAULT_SHOCK_FREE_FALL_TIME = 14;
        public const int DEFAULT_SHOCK_ACTIVITY_THRESHOLD = 128;

        /*
        public const int SHOCK_X_UNIT_INDEX = 32;
        public const int SHOCK_Y_UNIT_INDEX = 33;
        public const int SHOCK_Z_UNIT_INDEX = 34;
        public const int SHOCK_X_MAXIMUM_INDEX = 35;
        public const int SHOCK_Y_MAXIMUM_INDEX = 37;
        public const int SHOCK_Z_MAXIMUM_INDEX = 39;
        public const int SHOCK_X_MINIMUM_INDEX = 41;
        public const int SHOCK_Y_MINIMUM_INDEX = 43;
        public const int SHOCK_Z_MINIMUM_INDEX = 45;
        */
        //Light index values
        public const int LIGHT_SETTINGS_SIZE = 12;
        public const int LIGHT_REPORT_RATE_INDEX = 52;
        public const int LIGHT_OVER_THRESHOLD_REPORT_RATE_INDEX = LIGHT_REPORT_RATE_INDEX + 2;//54
        public const int LIGHT_HIGH_THRESHOLD_INDEX = LIGHT_OVER_THRESHOLD_REPORT_RATE_INDEX + 2;//56
        public const int LIGHT_LOW_THRESHOLD_INDEX = LIGHT_HIGH_THRESHOLD_INDEX + 2;//58
        public const int LIGHT_WAIT_TIME_INDEX = LIGHT_LOW_THRESHOLD_INDEX + 2;//60
        public const int LIGHT_PRESISTENCE_INDEX = LIGHT_WAIT_TIME_INDEX + 1;//61
        public const int LIGHT_OVER_THRESHOLD_INDEX = LIGHT_PRESISTENCE_INDEX + 1;//62
        public const int LIGHT_UNDER_THRESHOLD_INDEX = LIGHT_OVER_THRESHOLD_INDEX + 1;//63

        //Light default values
        public const int DEFAULT_LIGHT_REPORT_RATE = 30;
        public const int DEFAULT_LIGHT_OVER_THRESHOLD_REPORT_RATE = 15;
        public const int DEFAULT_LIGHT_HIGH_THRESHOLD = 32768;
        public const int DEFAULT_LIGHT_LOW_THRESHOLD = 0;
        public const int DEFAULT_LIGHT_WAIT_TIME = 171;
        public const int DEFAULT_LIGHT_PRESISTENCE = 2;
        public const int DEFAULT_LIGHT_OVER_THRESHOLD_INDEX = 0;
        public const int DEFAULT_LIGHT_UNDER_THRESHOLD_INDEX = 0;

        //Pressure index values
        public const int PRESSURE_SETTINGS_SIZE = 11;
        public const int PRESSURE_REPORT_RATE_INDEX = 64;
        public const int PRESSURE_OVER_THRESHOLD_REPORT_RATE_INDEX = PRESSURE_REPORT_RATE_INDEX + 2;//66
        public const int PRESSURE_HIGH_THRESHOLD_INDEX = PRESSURE_OVER_THRESHOLD_REPORT_RATE_INDEX + 2;//68
        public const int PRESSURE_CHANGE_INDEX = PRESSURE_HIGH_THRESHOLD_INDEX + 2;//70
        public const int PRESSURE_TIME_STEP_INDEX = PRESSURE_CHANGE_INDEX + 2;//72
        public const int PRESSURE_OVER_SAMPLE_RATE_INDEX = PRESSURE_TIME_STEP_INDEX + 1;//73
        public const int PRESSURE_OVER_THRESHOLD_INDEX = PRESSURE_OVER_SAMPLE_RATE_INDEX + 1;//74

        //Pressure default values
        public const int DEFAULT_PRESSURE_REPORT_RATE = 30;
        public const int DEFAULT_PRESSURE_OVER_THRESHOLD_REPORT_RATE = 15;
        public const int DEFAULT_PRESSURE_HIGH_THRESHOLD = 6000;
        public const int DEFAULT_PRESSURE_CHANGE = 500;
        public const int DEFAULT_PRESSURE_TIME_STEP = 4;
        public const int DEFAULT_PRESSURE_OVER_SAMPLE_RATE = 7;
        public const int DEFAULT_PRESSURE_OVER_THRESHOLD = 0;


        //Temperature Index
        public const int TEMP2_TIME_INDEX = 75;
        public const int TEMP2_TIME_OVER_THRES_INDEX = TEMP2_TIME_INDEX + 2; //77
        public const int TEMP2_MAX_THRESHOLD_INDEX = TEMP2_TIME_OVER_THRES_INDEX + 2;//79
        public const int TEMP2_MIN_THRESHOLD_INDEX = TEMP2_MAX_THRESHOLD_INDEX + 2;//81
        public const int TEMP2_OVER_THRESHOLD_INDEX = TEMP2_MIN_THRESHOLD_INDEX + 2;//83
        public const int TEMP2_UNDER_THRESHOLD_INDEX = TEMP2_MIN_THRESHOLD_INDEX + 1; //84

        //Temperature default values
        public const int DEFAULT_TEMP2_TIME = 10;
        public const int DEFAULT_TEMP2_TIME_OVER_THRES = 5;
        public const int DEFAULT_TEMP2_MAX_THRES = 2453;
        public const int DEFAULT_TEMP2_MIN_THRES = 1600;
        public const int DEFAULT_TEMP2_OVER_THRESHOLD = 0;
        public const int DEFAULT_TEMP2_UNDER_THRESHOLD = 0;


        //Humidity Index
        public const int HUMD2_TIME_INDEX = 85;
        public const int HUMD2_TIME_OVER_THRES_INDEX = HUMD2_TIME_INDEX + 2;//87
        public const int HUMD2_MAX_THRES_INDEX = HUMD2_TIME_OVER_THRES_INDEX + 2;//89
        public const int HUMD2_MIN_THRES_INDEX = HUMD2_MAX_THRES_INDEX + 2;//91
        public const int HUMD2_OVER_THRESHOLD_INDEX = HUMD2_MIN_THRES_INDEX + 2;//93
        public const int HUMD2_UNDER_THRESHOLD_INDEX = HUMD2_OVER_THRESHOLD_INDEX + 1; //94

        //Humidity default values
        public const int DEFAULT_HUMD2_TIME = 10;
        public const int DEFAULT_HUMD2_TIME_OVER_THRES = 5;
        public const int DEFAULT_HUMD2_MAX_THRES = 1664;
        public const int DEFAULT_HUMD2_MIN_THRES = 464;
        public const int DEFAULT_HUMD2_OVER_THRESHOLD = 0;
        public const int DEFAULT_HUMD2_UNDER_THRESHOLD = 0;

        public const string SensorDiscoveryName = "TI MSP430"; // Name to find sensor over com-port

        public const string DEFAULT_TEMPERATURE_UNIT = "F";
        public const string DEFAULT_HUMIDITY_UNIT = "%RH";
        public const string DEFAULT_VIBRATION_UNIT = "g";
        public const string DEFAULT_LIGHT_UNIT = "clear light";
        public const string DEFAULT_PRESSURE_UNIT = "KPa";


        public const double DefaultTemperatureThreshold = 80;
        public const double DefaultHumidityThrehold = 50;
        public const double DefaultVibrationThreshold = 0.2;
        public const double DefaultShockThreshold = 10;
        public const double DefaultPressureThrehold = 101000.0;
        public const double DefaultLightThreshold = 500.0;

        public const int DefaultBeforeTemperatureThresholdTimeMin = 10;
        public const int DefaultBeforeTemperatureThresholdTimeSec = 0;
        public const int DefaultAfterTemperatureThresholdTimeMin = 5;
        public const int DefaultAfterTemperatureThresholdTimeSec = 0;

        public const int DefaultBeforeHumidityThresholdTimeMin = 10;
        public const int DefaultBeforeHumidityThresholdTimeSec = 0;
        public const int DefaultAfterHumidityThresholdTimeMin = 5;
        public const int DefaultAfterHumidityThresholdTimeSec = 0;

        public const int DefaultAfterVibrationThresholdTimeMin = 0; // changed to 0 previously 5
        public const int DefaultAfterVibrationThresholdTimeSec = 30;

        public const int DefaultBeforePressureThresholdTimeMin = 10;
        public const int DefaultBeforePressureThresholdTimeSec = 0;
        public const int DefaultAfterPressureThresholdTimeMin = 5;
        public const int DefaultAfterPressureThresholdTimeSec = 0;

        public const int DefaultBeforeLightThresholdTimeMin = 10;
        public const int DefaultBeforeLightThresholdTimeSec = 0;
        public const int DefaultAfterLightThresholdTimeMin = 5;
        public const int DefaultAfterLightThresholdTimeSec = 0;

    }
}
