//
// Bobmer Stuff: Atomic Bomberman Remake
//  Copyright © 2008 Thomas Faber
//  All Rights Reserved.
//
// PCXReader.cs - utility class for reading .pcx files
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
using System.IO;

using Fabba.Utilities;

namespace Bomber.Files
{
	/// <summary>
	/// 
	/// </summary>
public static class PCXReader
{
	/// <summary>
	/// 
	/// </summary>
	/// <param name="filename"></param>
	/// <returns></returns>
	public static Stream StreamFromFile(string filename)
	{
		return FromFile(filename).GetStream();
	}
	/// <summary>
	/// 
	/// </summary>
	/// <param name="filename"></param>
	/// <returns></returns>
	public static BitmapBuilder FromFile(string filename)
	{
		Stream inStream = File.OpenRead(filename);
		BitmapBuilder ret = null;
		try
		{
			ret = FromStream(inStream);
		}
		catch (FormatException e)
		{
			throw new FormatException(filename + ": " + e.Message);
		}
		finally
		{
			inStream.Close();
		}
		
		return ret;

	}
	/// <summary>
	/// 
	/// </summary>
	/// <param name="s"></param>
	/// <returns></returns>
	public static Stream StreamFromStream(Stream s)
	{
		return RawFromStream(s).GetStream();
	}
	/// <summary>
	/// 
	/// </summary>
	/// <param name="s"></param>
	/// <returns></returns>
	public static BitmapBuilder FromStream(Stream s)
	{
		return RawFromStream(s);
	}
	private static BitmapBuilder RawFromStream(Stream s)
	{
		BinaryReader r = new BinaryReader(s);
		
		//
		// Read the PCX header
		//
		
		// Manufacturer, must be 10 (ZSoft)
		int manufacturer = r.ReadByte();
		if (manufacturer != 10)
			throw new FormatException("PCX specifies manufacturer " + manufacturer + ", which is invalid");
		
		// Skip version
		r.ReadByte();
		
		// Encoding, must be 1 (RLE)
		int encoding = r.ReadByte();
		if (encoding != 1)
			throw new FormatException("PCX specifies encoding " + encoding + ", which is invalid");
		
		byte bitsPerPixel = r.ReadByte();
		
		if (bitsPerPixel != 1 && bitsPerPixel != 2 && bitsPerPixel != 4 && bitsPerPixel != 8)
			throw new FormatException("PCX specifies " + bitsPerPixel + " bits per pixel, which is invalid");
		
		ushort xMin = r.ReadUInt16();
		ushort yMin = r.ReadUInt16();
		ushort xMax = r.ReadUInt16();
		ushort yMax = r.ReadUInt16();
		
		// Skip resolution
		r.ReadUInt32();
		
		// Skip short palette if > 4 bpp
		/*if (bitsPerPixel <= 4)
		{
			
		}
		else*/
			s.Seek(48, SeekOrigin.Current);
		
		// Skip reserved
		r.ReadByte();
		
		byte nPlanes = r.ReadByte();
		int bytesPerLine = r.ReadUInt16();
		
		// Go to start of image data
		s.Seek(128, SeekOrigin.Begin);
		
		//
		// Construct the bitmap
		//
		BitmapBuilder b = new BitmapBuilder((ushort)(nPlanes * bitsPerPixel),
												(uint)(xMax - xMin + 1),
												(uint)(yMax - yMin + 1));
		
		// Palette will be inserted later, as it's at the end of the PCX
		long i = b.DataLocation;
		//
		// Run-Length-Decode the PCX data and insert as bitmap data
		//
		byte data = 0;
		
		// we have either one (palette) or three (rgb) planes in a bitmap
		uint planeFactor = (nPlanes == 1U) ? 1U : 3U;
		for (int y = 0; y < b.Height; ++y)
		{
			int count = 0;
			
			for (int iPlane = 0; iPlane < nPlanes; ++iPlane)
			{
				i = b.DataLocation + (b.Height - y - 1) * b.BytesPerLine;
				for (int x = 0; x < bytesPerLine; ++x)
				{
					// we are still "running"
					if (count > 0)
						--count;
					// read new data and check if it's a count
					else if (((data = r.ReadByte()) & 0xC0) == 0xC0)
					{
						// one byte will be processed right now, so subtract 1
						count = (data & 0x3F) - 1;
						// the data to be repeated is the next byte
						data = r.ReadByte();
					}
					
					// ignore padding
					if (x >= b.Width)
						continue;
					
					// we can do at most 3 "planes" in a bitmap, ignore the others
					if (iPlane < 3)
					{
						b.BitmapData[i + (planeFactor - iPlane - 1) % planeFactor] = data;
						i += planeFactor;
					}
				}
			}
		}
		
		// if there's a palette in the bmp, then there must be one in the pcx
		if (b.PaletteSize > 0)
		{
			// HACKHACK: this should actually be done at the start instead of
			//           skipping the palette there, since this destroys the
			//           possibility of a seek-less implementation
			if (b.BitsPerPixel <= 4)
				s.Seek(16, SeekOrigin.Begin);
			else
			{
				s.Seek(-769, SeekOrigin.End);
				if (r.ReadByte() != 12)
					throw new FormatException("Something wrong with the palette");
			}
			
			// read palette and put into bitmap data
			b.ReadPaletteFromPCX(r);
		}
		
		return b;
	}
}
}