//
// Bomber Stuff: Atomic Bomberman Remake
//  Copyright © 2008 Thomas Faber
//  All Rights Reserved.
//
// SeqState.cs - class for storing an animation sequence state
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

using System.Drawing;
using System;

using Fabba.Utilities;
using Bomber.Graphics;

namespace Bomber.Files
{
public sealed class Frame : IDisposable
{
	public string FileName;

	// AB Animation properties (are they?) that are not required
	//public ushort Width, Height;
	public ushort HotSpotX, HotSpotY;
	public Color KeyColor;
	public ushort RawKeyColor;
	public BitmapBuilder BitmapBuilder;
	public Sprite CachedTexture;
	
	/// <summary>
	/// Initialize a frame with default attributes
	/// </summary>
	public Frame()
	{
		//Width = Height = 0;
		HotSpotX = HotSpotY = 0;
		KeyColor = Color.Transparent;
		RawKeyColor = 0xFFFF;
		BitmapBuilder = null;
		CachedTexture = null;
	}
	
	/// <summary>
	/// Copy constructor. Perform a deep copy of a specified object to
	/// fill the new one
	/// </summary>
	/// <param name="old">old object to copy</param>
	public Frame(Frame old)
	{
		//Width = old.Width;
		//Height = old.Height;
		FileName = old.FileName;
		HotSpotX = old.HotSpotX;
		HotSpotY = old.HotSpotY;
		KeyColor = old.KeyColor;
		RawKeyColor = old.RawKeyColor;
		BitmapBuilder = new BitmapBuilder(old.BitmapBuilder);
		CachedTexture = old.CachedTexture;
	}

	/// <summary>
	/// Frees the object's managed and unmanaged resources
	/// </summary>
	public void Dispose()
	{
		Dispose(true);
		
		GC.SuppressFinalize(this);
	}
	
	/// <summary>
	/// Frees the Frame's unmanaged resources, namely the Cached Texture,
	/// and optionally also disposes of managed resources
	/// </summary>
	/// <param name="disposing">
	/// Specifies whether managed resources should be freed in addition to
	/// unmanaged ones.
	/// </param>
	private /*protected virtual*/ void Dispose(bool disposing)
	{
		if (disposing)
		{
			FileName = null;
			BitmapBuilder = null;
		}
		
		if (CachedTexture != null)
		{
			CachedTexture.Dispose();
			CachedTexture = null;
		}
	}

	/// <summary>
	/// Frees the object's unmanaged resources so that the garbage
	/// collector can remove it
	/// </summary>
	~Frame()
	{
		Dispose(false);
	}
}

public sealed class SeqState : IDisposable
{
	private Frame m_Frame;
	// AB Animation properties that are not required
	//public int[] StateValues = new int[16];
	//public short NewSpeed;
	//public short MoveX, MoveY;
	public short FrameX, FrameY;
	public bool RemapDone = false;
	
	/// <summary>
	/// Default constructor.
	/// </summary>
	public SeqState()
	{
	}

	
	/// <summary>
	/// Perform a deep copy of a specified object to fill the new one
	/// </summary>
	/// <param name="old">old object to copy</param>
	public SeqState(SeqState old)
	{
		m_Frame = new Frame(old.m_Frame);
		//(copy state values)
		//NewSpeed = old.NewSpeed;
		//MoveX = old.MoveX;
		//MoveY = old.MoveY;
		FrameX = old.FrameX;
		FrameY = old.FrameY;
	}
	
	/// <summary>
	/// Frees the object's managed and unmanaged resources
	/// </summary>
	public void Dispose()
	{
		Dispose(true);
		
		GC.SuppressFinalize(this);
	}

	/// <summary>
	/// Frees the state's unmanaged resources
	/// and optionally also disposes of managed resources
	/// </summary>
	/// <param name="disposing">
	/// Specifies whether managed resources should be freed in addition to
	/// unmanaged ones.
	/// </param>
	private /*protected virtual*/ void Dispose(bool disposing)
	{
		if (disposing)
		{
			if (m_Frame != null)
			{
				m_Frame.Dispose();
				m_Frame = null;
			}
		}
	}

