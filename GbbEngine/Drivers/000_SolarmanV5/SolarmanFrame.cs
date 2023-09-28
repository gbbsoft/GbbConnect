using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GbbEngine.Drivers.SolarmanV5
{

    internal class SolarmanFrame
    {
        private UInt16 SequenceNumber;
        private byte[] Serial;


        public SolarmanFrame(UInt16 _SequenceNumber, long InverterSerialNumber)
        {
            SequenceNumber = _SequenceNumber;
            Serial = BitConverter.GetBytes(InverterSerialNumber);
        }

        public byte[] CreateFrame(byte[] command)
        {
            byte[] buf;

            byte[] frame = new byte[28 + command.Length];
            int offset = 0;

            // ==================================
            // Header

            // Start
            frame[offset++] = 0xa5;

            // Length
            buf = BitConverter.GetBytes((UInt16)command.Length + 15);
            frame[offset++] = buf[0];
            frame[offset++] = buf[1];

            // Code control
            frame[offset++] = 0x10;
            frame[offset++] = 0x45;

            // Serial
            buf = BitConverter.GetBytes((UInt16)SequenceNumber);
            frame[offset++] = buf[0]; // to jest wartość
            frame[offset++] = 0; // to jest zero

            // Rev. Serial // little-endian!
            frame[offset++] = Serial[0];
            frame[offset++] = Serial[1];
            frame[offset++] = Serial[2];
            frame[offset++] = Serial[3];

            // ==================================
            // Payload

            // Frame type(1), sensortype(2), TotalWorkingTime(4), PowerOnTime(4), OffsetTime(4)
            buf = new byte[] { 0x02, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };
            System.Buffer.BlockCopy(buf, 0, frame, offset, buf.Length);
            offset = offset + buf.Length;

            // Command (Modbus RTU Frame)
            System.Buffer.BlockCopy(command, 0, frame, offset, command.Length);
            offset = offset + command.Length;

            // ==================================
            // Trailer

            // Checksum
            UInt16 checksum = 0;
            for (int i = 1; i < offset; i++)
            {
                checksum = (UInt16)((checksum + frame[i]) & 255);
            }
            checksum &= 255;
            frame[offset++] = BitConverter.GetBytes(checksum)[0];

            // End Code
            frame[offset++] = 0x15;

            return frame;
        }


        public byte[] GetModBusFrame(byte[] Frame)
        {
            int FrameLen = Frame.Length;

            int PayloadLength = (int)ToUInt16(Frame, 1);

            if (FrameLen != PayloadLength + 13)
                throw new ApplicationException("SolarmanV5: FrameLen != PayloadLength + 13");
            if (Frame[0] != 0xa5)
                throw new ApplicationException("SolarmanV5: Wrong start byte");
            if (Frame[FrameLen - 1] != 0x15)
                throw new ApplicationException("SolarmanV5: Wrong end byte");

            // checksum
            // Źle działa z SofarSolar
            //byte checksum = 0;
            //for (int i = 1; i < FrameLen - 3; i++)
            //{
            //    checksum += Frame[i];
            //}
            //if (Frame[FrameLen - 2] != checksum)
            //    throw new ApplicationException("SolarmanV5: Wrong checksum");

            if (Frame[3] != 0x10
              || Frame[4] != 0x15)
                throw new ApplicationException("SolarmanV5: Wrong ControlCode");


            if (Frame[5] != SequenceNumber)
                throw new ApplicationException("SolarmanV5: Wrong SequenceNumber");

            if (Frame[7] != Serial[0] 
                || Frame[8]!= Serial[1]
                || Frame[9] != Serial[2]
                || Frame[10] != Serial[3])
                throw new ApplicationException("SolarmanV5: Wrong SerialNumber");

            if (Frame[11] != 2)
                throw new ApplicationException("SolarmanV5: Wrong FrameType");


            var ret = GetBytes(Frame, 25, FrameLen - 25 - 2);

            if (ret.Length < 5)
                throw new ApplicationException("SolarmanV5: frame does not contain a valid Modbus RTU frame");
            return ret;
        }

        // ======================================

        private static byte[] GetBytes(byte[] Frame, int startIndex, int length)
        {
            AssertArguments(Frame, startIndex, length);
            byte[] Buf = new byte[length];
            Buffer.BlockCopy(Frame, startIndex, Buf, 0, length);
            return Buf;
        }

        // ======================================

        //static private UInt32 ToUInt32(byte[] value, int startIndex)
        //{
        //    return unchecked((uint)(FromBytesAsserted(value, startIndex, 4)));
        //}

        static private UInt16 ToUInt16(byte[] value, int startIndex)
        {
            return unchecked((ushort)(FromBytesAsserted(value, startIndex, 2)));
        }

        static long FromBytesAsserted(byte[] value, int startIndex, int length)
        {
            AssertArguments(value, startIndex, length);
            return FromBytes(value, startIndex, length);
        }

        static void AssertArguments(byte[] value, int startIndex, int length)
        {
            if (value == null)
            {
                throw new ArgumentNullException("value");
            }
            if (startIndex < 0 || startIndex > value.Length - length)
            {
                throw new ArgumentOutOfRangeException("startIndex");
            }
        }

        //// Big-Endian
        //static long FromBytes(byte[] buffer, int startIndex, int length)
        //{
        //    long result = 0;
        //    for (int i = 0; i < length; i++)
        //    {
        //        result = unchecked((result << 8) | buffer[startIndex + i]);
        //    }
        //    return result;
        //}

        // Little-Endian
        static long FromBytes(byte[] buffer, int startIndex, int length)
        {
            long result = 0;
            for (int i = length-1; i >=0; i--)
            {
                result = unchecked((result << 8) | buffer[startIndex + i]);
            }
            return result;
        }
    }
}
