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

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Threading;

using BomberStuff.Core;
using BomberStuff.Core.Animation;
using BomberStuff.Core.Utilities;

namespace BomberStuff.Files
{
	/// <summary>
	/// Represents an Atomic Bomberman .ANI animation file
	/// </summary>
	public class AniFile
	{
		/// <summary>A list of the image frames in the animation</summary>
		public List<AnimationFrame> Frames = new List<AnimationFrame>();
		/// <summary>A list of the animation sequences in the file</summary>
		public List<Animation> Sequences = new List<Animation>();
		private bool ExtraData;

		/// <summary>
		/// Parses the specified AB animation file
		/// </summary>
		/// <param name="filename">Path to the .ani file</param>
		public AniFile(string filename)
			: this(filename, false) { }

		/// <summary>
		/// Parses the specified AB animation file
		/// </summary>
		/// <param name="filename">Path to the .ani file</param>
		/// <param name="extraData">
		/// A boolean value specifying whether to output extra (debug)
		/// information to the console
		/// </param>
		public AniFile(string filename, bool extraData)
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
		public AniFile(Stream s)
			: this(s, false) { }

		/// <summary>
		/// Parses an animation file read from the specified stream
		/// </summary>
		/// <param name="s">stream to load the animation from</param>
		/// <param name="extraData">
		/// pass true to output debugging information to the console during file
		/// parsing
		/// </param>
		public AniFile(Stream s, bool extraData)
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

				System.Diagnostics.Debug.Assert(bytesRead == itemEnd, "Ani File Item Handler bug",
										"Wrong number of bytes read for item " + itemSignature);
			}

			if (s.Position != s.Length)
				OutputInfo("File is longer than fileLength specifies. " + (s.Length - s.Position) + " bytes of additional data found.");

