using System;

namespace PakTrack.SensorsConfigurator.SensorDataReader.Rtpmt.Sensor.Util 
{
    public class Constants
    {
            public readonly static bool DEBUG = true;

            public readonly static String SEPERATOR = "-";
            public readonly static String NO_ID = "NO_ID";
            public readonly static int[] FRAME_SYNC = { 170, 255, 85 };
            public readonly static int SYNC_BYTE = 126;
            public readonly static int ESCAPE_BYTE = 125;
            public readonly static int P_ACK = 67;
            public readonly static int P_REGISTRATION = 153;
            public readonly static int P_SERVICE_REQUEST = 255;
            public readonly static int P_SERVICE_RESPONSE = 0;
            public readonly static int P_SERVICE_REPORT_RATE = 254;
            public readonly static int P_SERVICE_UPDATE_THRESHOLD = 253;
            public readonly static int P_BLACKBOX_REQUEST = 208;
            public readonly static int P_BLACKBOX_RESPONSE = 209;
            public readonly static int P_TIME_SYNC = 160;
            public readonly static int P_UPDATE = 1;
            public readonly static int P_UPDATE_THRESHOLD = 2;
            public readonly static int P_STREAM_PACKET = 210;
            public readonly static int P_UNKNOWN = 255;
            public readonly static int MTU = 1100; //Largest Frame size specified in bytes (8 bits)
            public readonly static int ACK_TIMEOUT = 1000; // in milliseconds
            public readonly static int P_CONFIG_SIZE = 85;


            // Report Rate Types
            public readonly static int NORMAL_REPORT_RATE = 0;
            public readonly static int AFTER_THRESHOLD_REPORT_RATE = 1;

            // Threshold Types
            public readonly static int HIGH_THRESHOLD = 0;
            public readonly static int LOW_THRESHOLD = 1;


            //Black Box serial Commands
            public readonly static byte[] GET_BOARD_ID = { 2, 00 };
            public readonly static byte[] GET_ALL_CONFIG = { 50, 00 };
            public readonly static byte[] GET_TEMPERATURE = { 17, 01 };
            public readonly static byte[] GET_HUMIDITY = { 17, 02 };
            public readonly static byte[] GET_VIBRATION = { 17, 03 };
            public readonly static byte[] GET_SHOCK = { 17, 04 };
            public readonly static byte[] GET_NOTE = { 17, 05 };
            public readonly static byte[] GET_LOG = { 17, 06 };
            public readonly static byte[] GET_LIGHT = { 17, 07 };
            public readonly static byte[] GET_PRESSURE = { 17, 8 };
            public readonly static byte[] GET_FLASH = { 33, 00 };

            public readonly static byte[] SET_ALL_CONFIG = { 52, 00 };
            public readonly static byte[] SAVE_ALL_CONFIG = { 53, 00 };
            public readonly static byte[] SAVE_COMMENT = { 19, 00 };

            public readonly static byte[] RESET_BOARD = { 1, 0 };
            public readonly static byte[] RESET_CONFIG = { 54, 0 };
            public readonly static byte[] RESET_RADIO = { 81, 0 };

            public readonly static byte[] FORMAT_SD_CARD = { 18, 00 };
            public readonly static byte[] FORMAT_FLASH = { 34, 00 };

            public readonly static byte[] GET_BATTERY_LEVEL = { 96, 00 };

            public readonly static byte[] RUN_ACCELEROMETER_CALIBRATION = { 65, 00 };

            public readonly static byte[] START_MEASUREMENT = { 112, 00 };
            public readonly static byte[] END_MEASUREMENT = { 113, 00 };

            public readonly static byte[] RETURN_TO_SCHEDULE = { 114, 00 };
            public readonly static byte[] DELETE_SCHEDULE = { 115, 00 };

            public readonly static byte[] SCHEDULE_SERVICE = { 1 };
            public readonly static byte[] CONFIG_SERVICE = { 2 };
            /**
             * Scheduling Configuration Stream Packet index
             */
            public readonly static int TIME_DATA_SIZE = 4;
            public readonly static int STREAM_START_TIME_INDEX = 0;
            public readonly static int STREAM_END_TIME_INDEX = 4;
            public readonly static int STREAM_SWITCH_TIME_INDEX = 8;

            public readonly static int STREAM_TEMP_NORMAL_REPORT_RATE_INDEX = 12;
            public readonly static int STREAM_TEMP_OVER_THRESHOLD_REPORT_RATE_INDEX = 14;
            public readonly static int STREAM_TEMP_MAX_THRESHOLD_INDEX = 16;
            public readonly static int STREAM_TEMP_MIN_THRESHOLD_INDEX = 18;

