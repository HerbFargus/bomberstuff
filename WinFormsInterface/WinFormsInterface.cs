//
// Interface.cs - Interface class
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
using System.Drawing;
using System.Windows.Forms;

using BomberStuff.Core.UserInterface;

namespace BomberStuff.WinFormsInterface
{
	/// <summary>
	/// Bomber Stuff user interface implemented using System.Windows.Forms
	/// with GDI+ graphics
	/// </summary>
	/// <remarks>
	/// TODO: Add keyboard input
	/// </remarks>
	public class WinFormsInterface : IUserInterface
	{
		/// <summary>The underlying Form</summary>
		protected Form Form;

		/// <summary>The Form's Graphics object during drawing</summary>
		protected Graphics Graphics;

		#region Events
		/// <summary></summary>
		public event EventHandler<LoadSpritesEventArgs> LoadSprites;
		/// <summary></summary>
		public event EventHandler<RenderEventArgs> Render;

		/// <summary>
		/// 
		/// </summary>
		/// <param name="e"></param>
		protected virtual void OnLoadSprites(LoadSpritesEventArgs e)
		{
			if (LoadSprites != null)
				LoadSprites(this, e);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="e"></param>
		protected virtual void OnRender(RenderEventArgs e)
		{
			if (Render != null)
				Render(this, e);
		}
		#endregion

		/// <summary></summary>
		public bool CacheAllSprites { get { return true; } }

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		bool IUserInterface.Initialize()
		{
			OnLoadSprites(new LoadSpritesEventArgs(new Device()));
			Form = new BomberForm();
			
			Form.ClientSize = new Size(640, 480);
			Form.Text = "Bomber Stuff, WinForms interface";
			Form.Paint += Form_OnPaint;
			return true;
		}

		/// <summary>
		/// 
		/// </summary>
		public void MainLoop()
		{
			Application.Run(Form);
		}

		/// <summary>
		/// 
		/// </summary>
		public void Terminate()
		{
			Application.Exit();
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="sprite"></param>
		/// <param name="position"></param>
		/// <param name="size"></param>
		/// <param name="color"></param>
		public void Draw(ISprite sprite, BomberStuff.Core.Drawing.PointF position, BomberStuff.Core.Drawing.SizeF size, System.Drawing.Color color)
		{
			Sprite s = (Sprite)sprite;
			Rectangle src, dest;

			float w = Form.ClientSize.Width;
			float h = Form.ClientSize.Height;
			src = new Rectangle(0, 0, s.Bitmap.Width, s.Bitmap.Height);
			dest = new Rectangle((int)Math.Round(w * position.X),
								(int)Math.Round(h * position.Y),
								(int)Math.Round(w * size.Width),
								(int)Math.Round(h * size.Height));
			
			Graphics.DrawImage(s.Bitmap, dest, src, GraphicsUnit.Pixel);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="text"></param>
		/// <param name="position"></param>
		/// <param name="color"></param>
		public void Draw(string text, BomberStuff.Core.Drawing.PointF position, System.Drawing.Color color)
		{
			int x = (int)(position.X * Form.ClientSize.Width);
			int y = (int)(position.Y * Form.ClientSize.Height);
			Font font = new Font("Arial Black", 18, FontStyle.Bold);
			StringFormat format = new StringFormat(StringFormatFlags.NoClip | StringFormatFlags.NoWrap);
			format.Alignment = StringAlignment.Center;
			format.LineAlignment = StringAlignment.Center;
			Graphics.DrawString(text, font, new SolidBrush(color), x, y, format);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		protected virtual void Form_OnPaint(object sender, PaintEventArgs e)
		{
			Graphics = e.Graphics;
			OnRender(new RenderEventArgs(this, new Device()));
			Graphics = null;
			Form.Invalidate();
		}
	}

	/// <summary>
	/// 
	/// </summary>
	public class BomberForm : Form
	{
		/// <summary>
		/// 
		/// </summary>
		/// <param name="e"></param>
		protected override void OnResize(EventArgs e)
		{
			base.OnResize(e);
			Invalidate();
		}

		/// <summary>
		/// 
		/// </summary>
		public BomberForm()
		{
			CreateParams.ClassStyle |= 0x23;

			this.SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.UserPaint | ControlStyles.Opaque | ControlStyles.ResizeRedraw, true);
			this.SetStyle(ControlStyles.StandardClick | ControlStyles.StandardDoubleClick | ControlStyles.SupportsTransparentBackColor, false);
		}
	}
}
