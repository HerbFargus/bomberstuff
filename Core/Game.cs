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

using BomberStuff.Core.UserInterface;
using BomberStuff.Core.Drawing;

using Bomber.Files;

namespace BomberStuff.Core
{
	/// <summary>
	/// 
	/// </summary>
	public class Game
	{
		/// <summary>
		/// List of the participants in the game, including the
		/// local player(s)
		/// </summary>
		public readonly List<Participant> Participants;

		// TODO: Players
		/// <summary>The board on which the game is played</summary>
		public readonly Board Board;
		// TODO: ?

		/// <summary>
		/// 
		/// </summary>
		public Game()
		{
			Board = new Board();
		}

		private ISprite background;
		internal static ISprite dings;

		/// <summary>
		/// 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		public void LoadSprites(object sender, LoadSpritesEventArgs e)
		{
			background = e.Device.LoadSprite(PCXReader.StreamFromFile(@"C:\Share\ABomber\DATA\RES\FIELD0.PCX"), false, System.Drawing.Color.Transparent);
			dings = e.Device.LoadSprite(PCXReader.StreamFromFile(@"C:\Share\ABomber\DATA\RES\POWBOMB.PCX"), false, System.Drawing.Color.Black);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		public void Render(object sender, RenderEventArgs e)
		{
			e.UserInterface.Draw(background, new PointF(), new SizeF(1.0f, 1.0f), System.Drawing.Color.White);
			e.UserInterface.Draw("Bomber Stuff v0.1", new BomberStuff.Core.Drawing.PointF(0.5f, 0.05f), System.Drawing.Color.YellowGreen);
			Stone s = new Stone(0, 0);
			DrawMobileObject(e.UserInterface, s, System.Drawing.Color.White);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="ui"></param>
		/// <param name="obj"></param>
		/// <param name="color"></param>
		private void DrawMobileObject(IUserInterface ui, MobileObject obj, System.Drawing.Color color)
		{
			ISprite sprite = obj.GetSprite();
			float boardWidth = Board.BottomRight.X - Board.TopLeft.X;
			float boardHeight = Board.BottomRight.Y - Board.TopLeft.Y;

			float fieldWidth = boardWidth / Board.Width;
			float fieldHeight = boardHeight / Board.Height;

			float x = Board.TopLeft.X + fieldWidth * obj.X;
			float y = Board.TopLeft.Y + fieldHeight * obj.Y;

			float w = obj.Width * fieldWidth;
			float h = obj.Height * fieldHeight;

			ui.Draw(sprite, new PointF(x, y), new SizeF(w, h), color);
		}
	}
}
