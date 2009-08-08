//
// Bomber Stuff: Atomic Bomberman Remake
//  Copyright © 2008-2009 Thomas Faber
//  All Rights Reserved.
//
// Sequence.cs - class for storing an animation sequence
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

using Fabba.Utilities;
using Bomber.Graphics;

namespace Bomber.Files
{
/// <summary>
/// An animation sequence
/// </summary>
public class Sequence : IDisposable
{
	public string Name;
	public SeqState[] States;
	public bool VideoMemory = false;
	private bool m_Cached = true;
	private bool m_Remap = false;
	private ColorRemapInfo RemapInfo;
	
	//public int[] SeqValues = new int[16];
	
	/// <summary>
	/// Default constructor
	/// </summary>
	public Sequence()
	{
		// TRYTRY: why is this needed if its empty?
	}
	
	/// <summary>
	/// Creates a remapped sequence from the specified original
	/// </summary>
	public Sequence(Sequence old, ColorRemapInfo remapInfo)
	{
		// do a shallow copy but note that we are hue-mapped
		VideoMemory = old.VideoMemory;
		Name = old.Name;
		States = (SeqState[])old.States.Clone();
		m_Cached = old.m_Cached;
		
		// no change? then don't remap
		if ((!remapInfo.SetHue || remapInfo.NewHue == ColorRemapper.OriginalHue)
				&& !remapInfo.SetSaturation && remapInfo.DiffLightness == 0.0)
			m_Remap = false;
		else
		{
			m_Remap = true;
			RemapInfo = remapInfo;
			Name +=  " (" + remapInfo + ")";
		}
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
	/// Frees the sequence's unmanaged resources
	/// and optionally also disposes of managed resources
	/// </summary>
	/// <param name="disposing">
	/// Specifies whether managed resources should be freed in addition to
	/// unmanaged ones.
	/// </param>
	protected virtual void Dispose(bool disposing)
	{
#if DEBUG
		//Console.WriteLine("Disposing of Sequence ({0}): {1}", _TextureCount, Name);
#endif
		
		if (disposing)
		{
			Name = null;
			
		}
		
		if (States != null)
		{
			for (int i = 0; i < States.Length; ++i)
				if (States[i] != null)
				{
					States[i].Dispose();
					States[i] = null;
				}
			States = null;
		}
	}

	/*/// <summary>
	/// Frees the object's unmanaged resources so that the garbage
	/// collector can remove it
	/// </summary>
	~Sequence()
	{
		Dispose(false);
	}*/
	
	/// <summary>
	/// Gets/Sets whether the textures for this sequence should be cached
	/// </summary>
	/// <remarks>
	/// This is <c>true</c> by default. It can be set to <c>false</c>,
	/// but never be changed to true.
	/// </remarks>
	public bool Cached
	{
		get { return m_Cached; }
		set
		{
			if (value == true)
				throw new InvalidOperationException("Caching of sequence textures can only be disabled! It is enabled by default");
			
			m_Cached = false;
		}
	}
	
	/// <summary>
	/// Retrieves the texture associated with the specified state
	/// of the sequence
	/// </summary>
	/// <param name="device">The Device to create the Texture for</param>
	/// <param name="i">The index of the sequence state</param>
	/// <returns>
	/// A Texture that can be drawn to the specified device
	/// </returns>
	/// <remarks>
	/// Note that the texture is cached if the Cached property is set to
	/// <c>true</c>, which is the default. In that case, passing a
	/// different Device to this function does not update the Texture to
	/// work with the new Device.
	/// </remarks>
	public Sprite GetStateTexture(Device device, int i)
	{
		lock (States[i])
		{
			if (States[i] == null)
			{
				Console.WriteLine("Sequence {0}, state {1} doesn't seem to exist", Name, i);
				return null;
			}
			
			if (!m_Cached && (States[i].CachedTexture == null || m_Remap && !States[i].RemapDone))
				LoadStateTexture(device, i);
#if DEBUG
			else if (States[i].CachedTexture == null)
				throw new NullReferenceException("No Cached Texture!");
#endif
			
			return States[i].CachedTexture;
		}
	}
	
	/// <summary>
	/// 
	/// </summary>
	/// <param name="device"></param>
	public void CacheTextures(Device device)
	{
		if (!m_Cached)
			return;
		
		for (int i = 0; i < States.Length; ++i)
			lock (States[i])
			{
				// delete the old texture before re-caching
				// (required on device reset)
				/*if (States[i].CachedTexture != null)
				{
					States[i].CachedTexture.Dispose();
					States[i].CachedTexture = null;
					//States[i].RemapDone = false; // this isn't necessary
				}*/
				LoadStateTexture(device, i);
			}
	}
	
	/// <summary>
	/// 
	/// </summary>
	/// <remarks>this function is thread safe</remarks>
	public void ClearTextureCache()
	{
		if (m_Cached)
			return;
		
		for (int i = 0; i < States.Length; ++i)
			lock (States[i])
			{
				if (States[i].CachedTexture != null)
				{
					//Console.WriteLine("Disposing of texture ({0}): state {1} of sequence {2}", --_TextureCount, i, Name);
					States[i].CachedTexture.Dispose();
					States[i].CachedTexture = null;
				}
			}
	}
#if DEBUG
	public static int _TextureCount = 0;
#endif
	/// <summary>
	///
	/// </summary>
	/// <param name="device">device to load textures for</param>
	/// <param name="i">index of the state to load a texture for</param>
	/// <remarks>Not thread safe! Lock the SeqState before calling!</remarks>
	private void LoadStateTexture(Device device, int i)
	{
		// locked by GetStateTexture/CacheTextures
		
		{
			if (m_Remap && !States[i].RemapDone)
			{
				// if this is a remapped sequence, deep copy the state, then remap
				// its bitmap builder
				SeqState newState = new SeqState(States[i]);
				
				ColorRemapper.Remap(newState, States[i], RemapInfo);
				newState.RemapDone = true;
				States[i] = newState;
			}
			else if (States[i].CachedTexture != null)
				return;

			States[i].CachedTexture = BomberStuff.Graphics.LoadSprite(device, States[i].BitmapBuilder, VideoMemory, States[i].KeyColor);
			//Console.WriteLine("Loading texture ({0}): state {1} of sequence {2}", ++_TextureCount, i, Name);
		}
	}
}
}