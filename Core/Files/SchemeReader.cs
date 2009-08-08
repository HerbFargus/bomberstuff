//
// Bobmer Stuff: Atomic Bomberman Remake
//
// SchemeReader.cs - utility class to read Atomic Bomberman .sch files
//
// Copyright © 2008-2009 Thomas Faber
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
// along with Bomber Stuff.  If not, see <http://www.gnu.org/licenses/>.
//

using System;
using System.IO;
using System.Text;

namespace Bomber.Files
{
	/// <summary>
	/// 
	/// </summary>
	public class SchemeReader
	{
		// TODO does this really need an extra class/file?
		/// <summary>
		/// Reads an Atomic Bomberman scheme file and returns a board built
		/// accordingly
		/// </summary>
		/// <param name="filename">path to the AB scheme file</param>
		/// <param name="startPositions"></param>
		/// <returns>
		/// An array of FieldContents values representing a new board filled
		/// according to the specified scheme
		/// </returns>
		public static FieldContents[, ] GetScheme(string filename, ref Point<BoardX, BoardY>[] startPositions)
		{
			FieldContents[, ] field = new FieldContents[Board.Width, Board.Height];
			string[] lines = File.ReadAllLines(filename, Encoding.ASCII);
			
			int i, x, y;
			int density = 100;
			
			foreach (string origLine in lines)
			{
				string line = origLine;
				int commentStart = line.IndexOf(';');
				
				if (commentStart != -1)
					line = line.Remove(commentStart);
				
				line = line.Trim().ToUpper();
				
				string[] items = line.Split(new char[] { ',' });
				
				if (items[0] == "-B"
							&& items.Length == 2
							&& int.TryParse(items[1], out i))
				{
					// i is the brick density here, must be in ]0, 100]
					if (i <= 0 || i > 100)
						continue;
					
					density = i;
				}
				else if (items[0] == "-S"
							&& items.Length >= 4
							&& int.TryParse(items[1], out i)
							&& startPositions != null
							&& i < startPositions.Length
							&& int.TryParse(items[2], out x)
							&& int.TryParse(items[3], out y))
				{
					x %= Board.Width;
					y %= Board.Height;
					
					if (x < 0)
						x += Board.Width;
					
					if (y < 0)
						y += Board.Height;
					
					startPositions[i] = new Point<BoardX, BoardY>(new BoardX(x), new BoardY(y));
				}
				else if (items[0] == "-R"
							&& items.Length == 3
							&& (items[2] = items[2].Trim()).Length == Board.Width
							&& int.TryParse(items[1], out y))
				{
					if (y < 0 || y >= Board.Height)
						continue;
					
					for (x = 0; x < Board.Width; ++x)
					{
						switch (items[2][x])
						{
							case ':':
								// random is in [0, 99], this is always < 100
								// and only < 1 in 1/100 of cases
								if (BomberStuff.Random.Next(100) < density)
									field[x, y] = FieldContents.Wall;
								else
									field[x, y] = FieldContents.Empty;
								break;
							case '.':
								field[x, y] = FieldContents.Empty;
								break;
							case '#':
								field[x, y] = FieldContents.Stone;
								break;
						}
					}
				}
			}
			
			return field;
		}
	}
}
