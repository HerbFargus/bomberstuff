//
// Bomber Stuff: Atomic Bomberman Remake
//  Copyright © 2008-2009 Thomas Faber
//
// BitmapBuilder.cs - utility class for creating a bitmap file in a byte array
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

namespace BomberStuff.Core.Utilities
{

/// <summary>
/// A wrapper object providing functionality for creating raw
/// bitmap data from different image formats
/// </summary>
public class BitmapBuilder : IDisposable
{
	/// <summary>The size of a bitmap file header</summary>
	public const int BitmapHeaderSize = 54;
	/// <summary>The size of the bitmap's palette</summary>
	public readonly uint PaletteSize;
	/// <summary>
	/// The index into the BitmapData array at which the palette starts,
	/// if present
	/// </summary>
	public readonly uint PaletteLocation;
	/// <summary>
	/// The index into the BitmapData array at which the image data starts
	/// </summary>
	public readonly uint DataLocation;

	private uint m_Width, m_Height;
	/// <summary>Width of the bitmap</summary>
	public uint Width { get { return m_Width; } }
	/// <summary>Height of the bitmap</summary>
	public uint Height { get { return m_Height; } }
	/// <summary></summary>
	public readonly ushort BitsPerPixel;
	private uint m_BytesPerLine;
	/// <summary></summary>
	public uint BytesPerLine { get { return m_BytesPerLine; } }
	private uint m_PaddingPerLine;
	/// <summary></summary>
	public uint PaddingPerLine { get { return m_PaddingPerLine; } }

	private byte[] m_BitmapData;
	/// <summary>The raw bitmap data</summary>
	public byte[] BitmapData { get { return m_BitmapData; } }
	
	private Stream m_Stream;

	#region Constructors

	/// <summary>
	/// Copy constructor. Performs a shallow copy, and creates a new,
	/// blank array of previous size in BitmapData
	/// </summary>
	/// <param name="old"></param>
	public BitmapBuilder(BitmapBuilder old)
	{
		PaletteSize = old.PaletteSize;
		PaletteLocation = old.PaletteLocation;
		DataLocation = old.DataLocation;
		m_Width = old.Width;
		m_Height = old.Height;
		BitsPerPixel = old.BitsPerPixel;
		m_BytesPerLine = old.BytesPerLine;
		m_PaddingPerLine = old.PaddingPerLine;
		m_BitmapData = new byte[old.BitmapData.Length];
	}
	
