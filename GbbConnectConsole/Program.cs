using System.Reflection.Metadata;
using GbbEngine.Configuration;
using GbbEngine.Server;
using GbbLibSmall;

namespace GbbConnectConsole
{
    internal class Program
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="DontWaitForKey">true - don't wait for key, but just wait forever</param>
        static void Main(bool DontWaitForKey)
        {

            Console.WriteLine($"Parameters file            : {GbbEngine.Configuration.Parameters.Parameters_GetFileName()}");
            Console.WriteLine($"Log and statistic directory: {GbbEngine.Configuration.Parameters.OurGetUserBaseDirectory()}");

            // load parameters
            var FileName = GbbEngine.Configuration.Parameters.Parameters_GetFileName();
            var Parameters = GbbEngine.Configuration.Parameters.Load(FileName);

            // start server
            var JobManeger = new GbbEngine.Server.JobManager();
            var Log = new Log();
            JobManeger.OurStartJobs(Parameters, Log);


            if (DontWaitForKey)
            {
                Thread.Sleep(Timeout.Infinite);
            }
            else
            {
                // Finish
                Console.WriteLine("Press Enter to finish application...");
                Console.ReadLine();
                Console.WriteLine("Goodbye!");
            }
        }

        private class Log : GbbLibSmall.IOurLog
        {
            private object LogSync = new();

            public void OurLog(LogLevel LogLevel, string message, params object?[] args)
            {
                var nw = DateTime.Now;

                if (args.Length > 0)
                    message = string.Format(message, args);

                // add time
                string msg;
                if (LogLevel == LogLevel.Error)
                    msg = $"{nw}: ERROR: {message}\r\n";
                else
                    msg = $"{nw}: {message}\r\n";

                lock (LogSync)
                {

                    // directory for log
                    string FileName = Path.Combine(GbbEngine.Configuration.Parameters.OurGetUserBaseDirectory(), "Log");
                    Directory.CreateDirectory(FileName);

                    // filename of log
                    FileName = Path.Combine(FileName, $"{nw:yyyy-MM-dd}.txt");
                    File.AppendAllText(FileName, msg);

                    // to console:
                    Console.Write(msg);
                }
            }
        }

    }

}