using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using GbbEngine.Configuration;
using GbbEngine.Drivers;
using GbbEngine.Drivers.Random;
using GbbEngine.Drivers.SolarmanV5;
using GbbEngine.Inverters;
using GbbLib;
using Microsoft.Extensions.Logging;
using Org.BouncyCastle.Bcpg;
using static System.Formats.Asn1.AsnWriter;

namespace GbbEngine.Server
{
    public partial class JobManager
    {

        internal async void OurInverterService(Configuration.Parameters Parameters, CancellationToken ct, GbbLib.IOurLog log)
        {

            try
            {
                while (!ct.IsCancellationRequested)
                {
                    try
                    {
                        // start
                        log.OurLog(LogLevel.Information, "InverterService: starting");
                        await OurInverterService_DoWork(Parameters, ct, log);

                    }
                    catch (TaskCanceledException)
                    {
                    }
                    catch (Exception ex) // eg System.Threading.Tasks.TaskCanceledException
                    {
                        // log
                        log.OurLog(LogLevel.Error, $"InverterService: {ex}");

                    }
                    // try again after 5min
                    await Task.Delay(5 * 60 * 1000, ct);
                }
            }
            catch (TaskCanceledException)
            {
            }
            // log
            log.OurLog(LogLevel.Information, "InverterService: finished");
        }

        /// <summary>
        /// loop every 1 minute
        /// </summary>
        /// <param name="Parameters"></param>
        /// <param name="ct"></param>
        /// <param name="log"></param>
        /// <returns></returns>
        private async Task OurInverterService_DoWork(Configuration.Parameters Parameters, CancellationToken ct, GbbLib.IOurLog log)
        {
            while (!ct.IsCancellationRequested)
            {
                GetDataFromInverters(Parameters, ct, log);

                // wait to full minute
                DateTime nw = DateTime.Now;
                DateTime nxt = new DateTime(nw.Year, nw.Month, nw.Day, nw.Hour, nw.Minute, 0).AddMinutes(1);
                var ms = (int)(nxt - nw).TotalMilliseconds;
                if (ms > 0)
                    await Task.Delay(ms, ct);

            }
        }

        /// <summary>
        /// loop per plant
        /// </summary>
        /// <param name="Parameters"></param>
        /// <param name="ct"></param>
        /// <param name="log"></param>
        /// <returns></returns>                                               7
        private void GetDataFromInverters(Configuration.Parameters Parameters, CancellationToken ct, GbbLib.IOurLog log)
        {
            foreach (var Plant in Parameters.Plants)
            {
                if (Plant.IsDisabled == 0)
                {
                    try
                    {
                        DateTime nw = DateTime.Now;
                        GetDataFromInverter(Plant, InverterInfo.OurGetInverterInfoByNumber(Plant.InverterNumber), ct, log, nw);
                        SaveStatisticFile(Plant, log, nw);
                    }
                    catch (TaskCanceledException)
                    {
                    }
                    catch (Exception ex)
                    {
                        log.OurLog(LogLevel.Error, $"{Plant.Name}: Inverter ERROR: {ex.Message}");
                    }
                }
                if (ct.IsCancellationRequested)
                    break;
            }
        }

