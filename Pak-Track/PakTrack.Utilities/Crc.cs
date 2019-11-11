using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PakTrack.Utilities
{
    public class Crc
    {
        public static int CalcByte(int crc, int b)
        {
            crc = crc ^ b << 8;

            for (int i = 0; i < 8; i++)
            {
                if ((crc & 0x8000) == 0x8000)
                {
                    crc = crc << 1 ^ 0x1021;
                }

                else
                {
                    crc = crc << 1;
                }
            }

            return crc & 0xffff;
        }

        public static int Calc(byte[] packet, int index, int count)
        {
            int crc = 0;

            while (count > 0)
            {
                crc = CalcByte(crc, packet[index++]);
                count--;
            }
            return crc;
        }

        public static int Calc(byte[] packet, int count)
        {
            return Calc(packet, 0, count);
        }

        public static void Set(byte[] packet)
        {
            int crc = Crc.Calc(packet, packet.Length - 2);

            packet[packet.Length - 2] = (byte)(crc & 0xFF);
            packet[packet.Length - 1] = (byte)((crc >> 8) & 0xFF);
        }

        //public static void main(String[] args)
        //{
        //    byte[] ia = new byte[args.length];

        //    for (int i = 0; i < args.length; i++)
        //    {
        //        try
        //        {
        //            ia[i] = Integer.decode(args[i]).byteValue();
        //        }
        //        catch (NumberFormatException e) { }
        //    }
        //    System.out.println(Integer.toHexString(Calc(ia, ia.length)));
        //}
    }
}
