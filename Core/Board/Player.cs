//
// Player.cs - Player class
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

using BomberStuff.Core.UserInterface;
using BomberStuff.Core.Drawing;
using BomberStuff.Core.Game;

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
		public int Range;
		/// <summary>
		/// 
		/// </summary>
		public int MaxBombs;

		/// <summary>
		/// 
		/// </summary>
		public int CurrentBombs;

		/// <summary>
		/// 
		/// </summary>
		public float Speed;

		private int DeathAnimationIndex;
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
			DeathAnimationIndex = Game.Game.GetRandom(9);
			Range = 2;
			MaxBombs = 1;
			CurrentBombs = 0;
			Speed = 0.125f;
			Animation = new PlayerDirectionAnimationIndex(PlayerDirectionAnimationIndex.Types.Stand, Directions.Down, 0);
		}

		/// <summary>
		/// Die
		/// </summary>
		public void Die()
		{
			if (!Alive)
				return;

			Alive = false;
			
			System.Console.WriteLine("That's it, you're dead " + ToString());
			
			// set death animation. This is non-looping, so the player object
			// will be removed when it's finished
			Animation = new PlayerDeathAnimationIndex(DeathAnimationIndex, 0);
			Loop = false;

			SpeedX = 0f;
			SpeedY = 0f;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <remarks>TODO: sensible in Player class?</remarks>
		public void PlaceBomb(Board board)
		{
			// dead players shouldn't place bombs. TRYTRY: is there a more senible place
			// to check this? See SetMoveState
			if (!Alive)
				return;

			int x = (int)System.Math.Round(X);
			int y = (int)System.Math.Round(Y);

			foreach (MobileObject bomb in board.Items)
				if (bomb is Bomb)
					if (new RectangleF(bomb.Position, bomb.Size).IntersectsWith(new RectangleF(x, y, 1.0f, 1.0f)))
					{
						System.Console.WriteLine("Already a bomb at {0}, {1}!", x, y);
						return;
					}

			board.Items.Add(new Bomb(x, y, PlayerIndex, Range));
			++CurrentBombs;
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
		/// <param name="primary"></param>
		/// <param name="secondary"></param>
		/// <param name="moving"></param>
		public override void SetMoveState(Directions primary, Directions secondary, bool moving)
		{
			if (!Alive)
				return;

			base.SetMoveState(primary, secondary, moving);

			if (!moving)
			{
				Animation = new PlayerDirectionAnimationIndex(PlayerDirectionAnimationIndex.Types.Stand, primary, 0);
			}
			else
				Animation = new PlayerDirectionAnimationIndex(PlayerDirectionAnimationIndex.Types.Walk, primary, 0);

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
				// -> this is respected here. The explosion itself is only in the
				//    center of the field, so a collision is only raised if
				//    we actually walk through the center!

				// see also Bomb.Explode
				Die();

				return false;
			}
			
			else if (other is Powerup)
			{
				Powerup pup = (Powerup)other;
				pup.Affect(this);
				
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