	/// <summary>
	/// Frees the object's unmanaged resources so that the garbage
	/// collector can remove it
	/// </summary>
	~SeqState()
	{
		Dispose(false);
	}
	
	/// <summary>
	/// Sets the Frame associated with this state
	/// </summary>
	internal Frame Frame
	{
		set { m_Frame = value; }
	}
	
	/// <summary>
	/// Gets the image file name of the state's associated frame
	/// </summary>
	public string FileName
	{
		get { return m_Frame.FileName; }
	}
	
	//public ushort Width { get { return m_Frame.BitmapBuilder.Width; } }
	//public ushort Height { get { return m_Frame.BitmapBuilder.Height; } }
	
	// TODO correctly document all the different kinds of origin points o_O
	/// <summary>
	/// Gets the state's origin point that can be used to draw the object.
	/// This is the point in the image that should be placed in the
	/// top-left corner of the field that the object is on
	/// </summary>
	/// <remarks>
	/// The Origin is Calculated as (HotSpotX - 18, HotSpotY - 16)
	/// </remarks>
	public Point<ScreenX, ScreenY> Origin
	{
		get
		{
			return new Point<ScreenX, ScreenY>(new ScreenX(HotSpotX - 18), new ScreenY(HotSpotY - 16));
		}
	}
	
	/// <summary>
	/// 
	/// </summary>
	public Point<ScreenX, ScreenY> OriginalOrigin
	{
		get
		{
			return new Point<ScreenX, ScreenY>(new ScreenX(m_Frame.HotSpotX), new ScreenY(m_Frame.HotSpotY));
		}
	}
	
	/// <summary>
	/// 
	/// </summary>
	public int HotSpotX
	{
		get { return m_Frame.HotSpotX - FrameX; }
	}
	
	/// <summary>
	/// 
	/// </summary>
	public int HotSpotY
	{
		get { return m_Frame.HotSpotY - FrameY; }
	}
	
	/// <summary>
	/// Subtracts the given values from the origin's x and y coordinates
	/// </summary>
	/// <param name="x"></param>
	/// <param name="y"></param>
	public void AddToOrigin(short x, short y)
	{
		FrameX += x;
		FrameY += y;
	}
	
	/// <summary>
	/// Gets the associated frame's key color
	/// </summary>
	/// <seealso cref="Bomber.Files.Frame.KeyColor"/>
	public Color KeyColor { get { return m_Frame.KeyColor; } }

	/// <summary>
	/// Gets the associated frame's raw key color
	/// </summary>
	/// <seealso cref="Bomber.Files.Frame.RawKeyColor"/>
	public ushort RawKeyColor { get { return m_Frame.RawKeyColor; } }

	/// <summary>
	/// Sets the associated frame's key color
	/// </summary>
	/// <param name="newRawKeyColor"></param>
	/// <param name="newKeyColor"></param>
	public void SetKeyColor(ushort newRawKeyColor, Color newKeyColor)
	{
		m_Frame.RawKeyColor = newRawKeyColor;
		m_Frame.KeyColor = newKeyColor;
	}
	
	/// <summary>
	/// Gets a GDI+ Bitmap image from the associated frame
	/// </summary>
	/// <remarks>
	/// Note that the image is not cached, but recreated on every
	/// access of the property.
	/// </remarks>
	public Bitmap Image
	{
		get
		{
			Bitmap image = (Bitmap)System.Drawing.Image.FromStream(m_Frame.BitmapBuilder.GetStream());
			image.MakeTransparent(m_Frame.KeyColor);
			return image;
		}
	}
	
	/// <summary>
	/// Gets the associated frame's BitmapBuilder or sets it to <c>null</c>
	/// </summary>
	public BitmapBuilder BitmapBuilder
	{
		get { return m_Frame.BitmapBuilder; }
		set
		{
			if (value != null)
				throw new InvalidOperationException("Sequence state bitmap builders can only be set to null!");
			
			m_Frame.BitmapBuilder = null;
		}
	}
	
	/// <summary>
	/// Gets/Sets the associated frame's Cached Texture
	/// </summary>
	public Sprite CachedTexture
	{
		get { return m_Frame.CachedTexture; }
		set { m_Frame.CachedTexture = value; }
	}
}
}