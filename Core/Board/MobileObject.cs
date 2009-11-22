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
		public float X { get { return Position.X; } }
		/// <summary></summary>
		public float Y { get { return Position.Y; } }

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

		private AnimationIndex ai;
		/// <summary></summary>
		protected AnimationIndex Animation
		{
			get { return ai; }
			set
			{
				if (this is Player)
					System.Console.WriteLine("Setting " + this + " animation to " + value);
				ai = value;
			}
		}
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
		public ISprite GetSprite(AnimationList aniList, IDevice device)
		{
			if (PlayerIndex == -1)
				return aniList[Animation].GetSprite(device, AnimationState);
			else
				return aniList[Animation].GetSprite(device, AnimationState, PlayerIndex);
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
		public void Move(Board board, int ticks)
		{
			float newX = X;
			float newY = Y;

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
				if (Math.Abs(m_SpeedX) >= Math.Abs(m_SpeedY))
				{
					newX = X + SpeedX;
					if (IsCollision(board, newX, newY))
						m_SpeedX = 0.0f;

					newY = Y + SpeedY;
					if (IsCollision(board, newX, newY))
						m_SpeedY = 0.0f;
				}
				else
				{
					newY = Y + SpeedY;
					if (IsCollision(board, newX, newY))
						m_SpeedY = 0.0f;

					newX = X + SpeedX;
					if (IsCollision(board, newX, newY))
						m_SpeedX = 0.0f;
				}
				
			}

			Position = new PointF(newX, newY);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="board"></param>
		/// <param name="newX"></param>
		/// <param name="newY"></param>
		protected bool IsCollision(Board board, float newX, float newY)
		{
			foreach (MobileObject obj in board.Items)
			{
				if (new RectangleF(new PointF(newX, newY), Size).IntersectsWith(new RectangleF(obj.Position, obj.Size))
						&& !new RectangleF(Position, Size).IntersectsWith(new RectangleF(obj.Position, obj.Size)))
				{
					Console.WriteLine("Possible collision between " + this + " and " + obj);
					if (Collide(obj))
					{
						return true;
					}
				}

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
