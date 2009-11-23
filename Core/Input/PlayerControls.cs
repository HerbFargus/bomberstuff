//
// PlayerControls.cs - PlayerControls class
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
	public sealed class PlayerControls
	{
		/// <summary>
		/// 
		/// </summary>
		public enum Types
		{
			/// <summary></summary>
			Up,
			/// <summary></summary>
			Down,
			/// <summary></summary>
			Left,
			/// <summary></summary>
			Right,

			/// <summary></summary>
			Action1,
			/// <summary></summary>
			Action2
		}

		private List<KeyValuePair<Types, Control>> Controls;

		/// <summary>
		/// 
		/// </summary>
		/// <param name="controls"></param>
		public PlayerControls(List<KeyValuePair<Types, Control>> controls)
		{
			Controls = controls;

			foreach (KeyValuePair<Types, Control> control in Controls)
			{
				control.Value.Pressed += Control_Event;
				control.Value.Released += Control_Event;
			}
		}

		#region Events
		/// <summary>
		/// 
		/// </summary>
		public event EventHandler<PlayerControlEventArgs> Pressed;
		/// <summary>
		/// 
		/// </summary>
		public event EventHandler<PlayerControlEventArgs> Released;

		/// <summary>
		/// 
		/// </summary>
		/// <param name="e"></param>
		private /*protected virtual*/ void OnPressed(PlayerControlEventArgs e)
		{
			if (Pressed != null)
				Pressed(this, e);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="e"></param>
		private /*protected virtual*/ void OnReleased(PlayerControlEventArgs e)
		{
			if (Released != null)
				Released(this, e);
		}
		#endregion

		private void Control_Event(object sender, ControlEventArgs e)
		{
			foreach (KeyValuePair<Types, Control> control in Controls)
			{
				if (control.Value == sender)
					if (e.Pressed)
					{
						//System.Console.WriteLine("Player's " + control.Key + " key pressed");
						OnPressed(new PlayerControlEventArgs(control.Key, true));
					}
					else
					{
						//System.Console.WriteLine("Player's " + control.Key + " key released");
						OnReleased(new PlayerControlEventArgs(control.Key, false));
					}
			}
		}
	}
}