			if (ExtraData)
			{
				Console.WriteLine("-- Summary --");
				Console.WriteLine("{0} Head(s), {1} Palettes, {2} T-Palettes, {3} CBoxes found",
									nHeads, nPals, nTPals, nCBoxes);
				Console.WriteLine("{0} Frame(s) found", Frames.Count);
				for (int i = 0; i < Frames.Count; ++i)
				{
					Console.WriteLine("\tFrame{0}:", i);
#if DEBUG
					Console.WriteLine("\t\tFilename: " + Frames[i].FileName);
#endif
					Console.WriteLine("\t\tDimensions: {0}x{1}", Frames[i].BitmapBuilder.Width, Frames[i].BitmapBuilder.Height);
					Console.WriteLine("\t\tHot spot: ({0}, {1})", Frames[i].Offset.Width, Frames[i].Offset.Height);
					Console.WriteLine("\t\tKey Color: {0} (raw: 0x{1:X})", Frames[i].KeyColor, Frames[i].RawKeyColor);
				}
				Console.WriteLine("{0} Sequence(s) found", Sequences.Count);
				for (int i = 0; i < Sequences.Count; ++i)
				{
					Animation seq = Sequences[i];
					Console.WriteLine("\tSequence{0}:", i);
#if DEBUG
					Console.WriteLine("\t\tName: " + seq.Name);
#endif
					Console.WriteLine("\t\tNumber of states: " + seq.Frames.Length);
					for (int j = 0; j < seq.Frames.Length; ++j)
					{
						//AnimationState stat = seq.States[j];

						Console.WriteLine("\t\tState{0}:", j);
#if DEBUG
						Console.WriteLine("\t\t\tFrame: {0}", seq.Frames[j].FileName ?? "(None)");
#endif
						//Console.WriteLine("\t\t\tNew Speed: {0}", (stat.NewSpeed == -1) ? "--" : stat.NewSpeed.ToString());
						//Console.WriteLine("\t\t\tFrame offset: ({0}, {1})",
						//						seq.FrameOffset[j].Width,
						//						seq.FrameOffset[j].Height);
						Console.WriteLine("\t\t\tRelative frame offset: ({0}, {1})", seq.FrameOffset[j].Width, seq.FrameOffset[j].Height);
					}
				}
			}
		}

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
			System.Diagnostics.Debug.Assert(itemStart == bytesRead);
			long a; // random var for temporary readings
			int iFrame = Frames.Count;
			AnimationFrame frame;
			// HACKHACK: we should first collect all the data, then create a frame out of it
			Frames.Add(frame = new AnimationFrame());

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
				System.Diagnostics.Debug.Assert(bytesRead == frameItemEnd);
			}

			if (nFrameHeads == 0)
				OutputInfo("Frame{0} has no head", iFrame);
			if (nFrameFNames == 0)
				OutputInfo("Frame{0} has no filename", iFrame);
			if (nFrameCImages == 0)
				OutputInfo("Frame{0} has no c-image", iFrame);

			return bytesRead;
		}


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
		private long ParseFrameCImage(Stream s, BinaryReader r, long bytesRead, int iFrame, AnimationFrame frame, ushort frameItemId, uint frameItemLength, long frameItemStart, long frameItemEnd)
		{
			System.Diagnostics.Debug.Assert(frameItemStart == bytesRead);
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
			ushort hotSpotX = r.ReadUInt16();
			ushort hotSpotY = r.ReadUInt16();
			frame.Offset = new BomberStuff.Core.Drawing.SizeF(hotSpotX, hotSpotY);
			frame.RawKeyColor = r.ReadUInt16();

			if (frame.RawKeyColor == 0xFFFF)
			{
				frame.KeyColor = Color.Transparent;
			}
			else
			{
				// no idea what bit 15 means
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
						throw new FormatException("CImage" + iFrame + " is type 11 but has a palette of "
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

			// crop and save
			uint dLeft, dTop;
			frame.BitmapBuilder = b.Crop(frame.KeyColor, frame.RawKeyColor, out dLeft, out dTop);
			frame.Offset.Width -= dLeft;
			frame.Offset.Height -= dTop;

			// normalize size and offset to field width/height
			// HACKHACK/TRYTRY: I believe this is the only location where those numbers
			// (40 = field width in pixels, 36 = field height in pixels) are ever
			// needed. If not, put them somewhere sensible as constants
			frame.Size = new BomberStuff.Core.Drawing.SizeF(frame.BitmapBuilder.Width / 40.0f,
															frame.BitmapBuilder.Height / 36.0f);

			frame.Offset.Width /= 40.0f;
			frame.Offset.Height /= 36.0f;

			// save the image, if we're into that
			if (ExtraData)
			{
				/*Stream f = File.OpenWrite(@"out\" + frame.FileName + ".bmp");
				int uh;
				while ((uh = frame.ImageStream.ReadByte()) != -1)
					f.WriteByte((byte)uh);
				f.Close();*/
				new Bitmap(frame.BitmapBuilder.GetStream()).Save(@"out\" + frame.FileName + ".png", ImageFormat.Png);
			}

			return bytesRead;
		}

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
			System.Diagnostics.Debug.Assert(itemStart == bytesRead);
			int iSeq = Sequences.Count;
			Animation seq = new Animation();
			Sequences.Add(seq);
			//List<AnimationState> states = new List<AnimationState>();
			List<ushort> frames = new List<ushort>();
			List<short> framesX = new List<short>();
			List<short> framesY = new List<short>();

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
							int iStat = frames.Count;
							//AnimationState stat;
							//states.Add(stat = new AnimationState());
							ushort frame;
							short frameX, frameY;
							bytesRead = ParseSequenceState(s, r, bytesRead, iSeq, iStat, out frame, out frameX, out frameY, seqItemId, seqItemLength, seqItemStart, seqItemEnd);
							frames.Add(frame);
							framesX.Add(frameX);
							framesY.Add(frameY);
							break;
						}
					default:
						OutputInfo("Unknown seq{0} item " + seqItemSignature, iSeq);
						s.Seek(seqItemLength, SeekOrigin.Current);
						bytesRead += seqItemLength;
						break;
				}
				System.Diagnostics.Debug.Assert(bytesRead == seqItemEnd);
			}

			//seq.States = states.ToArray();
			seq.Frames = new AnimationFrame[frames.Count];
			seq.FrameOffset = new BomberStuff.Core.Drawing.SizeF[frames.Count];
			for (int i = 0; i < frames.Count; ++i)
			{
				if (frames[i] >= Frames.Count)
					throw new FormatException("Seq" + iSeq + "State" + i + "Frame" + i + " tells us to use frame " + frames[i] + ", which doesn't exist");
				
				seq.Frames[i] = Frames[(int)frames[i]];
				// HACKHACK: field width/height in pixels. See other comment
				seq.FrameOffset[i] = new BomberStuff.Core.Drawing.SizeF(framesX[i] / 40.0f, framesY[i] / 36.0f);
			}

			return bytesRead;
		}


		/// <summary>
		/// parse an ani file "STAT" item (inside "SEQ ")
		/// </summary>
		/// <param name="s">the stream to read from. must be skippable</param>
		/// <param name="r">a BinaryReader for s</param>
		/// <param name="bytesRead">current position in the file</param>
		/// <param name="iSeq">current sequence number for debug output</param>
		/// <param name="iStat">current state number for debug output</param>
		/// <param name="frame">frame number of the state</param>
		/// <param name="frameX">frame x offset</param>
		/// <param name="frameY">frame y offset</param>
		/// <param name="seqItemId">item id read from item header</param>
		/// <param name="seqItemLength">item length</param>
		/// <param name="seqItemStart">item start position in the file</param>
		/// <param name="seqItemEnd">item end position</param>
		/// <returns>the new position in the file</returns>
		private long ParseSequenceState(Stream s, BinaryReader r, long bytesRead, int iSeq, int iStat, /*AnimationState stat,*/out ushort frame, out short frameX, out short frameY, ushort seqItemId, uint seqItemLength, long seqItemStart, long seqItemEnd)
		{
			System.Diagnostics.Debug.Assert(seqItemStart == bytesRead);
			long a;

			if (seqItemId != 0)
				OutputInfo("Seq{0}State{1} id: " + seqItemId, iSeq, iStat);

			int nStatHeads = 0,
				nStatFrames = 0;

			// shut up warnings. This is actually unnecessary
			frame = 0; frameX = frameY = 0;

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

							// TRYTRY: why the heck can there be multiple frames in a state?!
							int nFramesThisItem = r.ReadUInt16();
							bytesRead += 2;

							if (nFramesThisItem > 1)
								Console.WriteLine("Multiple frames ({0}) in Seq" + iSeq + "State" + iStat, nFramesThisItem);

							if (statItemEnd - bytesRead < nFramesThisItem * 10)
								throw new FormatException("Seq" + iSeq + "State" + iStat + " frame too short for " + nFramesThisItem + " frames");

							for (int i = 0; i < nFramesThisItem; ++i)
							{
								// frame number
								frame = r.ReadUInt16();

								//stat.FrameX = r.ReadInt16();
								//stat.FrameY = r.ReadInt16();
								frameX = r.ReadInt16();
								frameY = r.ReadInt16();

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

			if (nStatFrames == 0)
				throw new FormatException("Seq" + iSeq + "State" + iStat + " has no frames");

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

	/// <summary>
	/// Reads Atomic Bomberman ANI and ALI files
	/// </summary>
	public static class AniFileReader
	{
		private const bool ExtraData = false;
		/// <summary>
		/// 
		/// </summary>
		/// <param name="aniList"></param>
		/// <param name="filename"></param>
		public static void AddAliFile(AnimationList aniList, string filename)
		{
			bool extraData = ExtraData;
			string path = Path.GetDirectoryName(filename);
			using (FileStream s = File.Open(filename, FileMode.Open, FileAccess.Read, FileShare.Read))
				using (StreamReader r = new StreamReader(s))
				{
					string line;
					while ((line = r.ReadLine()) != null)
					{
						line = line.Trim();
						if (line.StartsWith(";"))
							continue;
						else if (line.StartsWith("-"))
						{
							AniFile aniFile;

							try
							{
								aniFile = new AniFile(path + @"\" + line.Substring(1));
							}
							catch (FileNotFoundException e)
							{
								if (extraData)
									Console.WriteLine("File listed in ALI not found: {0}", e);
								continue;
							}

							foreach (Animation ani in aniFile.Sequences)
							{
								if (!AddSequence(aniList, ani.Name, ani, extraData))
								//Console.WriteLine("Failed adding " + ani.Name + " from " + line.Substring(1));
								{ }
							}
						}
						else if (extraData)
							Console.WriteLine("Invalid line in ALI file: {0}", line);
					}
				}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="aniList"></param>
		/// <param name="filename"></param>
		public static void AddAniFile(AnimationList aniList, string filename)
		{
			bool extraData = ExtraData;
			AniFile aniFile = new AniFile(filename, extraData);

			foreach (Animation ani in aniFile.Sequences)
			{
				AddSequence(aniList, ani.Name, ani, extraData);
			}
		}

		/// <summary>
		/// Adds a Sequence with the specified name
		/// </summary>
		/// <param name="aniList">
		/// animtion list to add the animation to
		/// </param>
		/// <param name="name">
		/// A String containing the name of the sequence
		/// </param>
		/// <param name="seq">The Sequence to be added</param>
		/// <param name="extraData">
		/// Display additional information on the console?
		/// </param>
		/// <remarks>
		/// The sequence name is checked against a list of known animations.
		/// Unknown sequences will be ignored.
		/// </remarks>
		private static bool AddSequence(AnimationList aniList, string name, Animation seq, bool extraData)
		{
			string[] words = name.Split(new char[] { ' ' });
			AnimationIndex index;
			Directions direction = 0;

			try
			{
				switch (words[0])
				{
					case "stand":
						if (words.Length == 2)
						{
							index = new PlayerDirectionAnimationIndex(
								PlayerDirectionAnimationIndex.Types.Stand,
								DirectionUtilities.FromString(words[1]), 0);
						}
						else
							throw new FormatException();

						seq.VideoMemory = true;

						break;

					case "walk":
						if (words.Length == 2)
						{
							index = new PlayerDirectionAnimationIndex(
								PlayerDirectionAnimationIndex.Types.Walk,
								DirectionUtilities.FromString(words[1]), 0);
						}
						else
							throw new FormatException();

						seq.VideoMemory = true;

						break;

					case "kick":
						if (words.Length == 2)
						{
							index = new PlayerDirectionAnimationIndex(
								PlayerDirectionAnimationIndex.Types.Kick,
								DirectionUtilities.FromString(words[1]), 0);
						}
						else
							throw new FormatException();
#if !LOWMEM
						seq.VideoMemory = true;
#endif

						break;

					case "shadow":
						if (words.Length != 1)
							throw new FormatException();

						index = new SimpleAnimationIndex(SimpleAnimationIndex.Types.DudeShadow);
						seq.VideoMemory = true;

						break;

					case "die":
						if (words.Length != 3 || words[1] != "green")
							throw new FormatException();

						int deathNo = int.Parse(words[2]);
						if (deathNo < 1/* || deathNo > DeathCount*/)
							throw new FormatException();

						index = new PlayerDeathAnimationIndex(0, 0);

						seq.Cached = false;

						break;

					case "bomb":
						if (words.Length == 4 && words[1] == "regular"
								&& words[2] == "green" && words[3] == "dud")
						{
							index = new PlayerAnimationIndex(
								PlayerAnimationIndex.Types.BombDud, 0);
						}
						else if (words.Length != 3 || words[2] != "green")
							throw new FormatException();

						else if (words[1] == "regular")
						{
							index = new PlayerAnimationIndex(
								PlayerAnimationIndex.Types.BombRegular, 0);

							seq.VideoMemory = true;
						}
						else if (words[1] == "jelly")
						{
							index = new PlayerAnimationIndex(
								PlayerAnimationIndex.Types.BombJelly, 0);
						}
						else if (words[1] == "trigger")
						{
							index = new PlayerAnimationIndex(
								PlayerAnimationIndex.Types.BombTrigger, 0);
						}
						else
							throw new FormatException();

						break;

					case "flame":
						if (words.Length != 3)
							throw new FormatException();
						
						if (words[1] == "brick")
						{
							int tileset = int.Parse(words[2]);
							index = new TilesetAnimationIndex(TilesetAnimationIndex.Types.ExplodingWall, tileset);
						}
						else if (words[2] == "green")
							if (words[1] == "center")
							{
								index = new PlayerAnimationIndex(
									PlayerAnimationIndex.Types.ExplosionCenter, 0);
							}
							else if (words[1].Substring(0, 3) == "mid")
							{
								index = new PlayerDirectionAnimationIndex(
									PlayerDirectionAnimationIndex.Types.ExplosionMid,
									DirectionUtilities.FromString(words[1].Substring(3)), 0);
							}
							else if (words[1].Substring(0, 3) == "tip")
							{
								index = new PlayerDirectionAnimationIndex(
									PlayerDirectionAnimationIndex.Types.ExplosionTip,
									DirectionUtilities.FromString(words[1].Substring(3)), 0);
							}
							else
								throw new FormatException();
						else
							throw new FormatException();

#if !LOWMEM
						seq.VideoMemory = true;
#endif

						// HACKHACK: flames need origin adjustment? Hmpf. TODO: ??
						//foreach (AnimationState stat in seq.States)
						//	stat.AddToOrigin(2, 1);

						break;
					case "punch":
						if (words.Length != 2)
							throw new FormatException();

						index = new PlayerDirectionAnimationIndex(
							PlayerDirectionAnimationIndex.Types.Punch,
							DirectionUtilities.FromString(words[1]), 0);
						seq.Cached = false;

						break;
					case "pickup":
						if (words.Length != 2)
							throw new FormatException();

						index = new PlayerDirectionAnimationIndex(
							PlayerDirectionAnimationIndex.Types.Pickup,
							DirectionUtilities.FromString(words[1]), 0);
						seq.Cached = false;

						break;
					case "tile":
					{
						if (words.Length != 3)
							throw new FormatException();

						int tileset = int.Parse(words[1]);

						if (tileset < 0)
							throw new FormatException("Not loading negative tileset (map editor)");

						if (words[2] == "solid")
						{
							index = new TilesetAnimationIndex(TilesetAnimationIndex.Types.Stone, tileset);
							// TRYTRY: Either cache stones, or insert them into the background,
							// because they won't change anyway. Currently: cache
							//seq.Cached = false;
						}
						else if (words[2] == "brick")
						{
							index = new TilesetAnimationIndex(TilesetAnimationIndex.Types.Wall, tileset);
						}
						else
							throw new FormatException();

						seq.VideoMemory = true;

						break;
					}
					case "numeric":
						if (words.Length != 2 || words[1] != "font")
							throw new FormatException();

						index = new SimpleAnimationIndex(SimpleAnimationIndex.Types.NumericFont);
#if !LOWMEM
						seq.VideoMemory = true;
#endif

						break;

					case "extra":
						if (words.Length == 2 && words[1] == "trampoline")
						{
							index = new SimpleAnimationIndex(SimpleAnimationIndex.Types.Trampoline);
							break;
						}
						else if (words.Length != 3)
							throw new FormatException();

						if (words[1] == "warp" && words[2] == "1")
						{
							index = new SimpleAnimationIndex(SimpleAnimationIndex.Types.Warphole);
							break;
						}

						direction = DirectionUtilities.FromString(words[2]);

						if (words[1] == "arrow")
						{
							index = new DirectionAnimationIndex(DirectionAnimationIndex.Types.Arrow, direction);
						}
						else if (words[1] == "conveyor")
						{
							index = new DirectionAnimationIndex(DirectionAnimationIndex.Types.ConveyorBelt, direction);
						}
						else
							throw new FormatException();

						break;

					case "power":
						if (words.Length != 2)
							throw new FormatException();

						if (words[1] == "bomb")
							index = new PowerupAnimationIndex(PowerupTypes.Bomb);

						else if (words[1] == "flame")
							index = new PowerupAnimationIndex(PowerupTypes.Range);

						else if (words[1] == "skate")
							index = new PowerupAnimationIndex(PowerupTypes.Speed);

						else if (words[1] == "kicker")
							index = new PowerupAnimationIndex(PowerupTypes.Kick);

						else if (words[1] == "jelly")
							index = new PowerupAnimationIndex(PowerupTypes.Jelly);

						else if (words[1] == "trigger")
							index = new PowerupAnimationIndex(PowerupTypes.Trigger);

						else if (words[1] == "punch")
							index = new PowerupAnimationIndex(PowerupTypes.Punch);

						else if (words[1] == "grab")
							index = new PowerupAnimationIndex(PowerupTypes.Grab);

						else if (words[1] == "spooge")
							index = new PowerupAnimationIndex(PowerupTypes.Spooge);

						else if (words[1] == "goldflame")
							index = new PowerupAnimationIndex(PowerupTypes.Goldflame);

						else if (words[1] == "disease")
							index = new PowerupAnimationIndex(PowerupTypes.Virus);

						else if (words[1] == "disease3")
							index = new PowerupAnimationIndex(PowerupTypes.BadVirus);

						else if (words[1] == "random")
							index = new PowerupAnimationIndex(PowerupTypes.Random);

						else
							throw new FormatException();

#if !LOWMEM
						seq.VideoMemory = true;
#endif
						// TODO disallow transparency in powerups. This is
						// required because the default powerups' key color
						// is set to black (HACKHACK?)
						//foreach (AnimationState stat in seq.States)
						//	stat.SetKeyColor(stat.RawKeyColor, System.Drawing.Color.Transparent);

						break;

					default:
						throw new FormatException();
				}

				if (extraData)
					Console.WriteLine("Loading animation sequence \"{0}\","
										+ " which has {1} states",
										name, seq.Frames.Length);

				return aniList.AddAnimation(index, seq);

				/*if (playerSequence)
				{
					if (directedSequence)
					{
						for (int player = 1; player <= PlayerCount; ++player)
							this[new SequenceIndex(directedType, player, direction)] = new Sequence(seq, Game.PlayerColor(player));
					}
					else
						for (int player = 1; player <= PlayerCount; ++player)
							this[new SequenceIndex(type, player)] = new Sequence(seq, Game.PlayerColor(player));
					BitmapBuilder.SeqCropDone(true);
				}
				else if (deathSequence)
				{
					for (int player = 1; player <= PlayerCount; ++player)
						this[new SequenceIndex(player, deathNo)] = new Sequence(seq, Game.PlayerColor(player));
					BitmapBuilder.SeqCropDone(true);
				}
				else
				{
					this[index] = seq;
					Fabba.Utilities.BitmapBuilder.SeqCropDone(false);
					/*Console.WriteLine("Sequence {0} has been loaded. It has {3} states. Size of the first is {1}x{2}",
										name, seq.States[0].BitmapBuilder.Width, seq.States[0].BitmapBuilder.Height, seq.States.Length);* /
				}*/
			}
			catch (FormatException e)
			{
				if (extraData)
					Console.WriteLine("Not loading sequence \"{0}\" because"
										+ " it is not required or unknown (\"{1}\")",
										name, e.Message);
				return false;
			}
			catch (OverflowException)
			{
				Console.WriteLine("Not loading sequence \"{0}\" because the number caused an overflow",
									name);
				return false;
			}
		}
	}
}