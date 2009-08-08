//
// LoadSpritesEventArgs.cs - LoadSpritesEventArgs class
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

namespace BomberStuff.Core.UserInterface
{
	/// <summary>
	/// Event arguments for <see cref="IUserInterface.LoadSprites" />.
	/// Contains the <see cref="IDevice" /> needed to load the sprites.
	/// </summary>
	public class LoadSpritesEventArgs : EventArgs
	{
		/// <summary>
		/// The device for which the sprites should be loaded
		/// </summary>
		public readonly IDevice Device;

		/// <summary>
		/// Initialize a new LoadSpritesEventArgs object
		/// </summary>
		/// <param name="device">
		/// The device for which the sprites should be loaded
		/// </param>
		public LoadSpritesEventArgs(IDevice device)
		{
			Device = device;
		}
	}
}
