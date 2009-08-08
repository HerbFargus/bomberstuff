//
// Bomber Stuff: Atomic Bomberman Remake
//  Copyright © 2008 Thomas Faber
//  All Rights Reserved.
//
// Animation.cs - interface for reading Atomic Bomerman .ani files
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

using SlimDX.Direct3D9;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Threading;

using Fabba.Utilities;

namespace Bomber.Files
{
/// <summary>
/// Represents an Atomic Bomberman .ANI animation file
/// </summary>
public class Animation
{
	/// <summary>A list of the image frames in the animation</summary>
	public List<Frame> Frames = new List<Frame>();
	/// <summary>A list of the animation sequences in the file</summary>
	public List<Sequence> Sequences = new List<Sequence>();
	private bool ExtraData;
	
	/// <summary>
	/// Parses the specified AB animation file
	/// </summary>
	/// <param name="filename">Path to the .ani file</param>
	public Animation(string filename)
		: this(filename, false) { }
	
	/// <summary>
	/// Parses the specified AB animation file
	/// </summary>
	/// <param name="filename">Path to the .ani file</param>
	/// <param name="extraData">
	/// A boolean value specifying whether to output extra (debug)
	/// information to the console
	/// </param>
	public Animation(string filename, bool extraData)
	{
		ExtraData = extraData;
		Stream s = File.OpenRead(filename);
		try
		{
			Create(s);
		}
		catch (FormatException e)
		{
			throw new FormatException(filename + ": " + e.Message);
		}
		finally
		{
			s.Close();
		}
	}
	
	/// <summary>
	/// Parses an animation file read from the specified stream
	/// </summary>
	/// <param name="s">The stream to read ani data from</param>
	public Animation(Stream s)
		: this(s, false) { }
	
	/// <summary>
	/// Parses an animation file read from the specified stream
	/// </summary>
	/// <param name="s">stream to load the animation from</param>
	/// <param name="extraData">
	/// pass true to output debugging information to the console during file
	/// parsing
	/// </param>
	public Animation(Stream s, bool extraData)
	{
		ExtraData = extraData;
		Create(s);
	}
	
