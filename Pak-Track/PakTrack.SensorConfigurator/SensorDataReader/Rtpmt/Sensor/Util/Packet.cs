using System;
using System.Collections.Generic;
using System.Text;

namespace PakTrack.SensorsConfigurator.SensorDataReader.Rtpmt.Sensor.Util
{
    public class Packet
    {
        private int packetNumber;
        public int ProtocolType;

        //This 2 byte indicates the Node Id
        public int NodeId;

        //Payload Length
        public int PayloadLength;

        //service
        public int Service;

        //service Id
        public int ServiceId;
        private List<byte> PayLoad;
        //static DateFormat dateFormat = new SimpleDateFormat("yyyy/MM/dd HH:mm:ss");
        private DateTime date;

        public Packet()
        {
            PayLoad = new List<byte>();
            date = new DateTime();
        }

        public void addDataToPacket(byte value)
        {
            PayLoad.Add(value);
        }


        public int getPacketNumber()
        {
            return this.packetNumber;
        }

        public List<byte> getPayload()
        {
            return this.PayLoad;
        }

        public DateTime getDate()
        {
            return date;
        }

        public void setDate(DateTime date)
        {
            this.date = date;
        }

        public Packet(byte[] byteArray)
        {

            PayLoad = new List<byte>();

            this.ProtocolType = byteArray[0] & 0xff;

            int i = 5;
            this.NodeId = (byteArray[1] & 0xff) << 8 | (byteArray[2] & 0xff);
            this.PayloadLength = (byteArray[3] & 0xff << 8) | (byteArray[4] & 0xff);

            int payLoadLength = byteArray.Length;
            /*
             Commenting the condition as per new protocol changes
             */
            //to handle black box response packet
            if (this.ProtocolType != Constants.P_BLACKBOX_RESPONSE)
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

            //if (this.ProtocolType != Constants.P_BLACKBOX_RESPONSE)
            //{
            //    ByteBuffer bb = ByteBuffer.wrap(byteArray, i, 4);
            //    long timeStamp = (bb.getInt() * (long)1000);
            //    date = new Date(timeStamp);
            //}
            //if (this.isVibration() || this.isShock())
            //{
            //    packetNumber = (PayLoad.get(0) & 0xff) >> 4;
            //}
        }

        public void AddDataToPacket(byte value)
        {
            PayLoad.Add(value);
        }

        public byte[] GetData()
        {
            return PayLoad.ToArray();
        }

        public int GetBatteryLevel()
        {

            int batteryLevel = 0;
            if (this.PayLoad != null && this.PayLoad.Count > 1)
            {
                batteryLevel = (PayLoad[0] & 0xff)
                               | (PayLoad[1] & 0xff) << 8;
            }
            return batteryLevel;
        }

        public string SensorId()
        {
            StringBuilder sensorId = new StringBuilder();

            if (this.ProtocolType == Constants.P_BLACKBOX_RESPONSE)
            {
                for (int i = 0; i < 8; i++)
                {
                    sensorId.Append(((byte) this.PayLoad[i] & 0xff).ToString("x").ToUpper());
                }
                return sensorId.ToString();
                /*
                 return ((((long)this.PayLoad.get(0) & 0xff) << 56) |
                 (((long)this.PayLoad.get(1) & 0xff) << 48) |
                 (((long)this.PayLoad.get(2) & 0xff) << 40) |
                 (((long)this.PayLoad.get(3) & 0xff) << 32) |
                 (((long)this.PayLoad.get(4) & 0xff) << 24) |
                 (((long)this.PayLoad.get(5) & 0xff) << 16) |
                 (((long)this.PayLoad.get(6) & 0xff) <<  8) |
                 (((long)this.PayLoad.get(7) & 0xff)));
                 */
            }

            else
            {
                return "00";
            }
        }

        public bool isPartialPacket()
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
            return (this.getTimeStamp()) + this.NodeId + this.Service + this.ServiceId;
        }

