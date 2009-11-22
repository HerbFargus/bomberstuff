﻿//
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

using BomberStuff.Core.Drawing;
using BomberStuff.Core.UserInterface;

using BomberStuff.Core.Utilities;

namespace BomberStuff.Core.Animation
{
	/// <summary>
	/// Represents an animation, that is, a sequence of images
	/// </summary>
	public class Animation
	{
#if DEBUG
		/// <summary></summary>
		public string Name;
#endif
		/// <summary></summary>
		public bool VideoMemory;
		/// <summary></summary>
		public bool Cached = true;
		/// <summary></summary>
		public AnimationFrame[] Frames;
		/// <summary></summary>
		public SizeF[] FrameOffset;

		/// <summary>
		///  
		/// </summary>
		public Animation()
		{
			
		}

		/// <summary>
		/// Remap the animation to a new color
		/// </summary>
		/// <param name="playerRemapInfo"></param>
		public void Remap(ColorRemapInfo[] playerRemapInfo)
		{
			for (int i = 0; i < Frames.Length; ++i)
				Frames[i].Remap(playerRemapInfo);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="device"></param>
		/// <param name="frame"></param>
		/// <returns></returns>
		public ISprite GetSprite(IDevice device, int frame)
		{
			return Frames[frame].GetSprite(device, VideoMemory);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="device"></param>
		/// <param name="frame"></param>
		/// <param name="player"></param>
		/// <returns></returns>
		public ISprite GetSprite(IDevice device, int frame, int player)
		{
			return Frames[frame].GetSprite(device, VideoMemory, player);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="frame"></param>
		/// <returns></returns>
		public SizeF GetSpriteSize(int frame)
		{
			return Frames[frame].Size;
		}
		
		/// <summary>
		/// 
		/// </summary>
		/// <param name="frame"></param>
		/// <returns></returns>
		public SizeF GetOffset(int frame)
		{
			//System.Console.WriteLine(": " + Frames[frame].Offset + " / " + FrameOffset[frame]);
			return Frames[frame].Offset - FrameOffset[frame] - new SizeF(18.0f / 40.0f, 16.0f / 36.0f);
		}
	}

	/// <summary>
	/// 
	/// </summary>
	public class AnimationFrame
	{
		/// <summary></summary>
		public ISprite Sprite;
		/// <summary></summary>
		public BitmapBuilder BitmapBuilder;
		/// <summary>size of the frame, in fields</summary>
		public SizeF Size;
		/// <summary></summary>
		public SizeF Offset;
		/// <summary></summary>
		public System.Drawing.Color KeyColor;
		/// <summary></summary>
		public ushort RawKeyColor;
#if DEBUG
		/// <summary></summary>
		public string FileName;
#endif
		/// <summary>
		/// Remapped copies of this frame, <c>null</c> if it
		/// has not been remapped or is a remapped copy itself
		/// </summary>
		public AnimationFrame[] RemappedCopies;

		// HACKHACK: AnimationFrame() constructor shouldn't be required; most fields should be readonly
		/// <summary>
		/// 
		/// </summary>
		public AnimationFrame()
		{
		}

		/// <summary>
		/// Create a deep (enough) copy of another frame
		/// </summary>
		/// <param name="old"></param>
		public AnimationFrame(AnimationFrame old)
		{
			// TRYTRY: do we need to copy this? That can probably be optimized:
			// 1) if remap actually occurs, create a new BB in remap
			// 2) if no remap occurs, use the original BB
			// see ColorRemapper.Remap
			BitmapBuilder = new BitmapBuilder(old.BitmapBuilder);
			Offset = old.Offset;
			KeyColor = old.KeyColor;
			RawKeyColor = old.RawKeyColor;
#if DEBUG
			FileName = old.FileName;
#endif
			// this is
			RemappedCopies = null;
		}

		/// <summary>
		/// Sets the frame's key color
		/// </summary>
		/// <param name="newRawKeyColor"></param>
		/// <param name="newKeyColor"></param>
		public void SetKeyColor(ushort newRawKeyColor, System.Drawing.Color newKeyColor)
		{
			RawKeyColor = newRawKeyColor;
			KeyColor = newKeyColor;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="playerRemapInfo"></param>
		public void Remap(ColorRemapInfo[] playerRemapInfo)
		{
			//System.Diagnostics.Debug.Assert(RemappedCopies != null, "Trying to remap a frame twice!");
			if (RemappedCopies != null)
				return;
			RemappedCopies = new AnimationFrame[playerRemapInfo.Length];

			for (int i = 0; i < playerRemapInfo.Length; ++i)
			{
				RemappedCopies[i] = new AnimationFrame(this);
				ColorRemapper.Remap(RemappedCopies[i], this, playerRemapInfo[i]);
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="device"></param>
		/// <param name="videoMemory"></param>
		/// <returns></returns>
		public ISprite GetSprite(IDevice device, bool videoMemory)
		{
			System.Diagnostics.Debug.Assert(RemappedCopies == null, "Getting original Sprite for a remapped frame");
			if (Sprite == null)
			{
				Sprite = device.LoadSprite(BitmapBuilder.GetStream(), (int)BitmapBuilder.Width, (int)BitmapBuilder.Height, videoMemory, KeyColor);
				System.Diagnostics.Debug.Assert(Sprite != null);
			}

			return Sprite;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="device"></param>
		/// <param name="videoMemory"></param>
		/// <param name="player"></param>
		/// <returns></returns>
		public ISprite GetSprite(IDevice device, bool videoMemory, int player)
		{
			if (RemappedCopies == null)
				throw new System.InvalidOperationException("Requesting player copy of non-remapped frame");
			
			if (Sprite == null)
			{
				System.Diagnostics.Debug.Assert(player < RemappedCopies.Length);
				BitmapBuilder b = RemappedCopies[player].BitmapBuilder;
				Sprite = device.LoadSprite(b.GetStream(), (int)b.Width, (int)b.Height, videoMemory, RemappedCopies[player].KeyColor);
				System.Diagnostics.Debug.Assert(Sprite != null);
			}

			return Sprite;
		}
	}
}
