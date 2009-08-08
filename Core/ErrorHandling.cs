//
// ErrorHandling.cs - ErrorHandling class
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
	/// Centralized methods for displaying and handling errors in the program
	/// </summary>
	public static class ErrorHandling
	{
		/// <summary>
		/// Handle an expected, but fatal error.
		/// Outputs a user-readable message an may or may
		/// not terminate the program.
		/// </summary>
		/// <param name="e">an exception related to the error, or <c>null</c></param>
		/// <param name="fmt">an error message format string</param>
		/// <param name="args">arguments for the message format</param>
		public static void FatalError(Exception e, string fmt, params string[] args)
		{
			Console.Error.WriteLine(fmt, args);
			Console.Error.WriteLine(e);
			// HACKHACK: FatalError waits for input
			Console.ReadLine();
		}

		/// <summary>
		/// Handle an unexpected fatal error.
		/// Outputs a user-readable message and may prompt the user
		/// to report the error. May or may not terminate the program.
		/// </summary>
		/// <param name="e">an exception related to the error, or <c>null</c></param>
		/// <param name="fmt">an error message format string</param>
		/// <param name="args">arguments for the message format</param>
		public static void UnexpectedError(Exception e, string fmt, params string[] args)
		{
			Console.Error.WriteLine("An unpected error occured " + fmt, args);
			Console.Error.WriteLine("Please report this error message to the developers.");
			Console.Error.WriteLine(e);
			// HACKHACK: UnexpectedError waits for input
			Console.ReadLine();
		}
	}
}
