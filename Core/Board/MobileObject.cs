//
// MobileObject.cs - MobileObject class
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

using System;

using BomberStuff.Core.Animation;
using BomberStuff.Core.UserInterface;
using BomberStuff.Core.Drawing;


namespace BomberStuff.Core
{
	/// <summary>
	/// An item on the board: a player, wall, powerup, bomb or extra
	/// </summary>
	public abstract class MobileObject
	{
		#region Basic properties (Position, Size, Player)
		/// <summary>
		/// Coordinates of the object's top left corner
		/// </summary>
		/// <remarks>
		/// Note that these are game, not animation coordinates.
		/// A player's "Position" will thus be somewhere around
		/// his waist.
		/// </remarks>
		public PointF Position
		{
			get { return new PointF(X, Y); }
			protected set
			{
				X = value.X;
				Y = value.Y;
			}
		}

		/// <summary>
		/// Size of the object on the board
		/// </summary>
		/// <remarks>
		/// Note that these are game, not animation coordinates.
		/// A player's size will thus be (1, 1), even though it
		/// is probably displayed larger.
		/// </remarks>
		public readonly SizeF Size;

		/// <summary>
		/// The object's Left coordinate
		/// </summary>
		public float X
		{
			get;
			protected set;
		}

		/// <summary>
		/// The object's top coordinate
		/// </summary>
		public float Y
		{
			get;
			protected set;
		}

		/// <summary>
		/// The object's width
		/// </summary>
		public float Width { get { return Size.Width; } }

		/// <summary>
		/// The object's height
		/// </summary>
		public float Height { get { return Size.Height; } }

		/// <summary>
		/// The rectangle that the object spans on the board
		/// </summary>
		public RectangleF Bounds
		{
			get
			{
				return new RectangleF(X, Y, Size.Width, Size.Height);
			}
		}

		/// <summary>
		/// The player that this object belongs to (or represents), or -1 if none
		/// </summary>
		/// <remarks>
		/// This is used to identify players and to choose the right color for
		/// drawing them and their items
		/// </remarks>
		public readonly int PlayerIndex;
		#endregion

		#region Constructors
		/// <summary>
		/// Initialize a new board object with the given size at the given position
		/// and belonging to the specified player
		/// </summary>
		/// <param name="x">Left coordinate</param>
		/// <param name="y">Top coordinate</param>
		/// <param name="width">Object width</param>
		/// <param name="height">Object height</param>
		/// <param name="player">
		/// Index ([0, PlayerCount[) of the player that the object belongs
		/// to (or represents), or -1 for player-independent objects
		/// </param>
		protected MobileObject(float x, float y, float width, float height, int player)
		{
			Position = new PointF(x, y);
			Size = new SizeF(width, height);
			PlayerIndex = player;
			SpeedX = 0.125f;
			SpeedY = 0.125f;
		}

		/// <summary>
		/// Initialize a new board object with the given size at the given position
		/// </summary>
		/// <param name="x">Left coordinate</param>
		/// <param name="y">Top coordinate</param>
		/// <param name="width">Object width</param>
		/// <param name="height">Object height</param>
		protected MobileObject(float x, float y, float width, float height)
			: this(x, y, width, height, -1) { }
		#endregion

		#region Animation
		/// <summary>
		/// The animation that the object is visualized as
		/// </summary>
		protected AnimationIndex Animation;

		/// <summary>
		/// The current state (frame number) of the animation
		/// </summary>
		protected int AnimationState;

		/// <summary>
		/// Whether the animation loops
		/// </summary>
		/// <value>
		/// <c>true</c> if the animation should be looped endlessly,
		/// <c>false</c> if the animation should be played only once
		/// </value>
		/// <remarks>
		/// An object with a non-looping animation is destroyed
		/// in <see cref="Animate" /> when the animation is finished
		/// </remarks>
		protected bool Loop = true;

		/// <summary>
		/// Increase the animation state
		/// </summary>
		/// <param name="aniList">The game's animation list</param>
		/// <param name="ticks">
		/// Number of animation ticks that have passed
		/// </param>
		/// <returns>
		/// <c>true</c> if the animation should go on,
		/// <c>false</c> if the object should be removed from the board
		/// </returns>
		public bool Animate(AnimationList aniList, int ticks)
		{
			AnimationState += ticks;

			int nStates = aniList[Animation].Frames.Length;

			if (AnimationState >= nStates)
				if (Loop)
					AnimationState %= nStates;
				else
				{
					// non-looping animation is done. Assume the last
					// valid state and signal that the object
					// should be removed
					AnimationState = nStates - 1;
					return false;
				}

			return true;
		}

