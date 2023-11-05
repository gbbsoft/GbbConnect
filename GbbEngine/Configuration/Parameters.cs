using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace GbbEngine.Configuration
{
    public partial class Parameters : ObservableObject
    {
        public const string APP_VERSION = "1.2.1";

        // ======================================
        public PlantList Plants { get; set; } = new();

        //[ObservableProperty]
        //private Plant? m_CurrPlant;

        [ObservableProperty]
        private string? m_GbbVictronWeb_Mqtt_Address = "gbbconnect-mqtt.gbbsoft.pl";

        [ObservableProperty]
        private int? m_GbbVictronWeb_Mqtt_Port = 8883;

        [ObservableProperty]
        private bool m_Server_AutoStart;

        [ObservableProperty]
        private bool m_IsVerboseLog;

        [ObservableProperty]
        private bool m_IsDriverLog;

        [ObservableProperty]
        private bool m_IsDriverLog2;



        // ======================================
        // Save and Load
        // ======================================

        const int VERSION = 1;
        public void WriteToXML(XmlWriter xml)
        {
            xml.WriteStartElement("Parameters");
            xml.WriteAttributeString("Version", VERSION.ToString());

            if (GbbVictronWeb_Mqtt_Address!=null)
                xml.WriteAttributeString("GbbVictronWeb_Mqtt_Address", GbbVictronWeb_Mqtt_Address);
            if (GbbVictronWeb_Mqtt_Port!=null)
                xml.WriteAttributeString("GbbVictronWeb_Mqtt_Port", GbbVictronWeb_Mqtt_Port.ToString());

            xml.WriteAttributeString("Server_AutoStart", Server_AutoStart ? "1" : "0");
            xml.WriteAttributeString("IsVerboseLog", IsVerboseLog ? "1" : "0");
            xml.WriteAttributeString("IsDriverLog", IsDriverLog ? "1" : "0");
            xml.WriteAttributeString("IsDriverLog2", IsDriverLog2 ? "1" : "0");


            //if (CurrPlant!= null)
            //    xml.WriteAttributeString("CurrPlant", XmlConvert.ToString(Plants.IndexOf(CurrPlant)));

            foreach (var itm in Plants)
                itm.WriteToXML(xml);

            xml.WriteEndElement();
        }

        public static Parameters ReadFromXML(XmlReader xml)
        {
            Parameters ret = new();

            if (xml.IsStartElement("Parameters"))
            {
                int Version = int.Parse(xml.GetAttribute("Version") ?? "");
                if (Version > VERSION)
                    throw new ApplicationException("Can't read Parameters from newer program version!");

                string? s;
                int i;

                ret.GbbVictronWeb_Mqtt_Address = xml.GetAttribute("GbbVictronWeb_Mqtt_Address");

                s = xml.GetAttribute("GbbVictronWeb_Mqtt_Port");
                if (s != null && int.TryParse(s, out i))
                    ret.GbbVictronWeb_Mqtt_Port = i;

                s = xml.GetAttribute("Server_AutoStart");
                if (s != null)
                    ret.Server_AutoStart = s=="1";

                s = xml.GetAttribute("IsVerboseLog");
                if (s != null)
                    ret.IsVerboseLog= s=="1";

                s = xml.GetAttribute("IsDriverLog");
                if (s != null)
                    ret.IsDriverLog= s=="1";

                s = xml.GetAttribute("IsDriverLog2");
                if (s != null)
                    ret.IsDriverLog2= s=="1";


                //// for later
                //s = xml.GetAttribute("CurrLoadProfile");


                if (!xml.IsEmptyElement)
                {
                    xml.Read();
                    while (xml.NodeType != XmlNodeType.EndElement && xml.NodeType != XmlNodeType.None)
                    {
                        if (xml.IsStartElement("Plant"))
                            ret.Plants.Add(Plant.ReadFromXML(xml));
                        else
                            xml.Skip();

                        xml.MoveToContent();
                    }
                    xml.ReadEndElement();
                }
                else
                    xml.Skip();

                //// curr plant
                //if (s != null)
                //{
                //    int index = XmlConvert.ToInt32(s);
                //    if (index >= 0 && index < ret.Plants.Count)
                //        ret.CurrPlant = ret.Plants[index];
                //}


            }
            return ret;
        }
        // ======================================
        public void Save(string FileName) // SaveLocaly
        {
            var tmpFileName = FileName + ".tmp";
            if (System.IO.File.Exists(tmpFileName))
                System.IO.File.Delete(tmpFileName);

            // Create
            var param = new System.Xml.XmlWriterSettings();
            param.Indent = true;
            using var xml = System.Xml.XmlWriter.Create(tmpFileName, param);

            xml.WriteStartDocument();
            WriteToXML(xml);
            xml.WriteEndDocument();

            xml.Close();

            System.IO.File.Move(tmpFileName, FileName, true);

        }



        public static Parameters Load(string FileName)
        {


            Parameters ret;

            if (System.IO.File.Exists(FileName))
            {
                // Parse
                var param = new System.Xml.XmlReaderSettings();
                using var xml = System.Xml.XmlReader.Create(FileName, param);

                ret = ReadFromXML(xml);

            }
            else
            {
                ret = new();
            }


            // init plants
            if (ret.Plants.Count == 0)
            {
                var p = new Plant();
                p.Name = "My Main Plant";
                ret.Plants.Add(p);
            }

            //// curr plant
            //if (ret.CurrPlant == null && ret.Plants.Count > 0)
            //    ret.CurrPlant = ret.Plants[0];

            //// create forecasts
            //foreach (var itm in ret.Plants)
            //    itm.OurRecalcForecast();



            return ret;
        }

        // ======================================
        // Base directory
        // ======================================

        /// <summary>
        /// Base directory for program public data
        /// </summary>
        /// <returns></returns>
        public static string OurGetUserBaseDirectory()
        {
            var Dir=Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            Dir = Path.Combine(Dir, "GbbConnect");
            Directory.CreateDirectory(Dir);
            return Dir;
        }


        /// <summary>
        /// Old place for moving to new place
        /// </summary>
        /// <returns></returns>
        public static string OurGetMainDataDir_Old()
        {
            string mainDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Gbb Software", "GbbConnect");
            Directory.CreateDirectory(mainDir);
            return mainDir;
        }

        public static string Parameters_GetFileName()
        {
            return System.IO.Path.Combine(OurGetUserBaseDirectory(), "Parameters.xml");
        }

    }
}