	/// <summary>
	/// Constructor that creates the byte array required to hold a bitmap file
	/// with the specified properties and initializes the bitmap header
	/// information
	/// </summary>
	/// <param name="bitsPerPixel">
	/// Number of bits per pixel in the bitmap.
	/// </param>
	/// <param name="width">The width of the bitmap image</param>
	/// <param name="height">The height of the bitmap image</param>
	/// <remarks>
	/// Bitmap images support <paramref name="bitsPerPixel" /> values of
	/// 1, 4, 8, 16, 24 or 32.
	/// Images with less than 16 bits per pixel will need a palette of 2^bpp
	/// 32-bit entries, which is allocated by the constructor.
	/// </remarks>
	public BitmapBuilder(ushort bitsPerPixel, uint width, uint height)
	{
		// calculate some values
		BitsPerPixel = bitsPerPixel;
		m_Width = width;
		m_Height = height;
		
		uint bmpBytesPerLine = (uint)Math.Ceiling(Width * BitsPerPixel / 8.0);
		
		// bitmap data lines are always a multiple of four bytes
		m_PaddingPerLine = (4 - bmpBytesPerLine % 4) % 4;

		m_BytesPerLine = bmpBytesPerLine + PaddingPerLine;
		
		
		if (BitsPerPixel >= 16)
			PaletteSize = 0;
		else
			PaletteSize = 4U * (1U << BitsPerPixel); // palette is 4 * 2^bpp bytes long
		
		// Create the buffer for bitmap data
		uint bmpSize = (uint)(BitmapHeaderSize + PaletteSize + BytesPerLine * Height);
		byte[] bmpData = new byte[bmpSize];
		
		// Fill bitmap header
		uint i = 0;
		bmpData[i++] = (byte)'B';
		bmpData[i++] = (byte)'M';
		bmpData[i++] = (byte)(bmpSize & 0xFF);
		bmpData[i++] = (byte)((bmpSize >> 8) & 0xFF);
		bmpData[i++] = (byte)((bmpSize >> 16) & 0xFF);
		bmpData[i++] = (byte)((bmpSize >> 24) & 0xFF);
		i = 10;
		bmpData[i++] = (byte)((BitmapHeaderSize + PaletteSize) & 0xFF);
		bmpData[i++] = (byte)(((BitmapHeaderSize + PaletteSize) >> 8) & 0xFF);
		bmpData[i++] = (byte)(((BitmapHeaderSize + PaletteSize) >> 16) & 0xFF);
		bmpData[i++] = (byte)(((BitmapHeaderSize + PaletteSize) >> 24) & 0xFF);
		bmpData[i++] = (byte)40;
		i = 18;
		bmpData[i++] = (byte)(Width & 0xFF);
		bmpData[i++] = (byte)((Width >> 8) & 0xFF);
		bmpData[i++] = (byte)((Width >> 16) & 0xFF);
		bmpData[i++] = (byte)((Width >> 24) & 0xFF);
		bmpData[i++] = (byte)(Height & 0xFF);
		bmpData[i++] = (byte)((Height >> 8) & 0xFF);
		bmpData[i++] = (byte)((Height >> 16) & 0xFF);
		bmpData[i++] = (byte)((Height >> 24) & 0xFF);
		bmpData[i++] = (byte)1; ++i; // always one plane in a bitmap
		bmpData[i++] = (byte)(BitsPerPixel & 0xFF);
		bmpData[i++] = (byte)((BitsPerPixel >> 8) & 0xFF);

		m_BitmapData = bmpData;
		
		PaletteLocation = BitmapHeaderSize;
		DataLocation = PaletteLocation + PaletteSize;
	}

	#endregion

	/// <summary>
	/// Creates a new <see cref="Stream" /> backed by the BuildmapBuilder's
	/// bitmap data
	/// </summary>
	/// <returns>a Stream that will provide bitmap data</returns>
	/// <remarks>
	/// This method's return value is primarily intended for use
	/// with <see cref="System.Drawing.Bitmap(System.IO.Stream)" /> or similar (for
	/// example Texture creation) functions
	/// </remarks>
	public Stream GetStream()
	{
		if (m_Stream == null)
			m_Stream = new MemoryStream(BitmapData);

		return m_Stream;
	}
	
	/// <summary>
	/// Reads a bitmap-format palette from the specified <see cref="Stream" />
	/// and inserts it into the bitmap data
	/// </summary>
	/// <param name="s">the Stream to read the palette from</param>
	/// <remarks>
	/// This will read a constant amount of <see cref="PaletteSize" /> bytes.
	/// The caller needs to make sure that this amount of data is available.
	/// </remarks>
	public void ReadPaletteFromBitmap(Stream s)
	{
		// a bitmap palette can simply be copied
		s.Read(m_BitmapData, (int)PaletteLocation, (int)PaletteSize);
	}
	
	/// <summary>
	/// Reads a PCX-format palette from the specified <see cref="Stream" />
	/// and inserts it into the bitmap data
	/// </summary>
	/// <param name="s">the Stream to read the palette from</param>
	/// <remarks>
	/// TODO: This will read a currently unknown number of bytes.
	/// </remarks>
	public void ReadPaletteFromPCX(Stream s)
	{
		ReadPaletteFromPCX(new BinaryReader(s));
	}
	
	/// <summary>
	/// Reads a PCX-format palette from a Stream with the help of the
	/// specified <see cref="BinaryReader" /> and inserts it into the bitmap
	/// data
	/// </summary>
	/// <param name="r">the BinaryReader to read the palette with</param>
	/// <remarks>
	/// TODO: This will read a currently unknown number of bytes.
	/// </remarks>
	public void ReadPaletteFromPCX(BinaryReader r)
	{
		for (uint i = PaletteLocation; i < DataLocation; ++i)
		{
			int R = r.ReadByte(),
				G = r.ReadByte(),
				B = r.ReadByte();

			m_BitmapData[i++] = (byte)B;
			m_BitmapData[i++] = (byte)G;
			m_BitmapData[i++] = (byte)R;
			// skip one more byte, as bmp uses 32 bit palette entries
		}
	}
	