	/// <summary>
	/// Decodes an ani file into an Animation object
	/// </summary>
	/// <param name="s">source stream that delivers the animation file</param>
	private void Create(Stream s)
	{
		BinaryReader r = new BinaryReader(s, System.Text.Encoding.ASCII);
		
		//
		// Read the ANI header
		//
		
		string fileSignature = new string(r.ReadChars(10));
		if (fileSignature != "CHFILEANI ")
			throw new FormatException("Invalid ANI file signature: " + fileSignature);
		
		uint fileLength = r.ReadUInt32();
		
		int fileId = r.ReadUInt16();
		
		if (fileId != 0)
			OutputInfo("FileId: " + fileId);
		
		int nHeads = 0,
			nPals = 0,
			nTPals = 0,
			nCBoxes = 0;
		
		long bytesRead = 0;
		
		//
		// go through all items in the file
		//
		while (bytesRead < fileLength)
		{
			if (bytesRead + 10 > fileLength)
				throw new FormatException("Item header too short (" + (fileLength - bytesRead) + ")");
			
			string itemSignature = new string(r.ReadChars(4)); 
			uint itemLength = r.ReadUInt32();
			ushort itemId = r.ReadUInt16();
			long itemStart = (bytesRead += 10);
			long itemEnd = itemStart + itemLength;
			long a; // random var for temporary readings
			
			if (itemEnd > fileLength)
				throw new FormatException("Item " + itemSignature + " (" + itemId + ") of length " + itemLength
							+ " does not fit in file with " + (fileLength - bytesRead) + " bytes left.");
			
			// each item type is responsible for reading exactly itemLength
			// bytes and incrementing bytesRead by itemLength
			switch (itemSignature)
			{
				case "HEAD":
				{
					++nHeads;
					if (nHeads > 1)
						OutputInfo("Multiple heads detected o_O");
					
					if (itemId != 1)
						OutputInfo("Head id: " + itemId);
					
					if (itemLength != 48)
						OutputInfo("Head length: " + itemLength);
					
					if ((a = r.ReadUInt32()) != 0x00010000 && a != 0x00010001)
						OutputInfo("1st dword in head is {0,8:X}", a);
					
					if ((a = r.ReadUInt32()) != 0x00010064 && a != 0x00000064 && a != 0x0001001E)
						OutputInfo("2nd dword in head is {0,8:X}", a);
					
					if ((a = r.ReadUInt32()) != 0x00000001)
						OutputInfo("3nd dword in head is {0,8:X}", a);
					
					for (bytesRead += 12; bytesRead < itemEnd; ++bytesRead)
						if ((a = r.ReadByte()) != 0)
							OutputInfo("Extra data in head: " + a);
					
					break;
				}
				case "FRAM":
					bytesRead = ParseFrame(s, r, bytesRead, itemId, itemLength, itemStart, itemEnd);
					
					break;
				case "SEQ ":
					bytesRead = ParseSequence(s, r, bytesRead, itemId, itemLength, itemStart, itemEnd);
					
					break;
				case "PAL ":
#if DEBUG
				{
					++nPals;
					if (nPals > 1)
						OutputInfo("Multiple Pals detected");
					
					if (itemId != 1)
						OutputInfo("Pal id: " + itemId);
					
					if (itemLength != 8192)
						OutputInfo("Pal length: " + itemLength);
					
					s.Seek(itemLength, SeekOrigin.Current);
					bytesRead += itemLength;
					break;
				}
#endif
				case "TPAL":
#if DEBUG
				{
					++nTPals;
					if (nTPals > 1)
						OutputInfo("Multiple T-Pals detected");
					
					if (itemId != 1)
						OutputInfo("TPal id: " + itemId);
					
					if (itemLength != 1028)
						OutputInfo("TPal length: " + itemLength);
					
					s.Seek(itemLength, SeekOrigin.Current);
					bytesRead += itemLength;
					break;
				}
#endif
				case "CBOX":
#if DEBUG
				{
					++nCBoxes;
					if (nCBoxes > 1)
						OutputInfo("Multiple CBoxes detected");
					
					if (itemId != 1)
						OutputInfo("CBox id: " + itemId);
					
					if (itemLength != 4)
						OutputInfo("CBox length: " + itemLength);
					
					if (itemLength < 4)
						throw new FormatException("CBox item too short (" + (fileLength - bytesRead) + ")");
					
					if ((a = r.ReadUInt16()) != 16)
						OutputInfo("CBox X: " + a);
					
					if ((a = r.ReadUInt16()) != 16)
						OutputInfo("CBox Y: " + a);
					
					s.Seek(itemLength - 4, SeekOrigin.Current);
					
					bytesRead += itemLength;
					break;
				}
#else
					// known but ignored
					s.Seek(itemLength, SeekOrigin.Current);
					bytesRead += itemLength;
					break;
#endif	
				default:
					OutputInfo("Unknown item " + itemSignature);
					s.Seek(itemLength, SeekOrigin.Current);
					bytesRead += itemLength;
					break;
			}
		}
		
		if (s.Position != s.Length)
			OutputInfo("Extra data in file");
		
		if (ExtraData)
		{
			Console.WriteLine("-- Summary --");
			Console.WriteLine("{0} Head(s), {1} Palettes, {2} T-Palettes, {3} CBoxes found",
								nHeads, nPals, nTPals, nCBoxes);
			Console.WriteLine("{0} Frame(s) found", Frames.Count);
			for (int i = 0; i < Frames.Count; ++i)
			{
				Console.WriteLine("\tFrame{0}:", i);
				Console.WriteLine("\t\tFilename: " + Frames[i].FileName);
				Console.WriteLine("\t\tDimensions: {0}x{1}", Frames[i].BitmapBuilder.Width, Frames[i].BitmapBuilder.Height);
				Console.WriteLine("\t\tHot spot: ({0}, {1})", Frames[i].HotSpotX, Frames[i].HotSpotY);
				Console.WriteLine("\t\tKey Color: {0} (raw: 0x{1:X})", Frames[i].KeyColor, Frames[i].RawKeyColor);
			}
			Console.WriteLine("{0} Sequence(s) found", Sequences.Count);
			for (int i = 0; i < Sequences.Count; ++i)
			{
				Sequence seq = Sequences[i];
				Console.WriteLine("\tSequence{0}:", i);
				Console.WriteLine("\t\tName: " + seq.Name);
				Console.WriteLine("\t\tNumber of states: " + seq.States.Length);
				for (int j = 0; j < seq.States.Length; ++j)
				{
					SeqState stat = seq.States[j];
					
					Console.WriteLine("\t\tState{0}:", j);
					Console.WriteLine("\t\t\tFrame: {0}", stat.FileName ?? "(None)");
					//Console.WriteLine("\t\t\tNew Speed: {0}", (stat.NewSpeed == -1) ? "--" : stat.NewSpeed.ToString());
					Console.WriteLine("\t\t\tFrame offset: ({0}, {1})", stat.FrameX, stat.FrameY);
				}
			}
		}
	}
	