        /// <summary>
        /// Get Curr data from inverter to PlantState
        /// </summary>
        /// <param name="Plant"></param>
        /// <param name="Info"></param>
        /// <param name="ct"></param>
        /// <param name="log"></param>
        /// <exception cref="ApplicationException"></exception>
        private void GetDataFromInverter(Configuration.Plant Plant, InverterInfo Info, CancellationToken ct, GbbLib.IOurLog log, DateTime nw)
        {
            ArgumentNullException.ThrowIfNull(Plant.PlantState);

            // create driver
            Drivers.IDriver Driver;

            switch(Info.Driver)
            {
                case InverterInfo.Drivers.i000_SolarmanV5:
                    {
                        if (Plant.AddressIP == null) throw new ApplicationException("Missing Plant Address!");
                        if (Plant.PortNo == null) throw new ApplicationException("Missing Plant PortNumber!");
                        if (Plant.SerialNumber == null) throw new ApplicationException("Missing Plant SerialNumber!");

                        var drv = new SolarmanV5Driver(Plant.AddressIP, Plant.PortNo.Value, Plant.SerialNumber.Value);
                        drv.Connect();
                        Driver = drv;
                    }
                    break;

                case InverterInfo.Drivers.i999_Random:
                    Driver = new RandomDriver();
                    break;
                default:
                    throw new ApplicationException("Uknown driver type: " + Info.Driver);
            }

            try
            {

                Dictionary<int, int> Values = new();
                if (Info.FastRead1_RegStart != null && Info.FastRead1_RegCount != null)
                    GetRegisters(Driver, Values, Info.FastRead1_RegStart.Value, Info.FastRead1_RegCount.Value);

                // ==============================
                // decode registers



                // SOC
                if (Info.RegisterNo_SOC != null)
                {
                    Plant.PlantState.SOC = Get2Byte(Info.RegisterNo_SOC.Value, Values, Driver);

                    if (Plant.PlantState.MinSOC == null || Plant.PlantState.SOC < Plant.PlantState.MinSOC)
                        Plant.PlantState.MinSOC = Plant.PlantState.SOC;
                    if (Plant.PlantState.MaxSOC == null || Plant.PlantState.SOC > Plant.PlantState.MaxSOC)
                        Plant.PlantState.MaxSOC = Plant.PlantState.SOC;

                    if (Plant.PlantState.SumSOC == null)
                        Plant.PlantState.SumSOC = Plant.PlantState.SOC;
                    else
                        Plant.PlantState.SumSOC += Plant.PlantState.SOC;

                    Plant.PlantState.CountSOC++;
                }

                // PVProdCurr
                if (Info.PVProd_RegNo_Lo != null)
                {
                    Plant.PlantState.TotalPVProdCurr = Get4Byte(Info.PVProd_RegNo_Hi, Info.PVProd_RegNo_Lo.Value, Values, Driver);
                    if (Info.PVProd_Multipler != null)
                        Plant.PlantState.TotalPVProdCurr *= Info.PVProd_Multipler;
                }

                // TotalFromGridCurr
                if (Info.FromGrid_RegNo_TotalLo != null)
                {
                    Plant.PlantState.TotalFromGridCurr = Get4Byte(Info.FromGrid_RegNo_TotalHi, Info.FromGrid_RegNo_TotalLo.Value, Values, Driver);
                    if (Info.FromGrid_Multipler != null)
                        Plant.PlantState.TotalFromGridCurr *= Info.FromGrid_Multipler;
                }

                // TotalToGridCurr
                if (Info.ToGrid_RegNo_TotalLo != null)
                {
                    Plant.PlantState.TotalToGridCurr = Get4Byte(Info.ToGrid_RegNo_TotalHi, Info.ToGrid_RegNo_TotalLo.Value, Values, Driver);
                    if (Info.ToGrid_Multipler != null)
                        Plant.PlantState.TotalToGridCurr *= Info.FromGrid_Multipler;
                }

                // PVProdCurr
                if (Info.Load_RegNo_TotalLo != null)
                {
                    Plant.PlantState.TotalLoadCurr = Get4Byte(Info.Load_RegNo_TotalHi, Info.Load_RegNo_TotalLo.Value, Values, Driver);
                    if (Info.Load_Multipler != null)
                        Plant.PlantState.TotalLoadCurr *= Info.Load_Multipler;
                }

                Plant.PlantState.TimeOfCurr = nw;

                if (Configuration.Parameters.IsVerboseLog)
                {
                    decimal? AvrSOC = null;
                    if (Plant.PlantState.CountSOC!=0)
                        AvrSOC = Plant.PlantState.SumSOC / Plant.PlantState.CountSOC;

                    log.OurLog(LogLevel.Information,
                        $"{Plant.Name}: {nw}: SOC={Plant.PlantState.SOC}, MinSOC={Plant.PlantState.MinSOC}, MaxSOC={Plant.PlantState.MaxSOC}, AvrSOC={AvrSOC:N2}, " +
                        $"TotalPVProd={Plant.PlantState.TotalPVProdCurr}, TotalFromGrid={Plant.PlantState.TotalFromGridCurr}, " +
                        $"TotalToGrid={Plant.PlantState.TotalToGridCurr}, TotalLoad={Plant.PlantState.TotalLoadCurr}"
                        );
                }


            }
            finally
            {
                Plant.PlantState.OurSaveState();
                Driver.Dispose();
            }
            
            
        }

        private int Get4Byte(int? RegNo1, int RegNo2, Dictionary<int, int> Values, IDriver Driver)
        {
            if (RegNo1 != null)
                return Get2Byte(RegNo1.Value, Values, Driver) << 16 + Get2Byte(RegNo2, Values, Driver);
            else
                return Get2Byte(RegNo2, Values, Driver);
        }


