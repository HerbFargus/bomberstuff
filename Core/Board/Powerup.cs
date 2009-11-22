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

namespace BomberStuff.Core
{
	/// <summary>
	/// A type of <see cref="Powerup" />.
	/// </summary>
	public enum PowerupTypes
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
		/// <summary>A random powerup</summary>
		Random,
		/// <summary></summary>
		Last = Random
	}

	/// <summary>
	/// 
	/// </summary>
	public abstract class Powerup : MobileObject
	{
		/// <summary>
		/// 
		/// </summary>
		public Powerup()
			: base(0, 0, 0, 0)
		{
			throw new System.NotImplementedException();
		}
	}
}
