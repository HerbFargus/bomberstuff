//
// MobileObject.cs - MobileObject class
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
		/// <summary>
		/// 
		/// </summary>
		public PointF Position;

		/// <summary>
		/// 
		/// </summary>
		public readonly SizeF Size;

		/// <summary></summary>
		public float X
		{
			get { return Position.X; }
			protected set { Position.X = value; }
		}
		/// <summary></summary>
		public float Y
		{
			get { return Position.Y; }
			protected set { Position.Y = value; }
		}

		/// <summary></summary>
		public float Width { get { return Size.Width; } }
		/// <summary></summary>
		public float Height { get { return Size.Height; } }

		/// <summary>
		/// 
		/// </summary>
		public RectangleF Bounds { get { return new RectangleF(Position, Size); } }

		/// <summary>
		/// 
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <param name="width"></param>
		/// <param name="height"></param>
		/// <param name="player"></param>
		protected MobileObject(float x, float y, float width, float height, int player)
		{
			Position.X = x;
			Position.Y = y;
			Size.Width = width;
			Size.Height = height;
			PlayerIndex = player;
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <param name="width"></param>
		/// <param name="height"></param>
		protected MobileObject(float x, float y, float width, float height)
			: this(x, y, width, height, -1) { }

		/// <summary></summary>
		protected AnimationIndex Animation;
		/// <summary></summary>
		protected int AnimationState;
		/// <summary></summary>
		protected bool Loop = true;
		/// <summary></summary>
		public readonly int PlayerIndex;


		/// <summary>
		/// Increase the animation state
		/// </summary>
		/// <param name="aniList"></param>
		/// <param name="ticks">
		/// Number of time units that have passed
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
		/// 
		/// </summary>
		/// <param name="aniList"></param>
		/// <param name="device"></param>
		/// <returns></returns>
		public virtual ISprite GetSprite(AnimationList aniList, IDevice device)
		{
			//if (PlayerIndex == -1)
				return aniList[Animation].GetSprite(device, AnimationState);
			//else
			//	return aniList[Animation].GetSprite(device, AnimationState, PlayerIndex);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public virtual SizeF GetOffset(AnimationList aniList)
		{
			//Console.Write("Getting offset for " + Animation);
			return aniList[Animation].GetOffset(AnimationState);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="aniList"></param>
		/// <returns></returns>
		public SizeF GetSpriteSize(AnimationList aniList)
		{
			return aniList[Animation].GetSpriteSize(AnimationState);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="direction"></param>
		/// <param name="speed"></param>
		public virtual void SetMoveState(Directions direction, float speed)
		{
			m_SpeedX = DirectionUtilities.GetX(direction) * speed;
			m_SpeedY = DirectionUtilities.GetY(direction) * speed;
		}

		
		/// <summary>The object's speed</summary>
		/// <seealso cref="SpeedX" />
		/// <seealso cref="SpeedY" />
		protected float m_SpeedX, m_SpeedY;

		// TRYTRY: Do we actually need public speed querying?
		/// <summary>
		/// The object's speed in X direction in fields per tick
		/// </summary>
		public float SpeedX { get { return m_SpeedX; } }
		/// <summary>
		/// The object's speed in Y direction in fields per tick
		/// </summary>
		public float SpeedY { get { return m_SpeedY; } }

		/// <summary>
		/// 
		/// </summary>
		/// <param name="board"></param>
		/// <param name="ticks"></param>
		public virtual void Tick(Board board, int ticks)
		{
			Move(board, ticks);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="board"></param>
		/// <param name="ticks"></param>
		protected void Move(Board board, int ticks)
		{
			if (m_SpeedX == 0 && m_SpeedY == 0)
			{
				//System.Console.WriteLine(this + " not moving");
				return;
			}

			if (ticks == 0)
				return;

			//System.Console.WriteLine("Moving " + this + " with speed " + SpeedX + ", " + SpeedY + " for " + ticks + " ticks");

			while (ticks-- > 0)
			{
				// move. the direction with the higher speed goes first
				if (Math.Abs(m_SpeedX) >= Math.Abs(m_SpeedY))
				{
					X = MoveX(board);
					Y = MoveY(board);
				}
				else
				{
					Y = MoveY(board);
					X = MoveX(board);
				}
			}

			//System.Console.WriteLine(this + " at ({0}, {1}), speed ({2}, {3})", X, Y, SpeedX, SpeedY);
		}

		/// <summary>
		/// Try to move the object in X direction
		/// </summary>
		/// <param name="board"></param>
		/// <returns>the new X coordinate</returns>
		protected float MoveX(Board board)
		{
			float newX = X + SpeedX;
			
			// make sure the new position is still on the board
			if (newX < 0.0f)
			{
				BorderCollide();
				m_SpeedX = 0.0f;
				newX = 0.0f;
			}
			else if (newX > board.Width - 1.0f)
			{
				BorderCollide();
				m_SpeedX = 0.0f;
				newX = board.Width - 1.0f;
			}

			// check if the new position is blocked
			if (IsCollision(board, newX, Y))
			{
				//m_SpeedX = 0.0f;
				newX = X;
			}

			return newX;
		}

		/// <summary>
		/// Try to move the object in Y direction
		/// </summary>
		/// <param name="board"></param>
		/// <returns></returns>
		protected float MoveY(Board board)
		{
			float newY = Y + SpeedY;

			// make sure the new position is still on the board
			if (newY < 0.0f)
			{
				BorderCollide();
				m_SpeedY = 0.0f;
				newY = 0.0f;
			}
			else if (newY > board.Height - 1.0f)
			{
				BorderCollide();
				m_SpeedY = 0.0f;
				newY = board.Height - 1.0f;
			}

			// check if the new position is blocked
			if (IsCollision(board, X, newY))
			{
				//m_SpeedY = 0.0f;
				newY = Y;
			}

			return newY;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="board"></param>
		/// <param name="newX"></param>
		/// <param name="newY"></param>
		protected bool IsCollision(Board board, float newX, float newY)
		{
			RectangleF newRect = new RectangleF(new PointF(newX, newY), Size);
			RectangleF oldRect = new RectangleF(Position, Size);

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
	}
}
