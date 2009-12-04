//
// Explosion.cs - Explosion class
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
	public class Explosion : MobileObject
	{
		/// <summary>
		/// 
		/// </summary>
		public const long ExplosionTicks = 20;
		/// <summary>
		/// 
		/// </summary>
		public long TicksLeft = ExplosionTicks;

		/// <summary>
		/// 
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <param name="player"></param>
		/// <param name="direction"></param>
		public Explosion(int x, int y, int player, Directions direction)
			: this(x, y, player, direction, false) { }

		/// <summary>
		/// 
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <param name="player"></param>
		/// <param name="direction"></param>
		/// <param name="isTip"></param>
		public Explosion(int x, int y, int player, Directions direction, bool isTip)
			: base(x, y, 1.0f, 1.0f, player)
		{
			if (isTip)
				Animation = new PlayerDirectionAnimationIndex(PlayerDirectionAnimationIndex.Types.ExplosionTip, direction, 0);
			else
				Animation = new PlayerDirectionAnimationIndex(PlayerDirectionAnimationIndex.Types.ExplosionMid, direction, 0);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <param name="player"></param>
		public Explosion(int x, int y, int player)
			: base(x, y, 0.0f, 0.0f, player)
		{
			Animation = new PlayerAnimationIndex(PlayerAnimationIndex.Types.ExplosionCenter, 0);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="board"></param>
		/// <param name="ticks"></param>
		public override void Tick(Board board, int ticks)
		{
			base.Tick(board, ticks);

			if ((TicksLeft -= ticks) <= 0)
				board.Items.Remove(this);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="aniList"></param>
		/// <param name="device"></param>
		/// <returns></returns>
		public override BomberStuff.Core.UserInterface.ISprite GetSprite(AnimationList aniList, BomberStuff.Core.UserInterface.IDevice device)
		{
			return aniList[Animation].GetSprite(device, AnimationState, PlayerIndex);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public override SizeF GetOffset(AnimationList aniList)
		{
			SizeF offset = aniList[Animation].GetOffset(AnimationState);
			// HACKHACK: this belongs to the animation, not the object
			offset.Width -= 2.0f / 40.0f;
			offset.Height -= 1.0f / 36.0f;

			return offset;
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
