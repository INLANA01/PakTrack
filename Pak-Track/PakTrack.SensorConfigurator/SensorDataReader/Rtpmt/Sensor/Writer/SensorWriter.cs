using System.IO.Ports;
using PakTrack.SensorsConfigurator.SensorDataReader.Rtpmt.Motes.Packet;
using PakTrack.SensorsConfigurator.SensorDataReader.Rtpmt.Sensor.Util;

namespace  PakTrack.SensorsConfigurator.SensorDataReader.Rtpmt.Sensor.Writer
{
    public class SensorWriter
    {
        private SerialPort port;

        public SensorWriter(SerialPort _outputPort)
        {
            this.port = _outputPort;
        }

        public bool WriteFramedPacket(byte[] command)
        {
            RawPacket buffer = new RawPacket(command.Length + 7);
            int packetType = Constants.P_BLACKBOX_REQUEST;
            buffer.NextByte(packetType);

            //Node Id is 16 bit
            int nodeId = 0;
            buffer.NextByte(nodeId >> 8);
            buffer.NextByte(nodeId & 0xff);

            //+2 for crc //length 
            int length = command.Length + 2;
            buffer.NextByte(length >> 8);
            buffer.NextByte(length & 0xff);

            //Add command
            for (int i = 0; i<command.Length; i++)
            {
                buffer.NextByte(command[i]);
            }

            //Add values
            buffer.Terminate();

            return _Write(buffer);
        }

        private bool _Write(RawPacket buffer)
        {
            byte[] realPacket = new byte[buffer.escapePtr];
            System.Array.Copy(buffer.escaped, 0, realPacket, 0, buffer.escapePtr);

            //port.flush();
            bool write = false; 
            port.Write(realPacket,0,realPacket.Length); //does not return boolean like java 
            write = true;
            //port.flush();
            //if (Constants.DEBUG) {
            //    Dump.dump(System.err, "Sending :", realPacket);
            //    System.err.println();
            //}
            return write;
        }
    }
}