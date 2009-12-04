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
using System.Collections.Generic;

using BomberStuff.Core.UserInterface;
using BomberStuff.Core.Input;
using Control = BomberStuff.Core.Input.Control;
using ControlEventArgs = BomberStuff.Core.Input.ControlEventArgs;

using SlimDX;
using SlimDX.Direct3D9;
using System.Runtime.InteropServices;

namespace BomberStuff.SlimDXInterface
{
	/// <summary>
	/// Bomber Stuff user interface implemented using System.Windows.Forms
	/// with SlimDX Direct3D9 graphics
	/// </summary>
	/// <remarks>
	/// TODO: Add keyboard input via RawInput
	/// </remarks>
	public class SlimDXInterface : IUserInterface, IInputMethod
	{
		/// <summary>The underlying Form</summary>
		protected Form Form;

		/// <summary></summary>
		protected Direct3D Direct3D;
		/// <summary></summary>
		internal SDXDevice Device;
		/// <summary></summary>
		protected PresentParameters PresentParams;
		/// <summary></summary>
		protected Sprite Sprite;
		/// <summary></summary>
		protected Font d3dFont;

		#region UserInterface Events
		/// <summary></summary>
		public event EventHandler<LoadSpritesEventArgs> LoadSprites;
		/// <summary></summary>
		public event EventHandler<RenderEventArgs> Render;
		/// <summary>
		/// 
		/// </summary>
		protected event EventHandler<EventArgs> IdleEvent;
		/// <summary></summary>
		event EventHandler<EventArgs> IUserInterface.Idle
		{
			add
			{
				if (IdleEvent == null)
					IdleEvent = value;
				else lock (IdleEvent)
				{
					IdleEvent += value;
				}
			}
			remove
			{
				if (IdleEvent != null)
					lock (IdleEvent)
					{
						IdleEvent -= value;
					}
			}
		}

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

		/// <summary>
		/// 
		/// </summary>
		/// <param name="e"></param>
		protected virtual void OnIdleEvent(EventArgs e)
		{
			if (IdleEvent != null)
				IdleEvent(this, e);
		}
		#endregion

		/// <summary></summary>
		public bool CacheAllSprites { get { return false; } }

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		bool IUserInterface.Initialize(BomberStuff.Core.Settings settings)
		{
			Form = new SlimDXForm();

			Form.ClientSize = settings.Get<Size>(BomberStuff.Core.Settings.Types.WindowSize);
			Form.Text = "Bomber Stuff, SlimDX interface";
			Form.Load += Form_Load;
			Form.Resize += Form_Resize;
			Form.FormClosing += Form_Closing;
			Form.KeyDown += Form_KeyDown;
			Form.KeyUp += Form_KeyUp;
			return true;
		}

		#region IDisposable implementation
		/// <summary>
		/// 
		/// </summary>
		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="disposing"></param>
		protected virtual void Dispose(bool disposing)
		{
			if (disposing)
				PresentParams = null;

			if (Direct3D != null)
			{
				Direct3D.Dispose();
				Direct3D = null;
			}
			if (Sprite != null)
			{
				Sprite.Dispose();
				Sprite = null;
			}

			if (Device != null)
			{
				Device.Dispose();
				Device = null;
			}

			if (d3dFont != null)
			{
				d3dFont.Dispose();
				d3dFont = null;
			}
		}

		/// <summary>
		/// 
		/// </summary>
		~SlimDXInterface()
		{
			Dispose(false);
		}

		#endregion

		private void Form_Closing(object sender, EventArgs e)
		{
			Dispose(true);
		}

		private void Form_Load(object sender, EventArgs e)
		{
			Form form = (Form)sender;

			System.Diagnostics.Debug.Assert(Direct3D == null);
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

			System.Diagnostics.Debug.Assert(PresentParams == null);
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

			System.Diagnostics.Debug.Assert(Device == null);
			// create the Direct3D device
			Device = new SDXDevice(new Device(Direct3D, nAdapter, DeviceType.Hardware, form.Handle,
									devFlags, PresentParams),
									Form.ClientSize.Width / 640.0f, Form.ClientSize.Height / 480.0f);

			ResetDevice();

			using (System.Drawing.Font baseFont = new System.Drawing.Font("Arial Black", 18, FontStyle.Bold))
			{
				System.Diagnostics.Debug.Assert(d3dFont == null);
				d3dFont = new Font(Device, baseFont);
			}

			Application.Idle += OnApplicationIdle;
		}

