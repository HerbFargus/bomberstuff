//
// Powerup.cs - Powerup class
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
using BomberStuff.Core.Game;

namespace BomberStuff.Core
{
	/// <summary>
	/// A powerup that can be picked up by a player
	/// </summary>
	public class Powerup : MobileObject
	{
		/// <summary>
		/// A type of <see cref="Powerup" />.
		/// </summary>
		public enum Types
		{
			/// <summary>A bomb powerup</summary>
			Bomb,
			/// <summary>A range powerup</summary>
			Range,
			/// <summary>A virus</summary>
			Virus,
			/// <summary>A kick powerup</summary>
			Kick,
			/// <summary>A speed powerup</summary>
			Speed,
			/// <summary>A punch glove powerup</summary>
			Punch,
			/// <summary>A grab glove powerup</summary>
			Grab,
			/// <summary>A spooge powerup</summary>
			Spooge,
			/// <summary>A gold flame (infinite range) powerup</summary>
			Goldflame,
			/// <summary>A trigger bombs powerup</summary>
			Trigger,
			/// <summary>A jelly bombs powerup</summary>
			Jelly,
			/// <summary>Multiple viruses</summary>
			BadVirus,
			// NOTE: this needs to be last. See Affect()
			/// <summary>A random powerup</summary>
			Random,
			/// <summary></summary>
			Last = Random
		}

		/// <summary>
		/// 
		/// </summary>
		public readonly Types Type;

		/// <summary>
		/// 
		/// </summary>
		public Powerup(int x, int y, Types type)
			: base(x, y, 1.0f, 1.0f)
		{
			Type = type;
			Animation = new PowerupAnimationIndex(Type);
		}

		private bool Used = false;

		/// <summary>
		/// 
		/// </summary>
		/// <param name="board"></param>
		/// <param name="ticks"></param>
		public override void Tick(Board board, int ticks)
		{
			base.Tick(board, ticks);

			// HACKHACK: using Tick to remove used powerup? That's weird
			if (Used)
				board.Items.Remove(this);
		}

		/// <summary>
		/// Affect a player
		/// </summary>
		/// <param name="player"></param>
		public void Affect(Player player)
		{
			if (Used)
				return;
			Types type = Type;

			if (type == Types.Random)
				type = (Types)Game.Game.GetRandom((int)Types.Random);

			switch (type)
			{
				case Types.Bomb:
					++player.MaxBombs;
					break;
				case Types.Range:
					++player.Range;
					break;
				case Types.Virus:
				case Types.Kick:
				case Types.Speed:
				case Types.Punch:
				case Types.Grab:
				case Types.Spooge:
				case Types.Goldflame:
				case Types.Trigger:
				case Types.Jelly:
				case Types.BadVirus:
					break;
			}
			Used = true;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="aniList"></param>
		/// <returns></returns>
		public override BomberStuff.Core.Drawing.SizeF GetOffset(AnimationList aniList)
		{
			return new BomberStuff.Core.Drawing.SizeF();
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
