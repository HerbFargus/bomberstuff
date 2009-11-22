//
// Directions.cs - Directions and direction utilities
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

using System;

namespace BomberStuff.Core
{
	/// <summary>
	/// Directions in which objects can move
	/// </summary>
	public enum Directions
	{
		/// <summary>Right/East</summary>
		Right,
		/// <summary>Down/South</summary>
		Down,
		/// <summary>Left/West</summary>
		Left,
		/// <summary>Up/North</summary>
		Up
	}

	/// <summary>
	/// 
	/// </summary>
	public static class DirectionUtilities
	{
		/// <summary>
		/// Get the opposite of the direction
		/// </summary>
		/// <param name="d"></param>
		/// <returns></returns>
		public static Directions Opposite(/*this*/ Directions d)
		{
			switch (d)
			{
				case Directions.Right:
					return Directions.Left;
				case Directions.Down:
					return Directions.Up;
				case Directions.Left:
					return Directions.Right;
				case Directions.Up:
					return Directions.Down;
			}

			throw new ArgumentException("Invalid Direction");
		}

		/// <summary>
		/// Get the direction on the left of this one
		/// </summary>
		/// <param name="d"></param>
		/// <returns></returns>
		public static Directions LeftOf(/*this*/ Directions d)
		{
			switch (d)
			{
				case Directions.Right:
					return Directions.Up;
				case Directions.Down:
					return Directions.Right;
				case Directions.Left:
					return Directions.Down;
				case Directions.Up:
					return Directions.Left;
			}

			throw new ArgumentException("Invalid Direction");
		}

		/// <summary>
		/// Get the direction on the right of this one
		/// </summary>
		/// <param name="d"></param>
		/// <returns></returns>
		public static Directions RightOf(/*this*/ Directions d)
		{
			return /*d.LeftOf().Opposite()*/LeftOf(Opposite(d));
		}

		/// <summary>
		/// Get the X component of the direction
		/// </summary>
		/// <param name="d"></param>
		/// <returns></returns>
		public static int GetX(/*this*/ Directions d)
		{
			switch (d)
			{
				case Directions.Right:
					return 1;
				case Directions.Down:
					return 0;
				case Directions.Left:
					return -1;
				case Directions.Up:
					return 0;
			}

			throw new ArgumentException("Invalid Direction");
		}

		/// <summary>
		/// Get the X component of the direction
		/// </summary>
		/// <param name="d"></param>
		/// <returns></returns>
		public static int GetY(/*this*/ Directions d)
		{
			switch (d)
			{
				case Directions.Right:
					return 0;
				case Directions.Down:
					return 1;
				case Directions.Left:
					return 0;
				case Directions.Up:
					return -1;
			}

			throw new ArgumentException("Invalid Direction");
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="d"></param>
		/// <returns></returns>
		public static Directions FromString(string d)
		{
			switch (d)
			{
				case "north":
					return Directions.Up;
				case "east":
					return Directions.Right;
				case "south":
					return Directions.Down;
				case "west":
					return Directions.Left;
			}

			throw new FormatException();
		}
	}

}