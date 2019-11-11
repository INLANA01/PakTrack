using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PakTrack.Utilities;

namespace PakTrack.Models.Sensor
{
    public class Packet
    {
        private readonly int _packetNumber;
        public int ProtocolType;

        //This 2 byte indicates the Node Id
        public int NodeId;


        //service
        public int Service;

        //service Id
        public int ServiceId;
        private readonly List<byte> _payLoad;
        //static DateFormat dateFormat = new SimpleDateFormat("yyyy/MM/dd HH:mm:ss");
        private DateTime _date  = DateTime.Now;

        private readonly int _utcTimeStampSeconds = (int)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;

        public Packet()
        {
            _payLoad = new List<byte>();
        }

        public long GetTimeStampMilliSeconds()
        {
            return _date.Ticks;
        }

        public void addDataToPacket(byte value)
        {
            _payLoad.Add(value);
        }


        public int GetPacketNumber()
        {
            return this._packetNumber;
        }

        public List<byte> GetPayload()
        {
            return this._payLoad;
        }

        public DateTime getDate()
        {
            return _date;
        }

        public void setDate(DateTime date)
        {
            this._date = date;
        }

        public Packet(byte[] byteArray)
        {

            _payLoad = new List<byte>();

            this.ProtocolType = byteArray[0] & 0xff;

            int i = 5;
            this.NodeId = (byteArray[1] & 0xff) << 8 | (byteArray[2] & 0xff);

            int payLoadLength;
            /*
             Commenting the condition as per new protocol changes
             */
            //to handle black box response packet
            if (this.ProtocolType != SensorConstants.P_BLACKBOX_RESPONSE)
            {

                this.Service = byteArray[5];
                this.ServiceId = byteArray[6];
                //start from 7th position
                i = 7;
                //now payloadlength is -4 used for timestamp
                payLoadLength = byteArray.Length - 4;
            }

            else
            {
                this.Service = byteArray[5];
                this.ServiceId = byteArray[6];
                //start from 7th position
                i = 7;

                payLoadLength = byteArray.Length;
            }

            for (; i < payLoadLength; i++)
            {
                this.AddDataToPacket(byteArray[i]);
            }

            if (ProtocolType != SensorConstants.P_BLACKBOX_RESPONSE)
            {
                this._utcTimeStampSeconds = (byteArray[i +3] & 0xff) | (byteArray[i + 2] & 0xff) << 8
                    | (byteArray[i + 1] & 0xff) << 16 | (byteArray[i] & 0xff) << 24;
                this._date = getDateTime(_utcTimeStampSeconds);
            }
            if (isVibration() || isShock())
            {
                _packetNumber = (_payLoad[0] & 0xff) >> 4;
            }
        }

        private DateTime getDateTime(int timestamp)
        {
            var dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            return dtDateTime.AddSeconds(timestamp).ToLocalTime();
        }

        public void AddDataToPacket(byte value)
        {
            _payLoad.Add(value);
        }

        public byte[] GetData()
        {
            return _payLoad.ToArray();
        }
        public byte getData(int index)
        {

            return _payLoad[index];

        }
        public int GetBatteryLevel()
        {

            var batteryLevel = 0;
            if (this._payLoad != null && this._payLoad.Count > 1)
            {
                batteryLevel = (_payLoad[0] & 0xff)
                               | (_payLoad[1] & 0xff) << 8;
            }
            return batteryLevel;
        }

        public string SensorId()
        {
            var sensorId = new StringBuilder();

            if (this.ProtocolType == SensorConstants.P_BLACKBOX_RESPONSE)
            {
                for (int i = 0; i < 8; i++)
                {
                    sensorId.Append(((byte)this._payLoad[i] & 0xff).ToString("x").ToUpper());
                }
                return sensorId.ToString();
            }

            return "00";
        }

        public bool IsPartialPacket()
        {

            return this.isVibration() || this.isShock();
        }


        public bool isVibration()
        {
            return this.Service == 2;
        }

        public bool isShock()
        {
            return this.Service == 3;
        }

        public string uniqueId()
        {
            return (this.GetTimeStamp()) + this.NodeId + this.Service + this.ServiceId;
        }

        public string GetTimeStamp()
        {

            return _date.ToString("yyyy/MM/dd HH:mm:ss");
        }

        public bool isTemperature()
        {
            return this.Service == 0;
        }

        public bool isHumidty()
        {
            return this.Service == 1;
        }

        /**
         *
         * @return
         */

        public bool IsX()
        {

            bool isTrue;

            if (this.Service == 3 || this.Service == 2)
            {
                isTrue = this.ServiceId == 1;
            }
            else
            {
                isTrue = false;
            }
            return isTrue;
        }

        /**
         *
         * @return
         */

        public bool IsY()
        {

            bool isTrue;

            if (this.Service == 3 || this.Service == 2)
            {
                isTrue = this.ServiceId == 2;
            }
            else
            {
                isTrue = false;
            }
            return isTrue;
        }

        /**
         *
         * @return
         */

