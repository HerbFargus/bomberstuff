//
// SettingsReader.cs - SettingsReader class
//
// Copyright © 2009-2010  Thomas Faber
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

using System;
using System.Xml;
using System.Xml.Schema;

using Size = System.Drawing.Size;
using ColorRemapInfo = BomberStuff.Core.Utilities.ColorRemapInfo;

using BomberStuff.Core;

namespace BomberStuff.Files
{
	/// <summary>
	/// Reads game settings from an XML file
	/// </summary>
	public static class SettingsReader
	{
		/// <summary>
		/// 
		/// </summary>
		/// <param name="filename"></param>
		/// <returns></returns>
		public static Settings ReadFile(string filename)
		{
			Settings settings = new Settings();

			XmlSchemaSet schemaSet = new XmlSchemaSet();
			schemaSet.Add(null, "settings.xsd");

			XmlReaderSettings readerSettings = new XmlReaderSettings();
			readerSettings.ValidationType = ValidationType.Schema;
			readerSettings.ValidationFlags = XmlSchemaValidationFlags.ReportValidationWarnings;
			readerSettings.ValidationEventHandler += new ValidationEventHandler(readerSettings_ValidationEventHandler);
			readerSettings.Schemas = schemaSet;

			XmlReader reader = XmlReader.Create(filename, readerSettings);
		
			reader.Read();
			reader.ReadStartElement("Settings");
			while (reader.IsStartElement())
			{
				string type = reader.Name;
				string name = reader.GetAttribute("name");
				int index = -1;

				// the XML file is validated. We don't mind throwing an
				// exception if index isn't an integer
				if (reader.AttributeCount >= 2)
					index = int.Parse(reader.GetAttribute("index"));

				reader.ReadStartElement();

				switch (type)
				{
					case "String":
					{
						string value = reader.ReadString().Trim();
						
						AddString(settings, name, value);

						break;
					}
					case "Int":
					case "NInt":
					case "UInt":
					{
						int value = reader.ReadContentAsInt();

						AddInt(settings, name, value);

						break;
					}
					case "Size":
					{
						reader.ReadStartElement("Width");
						int width = reader.ReadContentAsInt();
						reader.ReadEndElement();
						reader.ReadStartElement("Height");
						int height = reader.ReadContentAsInt();
						reader.ReadEndElement();
						//System.Console.WriteLine(reader.NodeType + " node: " + reader.Name);

						AddSize(settings, name, width, height);

						break;
					}
					case "PlayerControls":
					{
						if (reader.IsStartElement())
						{
							// read Up-key
							if (reader.Name == "Up")
							{
								reader.ReadStartElement();

								if (!reader.IsEmptyElement)
									AddString(settings, "Player" + index + ".Up", reader.ReadString());								

								reader.ReadEndElement();
							}

							// skip whitespace
							while (reader.NodeType == XmlNodeType.Whitespace || reader.NodeType == XmlNodeType.Comment)
								reader.Read();

							// read Down-key
							if (reader.Name == "Down")
							{
								reader.ReadStartElement();

								if (!reader.IsEmptyElement)
									AddString(settings, "Player" + index + ".Down", reader.ReadString());

								reader.ReadEndElement();
							}

							// skip whitespace
							while (reader.NodeType == XmlNodeType.Whitespace || reader.NodeType == XmlNodeType.Comment)
								reader.Read();

							// read Left-key
							if (reader.Name == "Left")
							{
								reader.ReadStartElement();

								if (!reader.IsEmptyElement)
									AddString(settings, "Player" + index + ".Left", reader.ReadString());

								reader.ReadEndElement();
							}

							// skip whitespace
							while (reader.NodeType == XmlNodeType.Whitespace || reader.NodeType == XmlNodeType.Comment)
								reader.Read();

							// read Right-key
							if (reader.Name == "Right")
							{
								reader.ReadStartElement();

								if (!reader.IsEmptyElement)
									AddString(settings, "Player" + index + ".Right", reader.ReadString());

								reader.ReadEndElement();
							}

							// skip whitespace
							while (reader.NodeType == XmlNodeType.Whitespace || reader.NodeType == XmlNodeType.Comment)
								reader.Read();

							// read Action1-key
							if (reader.Name == "Action1")
							{
								reader.ReadStartElement();

								if (!reader.IsEmptyElement)
									AddString(settings, "Player" + index + ".Action1", reader.ReadString());

								reader.ReadEndElement();
							}

							// skip whitespace
							while (reader.NodeType == XmlNodeType.Whitespace || reader.NodeType == XmlNodeType.Comment)
								reader.Read();

							// read Action2-key
							if (reader.Name == "Action2")
							{
								reader.ReadStartElement();

								if (!reader.IsEmptyElement)
									AddString(settings, "Player" + index + ".Action2", reader.ReadString());

								reader.ReadEndElement();
							}
						}

						break;
					}
					case "ColorRemapInfo":
					{
						int hue = -1,
							saturation = -1,
							diffLightness = 0;

						if (reader.IsStartElement())
						{
							// read hue, if present
							if (reader.Name == "Hue")
							{
								reader.ReadStartElement("Hue");
								
								if (!reader.IsEmptyElement)
									hue = reader.ReadContentAsInt();

								reader.ReadEndElement();
							}

							// skip whitespace
							while (reader.NodeType == XmlNodeType.Whitespace || reader.NodeType == XmlNodeType.Comment)
								reader.Read();

							// read saturation, if present
							if (reader.Name == "Saturation")
							{
								reader.ReadStartElement("Saturation");

								if (!reader.IsEmptyElement)
									saturation = reader.ReadContentAsInt();

								reader.ReadEndElement();
							}

							// skip whitespace
							while (reader.NodeType == XmlNodeType.Whitespace || reader.NodeType == XmlNodeType.Comment)
								reader.Read();

							// read lightness difference, if present
							if (reader.Name == "LightnessDifference")
							{
								reader.ReadStartElement("LightnessDifference");
								
								if (!reader.IsEmptyElement)
									diffLightness = reader.ReadContentAsInt();

								reader.ReadEndElement();
							}
						}

						AddSetting<ColorRemapInfo>(settings, name, index,
										new ColorRemapInfo(hue != -1, hue,
													saturation != -1, saturation,
													diffLightness));

						break;
					}
					default:
					{
						System.Console.WriteLine("Got a " + reader.Name);
						break;
					}				
				}

				//System.Console.WriteLine(reader.NodeType + " node: " + reader.Name);
				if (reader.NodeType != XmlNodeType.Element)
					reader.ReadEndElement();
			}
			reader.ReadEndElement();

			reader.Close();

			return settings;
		}