            public readonly static int STREAM_HUMIDITY_NORMAL_REPORT_RATE_INDEX = 20;
            public readonly static int STREAM_HUMIDITY_OVER_THRESHOLD_REPORT_RATE_INDEX = 22;
            public readonly static int STREAM_HUMIDITY_MAX_THRESHOLD_INDEX = 24;
            public readonly static int STREAM_HUMIDITY_MIN_THRESHOLD_INDEX = 26;

            public readonly static int STREAM_VIB_NORMAL_REPORT_RATE_INDEX = 28;
            public readonly static int STREAM_VIB_OVER_THRESHOLD_REPORT_RATE_INDEX = 30;
            public readonly static int STREAM_VIB_MAX_THRESHOLD_INDEX = 32;

            public readonly static int STREAM_SHOCK_MAX_THRESHOLD_INDEX = 34;

            //Genereal Config Settings 
            public readonly static int GENERAL_SETTINGS_SIZE = 7;
            public readonly static int RADIO_RESTART_DELAY_INDEX = 0;
            public readonly static int RADIO_MAX_RETRIES_INDEX = 1;
            public readonly static int RADIO_MAX_FAILURES_INDEX = 2;
            public readonly static int RADIO_PANID_INDEX = 3;
            public readonly static int CONFIG_TYPE_INDEX = 5;
            public readonly static int DATA_IN_FLASH_INDEX = 6;

            //General Default Config Values
            public readonly static int DEFAULT_RADIO_RESTART_DELAY = 8;
            public readonly static int DEFAULT_RADIO_MAX_RETRIES = 3;
            public readonly static int DEFAULT_RADIO_MAX_FAILURES = 3;
            public readonly static int DEFAULT_RADIO_PANID = 65535;
            public readonly static int DEFAULT_CONFIG_TYPE = 0;
            public readonly static int DEFAULT_DATA_IN_FLASH = 0;

            //Default misc config
            public readonly static int CONFIG_VIB_SHOCk_BOTH_INDEX = 94;
            public readonly static int DEFAULT_CONFIG_VIB_SHOCk_BOTH = 1;


            /**
             *  Black Box Configuration Packet index
             */
            //Temperature Index
            public readonly static int TEMPERATURE_SETTINGS_SIZE = 10;
            public readonly static int TEMP_TIME_INDEX = 7;
            public readonly static int TEMP_TIME_OVER_THRES_INDEX = TEMP_TIME_INDEX + 2; //9
            public readonly static int TEMP_MAX_THRESHOLD_INDEX = TEMP_TIME_OVER_THRES_INDEX + 2;//11
            public readonly static int TEMP_MIN_THRESHOLD_INDEX = TEMP_MAX_THRESHOLD_INDEX + 2;//13
            public readonly static int TEMP_OVER_THRESHOLD_INDEX = TEMP_MIN_THRESHOLD_INDEX + 2;//15
            public readonly static int TEMP_UNDER_THRESHOLD_INDEX = TEMP_MIN_THRESHOLD_INDEX + 1; //16

            //Temperature default values
            public readonly static int DEFAULT_TEMP_TIME = 10;
            public readonly static int DEFAULT_TEMP_TIME_OVER_THRES = 5;
            public readonly static int DEFAULT_TEMP_MAX_THRES = 27419;
            public readonly static int DEFAULT_TEMP_MIN_THRES = 17473;
            public readonly static int DEFAULT_TEMP_OVER_THRESHOLD = 0;
            public readonly static int DEFAULT_TEMP_UNDER_THRESHOLD = 0;

            /*
            public readonly static int TEMP_UNITS = 8;
            public readonly static int TEMP_UNIT_MAXIMUM = 9;
            public readonly static int TEMP_UNIT_MINIMUM = 11;
            public readonly static int DEFAULT_TEMP_UNITS = 0;
            public readonly static int DEFAULT_TEMP_UNIT_MAXIMUM = 4320;
            public readonly static int DEFAULT_TEMP_UNIT_MINIMUM = 320;
            */
            //Humidity Index
            public readonly static int HUMIDITY_SETTINGS_SIZE = 10;
            public readonly static int HUMD_TIME_INDEX = 17;
            public readonly static int HUMD_TIME_OVER_THRES_INDEX = HUMD_TIME_INDEX + 2;//19
            public readonly static int HUMD_MAX_THRES_INDEX = HUMD_TIME_OVER_THRES_INDEX + 2;//21
            public readonly static int HUMD_MIN_THRES_INDEX = HUMD_MAX_THRES_INDEX + 2;//23
            public readonly static int HUMD_OVER_THRESHOLD_INDEX = HUMD_MIN_THRES_INDEX + 2;//25
            public readonly static int HUMD_UNDER_THRESHOLD_INDEX = HUMD_OVER_THRESHOLD_INDEX + 1; //26

