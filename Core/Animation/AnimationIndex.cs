//
// AnimationIndex.cs - Animation indices
//
// Copyright © 2009-2010  Thomas Faber
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

namespace BomberStuff.Core.Animation
{
	/// <summary>
	/// 
	/// </summary>
	public abstract class AnimationIndex
	{
		/// <summary>
		/// 
		/// </summary>
		public readonly int Value;

		/// <summary>
		/// 
		/// </summary>
		/// <param name="value"></param>
		protected AnimationIndex(int value)
		{
			Value = value;
		}
	}


	/// <summary>
	/// An animation index representing a simple animation, that is,
	/// not direction or player specific, and not a powerup
	/// </summary>
	public sealed class SimpleAnimationIndex : AnimationIndex
	{
		/// <summary>
		/// 
		/// </summary>
		public enum Types
		{
			/*/// <summary>A solid stone</summary>
			Stone,
			/// <summary>A penetrable wall</summary>
			Wall,
			/// <summary>An exploding wall</summary>
			ExplodingWall,*/
			/// <summary>The shadow of a bomb dude</summary>
			DudeShadow,
			/// <summary>The numeric font used to display time</summary>
			NumericFont,
			/// <summary>A trampoline extra</summary>
			Trampoline,
			/// <summary>A warp hole extra</summary>
			Warphole,

			/// <summary></summary>
			Last = Warphole
		}

		/// <summary>Number of animations of this type</summary>
		public const int Count = (int)Types.Last + 1;
		/// <summary>Index of the first animation of this type</summary>
		public const int First = 0;

		/// <summary></summary>
		private readonly Types Type;

		/// <summary>
		/// Initialize a new SimpleAnimationIndex of the specified type
		/// </summary>
		/// <param name="type"></param>
		public SimpleAnimationIndex(Types type)
			: base(First + (int)type)
		{
			Type = type;
		}

		/// <summary>
		/// Returns a string representation of the object
		/// </summary>
		/// <returns>a string representing the object</returns>
		public override string ToString()
		{
			return "SimpleAnimationIndex(" + Type + ")";
		}
	}


	/// <summary>
	/// An animation index representing a powerup
	/// </summary>
	public sealed class PowerupAnimationIndex : AnimationIndex
	{
		/// <summary>Number of animations of this type</summary>
		public const int Count = (int)Powerup.Types.Last + 1;
		/// <summary>Index of the first animation of this type</summary>
		public const int First = SimpleAnimationIndex.First + SimpleAnimationIndex.Count;

		/// <summary></summary>
		private readonly Powerup.Types Type;

		/// <summary>
		/// 
		/// </summary>
		/// <param name="type"></param>
		public PowerupAnimationIndex(Powerup.Types type)
			: base(First + (int)type)
		{
			Type = type;
		}

		/// <summary>
		/// Returns a string representation of the object
		/// </summary>
		/// <returns>a string representing the object</returns>
		public override string ToString()
		{
			return "PowerupAnimationIndex(" + Type + ")";
		}
	}


	/// <summary>
	/// An animation index representing an animation that exists
	/// once per direction
	/// </summary>
	public sealed class DirectionAnimationIndex : AnimationIndex
	{
		/// <summary>
		/// 
		/// </summary>
		public enum Types
		{
			/// <summary>Arrow extra</summary>
			Arrow,
			/// <summary>Conveyor belt extra</summary>
			ConveyorBelt,

			/// <summary></summary>
			Last = ConveyorBelt
		}

		/// <summary>Number of animations of this type</summary>
		public const int Count = (int)Types.Last + 1;
		/// <summary>Index of the first animation of this type</summary>
		public const int First = PowerupAnimationIndex.First + PowerupAnimationIndex.Count;

		/// <summary>The animation's direction</summary>
		private readonly Directions Direction;

		/// <summary></summary>
		private readonly Types Type;

		/// <summary>
		/// Initialize a new DirectionAnimationIndex of the specified
		/// type and direction
		/// </summary>
		/// <param name="type"></param>
		/// <param name="direction"></param>
		public DirectionAnimationIndex(Types type, Directions direction)
			: base(First + (int)direction * Count + (int)type)
		{
			Type = type;
			Direction = direction;
		}

		/// <summary>
		/// Returns a string representation of the object
		/// </summary>
		/// <returns>a string representing the object</returns>
		public override string ToString()
		{
			return "DirectionAnimationIndex(" + Direction + ", " + Type + ")";
		}
	}


	/// <summary>
	/// An animation index representing an animation that exists once
	/// per player (color)
	/// </summary>
	public sealed class PlayerAnimationIndex : AnimationIndex
	{
		/// <summary>
		/// The types of player specific animations
		/// </summary>
		public enum Types
		{
			/// <summary>A regular bomb</summary>
			BombRegular,
			/// <summary>A jelly bomb</summary>
			BombJelly,
			/// <summary>A trigger bomb</summary>
			BombTrigger,
			/// <summary>A dud bomb</summary>
			BombDud,
			/// <summary>The center of a bomb explosion</summary>
			ExplosionCenter,

