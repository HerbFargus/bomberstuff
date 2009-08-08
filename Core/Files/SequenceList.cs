//
// Bobmer Stuff: Atomic Bomberman Remake
//
// SequenceList.cs - a list of animation sequences
//
// Copyright © 2008-2009 Thomas Faber
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
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading;

using Bomber.Files;
using Bomber.Graphics;

namespace Bomber
{
// reeeeeally don't touch this

/// <summary>
/// A list of animation sequences
/// </summary>
/// <remarks>
/// (TODO/TRYTRY) static/singleton SequenceList?
/// </remarks>
public sealed class SequenceList : IEnumerable<Sequence>, IDisposable
{
	// HINTHINT number of death animations. See animation loading
	// in BomberForm.cs
	/// <summary>The number of death animations to be loaded</summary>
	public static int DeathCount = 24;

	// this is for debugging. Default death animation (0 = random)
	/// <summary>Debug Stuff</summary>
	public static int _DeathNo = 0;
	
	// TODO: this needs to be non-static and variable
	/// <summary>
	/// The number of players to load sprites for. This is the maximum
	/// number of players in the game.
	/// </summary>
	public static int PlayerCount = 10;
	
	/// <summary>
	/// The internal animation sequence array
	/// </summary>
	private Sequence[] m_Sequences;
	
	
	/// <summary>
	/// Initialize a new sequence list
	/// </summary>
	public SequenceList()
	{
		m_Sequences = new Sequence[SequenceIndex.NumberOfSequences];
	}
	
	//
	// utility functions
	//
	/// <summary>
	/// Get the animation sequence for the specified bomb dude
	/// </summary>
	/// <param name="moving">dude moving?</param>
	/// <param name="direction">direction in which dude is looking</param>
	/// <param name="player">the player number, which leads to its color</param>
	/// <returns></returns>
	public static SequenceIndex GetDudeSequence(bool moving, DudeDirections direction, int player)
	{
		SequenceIndex ret;
		
		if (moving)
			ret = new SequenceIndex(DirectedPlayerSequenceTypes.Walk, player, direction);
		else
			ret = new SequenceIndex(DirectedPlayerSequenceTypes.Stand, player, direction);
		
		return ret;
	}

	/// <summary>
	/// Get the animation sequence for the specified kicking bomb dude
	/// </summary>
	/// <param name="direction">direction in which dude is kicking</param>
	/// <param name="player">
	/// the player number, which leads to its color
	/// </param>
	/// <returns></returns>
	public static SequenceIndex GetKickSequence(DudeDirections direction, int player)
	{
		return new SequenceIndex(DirectedPlayerSequenceTypes.Kick, player, direction);
	}
	
	/// <summary>
	/// Get the animation sequence for the specified punching bomb dude
	/// </summary>
	/// <param name="direction">direction in which dude is punching</param>
	/// <param name="player">
	/// the player number, which leads to its color
	/// </param>
	/// <returns></returns>
	public static SequenceIndex GetPunchSequence(DudeDirections direction, int player)
	{
		return new SequenceIndex(DirectedPlayerSequenceTypes.Punch, player, direction);
	}

	/// <summary>
	/// Get the animation sequence for the specified dude picking up a bomb
	/// </summary>
	/// <param name="direction">direction in which dude is punching</param>
	/// <param name="player">
	/// the player number, which leads to its color
	/// </param>
	/// <returns></returns>
	public static SequenceIndex GetPickupSequence(DudeDirections direction, int player)
	{
		return new SequenceIndex(DirectedPlayerSequenceTypes.Pickup, player, direction);
	}
	
	/// <summary>
	/// Get an animation sequence for the specified bomb dude dying
	/// </summary>
	/// <param name="player">the player number, which leads to its color</param>
	/// <returns></returns>
	public static SequenceIndex GetDieSequence(int player)
	{
		return new SequenceIndex(player, (_DeathNo == 0)
											? (BomberStuff.Random.Next(DeathCount) + 1)
											: _DeathNo);
	}

	/// <summary>
	/// Get the death animation sequence for the specified bomb
	/// dude which is the angel
	/// </summary>
	/// <param name="player">the player number, which leads to its color</param>
	/// <returns></returns>
	public static SequenceIndex GetAngelSequence(int player)
	{
		return new SequenceIndex(8, player);
	}
	
