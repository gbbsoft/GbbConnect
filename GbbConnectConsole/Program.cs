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
        /// <param name="DontWaitForKey">don't wait for key, but just wait forever</param>
        static void Main(bool DontWaitForKey)
        {
            Console.WriteLine();
            Console.WriteLine("GbbConnectConsole by gbbsoft");
            Console.WriteLine();

            Console.WriteLine($"Version:                   : {GbbEngine.Configuration.Parameters.APP_VERSION}");
            Console.WriteLine($"Parameters file            : {GbbEngine.Configuration.Parameters.Parameters_GetFileName()}");
            Console.WriteLine($"Log and statistic directory: {GbbEngine.Configuration.Parameters.OurGetUserBaseDirectory()}");
            Console.WriteLine();

            var FileName = GbbEngine.Configuration.Parameters.Parameters_GetFileName();
            
            // create directory
            Directory.CreateDirectory(Path.GetDirectoryName(FileName)!);

            // load parameters
            if (!File.Exists(FileName))
            {
                Console.WriteLine("ERROR: No parameters.xml file!");
                DontWaitForKey = false;
            }
            else
            {
                var Parameters = GbbEngine.Configuration.Parameters.Load(FileName);

                // start server
                var JobManeger = new GbbEngine.Server.JobManager();
                var Log = new Log();
                JobManeger.OurStartJobs(Parameters, Log);
            }

            if (DontWaitForKey)
            {
                Thread.Sleep(Timeout.Infinite);
            }
            else
            {
                // Finish
                Console.WriteLine();
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