using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GbbEngine.Inverters
{
    public class InverterInfo
    {
        public int Number {  get; set; }
        public string Name { get; set; }

        public enum Drivers
        {
            i000_SolarmanV5 = 0,
            i999_Random = 999,
        }
        public Drivers Driver {  get; set; }


        public InverterInfo(int Number, string name, Drivers _driver)
        { 
            this.Number = Number;   
            this.Name = name; 
            this.Driver = _driver;
        }

        // ======================================
        // Read

        public int? RegisterNo_SOC;

        public int? PVProd_RegNo_Lo; // kWh
        public int? PVProd_RegNo_Hi;
        public decimal? PVProd_Multipler;

        public int? FromGrid_RegNo_TotalLo; // kWh
        public int? FromGrid_RegNo_TotalHi;
        public decimal? FromGrid_Multipler;

        public int? ToGrid_RegNo_TotalLo; // kWh
        public int? ToGrid_RegNo_TotalHi;
        public decimal? ToGrid_Multipler;

        public int? Load_RegNo_TotalLo; // kWh
        public int? Load_RegNo_TotalHi;
        public decimal? Load_Multipler;


        /// <summary>
        /// to load whole set of registers during getting of statistic. Null - read one by one. 
        /// Registers out of this set will be read one-by-one
        /// </summary>
        public int? FastRead1_RegStart;
        public int? FastRead1_RegCount;


        // ======================================
        // Write

        public int? MaxACharge;
        public int? Deya_TimeOfUser_RegNo;
        public int? Deya_WorkMode_RegNo;

        public enum Deya_Modes
        {
            i00_SellingFirst=0,
            i01_ZeroExportToLoad=1,
            i02_ZeroExportToCT=2,
        }


        // ======================================
        // Creating list
        // ======================================


        static List<InverterInfo>? Cache;
        public static List<InverterInfo> OurGetInverterInfos()
        {
            if (Cache==null)
            {
                Cache = new List<InverterInfo>()
                {
                    new InverterInfo(0, "Deye SUN-xK-SG0xLP3 - 3 fazowy", Drivers.i000_SolarmanV5)
                    {
                        RegisterNo_SOC = 588,

                        PVProd_RegNo_Lo = 534,
                        PVProd_RegNo_Hi = 535,
                        PVProd_Multipler = 0.1m,

                        FromGrid_RegNo_TotalLo = 522,
                        FromGrid_RegNo_TotalHi = 523,
                        FromGrid_Multipler = 0.1m,

                        ToGrid_RegNo_TotalLo = 524,
                        ToGrid_RegNo_TotalHi = 525,
                        ToGrid_Multipler = 0.1m,

                        Load_RegNo_TotalLo = 527,
                        Load_RegNo_TotalHi = 528,
                        Load_Multipler = 0.1m,

                        FastRead1_RegStart = 522,
                        FastRead1_RegCount = 535-522+1, // = 14

                        MaxACharge = 108,
                        Deya_TimeOfUser_RegNo = 148,
                        Deya_WorkMode_RegNo = 142,
                    },
                    new InverterInfo(1, "Deya SUN-xK-SG0xLP1 - 1 fazowy (Beta)", Drivers.i000_SolarmanV5)
                    {
                        RegisterNo_SOC = 184,

                        PVProd_RegNo_Lo = 96,
                        PVProd_RegNo_Hi = 97,
                        PVProd_Multipler = 0.1m,

                        FromGrid_RegNo_TotalLo = 78,
                        FromGrid_RegNo_TotalHi = 79,
                        FromGrid_Multipler = 0.1m,

                        ToGrid_RegNo_TotalLo = 81,
                        ToGrid_RegNo_TotalHi = 82,
                        ToGrid_Multipler = 0.1m,

                        Load_RegNo_TotalLo = 85,
                        Load_RegNo_TotalHi = 86,
                        Load_Multipler = 0.1m,

                        FastRead1_RegStart = 78,
                        FastRead1_RegCount = 86-78+1, // ==7

                        MaxACharge = 210,
                        Deya_TimeOfUser_RegNo = 250,
                        Deya_WorkMode_RegNo = 244, // limit control function
                    },
#if DEBUG
                    new InverterInfo(999, "Random", Drivers.i999_Random)
                    {
                        RegisterNo_SOC = 0,

                        PVProd_RegNo_Lo = 1,
                        PVProd_RegNo_Hi = null,

                        FromGrid_RegNo_TotalLo = 2,
                        FromGrid_RegNo_TotalHi = null,

                        ToGrid_RegNo_TotalLo = 3,
                        ToGrid_RegNo_TotalHi = null,

                        Load_RegNo_TotalLo = 4,
                        Load_RegNo_TotalHi = null,

                        FastRead1_RegStart = 0,
                        FastRead1_RegCount = 5,

                        // no write!

                    },

#endif
                };
            }
            return Cache;
        }

        public static InverterInfo OurGetInverterInfoByNumber(int Number)
        {
            foreach (var itm in OurGetInverterInfos())
                if (itm.Number == Number)
                    return itm;

            throw new ApplicationException("Unknown InverterInfoId=" + Number);
        }

    }
}
