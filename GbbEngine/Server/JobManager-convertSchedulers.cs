using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GbbEngine.Server
{

    public class Deya_TimeOfUse
    {
        public int FromTime;

        public bool IsSellingFirst;

        public int SOC;

        public bool IsGridCharging;

        public int? Power;

        public bool IsZeroChargeA;

        public override bool Equals(object? obj)
        {
            if (obj == null) return false;
            Deya_TimeOfUse itm = (Deya_TimeOfUse)obj;   

            //if (itm.FromTime!= this.FromTime) return false;
            if (itm.IsSellingFirst!= this.IsSellingFirst) return false;
            if (itm.SOC!= this.SOC) return false;
            if (itm.IsGridCharging!= this.IsGridCharging) return false;
            if ((itm.Power!=null )!= (this.Power!=null)) return false;
            if (itm.Power!= this.Power) return false;
            if (itm.IsZeroChargeA!= this.IsZeroChargeA) return false;

            return true;
        }
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

    }

    public partial class JobManager
    {

        public List<Deya_TimeOfUse> ConvertSchedulers(List<GbbConnectProtocol.Request_Scheduler> Schedulers)
        {
            List<Deya_TimeOfUse> ret = new();
            Deya_TimeOfUse? Last = null;
            int? LastToMinute = null;

            foreach (var itm in Schedulers)
            {
                Deya_TimeOfUse Next = new();

                if (itm.FromMinute == 0 && LastToMinute != null)
                    Next.FromTime = (itm.Hour - 1) * 100 + LastToMinute.Value+1;
                else
                    Next.FromTime = itm.Hour * 100;

                switch(itm.Operation)
                {
                    case "Normal":
                        Next.IsSellingFirst = false;
                        Next.SOC = 5;
                        Next.Power = null;
                        Next.IsZeroChargeA = false;
                        break;

                    case "Charge":
                        Next.IsSellingFirst = false;
                        if (itm.SOC!=null)
                            Next.SOC = (int)itm.SOC.Value;
                        Next.Power = (int?)itm.ChargeLimitW;
                        Next.IsZeroChargeA = false;
                        break;

                    case "Discharge":
                        Next.IsSellingFirst = true;
                        if (itm.SOC!=null)
                            Next.SOC = (int)itm.SOC.Value;

                        Next.Power = null;
                        Next.IsZeroChargeA = false;
                        break;

                    case "DisableDischarge":
                        Next.IsSellingFirst = true;
                        Next.SOC = 5;
                        Next.Power = null;
                        Next.IsZeroChargeA = true;
                        break;

                    default:
                        throw new ApplicationException("Unknown operation: " + itm.Operation);

                }

                // join the same rows
                if (Last==null || !Last.Equals(Next))
                {
                    ret.Add(Next);
                    Last = Next;
                }

                if (itm.ToMinute != 59)
                    LastToMinute = itm.ToMinute;
            }

            // join first and last
            if (Last != null && ret.Count > 1 && Last.Equals(ret[0]))
                ret.RemoveAt(0);


            return ret;

        }
        

    }
}
