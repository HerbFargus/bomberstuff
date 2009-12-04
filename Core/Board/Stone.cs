//
// Stone.cs - Stone class
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
	public class Stone : MobileObject
	{
		/// <summary>
		/// 
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <param name="tileset"></param>
		public Stone(int x, int y, int tileset)
			: base(x, y, 1.0f, 1.0f)
		{
			Animation = new TilesetAnimationIndex(TilesetAnimationIndex.Types.Stone, tileset);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="aniList"></param>
		/// <returns></returns>
		public override SizeF GetOffset(AnimationList aniList)
		{
			//SizeF offset = aniList[Animation].GetOffset(AnimationState);

			//offset.Width += 0.0f / 40.0f;
			//offset.Height -= 10.0f / 36.0f;

			//return offset;
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
