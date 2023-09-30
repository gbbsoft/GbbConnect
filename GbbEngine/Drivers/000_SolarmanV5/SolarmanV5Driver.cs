using System.Net;
using System.Net.Sockets;
using System.Reflection.Emit;
using System.Text;
using System.Threading;

namespace GbbEngine.Drivers.SolarmanV5
{
    public class SolarmanV5Driver : IDisposable , IDriver
    {

        public int Timeout { get; set; } = 500;

        private string IpAddress;
        private int PortNumber;
        private long SerialNumber;

        private Socket? Socket;

        public SolarmanV5Driver(string _IPAddress, int _PortNumber, long _SerialNumber)
        {
            IpAddress = _IPAddress;
            PortNumber = _PortNumber;
            SerialNumber = _SerialNumber;

            Connect();
        }

        public void Connect()
        {
            ArgumentNullException.ThrowIfNull(IpAddress);

            IPAddress? _ip;
            if (IPAddress.TryParse(IpAddress, out _ip) == false)
            {
                IPHostEntry hst = Dns.GetHostEntry(IpAddress);
                _ip= hst.AddressList[0];
            }
            Socket = new Socket(_ip.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            Socket.SendTimeout = Timeout;
            Socket.ReceiveTimeout = Timeout;
            Socket.NoDelay = true;
            Socket.Connect(new IPEndPoint(_ip, PortNumber));


        }

        // ======================================
        // IDisposable
        // ======================================
        private bool disposed = false;

        public void Dispose()
        {
            Dispose(disposing: true);
            // This object will be cleaned up by the Dispose method.
            // Therefore, you should call GC.SuppressFinalize to
            // take this object off the finalization queue
            // and prevent finalization code for this object
            // from executing a second time.
            GC.SuppressFinalize(this);
        }
        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    if (Socket != null)
                    {
                        try { Socket.Shutdown(SocketShutdown.Both); }
                        catch { }
                        Socket.Close();
                        Socket.Dispose();
                        Socket = null;  
                    }
                }
                disposed = true;
            }
        }

        // ======================================
        //
        //

        public async Task<byte[]> ReadHoldingRegister(byte unit, ushort startAddress, ushort numInputs)
        {
            if (numInputs > 125)
                throw new ApplicationException("Too much registers to read!");
            return await WriteSyncData(CreateReadHeader(unit, startAddress, numInputs, 3), false);
        }

        public async Task WriteMultipleRegister(byte unit, ushort startAddress, byte[] values)
        {
            if (values.Length > 250)
                throw new ApplicationException("Too much registers to write!");
            ushort numBytes = Convert.ToUInt16(values.Length);

            if (numBytes % 2 > 0) numBytes++;

            byte[] data = CreateWriteHeader(unit, startAddress, Convert.ToUInt16(numBytes / 2), Convert.ToUInt16(numBytes), 16);
            Array.Copy(values, 0, data, 6, values.Length);
            var crc = GetCRC(data);
            data[data.Length - 2] = crc[0];
            data[data.Length - 1] = crc[1];
            await WriteSyncData(data, true);
        }

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

            for (int i = 0; i < data.Length-2; i++)
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

        // ------------------------------------------------------------------------
        // Write data and and wait for response
        private const int WAIT_READ_TIME_MS = 50;
        private const int WAIT_WRITE_TIME_MS = 50;
        private DateTime? LastSend;

        private async Task<byte[]> WriteSyncData(byte[] write_data, bool iswrite)
        {
            ArgumentNullException.ThrowIfNull(Socket);

            byte[] Buf = new byte[2048];

            if (Socket.Connected)
            {
                //tb.AppendText($"{DateTime.Now}: Send ModBus: {BitConverter.ToString(write_data)}\r\n");

                // up to 50ms delay
                if (LastSend!=null)
                {
                    int DelayMs;
                    if (iswrite)
                        DelayMs = WAIT_READ_TIME_MS;
                    else
                        DelayMs = WAIT_READ_TIME_MS;

                    var ms = (int)(LastSend.Value.AddSeconds(DelayMs) - DateTime.Now).TotalMilliseconds;
                    if (ms > 0)
                        await Task.Delay(ms); 
                }


                var Frame = new SolarmanFrame(GetNextSequenceNumber(), SerialNumber);
                var OutBuf = Frame.CreateFrame(write_data);
                //tb.AppendText($"{DateTime.Now}: Send: {BitConverter.ToString(OutBuf)}\r\n");
                int bytesSent = Socket.Send(OutBuf);

                byte[] buffer = new byte[1024];
                int bytesReceived = Socket.Receive(buffer);
                if (bytesReceived == 0) 
                    throw new ApplicationException("Connection Lost");
                byte[] InBuf = new byte[bytesReceived];
                Buffer.BlockCopy(buffer, 0, InBuf, 0, bytesReceived);
                //tb.AppendText($"{DateTime.Now}: Received: {BitConverter.ToString(InBuf)}\r\n");

                Buf = Frame.GetModBusFrame(InBuf);
                //tb.AppendText($"{DateTime.Now}: Received ModBus: {BitConverter.ToString(Buf)}\r\n");

                var crc = GetCRC(Buf);
                if (crc[0] != Buf[Buf.Length-2]
                 || crc[1] != Buf[Buf.Length-1])
                    throw new ApplicationException("SolarmanV5: Wrong CRC!");

                byte function = Buf[1];
                byte[] data;


                // ------------------------------------------------------------
                // Response data is slave exception
                if (function > 128)
                {
                    function -= 128;
                    throw new ApplicationException($"Error response: function: {function}, error={Buf[2]}");
                }
                // ------------------------------------------------------------
                // Write response data
                else if ((function >= 5) && (function != 23))
                {
                    //data = new byte[2];
                    //Array.Copy(Buf, 3, data, 0, 2);
                    data = Buf;
                }
                // ------------------------------------------------------------
                // Read response data
                else
                {
                    var len = Buf[2];
                    data = new byte[len];
                    Array.Copy(Buf, 3, data, 0, len);
                }

                LastSend = DateTime.Now;

                return data;
            }
            else
                throw new ApplicationException("Connection closed!");
        }

        // ======================================
        // Sequence Number
        //

        private UInt16? SequenceNumber;

        private UInt16 GetNextSequenceNumber()
        {
            if (SequenceNumber == null)
                SequenceNumber = (UInt16)new System.Random().Next(1, 255);
            else
                SequenceNumber = (UInt16)((SequenceNumber +1 ) & 255);

            return SequenceNumber.Value;
        }


        // ======================================
        // Scan
        //

        public static List<string> OurSearchSolarman()
        {
            var ret = new List<string>();

            int PORT = 48899;
            UdpClient udpClient = new UdpClient();
            udpClient.Client.Bind(new IPEndPoint(IPAddress.Any, PORT));
            udpClient.EnableBroadcast = true;
            udpClient.Client.SendTimeout = 5000;
            udpClient.Client.ReceiveTimeout = 5000;
            //udpClient.AllowNatTraversal(true);

            var request = "WIFIKIT-214028-READ";

            var from = new IPEndPoint(0, 0);
            var task = Task.Run(() =>
            {
                try
                {
                    while (true)
                    {
                        var recvBuffer = udpClient.Receive(ref from);
                        var s = Encoding.ASCII.GetString(recvBuffer);
                        if (s != request)
                        {
                            ret.Add(BitConverter.ToString(recvBuffer));
                            ret.Add(s);
                        }

                    }
                }
                catch(System.Net.Sockets.SocketException)
                {
                }
            });

            var data = Encoding.UTF8.GetBytes(request);
            udpClient.Send(data, data.Length, "255.255.255.255", PORT);

            task.Wait();

            udpClient.Close();

            return ret;


        }


    }


}
