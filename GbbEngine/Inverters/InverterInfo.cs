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

        public InverterInfo(int Number, string name)
        { 
            this.Number = Number;   
            this.Name = name; 
        }


        public int? RegisterNo_SOC;

        public int? RegisterNo_PVProdLo; // kWh
        public int? RegisterNo_PVProdHi;

        public int? RegisterNo_TotalFromGridLo; // kWh
        public int? RegisterNo_TotalFromGridHi; 

        public int? RegisterNo_TotalToGridLo; // kWh
        public int? RegisterNo_TotalToGridHi;

        public int? RegisterNo_TotalLoadLo; // kWh
        public int? RegisterNo_TotalLoadHi;

        
        /// <summary>
        /// to load one set of registers during getting of statistic. Null - read one by one
        /// </summary>
        public int? StatisticCache_RegStart;
        public int? StatisticCache_RefCount;


        public static List<InverterInfo> OurGetInverterInfos()
        {
            return new List<InverterInfo>()
            {
                new InverterInfo(0, "Deya SUN-5/6/8/10/12K-SG04LP3")
                {
                    RegisterNo_SOC = 588,

                    RegisterNo_PVProdLo = 534,
                    RegisterNo_PVProdHi = 535,

                    RegisterNo_TotalFromGridLo = 522,
                    RegisterNo_TotalFromGridHi = 523,

                    RegisterNo_TotalToGridLo = 524,
                    RegisterNo_TotalToGridHi = 525,

                    RegisterNo_TotalLoadLo = 527,
                    RegisterNo_TotalLoadHi = 528,

                    StatisticCache_RegStart = 522,
                    StatisticCache_RefCount = 528-535+1,

                }
            };
        }

    }
}
