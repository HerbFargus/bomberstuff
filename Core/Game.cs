//
// Game.cs - Game class
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

using System.Collections.Generic;

using BomberStuff.Core.Animation;
using BomberStuff.Core.UserInterface;
using BomberStuff.Core.Drawing;

using BomberStuff.Files;

namespace BomberStuff.Core.Game
{
	/// <summary>
	/// Represents a game, that is, a series of rounds
	/// </summary>
	public class Game
	{
		/// <summary>
		/// List of the participants in the game, including the
		/// local player(s)
		/// </summary>
		public List<Participant> Participants;

		/// <summary>The players in the game</summary>
		public Player[] Players;

		/// <summary>The board on which the game is played</summary>
		private Board Board;

		/// <summary></summary>
		private AnimationList Animations;

		/// <summary></summary>
		private Settings Settings;

		// TRYTRY/HACKHACK: belongs to Board?
		/// <summary>The tileset number</summary>
		private int Tileset;
		
		/// <summary>
		/// 
		/// </summary>
		public Game(Settings settings)
		{
			Settings = settings;
			Participants = new List<Participant>();

			System.Version ver = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;
			LogoString = "Bomber Stuff " + ver.Major + "." + ver.Minor + "." + ver.Build + " (Build " + ver.Revision + ")";
		}

		private static System.Security.Cryptography.RandomNumberGenerator Random = new System.Security.Cryptography.RNGCryptoServiceProvider();

		/// <summary>
		/// Generates a random numbers between 0 (inclusive)
		/// and <paramref name="max" /> (exclusive)
		/// </summary>
		/// <param name="max">exclusive maximum</param>
		/// <returns>a number in [0, max[</returns>
		public static int GetRandom(byte max)
		{
			byte[] rand = new byte[1];

			// we get numbers in [0, 255]
			// we want numbers in [0, max[

			// ex: max = 9
			// maxFair = 251
			//
			// => then there are 28 (252 / 9) intervals:
			// [0, 8], [9, 17], ..., [243, 251]
			// all resulting in [0, 8] if taken modulo 9

			// any value above this is unfair and has to be generated again
			int maxFair = byte.MaxValue - byte.MaxValue % max - 1;

			do
			{
				Random.GetBytes(rand);
			} while (rand[0] > maxFair);

			return rand[0] % max;
		}

		// HACKHACK make this some nice member variables
		float boardWidth;
		float boardHeight;

		
		float fieldWidth;
		float fieldHeight;