            //Humidity default values
            public readonly static int DEFAULT_HUMD_TIME = 10;
            public readonly static int DEFAULT_HUMD_TIME_OVER_THRES = 5;
            public readonly static int DEFAULT_HUMD_MAX_THRES = 45088;
            public readonly static int DEFAULT_HUMD_MIN_THRES = 5767;
            public readonly static int DEFAULT_HUMD_OVER_THRESHOLD = 0;
            public readonly static int DEFAULT_HUMD_UNDER_THRESHOLD = 0;

            /*
            public readonly static int HUMD_UNITS = 21;
            public readonly static int HUMD_UNIT_MAXIMUM = 22;
            public readonly static int HUMD_UNIT_MINIMUM = 24;
            public readonly static int DEFAULT_HUMD_UNITS = 1;
            public readonly static int DEFAULT_HUMD_UNIT_MAXIMUM = 1984;
            public readonly static int DEFAULT_HUMD_UNIT_MINIMUM = 384;
            */
            //Vibration Config Index
            public readonly static int VIBRATION_SETTINGS_SIZE = 17;
            public readonly static int VIB_TIME_INDEX = 27;
            public readonly static int VIB_TIME_OVER_THRES_INDEX = VIB_TIME_INDEX + 2;//29
            public readonly static int VIB_MAX_THRESHOLD_INDEX = VIB_TIME_OVER_THRES_INDEX + 2;//31
            public readonly static int VIB_OVER_THRESHOLD_INDEX = VIB_MAX_THRESHOLD_INDEX + 2;//33
            public readonly static int VIB_X_OFFSET_INDEX = VIB_OVER_THRESHOLD_INDEX + 1;//34
            public readonly static int VIB_Y_OFFSET_INDEX = VIB_X_OFFSET_INDEX + 1;//35
            public readonly static int VIB_Z_OFFSET_INDEX = VIB_Y_OFFSET_INDEX + 1;//36
            public readonly static int VIB_INACTIVITY_THRESHOLD_INDEX = VIB_Z_OFFSET_INDEX + 1;//37
            public readonly static int VIB_INACTIVITY_TIME = VIB_INACTIVITY_THRESHOLD_INDEX + 1;//38
            public readonly static int VIB_TAP_THRESHOLD = VIB_INACTIVITY_TIME + 1;//39
            public readonly static int VIB_TAP_DURATION = VIB_TAP_THRESHOLD + 1;//40
            public readonly static int VIB_TAP_LATENCY = VIB_TAP_DURATION + 1;//41
            public readonly static int VIB_TAP_WINDOW = VIB_TAP_LATENCY + 1;//42

            //Vibration default values
            public readonly static int DEFAULT_VIB_TIME = 120;
            public readonly static int DEFAULT_VIB_TIME_OVER_THRES = 60;
            public readonly static int DEFAULT_VIB_MAX_THRESHOLD = 64;
            public readonly static int DEFAULT_VIB_OVER_THRESHOLD = 0;
            public readonly static int DEFAULT_VIB_X_OFFSET = 0;
            public readonly static int DEFAULT_VIB_Y_OFFSET = 0;
            public readonly static int DEFAULT_VIB_Z_OFFSET = 0;
            public readonly static int DEFAULT_VIB_INACTIVITY_THRES = 5;
            public readonly static int DEFAULT_VIB_INACTIVITY_TIME = 2;
            public readonly static int DEFAULT_VIB_TAP_THRESHOLD = 24;//0.5g
            public readonly static int DEFAULT_VIB_TAP_DURATION = 255;//159.375 ms
            public readonly static int DEFAULT_VIB_TAP_LATENCY = 12;//15
            public readonly static int DEFAULT_VIB_TAP_WINDOW = 255;//318.75

            /*
            public readonly static int VIB_X_UNIT_INDEX = 68;
            public readonly static int VIB_Y_UNIT_INDEX = 69;
            public readonly static int VIB_Z_UNIT_INDEX = 70;
            public readonly static int VIB_X_MAXIMUM_INDEX = 71;
            public readonly static int VIB_Y_MAXIMUM_INDEX = 73;
            public readonly static int VIB_Z_MAXIMUM_INDEX = 75;
            public readonly static int VIB_X_MINIMUM_INDEX = 77;
            public readonly static int VIB_Y_MINIMUM_INDEX = 79;
            public readonly static int VIB_Z_MINIMUM_INDEX = 81;
           */

