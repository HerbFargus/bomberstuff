﻿//
// Bomber Stuff: Atomic Bomberman Remake
//  Copyright © 2008 Thomas Faber
//  All Rights Reserved.
//
// ColorRemapper.cs - utility class to remap colors in a BitmapBuilder
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
// along with Bomber Stuff.  If not, see <http://www.gnu.org/licenses/>.
//

using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

using BomberStuff.Files;
using BomberStuff.Core.Animation;

namespace BomberStuff.Core.Utilities
{
	/// <summary>
	/// The ColorRemapInfo structure contains information about how a color is
	/// to be remapped, including a hue and saturation as well as a lightness
	/// difference setting
	/// </summary>
	public struct ColorRemapInfo
	{
		/// <summary>
		/// A boolean value specifying whether or not to perform hue remapping
		/// </summary>
		public readonly bool SetHue;
		/// <summary>
		/// The new hue to remap to, if hue remapping is performed
		/// </summary>
		public readonly int NewHue;

		/// <summary>
		/// A boolean value specifying whether or not to perform saturation
		/// remapping
		/// </summary>
		public readonly bool SetSaturation;
		/// <summary>
		/// The new saturation to remap to, if saturation remapping is performed
		/// </summary>
		public readonly int NewSaturation;

		/// <summary>The lightness difference to apply during remapping</summary>
		public readonly int DiffLightness;

		/// <summary>
		/// Initializes a new ColorRemapInfo specifying that hue should be
		/// remapped to the specified value
		/// </summary>
		/// <param name="newHue">destination hue value for remapping</param>
		public ColorRemapInfo(int newHue)
			: this(true, newHue, false, 0, 0) { }

		/// <summary>
		/// Initializes a new ColorRemapInfo specifying that saturation and
		/// lightness should be remapped to the specified values
		/// </summary>
		/// <param name="newSaturation">
		/// destination saturation value for remapping
		/// </param>
		/// <param name="diffLightness">lightness difference for remapping</param>
		public ColorRemapInfo(int newSaturation, int diffLightness)
			: this(false, 0, true, newSaturation, diffLightness) { }

		/// <summary>
		/// Initializes a new ColorRemapInfo specifying that hue, saturation and
		/// lightness should be remapped to the specified values
		/// </summary>
		/// <param name="newHue">destination hue value for remapping</param>
		/// <param name="newSaturation">
		/// destination saturation value for remapping
		/// </param>
		/// <param name="diffLightness">lightness difference for remapping</param>
		public ColorRemapInfo(int newHue, int newSaturation, int diffLightness)
			: this(true, newHue, true, newSaturation, diffLightness) { }

		private ColorRemapInfo(bool setHue, int newHue, bool setSaturation,
							int newSaturation, int diffLightness)
		{
			SetHue = setHue;
			NewHue = newHue;
			SetSaturation = setSaturation;
			NewSaturation = newSaturation;
			DiffLightness = diffLightness;
		}

#if DEBUG
		/// <summary>
		/// Returns a string representation of this object
		/// </summary>
		/// <returns>A String representing this ColorRemapInfo</returns>
		public override string ToString()
		{
			string ret = "ColorRemapInfo[";

			if (SetHue)
			{
				ret += NewHue;

				if (SetSaturation)
					ret += ", " + NewSaturation + ", " + DiffLightness;
				else if (DiffLightness != 0)
					ret += "H, " + DiffLightness + "L";
			}
			else if (SetSaturation)
				ret += NewSaturation + ", " + DiffLightness;
			else
				ret += DiffLightness + "L";

			return ret + "]";
		}
#endif
	}

	/// <summary>
	/// Provides color remapping of animation states
	/// </summary>
	public static class ColorRemapper
	{
		/// <summary>The source hue value that shall be remapped</summary>
		/// <value>the hue value for green</value>
		public const int OriginalHue = 135;