	/// <summary>
	/// Reads the actual image data from a Stream with the help of the
	/// specified <see cref="BinaryReader" /> and insert it into the bitmap
	/// data
	/// </summary>
	/// <param name="r">the BinaryReader to read the data with</param>
	/// <param name="maxLength">
	/// the maximum number of bytes to be read from the stream. Pass
	/// <see cref="UInt64.MaxValue" /> if there is no limit
	/// </param>
	/// <returns>
	/// A positive number of bytes read from the stream if successful, the
	/// negative of the number of bytes read on failure, zero if no bytes have
	/// been read.
	/// </returns>
	/// <remarks>
	/// The data will be read using the Run-length-encoding format used in
	/// Atomic bomberman ANI files, which is (exactly?) the same as that used
	/// in TGA files.
	/// </remarks>
	// TODO: probably make ReadDataFromAni work on the stream instead?
	// it only uses ReadByte anyway
	public long ReadDataFromAni(BinaryReader r, ulong maxLength)
	{
		long bytesRead = 0;
		
		// this is one RLE-unit (usually a pixel, two for 4 bpp)
		uint bytesPerUnit = (uint)Math.Ceiling(BitsPerPixel / 8.0);
		
		// is RLE active (or are we in a raw block)?
		bool rle = false;
		// the data unit that is repeated in an RLE block
		byte[] data = new byte[bytesPerUnit];
		// and the number of times it is to be repeated
		// (or the number of raw bytes)
		uint count = 0;
		
		// our array index. Will jump around a bit as bitmaps are bottom-up
		uint i;
		
		// we will find the pixels ordered correctly, so we loop through
		// them, and x and y provide the current pixel's location
		for (int y = 0; y < Height; ++y)
		{
			// calculate the position of the current line
			// invert the y coordinate, then find the start of that line
			i = (uint)(DataLocation + (Height - y - 1) * BytesPerLine);
			//Console.WriteLine("Reading line {0}, read {1} bytes so far", y, bytesRead);
			for (int x = 0; x < Width; ++x)
			{
				if (y == 107)
				{
					//Console.WriteLine("Reading column {0}, read {1} bytes so far, rle mode {2}, count is {3}", y, bytesRead, rle, count);
				}
				// if count is zero, a new block starts, either raw or RLE
				if (count == 0)
				{
					// check whether we can still read a byte
					if ((ulong)bytesRead + 1 > maxLength)
						return -bytesRead;
					
					// this is the status/count byte
					count = r.ReadByte();
					++bytesRead;
					
					// if bit 7 is set, this denotes the start of an RLE block
					if ((count & 0x80) == 0x80)
					{
						// unset bit 7 to get the repeat count minus one
						// the one unit will be added right below in this loop
						count -= 0x80;
						rle = true;
						
						// check whether we can still read a unit
						if ((ulong)bytesRead + bytesPerUnit > maxLength)
							return -bytesRead;
						
						for (int j = 0; j < bytesPerUnit; ++j)
							data[j] = r.ReadByte();
						
						bytesRead += bytesPerUnit;
					}
					// if bit 7 is unset, count is the number of raw units to
					// copy minus one. The one unit will be copied right below
					// in this loop
					else
					{
						rle = false;
						
						// return failure if we cannot read so many units
						if ((ulong)bytesRead + count * bytesPerUnit > maxLength)
							return -bytesRead;
					}
				}
				// we are in the middle of a block. We decrease the count and
				// then copy the apropriate item below
				else
					--count;
				
				// in RLE mode we just add the saved data unit once
				if (rle)
					for (int j = 0; j < bytesPerUnit; ++j)
						m_BitmapData[i++] = data[j];
				// in raw mode, we copy one unit
				else
				{
					for (int j = 0; j < bytesPerUnit; ++j)
						m_BitmapData[i++] = r.ReadByte();
					
					bytesRead += bytesPerUnit;
				}
			}
		}
		
		// HACKHACK: debug stuff. This should probably be returned somehow
		if (count > 0)
			Console.WriteLine("{0} bytes of extra data encoded in image", count);
		
		// success. we return the new position
		return bytesRead;
	}
#if DEBUG
	private static long texturesCropped = 0;
	private static long m_TexturesCropped = 0;
	private static long m_RTexturesCropped = 0;
	/// <summary>Debug/Optimization/Test Value</summary>
	public static long TexturesCropped { get { return m_TexturesCropped; } }
	/// <summary>Debug/Optimization/Test Value</summary>
	public static long RTexturesCropped { get { return m_RTexturesCropped; } }
	private static long columnsCropped = 0;
	private static long m_ColumnsCropped = 0;
	private static long m_RColumnsCropped = 0;
	/// <summary>Debug/Optimization/Test Value</summary>
	public static long ColumnsCropped { get { return m_ColumnsCropped; } }
	/// <summary>Debug/Optimization/Test Value</summary>
	public static long RColumnsCropped { get { return m_RColumnsCropped; } }
	private static long rowsCropped = 0;
	private static long m_RowsCropped = 0;
	private static long m_RRowsCropped = 0;
	/// <summary>Debug/Optimization/Test Value</summary>
	public static long RowsCropped { get { return m_RowsCropped; } }
	/// <summary>Debug/Optimization/Test Value</summary>
	public static long RRowsCropped { get { return m_RRowsCropped; } }
	private static long bytesSavedByCropping = 0;
	private static long m_BytesSavedByCropping = 0;
	private static long m_RBytesSavedByCropping = 0;
	/// <summary>Debug/Optimization/Test Value</summary>
	public static long BytesSavedByCropping { get { return m_BytesSavedByCropping; } }
	/// <summary>Debug/Optimization/Test Value</summary>
	public static long RBytesSavedByCropping { get { return m_RBytesSavedByCropping; } }

