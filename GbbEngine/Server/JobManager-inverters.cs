using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
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
using static GbbLib.Application.StatusBar;

namespace GbbEngine.Server
{
    public partial class JobManager
    {

        private const int UNIT_NO = 1;

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
                await ProcessInverters(Parameters, ct, log);

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
        /// <returns></returns>
        private async Task ProcessInverters(Configuration.Parameters Parameters, CancellationToken ct, GbbLib.IOurLog log)
        {
            foreach (var Plant in Parameters.Plants)
            {
                if (Plant.IsDisabled == 0)
                {
                    try
                    {
                        DateTime nw = DateTime.Now;

                        // create driver
                        var Info = InverterInfo.OurGetInverterInfoByNumber(Plant.InverterNumber);
                        Drivers.IDriver Driver;

                        switch (Info.Driver)
                        {
                            case InverterInfo.Drivers.i000_SolarmanV5:
                                {
                                    if (Plant.AddressIP == null) throw new ApplicationException("Missing Plant Address!");
                                    if (Plant.PortNo == null) throw new ApplicationException("Missing Plant PortNumber!");
                                    if (Plant.SerialNumber == null) throw new ApplicationException("Missing Plant SerialNumber!");

                                    var drv = new SolarmanV5Driver(Parameters, Plant.AddressIP, Plant.PortNo.Value, Plant.SerialNumber.Value, log);
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


                        await GetDataFromInverter(Parameters, Plant, Info, Driver, ct, log, nw);
                        SaveStatisticFile(Plant, log, nw);
#if DEBUG
                        await ProcessSchedulers(Plant, Info, Driver, log, nw);
#endif
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
        private async Task GetDataFromInverter(Configuration.Parameters Parameters, Configuration.Plant Plant, InverterInfo Info, Drivers.IDriver Driver, CancellationToken ct, GbbLib.IOurLog log, DateTime nw)
        {
            ArgumentNullException.ThrowIfNull(Plant.PlantState);


            try
            {

                // Pre-Get multi registers
                Dictionary<int, int> Values = new();
                if (Info.FastRead1_RegStart != null && Info.FastRead1_RegCount != null)
                    await GetRegisters(Driver, Values, Info.FastRead1_RegStart.Value, Info.FastRead1_RegCount.Value);

                // ==============================
                // decode registers



                // SOC
                if (Info.RegisterNo_SOC != null)
                {
                    Plant.PlantState.SOC = await Get2Byte(Info.RegisterNo_SOC.Value, Values, Driver);

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
                    Plant.PlantState.TotalPVProdCurr = await Get4Byte(Info.PVProd_RegNo_Hi, Info.PVProd_RegNo_Lo.Value, Values, Driver);
                    if (Info.PVProd_Multipler != null)
                        Plant.PlantState.TotalPVProdCurr *= Info.PVProd_Multipler;
                }

                // TotalFromGridCurr
                if (Info.FromGrid_RegNo_TotalLo != null)
                {
                    Plant.PlantState.TotalFromGridCurr = await Get4Byte(Info.FromGrid_RegNo_TotalHi, Info.FromGrid_RegNo_TotalLo.Value, Values, Driver);
                    if (Info.FromGrid_Multipler != null)
                        Plant.PlantState.TotalFromGridCurr *= Info.FromGrid_Multipler;
                }

                // TotalToGridCurr
                if (Info.ToGrid_RegNo_TotalLo != null)
                {
                    Plant.PlantState.TotalToGridCurr = await Get4Byte(Info.ToGrid_RegNo_TotalHi, Info.ToGrid_RegNo_TotalLo.Value, Values, Driver);
                    if (Info.ToGrid_Multipler != null)
                        Plant.PlantState.TotalToGridCurr *= Info.FromGrid_Multipler;
                }

                // PVProdCurr
                if (Info.Load_RegNo_TotalLo != null)
                {
                    Plant.PlantState.TotalLoadCurr = await Get4Byte(Info.Load_RegNo_TotalHi, Info.Load_RegNo_TotalLo.Value, Values, Driver);
                    if (Info.Load_Multipler != null)
                        Plant.PlantState.TotalLoadCurr *= Info.Load_Multipler;
                }

                Plant.PlantState.TimeOfCurr = nw;

                if (Parameters.IsVerboseLog)
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

        private async Task<int> Get4Byte(int? RegNoHi, int RegNoLo, Dictionary<int, int> Values, IDriver Driver)
        {
            if (RegNoHi != null)
                return await Get2Byte(RegNoLo, Values, Driver)+ (await Get2Byte(RegNoHi.Value, Values, Driver) << 16) ; // first lo, so hi will be took together
            else
                return await Get2Byte(RegNoLo, Values, Driver);
        }


        private async Task<int> Get2Byte(int RegNo, Dictionary<int, int> Values, IDriver Driver)
        {
            int ret;
            if (!Values.TryGetValue(RegNo, out ret))
            {
                await GetRegisters(Driver, Values, RegNo, 2); // get also next register for Get4Bytes
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
        private async Task GetRegisters(IDriver Driver, Dictionary<int, int> Values, int RegStart, int RegCount)
        {
            var responce = await Driver.ReadHoldingRegister(UNIT_NO, (ushort)RegStart, (ushort)RegCount);
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

        // ======================================
        // Send Schedulers to Inverter
        // ======================================


        private async Task ProcessSchedulers(Plant Plant, InverterInfo Info, Drivers.IDriver Driver, IOurLog log, DateTime nw)
        {
            if (Plant.PlantState!.SchedulersReadyToProcess)
            {
                var Schedulers = Plant.PlantState!.Schedulers!;
                Plant.PlantState!.SchedulersReadyToProcess = false;

                Dictionary<int, int> Values = new();

                if (Info.Deya_TimeOfUser_RegNo != null)
                {

                    // =============================
                    // Convert Scheduler to Deya TimeOfUse
                    // =============================
                    var TimeOfUse = ConvertSchedulers(Schedulers);

                    // =============================
                    // Send TimeOfUse to Inverter
                    // =============================

                    // jeżeli wpisów jest za dużo, to usuwamy wpisy z przeszłości
                    int CurrHour = nw.Hour * 100;
                    while (TimeOfUse.Count > 6)
                    {
                        if (TimeOfUse[1].FromTime < CurrHour)
                            TimeOfUse.RemoveAt(0);
                    }

                    byte[] Tab = new byte[2];
                    int Pos = 0;
                    int SrcPos = 0;
                    Deya_TimeOfUse? Curr = null;
                    while (Pos < 6)
                    {
                        var itm = TimeOfUse[SrcPos];

                        if (itm.FromTime < CurrHour)
                            Curr = itm;

#if !DEBUG
                        int Offset = 2 * 6; // bytes

                        // Time
                        await Driver.WriteMultipleRegister(UNIT_NO, (ushort)(Info.Deya_TimeOfUser_RegNo + Pos), Put2Byte(Tab, Pos, itm.FromTime));

                        // Power
                        if (itm.Power != null)
                        {
                            await Driver.WriteMultipleRegister(UNIT_NO, (ushort)(Info.Deya_TimeOfUser_RegNo + Pos + Offset), Put2Byte(Tab, Pos, itm.Power.Value));
                        }

                        // SOC
                        await Driver.WriteMultipleRegister(UNIT_NO, (ushort)(Info.Deya_TimeOfUser_RegNo + Pos + 3 * Offset), Put2Byte(Tab, Pos, itm.SOC));

                        // Grid Charge
                        int Value = itm.IsGridCharging? 1 : 0;
                        await Driver.WriteMultipleRegister(UNIT_NO, (ushort)(Info.Deya_TimeOfUser_RegNo + Pos + 4 * Offset), Put2Byte(Tab, Pos, Value));

#endif
                        // jak jest za mało TimeOfUser, to powtarzamy ostatni do końca
                        if (SrcPos+1 < TimeOfUse.Count)
                            SrcPos++;
                        Pos++;
                    }

                    if (Curr != null)
                    {
                        // IsSellingFirst
                        if (Info.Deya_WorkMode_RegNo!=null)
                        {
                            if (Curr.IsSellingFirst)
                            {
                                // read prev Mode
                                if (Plant.PlantState.PrevDeyaMode == null)
                                {
                                    Plant.PlantState.PrevDeyaMode = await Get2Byte(Info.Deya_WorkMode_RegNo.Value, Values, Driver);
                                }
                                // change to 0
                                await Driver.WriteMultipleRegister(UNIT_NO, (ushort)(Info.Deya_WorkMode_RegNo), Put2Byte(Tab, Pos, (int)InverterInfo.Deya_Modes.i00_SellingFirst));
                            }
                            else if (Plant.PlantState.PrevDeyaMode != null)
                            {
                                // revent to prev
                                await Driver.WriteMultipleRegister(UNIT_NO, (ushort)(Info.Deya_WorkMode_RegNo), Put2Byte(Tab, Pos, Plant.PlantState.PrevDeyaMode.Value));
                                Plant.PlantState.PrevDeyaMode= null;
                            }
                        }

                        // ZeroChargeA
                        if (Info.MaxACharge != null)
                        {
                            if (Curr.IsZeroChargeA)
                            {
                                // read prev ChargeA
                                if (Plant.PlantState.PrevGridChargeA == null)
                                {
                                    Plant.PlantState.PrevGridChargeA = await Get2Byte(Info.MaxACharge.Value, Values, Driver);
                                }
                                // change to 0
                                await Driver.WriteMultipleRegister(UNIT_NO, (ushort)(Info.MaxACharge), Put2Byte(Tab, Pos, 0));
                            }
                            else if (Plant.PlantState.PrevGridChargeA != null)
                            {
                                // revent to prev
                                await Driver.WriteMultipleRegister(UNIT_NO, (ushort)(Info.MaxACharge), Put2Byte(Tab, Pos, Plant.PlantState.PrevGridChargeA.Value));
                                Plant.PlantState.PrevGridChargeA = null;
                            }
                        }
                        Plant.PlantState.OurSaveState();
                    }

                }
            }

        }

        private byte[] Put2Byte(byte[] Tab, int Pos, int Value)
        {
            Tab[0] = (byte)((Value >> 8) & 0xff);
            Tab[1] = (byte)(Value & 0xff);
            return Tab;
        }


    }
}