			/// <summary></summary>
			Last = ExplosionCenter
		}

		/// <summary>Number of animations of this type</summary>
		public const int Count = (int)Types.Last + 1;
		/// <summary>Index of the first animation of this type</summary>
		public const int First = DirectionAnimationIndex.First + 4 * DirectionAnimationIndex.Count;

		/// <summary></summary>
		private readonly Types Type;

		/// <summary>
		/// Initialize a new PlayerAnimationIndex of the specified type
		/// </summary>
		/// <param name="type"></param>
		public PlayerAnimationIndex(Types type)
			: base(First + (int)type)
		{
			Type = type;
		}

		/// <summary>
		/// Returns a string representation of the object
		/// </summary>
		/// <returns>a string representing the object</returns>
		public override string ToString()
		{
			return "PlayerAnimationIndex(" + Type + ")";
		}
	}


	/// <summary>
	/// An animation index representing an animation that exists once
	/// for each player (color) and direction
	/// </summary>
	public sealed class PlayerDirectionAnimationIndex : AnimationIndex
	{
		/// <summary>
		/// The types of player and direction specific animations
		/// </summary>
		public enum Types
		{
			/// <summary>A standing bomb dude</summary>
			Stand,
			/// <summary>A walking bomb dude</summary>
			Walk,
			/// <summary>A kicking bomb dude</summary>
			Kick,
			/// <summary>A punching bomb dude</summary>
			Punch,
			/// <summary>A dude picking up a bomb</summary>
			Pickup,
			/// <summary>A bomb explosion</summary>
			ExplosionMid,
			/// <summary>The tip of a bomb explosion</summary>
			ExplosionTip,

			/// <summary></summary>
			Last = ExplosionTip
		}

		/// <summary>Number of animations of this type</summary>
		public const int Count = (int)Types.Last + 1;

		/// <summary>The animation's direction</summary>
		private readonly Directions Direction;

		/// <summary>The type of this animation</summary>
		private readonly Types Type;

		/// <summary>
		/// Initialize a PlayerDirectionAnimationIndex of the specified
		/// type and direction for the specified player
		/// </summary>
		/// <param name="type"></param>
		/// <param name="direction"></param>
		public PlayerDirectionAnimationIndex(Types type, Directions direction)
			: base(PlayerAnimationIndex.First + PlayerAnimationIndex.Count
						+ Count * (int)direction + (int)type)
		{
			Type = type;
			Direction = direction;
		}

		/// <summary>
		/// Returns a string representation of the object
		/// </summary>
		/// <returns>a string representing the object</returns>
		public override string ToString()
		{
			return "PlayerDirectionAnimationIndex(" + Direction + ", " + Type + ")";
		}
	}

	/// <summary>
	/// An animation index representing a tileset animation, that is,
	/// an animation that exists once per tileset
	/// </summary>
	public sealed class TilesetAnimationIndex : AnimationIndex
	{
		/// <summary>
		/// 
		/// </summary>
		public enum Types
		{
			/// <summary>A solid stone</summary>
			Stone,
			/// <summary>A penetrable wall</summary>
			Wall,
			/// <summary>An exploding wall</summary>
			ExplodingWall,

			/// <summary></summary>
			Last = ExplodingWall
		}

		/// <summary>Number of animations of this type</summary>
		public const int Count = (int)Types.Last + 1;

		/// <summary></summary>
		private readonly Types Type;

		/// <summary></summary>
		private readonly int Tileset;

		/// <summary>
		/// Initialize a new SimpleAnimationIndex of the specified type
		/// </summary>
		/// <param name="type"></param>
		/// <param name="tileset"></param>
		public TilesetAnimationIndex(Types type, int tileset)
			: base(tileset * Count + (int)type)
		{
			Type = type;
			Tileset = tileset;
		}

		/// <summary>
		/// Returns a string representation of the object
		/// </summary>
		/// <returns>a string representing the object</returns>
		public override string ToString()
		{
			return "TilesetAnimationIndex(Tileset " + Tileset + ", " + Type + ")";
		}
	}

	/// <summary>
	/// An animation index representing a player death animation
	/// </summary>
	public sealed class PlayerDeathAnimationIndex : AnimationIndex
	{
		/// <summary></summary>
		public readonly int Type;

		/// <summary>
		/// Initialize a new PlayerAnimationIndex of the specified
		/// type and for the specified player
		/// </summary>
		/// <param name="type"></param>
		public PlayerDeathAnimationIndex(int type)
			: base(-1)
		{
			Type = type;
		}

		/// <summary>
		/// Returns a string representation of the object
		/// </summary>
		/// <returns>a string representing the object</returns>
		public override string ToString()
		{
			return "PlayerDeatAnimationIndex(" + Type + ")";
		}
	}
}
