//
// Program.cs - Program class
//
// Copyright © 2009  Thomas Faber
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
using System.IO;
using System.Reflection;
using System.Collections.Generic;

using BomberStuff.Core.UserInterface;
using BomberStuff.Core.Input;
using BomberStuff.Core.Utilities;

namespace BomberStuff.Core
{
	/// <summary>
	/// 
	/// </summary>
	internal static class Program
	{
		private static Game Game;
		private static int playerCount = 0;
		private static int nParticipantsNegotiating;
		private static int[] playersRequested;
		private static void Participant_Negotiate(object sender, NegotiateEventArgs e)
		{
			int i = Game.Participants.IndexOf((Participant)sender);

			System.Diagnostics.Debug.Assert(i != -1);

			playersRequested[i] = e.NumberOfOwnPlayers;
			playerCount += e.NumberOfOwnPlayers;
			--nParticipantsNegotiating;
		}

		/// <summary>
		/// Program entry. Handles command line arguments and loads
		/// plugins accordingly
		/// </summary>
		/// <param name="args">Program arguments</param>
		private static void Main(string/*^!^*/[]/*^!^*/ args)
		{
			// load settings
			Settings settings;

			settings = BomberStuff.Files.SettingsReader.ReadFile("settings.xml");

			if (settings.Get<string>("GotSettings") != "true")
				return;

			if (settings.Get<string>(Settings.Types.ABDirectory) == null)
				settings.Set<string>(Settings.Types.ABDirectory, @"C:\Temp\atomic_bomberman\bomber");

			if (settings.Get<ColorRemapInfo[]>(Settings.Types.PlayerColor) == null)
			{
				ColorRemapInfo[] remapInfo = new ColorRemapInfo[10];
				for (int i = 0; i < remapInfo.Length; ++i)
					remapInfo[i] = PlayerColor(i);
				settings.Set<ColorRemapInfo[]>(Settings.Types.PlayerColor, remapInfo);
			}
			
			if (settings.Get<string>(Settings.Types.UserInterface) == null)
				settings.Set<string>(Settings.Types.UserInterface, "SlimDXInterface");

			if (settings.Get<object>(Settings.Types.Tileset) == null)
				settings.Set<int>(Settings.Types.Tileset, -1);

			if (settings.Get<object>(Settings.Types.PlayerCount) == null)
				settings.Set<int>(Settings.Types.PlayerCount, 1);

			if (settings.Get<string>(Settings.Types.Scheme) == null)
				settings.Set<string>(Settings.Types.Scheme, "BASIC");

			System.Console.WriteLine("Settings found:");
			foreach (KeyValuePair<string, object> setting in settings)
				System.Console.WriteLine("{0}: {1}", setting.Key, setting.Value);
			System.Console.WriteLine();

			string uiName = settings.Get<string>(Settings.Types.UserInterface);

			try
			{
				Assembly assembly = Assembly.LoadFrom(uiName + ".dll");
				object uiObject = assembly.CreateInstance("BomberStuff." + uiName + "." + uiName);

				IUserInterface ui = uiObject as IUserInterface;

				if (uiObject == null)
					throw new MissingMethodException();

				// start a new game
				Game = new Game(settings);

				ui.LoadSprites += Game.LoadSprites;
				ui.Render += Game.Render;
				ui.Idle += Game.Idle;

				PlayerControls[] playerControls = new PlayerControls[settings.Get<int>(Settings.Types.PlayerCount)];

				IInputMethod im = uiObject as IInputMethod;

				Dictionary<string, Control> imControls = im.GetControls();

				for (int i = 0; i < settings.Get<int>(Settings.Types.PlayerCount); ++i)
				{
					List<KeyValuePair<PlayerControls.Types, Control>> controls = new List<KeyValuePair<PlayerControls.Types, Control>>();

					string key = settings.Get<string>("Player" + i + ".Up");
					if (key != null && imControls.ContainsKey(key))
						controls.Add(new KeyValuePair<PlayerControls.Types, Control>(PlayerControls.Types.Up, imControls[key]));

					key = settings.Get<string>("Player" + i + ".Down");
					if (key != null && imControls.ContainsKey(key))
						controls.Add(new KeyValuePair<PlayerControls.Types, Control>(PlayerControls.Types.Down, imControls[key]));

					key = settings.Get<string>("Player" + i + ".Left");
					if (key != null && imControls.ContainsKey(key))
						controls.Add(new KeyValuePair<PlayerControls.Types, Control>(PlayerControls.Types.Left, imControls[key]));

					key = settings.Get<string>("Player" + i + ".Right");
					if (key != null && imControls.ContainsKey(key))
						controls.Add(new KeyValuePair<PlayerControls.Types, Control>(PlayerControls.Types.Right, imControls[key]));

					key = settings.Get<string>("Player" + i + ".Action1");
					if (key != null && imControls.ContainsKey(key))
						controls.Add(new KeyValuePair<PlayerControls.Types, Control>(PlayerControls.Types.Action1, imControls[key]));

					key = settings.Get<string>("Player" + i + ".Action2");
					if (key != null && imControls.ContainsKey(key))
						controls.Add(new KeyValuePair<PlayerControls.Types, Control>(PlayerControls.Types.Action2, imControls[key]));

					playerControls[i] = new PlayerControls(controls);

					foreach (KeyValuePair<PlayerControls.Types, Control> control in controls)
						im.RegisterControl(control.Value);
				}


				im.RegisterControl(imControls["Escape"]);
				imControls["Escape"].Pressed += (sender, e) => ui.Terminate();
				
				

				Game.Participants.Add(new LocalParticipant(playerControls));
				nParticipantsNegotiating = 1;
				playersRequested = new int[nParticipantsNegotiating];

				foreach (Participant p in Game.Participants)
				{
					p.Negotiate += Participant_Negotiate;
					p.StartNegotiation();
				}

				while (nParticipantsNegotiating != 0);

				Game.StartRound(playerCount);

				int iFirstPlayer = 0;
				
				for (int i = 0; i < Game.Participants.Count; ++i)
				{
					Player[] players = new Player[playersRequested[i]];
					Array.Copy(Game.Players, iFirstPlayer, players, 0, playersRequested[i]);
					iFirstPlayer += playersRequested[i];
					System.Console.WriteLine(Game.Participants[i] + " starting round controlling " + players.Length + " players");
					Game.Participants[i].StartRound(players);
				}


				// start up the game UI
				ui.Initialize(settings);
				ui.MainLoop();
				ui.Terminate();

			}
			catch (FileNotFoundException/*^!^*/ e)
			{
				ErrorHandling.FatalError(e, @"The interface module ""{0}"" or one of its"
											+ @" dependencies could not be found:",
												uiName);
			}
			/* This is not in CF. Does it throw an alternative error? TRYTRY
			catch (FileLoadException e)
			{
				ErrorHandling.FatalError(e, @"The interface module ""{0}"" or one of its"
											+ @" dependencies could not be loaded:\n{1}",
												uiName);
			}*/
			catch (BadImageFormatException/*^!^*/ e)
			{
				ErrorHandling.FatalError(e, @"The interface module ""{0}"" or one of its"
											+ @" dependencies could has an invalid format:",
												uiName);
			}
			catch (MissingMethodException/*^!^*/ e)
			{
				ErrorHandling.FatalError(e, @"The module ""{0}"" is not a valid BomberStuff"
											+ @" interface module:",
												uiName);
			}
			catch (Exception/*^!^*/ e)
			{
				ErrorHandling.UnexpectedError(e, @"while trying to load the interface module {0}:",
														uiName);
			}
		}