            //Shock Config Index
            public readonly static int SHOCK_SETTINGS_SIZE = 9;
            public readonly static int SHOCK_TIME_INDEX = 43;
            public readonly static int SHOCK_OVER_THRESHOLD_TIME_INDEX = SHOCK_TIME_INDEX + 2;//45
            public readonly static int SHOCK_MAX_THRESHOLD_INDEX = SHOCK_OVER_THRESHOLD_TIME_INDEX + 2;//47
            public readonly static int SHOCK_FREE_FALL_THRESHOLD_INDEX = SHOCK_MAX_THRESHOLD_INDEX + 2;//49
            public readonly static int SHOCK_FREE_FALL_TIME_INDEX = SHOCK_FREE_FALL_THRESHOLD_INDEX + 1;//50
            public readonly static int SHOCK_ACTIVITY_THRESHOLD_INDEX = SHOCK_FREE_FALL_THRESHOLD_INDEX + 1;//51

            //Shock default values
            public readonly static int DEFAULT_SHOCK_TIME = 0;
            public readonly static int DEFAULT_SHOCK_OVER_THRESHOLD_TIME = 0;//45
            public readonly static int DEFAULT_SHOCK_MAX_THRESHOLD = 10;//47
            public readonly static int DEFAULT_SHOCK_FREE_FALL_THRESHOLD = 7;
            public readonly static int DEFAULT_SHOCK_FREE_FALL_TIME = 14;
            public readonly static int DEFAULT_SHOCK_ACTIVITY_THRESHOLD = 128;

            /*
            public readonly static int SHOCK_X_UNIT_INDEX = 32;
            public readonly static int SHOCK_Y_UNIT_INDEX = 33;
            public readonly static int SHOCK_Z_UNIT_INDEX = 34;
            public readonly static int SHOCK_X_MAXIMUM_INDEX = 35;
            public readonly static int SHOCK_Y_MAXIMUM_INDEX = 37;
            public readonly static int SHOCK_Z_MAXIMUM_INDEX = 39;
            public readonly static int SHOCK_X_MINIMUM_INDEX = 41;
            public readonly static int SHOCK_Y_MINIMUM_INDEX = 43;
            public readonly static int SHOCK_Z_MINIMUM_INDEX = 45;
            */
            //Light index values
            public readonly static int LIGHT_SETTINGS_SIZE = 12;
            public readonly static int LIGHT_REPORT_RATE_INDEX = 52;
            public readonly static int LIGHT_OVER_THRESHOLD_REPORT_RATE_INDEX = LIGHT_REPORT_RATE_INDEX + 2;//54
            public readonly static int LIGHT_HIGH_THRESHOLD_INDEX = LIGHT_OVER_THRESHOLD_REPORT_RATE_INDEX + 2;//56
            public readonly static int LIGHT_LOW_THRESHOLD_INDEX = LIGHT_HIGH_THRESHOLD_INDEX + 2;//58
            public readonly static int LIGHT_WAIT_TIME_INDEX = LIGHT_LOW_THRESHOLD_INDEX + 2;//60
            public readonly static int LIGHT_PRESISTENCE_INDEX = LIGHT_WAIT_TIME_INDEX + 1;//61
            public readonly static int LIGHT_OVER_THRESHOLD_INDEX = LIGHT_PRESISTENCE_INDEX + 1;//62
            public readonly static int LIGHT_UNDER_THRESHOLD_INDEX = LIGHT_OVER_THRESHOLD_INDEX + 1;//63

            //Light default values
            public readonly static int DEFAULT_LIGHT_REPORT_RATE = 30;
            public readonly static int DEFAULT_LIGHT_OVER_THRESHOLD_REPORT_RATE = 15;
            public readonly static int DEFAULT_LIGHT_HIGH_THRESHOLD = 32768;
            public readonly static int DEFAULT_LIGHT_LOW_THRESHOLD = 0;
            public readonly static int DEFAULT_LIGHT_WAIT_TIME = 171;
            public readonly static int DEFAULT_LIGHT_PRESISTENCE = 2;
            public readonly static int DEFAULT_LIGHT_OVER_THRESHOLD_INDEX = 0;
            public readonly static int DEFAULT_LIGHT_UNDER_THRESHOLD_INDEX = 0;

