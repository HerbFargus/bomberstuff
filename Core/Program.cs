//
// Program.cs - Program class
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
using System.IO;
using System.Reflection;

namespace BomberStuff.Core
{
	/// <summary>
	/// 
	/// </summary>
	public class Program
	{
		/// <summary>
		/// Program entry. Handles command line arguments and loads
		/// plugins accordingly
		/// </summary>
		/// <param name="args">Program arguments</param>
		static void Main(string[] args)
		{
			
			string uiName = "WinFormsInterface";

			try
			{
				Assembly assembly = Assembly.LoadFrom(uiName + ".dll");
				object uiObject = assembly.CreateInstance("BomberStuff." + uiName + ".UserInterface");

				IUserInterface ui = uiObject as IUserInterface;

				if (uiObject == null)
					throw new MissingMethodException();

				ui.Initialize();
				ui.StartMainLoop();
				ui.Terminate();
			}
			catch (FileNotFoundException e)
			{
				ErrorHandling.FatalError(e, @"The interface module ""{0}"" or one of its"
											+ @" dependencies could not be found:",
												uiName);
			}
			/* This is not in CF. Does it throw an alternative error? TRYTRY
			catch (FileLoadException e)
			{
				ErrorHandling.FatalError(e, @"The interface module ""{0}"" or one of its"
											+ @" dependencies could not be loaded:\n{1}",
												uiName);
			}*/
			catch (BadImageFormatException e)
			{
				ErrorHandling.FatalError(e, @"The interface module ""{0}"" or one of its"
											+ @" dependencies could has an invalid format:",
												uiName);
			}
			catch (MissingMethodException e)
			{
				ErrorHandling.FatalError(e, @"The module ""{0}"" is not a valid BomberStuff"
											+ @" interface module:",
												uiName);
			}
			catch (Exception e)
			{
				ErrorHandling.UnexpectedError(e, @"while trying to load the interface module {0}:",
														uiName);
			}
		}
	}
}