	// HACKHACK/TRYTRY itemStart should be equal to bytesRead?!
	/// <summary>
	/// parse an ani file FRAM item
	/// </summary>
	/// <param name="s">the stream to read from. must be skippable</param>
	/// <param name="r">a BinaryReader for s</param>
	/// <param name="bytesRead">current position in the file</param>
	/// <param name="itemId">item id read from item header</param>
	/// <param name="itemLength">item length</param>
	/// <param name="itemStart">item start position in the file</param>
	/// <param name="itemEnd">item end position</param>
	/// <returns>the new position in the file</returns>
	private long ParseFrame(Stream s, BinaryReader r, long bytesRead, ushort itemId, uint itemLength, long itemStart, long itemEnd)
	{
		long a; // random var for temporary readings
		int iFrame = Frames.Count;
		Frame frame;
		Frames.Add(frame = new Frame());
		
		if (itemId != 0)
			OutputInfo("Frame id: " + itemId);
		
		int nFrameHeads = 0,
			nFrameFNames = 0,
			nFrameCImages = 0;
		
		while (bytesRead < itemEnd)
		{
			if (bytesRead + 10 > itemEnd)
			{
				OutputInfo("bytesRead: {0}, stream is at {1}", bytesRead, s.Position);
				throw new FormatException("Frame Item header too short (" + (itemEnd - bytesRead) + ")");
			}
			
			string frameItemSignature = new string(r.ReadChars(4)); 
			uint frameItemLength = r.ReadUInt32();
			ushort frameItemId = r.ReadUInt16();
			long frameItemStart = (bytesRead += 10);
			long frameItemEnd = frameItemStart + frameItemLength;
			
			if (bytesRead + frameItemLength > itemEnd)
				throw new FormatException("Frame Item " + frameItemSignature
							+ " (" + frameItemId + ") of length " + frameItemLength
							+ " does not fit in frame with " + (itemEnd - bytesRead) + " bytes left");
			
			switch (frameItemSignature)
			{
				case "HEAD":
				{
					++nFrameHeads;
					if (nFrameHeads > 1)
						OutputInfo("Multiple frame{0} heads detected", iFrame);
					
					if (frameItemId != 1)
						OutputInfo("Frame{0} head id: " + frameItemId, iFrame);
					
					if (frameItemLength != 2)
					{
						OutputInfo("Frame{0} head size: " + frameItemLength, iFrame);
						s.Seek(frameItemLength, SeekOrigin.Current);
					}
					else
					{
						if ((a = r.ReadUInt16()) != 0x6403 && a != 0x0F03 && a != 0x1E03)
							OutputInfo("Frame{0} head data: " + a, iFrame);
					}
					
					bytesRead += frameItemLength;
					break;
				}
				case "FNAM":
				{
					++nFrameFNames;
					if (nFrameFNames > 1)
						OutputInfo("Multiple frame{0} filenames detected, previous was " + frame.FileName, iFrame);
					
					if (frameItemId != 1)
						OutputInfo("Frame{0} filename id: " + frameItemId, iFrame);
					
					if (frameItemLength == 0)
						throw new FormatException("Zero length frame" + iFrame + " filename");
					
					frame.FileName = new String(r.ReadChars((int)frameItemLength - 1));
					
					if ((a = r.ReadByte()) != 0)
						OutputInfo("Last frame{0} filename char: " + a, iFrame);
					
					bytesRead += frameItemLength;
					break;
				}
				case "CIMG":
					++nFrameCImages;
					if (nFrameCImages > 1)
						OutputInfo("Multiple frame{0} images detected", iFrame);
					
					bytesRead = ParseFrameCImage(s, r, bytesRead, iFrame, frame, frameItemId, frameItemLength, frameItemStart, frameItemEnd);
					
					break;
				case "ATBL":
					// known but ignored
					s.Seek(frameItemLength, SeekOrigin.Current);
					bytesRead += frameItemLength;
					break;
				default:
					OutputInfo("Unknown frame{0} item " + frameItemSignature, iFrame);
					s.Seek(frameItemLength, SeekOrigin.Current);
					bytesRead += frameItemLength;
					break;
			}
		}
		
		if (nFrameHeads == 0)
			OutputInfo("Frame{0} has no head", iFrame);
		if (nFrameFNames == 0)
			OutputInfo("Frame{0} has no filename", iFrame);
		if (nFrameCImages == 0)
			OutputInfo("Frame{0} has no c-image", iFrame);
		
		return bytesRead;
	}
	
	
	// HACKHACK/TRYTRY frameItemStart should be equal to bytesRead?!
	/// <summary>
	/// parse an ani file "CIMG" item (inside "FRAM")
	/// </summary>
	/// <param name="s">the stream to read from. must be skippable</param>
	/// <param name="r">a BinaryReader for s</param>
	/// <param name="bytesRead">current position in the file</param>
	/// <param name="iFrame">current sequence number for debug output</param>
	/// <param name="frame">current frame</param>
	/// <param name="frameItemId">item id read from item header</param>
	/// <param name="frameItemLength">item length</param>
	/// <param name="frameItemStart">item start position in the file</param>
	/// <param name="frameItemEnd">item end position</param>
	/// <returns>the new position in the file</returns>
	private long ParseFrameCImage(Stream s, BinaryReader r, long bytesRead, int iFrame, Frame frame, ushort frameItemId, uint frameItemLength, long frameItemStart, long frameItemEnd)
	{
		long a;
		if (frameItemId != 1)
			OutputInfo("Frame{0} CImage id: " + frameItemId, iFrame);
		
		if (frameItemLength < 32)
			throw new FormatException("CImage too short (" + (frameItemEnd - bytesRead) + ")");
		
		ushort imageType = r.ReadUInt16();
		
		ushort imageUnknown1 = r.ReadUInt16();
		
		if (imageUnknown1 != 4 && imageUnknown1 != 0)
			OutputInfo("CImage{0} header unknown1: " + imageUnknown1, iFrame);
		
		uint imageAdditionalSize = r.ReadUInt32();
		
		bytesRead += 8;
		
		if (imageAdditionalSize < 24)
			throw new FormatException("CImage" + iFrame + " header of size " + imageAdditionalSize + " can't contain any valid information");
		
		if (imageAdditionalSize > frameItemEnd - bytesRead)
			throw new FormatException("CImage" + iFrame + " too short (" + (frameItemEnd - bytesRead) + ") for header");
		
		bool imagePaletteHeader = false;
		ushort imagePaletteSize = 0;
		
		if (imageAdditionalSize >= 32)
			imagePaletteHeader = true;
		
		if (imageAdditionalSize > 32)
			imagePaletteSize = (ushort)(imageAdditionalSize - 32);
		
		if (imageAdditionalSize == 32)
			OutputInfo("CImage{0} has a palette header but no palette o_O", iFrame);
		
		//
		// "following header" (16 bytes)
		//
		uint imageUnknown2 = r.ReadUInt32();
		
		if (!imagePaletteHeader && imageUnknown2 != 0 || imagePaletteHeader && imageUnknown2 != 24)
			OutputInfo("CImage{0} header unknown2: " + imageUnknown2, iFrame);
		
		ushort bitsPerPixel;
		ushort width = r.ReadUInt16();
		ushort height = r.ReadUInt16();
		frame.HotSpotX = r.ReadUInt16();
		frame.HotSpotY = r.ReadUInt16();
		frame.RawKeyColor = r.ReadUInt16();
		
		if (frame.RawKeyColor == 0xFFFF)
		{
			frame.KeyColor = Color.Transparent;
		}
		else
		{
			if (frame.RawKeyColor >> 15 == 1)
				OutputInfo("CImage{0} key color has bit 15 set, but is not white",
							iFrame);
			
			int R = (frame.RawKeyColor >> 10) << 3,
				G = ((frame.RawKeyColor >> 5) & 0x1F) << 3,
				B = (frame.RawKeyColor & 0x1F) << 3;
			
			R += (int)Math.Ceiling(R * 6.0 / 239.0);
			G += (int)Math.Ceiling(G * 6.0 / 239.0);
			B += (int)Math.Ceiling(B * 6.0 / 239.0);
			
			frame.KeyColor = Color.FromArgb(R, G, B);
		}
		
		ushort imageUnknown3 = r.ReadUInt16();
		
		if (imageUnknown3 != 0)
			OutputInfo("CImage{0} header unknown3: " + imageUnknown3, iFrame);
		
		bytesRead += 16;
		
		//
		// calculate image information from header data
		//
		switch (imageType)
		{
			case 0x04:
				bitsPerPixel = 16;
				
				// for some reason, type 4 images can have a palette
				/*if (imagePaletteSize > 0)
					OutputInfo("CImage{0} is type 4 but has a palette of "
												+ imagePaletteSize + " bytes", iFrame);*/
				
				break;
			case 0x05:
				bitsPerPixel = 24;
				if (imagePaletteSize > 0)
					throw new FormatException("CImage" + iFrame + " is type 5 but has a palette of "
													+ imagePaletteSize + " bytes");
				break;
			case 0x0A:
				bitsPerPixel = 4;
				
				if (imagePaletteSize != 64)
					throw new FormatException("CImage" + iFrame + " is type 10 but has a palette of "
													+ imagePaletteSize + " bytes");
				
				break;
			case 0x0B:
				bitsPerPixel = 8;
				
				if (imagePaletteSize != 1024)
					throw new FormatException("CImage" + iFrame + " is type 10 but has a palette of "
													+ imagePaletteSize + " bytes");
				
				break;
			default:
				throw new FormatException("CImage" + iFrame + " has unknown image type");
		}
		
		//
		// Construct the bitmap
		//
		BitmapBuilder b = new BitmapBuilder(bitsPerPixel, width, height);
		
		//
		// read optional palette header (8 bytes)
		//
		uint imageUnknownP1, imageUnknownP2;
		
		if (imagePaletteHeader)
		{
			if ((imageUnknownP1 = r.ReadUInt32()) != 0x1000000 && imageUnknownP1 != 0x100000)
				OutputInfo("CImage{0} header unknownP1: " + imageUnknownP1, iFrame);
			
			if ((imageUnknownP2 = r.ReadUInt32()) != 8)
				OutputInfo("CImage{0} header unknownP2: " + imageUnknownP2, iFrame);
			
			bytesRead += 8;
		}
		
		//
		// read palette data, which is in bitmap format
		//
		if (imagePaletteSize > 0)
		{
			b.ReadPaletteFromBitmap(s);
			
			bytesRead += b.PaletteSize;
			
			if (imagePaletteSize > b.PaletteSize)
			{
				s.Seek(imagePaletteSize - b.PaletteSize, SeekOrigin.Current);
				bytesRead += imagePaletteSize - b.PaletteSize;
			}
		}
		
		//
		// additional header
		//
		ushort imageUnknownA1 = r.ReadUInt16();
		
		if (imageUnknownA1 != 0x10 && imageUnknownA1 != 0x11 && imageUnknownA1 != 0x12)
			OutputInfo("CImage{0} header unknownA1: " + imageUnknownA1, iFrame);
		
		ushort imageUnknownA2 = r.ReadUInt16();
		
		if (imageUnknownA2 != 12)
			OutputInfo("CImage{0} header unknownA2: " + imageUnknownA2, iFrame);
		
		long imageCompressedSize = r.ReadUInt32() - 12,
			imageUncompressedSize = r.ReadUInt32();
		
		// check if size matches
		uint bytesRequired = (uint)Math.Ceiling(bitsPerPixel * width / 8.0) * height;
		
		if (imageUncompressedSize != 0xFF000000 && imageUncompressedSize != bytesRequired)
			OutputInfo("CImage{0} uncompressed size is {1}, expected {2}", iFrame, imageUncompressedSize, bytesRequired);
		
		bytesRead += 12;
		
		//
		// Run-Length-Decode the image data and insert as bitmap data
		// this is tga-type RLE
		//
		long imageDataBytes = b.ReadDataFromAni(r, (ulong)(frameItemEnd - bytesRead));
		
		bytesRead += Math.Abs(imageDataBytes);
		
		if (imageDataBytes < 0)
			throw new FormatException("CImage" + iFrame + " data too short. Only " + (frameItemEnd - bytesRead) + " bytes left");
		
		
		if (bytesRead + 1 > frameItemEnd)
			OutputInfo("No terminator in image{0}:", iFrame);
		else if (bytesRead + 1 < frameItemEnd)
		{
			OutputInfo("Extra data in image{0}: {1} and {2} more bytes", iFrame, r.ReadByte(), frameItemEnd - bytesRead - 1);
			s.Seek(frameItemEnd - bytesRead - 1, SeekOrigin.Current);
			bytesRead = frameItemEnd;
		}
		else if ((a = r.ReadByte()) != 0xFF)
		{
			OutputInfo("Unusual terminator in image{0}: " + r.ReadByte(), iFrame);
			bytesRead += 2;
		}
		else
			++bytesRead;
		
		//
		// image creation is done!
		//
		//frame.Width = width;
		//frame.Height = height;
		
		// crop and save
		uint dLeft, dTop;
		frame.BitmapBuilder = b.Crop(frame.KeyColor, frame.RawKeyColor, out dLeft, out dTop);
		frame.HotSpotX -= (ushort)dLeft;
		frame.HotSpotY -= (ushort)dTop;
		
		if (ExtraData)
		{
			/*Stream f = File.OpenWrite(@"out\" + frame.FileName + ".bmp");
			int uh;
			while ((uh = frame.ImageStream.ReadByte()) != -1)
				f.WriteByte((byte)uh);
			f.Close();*/
			Image.FromStream(frame.BitmapBuilder.GetStream()).Save(@"out\" + frame.FileName + ".png", ImageFormat.Png);
		}
		
		return bytesRead;
	}
	
	// HACKHACK/TRYTRY itemStart should be equal to bytesRead?!
	/// <summary>
	/// parse an ani file "SEQ " item
	/// </summary>
	/// <param name="s">the stream to read from. must be skippable</param>
	/// <param name="r">a BinaryReader for s</param>
	/// <param name="bytesRead">current position in the file</param>
	/// <param name="itemId">item id read from item header</param>
	/// <param name="itemLength">item length</param>
	/// <param name="itemStart">item start position in the file</param>
	/// <param name="itemEnd">item end position</param>
	/// <returns>the new position in the file</returns>
	private long ParseSequence(Stream s, BinaryReader r, long bytesRead, ushort itemId, uint itemLength, long itemStart, long itemEnd)
	{
		int iSeq = Sequences.Count;
		Sequence seq;
		Sequences.Add(seq = new Sequence());
		List<SeqState> states = new List<SeqState>();
		
		if (itemId != 0)
			OutputInfo("Seq{0} id: " + itemId, iSeq);
		
		int nSeqHeads = 0;
		
		while (bytesRead < itemEnd)
		{
			if (bytesRead + 10 > itemEnd)
				throw new FormatException("Seq" + iSeq + " Item header too short (" + (itemEnd - bytesRead) + ")");
			
			string seqItemSignature = new string(r.ReadChars(4)); 
			uint seqItemLength = r.ReadUInt32();
			ushort seqItemId = r.ReadUInt16();
			long seqItemStart = (bytesRead += 10);
			long seqItemEnd = seqItemStart + seqItemLength;
			
			if (bytesRead + seqItemLength > itemEnd)
				throw new FormatException("Seq" + iSeq + " Item " + seqItemSignature
							+ " (" + seqItemId + ") of length " + seqItemLength
							+ " does not fit in sequence with " + (itemEnd - bytesRead) + " bytes left");
			
			switch (seqItemSignature)
			{
				case "HEAD":
				{
					++nSeqHeads;
					
					if (nSeqHeads > 1)
						OutputInfo("Multiple seq{0} heads ^^", iSeq);
					
					if (seqItemId != 1)
						OutputInfo("Seq{0} head id: " + seqItemId, iSeq);
					
					if (seqItemLength != 96)
						OutputInfo("Seq{0} head size: " + seqItemLength, iSeq);
					
					seq.Name = String.Empty;
					
					while (bytesRead++ != seqItemEnd)
					{
						char c = r.ReadChar();
						
						if (c == '\0')
							break;
						
						seq.Name += c;
					}
					
					if (bytesRead > seqItemEnd)
						throw new FormatException("Seq" + iSeq + " Head name is not terminated");
					
					
					s.Seek(seqItemEnd - bytesRead, SeekOrigin.Current);
					bytesRead = seqItemEnd;
					break;
				}
				case "STAT":
				{
					int iStat = states.Count;
					SeqState stat;
					states.Add(stat = new SeqState());
					bytesRead = ParseSequenceState(s, r, bytesRead, iSeq, iStat, stat, seqItemId, seqItemLength, seqItemStart, seqItemEnd);
					
					break;
				}
				default:
					OutputInfo("Unknown seq{0} item " + seqItemSignature, iSeq);
					s.Seek(seqItemLength, SeekOrigin.Current);
					bytesRead += seqItemLength;
					break;
			}
		}
		
		seq.States = states.ToArray();
		
		return bytesRead;
	}
	
	
	// HACKHACK/TRYTRY seqItemStart should be equal to bytesRead?!
	/// <summary>
	/// parse an ani file "STAT" item (inside "SEQ ")
	/// </summary>
	/// <param name="s">the stream to read from. must be skippable</param>
	/// <param name="r">a BinaryReader for s</param>
	/// <param name="bytesRead">current position in the file</param>
	/// <param name="iSeq">current sequence number for debug output</param>
	/// <param name="iStat">current state number for debug output</param>
	/// <param name="stat">current state</param>
	/// <param name="seqItemId">item id read from item header</param>
	/// <param name="seqItemLength">item length</param>
	/// <param name="seqItemStart">item start position in the file</param>
	/// <param name="seqItemEnd">item end position</param>
	/// <returns>the new position in the file</returns>
	private long ParseSequenceState(Stream s, BinaryReader r, long bytesRead, int iSeq, int iStat, SeqState stat, ushort seqItemId, uint seqItemLength, long seqItemStart, long seqItemEnd)
	{
		long a;
		
		if (seqItemId != 0)
			OutputInfo("Seq{0}State{1} id: " + seqItemId, iSeq, iStat);
		
		int nStatHeads = 0,
			nStatFrames = 0;
		
		while (bytesRead < seqItemEnd)
		{
			if (bytesRead + 10 > seqItemEnd)
				throw new FormatException("Seq" + iSeq + "State" + iStat + " Item header too short (" + (seqItemEnd - bytesRead) + ")");
			
			string statItemSignature = new string(r.ReadChars(4)); 
			uint statItemLength = r.ReadUInt32();
			int statItemId = r.ReadUInt16();
			long statItemStart = (bytesRead += 10);
			long statItemEnd = statItemStart + statItemLength;
			
			if (bytesRead + statItemLength > seqItemEnd)
				throw new FormatException("Seq" + iSeq + "State" + iStat + " Item " + statItemSignature
							+ " (" + statItemId + ") of length " + statItemLength
							+ " does not fit in state with " + (seqItemEnd - bytesRead) + " bytes left");
			
			switch (statItemSignature)
			{
				case "HEAD":
				{
					++nStatHeads;
					
					if (nStatHeads > 1)
						OutputInfo("Multiple seq{0}state{1} heads", iSeq, iStat);
					
					if (statItemId != 1)
						OutputInfo("Seq{0}State{1} head id: " + statItemId, iSeq, iStat);
					
					if (statItemLength != 46)
					{
						OutputInfo("Seq{0}State{1} head size: " + statItemLength, iSeq, iStat);
						s.Seek(statItemLength, SeekOrigin.Current);
					}
					else
					{
						// this is some additional info stored with each state
						/*stat.NewSpeed = r.ReadInt16();
						stat.MoveX = r.ReadInt16();
						stat.MoveY = r.ReadInt16();
						for (int i = 0; i < 16; ++i)
							stat.StateValues[i] = r.ReadInt16();
						
						// now the bitmask
						a = r.ReadUInt16();
						
						for (int i = 0; i < 16; ++i)
							if ((a & (2 << i)) == 0)
								stat.StateValues[i] = int.MaxValue;
						*/
						// we don't need the above info, so just skip it
						s.Seek(40, SeekOrigin.Current);
						
						// now there should be 3 zero-words
						if ((a = r.ReadUInt16()) != 0)
							OutputInfo("Seq{0}State{1} head word 1: " + a, iSeq, iStat);
						if ((a = r.ReadUInt16()) != 0)
							OutputInfo("Seq{0}State{1} head word 2: " + a, iSeq, iStat);
						if ((a = r.ReadUInt16()) != 0)
							OutputInfo("Seq{0}State{1} head word 3: " + a, iSeq, iStat);
					}
					
					bytesRead += statItemLength;
					break;
				}
				case "FRAM":
				{
					++nStatFrames;
					
					if (nStatFrames > 1)
						OutputInfo("Multiple seq{0}state{1} frames o_O", iSeq, iStat);
					
					if (statItemId != 1)
						OutputInfo("Seq{0}State{1} frame id: " + statItemId, iSeq, iStat);
					
					if (statItemLength < 2)
						throw new FormatException("Seq" + iSeq + "State" + iStat + " frame too short");
					
					int nFramesThisItem = r.ReadUInt16();
					bytesRead += 2;
					
					if (statItemEnd - bytesRead < nFramesThisItem * 10)
						throw new FormatException("Seq" + iSeq + "State" + iStat + " frame too short for " + nFramesThisItem + " frames");
					
					for (int i = 0; i < nFramesThisItem; ++i)
					{
						// frame number
						a = r.ReadUInt16();
						if (a < Frames.Count)
							stat.Frame = Frames[(int)a];
						else
							throw new FormatException("Seq" + iSeq + "State" + iStat + "Frame" + i + " tells us to use frame " + a + ", which doesn't exist");
						
						stat.FrameX = r.ReadInt16();
						stat.FrameY = r.ReadInt16();
						
						if ((a = r.ReadUInt32()) != 0)
							OutputInfo("Seq{0}State{1}Frame{2} end dword: " + a, iSeq, iStat, i);
						bytesRead += 10;
					}
					
					if (bytesRead < statItemEnd)
					{
						OutputInfo("Seq{0}State{1} has {2} bytes of extra data", iSeq, iStat,
											statItemEnd - bytesRead);
						s.Seek(statItemEnd - bytesRead, SeekOrigin.Current);
						bytesRead = statItemEnd;
					}
					
					break;
				}
				case "RECT":
				case "POIN":
					// known but ignored
					s.Seek(statItemLength, SeekOrigin.Current);
					bytesRead += statItemLength;
					break;
				default:
					OutputInfo("Unknown seq{0}stat{1} item " + statItemSignature, iSeq, iStat);
					s.Seek(statItemLength, SeekOrigin.Current);
					bytesRead += statItemLength;
					break;
			}
		}
		
		return bytesRead;
	}
	
	
	//
	// OutputInfo
	//
	[System.Diagnostics.Conditional("DEBUG")]
	private static void OutputInfo()
	{
		Console.WriteLine();
	}
	[System.Diagnostics.Conditional("DEBUG")]
	private static void OutputInfo(string info)
	{
		Console.WriteLine(info);
	}
	[System.Diagnostics.Conditional("DEBUG")]
	private static void OutputInfo(string format, params object[] args)
	{
		Console.WriteLine(format, args);
	}
	
	// a test program for animation parsing
#if false
	private const string Path = @"E:\Install\Atomic Bomberman\DATA\ANI\";
	private static void Main(String[] args)
	{
		if (args.Length == 0)
		{
			string[] ignoreFiles = new string[] { "classics", "mflame", "unnamed", "unnamed2", "unnamed3", "tilesx", "bfontx", "bfonty", };
			foreach (string filename in Directory.GetFiles(Path, "*.ani"))
			{
				bool next = false;
				
				//Console.WriteLine("----------------------{0}--------------", filename);
				foreach (string s in ignoreFiles)
					if (String.Compare(filename, Path + s + ".ani", true) == 0)
					{
						//Console.WriteLine("--ignored");
						next = true;
						break;
					}
				
				if (next)
					continue;
				
				Animation ani = new Animation(filename);
			}
		}
		else
		{
			Animation ani = new Animation(Path + args[0] + ".ani", true);
		}
	}
#endif
}
}