		/// <summary>
		/// Creates a ColorRemapInfo structure defining the color of the
		/// specified player
		/// </summary>
		/// <param name="player">player number to retrieve color for</param>
		/// <returns>a ColorRemapInfo specifying the player's color</returns>
		private static ColorRemapInfo PlayerColor(int player)
		{
			// HSL scales are in [0, 360[. Some nice values are:
			// 0 = red
			// 45 = orange
			// 75 = yellow
			// 135 = green (original)
			// 195 = turquoise
			// 225 = light blue
			// 240 = dark blue
			// 285 = purple
			// 315 = pink
			// 0, -120 = black
			// 0, +150 = (a little too) white
			switch (player)
			{
				case 0:
					return new ColorRemapInfo(135); // the green bomber (unmodified)
				case 1:
					return new ColorRemapInfo(225); // light blue
				case 2:
					return new ColorRemapInfo(75, 240, +20); // some kind of yellow
				case 3:
					return new ColorRemapInfo(0); // red
				case 4:
					return new ColorRemapInfo(45); // orange
				case 5:
					return new ColorRemapInfo(315); // pink
				case 6:
					return new ColorRemapInfo(195); // turquoise
				case 7:
					return new ColorRemapInfo(0, +150); // white
				case 8:
					return new ColorRemapInfo(0, -120); // black
				default:
					return new ColorRemapInfo(285); // all others are purple for now
			}
		}
	}
}
