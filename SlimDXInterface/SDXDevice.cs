//
// SDXDevice.cs - SlimDX Device class
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
using BomberStuff.Core.Utilities;

using SlimDX.Direct3D9;

namespace BomberStuff.SlimDXInterface
{
	/// <summary>
	/// 
	/// </summary>
	internal class SDXDevice : IDevice, IDisposable
	{
		private Device Device;

		private bool AllowLoadTextures;

		private float XRatio, YRatio;

		/// <summary>
		/// 
		/// </summary>
		/// <param name="device"></param>
		public SDXDevice(Device device)
		{
			Device = device;
			AllowLoadTextures = false;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="device"></param>
		/// <param name="xr"></param>
		/// <param name="yr"></param>
		public SDXDevice(Device device, float xr, float yr)
		{
			Device = device;
			AllowLoadTextures = true;
			XRatio = xr;
			YRatio = yr;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="b"></param>
		/// <param name="storeInVideoMemory"></param>
		/// <param name="keyColor"></param>
		/// <returns></returns>
		public ISprite LoadSprite(BitmapBuilder b, bool storeInVideoMemory, Color keyColor)
		{
			if (!AllowLoadTextures)
				throw new InvalidOperationException("Loading sprites is only allowed inside the LoadSprites event");
			Format format = Format.A8R8G8B8;
			if (keyColor == System.Drawing.Color.Transparent)
				format = Format.R8G8B8;

			Bitmap bmp = new Bitmap(b.GetStream());
			int w = (int)(b.Width * XRatio), h = (int)(b.Height * YRatio);
			if (keyColor != Color.Transparent)
				bmp.MakeTransparent(keyColor);
			bmp = new Bitmap(bmp, w, h);
			using (MemoryStream s = new MemoryStream())
			{
				bmp.Save(s, System.Drawing.Imaging.ImageFormat.Bmp);
				bmp.Dispose();
				s.Seek(0, SeekOrigin.Begin);

				return new SDXSprite(Texture.FromStream(Device, s, w, h, 0, Usage.None, format,
						storeInVideoMemory ? Pool.Managed : Pool.Managed, Filter.None, Filter.None, keyColor.ToArgb()));
			}
			//return new SDXSprite(Texture.FromMemory(Device, b.BitmapData, (int)b.Width, (int)b.Height, 0, Usage.None, format,
			//		storeInVideoMemory ? Pool.Default : Pool.Managed, Filter.None, Filter.None, keyColor.ToArgb()));
		}

		#region Implicit conversions
		/// <summary>
		/// 
		/// </summary>
		/// <param name="d"></param>
		/// <returns></returns>
		public static implicit operator Device(SDXDevice d)
		{
			return d.Device;
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="d"></param>
		/// <returns></returns>
		public static implicit operator SDXDevice(Device d)
		{
			return new SDXDevice(d);
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

			//Device.Dispose();
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
		~SDXDevice()
		{
			Dispose(false);
		}
		#endregion
	}
}