	/// <summary>
	/// 
	/// </summary>
	/// <param name="remapped"></param>
	public static void SeqCropDone(bool remapped)
	{
		if (remapped)
		{
			m_RTexturesCropped += texturesCropped; texturesCropped = 0;
			m_RRowsCropped += rowsCropped; rowsCropped = 0;
			m_RColumnsCropped += columnsCropped; columnsCropped = 0;
			m_RBytesSavedByCropping += bytesSavedByCropping; bytesSavedByCropping = 0;
		}
		else
		{
			m_TexturesCropped += texturesCropped; texturesCropped = 0;
			m_RowsCropped += rowsCropped; rowsCropped = 0;
			m_ColumnsCropped += columnsCropped; columnsCropped = 0;
			m_BytesSavedByCropping += bytesSavedByCropping; bytesSavedByCropping = 0;
		}
	}

	/// <summary>
	/// 
	/// </summary>
	/// <returns></returns>
	public static long MissingTextures()
	{
		return texturesCropped;
	}
#endif
	/// <summary>
	/// 
	/// </summary>
	/// <param name="keyColor"></param>
	/// <param name="rawKeyColor"></param>
	/// <param name="dLeft"></param>
	/// <param name="dTop"></param>
	/// <returns></returns>
	public BitmapBuilder Crop(System.Drawing.Color keyColor, ushort rawKeyColor, out uint dLeft, out uint dTop)
	{
		BitmapBuilder newBB;
		
		switch (BitsPerPixel)
		{
			case 16:
				//
				// bottom cropping
				//
				uint cropBottom = 0;
				bool done = false;
				
				for (long y = 0; y < Height; ++y)
				{
					long i = DataLocation + BytesPerLine * y;
					for (long x = 0; x < Width; ++x)
					{
						ushort color = (ushort)(BitmapData[i] + (BitmapData[i + 1] << 8));

						if (color != rawKeyColor)
						{
							done = true;
							break;
						}
						i += 2;
					}
					if (done)
						break;
					++cropBottom;
				}
				
				// is the image all transparent?
				if (cropBottom == m_Height)
				{
					// leave a 1x1 transparent image
					newBB = new BitmapBuilder(BitsPerPixel, 1, 1);
					
					newBB.BitmapData[BitmapHeaderSize] = (byte)(rawKeyColor & 0xFF);
					newBB.BitmapData[BitmapHeaderSize] = (byte)((rawKeyColor >> 8) & 0xFF);
#if DEBUG
					rowsCropped += m_Height - 1;
					columnsCropped += m_Width - 1;
#endif
					dLeft = 0;
					dTop = 0;
					// done. exit
					break;
				}

				//
				// top cropping
				//
				uint cropTop = 0;
				done = false;
				
				for (long y = Height - 1; y >= 0; --y)
				{
					long i = DataLocation + BytesPerLine * y;
					for (long x = 0; x < Width; ++x)
					{
						ushort color = (ushort)(BitmapData[i] + (BitmapData[i + 1] << 8));
						
						if (color != rawKeyColor)
						{
							done = true;
							break;
						}
						i += 2;
					}
					if (done)
						break;
					++cropTop;
				}
				
				//
				// left cropping
				//
				uint cropLeft = 0;
				done = false;

				for (long x = 0; x < Width; ++x)
				{
					for (long y = cropBottom; y < m_Height - cropTop; ++y)
					{
						long i = DataLocation + BytesPerLine * y + 2 * x;
						ushort color = (ushort)(BitmapData[i] + (BitmapData[i + 1] << 8));

						if (color != rawKeyColor)
						{
							done = true;
							break;
						}
					}
					if (done)
						break;
					++cropLeft;
				}
				
				//
				// right cropping
				//
				uint cropRight = 0;
				done = false;
				
				for (long x = Width - 1; x >= 0; --x)
				{
					for (long y = cropBottom; y < m_Height - cropTop; ++y)
					{
						long i = DataLocation + BytesPerLine * y + 2 * x;
						ushort color = (ushort)(BitmapData[i] + (BitmapData[i + 1] << 8));
						
						if (color != rawKeyColor)
						{
							done = true;
							break;
						}
					}
					if (done)
						break;
					++cropRight;
				}
				
				//
				// do cropping
				//
				
				// do we have anything to crop?
				if (cropTop + cropBottom + cropLeft + cropRight > 0)
				{
					uint newWidth = (uint)(m_Width - cropLeft - cropRight);
					uint newHeight = (uint)(m_Height - cropTop - cropBottom);
					
					newBB = new BitmapBuilder(BitsPerPixel, newWidth, newHeight);
					
					// bytes per line (to copy). without padding.
					uint newBytesPerLine = newBB.BytesPerLine - newBB.m_PaddingPerLine;
					uint newBytesPerLineWithPadding = newBB.BytesPerLine;
					
					for (int y = 0; y < newHeight; ++y)
						Array.Copy(m_BitmapData, (int)(BitmapHeaderSize + (y + cropBottom) * BytesPerLine + 2 * cropLeft),
										newBB.BitmapData, (int)(BitmapHeaderSize + y * newBytesPerLineWithPadding),
										(int)newBytesPerLine);
				}
				else
					newBB = this;
				
#if DEBUG
				rowsCropped += cropTop + cropBottom;
				columnsCropped += cropLeft + cropRight;
#endif
				
				dLeft = cropLeft;
				dTop = cropTop;
				
				break;
			// TODO: image formats
			// we can and need currently only crop 16 bpp images
			// This might change when using custom animations
			case 4:
			case 8:
			case 24:
			default:
				throw new FormatException("Cannot remap unexpected"
											+ " bitmap format "
											+ BitsPerPixel + " bpp");
		}

#if DEBUG
		bytesSavedByCropping += BitmapData.Length - newBB.BitmapData.Length;
#endif
		++texturesCropped;
		return newBB;
	}

	#region IDisposable implementation
	/// <summary>
	/// Disposes of the object's unmanaged and optionally
	/// managed resources
	/// </summary>
	/// <param name="disposing">
	/// <c>true</c> to free managed resources
	/// </param>
	protected virtual void Dispose(bool disposing)
	{
		if (disposing)
		{
			m_BitmapData = null;
		}

		if (m_Stream != null)
		{
			m_Stream.Close();
			m_Stream = null;
		}
	}

	/// <summary>
	/// Disposes of the object's managed and unmanaged resources
	/// </summary>
	public void Dispose()
	{
		Dispose(true);
		GC.SuppressFinalize(this);
	}

	/// <summary>
	/// Finalizer. Disposes of unmanaged resources
	/// </summary>
	~BitmapBuilder()
	{
		Dispose(false);
	}
	#endregion
}


}