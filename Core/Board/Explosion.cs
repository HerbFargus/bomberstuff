//
// Explosion.cs - Explosion class
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
		public const long ExplosionTicks = 9;
		/// <summary>
		/// 
		/// </summary>
		public long TicksLeft = ExplosionTicks;

		float xOffset, yOffset;

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
			: base(GetX(x, direction, isTip), GetY(y, direction, isTip),
					GetWidth(direction, isTip), GetHeight(direction, isTip),
					player)
		{
			xOffset = X - x;
			yOffset = Y - y;
			if (isTip)
				Animation = new PlayerDirectionAnimationIndex(PlayerDirectionAnimationIndex.Types.ExplosionTip, direction);
			else
				Animation = new PlayerDirectionAnimationIndex(PlayerDirectionAnimationIndex.Types.ExplosionMid, direction);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <param name="player"></param>
		public Explosion(int x, int y, int player)
			: base(x, y, 1.0f, 1.0f, player)
		{
			Animation = new PlayerAnimationIndex(PlayerAnimationIndex.Types.ExplosionCenter);
		}

		//
		// ·  ···  ···  ···  ···  ···  ·
		// ·  ···  ···  ·^·  ···  ···  ·
		// ·  ···  ···  ·|·  ···  ···  ·
		//
		// ·  ···  ···  ·|·  ···  ···  ·
		// ·  ···  ···  ·|·  ···  ···  ·
		// ·  ···  ···  ·|·  ···  ···  ·
		//
		// ·  ···  ···  ·|·  ···  ···  ·
		// ·  ·<-  ---  -+-  ---  ->·  ·
		// ·  ···  ···  ·|·  ···  ···  ·
		//
		// ·  ···  ···  ·|·  ···  ···  ·
		// ·  ···  ···  ·|·  ···  ···  ·
		// ·  ···  ···  ·|·  ···  ···  ·
		//
		// ·  ···  ···  ·|·  ···  ···  ·
		// ·  ···  ···  ·v·  ···  ···  ·
		// ·  ···  ···  ···  ···  ···  ·
		//

		private static float GetX(int x, Directions direction, bool isTip)
		{
			if (direction == Directions.Down || direction == Directions.Up)
				return x + 0.5f;
			if (isTip && direction == Directions.Left)
				return x + 0.5f;

			return x;
		}

		private static float GetY(int y, Directions direction, bool isTip)
		{
			if (direction == Directions.Left || direction == Directions.Right)
				return y + 0.5f;
			if (isTip && direction == Directions.Up)
				return y + 0.5f;

			return y;
		}

		private static float GetWidth(Directions direction, bool isTip)
		{
			if (direction == Directions.Down || direction == Directions.Up)
				return 0.0f;

			else if (isTip)
				return 0.5f;

			return 1.0f;
		}

		private static float GetHeight(Directions direction, bool isTip)
		{
			if (direction == Directions.Left || direction == Directions.Right)
				return 0.0f;

			else if (isTip)
				return 0.5f;

			return 1.0f;
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
			offset.Width -= 2.0f / 40.0f - xOffset;
			offset.Height -= 1.0f / 36.0f - yOffset;

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
