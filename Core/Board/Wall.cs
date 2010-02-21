//
// Wall.cs - Stone class
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

using BomberStuff.Core.Animation;
using BomberStuff.Core.Drawing;

namespace BomberStuff.Core
{
	/// <summary>
	/// 
	/// </summary>
	public sealed class Wall : MobileObject
	{
		/// <summary>
		/// 
		/// </summary>
		private const long ExplosionTicks = 9;
		
		/// <summary>
		/// 
		/// </summary>
		private long TicksLeft = ExplosionTicks;

		/// <summary>
		/// 
		/// </summary>
		private bool Exploding = false;

		/// <summary>The tileset number</summary>
		private int Tileset;

		/// <summary>
		/// 
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <param name="tileset"></param>
		public Wall(int x, int y, int tileset)
			: base(x, y, 1.0f, 1.0f)
		{
			Tileset = tileset;
			Animation = new TilesetAnimationIndex(TilesetAnimationIndex.Types.Wall, tileset);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="board"></param>
		/// <param name="ticks"></param>
		public override void Tick(Board board, int ticks)
		{
			base.Tick(board, ticks);

			if (Exploding && (TicksLeft -= ticks) <= 0)
				board.Items.Remove(this);
		}

		/// <summary>
		/// Lets the wall explode
		/// </summary>
		public void Explode()
		{
			Animation = new TilesetAnimationIndex(TilesetAnimationIndex.Types.ExplodingWall, Tileset);
			Exploding = true;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="aniList"></param>
		/// <returns></returns>
		public override SizeF GetOffset(AnimationList aniList)
		{
			// Wall offset is completely weird. They shouldn't have any.
			return new SizeF();
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="other"></param>
		/// <returns></returns>
		protected override bool Collide(MobileObject other)
		{
			return true;
		}

		/// <summary>
		/// 
		/// </summary>
		protected override void BorderCollide()
		{
		}
	}

}
