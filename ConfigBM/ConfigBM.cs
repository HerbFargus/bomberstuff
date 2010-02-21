//
// ConfigBM.cs - Graphical Configuration Utility
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

using BomberStuff.Core;
using BomberStuff.Core.Utilities;
using System.Collections.Generic;
using System.Windows.Forms;


namespace BomberStuff.ConfigBM
{
	/// <summary>
	/// 
	/// </summary>
	public static class ConfigBM
	{
		/// <summary>
		/// 
		/// </summary>
		[System.STAThread]
		private static void Main()
		{
			// load settings
			Settings settings;

			settings = BomberStuff.Files.SettingsReader.ReadFile("settings.xml");

			if (settings.Get<string>("GotSettings") != "true")
				return;

			if (settings.Get<string>(Settings.Types.ABDirectory) == null)
				settings.Set<string>(Settings.Types.ABDirectory, @"C:\Temp\atomic_bomberman\bomber");

			System.Console.WriteLine("Settings found:");
			foreach (KeyValuePair<string, object> setting in settings)
				System.Console.WriteLine("{0}: {1}", setting.Key, setting.Value);
			System.Console.WriteLine();

			string uiName = settings.Get<string>(Settings.Types.UserInterface);

			Application.Run(new ConfigForm(settings));
		}
	}
}
