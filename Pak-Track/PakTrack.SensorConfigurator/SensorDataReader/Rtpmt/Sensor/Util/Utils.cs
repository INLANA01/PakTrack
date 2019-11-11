using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PakTrack.SensorsConfigurator.SensorDataReader.Rtpmt.Sensor.Util
{
    public class Utils
    {

        //compare two integer arrays and return boolean
        public static bool compare(byte[] first, int[] second)
        {
            bool isEqual = true;

            if (first.Length > 0 && second.Length > 0 && first.Length == second.Length)
            {

                for (int i = 0; i < second.Length; i++)
                {
                    if ((first[i] & 0xff) != second[i])
                    {
                        isEqual = false;
                        break;
                    }
                }
            }
            else
            {
                isEqual = false;
            }
            return isEqual;
        }


        public static int getTemperature(double value)
        {
            value -= 32;
            value /= 1.8;
            value += 46.85;
            value = value * (65536.0 / 175.72);
            return (int)(value);
        }

        public static double getTemperature(int value)
        {

            return ((value * 175.72) / 65536.0 - 46.85) * 1.8 + 32.0;
        }

        public static int getHumidity(double value)
        {
            return (int)((value + 6) * (65536.0 / 125.0));
        }

        public static double getHumidity(int value)
        {

            return (((value * 125.0) / 65536.0) - 6);
        }

        //RMS Threshold calculation is different, refer the conversion formulae document
        public static int getVibrationThreshold(double value)
        {
            return (int)(value / 0.015625);
        }

        public static double getVibrationThreshold(int value)
        {
            return (value * 0.015625);
        }

        public static int getVibration(double value)
        {
            return (int)(value / 0.03125);
        }

        public static double getVibration(int value)
        {
            return (value * 0.03125);
        }

        public static int getShock(double value)
        {
            return (int)((value / 0.64) + 128);
        }

        public static double getShock(int value)
        {
            return ((value - 128) * 0.64);
        }

        public static int getPressure(double value)
        {
            return (int)(value / 2);
        }

        public static double getPressure(int value)
        {
            return (value / 64);
        }

        public static double getClearLight(int value)
        {
            return (double)value;
        }

        public static int getClearLight(double value)
        {
            return (int)value;
        }

        public static int getIlluminance(double value)
        {
            return (int)(value / 0.64);
        }

        public static double getIlluminace(long r, long g, long b)
        {
            return (-0.32466 * r) + (1.57837 * g) + (-0.73191 * b);
        }

        public static byte[] toByteArray(byte[] byteArr)
        {

            byte[] newArr = new byte[byteArr.Length];

            for (int i = 0; i < byteArr.Length; i++)
            {
                newArr[i] = byteArr[i];
            }
            return newArr;
        }
    }
}
