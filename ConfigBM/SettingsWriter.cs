//
// SettingsWriter.cs - SettingsWriter class
//
// Copyright © 2010  Thomas Faber
//
// This file is part of Bomber Stuff.
//
// Bomber Stuff is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// Bomber Stuff is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with Bomber Stuff. If not, see <http://www.gnu.org/licenses/>.
//

using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Xml.Schema;

using Size = System.Drawing.Size;
using ColorRemapInfo = BomberStuff.Core.Utilities.ColorRemapInfo;

using BomberStuff.Core;

namespace BomberStuff.ConfigBM
{
	internal static class SettingsWriter
	{
		public static void WriteFile(Settings settings, string filename)
		{
			XmlWriter writer = new XmlTextWriter(filename, new UTF8Encoding(false));

			writer.WriteStartDocument();
			writer.WriteWhitespace(System.Environment.NewLine);
			
			writer.WriteStartElement("Settings", "BomberStuff");
			writer.WriteAttributeString("schemaLocation", "http://www.w3.org/2001/XMLSchema-instance", "BomberStuff settings.xsd");
			writer.WriteWhitespace(System.Environment.NewLine);

			List<Dictionary<string, string>> playerKeys = new List<Dictionary<string,string>>(10);

			foreach (KeyValuePair<string, object> setting in settings)
			{
				if (setting.Value is ColorRemapInfo[])
					WriteColorRemapInfoArray(writer, setting.Key, (ColorRemapInfo[])setting.Value);
				else if (setting.Value is Size)
					WriteSize(writer, setting.Key, (Size)setting.Value);
				else if (setting.Value is int)
				{
					if (setting.Key != "Tileset" || (int)setting.Value != -1)
						WriteInt(writer, setting.Key, (int)setting.Value);
				}
				else if (setting.Key.StartsWith("Player") && setting.Key.Contains("."))
				{
					string name = setting.Key.Substring("Player".Length);
					int dot = name.IndexOf('.');
					int index = int.Parse(name.Substring(0, dot));
					string key = name.Substring(dot + 1);

					while (playerKeys.Count < index + 1)
						playerKeys.Add(new Dictionary<string, string>());

					playerKeys[index].Add(key, setting.Value.ToString());
				}
				else
					WriteString(writer, setting.Key, setting.Value.ToString());
			}

			for (int i = 0; i < playerKeys.Count; ++i)
				WritePlayerKeys(writer, i, playerKeys[i]);

			writer.WriteEndElement();


			writer.Close();
		}

		private static void WritePlayerKeys(XmlWriter writer, int index, Dictionary<string, string> value)
		{
			writer.WriteStartElement("PlayerControls");
			writer.WriteAttributeString("name", "PlayerControls");
			writer.WriteAttributeString("index", index.ToString());

			foreach (KeyValuePair<string, string> control in value)
				writer.WriteElementString(control.Key, control.Value);

			writer.WriteEndElement();
			writer.WriteWhitespace(System.Environment.NewLine);
		}

		private static void WriteColorRemapInfoArray(XmlWriter writer, string name, ColorRemapInfo[] value)
		{
			for (int i = 0; i < value.Length; ++i)
				WriteColorRemapInfo(writer, name, i, value[i]);
		}

		private static void WriteColorRemapInfo(XmlWriter writer, string name, int index, ColorRemapInfo value)
		{
			writer.WriteStartElement("ColorRemapInfo");
			writer.WriteAttributeString("name", name);
			writer.WriteAttributeString("index", index.ToString());

			if (value.SetHue)
				writer.WriteElementString("Hue", value.NewHue.ToString());
			if (value.SetSaturation)
				writer.WriteElementString("Saturation", value.NewSaturation.ToString());

			if (value.DiffLightness != 0)
				writer.WriteElementString("LightnessDifference", value.DiffLightness.ToString());

			writer.WriteEndElement();
			writer.WriteWhitespace(System.Environment.NewLine);
		}

		private static void WriteSize(XmlWriter writer, string name, Size value)
		{
			writer.WriteStartElement("Size");
			writer.WriteAttributeString("name", name);
			writer.WriteElementString("Width", value.Width.ToString());
			writer.WriteElementString("Height", value.Height.ToString());
			writer.WriteEndElement();
			writer.WriteWhitespace(System.Environment.NewLine);
		}

		private static void WriteInt(XmlWriter writer, string name, int value)
		{
			writer.WriteStartElement("Int");
			writer.WriteAttributeString("name", name);
			writer.WriteString(value.ToString());
			writer.WriteEndElement();
			writer.WriteWhitespace(System.Environment.NewLine);
		}

		private static void WriteString(XmlWriter writer, string name, string value)
		{
			writer.WriteStartElement("String");
			writer.WriteAttributeString("name", name);
			writer.WriteString(value);
			writer.WriteEndElement();
			writer.WriteWhitespace(System.Environment.NewLine);
		}
	}
}