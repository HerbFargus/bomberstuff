//
// Interface.cs - Interface class
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
using System.Drawing;

using BomberStuff.Core.UserInterface;

namespace BomberStuff.WinFormsInterface
{
	/// <summary>
	/// 
	/// </summary>
	internal class Sprite : ISprite, IDisposable
	{
		/// <summary></summary>
		public readonly Bitmap Bitmap;

		/// <summary>
		/// 
		/// </summary>
		/// <param name="bitmap"></param>
		public Sprite(Bitmap bitmap)
		{
			Bitmap = bitmap;
		}

		#region Implicit conversions
		/// <summary>
		/// 
		/// </summary>
		/// <param name="s"></param>
		/// <returns></returns>
		public static implicit operator Bitmap(Sprite s)
		{
			return s.Bitmap;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="bitmap"></param>
		/// <returns></returns>
		public static implicit operator Sprite(Bitmap bitmap)
		{
			return new Sprite(bitmap);
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
				Bitmap.Dispose();
			}
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
		~Sprite()
		{
			Dispose(false);
		}
		#endregion
	}

	/// <summary>
	/// 
	/// </summary>
	public class Device : IDevice, IDisposable
	{
		/// <summary>
		/// 
		/// </summary>
		/// <param name="s"></param>
		/// <param name="storeInVideoMemory"></param>
		/// <param name="keyColor"></param>
		/// <returns></returns>
		public ISprite LoadSprite(Stream s, bool storeInVideoMemory, Color keyColor)
		{
			Bitmap bitmap = new Bitmap(s);
			//bitmap.MakeTransparent(keyColor);
			return new Sprite(bitmap);
		}

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
		~Device()
		{
			Dispose(false);
		}
		#endregion
	}
}