        private int Get2Byte(int RegNo, Dictionary<int, int> Values, IDriver Driver)
        {
            int ret;
            if (!Values.TryGetValue(RegNo, out ret))
            {
                GetRegisters(Driver, Values, RegNo, 1);
            }
            return Values[RegNo];
        }


        /// <summary>
        /// Read set of registers to dictionary cache
        /// </summary>
        /// <param name="Driver"></param>
        /// <param name="Values"></param>
        /// <param name="RegStart"></param>
        /// <param name="RegCount"></param>
        private void GetRegisters(IDriver Driver, Dictionary<int, int> Values, int RegStart, int RegCount)
        {
            var responce = Driver.ReadHoldingRegister(unit: 1, (ushort)RegStart, (ushort)RegCount);
            for(var i = 0; i<RegCount; i++)
                Values.TryAdd(RegStart+i, (responce[2*i]<<8) + responce[2*i+1]);

        }

        // ======================================
        // Create files with data
        // ======================================

        const string FILE_HEADER = "Date\tHour\tSOC\tMinSOC\tMaxSOC\tAvrSOC\tPVProd\tFromGrid\tToGrid\tLoad\t";

        /// <summary>
        /// after read today data
        /// </summary>
        /// <param name="Plant"></param>
        /// <param name="nw"></param>
        private void SaveStatisticFile(Configuration.Plant Plant, GbbLib.IOurLog log, DateTime nw)
        {
            ArgumentNullException.ThrowIfNull(Plant.PlantState);

            CultureInfo ci = CultureInfo.InvariantCulture;

            if (Plant.PlantState.TimeOfStart == null
                || (nw - Plant.PlantState.TimeOfStart.Value).TotalMinutes > 70)
            {
                // first use of new start counters
                // or start counters too old
                StartNewHour(Plant, log, nw);

            }
            else
            {
                // finish start hour
                string FileName = OurGetStatFileName(Plant, Plant.PlantState.TimeOfStart.Value);

                // if no file in cache than load today file if present
                if (Plant.PlantState.CurrentDayFileContent == null)
                {
                    //Plant.PlantState.CurrentDayFileStatistic = new();
                    Plant.PlantState.CurrentDayFileContent = new();
                    if (File.Exists(FileName))
                    {
                        string s = File.ReadAllText(FileName);
                        string[] lines = s.Split("\r\n");
                        bool FirstLine = true;
                        foreach (var line in lines)
                        {
                            if (FirstLine)
                            {
                                // skip header
                                FirstLine = false;
                                Plant.PlantState.CurrentDayFileContent.AppendLine(line);
                            }
                            else
                            {
                                string[] col = line.Split('\t');
                                if (col.Length > 1)
                                {
                                    // add hours before current
                                    if (int.Parse(col[1]) < nw.Hour)
                                        Plant.PlantState.CurrentDayFileContent.AppendLine(line);
                                }
                            }
                        }
                    }
                    else
                    {
                        // add header
                        Plant.PlantState.CurrentDayFileContent.AppendLine(FILE_HEADER);
                    }
                }

                // add current hour
                StringBuilder CurrHour = new();
                CurrHour.Append(Plant.PlantState.TimeOfStart.Value.Date.ToString("yyyy-MM-dd")); CurrHour.Append('\t');
                CurrHour.Append(Plant.PlantState.TimeOfStart.Value.Hour); CurrHour.Append('\t');
                if (Plant.PlantState.CountSOC != 0)
                {
                    CurrHour.Append(Plant.PlantState.SOC!.Value.ToString(ci)); CurrHour.Append('\t');
                    CurrHour.Append(Plant.PlantState.MinSOC!.Value.ToString(ci)); CurrHour.Append('\t');
                    CurrHour.Append(Plant.PlantState.MaxSOC!.Value.ToString(ci)); CurrHour.Append('\t');
                    CurrHour.Append((Plant.PlantState.SumSOC!.Value / Plant.PlantState.CountSOC).ToString("N2", ci)); CurrHour.Append('\t'); 
                }
                else
                {
                    CurrHour.Append('\t');
                    CurrHour.Append('\t');
                    CurrHour.Append('\t');
                    CurrHour.Append('\t');
                }
                CurrHour.Append(CalcDiff(Plant.PlantState.TotalPVProdCurr, Plant.PlantState.TotalPVProdStart)?.ToString(ci)); CurrHour.Append('\t');
                CurrHour.Append(CalcDiff(Plant.PlantState.TotalFromGridCurr, Plant.PlantState.TotalFromGridStart)?.ToString(ci)); CurrHour.Append('\t');
                CurrHour.Append(CalcDiff(Plant.PlantState.TotalToGridCurr, Plant.PlantState.TotalToGridStart)?.ToString(ci)); CurrHour.Append('\t');
                CurrHour.Append(CalcDiff(Plant.PlantState.TotalLoadCurr, Plant.PlantState.TotalLoadStart)?.ToString(ci)); CurrHour.Append('\t');
                CurrHour.AppendLine();

                Plant.PlantState.CurrentHourFileContent = CurrHour;

                lock (StatisticFileLock)
                {
                    // save file
                    File.WriteAllText(FileName, Plant.PlantState.CurrentDayFileContent.ToString() + Plant.PlantState.CurrentHourFileContent.ToString());
                }

                // check end of hour/day
                if (Plant.PlantState.TimeOfStart.Value.Date != nw.Date)
                    // end of day
                {
                    Plant.PlantState.CurrentDayFileContent = null;
                    Plant.PlantState.CurrentHourFileContent = null;
                    StartNewHour(Plant, log, nw);
                }
                else if (Plant.PlantState.TimeOfStart.Value.Hour != nw.Hour)
                    // end if hour
                {
                    // dopisanie poprzedniej godziny do pliku
                    Plant.PlantState.CurrentDayFileContent.Append(Plant.PlantState.CurrentHourFileContent.ToString());
                    Plant.PlantState.CurrentHourFileContent = null;
                    StartNewHour(Plant, log, nw);
                }


            }

        }

