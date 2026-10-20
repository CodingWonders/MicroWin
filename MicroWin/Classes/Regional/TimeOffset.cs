using MicroWin.functions.Helpers.ProgramResources;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace MicroWin.Classes.Regional
{
    public class TimeOffset
    {
        [XmlAttribute("Id")]
        public required string Id;

        [XmlAttribute("DisplayName")]
        public required string DisplayName;

        public override string ToString() {
            return $"Time Offset {Id} -- {DisplayName}";
        }

        public static List<TimeOffset> LoadItems() {
            List<TimeOffset> offsetList = [];

            try
            {
                XmlReaderSettings xs = new() { IgnoreWhitespace = true };
#pragma warning disable CS8604
                using XmlReader reader = XmlReader.Create(ResourceHelper.GetProgramResourceStream("MicroWin.Resources.Regional.TimeOffset.xml"), xs);
#pragma warning restore CS8604
                while (reader.Read())
                {
                    if (reader.NodeType == XmlNodeType.Element && reader.Name == "TimeOffset")
                    {
                        offsetList.Add(new() { Id = reader.GetAttribute("Id") ?? "", DisplayName = reader.GetAttribute("DisplayName") ?? "" });
                    }
                }
            }
            catch (Exception ex)
            {
                if (Debugger.IsAttached)
                    Debugger.Break();
                Debug.WriteLine(ex.Message);
            }

            return offsetList;
        }

        public TimeOffset Clone()
        {
            return (TimeOffset)MemberwiseClone();
        }
    }
}
