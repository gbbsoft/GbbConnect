using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GbbEngine.Drivers
{
    internal interface IDriver
    {
        public byte[] ReadHoldingRegister(byte unit, ushort startAddress, ushort numInputs);
        public byte[] WriteMultipleRegister(byte unit, ushort startAddress, byte[] values);

        public void Dispose();

    }
}
