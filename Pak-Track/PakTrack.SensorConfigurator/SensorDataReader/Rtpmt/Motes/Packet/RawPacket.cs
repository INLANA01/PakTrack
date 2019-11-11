using PakTrack.SensorsConfigurator.SensorDataReader.Rtpmt.Sensor.Util;

namespace  PakTrack.SensorsConfigurator.SensorDataReader.Rtpmt.Motes.Packet
{
    public class RawPacket
    {
        public byte[] escaped;
        public int escapePtr;
        int crc;

        // We're building a length-byte packet
        public RawPacket(int length)
        {
            escaped = new byte[2 * length];
            escapePtr = 0;
            crc = 0;
            while (escapePtr < Constants.FRAME_SYNC.Length)
            {
                escaped[escapePtr] = (byte)Constants.FRAME_SYNC[escapePtr];
                escapePtr++;
            }
        }

        public void NextByte(int b)
        {
            b = b & 0xff;
            crc = Crc.CalcByte(crc, b);

            escaped[escapePtr++] = (byte)b;
        }

        public void Terminate()
        {
            crc = crc & 0xff;
            escaped[escapePtr++] = (byte)(crc >> 8);
            escaped[escapePtr++] = (byte)crc;
        }
    }
}