	/// <summary>
	/// Get the animation sequence for the specified "middle" explosion part
	/// </summary>
	/// <param name="direction">direction of the explosion</param>
	/// <param name="player">the player number, which leads to its color</param>
	/// <returns></returns>
	public static SequenceIndex GetFlameMidSequence(DudeDirections direction, int player)
	{
		return new SequenceIndex(DirectedPlayerSequenceTypes.FlameMid, player, direction);
	}

	/// <summary>
	/// Get the animation sequence for the specified tip explosion part
	/// </summary>
	/// <param name="direction">direction of the explosion</param>
	/// <param name="player">the player number, which leads to its color</param>
	/// <returns></returns>
	public static SequenceIndex GetFlameTipSequence(DudeDirections direction, int player)
	{
		return new SequenceIndex(DirectedPlayerSequenceTypes.FlameTip, player, direction);
	}

	/// <summary>
	/// Get the animation sequence for the specified center explosion part
	/// </summary>
	/// <param name="player">the player number, which leads to its color</param>
	/// <returns></returns>
	public static SequenceIndex GetFlameCenterSequence(int player)
	{
		return new SequenceIndex(PlayerSequenceTypes.FlameCenter, player);
	}
	
	/// <summary>
	/// Get the animation sequence for the specified bomb
	/// </summary>
	/// <param name="jelly">whether the bomb is a jelly bomb</param>
	/// <param name="trigger">whether the bomb is a trigger bomb</param>
	/// <param name="dud">whether the bomb is a dud bomb</param>
	/// <param name="player">the player number, which leads to its color</param>
	/// <returns></returns>
	public static SequenceIndex GetBombSequence(bool jelly, bool trigger, bool dud, int player)
	{
		// bomb appearance is prioritized in this order
		if (dud)
			return new SequenceIndex(PlayerSequenceTypes.BombDud, player);
		if (trigger)
			return new SequenceIndex(PlayerSequenceTypes.BombTrigger, player);
		else if (jelly)
			return new SequenceIndex(PlayerSequenceTypes.BombJelly, player);
		else
			return new SequenceIndex(PlayerSequenceTypes.BombRegular, player);
	}
	
	/// <summary>
	/// Get the animation sequence for the specified powerup
	/// </summary>
	/// <param name="type">the powerup type</param>
	/// <returns></returns>
	public static SequenceIndex GetPowerupSequence(PowerupTypes type)
	{
		return new SequenceIndex(type);
	}
	
	/// <summary>
	/// Get the animation sequence for the specified arrow extra
	/// </summary>
	/// <param name="direction">direction of the arrow</param>
	/// <returns></returns>
	public static SequenceIndex GetArrowSequence(DudeDirections direction)
	{
		return new SequenceIndex(DirectedSequenceTypes.Arrow, direction);
	}

	/// <summary>
	/// Get the animation sequence for the specified conveyor belt item
	/// </summary>
	/// <param name="direction">direction of the conveyor belt</param>
	/// <returns></returns>
	public static SequenceIndex GetConveyorBeltSequence(DudeDirections direction)
	{
		return new SequenceIndex(DirectedSequenceTypes.ConveyorBelt, direction);
	}
	
	//
	// The Indexer
	//
	/// <value>
	/// Gets/sets the Sequence associated with the specified
	/// <c>SequenceIndex</c> enumeration member
	/// </value>
	public Sequence this[SequenceIndex i]
	{
		get
		{
			return m_Sequences[(int)i];
		}
		private set
		{
			if (m_Sequences[(int)i] != null)
				throw new InvalidOperationException("Sequence " + i + " added twice");
			
			m_Sequences[(int)i] = value;
		}
	}
	
	/// <summary>
	/// Check whether all required animations are loaded into the list
	/// </summary>
	/// <exception cref="FileNotFoundException">
	/// A required sequence is missing
	/// </exception>
	// TODO: rename?
	public void Check()
	{
		for (int i = 1; i < m_Sequences.Length; ++i)
		{
			if (m_Sequences[i] == null)
				throw new FileNotFoundException("Animation sequence " + i + " is missing. Your Atomic Bomberman files seem corrupt.");
		}
	}
	
