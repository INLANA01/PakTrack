using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PakTrack.Utilities
{
    public class RawPacket
    {
        public byte[] escaped;
        public int escapePtr;
        int crc;

        // We're building a length-byte packet
        public RawPacket(int length)
        {
            escaped = new byte[2 * length];// about 12 if buffered then comment it that we re using 18
            escapePtr = 0;
            crc = 0;
            while (escapePtr < SensorConstants.FRAME_SYNC.Length)
            {
                escaped[escapePtr] = (byte)SensorConstants.FRAME_SYNC[escapePtr];
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