		/// <summary>
		/// Remaps all pixels in the specified sequence state's frame that have
		/// a hue value of OriginalHue ± 45 (and a lightness less than 340) to the
		/// new values specified by <c>changes</c>
		/// </summary>
		/// <param name="srcFrame">
		/// the animation frame is frame is to be remapped
		/// </param>
		/// <param name="destFrame">
		/// The animation frame in which the new information should be saved
		/// </param>
		/// <param name="changes">
		/// a ColorRemapInfo specifying the destination color range
		/// </param>
		/// <remarks>
		/// This is an O(width*height) operation - and pixel operations are slow!
		/// Note that this will also loop through the whole image if
		/// <c>changes</c> specifies no remap is to be done. This should be
		/// prevented by the caller.
		/// Also note that the original frame will be overwritten
		/// TODO: this should instead create a new, remapped copy of the frame,
		///       so that copying as an extra operation is not required beforehand
		/// </remarks>
		public static void Remap(AnimationFrame destFrame, AnimationFrame srcFrame, ColorRemapInfo changes)
		{
			BitmapBuilder dest = destFrame.BitmapBuilder;
			BitmapBuilder src = srcFrame.BitmapBuilder;
			ushort newRawKeyColor = 0x7A38;
			Color newKeyColor = Color.FromArgb(247, 140, 198);
			if (changes.SetHue && (changes.NewHue > 220 || changes.NewHue < 60))
			{
				newRawKeyColor = 0x47D8;
				newKeyColor = Color.FromArgb(140, 247, 198);
			}
			byte newKeyColorL = (byte)(newRawKeyColor & 0xFF);
			byte newKeyColorH = (byte)(newRawKeyColor >> 8);

			switch (src.BitsPerPixel)
			{
				// X1R5G5B5 little endian 16 bit color
				// (as found in most animations)
				case 16:
					{
						uint lineSize;

						if (src.Width % 2 == 0)
							lineSize = src.Width;
						else
							lineSize = src.Width + 1;

						// copy everything that's not image data
						Array.Copy(src.BitmapData, dest.BitmapData, (int)src.DataLocation);

						for (int y = 0; y < src.Height; ++y)
							for (int x = 0; x < src.Width; ++x)
							{
								long i = src.DataLocation + 2 * (lineSize * y + x);

								ushort c = (ushort)(src.BitmapData[i]
											+ (src.BitmapData[i + 1] << 8));

								if (c == srcFrame.RawKeyColor)
								{
									dest.BitmapData[i] = newKeyColorL;
									dest.BitmapData[i + 1] = newKeyColorH;
									continue;
								}

								int R = (c >> 10) << 3,
									G = ((c >> 5) & 0x1F) << 3,
									B = (c & 0x1F) << 3;

								R += (int)Math.Ceiling(R * 6.0 / 239.0);
								G += (int)Math.Ceiling(G * 6.0 / 239.0);
								B += (int)Math.Ceiling(B * 6.0 / 239.0);

								int h, s, l;
								RGBToHSL(R, G, B, out h, out s, out l);

								// apply changes to all green color
								if (h > OriginalHue - 45 && h < OriginalHue + 45 && l < 340)
								{
									if (changes.SetHue)
									{
										h += changes.NewHue - OriginalHue;

										if (h < 0)
											h += 360;
									}
									if (changes.SetSaturation)
										s = changes.NewSaturation;
									l += changes.DiffLightness;
									if (l < 0)
										l = 0;
									else if (l > 359)
										l = 359;

									ushort to = ColorToShort(ColorFromHSL(h, s, l));
									byte toL = (byte)(to & 0xFF);
									byte toH = (byte)(to >> 8);

									dest.BitmapData[i] = toL;
									dest.BitmapData[i + 1] = toH;
								}
								else
								{
									dest.BitmapData[i] = src.BitmapData[i];
									dest.BitmapData[i + 1] = src.BitmapData[i + 1];
								}
							}
						break;
					}

				// TODO
				// we can and need currently only remap 16 bpp images
				// This might change when using custom animations
				case 4:
				case 8:
				case 24:
				default:
					throw new FormatException("Cannot remap unexpected"
												+ " bitmap format "
												+ src.BitsPerPixel + " bpp");
			}

			destFrame.SetKeyColor(newRawKeyColor, newKeyColor);
		}

		/// <summary>
		/// A helper function for Remap that converts a Color to its 16 bit
		/// representation
		/// </summary>
		/// <param name="c"></param>
		/// <returns></returns>
		private static ushort ColorToShort(Color c)
		{
			int r = c.R >> 3,
				g = c.G >> 3,
				b = c.B >> 3;

			return (ushort)((r << 10) | (g << 5) | b);
		}

		/// <summary>
		/// A helper function for Remap that converts an RGB color to a HSL color
		/// </summary>
		/// <param name="r"></param>
		/// <param name="g"></param>
		/// <param name="b"></param>
		/// <param name="h"></param>
		/// <param name="s"></param>
		/// <param name="l"></param>
		private static void RGBToHSL(int r, int g, int b,
										out int h, out int s, out int l)
		{
			//calculate min and max
			int min, max;
			if (r > b) { max = r; min = b; }
			else { max = b; min = r; }
			if (g > max) max = g;
			else if (g < min) min = g;
			int maxMinDiff = max - min,
				maxMinSum = max + min;

			l = maxMinSum * 359 / 510;

			if (max == min)
			{
				h = 0;
				s = 0;
			}
			else
			{
				if (max == r)
				{
					h = 60 * (g - b) / maxMinDiff;
					if (h < 0)
						h += 360;
				}
				else if (max == g)
					h = 60 * (b - r) / maxMinDiff + 120;
				else /*if (max == b)*/
					h = 60 * (r - g) / maxMinDiff + 240;

				if (l <= 180)
					s = 359 * maxMinDiff / maxMinSum;
				else
					s = 359 * maxMinDiff / (510 - maxMinSum);
			}
		}

		/// <summary>
		/// A helper function for Remap that converts a hsl color to a Color
		/// structure
		/// </summary>
		/// <param name="h"></param>
		/// <param name="s"></param>
		/// <param name="l"></param>
		/// <returns></returns>
		private static Color ColorFromHSL(int h, int s, int l)
		{
			int q = 255 * ((l < 180)
						? (l + l * s / 359)
						: (l + s - l * s / 359)) / 359;

			int p = 510 * l / 359 - q;

			int tR = (h + 120) % 360;
			int tG = h;
			int tB = (h + 240) % 360;

			return Color.FromArgb(component(q, p, tR),
									component(q, p, tG),
									component(q, p, tB));
		}

		/// <summary>
		/// A helper function for ColorFromHSL
		/// </summary>
		/// <param name="q"></param>
		/// <param name="p"></param>
		/// <param name="t"></param>
		/// <returns></returns>
		private static int component(int q, int p, int t)
		{
			if (t < 60)
				return p + (q - p) * 6 * t / 359;
			else if (t < 180)
				return q;
			else if (t < 240)
				return p + (q - p) * 6 * (240 - t) / 359;
			else
				return p;
		}
	}
}
