using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;

namespace PakTrack.Utilities
{
    public static class SensorUtils
    {


        //compare two integer arrays and return boolean
        public static bool Compare(byte[] first, int[] second)
        {
            var isEqual = true;

            if (first.Length > 0 && second.Length > 0 && first.Length == second.Length)
            {
                if (second.Where((t, i) => (first[i] & 0xff) != t).Any())
                {
                    isEqual = false;
                }
            }
            else
            {
                isEqual = false;
            }
            return isEqual;
        }


        public static int GetTemperature(double value)
        {
            value -= 32;
            value /= 1.8;
            value += 46.85;
            value = value * (65536.0 / 175.72);
            return (int)(value);
        }

        public static double GetTemperature(int value)
        {

            return (value * 175.72 / 65536.0 - 46.85) * 1.8 + 32.0;
        }

        public static int GetHumidity(double value)
        {
            return (int)((value + 6) * (65536.0 / 125.0));
        }

        public static double GetHumidity(int value)
        {

            return (((value * 125.0) / 65536.0) - 6);
        }

        //RMS Threshold calculation is different, refer the conversion formulae document
        public static int GetVibrationThreshold(double value)
        {
            return (int)(value / 0.015625);
        }

        public static double GetVibrationThreshold(int value)
        {
            return value * 0.015625;
        }

        public static int GetVibration(double value)
        {
            return (int)(value / 0.03125);
        }

        public static double GetVibration(int value)
        {
            return (value * 0.03125);
        }

        public static int GetShock(double value)
        {
            return (int)((value / 0.64) + 128);
        }

        public static double GetShock(int value)
        {
            return (value - 128) * 0.64;
        }

        public static int GetPressure(double value)
        {
            return (int)(value / 2);
        }

        public static double GetPressure(int value)
        {
            return ((double)value / 64);
        }

        public static double GetPressureThreshold(int value)
        {
            return ((double)value * 2);
        }

        public static double GetClearLight(int value)
        {
            return value;
        }

        public static int GetClearLight(double value)
        {
            return (int)value;
        }

        public static int GetIlluminance(double value)
        {
            return (int)(value / 0.64);
        }

        public static double GetIlluminace(long r, long g, long b)
        {
            return (-0.32466 * r) + (1.57837 * g) + (-0.73191 * b);
        }

        public static byte[] ToByteArray(byte[] byteArr)
        {

            var newArr = new byte[byteArr.Length];

            for (var i = 0; i < byteArr.Length; i++)
            {
                newArr[i] = byteArr[i];
            }
            return newArr;
        }

        public static string GetCorrectPort()
        {
            const string comName = SensorConstants.SensorDiscoveryName;
            //COM6 - TI MSP430 USB (COM6)
            using (var searcher = new ManagementObjectSearcher
               ("SELECT * FROM WIN32_SerialPort Where Caption Like '%" + comName + "%'"))
            {
                var ports = searcher.Get().Cast<ManagementBaseObject>().ToList();
                return ports.Any() ? ports.First()["DeviceID"].ToString() : string.Empty;
            }
        }

        public static Tuple<int, double> GetMax(List<double> list)
        {
            var max = list[0];
            var index = 0;
            for (var i = 1; i < list.Count; i++)
            {

                if (!(max < list[i])) continue;
                max = list[i];
                index = i;
            }
            return new Tuple<int, double>(index, max);
        }

        public static Tuple<int, double> GetAbsoluteMax(List<double> list)
        {
            var max = list[0];
            var index = 0;
            for (var i = 1; i < list.Count; i++)
            {
                if (!(Math.Abs(max) < Math.Abs(list[i]))) continue;
                max = list[i];
                index = i;
            }
            return new Tuple<int, double>(index, max);
        }

        public static double GetAverage(List<double> list)
        {
            var count = list.Count;
            return list.Sum() / count;
        }

        public static IEnumerable<double> GetVector(IEnumerable<double> list1, IEnumerable<double> list2,
            IEnumerable<double> list3)
        {
            return list1.Zip(list2, (l1, l2) => l1 * l1 + l2 * l2)
                .Zip(list3,(l2,l3) => Math.Sqrt(l2*l2 + l3*l3));
        }
    }

}
