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
			string/*^!^*/ uiName = "SlimDXInterface";

			try
			{
				Assembly assembly = Assembly.LoadFrom(uiName + ".dll");
				object uiObject = assembly.CreateInstance("BomberStuff." + uiName + "." + uiName);

				IUserInterface ui = uiObject as IUserInterface;

				if (uiObject == null)
					throw new MissingMethodException();

				// TODO/TRYTRY: start with some kind of menu?

				// load settings
				Settings settings = new Settings();
				settings.Set<string>(Settings.Types.ABDirectory, @"H:\Lappy\Temp\atomic_bomberman\bomber");

				// start a new game
				Game = new Game(settings);

				ui.LoadSprites += Game.LoadSprites;
				ui.Render += Game.Render;

				PlayerControls[] playerControls = new PlayerControls[2];

				List<KeyValuePair<PlayerControls.Types, Control>> controls = new List<KeyValuePair<PlayerControls.Types, Control>>();

				IInputMethod im = uiObject as IInputMethod;

				Dictionary<string, Control> imControls = im.GetControls();

				controls.Add(new KeyValuePair<PlayerControls.Types, Control>(PlayerControls.Types.Up, imControls["W"]));
				controls.Add(new KeyValuePair<PlayerControls.Types, Control>(PlayerControls.Types.Down, imControls["S"]));
				controls.Add(new KeyValuePair<PlayerControls.Types, Control>(PlayerControls.Types.Left, imControls["A"]));
				controls.Add(new KeyValuePair<PlayerControls.Types, Control>(PlayerControls.Types.Right, imControls["D"]));
				//System.Windows.Forms.Keys.Up;

				foreach (KeyValuePair<PlayerControls.Types, Control> control in controls)
					im.RegisterControl(control.Value);
				
				playerControls[0] = new PlayerControls(controls);

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
				ui.Initialize();
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
	}
}