        public string getTimeStamp()
        {

            return date.ToString("yyyy/MM/dd HH:mm:ss");
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

        public bool isX()
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

        public bool isY()
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

        public bool isZ()
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


        public double getTemperature()
        {

            int value = (PayLoad[0] & 0xff) << 8 | (PayLoad[1] & 0xff);

            return truncate(Utils.getTemperature(value), 4);

        }

        public double truncate(double number, int precision)
        {
            double prec = Math.Pow(10, precision);
            int integerPart = (int) number;
            double fractionalPart = number - integerPart;
            fractionalPart *= prec;
            int fractPart = (int) fractionalPart;
            fractionalPart = (double) (integerPart) + (double) (fractPart) / prec;
            return fractionalPart;
        }

        /**
           * get the value from the packet received from the sensor will return only
           * if it is update packet otherwise -1
           *
           * @return
           */

        public double getHumidity()
        {
            int value = (PayLoad[1] & 0xff)
                        | (PayLoad[0] & 0xff) << 8;

            double humdity = Utils.getHumidity(value);

            return truncate(humdity, 4);
        }

        public String getVibration()
        {

            StringBuilder vibration = new StringBuilder();
            double g;

            for (int i = 1; i < PayLoad.Count; i++)
            {
                short value = PayLoad[i];
                g = Utils.getVibration(value);
                vibration.Append(truncate(g, 4) + " ");
            }
            vibration.Remove(vibration.Length - 1, 1);

            return vibration.ToString();
        }

        public String getShock()
        {

            StringBuilder shock = new StringBuilder();
            double g;

            for (int i = 1; i < PayLoad.Count; i++)
            {
                //Unsigned integer value, note the 0xffff not 0xff
                short value = (short) (PayLoad[i] & 0xFF);
                g = Utils.getShock(value);
                shock.Append(truncate(g, 4) + " ");
            }

            shock.Remove(shock.Length - 1, 1);
            return shock.ToString();

        }


        private double getIlluminance()
        {
            int c = 0, r = 0, g = 0, b = 0;
            long unsignedClear = 0, unsignedRed = 0, unsignedGreen = 0, unsignedBlue = 0;
            if (this.PayLoad != null && this.PayLoad.Count > 1)
            {
                c = (PayLoad[0] & 0xff) << 8 | (PayLoad[1] & 0xff);
                unsignedClear = c & 0xffffl;
                r = (PayLoad[2] & 0xff) << 8 | (PayLoad[3] & 0xff);
                unsignedRed = r & 0xffffl;
                g = (PayLoad[4] & 0xff) << 8 | (PayLoad[5] & 0xff);
                unsignedGreen = g & 0xffffl;
                b = (PayLoad[6] & 0xff) << 8 | (PayLoad[7] & 0xff);
                unsignedBlue = b & 0xffffl;
            }
            return truncate(Utils.getIlluminace(unsignedRed, unsignedGreen, unsignedBlue), 4);
        }


        private double getR()
        {
            int r = 0;
            long unsignedRed = 0;
            if (this.PayLoad != null && this.PayLoad.Count > 1)
            {
                r = (PayLoad[2] & 0xff) << 8 | (PayLoad[3] & 0xff);
                unsignedRed = r & 0xffffl;
            }
            return unsignedRed;
        }

        private double getG()
        {
            int g = 0;
            long unsignedGreen = 0;
            if (this.PayLoad != null && this.PayLoad.Count > 1)
            {
                g = (PayLoad[4] & 0xff) << 8 | (PayLoad[5] & 0xff);
                unsignedGreen = g & 0xffffl;
            }
            return unsignedGreen;
        }

        private double getB()
        {
            int b = 0;
            long unsignedBlue = 0;
            if (this.PayLoad != null && this.PayLoad.Count > 1)
            {
                b = (PayLoad[6] & 0xff) << 8 | (PayLoad[7] & 0xff);
                unsignedBlue = b & 0xffffl;
            }
            return unsignedBlue;
        }

        private double getClearLight()
        {
            int c = 0;
            long unsignedClear = 0;
            if (this.PayLoad != null && this.PayLoad.Count > 1)
            {
                c = (PayLoad[0] & 0xff) << 8 | (PayLoad[1] & 0xff);
                unsignedClear = c & 0xffffl;
            }
            return unsignedClear;
        }

        private double getPressure()
        {
            int pressure = 0;
            if (this.PayLoad != null && this.PayLoad.Count > 1)
            {
                pressure = (PayLoad[0] & 0xff) << 16
                           | (PayLoad[1] & 0xff) << 8
                           | (PayLoad[2] & 0xff);
            }
            return truncate(Utils.getPressure(pressure), 4);
        }

        public void getSensorMessage()
        {

            if (this.isTemperature())
            {
                Console.WriteLine("Temperature is " + this.getTemperature() + " F");

            }
            else if (this.isHumidty())
            {
                Console.WriteLine("Humidity is " + this.getHumidity() + " KPa");

            }
            else if (this.isVibration())
            {

                Console.WriteLine("Vibration is  " + this.getVibration() + " g");

            }
            else if (this.isShock())
            {



                Console.WriteLine("Shock is  " + this.getShock() + " g");
            }
            else if (this.isLight())
            {

                Console.WriteLine("R is  " + this.getR());
                Console.WriteLine("G is  " + this.getG());
                Console.WriteLine("B is  " + this.getB());
                Console.WriteLine("ClearLight is  " + this.getClearLight());
                Console.WriteLine("Luminance is  " + this.getIlluminance());
            }
            else if (this.isPressure())
            {
                Console.WriteLine("Pressure is  " + this.getPressure());
            }
        }
    }
}