		/// <summary>
		/// Start a new game round
		/// </summary>
		/// <param name="playerCount"></param>
		public void StartRound(int playerCount)
		{
			Players = new Player[playerCount];
			Board = new Board();
			Tileset = Settings.Get<int>(Settings.Types.Tileset);
			if (Tileset == -1)
				Tileset = Game.GetRandom(10);

			// calculate the width and height of the actual board
			// (minus the borders) in normalized coords
			boardWidth = Board.BottomRight.X - Board.TopLeft.X;
			boardHeight = Board.BottomRight.Y - Board.TopLeft.Y;

			// width and height of one field in normalized coords
			fieldWidth = boardWidth / Board.Width;
			fieldHeight = boardHeight / Board.Height;
			
			System.Drawing.Point[] startPositions = new System.Drawing.Point[playerCount];
			Board.Items.AddRange(BomberStuff.Files.SchemeReader.GetScheme(Settings.Get<string>(Settings.Types.ABDirectory) + @"\DATA\SCHEMES\" + Settings.Get<string>(Settings.Types.Scheme) + ".SCH", ref startPositions, Board.Width, Board.Height, Tileset));

			for (int i = 0; i < playerCount; ++i)
			{
				Players[i] = new Player(startPositions[i].X, startPositions[i].Y, i);
				Board.Items.Add(Players[i]);
				Board.AddPlayer(Players[i]);
			}

			Board.Items.Add(new Powerup(8, 6, Powerup.Types.Bomb));
			Board.Items.Add(new Powerup(9, 6, Powerup.Types.Range));

			foreach (Participant p in Participants)
			{
				p.MovePlayer += Participant_MovePlayer;
				p.PlayerAction += Participant_PlayerAction;
			}


		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void Participant_MovePlayer(object sender, MovePlayerEventArgs e)
		{
			Participant p = (Participant)sender;

			if (p.HasAuthority || p.ControlsPlayer(e.PlayerIndex))
				Players[e.PlayerIndex].SetMoveState(e.Direction, e.SecondaryDirection, e.Moving);
			else
				System.Diagnostics.Debug.Assert(false);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void Participant_PlayerAction(object sender, PlayerActionEventArgs e)
		{
			Participant p = (Participant)sender;

			// action 1: place bomb
			if (e.Type == PlayerActionEventArgs.Types.Action1)
			{
				if (p.HasAuthority || p.ControlsPlayer(e.PlayerIndex))
					Players[e.PlayerIndex].PlaceBomb(Board);
				else
					System.Diagnostics.Debug.Assert(false, "Unauthorized Participant trying to control Player" + e.PlayerIndex);
			}
		}

		private ISprite background;

		/// <summary>
		/// 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		public void LoadSprites(object sender, LoadSpritesEventArgs e)
		{
			if (Animations != null)
			{
				System.Console.WriteLine("Reloading animations:");
				Animations.Dispose();
				Animations = null;
				background.Dispose();
				background = null;
				System.GC.Collect();
			}

			Animations = new AnimationList(Settings.Get<BomberStuff.Core.Utilities.ColorRemapInfo[]>(Settings.Types.PlayerColor));
			System.Console.WriteLine("Loading animations...");
			//long ticks = System.DateTime.Now.Ticks;
			AniFileReader.AddAliFile(Animations, Settings.Get<string>(Settings.Types.ABDirectory) + @"\DATA\ANI\master.ali");
			System.Console.WriteLine("Done loading animations..."/* took {0} µs", (System.DateTime.Now.Ticks - ticks) / 10.0*/);
			Animations.Check();
			System.Console.WriteLine("Animation check succeeded. All animations accounted for.");

			System.Console.WriteLine("Caching animations...");
			Animations.Cache(e.Device, Players.Length);
			System.Console.WriteLine("Done caching animations.");

			background = e.Device.LoadSprite(PCXReader.FromFile(Settings.Get<string>(Settings.Types.ABDirectory) + @"\DATA\RES\FIELD" + Tileset + ".PCX"), false, System.Drawing.Color.Transparent);

			System.Console.WriteLine("Stand by...");
			System.GC.Collect();
		}

		private void Ani_Tick(int ticks)
		{
			for (int i = 0; i < Board.Items.Count; ++i)
			{
				MobileObject obj = Board.Items[i];
				if (!obj.Animate(Animations, ticks))
					Board.Items.RemoveAt(i--);
			}
		}

		private void Game_Tick(int ticks)
		{
			// we need to create a snapshot of the collection because
			// MobileObject.Tick may modify Board.Items
			MobileObject[] boardItems = Board.Items.ToArray();

			foreach (MobileObject obj in boardItems)
				obj.Tick(Board, ticks);
		}

		long lastSec;
		int framesThisSec;
		string fps = "? FPS";
		string LogoString;

		/// <summary>
		/// 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		public void Render(object sender, RenderEventArgs e)
		{
			//
			// Draw background
			//
			e.UserInterface.Draw(background, new PointF(), new SizeF(1.0f, 1.0f), System.Drawing.Color.White);

			//
			// Draw text
			//
			e.UserInterface.Draw(fps, new PointF(0.1f, 0.05f), System.Drawing.Color.White);
			e.UserInterface.Draw(LogoString, new PointF(0.5f, 0.05f), System.Drawing.Color.YellowGreen);

			//
			// Draw player shadows (only living players cast a shadow)
			//
			foreach (Player p in Players)
				if (p.Alive)
					DrawMobileObject(e.UserInterface, e.Device, p.GetShadow(), System.Drawing.Color.White);

			//
			// Draw items on the board
			//
			foreach (MobileObject obj in Board.Items)
				DrawMobileObject(e.UserInterface, e.Device, obj, System.Drawing.Color.White);

			++framesThisSec;
		}

		long lastAniTick = 0;
		long lastGameTick = 0;

		/// <summary>
		/// 
		/// </summary>
		public void Idle(object sender, System.EventArgs e)
		{
			const int TicksPerAniTick = 250000;
			const int TicksPerGameTick = 250000;

			long ticks = System.DateTime.Now.Ticks;
			if (ticks - lastSec > 10000000)
			{
				lastSec = ticks;
				fps = framesThisSec + " FPS";
				framesThisSec = 0;
			}

			if (ticks - lastAniTick > TicksPerAniTick)
			{
				if (lastAniTick == 0)
					lastAniTick = ticks;
				else
				{
					int diffTicks = (int)((ticks - lastAniTick) / TicksPerAniTick);

					lastAniTick += TicksPerAniTick * diffTicks;
					Ani_Tick(diffTicks);
				}
			}

			if (ticks - lastGameTick > TicksPerGameTick)
			{
				if (lastGameTick == 0)
					lastGameTick = ticks;
				else
				{
					int diffTicks = (int)((ticks - lastGameTick) / TicksPerGameTick);

					lastGameTick += TicksPerGameTick * diffTicks;
					Game_Tick(diffTicks);
				}
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="ui"></param>
		/// <param name="device"></param>
		/// <param name="obj"></param>
		/// <param name="color"></param>
		private void DrawMobileObject(IUserInterface ui, IDevice device, MobileObject obj, System.Drawing.Color color)
		{
			ISprite sprite = obj.GetSprite(Animations, device);

			//System.Diagnostics.Debug.Assert(sprite != null);

			// the coordinates we want in the end are normalized
			// [0, 0] is the top left, [1, 1] is the bottom right
			// of the window/screen

			// calculate the width and height of the actual board
			// (minus the borders) in normalized coords
			//float boardWidth = Board.BottomRight.X - Board.TopLeft.X;
			//float boardHeight = Board.BottomRight.Y - Board.TopLeft.Y;

			// width and height of one field in normalized coords
			//float fieldWidth = boardWidth / Board.Width;
			//float fieldHeight = boardHeight / Board.Height;

			// now we need to offset the object so that it is
			// correctly centered
			SizeF offset = obj.GetOffset(Animations);

			// the object's coordinates are measured in fields
			// -> multiply by field width/height
			// then add the top left corner of the board
			float x = Board.TopLeft.X + fieldWidth * (obj.X - offset.Width);
			float y = Board.TopLeft.Y + fieldHeight * (obj.Y - offset.Height);

			// here we need not the actual size of the object, but
			// the size of its sprite
			/*SizeF size = obj.GetSpriteSize(Animations);
			float w = size.Width * fieldWidth;
			float h = size.Height * fieldHeight;*/
			
			ui.Draw(sprite, new PointF(x, y), new SizeF(/*w, h*/), color);
		}
	}
}
