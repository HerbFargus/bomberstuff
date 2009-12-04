//
// Bomb.cs - Bomb class
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
using System.Collections.Generic;

namespace BomberStuff.Core
{
	/// <summary>
	/// 
	/// </summary>
	public class Bomb : MobileObject
	{
		/// <summary>
		/// 
		/// </summary>
		protected readonly int Range;

		/// <summary>
		/// 
		/// </summary>
		public const long BombTicks = 80;
		/// <summary>
		/// 
		/// </summary>
		public long TicksLeft = BombTicks;
		/// <summary>
		/// 
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <param name="player"></param>
		/// <param name="range"></param>
		public Bomb(int x, int y, int player, int range)
			: base(x, y, 1.0f, 1.0f, player)
		{
			Range = range;
			Animation = new PlayerAnimationIndex(PlayerAnimationIndex.Types.BombRegular, 0);
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
				Explode(board);
		}

		private bool IsExplosionStopper(Board board, int x, int y, List<Bomb> triggeredBombs)
		{
			bool result = false;
			RectangleF rect = new RectangleF(x, y, 1.0f, 1.0f);

			for (int i = 0; i < board.Items.Count; ++i)
			{
				MobileObject obj = board.Items[i];

				if (rect.IntersectsWith(new RectangleF(obj.Position, obj.Size)))
				{
					if (obj is Wall)
					{
						((Wall)obj).Explode();
						System.Console.WriteLine("Explosion hit wall at ({0}, {1})", x, y);
						result = true;
					}

					else if (obj is Stone)
					{
						System.Console.WriteLine("Explosion stopped by stone at ({0}, {1})", x, y);
						return true;
					}

					else if (obj is Player)
					{
						// TRYTRY/HACKHACK: this is necessary. And a relatively nice way
						//                  of doing it. But can we still improve that?

						// Players shouldn't die on the tiniest touch of the explosion.
						// Leave them half a field of room in each direction

						// NOTE: an analogous check has to happen in Player.Collide
						if (rect.Contains(obj.X + 0.5f, obj.Y + 0.5f))
							((Player)obj).Die();

					}
					else if (obj is Powerup)
						board.Items.RemoveAt(i--);

					else if (obj is Bomb && obj != this)
					{
						triggeredBombs.Add(((Bomb)obj));

						// A bomb stops an explosion.
						// Yes, that means bombs with short range stop explosions
						// with much larger range. That is desired behaviour!
						// (At least that's how it works in AB) TRYTRY
						return true;
					}

					else if (obj is Explosion)
						return true;
				}
			}

			return result;
		}

		private bool Exploded = false;

		/// <summary>
		/// Lets the bomb explode
		/// </summary>
		/// <param name="board"></param>
		public void Explode(Board board)
		{
			if (Exploded)
				return;
			Exploded = true;
			int x = (int)System.Math.Round(X),
				y = (int)System.Math.Round(Y);

			int xMin = x - Range;
			if (xMin < 0)
				xMin = 0;

			int yMin = y - Range;
			if (yMin < 0)
				yMin = 0;

			int xMax = x + Range;
			if (xMax >= board.Width)
				xMax = board.Width - 1;

			int yMax = y + Range;
			if (yMax >= board.Height)
				yMax = board.Height - 1;

			List<Bomb> triggeredBombs = new List<Bomb>();

			// left side explosions
			for (int currX = x - 1; currX >= xMin; --currX)
			{
				if (IsExplosionStopper(board, currX, y, triggeredBombs))
					break;
				board.Items.Add(new Explosion(currX, y, PlayerIndex, Directions.Left, currX == xMin));
			}

			// right side explosions
			for (int currX = x + 1; currX <= xMax; ++currX)
			{
				if (IsExplosionStopper(board, currX, y, triggeredBombs))
					break;
				board.Items.Add(new Explosion(currX, y, PlayerIndex, Directions.Right, currX == xMax));
			}

			// top explosions
			for (int currY = y - 1; currY >= yMin; --currY)
			{
				if (IsExplosionStopper(board, x, currY, triggeredBombs))
					break;
				board.Items.Add(new Explosion(x, currY, PlayerIndex, Directions.Up, currY == yMin));
			}

			// bottom explosions
			for (int currY = y + 1; currY <= yMax; ++currY)
			{
				if (IsExplosionStopper(board, x, currY, triggeredBombs))
					break;
				board.Items.Add(new Explosion(x, currY, PlayerIndex, Directions.Down, currY == yMax));
			}

			// center explosion
			if (!IsExplosionStopper(board, x, y, triggeredBombs))
				board.Items.Add(new Explosion(x, y, PlayerIndex));

			board.Items.Remove(this);
			
			foreach (Bomb bomb in triggeredBombs)
				bomb.Explode(board);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="aniList"></param>
		/// <param name="device"></param>
		/// <returns></returns>
		public override BomberStuff.Core.UserInterface.ISprite GetSprite(AnimationList aniList, BomberStuff.Core.UserInterface.IDevice device)
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
		public override SizeF GetOffset(AnimationList aniList)
		{
			SizeF offset = aniList[Animation].GetOffset(AnimationState);
			// HACKHACK: this belongs to the animation, not the object
			offset.Height -= 17.0f / 36.0f;

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
