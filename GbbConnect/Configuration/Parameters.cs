using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace GbbConnect.Configuration
{
    public partial class Parameters : ObservableObject
    {
        public PlantList Plants { get; set; } = new();

        [ObservableProperty]
        private Plant? m_CurrPlant;


        // ======================================
        const int VERSION = 1;
        public void WriteToXML(XmlWriter xml)
        {
            xml.WriteStartElement("Parameters");
            xml.WriteAttributeString("Version", VERSION.ToString());
            if (CurrPlant!= null)
                xml.WriteAttributeString("CurrPlant", XmlConvert.ToString(Plants.IndexOf(CurrPlant)));

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
                //int i;

                //string? sCurrentGame = xml.GetAttribute("CurrentGame");

                // for later
                s = xml.GetAttribute("CurrLoadProfile");


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

                // curr plant
                if (s != null)
                {
                    int index = XmlConvert.ToInt32(s);
                    if (index >= 0 && index < ret.Plants.Count)
                        ret.CurrPlant = ret.Plants[index];
                }


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

            // curr plant
            if (ret.CurrPlant == null && ret.Plants.Count > 0)
                ret.CurrPlant = ret.Plants[0];

            //// create forecasts
            //foreach (var itm in ret.Plants)
            //    itm.OurRecalcForecast();



            return ret;
        }

        // ======================================

    }
}