		static void AddString(Settings settings, string name, string value)
		{
			try
			{
				settings.Set<string>((Settings.Types)Enum.Parse(typeof(Settings.Types), name, true), value);
			}
			catch (ArgumentException)
			{
				settings.Set<string>(name, value);
			}
		}

		static void AddInt(Settings settings, string name, int value)
		{
			try
			{
				settings.Set<int>((Settings.Types)Enum.Parse(typeof(Settings.Types), name, true), value);
			}
			catch (ArgumentException)
			{
				settings.Set<int>(name, value);
			}
		}

		static void AddSize(Settings settings, string name, int width, int height)
		{
			try
			{
				settings.Set<Size>((Settings.Types)Enum.Parse(typeof(Settings.Types), name, true), new Size(width, height));
			}
			catch (ArgumentException)
			{
				settings.Set<Size>(name, new Size(width, height));
			}
		}

		static void AddSetting<T>(Settings settings, string name, T value)
		{
			try
			{
				settings.Set<T>((Settings.Types)Enum.Parse(typeof(Settings.Types), name, true), value);
			}
			catch (ArgumentException)
			{
				settings.Set<T>(name, value);
			}
		}

		static void AddSetting<T>(Settings settings, string name, int index, T value)
		{
			if (index == -1)
				AddSetting<T>(settings, name, value);
			
			else try
			{
				Settings.Types type = (Settings.Types)Enum.Parse(typeof(Settings.Types), name, true);

				T[] arr = settings.Get<T[]>(type);

				if (arr == null)
					arr = new T[index + 1];
				else if (arr.Length <= index)
				{
					T[] oldArr = arr;
					arr = new T[index + 1];
					Array.Copy(oldArr, arr, oldArr.Length);
				}

				arr[index] = value;

				settings.Set<T[]>(type, arr);
			}
			catch (ArgumentException)
			{
				T[] arr = settings.Get<T[]>(name);

				if (arr == null)
					arr = new T[index + 1];
				else if (arr.Length <= index)
				{
					T[] oldArr = arr;
					arr = new T[index + 1];
					Array.Copy(oldArr, arr, oldArr.Length);
				}

				arr[index] = value;

				settings.Set<T[]>(name, arr);
			}
		}

		static void readerSettings_ValidationEventHandler(object sender, ValidationEventArgs e)
		{
			System.Console.WriteLine("Validation Error: {0}", e.Message);
			throw e.Exception;
		}
	}
}
