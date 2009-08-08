//
// Animation.cs - Animation class
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

using System.Collections.Generic;
using BomberStuff.Core.Drawing;
using BomberStuff.Core.UserInterface;

namespace BomberStuff.Core
{
	/// <summary>
	/// Represents an animation, that is, a sequence of images
	/// </summary>
	public class Animation
	{
		/// <summary></summary>
		protected readonly AnimationState[] States;

		/// <summary>
		///  
		/// </summary>
		/// <param name="states"></param>
		public Animation(AnimationState[] states)
		{
			States = states;
		}
	}

	/// <summary>
	/// 
	/// </summary>
	public class AnimationState
	{
		/// <summary>
		/// 
		/// </summary>
		public readonly ISprite Sprite;

		/// <summary>
		/// 
		/// </summary>
		public readonly SizeF Offset;


	}
}
