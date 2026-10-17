using MicroWin.functions.Helpers.ProgramResources;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace MicroWin.Classes.Regional
{
    public class GeoId
    {
        [XmlAttribute("Id")]
        public required string Id;

        [XmlAttribute("DisplayName")]
        public required string DisplayName;

        public override string ToString() {
            return $"GeoID {Id} -- {DisplayName}";
        }

        public static List<GeoId> LoadItems() {
            List<GeoId> geoList = [];

            try {
                XmlReaderSettings xs = new() { IgnoreWhitespace = true };
#pragma warning disable CS8604
                using XmlReader reader = XmlReader.Create(ResourceHelper.GetProgramResourceStream("MicroWin.Resources.Regional.GeoId.xml"), xs);
#pragma warning restore CS8604
                while (reader.Read()) {
                    if (reader.NodeType == XmlNodeType.Element && reader.Name == "GeoId") {
                        geoList.Add(new() { Id = reader.GetAttribute("Id") ?? "", DisplayName = reader.GetAttribute("DisplayName") ?? "" });
                    }
                }
            } catch (Exception ex) {
                if (Debugger.IsAttached)
                    Debugger.Break();
                Debug.WriteLine(ex.Message);
            }

            return geoList;
        }
    }
}
