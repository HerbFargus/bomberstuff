//
// SDXSprite.cs - SlimDX Sprite class
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

using BomberStuff.Core.UserInterface;

using SlimDX.Direct3D9;

namespace BomberStuff.SlimDXInterface
{
	/// <summary>
	/// 
	/// </summary>
	internal class SDXSprite : ISprite, IDisposable
	{
		/// <summary></summary>
		public readonly Texture Texture;

		/// <summary>
		/// 
		/// </summary>
		/// <param name="texture"></param>
		public SDXSprite(Texture texture)
		{
			Texture = texture;
		}

		#region Implicit conversions
		/// <summary>
		/// 
		/// </summary>
		/// <param name="s"></param>
		/// <returns></returns>
		public static implicit operator Texture(SDXSprite s)
		{
			return s.Texture;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="texture"></param>
		/// <returns></returns>
		public static implicit operator SDXSprite(Texture texture)
		{
			return new SDXSprite(texture);
		}
		#endregion

		#region IDisposable implementation
		/// <summary>
		/// 
		/// </summary>
		/// <param name="disposing"></param>
		protected virtual void Dispose(bool disposing)
		{
			if (disposing)
			{

			}
			Texture.Dispose();
		}

		/// <summary>
		/// 
		/// </summary>
		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		/// <summary>
		/// 
		/// </summary>
		~SDXSprite()
		{
			Dispose(false);
		}
		#endregion
	}
}
