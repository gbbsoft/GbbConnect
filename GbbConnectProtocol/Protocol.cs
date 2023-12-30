namespace GbbConnectProtocol;

public class Request
{
    public Request(string operation) { Operation = operation; }

    public string Operation { get; set; }

    public DateTime? FromDate { get; set; }
    public DateTime? ToDate { get; set; }

    public List<Request_Scheduler>? Schedulers { get; set; }


}

public class Request_Scheduler
{
    public bool IsFastDischarge; // for internal use

    public int Hour { get; set; }
    public int FromMinute { get; set; } = 0;
    public int ToMinute { get; set; } = 59;
    public long? ChargeLimitW { get; set; }
    public long? InputLimitW { get; set; }
    public int PriceLessZero { get; set; }

    public string? Operation { get; set; } // "Normal", "Discharge", "DisableCharge", "Charge"
    public decimal? SOC { get; set; }


}

public class Response
{
    public string? Operation { get; set; }
    public string? Status { get; set; }
    public string? ErrDesc { get; set; }
    public decimal? SOC { get; set; }
    public DateTime? FromDate { get; set; }
    public DateTime? ToDate { get; set; }
    public List<Response_Statistic>? Statistics { get; set; }

}

public class Response_Statistic
{
    public DateTime Day { get; set; }
    public int Hour { get; set; }

    public decimal? SOC { get; set; }
    public decimal? MinSOC { get; set; }
    public decimal? MaxSOC { get; set; }
    public decimal? AvrSOC { get; set; }


    public decimal? FromGridkWh { get; set; }
    public decimal? ToGridkWh { get; set; }
    public decimal? LoadskWh { get; set; }
    public decimal? PVProdkWh { get; set; }


    public decimal? ExtraLoads_EV { get; set;}
    public decimal? ExtraLoads_HeatingPump { get; set;}
    public decimal? ExtraLoads_Generic1 { get; set;}
    public decimal? ExtraLoads_Generic2 { get; set;}

}