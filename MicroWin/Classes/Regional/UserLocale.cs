using MicroWin.functions.Helpers.ProgramResources;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace MicroWin.Classes.Regional
{
    public class UserLocale
    {
        [XmlAttribute("Id")]
        public required string Id;
        [XmlAttribute("DisplayName")]
        public required string DisplayName;
        [XmlAttribute("LCID")]
        public required string LCID;
        [XmlAttribute("KeyboardLayout")]
        public string? KeyboardLayout;
        [XmlAttribute("GeoLocation")]
        public string? GeoLocation;

        public override string ToString()
        {
            return $"User Locale {Id} -- {DisplayName} -- {LCID} -- {KeyboardLayout} -- {GeoLocation}";
        }

        public UserLocale Clone() {
            return (UserLocale)MemberwiseClone();
        }

        public static List<UserLocale> LoadItems() {
            List<UserLocale> localeList = [];

            try
            {
                XmlReaderSettings xs = new() { IgnoreWhitespace = true };
#pragma warning disable CS8604
                using XmlReader reader = XmlReader.Create(ResourceHelper.GetProgramResourceStream("MicroWin.Resources.Regional.UserLocale.xml"), xs);
#pragma warning restore CS8604
                while (reader.Read())
                {
                    if (reader.NodeType == XmlNodeType.Element && reader.Name == "UserLocale")
                    {
                        localeList.Add(new() { Id = reader.GetAttribute("Id") ?? "", DisplayName = reader.GetAttribute("DisplayName") ?? "",
                                               LCID = reader.GetAttribute("LCID") ?? "", KeyboardLayout = reader.GetAttribute("KeyboardLayout") ?? "",
                                               GeoLocation = reader.GetAttribute("GeoLocation") ?? "" });
                    }
                }
            }
            catch (Exception ex)
            {
                if (Debugger.IsAttached)
                    Debugger.Break();
                Debug.WriteLine(ex.Message);
            }

            return localeList;
        }

    }
}
