//
// IUserInterface.cs - IUserInterface interface
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
using BomberStuff.Core.Drawing;

namespace BomberStuff.Core.UserInterface
{
	/// <summary>
	/// 
	/// </summary>
	public interface IUserInterface
	{
		/// <summary>
		/// Boolean value specifying whether all sprites should be
		/// cached (<c>true</c>), or only the most important ones
		/// (<c>false</c>)
		/// </summary>
		bool CacheAllSprites { get; }

		/// <summary>
		/// Initialize the user interface
		/// </summary>
		/// <param name="settings"></param>
		/// <returns>
		/// <c>true</c> on success, <c>false</c> otherwise
		/// </returns>
		bool Initialize(Settings settings);

		/// <summary>
		/// 
		/// </summary>
		/// <remarks>
		/// This method is not expected to return until the user interface
		/// was in some way terminated, either by calling Terminate(), or
		/// by an action such as closing the window.
		/// </remarks>
		void MainLoop();

		/// <summary>
		/// Finalize the user interface
		/// </summary>
		/// <remarks>
		/// This should close the interface, clean up all of its resources
		/// and cause <see cref="MainLoop" /> to return.
		/// </remarks>
		void Terminate();

		/// <summary>
		/// 
		/// </summary>
		/// <remarks>Shall only be called from the Render event handler</remarks>
		/// <param name="sprite"></param>
		/// <param name="position"></param>
		/// <param name="size"></param>
		/// <param name="color"></param>
		void Draw(ISprite sprite, PointF position, SizeF size, System.Drawing.Color color);

		/// <summary>
		/// 
		/// </summary>
		/// <param name="text"></param>
		/// <param name="position"></param>
		/// <param name="color"></param>
		void Draw(string text, PointF position, System.Drawing.Color color);

		/// <summary>
		/// 
		/// </summary>
		event EventHandler<LoadSpritesEventArgs> LoadSprites;
		/// <summary>
		/// 
		/// </summary>
		event EventHandler<RenderEventArgs> Render;

		/// <summary>
		/// 
		/// </summary>
		event EventHandler<EventArgs> Idle;
	}
}
