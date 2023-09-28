using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace GbbEngine.Drivers.Random
{
    internal class RandomDriver : IDriver
    {

        static byte[] PrevRet= { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
        static System.Random rnd = new();

        public void Dispose()
        {
        }

        public byte[] ReadHoldingRegister(byte unit, ushort startAddress, ushort numInputs)
        {
            if (startAddress != 0 && numInputs != 5)
                throw new ArgumentException("Wrong arguments!");

            byte[] ret = new byte[10];

            // soc
            PutValue(ret, 0, rnd.Next(35, 95));
            for(int i =1; i<5; i++)
            {
                var q = GetValue(PrevRet, i);
                q = q + rnd.Next(1, 19);
                if (q > 65535)
                    q = 0;
                PutValue(ret, i, q);
            }
            PrevRet = ret;
            return ret;
        }

        public byte[] WriteMultipleRegister(byte unit, ushort startAddress, byte[] values)
        {
            throw new NotImplementedException();
        }

        private void PutValue(byte[] ret, int pos, int Value)
        {
            ret[pos*2] = (byte)((Value >> 8) & 0xff);
            ret[pos*2+1] = (byte)(Value & 0xff);    
        }

        private int GetValue(byte[] ret, int pos)
        {
            return (ret[pos*2] << 8) + ret[pos*2+1];
        }
    }
}
