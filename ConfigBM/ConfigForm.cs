//
// ConfigForm.cs - ConfigForm class
//
// Copyright © 2010  Thomas Faber
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
using System.Linq;
using System.Drawing;
using System.Windows.Forms;

using BomberStuff.Core;
using BomberStuff.Core.Animation;
using BomberStuff.Files;
using BomberStuff.Core.Utilities;

namespace BomberStuff.ConfigBM
{
	/// <summary>
	/// 
	/// </summary>
	public partial class ConfigForm : Form
	{
		AnimationFrame OrigFrame;
		Settings Settings;
		ColorRemapInfo[] PlayerRemapInfo = new ColorRemapInfo[10];
		/// <summary>
		/// 
		/// </summary>
		public ConfigForm(Settings settings)
		{
			InitializeComponent();

			AniFile aniFile = new AniFile(settings.Get<string>(Settings.Types.ABDirectory) + @"\DATA\ANI\stand.ani");
			foreach (Animation ani in aniFile.Sequences)
			{
				if (ani.Name == "stand south")
				{
					OrigFrame = ani.Frames[0];
					break;
				}
			}

			Settings = settings;

			Size windowSize = settings.Get<Size>(Settings.Types.WindowSize);
			txtWidth.Text = windowSize.Width.ToString();
			txtHeight.Text = windowSize.Height.ToString();
			txtWidthHeight_Validated(txtWidth, new EventArgs());

			int tileset = settings.Get<int>(Settings.Types.Tileset);
			if (settings.Get<object>(Settings.Types.Tileset) == null)
				tileset = -1;
			numTileset.Value = tileset;

			ColorRemapInfo[] remapInfo = settings.Get<ColorRemapInfo[]>(Settings.Types.PlayerColor);
			for (int i = 0; i < remapInfo.Length; ++i)
				PlayerRemapInfo[i] = remapInfo[i];

			int playerCount = settings.Get<int>(Settings.Types.PlayerCount);
			if (playerCount == 0)
				playerCount = 1;
			numPlayers.Value = playerCount;

			txtPath.Text = settings.Get<string>(Settings.Types.ABDirectory);
			browsePath.SelectedPath = txtPath.Text;

			cbScheme.Text = settings.Get<string>(Settings.Types.Scheme);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="color"></param>
		private void PreviewColor(ColorRemapInfo color)
		{
			AnimationFrame frame = ColorRemapper.Remap(OrigFrame, color);

			Bitmap back;

			using (Bitmap bmp = new Bitmap(frame.BitmapBuilder.GetStream()))
			{
				back = new Bitmap(bmp.Width * 2, bmp.Height * 2);

				using (Graphics g = Graphics.FromImage(back))
				{
					g.DrawImage(bmp, new Rectangle(0, 0, back.Width, back.Height),
								new Rectangle(0, 0, bmp.Width, bmp.Height), GraphicsUnit.Pixel);
				}
			}

			picPreviewColor.Image = back;
		}

		private void UpdatePreview(object sender, EventArgs e)
		{
			lblHueValue.Text = trackHue.Value.ToString();
			lblSaturationValue.Text = trackSaturation.Value.ToString();
			lblLightnessValue.Text = trackLightness.Value.ToString();

			PlayerRemapInfo[cbPlayer.SelectedIndex] = new ColorRemapInfo(checkHue.Checked, trackHue.Value,
											checkSaturation.Checked, trackSaturation.Value,
											trackLightness.Value);

			PreviewColor(PlayerRemapInfo[cbPlayer.SelectedIndex]);
		}

		private void numPlayers_ValueChanged(object sender, EventArgs e)
		{
			int newCount = (int)numPlayers.Value;
			int oldCount = cbPlayer.Items.Count;

			if (newCount > oldCount)
				for (int i = oldCount; i < newCount; ++i)
					cbPlayer.Items.Add("Player" + i);
			else if (newCount < oldCount)
				for (int i = newCount; i < oldCount; ++i)
					cbPlayer.Items.RemoveAt(newCount);

			if (cbPlayer.SelectedIndex == -1 && newCount != 0)
				cbPlayer.SelectedIndex = 0;
		}

		private void cbPlayer_SelectedIndexChanged(object sender, EventArgs e)
		{
			int i = cbPlayer.SelectedIndex;
			ColorRemapInfo info = PlayerRemapInfo[i];

			checkHue.Checked = info.SetHue;
			if (info.SetHue)
				trackHue.Value = info.NewHue;
			else
				trackHue.Value = 135;

			checkSaturation.Checked = info.SetSaturation;
			if (info.SetSaturation)
				trackSaturation.Value = info.NewSaturation;
			else
				trackSaturation.Value = 0;

			trackLightness.Value = info.DiffLightness;
		}

		private void btnCancel_Click(object sender, EventArgs e)
		{
			Close();
		}

		private void btnApply_Click(object sender, EventArgs e)
		{
			// apply settings
			Settings.Set<int>(Settings.Types.PlayerCount, (int)numPlayers.Value);
			Settings.Set<int>(Settings.Types.Tileset, (int)numTileset.Value);
			Settings.Set<string>(Settings.Types.Scheme, cbScheme.Text);
			Settings.Set<Size>(Settings.Types.WindowSize, new Size(int.Parse(txtWidth.Text), int.Parse(txtHeight.Text)));
			Settings.Set<ColorRemapInfo[]>(Settings.Types.PlayerColor, PlayerRemapInfo);
			Settings.Set<string>(Settings.Types.ABDirectory, txtPath.Text);

			// make a backup
			System.IO.File.Copy("settings.xml", "settings.last.xml", true);

			// save
			SettingsWriter.WriteFile(Settings, "settings.xml");
		}

		private void btnOK_Click(object sender, EventArgs e)
		{
			btnApply_Click(sender, e);
			btnCancel_Click(sender, e);
		}

		private void txtWidthHeight_Validated(object sender, EventArgs e)
		{
			for (int i = 1; i < cbSize.Items.Count; ++i)
				if ((string)cbSize.Items[i] == txtWidth.Text + "x" + txtHeight.Text)
				{
					cbSize.SelectedIndex = i;
					return;
				}

			cbSize.SelectedIndex = 0;
		}

		private void cbSize_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (cbSize.SelectedIndex == 1)
			{
				txtWidth.Text = "320";
				txtHeight.Text = "240";
			}
			else if (cbSize.SelectedIndex == 2)
			{
				txtWidth.Text = "640";
				txtHeight.Text = "480";
			}
			else if (cbSize.SelectedIndex == 3)
			{
				txtWidth.Text = "1280";
				txtHeight.Text = "960";
			}
			else if (cbSize.SelectedIndex == 4)
			{
				txtWidth.Text = "1600";
				txtHeight.Text = "1200";
			}
		}

		private void txtWidthHeight_Validating(object sender, System.ComponentModel.CancelEventArgs e)
		{
			TextBox txt = (TextBox)sender;
			ushort value;

			if (!ushort.TryParse(txt.Text, out value))
				e.Cancel = true;
		}

		private void btnBrowse_Click(object sender, EventArgs e)
		{
			browsePath.SelectedPath = txtPath.Text;
			if (browsePath.ShowDialog(this) == DialogResult.OK)
				txtPath.Text = browsePath.SelectedPath;
		}

		private bool TestABPath()
		{
			string path = txtPath.Text;
			bool valid = false;

			if (System.IO.Directory.Exists(path + @"\DATA\ANI")
					&& System.IO.Directory.Exists(path + @"\DATA\RES"))
			{
				valid = true;
				string[] schemes = System.IO.Directory.GetFiles(txtPath.Text + @"\DATA\SCHEMES");

				schemes = schemes.Select<string, string>(System.IO.Path.GetFileNameWithoutExtension).ToArray<string>();

				cbScheme.Items.AddRange(schemes);
			}

			lblPathInvalid.Visible = !valid;

			return valid;
		}

		private void txtPath_TextChanged(object sender, EventArgs e)
		{
			TestABPath();
		}
	}
}