		/// <summary>
		/// Get a sprite to draw for this object on the specified device
		/// </summary>
		/// <param name="aniList">The game's animation list</param>
		/// <param name="device">The device to get a sprite for</param>
		/// <returns>The sprite the draw</returns>
		public virtual ISprite GetSprite(AnimationList aniList, IDevice device)
		{
			// TRYTRY: this seems to be quite a performance penalty. Verify.
			//         Player-related objects currently override this instead
			//         of relying on this check.
			//if (PlayerIndex == -1)
				return aniList[Animation].GetSprite(device, AnimationState);
			//else
			//	return aniList[Animation].GetSprite(device, AnimationState, PlayerIndex);
		}

		/// <summary>
		/// Get the current animation frame's offset
		/// </summary>
		/// <returns>
		/// The coordinates of the object's <see cref="Position" />
		/// relative to the top left corner of the current animation
		/// frame
		/// </returns>
		public virtual SizeF GetOffset(AnimationList aniList)
		{
			return aniList[Animation].GetOffset(AnimationState);
		}

		/// <summary>
		/// Get the current animation frame's size
		/// </summary>
		/// <param name="aniList">The game's animation list</param>
		/// <returns>
		/// The size of the object's current animation frame
		/// </returns>
		public SizeF GetSpriteSize(AnimationList aniList)
		{
			return aniList[Animation].GetSpriteSize(AnimationState);
		}
		#endregion

		/// <summary>
		/// Handle a number of game ticks
		/// </summary>
		/// <param name="board">
		/// The board that the object is on
		/// </param>
		/// <param name="ticks">
		/// The number of game ticks that have passed
		/// </param>
		/// <remarks>
		/// All MobileObject's implementation does here is moving.
		/// Derived classes should override this function if they
		/// need to handle game ticks. base.Tick() must be called
		/// in the overridden function in this case.
		/// </remarks>
		public virtual void Tick(Board board, int ticks)
		{
			// Move the object. Nothing else to do here.
			Move(board, ticks);
		}

		#region General movement fields and methods
		/// <summary>
		/// The object's horizontal speed in fields per game tick
		/// </summary>
		/// <value>
		/// A positive value that represents the object's
		/// "potential" speed if it were moving
		/// </value>
		/// <seealso cref="SpeedY" />
		protected float SpeedX;

		/// <summary>
		/// The object's vertical speed in fields per game tick
		/// </summary>
		/// <value>
		/// A positive value that represents the object's
		/// "potential" speed if it were moving
		/// </value>
		/// <seealso cref="SpeedX" />
		protected float SpeedY;

		/// <summary>
		/// The direction in which the object is primarily moving
		/// </summary>
		public Directions Direction
		{
			get;
			private set;
		}

		/// <summary>
		/// The secondary direction in which the object is moving
		/// </summary>
		/// <remarks>
		/// If the object hits anything and has to stop in the
		/// primary direction, this is swapped with Direction
		/// </remarks>
		public Directions SecondaryDirection
		{
			get;
			private set;
		}

		/// <summary>
		/// 
		/// </summary>
		public bool Moving
		{
			get;
			private set;
		}

		/// <summary>
		/// Sets the primary and secondary direction of the object and
		/// whether it is moving
		/// </summary>
		/// <param name="primary">Primary direction</param>
		/// <param name="secondary">Secondary direction</param>
		/// <param name="moving">
		/// <c>true</c> if the object is moving, <c>false</c> otherwise
		/// </param>
		/// <remarks>
		/// Derived classes that require different animations depending on
		/// whether the object moves or on the movement direction should override
		/// this. base.SetMoveState must be called in that case.
		/// </remarks>
		public virtual void SetMoveState(Directions primary, Directions secondary, bool moving)
		{
			Direction = primary;
			SecondaryDirection = secondary;
			Moving = moving;
		}

		/// <summary>
		/// Sets the direction of the object and whether it is moving
		/// </summary>
		/// <param name="direction">Object's direction</param>
		/// <param name="moving">
		/// <c>true</c> if the object is moving, <c>false</c> otherwise
		/// </param>
		/// <remarks>
		/// The object's secondary direction is set to
		/// <paramref name="direction"/> as well to imply that there is no
		/// secondary direction
		/// </remarks>
		public void SetMoveState(Directions direction, bool moving)
		{
			SetMoveState(direction, direction, moving);
		}
		#endregion

		#region Internal movement handling
		/// <summary>
		/// 
		/// </summary>
		/// <param name="board"></param>
		/// <param name="ticks"></param>
		private void Move(Board board, int ticks)
		{
			// move one 'step' per tick
			while (Moving && ticks-- > 0)
			{
				float dX = DirectionUtilities.GetX(Direction) * SpeedX;
				float dY = DirectionUtilities.GetY(Direction) * SpeedY;
			
				if (dX != 0)
				{
					// moving in X direction
					if (!AdjustY(board, X + dX))
						X = MoveX(board, X + dX);
				}
				else /* if (DirectionUtilities.GetY(Direction) != 0) */
				{
					// moving in Y direction
					if (!AdjustX(board, Y + dY))
						Y = MoveY(board, Y + dY);
				}
			}
		}

