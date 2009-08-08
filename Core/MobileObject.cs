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
		protected MobileObject(float x, float y, float width, float height)
		{
			Position.X = x;
			Position.Y = y;
			Size.Width = width;
			Size.Height = height;
		}

		// TODO: Draw
		/// <summary></summary>
		protected PointF Origin;
		//protected AnimationIndex Animation;
		/// <summary></summary>
		protected int AnimationState;
		/// <summary></summary>
		protected bool Loop;


		/// <summary>
		/// Increase the animation state
		/// </summary>
		/// <param name="ticks">
		/// Number of time units that have passed
		/// </param>
		/// <returns>
		/// <c>true</c> if the animation should go on,
		/// <c>false</c> if the object should be removed from the board
		/// </returns>
		public bool Animate(/*AnimationList list, */int ticks)
		{
			
			return true;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public ISprite GetSprite()
		{
			// TODO: GetSprite()
			return Game.dings;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public SizeF GetOffset()
		{
			// TODO: GetOffset()
			return new SizeF();
		}


		
		/// <summary>The object's speed</summary>
		/// <seealso cref="SpeedX" />
		/// <seealso cref="SpeedY" />
		protected float SpeedX, SpeedY;

		// TRYTRY: Do we actually need public speed querying?
		///// <summary>
		///// The object's speed in X direction in fields per tick
		///// </summary>
		//public float SpeedX { get { return m_SpeedX; } }
		///// <summary>
		///// The object's speed in Y direction in fields per tick
		///// </summary>
		//public float SpeedY { get { return m_SpeedY; } }

		/// <summary>
		/// 
		/// </summary>
		/// <param name="board"></param>
		/// <param name="ticks"></param>
		public void Move(Board board, int ticks)
		{
			float newX = X;
			float newY = Y;

			while (ticks-- > 0)
			{
                if (Math.Abs(SpeedX) >= Math.Abs(SpeedY))
                {
                    newX = X + SpeedX;
                    if (IsCollision(board, newX, newY))
                        SpeedX = 0.0f;

                    newY = Y + SpeedX;
                    if (IsCollision(board, newX, newY))
                        SpeedY = 0.0f;
                }
                else
                {
                    newY = Y + SpeedX;
                    if (IsCollision(board, newX, newY))
                        SpeedY = 0.0f;

                    newX = X + SpeedX;
                    if (IsCollision(board, newX, newY))
                        SpeedX = 0.0f;
                }
				
			}
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