        private static void StartNewHour(Plant Plant, IOurLog log, DateTime nw)
        {
            ArgumentNullException.ThrowIfNull(Plant.PlantState);

            Plant.PlantState.MinSOC = null;
            Plant.PlantState.MaxSOC = null;
            Plant.PlantState.SumSOC = null;
            Plant.PlantState.CountSOC = 0;

            Plant.PlantState.TimeOfStart = Plant.PlantState.TimeOfCurr;
            Plant.PlantState.TotalPVProdStart = Plant.PlantState.TotalPVProdCurr;
            Plant.PlantState.TotalFromGridStart = Plant.PlantState.TotalFromGridCurr;
            Plant.PlantState.TotalToGridStart = Plant.PlantState.TotalToGridCurr;
            Plant.PlantState.TotalLoadStart = Plant.PlantState.TotalLoadCurr;
            Plant.PlantState.OurSaveState();

            Plant.PlantState.TimeOfStart = nw;

            log.OurLog(LogLevel.Information,
                $"{Plant.Name}: {nw}: Hour start: TotalPVProd={Plant.PlantState.TotalPVProdCurr}, TotalFromGrid={Plant.PlantState.TotalFromGridCurr}, " +
                $"TotalToGrid={Plant.PlantState.TotalToGridCurr}, TotalLoad={Plant.PlantState.TotalLoadCurr}"
                );
        }

        /// <summary>
        /// If counter is going back then return 0
        /// </summary>
        /// <param name="curr"></param>
        /// <param name="prev"></param>
        /// <returns></returns>
        private decimal? CalcDiff(decimal? curr, decimal? prev)
        {
            if (prev != null && curr != null)
                if (curr >= prev)
                    return curr.Value - prev.Value;
                else
                    return 0;
            else if (curr != null)
                return curr.Value;
            return null;

        }



        /// <summary>
        /// File of statistics: c:\Users\[user]\Dokuments\GbbConnect\Statistics\Inv_[Number]\[year]\Stat_[date].csv
        /// </summary>
        /// <param name="Plant"></param>
        /// <param name="nw"></param>
        /// <returns></returns>
        private static string OurGetStatFileName(Plant Plant, DateTime nw)
        {
            // directory
            string FileName = Path.Combine(Parameters.OurGetUserBaseDirectory(), "Statistics", $"Inv_{Plant.Number:000}", nw.Year.ToString());
            Directory.CreateDirectory(FileName);

            // file name
            FileName = Path.Combine(FileName,$"Stat_{nw:yyyy-MM-dd}.csv");

            return FileName;
        }


    }
}
