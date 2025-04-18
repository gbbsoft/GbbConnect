﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using GbbEngine.Configuration;

namespace GbbEngine.Server
{
    internal class PlantState
    {
        // ======================================
        // Plant State

        public Plant? Plant;

        public DateTime? TimeOfStart { get; set; }
        public DateTime? TimeOfCurr { get; set; }

        public decimal? SOC {  get; set; }
        public decimal? MinSOC {  get; set; }
        public decimal? MaxSOC {  get; set; }
        public decimal? SumSOC {  get; set; } // for AvrSOC = SumSOC/CountSOC
        public int CountSOC {  get; set; }


        public decimal? TotalPVProdStart { get; set; } // kWh
        public decimal? TotalPVProdCurr { get; set; }

        public decimal? TotalFromGridStart { get; set; } // kWh
        public decimal? TotalFromGridCurr { get; set; }

        public decimal? TotalToGridStart { get; set; } // kWh
        public decimal? TotalToGridCurr { get; set; }

        public decimal? TotalLoadStart { get; set; } // kWh
        public decimal? TotalLoadCurr { get; set; }

        // ======================================
        // Plant State

        //EventWaitHandle

        public DateTime SchedulerLastprocess {  get; set; }
        public bool SchedulersReadyToProcess {  get; set; }
        public List<GbbConnectProtocol.Request_Scheduler>? Schedulers { get; set; }

        public int? PrevGridChargeA { get; set; }
        public int? PrevDeyaMode {  get; set; }


        /// <summary>
        /// Previous Voltage in LV1
        /// </summary>
        public int? PrevDeyeLV1 {  get; set; }



        // ======================================
        // additional properties

        public MQTTnet.IMqttClient? MqttClient;

        // ======================================
        // cache of today file

        //internal class FileRow
        //{
        //    public DateTime Date { get; set; }
        //    public int Hour { get; set; }

        //    //public double? SOC { get; set; }
        //    //public double? PVProd { get; set; }
        //    //public double? FromGrid { get; set; }
        //    //public double? ToGrid { get; set; }
        //    //public double? Load { get; set; }

        //}

        internal System.Text.StringBuilder? CurrentDayFileContent;   // up-to current hour
        internal System.Text.StringBuilder? CurrentHourFileContent;   // current hour (with new line on end)




        // ======================================
        // Save/Load State
        // ======================================
        internal object SaveFileLoc = new();

        public void OurSaveState()
        {
            // options for json serialization
            var SerOpt = new JsonSerializerOptions();
            SerOpt.DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull;


            // save
            var FileName = OurGetFileName(Plant!);
            var json = JsonSerializer.Serialize(this, SerOpt);
            lock (SaveFileLoc)
            {
                File.WriteAllText(FileName, json);
            }
        }

        public static PlantState OurLoadState(Plant plant)
        {
            PlantState? ret = null;

            var Filename = OurGetFileName(plant);
            if (File.Exists(Filename))
            {
                try
                {
                    var s = File.ReadAllText(Filename);
                    ret = JsonSerializer.Deserialize<PlantState>(s, new JsonSerializerOptions() { IncludeFields = false });
                }
                catch
                {
                }
            }

            if (ret == null)
                ret = new PlantState();
            ret.Plant = plant;

            return ret;
        }


        /// <summary>
        /// File fo PlantStates: "c:\Users\[user]\AppData\Roaming\Gbb Software\GbbConnect\PlantStates\[Number].json"
        /// </summary>
        /// <param name="plant"></param>
        /// <returns></returns>
        private static string OurGetFileName(Plant plant)
        {
            // directory
            string FileName = Path.Combine(Parameters.OurGetUserBaseDirectory(), "PlantStates");
            Directory.CreateDirectory(FileName);

            // file name
            FileName = Path.Combine(FileName, $"{plant.Number:00000}.json");

            return FileName;
        }


    }
}