		/// <summary>
		/// 
		/// </summary>
		private void ResetDevice()
		{
			if (Sprite != null)
				Sprite.Dispose();
			Sprite = new Sprite(Device);
			OnLoadSprites(new LoadSpritesEventArgs(Device));
		}

		private void Form_Resize(object sender, EventArgs e)
		{
			//ResetDevice();
		}

		/// <summary>
		/// 
		/// </summary>
		public void MainLoop()
		{
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
			System.Diagnostics.Debug.Assert(s.Texture != null);

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
			Rectangle rect = new Rectangle(x - 1, y - 1, 2, 2);
			DrawTextFormat format = DrawTextFormat.Center | DrawTextFormat.VerticalCenter | DrawTextFormat.NoClip;
			d3dFont.DrawString(Sprite, text, rect, format, color);
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

			OnRender(new RenderEventArgs(this, Device));

			System.Diagnostics.Debug.Assert(device != null);

			// aaaannnd.... done!
			Sprite.End();

			// painting done ...
			device.EndScene();

			// ... so show the result on screen
			device.Present();

			OnIdleEvent(new EventArgs());
		}

		/// <summary>
		/// 
		/// </summary>
		string IInputMethod.Name
		{
			get { return "SlimDXInterface"; }
		}

		#region InputMethod methods
		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public Dictionary<string, Control> GetControls()
		{
			string[] keyNames = Enum.GetNames(typeof(Keys));
			Array keyValues = Enum.GetValues(typeof(Keys));
			Dictionary<string, Control> keys = new Dictionary<string, Control>(keyValues.Length);

			for (int i = 0; i < keyValues.Length; ++i)
				keys.Add(keyNames[i], new WinFormsKeyControl((Keys)keyValues.GetValue(i)));

			return keys;
		}

		private List<WinFormsKeyControl> RegisteredControls = new List<WinFormsKeyControl>();

		/// <summary>
		/// 
		/// </summary>
		/// <param name="c"></param>
		public void RegisterControl(Control c)
		{
			WinFormsKeyControl key = c as WinFormsKeyControl;

			System.Diagnostics.Debug.Assert(key != null);

			System.Diagnostics.Debug.Assert(RegisteredControls.IndexOf(key) == -1);

			RegisteredControls.Add(key);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="c"></param>
		public void UnregisterControl(Control c)
		{
			WinFormsKeyControl key = c as WinFormsKeyControl;

			System.Diagnostics.Debug.Assert(key != null);

			System.Diagnostics.Debug.Assert(RegisteredControls.Remove(key));
		}

		private void Form_KeyDown(object sender, KeyEventArgs e)
		{
			foreach (WinFormsKeyControl key in RegisteredControls)
				if (key.Key == e.KeyCode)
				{
					key.Press();
					break;
				}
		}

		private void Form_KeyUp(object sender, KeyEventArgs e)
		{
			foreach (WinFormsKeyControl key in RegisteredControls)
				if (key.Key == e.KeyCode)
				{
					key.Release();
					break;
				}
		}
		#endregion
	}

	internal sealed class WinFormsKeyControl : Control
	{
		public readonly Keys Key;
		private bool IsPressed;

		/// <summary>
		/// 
		/// </summary>
		public WinFormsKeyControl(Keys key)
		{
			Key = key;
		}

		/// <summary>
		/// 
		/// </summary>
		public override string Name
		{
			get { return Key.ToString(); }
		}

		internal void Press()
		{
			if (!IsPressed)
			{
				IsPressed = true;
				//System.Console.WriteLine(Key + " pressed");
				OnPressed(new ControlEventArgs(true));
			}
		}

		internal void Release()
		{
			if (IsPressed)
			{
				IsPressed = false;
				//System.Console.WriteLine(Key + " released");
				OnReleased(new ControlEventArgs(false));
			}
		}
	}

	
}
