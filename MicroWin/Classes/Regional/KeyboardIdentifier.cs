using MicroWin.functions.Helpers.ProgramResources;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Xml;

namespace MicroWin.Classes.Regional
{
    public class KeyboardIdentifier
    {

        public required string Id;

        public required string DisplayName;

        public required string TypeString;

        public KeyboardType GetKeyboardType()
        {
            return TypeString switch
            {
                "Keyboard" => KeyboardType.Keyboard,
                "IME" => KeyboardType.IME,
                _ => KeyboardType.Unknown
            };
        }

        public override string ToString() {
            return $"Keyboard {Id} -- {DisplayName} -- {GetKeyboardType()}";
        }

        public static List<KeyboardIdentifier> LoadItems() {
            List<KeyboardIdentifier> keyboardList = [];

            try
            {
                XmlReaderSettings xs = new() { IgnoreWhitespace = true };
#pragma warning disable CS8604
                using XmlReader reader = XmlReader.Create(ResourceHelper.GetProgramResourceStream("MicroWin.Resources.Regional.KeyboardIdentifier.xml"), xs);
#pragma warning restore CS8604
                while (reader.Read())
                {
                    if (reader.NodeType == XmlNodeType.Element && reader.Name == "KeyboardIdentifier")
                    {
                        keyboardList.Add(new() { Id = reader.GetAttribute("Id") ?? "", DisplayName = reader.GetAttribute("DisplayName") ?? "", TypeString = reader.GetAttribute("Type") ?? "" });
                    }
                }
            }
            catch (Exception ex)
            {
                if (Debugger.IsAttached)
                    Debugger.Break();
                Debug.WriteLine(ex.Message);
            }
            return keyboardList;
        }
    }

    public enum KeyboardType : int {
        Unknown = -1,
        Keyboard = 0,
        IME = 1
    }
}