        public bool IsZ()
        {

            bool isTrue;

            if (this.Service == 3 || this.Service == 2)
            {
                isTrue = this.ServiceId == 3;
            }
            else
            {
                isTrue = false;
            }
            return isTrue;
        }

        public bool isLight()
        {
            return this.Service == 4;
        }

        public bool isPressure()
        {
            return this.Service == 5;
        }


        public double GetTemperature()
        {

            int value = (_payLoad[0] & 0xff) << 8 | (_payLoad[1] & 0xff);

            return truncate(SensorUtils.GetTemperature(value), 4);

        }

        private double truncate(double number, int precision)
        {
            var prec = Math.Pow(10, precision);
            var integerPart = (int)number;
            var fractionalPart = number - integerPart;
            fractionalPart *= prec;
            var fractPart = (int)fractionalPart;
            fractionalPart = integerPart + fractPart / prec;
            return fractionalPart;
        }

        /**
           * get the value from the packet received from the sensor will return only
           * if it is update packet otherwise -1
           *
           * @return
           */

        public double GetHumidity()
        {
            int value = (_payLoad[1] & 0xff)
                        | (_payLoad[0] & 0xff) << 8;

            double humdity = SensorUtils.GetHumidity(value);

            return truncate(humdity, 4);
        }

        public IEnumerable<double> GetVibration()
        {

            var vibration = new List<double>();
            double g;

            for (var i = 1; i < _payLoad.Count; i++)
            {
                short value = (sbyte)_payLoad[i];
                g = SensorUtils.GetVibration(value);
                vibration.Add(truncate(g, 4));
            }
            return vibration;
        }

        public IEnumerable<double> GetShock()
        {

            var shock = new List<double>();
            double g;
            for (var i = 1; i < _payLoad.Count; i++)
            {
                //Unsigned integer value, note the 0xffff not 0xff
                var value = (short)(_payLoad[i] & 0xFF);
                g = SensorUtils.GetShock(value);
                shock.Add(truncate(g, 4));
            }
            return shock;
        }


        public double GetIlluminance()
        {
            long unsignedRed = 0, unsignedGreen = 0, unsignedBlue = 0;
            if (this._payLoad != null && this._payLoad.Count > 1)
            {
                var r = (_payLoad[2] & 0xff) << 8 | (_payLoad[3] & 0xff);
                unsignedRed = r & 0xffffL;
                var g = (_payLoad[4] & 0xff) << 8 | (_payLoad[5] & 0xff);
                unsignedGreen = g & 0xffffL;
                var b = (_payLoad[6] & 0xff) << 8 | (_payLoad[7] & 0xff);
                unsignedBlue = b & 0xffffL;
            }
            return truncate(SensorUtils.GetIlluminace(unsignedRed, unsignedGreen, unsignedBlue), 4);
        }


        public double GetR()
        {
            long unsignedRed = 0;
            if (this._payLoad != null && this._payLoad.Count > 1)
            {
                var r = (_payLoad[2] & 0xff) << 8 | (_payLoad[3] & 0xff);
                unsignedRed = r & 0xffffL;
            }
            return unsignedRed;
        }
        public bool IsAboveThreshold()
        {
            return this.ProtocolType == SensorConstants.P_UPDATE_THRESHOLD;
        }
        public double GetG()
        {
            long unsignedGreen = 0;
            if (this._payLoad != null && this._payLoad.Count > 1)
            {
                var g = (_payLoad[4] & 0xff) << 8 | (_payLoad[5] & 0xff);
                unsignedGreen = g & 0xffffL;
            }
            return unsignedGreen;
        }

        public double GetB()
        {
            long unsignedBlue = 0;
            if (this._payLoad != null && this._payLoad.Count > 1)
            {
                var b = (_payLoad[6] & 0xff) << 8 | (_payLoad[7] & 0xff);
                unsignedBlue = b & 0xffffL;
            }
            return unsignedBlue;
        }

        public double GetClearLight()
        {
            var c = 0;
            long unsignedClear = 0;
            if (this._payLoad != null && this._payLoad.Count > 1)
            {
                c = (_payLoad[0] & 0xff) << 8 | (_payLoad[1] & 0xff);
                unsignedClear = c & 0xffffL;
            }
            return unsignedClear;
        }

        public double GetPressure()
        {
            int pressure = 0;
            if (this._payLoad != null && this._payLoad.Count > 1)
            {
                pressure = (_payLoad[0] & 0xff) << 16
                           | (_payLoad[1] & 0xff) << 8
                           | (_payLoad[2] & 0xff);
            }
            return truncate(SensorUtils.GetPressure(pressure), 4);
        }

        public bool  IsInstanteaous()
        {
            return IsBitSet(_payLoad[0], 2);
        }

        public bool IsFreeFall()
        {
            return IsBitSet(_payLoad[0], 3);
        }

            /**
         *
         * @param byteValue
         * @param N
         * @return
         */
        private static bool IsBitSet(byte byteValue, int n)
        {

            return ((byteValue >> n) & 1) == 1;

        }
    }
}
