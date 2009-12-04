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
using BomberStuff.Core.Drawing;

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
		public bool Alive
		{
			get;
			protected set;
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <param name="player"></param>
		public Player(int x, int y, int player)
			: base(x, y, 1.0f, 1.0f, player)
		{
			Alive = true;
			Animation = new PlayerDirectionAnimationIndex(PlayerDirectionAnimationIndex.Types.Stand, Directions.Down, 0);
		}

		/// <summary>
		/// 
		/// </summary>
		public void Die()
		{
			if (!Alive)
				return;
			Alive = false;
			
			System.Console.WriteLine("That's it, you're dead " + ToString());
			//board.Items.Remove(this);
			Animation = new PlayerDeathAnimationIndex(Game.GetRandom(9), 0);
			Loop = false;
			m_SpeedX = m_SpeedY = 0.0f;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <remarks>TODO: sensible in Player class?</remarks>
		public void PlaceBomb(Board board)
		{
			board.Items.Add(new Bomb((int)System.Math.Round(X), (int)System.Math.Round(Y), PlayerIndex, 3));
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="aniList"></param>
		/// <param name="device"></param>
		/// <returns></returns>
		public override ISprite GetSprite(AnimationList aniList, IDevice device)
		{
			//if (PlayerIndex == -1)
			//return aniList[Animation].GetSprite(device, AnimationState);
			//else
			return aniList[Animation].GetSprite(device, AnimationState, PlayerIndex);
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
			if (!Alive)
				return;
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
			// Players can walk through each other
			if (other is Player)
				return false;


			else if (other is Bomb)
			{
				// TODO: kicking/punching
				return true;
			}

			else if (other is Explosion)
			{
				// Players shouldn't die on the tiniest touch of the explosion.
				// Leave them half a field of room in each direction

				// NOTE: an analogous check has to happen in Bomb.Explode
				if (new RectangleF(other.Position, other.Size).Contains(X + 0.5f, Y + 0.5f))
					Die();
				else
					System.Console.WriteLine("Explosion at ({2}, {3}) not killing player at ({0}, {1})", X, Y, other.X, other.Y);

				return false;
			}
			
			else if (other is Powerup)
			{
				// TODO: pick up powerup
				return false;
			}

			return true;
		}

		/// <summary>
		/// Handle a collision of the Player with a border.
		/// </summary>
		/// <remarks>
		/// Nothing happens when a Player runs against the border.
		/// </remarks>
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
