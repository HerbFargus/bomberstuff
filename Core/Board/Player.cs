//
// Player.cs - Player class
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

using BomberStuff.Core.UserInterface;

namespace BomberStuff.Core
{
	/// <summary>
	/// 
	/// </summary>
	public class Player : MobileObject
	{
		/// <summary>
		/// 
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <param name="player"></param>
		public Player(int x, int y, int player)
			: base(x, y, 1.0f, 1.0f, player)
		{
			Animation = new PlayerDirectionAnimationIndex(PlayerDirectionAnimationIndex.Types.Stand, Directions.Down, 0);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			return "Player" + PlayerIndex;
		}

		/// <summary>
		/// Returns the shadow for this player
		/// </summary>
		/// <returns></returns>
		public Shadow GetShadow()
		{
			return new Shadow(X, Y);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="direction"></param>
		/// <param name="speed"></param>
		public override void SetMoveState(Directions direction, float speed)
		{
			base.SetMoveState(direction, speed);

			if (speed == 0)
			{
				Animation = new PlayerDirectionAnimationIndex(PlayerDirectionAnimationIndex.Types.Stand, direction, 0);
			}
			else
				Animation = new PlayerDirectionAnimationIndex(PlayerDirectionAnimationIndex.Types.Walk, direction, 0);
			AnimationState = 0;
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

		/// <summary>
		/// Represents the shadow of a dude. Should never
		/// be added to Board.Items
		/// </summary>
		public class Shadow : MobileObject
		{
			/// <summary>
			/// 
			/// </summary>
			/// <param name="x"></param>
			/// <param name="y"></param>
			public Shadow(float x, float y)
				: base(x, y, 1.0f, 1.0f)
			{
				Animation = new SimpleAnimationIndex(SimpleAnimationIndex.Types.DudeShadow);
			}

			/// <summary>
			/// 
			/// </summary>
			/// <param name="aniList"></param>
			/// <returns></returns>
			public override BomberStuff.Core.Drawing.SizeF GetOffset(AnimationList aniList)
			{
				return new BomberStuff.Core.Drawing.SizeF(-6.0f / 40.0f, -19.0f / 36.0f);
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

}
