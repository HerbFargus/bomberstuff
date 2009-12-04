﻿//
// AnimationList.cs - AnimationList class
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
using System.Collections.Generic;

using BomberStuff.Core.Utilities;

namespace BomberStuff.Core.Animation
{
	/// <summary>
	/// 
	/// </summary>
	public class AnimationList : IDisposable
	{
		/// <summary></summary>
		private Animation[] Animations;
		/// <summary></summary>
		private Animation[] TilesetAnimations;
		/// <summary></summary>
		private List<Animation>[] DeathAnimations;
		///// <summary>Number of players that animations exist for</summary>
		//private int PlayerCount;
		/// <summary>Player Colors</summary>
		private ColorRemapInfo[] RemapInfo;

		#region Constructor
		/// <summary>
		/// Initialize an AnimationList for the specified
		/// number of players
		/// </summary>
		/// <param name="remapInfo"></param>
		public AnimationList(ColorRemapInfo[] remapInfo)
		{
			//PlayerCount = playerCount;
			Animations = new Animation[SimpleAnimationIndex.Count
									+ PowerupAnimationIndex.Count
									+ 4 * DirectionAnimationIndex.Count
									+ 1/*playerCount*/ * (PlayerAnimationIndex.Count
											+ 4 * PlayerDirectionAnimationIndex.Count)];
			// this is enlarged automatically
			TilesetAnimations = new Animation[TilesetAnimationIndex.Count];
			DeathAnimations = new List<Animation>[1/*playerCount*/];
			for (int i = 0; i < 1/*playerCount*/; ++i)
				DeathAnimations[i] = new List<Animation>();
			RemapInfo = remapInfo;/* new ColorRemapInfo[playerCount];
			for (int i = 0; i < playerCount; ++i)
				RemapInfo[i] = PlayerColor(i);*/
		}
		#endregion

		/// <summary>
		/// 
		/// </summary>
		/// <param name="i"></param>
		/// <returns></returns>
		public Animation this[AnimationIndex i]
		{
			get
			{
				if (i is PlayerDeathAnimationIndex)
				{
					PlayerDeathAnimationIndex di = (PlayerDeathAnimationIndex)i;
					return DeathAnimations[di.Player][di.Type];
				}
				else if (i is TilesetAnimationIndex)
				{
					// TRYTRY: catch invalid tileset number?
					return TilesetAnimations[i.Value];
				}
				else
					return Animations[i.Value];
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="i"></param>
		/// <param name="ani"></param>
		public bool AddAnimation(AnimationIndex i, Animation ani)
		{
			//Console.WriteLine("Trying to load animation " + i.Value + ": " + i);
			if (i is PlayerDeathAnimationIndex)
			{
				for (int iPlayer = 0; iPlayer < 1/*PlayerCount*/; ++iPlayer)
				{
					DeathAnimations[iPlayer].Add(ani);
					ani.Remap(RemapInfo);
				}
			}
			else if (i is TilesetAnimationIndex)
			{
				// enlarge array as required
				if (i.Value >= TilesetAnimations.Length)
				{
					Animation[] newAnis = new Animation[i.Value + 1];
					Array.Copy(TilesetAnimations, newAnis, TilesetAnimations.Length);
					TilesetAnimations = newAnis;
				}

				if (TilesetAnimations[i.Value] != null)
					throw new InvalidOperationException("Cannot add existing animation " + i.Value + ": " + i);
				else
				{
					TilesetAnimations[i.Value] = ani;
					//Console.WriteLine("Successfully added " + i.Value + ": " + i);
				}
			}
			else if (i.Value >= Animations.Length)
				throw new InvalidOperationException("There is no space for animation " + i.Value + ": " + i + " - Animations.Length is " + Animations.Length);
			else if (Animations[i.Value] != null)
			{
				//throw new InvalidOperationException("Cannot add existing animation " + i.Value + ": " + i);
				Console.WriteLine("Not adding existing animation " + i.Value + ": " + i);
				return false;
			}
			else if (i is PlayerAnimationIndex || i is PlayerDirectionAnimationIndex)
			{
				Animations[i.Value] = ani;
				ani.Remap(RemapInfo);
			}
			else
			{
				//Console.WriteLine("Successfully added " + i.Value + ": " + i);
				Animations[i.Value] = ani;
			}
			return true;
		}

		/// <summary>
		/// Check whether all animations are properly loaded
		/// </summary>
		public void Check()
		{
			for (int i = 0; i < Animations.Length; ++i)
				if (Animations[i] == null)
					Console.WriteLine("Animation " + i + " has not been loaded!");
		}

		/// <summary>
		/// Load sprites for all animations that are supposed to be cached
		/// </summary>
		public void Cache(BomberStuff.Core.UserInterface.IDevice device, int nPlayers)
		{
			foreach (Animation ani in TilesetAnimations)
				if (ani.Cached)
				{
					//System.Console.WriteLine("Caching " + ani.Name);
					foreach (AnimationFrame frame in ani.Frames)
						if (frame.RemappedCopies == null)
							frame.GetSprite(device, ani.VideoMemory);
						else
							for (int iPlayer = 0; iPlayer < nPlayers; ++iPlayer)
								frame.GetSprite(device, ani.VideoMemory, iPlayer);

				}
			foreach (Animation ani in Animations)
				if (ani.Cached)
				{
					//System.Console.WriteLine("Caching " + ani.Name);
					foreach (AnimationFrame frame in ani.Frames)
						if (frame.RemappedCopies == null)
							frame.GetSprite(device, ani.VideoMemory);
						else
							for (int iPlayer = 0; iPlayer < nPlayers; ++iPlayer)
								frame.GetSprite(device, ani.VideoMemory, iPlayer);
						
				}
			foreach (Animation ani in DeathAnimations[0])
				if (ani.Cached)
				{
					//System.Console.WriteLine("Caching " + ani.Name);
					foreach (AnimationFrame frame in ani.Frames)
						if (frame.RemappedCopies == null)
							frame.GetSprite(device, ani.VideoMemory);
						else
							for (int iPlayer = 0; iPlayer < nPlayers; ++iPlayer)
								frame.GetSprite(device, ani.VideoMemory, iPlayer);

				}
		}

		#region IDisposable implementation
		/// <summary>
		/// Frees the object's unmanaged and optionally managed
		/// resources
		/// </summary>
		/// <param name="disposing">
		/// <c>true</c> to free managed resources
		/// </param>
		protected virtual void Dispose(bool disposing)
		{
			if (disposing)
			{

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
		/// Finalizer. Frees the object's unmanaged resources
		/// </summary>
		~AnimationList()
		{
			Dispose(false);
		}
		#endregion
	}
}