using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Reflection.Emit;
using System.Runtime.InteropServices.Marshalling;
using System.Text;
using System.Threading;

namespace GbbEngine.Drivers.SolarmanV5
{
    public partial class SolarmanV5Driver : IDisposable , IDriver
    {

        public int Timeout { get; set; } = 5000; // 5 sec

        Configuration.Parameters Parameters;
        GbbLib.IOurLog? OurLog;

        private string IpAddress;
        private int PortNumber;
        private long SerialNumber;

        private Socket? Socket;

        public SolarmanV5Driver( Configuration.Parameters _Parameters, string _IPAddress, int _PortNumber, long _SerialNumber, GbbLib.IOurLog? ourLog)
        {
            Parameters = _Parameters;
            IpAddress = _IPAddress;
            PortNumber = _PortNumber;
            SerialNumber = _SerialNumber;
            OurLog = ourLog;

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
            Array.Copy(values, 0, data, 7, values.Length);
            var crc = GetCRC(data);
            data[data.Length - 2] = crc[0];
            data[data.Length - 1] = crc[1];
            await WriteSyncData(data, true);
        }


        // ------------------------------------------------------------------------
        // Write data and and wait for response
        private const int WAIT_READ_TIME_MS = 100; // 50ms
        private const int WAIT_WRITE_TIME_MS = 3000; // 3s
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
                        DelayMs = WAIT_WRITE_TIME_MS;
                    else
                        DelayMs = WAIT_READ_TIME_MS;

                    var ms = (int)(LastSend.Value.AddMilliseconds(DelayMs) - DateTime.Now).TotalMilliseconds;
                    if (ms > 0)
                    {
                        if (Parameters.IsDriverLog && OurLog != null)
                        {
                            OurLog.OurLog(Microsoft.Extensions.Logging.LogLevel.Information, $"Delay: {ms}ms");
                        }
                        await Task.Delay(ms);
                    }
                }

                // Prepare Frame
                if (Parameters.IsDriverLog && OurLog != null)
                {
                    OurLog.OurLog(Microsoft.Extensions.Logging.LogLevel.Information, $"Send ModBus: {BitConverter.ToString(write_data)}");
                }
                var Frame = new SolarmanFrame(GetNextSequenceNumber(), SerialNumber);
                var OutBuf = Frame.CreateFrame(write_data);

                // Send
                //if (Parameters.IsDriverLog && OurLog!=null)
                //{
                //    OurLog.OurLog(Microsoft.Extensions.Logging.LogLevel.Information, $"Send SolarmanV5: {BitConverter.ToString(OutBuf)}");
                //}
                int bytesSent = Socket.Send(OutBuf);

                // Receive
                byte[] buffer = new byte[1024];
                int bytesReceived = 0;
                bytesReceived = Socket.Receive(buffer);
                if (bytesReceived == 0) 
                    throw new ApplicationException("Connection Lost (received 0 bytes)");
                byte[] InBuf = new byte[bytesReceived];
                Buffer.BlockCopy(buffer, 0, InBuf, 0, bytesReceived);
                //if (Parameters.IsDriverLog && OurLog != null)
                //{
                //    OurLog.OurLog(Microsoft.Extensions.Logging.LogLevel.Information, $"Received SolarmanV5: {BitConverter.ToString(InBuf)}");
                //}

                // Get ModBUs frame
                Buf = Frame.GetModBusFrame(InBuf);
                if (Parameters.IsDriverLog && OurLog != null)
                {
                    OurLog.OurLog(Microsoft.Extensions.Logging.LogLevel.Information, $"Received ModBus: {BitConverter.ToString(Buf)}");
                }

                // Check Modbus CRC
                var crc = GetCRC(Buf);
                if (crc[0] != Buf[Buf.Length-2]
                 || crc[1] != Buf[Buf.Length-1])
                    throw new ApplicationException("SolarmanV5: Wrong CRC!");

                // get function
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

        public static List<string> OurSearchSolarman(IPAddress address)
        {
            var ret = new List<string>();


            int PORT = 48899;
            UdpClient udpClient = new UdpClient();
            var ep = new IPEndPoint(address, PORT);
            udpClient.Client.Bind(ep);
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
                            //ret.Add(BitConverter.ToString(recvBuffer));
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
