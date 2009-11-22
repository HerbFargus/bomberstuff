//
// UserInterface.cs - SlimDX UserInterface class
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
using Font = SlimDX.Direct3D9.Font;
using System.Windows.Forms;

using BomberStuff.Core.UserInterface;

using SlimDX;
using SlimDX.Direct3D9;
using System.Runtime.InteropServices;

namespace BomberStuff.SlimDXInterface
{
	/// <summary>
	/// Bomber Stuff user interface implemented using System.Windows.Forms
	/// with GDI+ graphics
	/// </summary>
	/// <remarks>
	/// TODO: Add keyboard input
	/// </remarks>
	public class UserInterface : IUserInterface
	{
		/// <summary>The underlying Form</summary>
		protected Form Form;

		/// <summary></summary>
		protected Direct3D Direct3D;
		/// <summary></summary>
		protected Device Device;
		/// <summary></summary>
		protected PresentParameters PresentParams;
		/// <summary></summary>
		protected Sprite Sprite;
		/// <summary></summary>
		protected Font d3dFont;

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
		public bool CacheAllSprites { get { return false; } }

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		bool IUserInterface.Initialize()
		{
			Form = new BomberForm();

			Form.ClientSize = new Size(640, 480);
			Form.Text = "Bomber Stuff, SlimDX interface";
			Form.Activated += new EventHandler(Form_Activated);
			return true;
		}

		void Form_Activated(object sender, EventArgs e)
		{
			Form form = (Form)sender;
			Direct3D = new Direct3D();

			int nAdapter = Direct3D.Adapters[0].Adapter;
			Capabilities devCaps = Direct3D.GetDeviceCaps(nAdapter,
													DeviceType.Hardware);

			// the flags for our Direct3D device. Use software rendering by
			// default
			CreateFlags devFlags = CreateFlags.SoftwareVertexProcessing;

			// use hardware vertex processing if supported
			if ((devCaps.DeviceCaps & DeviceCaps.HWTransformAndLight)
										== DeviceCaps.HWTransformAndLight)
				devFlags = CreateFlags.HardwareVertexProcessing;

			// use pure device (whatever that is) if supported
			if ((devCaps.DeviceCaps & DeviceCaps.PureDevice)
										== DeviceCaps.PureDevice)
				devFlags |= CreateFlags.PureDevice;

			DisplayMode displayMode = Direct3D.GetAdapterDisplayMode(nAdapter);

			PresentParams = new PresentParameters();
			PresentParams.BackBufferWidth = form.ClientSize.Width;
			PresentParams.BackBufferHeight = form.ClientSize.Height;
			PresentParams.BackBufferFormat = displayMode.Format;
			PresentParams.BackBufferCount = 1;
			PresentParams.Windowed = true;
			PresentParams.SwapEffect = SwapEffect.Discard;
			PresentParams.AutoDepthStencilFormat = Format.D16;
			PresentParams.EnableAutoDepthStencil = true;
			PresentParams.DeviceWindowHandle = form.Handle;

			// create the Direct3D device
			Device = new SlimDX.Direct3D9.Device(Direct3D, nAdapter, DeviceType.Hardware, form.Handle,
									devFlags, PresentParams);

			// ResetDevice;
			Sprite = new Sprite(Device);
			OnLoadSprites(new LoadSpritesEventArgs(new SDXDevice(Device)));

			using (System.Drawing.Font baseFont = new System.Drawing.Font("Arial Black", 18, FontStyle.Bold))
			{
				d3dFont = new Font(Device, baseFont);
			}
		}

		/// <summary>
		/// 
		/// </summary>
		public void MainLoop()
		{
			Application.Idle += OnApplicationIdle;
			Application.Run(Form);
		}

		#region OnApplicationIdle
		private void OnApplicationIdle(object sender, EventArgs e)
		{
			Message msg;

			while (!PeekMessage(out msg, IntPtr.Zero, 0, 0, 0))
					Idle();
		}


		[StructLayout(LayoutKind.Sequential)]
		private struct Message
		{
			public IntPtr hWnd;
			public uint msg;
			public IntPtr wParam;
			public IntPtr lParam;
			public uint time;
			public Point p;
		}


		[DllImport("user32.dll", CharSet = CharSet.Auto)]
		[return: MarshalAs(UnmanagedType.Bool)]
		private static extern bool PeekMessage(out Message msg, IntPtr hWnd,
												uint messageFilterMin,
												uint messageFilterMax, uint flags);
		#endregion

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
			SDXSprite s = (SDXSprite)sprite;

			float w = Form.ClientSize.Width;
			float h = Form.ClientSize.Height;

			Vector3 vPosition = new Vector3(w * position.X, h * position.Y, 0);

			Sprite.Draw((SDXSprite)sprite, new Vector3(), vPosition, color);
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
			/*Font font = new Font("Arial Black", 18, FontStyle.Bold);
			StringFormat format = new StringFormat(StringFormatFlags.NoClip | StringFormatFlags.NoWrap);
			format.Alignment = StringAlignment.Center;
			format.LineAlignment = StringAlignment.Center;
			Graphics.DrawString(text, font, new SolidBrush(color), x, y, format);*/
			d3dFont.DrawString(Sprite, text, x, y, color);
		}

		/// <summary>
		/// 
		/// </summary>
		protected virtual void Idle()
		{
			SlimDX.Direct3D9.Device device = Device;

			device.BeginScene();

			// reset the depth buffer to 1.0 and the render target to black
			device.Clear(ClearFlags.Target | ClearFlags.ZBuffer,
							new Color4(0, 0, 0), 1.0f, 0);

			// prepare for drawing sprites. We need transparency and z-order
			Sprite.Begin(SpriteFlags.AlphaBlend
							| SpriteFlags.SortDepthBackToFront);

			OnRender(new RenderEventArgs(this, new SDXDevice(Device)));

			// aaaannnd.... done!
			Sprite.End();

			// painting done ...
			device.EndScene();

			// ... so show the result on screen
			device.Present();
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
			MinimizeBox = false;
			MaximizeBox = false;

			CreateParams.ClassStyle |= 0x23;

			this.SetStyle(ControlStyles.AllPaintingInWmPaint
				//| ControlStyles.OptimizedDoubleBuffer
				| ControlStyles.UserPaint
				| ControlStyles.Opaque
				| ControlStyles.ResizeRedraw, true);
			this.SetStyle(ControlStyles.StandardClick | ControlStyles.StandardDoubleClick | ControlStyles.SupportsTransparentBackColor, false);

			FormBorderStyle = FormBorderStyle.FixedSingle;
		}
	}
}