	/// <summary>
	/// Adds a Sequence with the specified name
	/// </summary>
	/// <param name="name">
	/// A String containing the name of the sequence
	/// </param>
	/// <param name="seq">The Sequence to be added</param>
	/// <remarks>
	/// The sequence name is checked against a list of known animations.
	/// Unknown sequences will be ignored.
	/// </remarks>
	private void AddSequence(string name, Sequence seq)
	{
		string[] words = name.Split(new char[] { ' ' });
		SequenceIndex index = new SequenceIndex();
		bool playerSequence = false;
		bool directedSequence = false;
		bool deathSequence = false;
		PlayerSequenceTypes type = 0;
		DirectedPlayerSequenceTypes directedType = 0;
		DudeDirections direction = 0;
		int deathNo = 0;
		
		try
		{
			switch (words[0])
			{
				case "stand":
					if (words.Length == 2)
					{
						directedType = DirectedPlayerSequenceTypes.Stand;
						playerSequence = true;
						directedSequence = true;
						direction = DirectionUtilities.FromString(words[1]);
					}
					else
						throw new FormatException();
					
					seq.VideoMemory = true;
					
					break;
				
				case "walk":
					if (words.Length == 2)
					{
						directedType = DirectedPlayerSequenceTypes.Walk;
						playerSequence = true;
						directedSequence = true;
						direction = DirectionUtilities.FromString(words[1]);
					}
					else
						throw new FormatException();
					
					seq.VideoMemory = true;
					
					break;
					
				case "kick":
					if (words.Length == 2)
					{
						directedType = DirectedPlayerSequenceTypes.Kick;
						playerSequence = true;
						directedSequence = true;
						direction = DirectionUtilities.FromString(words[1]);
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
					
					index = new SequenceIndex(SequenceTypes.DudeShadow);
					seq.VideoMemory = true;
					
					break;
					
				case "die":
					if (words.Length != 3 || words[1] != "green")
						throw new FormatException();

					deathSequence = true;
					deathNo = int.Parse(words[2]);
					
					if (deathNo < 1 || deathNo > DeathCount)
						throw new FormatException();
					
					seq.Cached = false;
					
					break;
					
				case "bomb":
					if (words.Length == 4 && words[1] == "regular"
							&& words[2] == "green" && words[3] == "dud")
					{
						playerSequence = true;
						type = PlayerSequenceTypes.BombDud;
					}
					else if (words.Length != 3 || words[2] != "green")
						throw new FormatException();
					
					else if (words[1] == "regular")
					{
						playerSequence = true;
						type = PlayerSequenceTypes.BombRegular;
						
						seq.VideoMemory = true;
					}
					else if (words[1] == "jelly")
					{
						playerSequence = true;
						type = PlayerSequenceTypes.BombJelly;
					}
					else if (words[1] == "trigger")
					{
						playerSequence = true;
						type = PlayerSequenceTypes.BombTrigger;
					}
					else
						throw new FormatException();
					
					break;
					
				case "flame":
					if (words.Length != 3)
						throw new FormatException();
					
					if (words[1] == "brick" && words[2] == Board.Tileset.ToString())
						index = new SequenceIndex(SequenceTypes.WallExplode);
					else if (words[2] == "green")
						if (words[1] == "center")
						{
							playerSequence = true;
							type = PlayerSequenceTypes.FlameCenter;
						}
						else if (words[1].Substring(0, 3) == "mid")
						{
							direction = DirectionUtilities.FromString(words[1].Substring(3));
							playerSequence = true;
							directedSequence = true;
							directedType = DirectedPlayerSequenceTypes.FlameMid;
						}
						else if (words[1].Substring(0, 3) == "tip")
						{
							direction = DirectionUtilities.FromString(words[1].Substring(3));
							playerSequence = true;
							directedSequence = true;
							directedType = DirectedPlayerSequenceTypes.FlameTip;
						}
						else
							throw new FormatException();
					else
						throw new FormatException();
					
#if !LOWMEM
					seq.VideoMemory = true;
#endif
					
					// HACKHACK: flames need origin adjustment? Hmpf.
					foreach (SeqState stat in seq.States)
						stat.AddToOrigin(2, 1);
					
					break;
				case "punch":
					if (words.Length != 2)
						throw new FormatException();

					playerSequence = true;
					directedSequence = true;
					directedType = DirectedPlayerSequenceTypes.Punch;
					direction = DirectionUtilities.FromString(words[1]);
					seq.Cached = false;
					
					break;
				case "pickup":
					if (words.Length != 2)
						throw new FormatException();
					
					playerSequence = true;
					directedSequence = true;
					directedType = DirectedPlayerSequenceTypes.Pickup;
					direction = DirectionUtilities.FromString(words[1]);
					seq.Cached = false;
					
					break;
				case "tile":
					if (words.Length != 3)
						throw new FormatException();
					
					if (words[1] != Board.Tileset.ToString())
						throw new FormatException("Tile which does not belong to tileset");
					
					if (words[2] == "solid")
					{
						index = new SequenceIndex(SequenceTypes.Stone);
						
						// we do not need a texture for stones as they'll
						// be inserted into the background image
						seq.Cached = false;
					}
					else if (words[2] == "brick")
						index = new SequenceIndex(SequenceTypes.Wall);
					else
						throw new FormatException();
					
					seq.VideoMemory = true;
					
					break;
					
				case "numeric":
					if (words.Length != 2 || words[1] != "font")
						throw new FormatException();
					
					index = new SequenceIndex(SequenceTypes.NumericFont);
#if !LOWMEM
					seq.VideoMemory = true;
#endif
					
					break;
					
				case "extra":
					if (words.Length == 2 && words[1] == "trampoline")
					{
						index = new SequenceIndex(SequenceTypes.Trampoline);
						break;
					}
					else if (words.Length != 3)
						throw new FormatException();
					
					if (words[1] == "warp" && words[2] == "1")
					{
						index = new SequenceIndex(SequenceTypes.Warphole);
						break;
					}

					direction = DirectionUtilities.FromString(words[2]);
					
					if (words[1] == "arrow")
					{
						index = new SequenceIndex(DirectedSequenceTypes.Arrow, direction);
					}
					else if (words[1] == "conveyor")
					{
						index = new SequenceIndex(DirectedSequenceTypes.ConveyorBelt, direction);
					}
					else
						throw new FormatException();
					
					break;
					
				case "power":
					if (words.Length != 2)
						throw new FormatException();
					
					if (words[1] == "bomb")
						index = new SequenceIndex(PowerupTypes.Bomb);
					
					else if (words[1] == "flame")
						index = new SequenceIndex(PowerupTypes.Range);
					
					else if (words[1] == "skate")
						index = new SequenceIndex(PowerupTypes.Speed);
					
					else if (words[1] == "kicker")
						index = new SequenceIndex(PowerupTypes.Kick);
					
					else if (words[1] == "jelly")
						index = new SequenceIndex(PowerupTypes.Jelly);
					
					else if (words[1] == "trigger")
						index = new SequenceIndex(PowerupTypes.Trigger);
					
					else if (words[1] == "punch")
						index = new SequenceIndex(PowerupTypes.Punch);
					
					else if (words[1] == "grab")
						index = new SequenceIndex(PowerupTypes.Grab);
					
					else if (words[1] == "spooge")
						index = new SequenceIndex(PowerupTypes.Spooge);
					
					else if (words[1] == "goldflame")
						index = new SequenceIndex(PowerupTypes.Goldflame);
					
					else if (words[1] == "disease")
						index = new SequenceIndex(PowerupTypes.Virus);
					
					else if (words[1] == "disease3")
						index = new SequenceIndex(PowerupTypes.BadVirus);
					
					else if (words[1] == "random")
						index = new SequenceIndex(PowerupTypes.Random);
					
					else
						throw new FormatException();
					
#if !LOWMEM
					seq.VideoMemory = true;
#endif
					// disallow transparency in powerups. This is
					// required because the default powerups' key color
					// is set to black (HACKHACK?)
					foreach (SeqState stat in seq.States)
						stat.SetKeyColor(stat.RawKeyColor, System.Drawing.Color.Transparent);
					
					break;
					
				default:
					throw new FormatException();
			}
			
			if (Settings.ExtraData)
				Console.WriteLine("Loading animation sequence \"{0}\","
									+ " which has {1} states",
									name, seq.States.Length);
			
			if (playerSequence)
			{
				if (directedSequence)
				{
					for (int player = 1; player <= PlayerCount; ++player)
						this[new SequenceIndex(directedType, player, direction)] = new Sequence(seq, Game.PlayerColor(player));
				}
				else
					for (int player = 1; player <= PlayerCount; ++player)
						this[new SequenceIndex(type, player)] = new Sequence(seq, Game.PlayerColor(player));
				Fabba.Utilities.BitmapBuilder.SeqCropDone(true);
			}
			else if (deathSequence)
			{
				for (int player = 1; player <= PlayerCount; ++player)
					this[new SequenceIndex(player, deathNo)] = new Sequence(seq, Game.PlayerColor(player));
				Fabba.Utilities.BitmapBuilder.SeqCropDone(true);
			}
			else
			{
				this[index] = seq;
				Fabba.Utilities.BitmapBuilder.SeqCropDone(false);
				/*Console.WriteLine("Sequence {0} has been loaded. It has {3} states. Size of the first is {1}x{2}",
									name, seq.States[0].BitmapBuilder.Width, seq.States[0].BitmapBuilder.Height, seq.States.Length);*/
			}
		}
		catch (FormatException e)
		{
			if (Settings.ExtraData)
				Console.WriteLine("Not loading sequence \"{0}\" because"
									+ " it is not required or unknown (\"{1}\")",
									name, e.Message);
		}
		catch (OverflowException)
		{
			Console.WriteLine("Not loading sequence \"{0}\" because the number caused an overflow",
								name);
		}
	}
	
	//
	// IEnumerable implementation
	//
	/// <summary>
	/// Returns an enumerator that iterates through the collection.
	/// </summary>
	/// <returns>A <see cref="IEnumerator{Sequence}" /> that can be used to
	/// iterate through the collection.
	/// </returns>
	public IEnumerator<Sequence> GetEnumerator()
	{
		// simply return all items in our internal array
		for (int i = 1; i < m_Sequences.Length; ++i)
			yield return m_Sequences[i];
	}

	/// <summary>
	/// Returns an enumerator that iterates through the collection.
	/// </summary>
	/// <returns>A <see cref="IEnumerator" /> that can be used to
	/// iterate through the collection.
	/// </returns>
	IEnumerator IEnumerable.GetEnumerator()
	{
		return GetEnumerator();
	}
	
	//
	//
	//
	/// <summary>
	/// Dispose the bitmap builders of all cached sequences' frames
	/// </summary>
	public void DisposeBitmapBuilders()
	{
		foreach (Sequence seq in this)
			if (seq.Cached)
				foreach (SeqState stat in seq.States)
					if (stat.BitmapBuilder != null)
						stat.BitmapBuilder = null;
	}
	
	/// <summary>
	/// Initializes all textures by creating them for the specified device.
	/// </summary>
	/// <param name="device">the device to create textures on</param>
	/// <remarks>
	/// This should be called directly after LoadFile or LoadAnimationList
	/// </remarks>
	public void CacheTextures(Device device)
	{
		foreach (Sequence seq in this)
			seq.CacheTextures(device);
	}
	
	/// <summary>
	/// Clears the Texture cache
	/// </summary>
	public void ClearTextureCache()
	{
		foreach (Sequence seq in this)
			seq.ClearTextureCache();
	}
	
	/*//
	// Loading functions
	//
	/// <summary>
	/// Loads an ALI animation list file, and loads each ANI file specified
	/// therein
	/// </summary>
	/// <param name="listFile">path to the master.ali animation list file</param>
	/// <seealso cref="LoadFile" />
	public void LoadAnimationList(string listFile)
	{
		throw new System.NotImplementedException();
	}*/
	
	/// <summary>
	/// Adds all known sequences found in the specified ANI file to the list
	/// </summary>
	/// <param name="filename">name of an ANI file</param>
	public Thread LoadFile(string filename)
	{
		Animation ani = new Animation(filename, Settings.ExtraData);
		
		// HACKHACK: this is not the nicest way to get this multithreaded
		Thread t = new Thread(new LoadAniThread(this, ani).Run);
		t.Start();
		return t;
	}
	
	/// <summary>
	/// A structure used to start a thread for animation loading
	/// </summary>
	private struct LoadAniThread
	{
		private Animation Animation;
		private SequenceList SequenceList;
		internal LoadAniThread(SequenceList s, Animation ani)
		{
			SequenceList = s;
			Animation = ani;
		}
		
		internal void Run()
		{
			foreach (Sequence seq in Animation.Sequences)
				SequenceList.AddSequence(seq.Name, seq);
			
			Animation = null;
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
	/// Frees the sequence list's unmanaged resources
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
			for (int i = 1; i < m_Sequences.Length; ++i)
				if (m_Sequences[i] != null)
				{
					m_Sequences[i].Dispose();
					m_Sequences[i] = null;
				}
			
			m_Sequences = null;
		}
	}

	// TRYTRY destructor not needed here. Is it good practice to leave it out though?
	/*/// <summary>
	/// Frees the object's unmanaged resources so that the garbage
	/// collector can remove it
	/// </summary>
	~SequenceList()
	{
		Dispose(false);
	}*/
}

}