            //Pressure index values
            public readonly static int PRESSURE_SETTINGS_SIZE = 11;
            public readonly static int PRESSURE_REPORT_RATE_INDEX = 64;
            public readonly static int PRESSURE_OVER_THRESHOLD_REPORT_RATE_INDEX = PRESSURE_REPORT_RATE_INDEX + 2;//66
            public readonly static int PRESSURE_HIGH_THRESHOLD_INDEX = PRESSURE_OVER_THRESHOLD_REPORT_RATE_INDEX + 2;//68
            public readonly static int PRESSURE_CHANGE_INDEX = PRESSURE_HIGH_THRESHOLD_INDEX + 2;//70
            public readonly static int PRESSURE_TIME_STEP_INDEX = PRESSURE_CHANGE_INDEX + 2;//72
            public readonly static int PRESSURE_OVER_SAMPLE_RATE_INDEX = PRESSURE_TIME_STEP_INDEX + 1;//73
            public readonly static int PRESSURE_OVER_THRESHOLD_INDEX = PRESSURE_OVER_SAMPLE_RATE_INDEX + 1;//74

            //Pressure default values
            public readonly static int DEFAULT_PRESSURE_REPORT_RATE = 30;
            public readonly static int DEFAULT_PRESSURE_OVER_THRESHOLD_REPORT_RATE = 15;
            public readonly static int DEFAULT_PRESSURE_HIGH_THRESHOLD = 6000;
            public readonly static int DEFAULT_PRESSURE_CHANGE = 500;
            public readonly static int DEFAULT_PRESSURE_TIME_STEP = 4;
            public readonly static int DEFAULT_PRESSURE_OVER_SAMPLE_RATE = 7;
            public readonly static int DEFAULT_PRESSURE_OVER_THRESHOLD = 0;


            //Temperature Index
            public readonly static int TEMP2_TIME_INDEX = 75;
            public readonly static int TEMP2_TIME_OVER_THRES_INDEX = TEMP2_TIME_INDEX + 2; //77
            public readonly static int TEMP2_MAX_THRESHOLD_INDEX = TEMP2_TIME_OVER_THRES_INDEX + 2;//79
            public readonly static int TEMP2_MIN_THRESHOLD_INDEX = TEMP2_MAX_THRESHOLD_INDEX + 2;//81
            public readonly static int TEMP2_OVER_THRESHOLD_INDEX = TEMP2_MIN_THRESHOLD_INDEX + 2;//83
            public readonly static int TEMP2_UNDER_THRESHOLD_INDEX = TEMP2_MIN_THRESHOLD_INDEX + 1; //84

            //Temperature default values
            public readonly static int DEFAULT_TEMP2_TIME = 10;
            public readonly static int DEFAULT_TEMP2_TIME_OVER_THRES = 5;
            public readonly static int DEFAULT_TEMP2_MAX_THRES = 2453;
            public readonly static int DEFAULT_TEMP2_MIN_THRES = 1600;
            public readonly static int DEFAULT_TEMP2_OVER_THRESHOLD = 0;
            public readonly static int DEFAULT_TEMP2_UNDER_THRESHOLD = 0;


            //Humidity Index
            public readonly static int HUMD2_TIME_INDEX = 85;
            public readonly static int HUMD2_TIME_OVER_THRES_INDEX = HUMD2_TIME_INDEX + 2;//87
            public readonly static int HUMD2_MAX_THRES_INDEX = HUMD2_TIME_OVER_THRES_INDEX + 2;//89
            public readonly static int HUMD2_MIN_THRES_INDEX = HUMD2_MAX_THRES_INDEX + 2;//91
            public readonly static int HUMD2_OVER_THRESHOLD_INDEX = HUMD2_MIN_THRES_INDEX + 2;//93
            public readonly static int HUMD2_UNDER_THRESHOLD_INDEX = HUMD2_OVER_THRESHOLD_INDEX + 1; //94

            //Humidity default values
            public readonly static int DEFAULT_HUMD2_TIME = 10;
            public readonly static int DEFAULT_HUMD2_TIME_OVER_THRES = 5;
            public readonly static int DEFAULT_HUMD2_MAX_THRES = 1664;
            public readonly static int DEFAULT_HUMD2_MIN_THRES = 464;
            public readonly static int DEFAULT_HUMD2_OVER_THRESHOLD = 0;
            public readonly static int DEFAULT_HUMD2_UNDER_THRESHOLD = 0;
       
    }
}