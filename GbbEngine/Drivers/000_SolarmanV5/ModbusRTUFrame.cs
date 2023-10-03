using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace GbbEngine.Drivers.SolarmanV5
{
    public partial class SolarmanV5Driver
    {

        // ------------------------------------------------------------------------
        // Create modbus header for read action
        private byte[] CreateReadHeader(byte unit, ushort startAddress, ushort length, byte function)
        {
            byte[] data = new byte[8];

            data[0] = unit;
            data[1] = function;				// Function code
            byte[] _adr = BitConverter.GetBytes((short)IPAddress.HostToNetworkOrder((short)startAddress));
            data[2] = _adr[0];				// Start address
            data[3] = _adr[1];				// Start address
            byte[] _length = BitConverter.GetBytes((short)IPAddress.HostToNetworkOrder((short)length));
            data[4] = _length[0];			// Number of registers to read 
            data[5] = _length[1];			// Number of registers to read
            var crc = GetCRC(data);
            data[6] = crc[0];
            data[7] = crc[1];
            return data;
        }

        // ------------------------------------------------------------------------
        // Create modbus header for write action
        private byte[] CreateWriteHeader(byte unit, ushort startAddress, ushort numData, ushort numBytes, byte function)
        {
            byte[] data = new byte[numBytes + 9];

            data[0] = unit;
            data[1] = function;				// Function code
            byte[] _adr = BitConverter.GetBytes((short)IPAddress.HostToNetworkOrder((short)startAddress));
            data[2] = _adr[0];				// Start address
            data[3] = _adr[1];				// Start address
            if (function >= 15)
            {
                byte[] _cnt = BitConverter.GetBytes((short)IPAddress.HostToNetworkOrder((short)numData));
                data[4] = _cnt[0];			// Number of registers
                data[5] = _cnt[1];			// Number of registers
                data[6] = (byte)(numBytes);
            }
            return data;
        }

        private byte[] GetCRC(byte[] data)
        {
            ushort CRCFull = 0xFFFF;
            char CRCLSB;

            byte[] crc16 = new byte[2];

            for (int i = 0; i < data.Length - 2; i++)
            {
                CRCFull = (ushort)(CRCFull ^ data[i]);

                for (int j = 0; j < 8; j++)
                {
                    CRCLSB = (char)(CRCFull & 0x0001);
                    CRCFull = (ushort)((CRCFull >> 1) & 0x7FFF);

                    if (CRCLSB == 1)
                        CRCFull = (ushort)(CRCFull ^ 0xA001);
                }
            }

            crc16[0] = (byte)(CRCFull & 0xFF);
            crc16[1] = (byte)((CRCFull >> 8) & 0xFF);

            return crc16;
        }

    }
}
