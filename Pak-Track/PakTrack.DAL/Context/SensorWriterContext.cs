using System;
using System.IO;
using System.IO.Ports;
using PakTrack.Utilities;

namespace PakTrack.DAL.Context
{
    public class SensorWriterContext
    {
        private readonly SerialPort _port;
        private readonly BufferedStream _outWriterStream;
        public SensorWriterContext(SerialPort outputPort)
        {
            _port = outputPort;
            _outWriterStream = new BufferedStream(_port.BaseStream);
        }

        public bool WriteFramedPacket(byte[] command, byte[] packet = null)
        {
            var commandPackLength = command.Length + packet?.Length ?? command.Length; 
            var buffer = new RawPacket(commandPackLength + 7);
            const int packetType = SensorConstants.P_BLACKBOX_REQUEST;
            buffer.NextByte(packetType);

            //Node Id is 16 bit
            const int nodeId = 0;
            buffer.NextByte(nodeId >> 8);
            buffer.NextByte(nodeId & 0xff);

            //+2 for crc //length 
            var length = commandPackLength + 2;
            buffer.NextByte(length >> 8);
            buffer.NextByte(length & 0xff);

            //Add command
            foreach (var b in command)
            {
                buffer.NextByte(b);
            }

            //Add values
            if (packet != null)
            {
                foreach (var p in packet)
                {
                    buffer.NextByte(p);
                }
            }
            buffer.Terminate();

            return _Write(buffer);
        }


        public bool WriteFramedPacket(int packetType, byte[] packet = null)  {
            var commandPackLength =  packet?.Length ?? 0;
            var buffer = new RawPacket(commandPackLength + 7);

            buffer.NextByte(packetType);

            //Node Id is 16 bit
            const int nodeId = 0;
            buffer.NextByte(nodeId >> 8);
            buffer.NextByte(nodeId & 0xff);

            //+2 for crc
            int length = commandPackLength + 2;
            //length 
            buffer.NextByte(length >> 8);
            buffer.NextByte(length & 0xff);
            if (packet != null)
            {
                for (var i = 0; i < length - 2; i++)
                {
                    buffer.NextByte(packet[i]);
                }
            }

            buffer.Terminate();

            return _Write(buffer);
        }

      
        private bool _Write(RawPacket buffer)
        {
            var realPacket = new byte[buffer.escapePtr];
            Array.Copy(buffer.escaped, 0, realPacket, 0, buffer.escapePtr);
            _outWriterStream.Write(realPacket, 0, realPacket.Length); //does not return boolean like java 
            _outWriterStream.Flush();
            System.Threading.Thread.Sleep(50);
            return true;
        }

        public bool WriteStreamedPacket(byte[] command, byte[] packet) {

            var buffer = new RawPacket(packet.Length + command.Length + 8);
            const int packetType = SensorConstants.P_STREAM_PACKET;
            buffer.NextByte(packetType);

            //Node Id is 16 bit
            const int nodeId = 0;
            buffer.NextByte(nodeId >> 8);
            buffer.NextByte(nodeId & 0xff);

            //+1 for stream service //length 
            var length = packet.Length + 1;
            buffer.NextByte(length >> 8);
            buffer.NextByte(length & 0xff);

            //Add command
            foreach (var b in command)
            {
                buffer.NextByte(b);
            }

            //Add values
            foreach (var p in packet)
            {
                buffer.NextByte(p);
            }
            

            return _Write(buffer);

        }
    }
}
