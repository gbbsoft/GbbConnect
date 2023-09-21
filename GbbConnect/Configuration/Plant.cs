using CommunityToolkit.Mvvm.ComponentModel;
using System.Runtime.CompilerServices;
using System.Xml;

namespace GbbConnect.Configuration
{
    public partial class Plant : ObservableObject
    {

        [ObservableProperty]
        private string m_Name = "";

        [ObservableProperty]
        private string m_AddressIP = "";

        [ObservableProperty]
        private int m_PortNo = 502;

        [ObservableProperty]
        private long m_SerialNumber = 502;

        [ObservableProperty]
        private string? m_Password;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(Cerbo_UseDirectConn))]
        private bool m_Cerbo_UseVRMConn;



        //public ForecastList Forecasts { get; init; }

        public Plant()
        {
            //this.Forecasts = new ForecastList(this);
        }


        public bool Cerbo_UseDirectConn
        {
            get
            {
                return !Cerbo_UseVRMConn;
            }
            set
            {
                Cerbo_UseVRMConn = !value;
            }
        }

        // ======================================
        const int VERSION = 1;
        const string Key = "xLwh1WTkQX9nH8vIsg6bmS+qCQTTBlVXOvXb3WLhMcw=";
        const string IV = "/O0Hsgp3epBwF0yaawcP37==";

        public void WriteToXML(XmlWriter xml)
        {
            var cipher = GbbLib.Encryption.AES_CreateCipher(Key);

            xml.WriteStartElement("Plant");
            xml.WriteAttributeString("Version", VERSION.ToString());

            xml.WriteAttributeString("Name", Name);
            xml.WriteAttributeString("AddressIP", AddressIP);
            xml.WriteAttributeString("PortNo", PortNo.ToString());
            xml.WriteAttributeString("SerialNumber", SerialNumber.ToString());
            if (Password != null)
                xml.WriteAttributeString("Password", GbbLib.Encryption.AES_Encrypt(cipher, IV, Password));
            xml.WriteAttributeString("Cerbo_UseVRMConn", XmlConvert.ToString(Cerbo_UseVRMConn));
            
            //foreach (var itm in Forecasts)
            //    itm.WriteToXML(xml);


            xml.WriteEndElement();
        }

        public static Plant ReadFromXML(XmlReader xml)
        {
            Plant ret = new();

            if (xml.IsStartElement("Plant"))
            {
                var cipher = GbbLib.Encryption.AES_CreateCipher(Key);

                int Version = int.Parse(xml.GetAttribute("Version") ?? "");
                if (Version > VERSION)
                    throw new ApplicationException("Can't read Plant from newer program version!");

                string? s;
                int i;
                long l;

                ret.Name = xml.GetAttribute("Name") ?? "";
                ret.AddressIP= xml.GetAttribute("AddressIP") ?? "";

                s = xml.GetAttribute("Password");
                if (s != null)
                    ret.Password = GbbLib.Encryption.AES_Decrypt(cipher, IV, s);
                s = xml.GetAttribute("PortNo");
                if (s != null && int.TryParse(s, out i))
                    ret.PortNo = i;
                s = xml.GetAttribute("SerialNumber");
                if (s != null && long.TryParse(s, out l))
                    ret.SerialNumber = l;
                s = xml.GetAttribute("Cerbo_UseVRMConn");
                if (s != null)
                    ret.Cerbo_UseVRMConn = XmlConvert.ToBoolean(s);

                // for later
                //var sCurrLoadProfile = xml.GetAttribute("CurrLoadProfile");


                if (!xml.IsEmptyElement)
                {
                    //List<Schedule> schedules = new List<Schedule>();

                    xml.Read();
                    while (xml.NodeType != XmlNodeType.EndElement && xml.NodeType != XmlNodeType.None)
                    {
                        //if (xml.IsStartElement("Forecast"))
                        //    ret.Forecasts.Add(Forecast.ReadFromXML(xml));
                        //else
                            xml.Skip();

                        xml.MoveToContent();
                    }
                    xml.ReadEndElement();

                    //// sort schcedules
                    //foreach (var itm in schedules.OrderBy(q => q.Number))
                    //    ret.Schedules.Add(itm);

                }
                else
                    xml.Skip();

                // curr profile
                //if (sCurrLoadProfile != null)
                //{
                //    int index = XmlConvert.ToInt32(sCurrLoadProfile);
                //    if (index >= 0 && index < ret.LoadProfiles.Count)
                //        ret.m_CurrLoadProfile = ret.LoadProfiles[index];
                //}


            }
            return ret;
        }
        // =======================================
        // Operations
        // ======================================

        public void OurCheckDataForUI()
        {
            if (string.IsNullOrWhiteSpace(Name))
            {
                throw new ApplicationException("Name can't be empty!");
            }

            //CheckSellingPrices();

        }



    }
}