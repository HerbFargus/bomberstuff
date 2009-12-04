//
// Board.cs - Board class
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

using BomberStuff.Core.Drawing;

namespace BomberStuff.Core
{
	/// <summary>
	/// 
	/// </summary>
	public class Board
	{
		/// <summary>
		/// Dimensions of the board, measured in fields
		/// </summary>
		public readonly int Width, Height;

		/// <summary>
		/// 
		/// </summary>
		public readonly PointF TopLeft, BottomRight;
		
		/// <summary>
		/// List of items on the board
		/// </summary>
		/// <remarks>
		/// Items are: stones, walls, players, powerups, extras, bombs.
		/// Flames?
		/// </remarks>
		public readonly List<MobileObject> Items;

		/// <summary>
		/// Initialize a new board with the specified number of fields
		/// </summary>
		public Board()
		{
			Width = 15;
			Height = 11;
			TopLeft = new PointF(20.0f / 640.0f, 68.0f / 480.0f);
			BottomRight = new PointF(620.0f / 640.0f, 464.0f / 480.0f);
			Items = new List<MobileObject>();
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="player"></param>
		/// <remarks>
		/// TODO: is this sensible? Here?
		/// </remarks>
		public void AddPlayer(Player player)
		{
			float x = player.X;
			float y = player.Y;
			float w = player.Width;
			float h = player.Height;
			for (int i = 0; i < Items.Count; ++i)
			{
				MobileObject other = Items[i];
				if (other == player)
					continue;
				if (new RectangleF(x + 0.5f * w - 1.0f, y + 0.5f * h - 1.0f, 2.0f, 2.0f).IntersectsWith(new RectangleF(other.Position, other.Size))
						&& !(other is Stone) && !(other is Player))
				{
					System.Console.WriteLine("Removing a " + other + " from (" + other.X + ", " + other.Y + ")");
					Items.Remove(other);
					--i;
				}
			}
		}
	}
}
