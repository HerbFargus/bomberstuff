//
// IInputMethod.cs - IInputMethod interface
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
using System.Collections.Generic;

namespace BomberStuff.Core.Input
{
	/// <summary>
	/// 
	/// </summary>
	public interface IInputMethod
	{
		/// <summary>
		/// 
		/// </summary>
		string Name { get; }

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		Dictionary<string, Control> GetControls();

		/// <summary>
		/// 
		/// </summary>
		/// <param name="control"></param>
		void RegisterControl(Control control);

		/// <summary>
		/// 
		/// </summary>
		/// <param name="control"></param>
		void UnregisterControl(Control control);
	}
}
