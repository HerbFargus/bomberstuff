//
// Game.cs - Game class
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

using System.Collections.Generic;

using BomberStuff.Core.Animation;
using BomberStuff.Core.UserInterface;
using BomberStuff.Core.Drawing;

using BomberStuff.Files;

namespace BomberStuff.Core
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
		
		/// <summary>
		/// 
		/// </summary>
		public Game()
		{
			Participants = new List<Participant>();
		}

		/// <summary>
		/// Start a new game round
		/// </summary>
		/// <param name="playerCount"></param>
		public void StartRound(int playerCount)
		{
			Players = new Player[playerCount];
			Board = new Board();
			
			System.Drawing.Point[] startPositions = new System.Drawing.Point[playerCount];
			Board.Items.AddRange(BomberStuff.Files.SchemeReader.GetScheme(ABPath + @"\DATA\SCHEMES\BASIC.SCH", ref startPositions, Board.Width, Board.Height));

			for (int i = 0; i < playerCount; ++i)
			{
				Players[i] = new Player(startPositions[i].X, startPositions[i].Y, i);
				Board.Items.Add(Players[i]);
				Board.AddPlayer(Players[i]);
			}

			foreach (Participant p in Participants)
				p.ControlPlayer += Participant_ControlPlayer;
		}


		private void Participant_ControlPlayer(object sender, ControlPlayerEventArgs e)
		{
			Participant p = (Participant)sender;

			System.Console.WriteLine(p + " is controlling " + Players[e.PlayerIndex] + " " + e.PlayerIndex + " to " + (e.Moving ? "move " : "stand facing ") + e.Direction);

			//if (p.HasAuthority || ...)
				Players[e.PlayerIndex].SetMoveState(e.Direction, e.Moving ? 0.05f : 0);

				System.Console.WriteLine("Controlled " + Players[e.PlayerIndex] + " is board item " + Board.Items.IndexOf(Players[e.PlayerIndex]));
		}

		private ISprite background;
		internal const string ABPath = @"H:\Lappy\Temp\atomic_bomberman\bomber";

		/// <summary>
		/// 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		public void LoadSprites(object sender, LoadSpritesEventArgs e)
		{
			if (Animations != null)
			{
				Animations.Dispose();
				Animations = null;
				System.GC.Collect();
			}

			Animations = new AnimationList(1 /*PlayerCount*/);
			AniFileReader.AddAliFile(Animations, ABPath + @"\DATA\ANI\master.ali");
			Animations.Check();

			background = e.Device.LoadSprite(PCXReader.StreamFromFile(ABPath + @"\DATA\RES\FIELD0.PCX"), 640, 480, false, System.Drawing.Color.Transparent);
		}

		long lastSec;
		long lastTick = 0;
		int framesThisSec;
		string fps = "? FPS";

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

			++framesThisSec;
			long ticks = System.DateTime.Now.Ticks;
			if (ticks - lastSec > 10000000)
			{
				lastSec = ticks;
				fps = framesThisSec + " FPS";
				framesThisSec = 0;
			}

			//
			// Draw text
			//
			e.UserInterface.Draw(fps, new BomberStuff.Core.Drawing.PointF(0.1f, 0.05f), System.Drawing.Color.White);
			e.UserInterface.Draw("Bomber Stuff v0.1", new BomberStuff.Core.Drawing.PointF(0.5f, 0.05f), System.Drawing.Color.YellowGreen);

			// HACKHACK/TODO: Seperate animation!!!
			int diffTicks = 0;
			
			if (ticks - lastTick > 10000 * 50)
			{
				diffTicks = (int)((ticks - lastTick) / 10000 / 50);

				if (lastTick == 0)
				{
					diffTicks = 0;
					lastTick = ticks;
				}
				else
					lastTick += 10000 * 50 * diffTicks;
			}

			//
			// Draw items on the board
			//
			foreach (MobileObject obj in Board.Items)
			{
				// players have a shadow. This is a special case
				if (obj is Player)
					DrawMobileObject(e.UserInterface, e.Device, ((Player)obj).GetShadow(), System.Drawing.Color.White);
				DrawMobileObject(e.UserInterface, e.Device, obj, System.Drawing.Color.White);
				obj.Animate(Animations, diffTicks);
				obj.Move(Board, diffTicks);
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

			// the coordinates we want in the end are normalized
			// [0, 0] is the top left, [1, 1] is the bottom right
			// of the window/screen

			// calculate the width and height of the actual board
			// (minus the borders) in normalized coords
			float boardWidth = Board.BottomRight.X - Board.TopLeft.X;
			float boardHeight = Board.BottomRight.Y - Board.TopLeft.Y;

			// width and height of one field in normalized coords
			float fieldWidth = boardWidth / Board.Width;
			float fieldHeight = boardHeight / Board.Height;

			// the object's coordinates are measured in fields
			// -> multiply by field width/height
			// then add the top left corner of the board
			float x = Board.TopLeft.X + fieldWidth * obj.X;
			float y = Board.TopLeft.Y + fieldHeight * obj.Y;

			// here we need not the actual size of the object, but
			// the size of its sprite
			SizeF size = obj.GetSpriteSize(Animations);
			float w = size.Width * fieldWidth;
			float h = size.Height * fieldHeight;

			// now we need to offset the object so that it is
			// correctly centered
			SizeF offset = obj.GetOffset(Animations);
			x -= offset.Width * fieldWidth;
			y -= offset.Height * fieldHeight;
			
			ui.Draw(sprite, new PointF(x, y), new SizeF(w, h), color);
		}
	}
}