		private bool AdjustY(Board board, float newX)
		{
			float y = (float)Math.Floor(Y);
			float dY = Y - y;

			float newY = TryAdjust(board, newX, y, newX, y + 1f, dY);

			// way is blocked or no adjustment needed
			if (newY == 0f)
				return false;

			Y += newY * SpeedY;

			return true;
		}

		private bool AdjustX(Board board, float newY)
		{
			float x = (float)Math.Floor(X);
			float dX = X - x;

			float newX = TryAdjust(board, x, newY, x + 1f, newY, dX);

			// way is blocked or no adjustment needed
			if (newX == 0f)
				return false;

			X += newX * SpeedX;

			return true;
		}

		//
		// +---+---+
		// |   |   |
		// | 1 | 2 |
		// |   |   |
		// +---+---+
		// |   |   |
		// | 3 | 4 |
		// |   |   |
		// +---+---+
		//

		private float TryAdjust(Board board, float firstX, float firstY, float secondX, float secondY, float delta)
		{
			// exactly on the field. No adjustment needed
			if (delta == 0f)
				return 0f;

			// if we're in the top half of the field (on field 1),
			// try to move towards field 2 first
			if (delta < 0.5f)
			{
				if (!IsCollision(board, firstX, firstY))
					return -1f; // top is okay

				else if (!IsCollision(board, secondX, secondY))
					return +1f; // bottom is okay

				else
					return 0f;
			}
			// otherwise check field 3 first
			else /* if (delta >= 0.5f) */
			{
				if (!IsCollision(board, secondX, secondY))
					return +1f; // bottom is okay

				else if (!IsCollision(board, firstX, firstY))
					return -1f; // top is okay

				else
					return 0f;
			}
		}

		private void SwapDirections()
		{
			if (Direction != SecondaryDirection)
				SetMoveState(SecondaryDirection, Direction, Moving);
		}

		/// <summary>
		/// Try to move the object in X direction
		/// </summary>
		/// <param name="board"></param>
		/// <param name="newX"></param>
		/// <returns>the new X coordinate</returns>
		private float MoveX(Board board, float newX)
		{
			// make sure the new position is still on the board
			if (newX < 0.0f)
			{
				BorderCollide();
				SwapDirections();
				newX = 0f;
			}
			else if (newX > board.Width - 1f)
			{
				BorderCollide();
				SwapDirections();
				newX = board.Width - 1f;
			}

			// check if the new position is blocked
			if (IsCollision(board, newX, Y))
			{
				SwapDirections();
				// TODO: set us as close as possible to the offending object
				newX = X;
			}

			return newX;
		}

		/// <summary>
		/// Try to move the object in Y direction
		/// </summary>
		/// <param name="board"></param>
		/// <param name="newY"></param>
		/// <returns></returns>
		private float MoveY(Board board, float newY)
		{
			// make sure the new position is still on the board
			if (newY < 0.0f)
			{
				BorderCollide();
				SwapDirections();
				newY = 0.0f;
			}
			else if (newY > board.Height - 1.0f)
			{
				BorderCollide();
				SwapDirections();
				newY = board.Height - 1.0f;
			}

			// check if the new position is blocked
			if (IsCollision(board, X, newY))
			{
				SwapDirections();
				// TODO: set us as close as possible to the offending object
				newY = Y;
			}

			return newY;
		}
		#endregion

		#region Collision testing and handling
		/// <summary>
		/// 
		/// </summary>
		/// <param name="board"></param>
		/// <param name="newX"></param>
		/// <param name="newY"></param>
		private bool IsCollision(Board board, float newX, float newY)
		{
			RectangleF newRect = new RectangleF(new PointF(newX, newY), Size);
			RectangleF oldRect = Bounds;

			foreach (MobileObject obj in board.Items)
			{
				RectangleF objRect = new RectangleF(obj.Position, obj.Size);

				if (newRect.IntersectsWith(objRect) && !oldRect.IntersectsWith(objRect))
					//if (Collide(obj))
					//	return true;
				// this means if there's a player (you can walk through) AND a
				// stone (can't walk through) on the field, you might be able to
				// go there. As this shouldn't happen, we use this (because it saves
				// the rest of the loop)
					return Collide(obj);
			}

			return false;
		}

		/// <summary>
		/// When overridden in a derived class, handles collision with
		/// another object.
		/// </summary>
		/// <param name="other">
		/// the object with which a collision is occuring
		/// </param>
		/// <returns>
		/// <c>false</c> if the object can move on,
		/// <c>true</c> if the collision caused the object to stop
		/// </returns>
		protected abstract bool Collide(MobileObject other);

		/// <summary>
		/// When overridden in a derived class, handles collision with the
		/// board border. Any required actions, such as bouncing, should
		/// be initiated.
		/// </summary>
		protected abstract void BorderCollide();
		#endregion
	}
}
