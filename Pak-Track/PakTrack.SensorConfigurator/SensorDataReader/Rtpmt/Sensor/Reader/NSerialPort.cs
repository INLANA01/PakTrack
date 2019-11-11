// Custom Serial Port class

//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading;

//namespace PakTrack.SensorsConfigurator.SensorDataReader.Rtpmt.Sensor.Reader
//{
//    class NSerialPort
//    {

//        private readonly BufferedInputStream input;
//        private readonly BufferedOutputStream output;

//        public NSerialPort(InputStream _input, OutputStream _output)
//        {
//            this.input = new BufferedInputStream(_input, 4096);
//            this.output = new BufferedOutputStream(_output);
//        }

//        //@Override
//        public void open()
//        {

//        }

//        //@Override
//        public void close() //throws IOException
//        {
//            input.close();
//            output.close();
//        }

//        //@Override
//        public byte read() //throws IOException
//        {
//            byte data = 0;
//            int tempData = 0;
//            int counter = 0;
//            tempData = input.read();
//            while (tempData == -1)
//            {
//                try
//                {
//                    Thread.Sleep(10);
//                    counter++;
//                    if (counter > 100)
//                    {
//                        return Convert.ToByte(-1);
//                    }
//                }
//                catch (Exception ex)
//                { // InterruptedException
//                    //Logger.getLogger(SerialPort.class.getName()).log(Level.SEVERE, null, ex);
//                    Console.WriteLine(ex.ToString());
//                }
//                tempData = input.read();
//            }

//            data = (byte)(tempData & 0xff);
//            return data;
//        }

//        //@Override
//        public bool write(byte data) //throws IOException
//        {
//            output.write(data);
//            return true;
//        }

//        //@Override
//        public bool write(byte[] data) //throws IOException
//        {
//            output.write(data);
//            return true;
//        }

//        //@Override
//        public void flush()
//        {
//            try
//            {
//                output.flush();
//            }
//            catch (Exception ex) //IOexception
//            {
//                //Logger.getLogger(SerialPort.class.getName()).log(Level.SEVERE, null, ex);
//                Console.WriteLine(ex.ToString());
//            }
//        }

//        //@Override
//        public bool isAvailable() //throws IOException
//        {

//            return input.available() > 0;

//        }

//        //@Override
//        public byte[] readAll() //throws IOException
//        {
//            int available = input.available();

//            byte[] byteArray = new byte[available];
//            input.read(byteArray);

//            return byteArray;

//        }

//    }

//}
