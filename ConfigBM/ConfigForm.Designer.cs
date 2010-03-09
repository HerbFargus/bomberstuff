//
// ConfigForm.Designer.cs - ConfigForm Window Designer parts
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

namespace BomberStuff.ConfigBM
{
	partial class ConfigForm
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.checkFullscreen = new System.Windows.Forms.CheckBox();
			this.txtHeight = new System.Windows.Forms.TextBox();
			this.label1 = new System.Windows.Forms.Label();
			this.txtWidth = new System.Windows.Forms.TextBox();
			this.cbSize = new System.Windows.Forms.ComboBox();
			this.groupBox2 = new System.Windows.Forms.GroupBox();
			this.label7 = new System.Windows.Forms.Label();
			this.label6 = new System.Windows.Forms.Label();
			this.checkBox5 = new System.Windows.Forms.CheckBox();
			this.checkBox4 = new System.Windows.Forms.CheckBox();
			this.checkBox3 = new System.Windows.Forms.CheckBox();
			this.checkBox2 = new System.Windows.Forms.CheckBox();
			this.checkBox1 = new System.Windows.Forms.CheckBox();
			this.checkLeft = new System.Windows.Forms.CheckBox();
			this.label3 = new System.Windows.Forms.Label();
			this.numPlayers = new System.Windows.Forms.NumericUpDown();
			this.cbPlayer = new System.Windows.Forms.ComboBox();
			this.lblLightnessValue = new System.Windows.Forms.Label();
			this.lblSaturationValue = new System.Windows.Forms.Label();
			this.lblHueValue = new System.Windows.Forms.Label();
			this.picPreviewColor = new System.Windows.Forms.PictureBox();
			this.lblLightness = new System.Windows.Forms.Label();
			this.checkSaturation = new System.Windows.Forms.CheckBox();
			this.checkHue = new System.Windows.Forms.CheckBox();
			this.trackLightness = new System.Windows.Forms.TrackBar();
			this.trackSaturation = new System.Windows.Forms.TrackBar();
			this.trackHue = new System.Windows.Forms.TrackBar();
			this.groupBox3 = new System.Windows.Forms.GroupBox();
			this.label2 = new System.Windows.Forms.Label();
			this.rbCustom = new System.Windows.Forms.RadioButton();
			this.rbWinForms = new System.Windows.Forms.RadioButton();
			this.rbSlimDX = new System.Windows.Forms.RadioButton();
			this.groupBox4 = new System.Windows.Forms.GroupBox();
			this.lblPathInvalid = new System.Windows.Forms.Label();
			this.btnBrowse = new System.Windows.Forms.Button();
			this.label9 = new System.Windows.Forms.Label();
			this.txtPath = new System.Windows.Forms.TextBox();
			this.label8 = new System.Windows.Forms.Label();
			this.cbScheme = new System.Windows.Forms.ComboBox();
			this.label5 = new System.Windows.Forms.Label();
			this.label4 = new System.Windows.Forms.Label();
			this.numTileset = new System.Windows.Forms.NumericUpDown();
			this.btnCancel = new System.Windows.Forms.Button();
			this.btnOK = new System.Windows.Forms.Button();
			this.browsePath = new System.Windows.Forms.FolderBrowserDialog();
			this.btnRun = new System.Windows.Forms.Button();
			this.groupBox1.SuspendLayout();
			this.groupBox2.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.numPlayers)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.picPreviewColor)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.trackLightness)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.trackSaturation)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.trackHue)).BeginInit();
			this.groupBox3.SuspendLayout();
			this.groupBox4.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.numTileset)).BeginInit();
			this.SuspendLayout();
			// 
			// groupBox1
			// 
			this.groupBox1.Controls.Add(this.checkFullscreen);
			this.groupBox1.Controls.Add(this.txtHeight);
			this.groupBox1.Controls.Add(this.label1);
			this.groupBox1.Controls.Add(this.txtWidth);
			this.groupBox1.Controls.Add(this.cbSize);
			this.groupBox1.Location = new System.Drawing.Point(314, 12);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(329, 115);
			this.groupBox1.TabIndex = 2;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "Window Size";
			// 
			// checkFullscreen
			// 
			this.checkFullscreen.Enabled = false;
			this.checkFullscreen.Location = new System.Drawing.Point(188, 40);
			this.checkFullscreen.Name = "checkFullscreen";
			this.checkFullscreen.Size = new System.Drawing.Size(95, 21);
			this.checkFullscreen.TabIndex = 4;
			this.checkFullscreen.Text = "Fullscreen";
			// 
			// txtHeight
			// 
			this.txtHeight.Location = new System.Drawing.Point(118, 83);
			this.txtHeight.MaxLength = 4;
			this.txtHeight.Name = "txtHeight";
			this.txtHeight.Size = new System.Drawing.Size(49, 22);
			this.txtHeight.TabIndex = 3;
			this.txtHeight.Validated += new System.EventHandler(this.txtWidthHeight_Validated);
			this.txtHeight.Validating += new System.ComponentModel.CancelEventHandler(this.txtWidthHeight_Validating);
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(98, 83);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(14, 17);
			this.label1.TabIndex = 17;
			this.label1.Text = "x";
			// 
			// txtWidth
			// 
			this.txtWidth.Location = new System.Drawing.Point(43, 83);
			this.txtWidth.MaxLength = 4;
			this.txtWidth.Name = "txtWidth";
			this.txtWidth.Size = new System.Drawing.Size(49, 22);
			this.txtWidth.TabIndex = 2;
			this.txtWidth.Validated += new System.EventHandler(this.txtWidthHeight_Validated);
			this.txtWidth.Validating += new System.ComponentModel.CancelEventHandler(this.txtWidthHeight_Validating);
			// 
			// cbSize
			// 
			this.cbSize.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.cbSize.Items.AddRange(new object[] {
            "<Custom>",
            "320x240",
            "640x480",
            "1280x960",
            "1600x1200"});
			this.cbSize.Location = new System.Drawing.Point(20, 38);
			this.cbSize.Name = "cbSize";
			this.cbSize.Size = new System.Drawing.Size(146, 24);
			this.cbSize.TabIndex = 1;
			this.cbSize.SelectedIndexChanged += new System.EventHandler(this.cbSize_SelectedIndexChanged);
			// 
			// groupBox2
			// 
			this.groupBox2.Controls.Add(this.label7);
			this.groupBox2.Controls.Add(this.label6);
			this.groupBox2.Controls.Add(this.checkBox5);
			this.groupBox2.Controls.Add(this.checkBox4);
			this.groupBox2.Controls.Add(this.checkBox3);
			this.groupBox2.Controls.Add(this.checkBox2);
			this.groupBox2.Controls.Add(this.checkBox1);
			this.groupBox2.Controls.Add(this.checkLeft);
			this.groupBox2.Controls.Add(this.label3);
			this.groupBox2.Controls.Add(this.numPlayers);
			this.groupBox2.Controls.Add(this.cbPlayer);
			this.groupBox2.Controls.Add(this.lblLightnessValue);
			this.groupBox2.Controls.Add(this.lblSaturationValue);
			this.groupBox2.Controls.Add(this.lblHueValue);
			this.groupBox2.Controls.Add(this.picPreviewColor);
			this.groupBox2.Controls.Add(this.lblLightness);
			this.groupBox2.Controls.Add(this.checkSaturation);
			this.groupBox2.Controls.Add(this.checkHue);
			this.groupBox2.Controls.Add(this.trackLightness);
			this.groupBox2.Controls.Add(this.trackSaturation);
			this.groupBox2.Controls.Add(this.trackHue);
			this.groupBox2.Location = new System.Drawing.Point(12, 281);
			this.groupBox2.Name = "groupBox2";
			this.groupBox2.Size = new System.Drawing.Size(631, 290);
			this.groupBox2.TabIndex = 4;
			this.groupBox2.TabStop = false;
			this.groupBox2.Text = "Player Settings";
			// 
			// label7
			// 
			this.label7.AutoSize = true;
			this.label7.Location = new System.Drawing.Point(12, 81);
			this.label7.Name = "label7";
			this.label7.Size = new System.Drawing.Size(41, 17);
			this.label7.TabIndex = 34;
			this.label7.Text = "Color";
			// 
			// label6
			// 
			this.label6.AutoSize = true;
			this.label6.Location = new System.Drawing.Point(257, 23);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(60, 17);
			this.label6.TabIndex = 33;
			this.label6.Text = "Controls";
			// 
			// checkBox5
			// 
			this.checkBox5.Appearance = System.Windows.Forms.Appearance.Button;
			this.checkBox5.AutoCheck = false;
			this.checkBox5.Enabled = false;
			this.checkBox5.Location = new System.Drawing.Point(341, 84);
			this.checkBox5.Name = "checkBox5";
			this.checkBox5.Size = new System.Drawing.Size(79, 27);
			this.checkBox5.TabIndex = 4;
			this.checkBox5.Text = "Action 2";
			this.checkBox5.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			this.checkBox5.UseVisualStyleBackColor = true;
			// 
			// checkBox4
			// 
			this.checkBox4.Appearance = System.Windows.Forms.Appearance.Button;
			this.checkBox4.AutoCheck = false;
			this.checkBox4.Enabled = false;
			this.checkBox4.Location = new System.Drawing.Point(341, 18);
			this.checkBox4.Name = "checkBox4";
			this.checkBox4.Size = new System.Drawing.Size(79, 27);
			this.checkBox4.TabIndex = 3;
			this.checkBox4.Text = "Action 1";
			this.checkBox4.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			this.checkBox4.UseVisualStyleBackColor = true;
			// 
			// checkBox3
			// 
			this.checkBox3.Appearance = System.Windows.Forms.Appearance.Button;
			this.checkBox3.AutoCheck = false;
			this.checkBox3.Enabled = false;
			this.checkBox3.Location = new System.Drawing.Point(475, 84);
			this.checkBox3.Name = "checkBox3";
			this.checkBox3.Size = new System.Drawing.Size(79, 27);
			this.checkBox3.TabIndex = 8;
			this.checkBox3.Text = "Down";
			this.checkBox3.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			this.checkBox3.UseVisualStyleBackColor = true;
			// 
			// checkBox2
			// 
			this.checkBox2.Appearance = System.Windows.Forms.Appearance.Button;
			this.checkBox2.AutoCheck = false;
			this.checkBox2.Enabled = false;
			this.checkBox2.Location = new System.Drawing.Point(475, 18);
			this.checkBox2.Name = "checkBox2";
			this.checkBox2.Size = new System.Drawing.Size(79, 27);
			this.checkBox2.TabIndex = 5;
			this.checkBox2.Text = "Up";
			this.checkBox2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			this.checkBox2.UseVisualStyleBackColor = true;
			// 
			// checkBox1
			// 
			this.checkBox1.Appearance = System.Windows.Forms.Appearance.Button;
			this.checkBox1.AutoCheck = false;
			this.checkBox1.Enabled = false;
			this.checkBox1.Location = new System.Drawing.Point(525, 51);
			this.checkBox1.Name = "checkBox1";
			this.checkBox1.Size = new System.Drawing.Size(79, 27);
			this.checkBox1.TabIndex = 7;
			this.checkBox1.Text = "Right";
			this.checkBox1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			this.checkBox1.UseVisualStyleBackColor = true;
			// 
			// checkLeft
			// 
			this.checkLeft.Appearance = System.Windows.Forms.Appearance.Button;
			this.checkLeft.AutoCheck = false;
			this.checkLeft.Enabled = false;
			this.checkLeft.Location = new System.Drawing.Point(422, 51);
			this.checkLeft.Name = "checkLeft";
			this.checkLeft.Size = new System.Drawing.Size(79, 27);
			this.checkLeft.TabIndex = 6;
			this.checkLeft.Text = "Left";
			this.checkLeft.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			this.checkLeft.UseVisualStyleBackColor = true;
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Location = new System.Drawing.Point(12, 18);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(125, 17);
			this.label3.TabIndex = 26;
			this.label3.Text = "Number of Players";
			// 
			// numPlayers
			// 
			this.numPlayers.Location = new System.Drawing.Point(162, 18);
			this.numPlayers.Maximum = new decimal(new int[] {
            10,
            0,
            0,
            0});
			this.numPlayers.Name = "numPlayers";
			this.numPlayers.Size = new System.Drawing.Size(50, 22);
			this.numPlayers.TabIndex = 1;
			this.numPlayers.ValueChanged += new System.EventHandler(this.numPlayers_ValueChanged);
			// 
			// cbPlayer
			// 
			this.cbPlayer.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.cbPlayer.Location = new System.Drawing.Point(135, 54);
			this.cbPlayer.MaxDropDownItems = 10;
			this.cbPlayer.Name = "cbPlayer";
			this.cbPlayer.Size = new System.Drawing.Size(146, 24);
			this.cbPlayer.TabIndex = 2;
			this.cbPlayer.SelectedIndexChanged += new System.EventHandler(this.cbPlayer_SelectedIndexChanged);
			// 
			// lblLightnessValue
			// 
			this.lblLightnessValue.Location = new System.Drawing.Point(102, 220);
			this.lblLightnessValue.Name = "lblLightnessValue";
			this.lblLightnessValue.Size = new System.Drawing.Size(32, 17);
			this.lblLightnessValue.TabIndex = 23;
			this.lblLightnessValue.Text = "0";
			// 
			// lblSaturationValue
			// 
			this.lblSaturationValue.Location = new System.Drawing.Point(102, 168);
			this.lblSaturationValue.Name = "lblSaturationValue";
			this.lblSaturationValue.Size = new System.Drawing.Size(32, 21);
			this.lblSaturationValue.TabIndex = 22;
			this.lblSaturationValue.Text = "0";
			// 
			// lblHueValue
			// 
			this.lblHueValue.Location = new System.Drawing.Point(102, 120);
			this.lblHueValue.Name = "lblHueValue";
			this.lblHueValue.Size = new System.Drawing.Size(32, 17);
			this.lblHueValue.TabIndex = 21;
			this.lblHueValue.Text = "135";
			// 
			// picPreviewColor
			// 
			this.picPreviewColor.Location = new System.Drawing.Point(524, 136);
			this.picPreviewColor.Name = "picPreviewColor";
			this.picPreviewColor.Size = new System.Drawing.Size(80, 140);
			this.picPreviewColor.TabIndex = 20;
			this.picPreviewColor.TabStop = false;
			// 
			// lblLightness
			// 
			this.lblLightness.Location = new System.Drawing.Point(2, 220);
			this.lblLightness.Name = "lblLightness";
			this.lblLightness.Size = new System.Drawing.Size(69, 17);
			this.lblLightness.TabIndex = 19;
			this.lblLightness.Text = "Lightness";
			// 
			// checkSaturation
			// 
			this.checkSaturation.Location = new System.Drawing.Point(5, 168);
			this.checkSaturation.Name = "checkSaturation";
			this.checkSaturation.Size = new System.Drawing.Size(95, 21);
			this.checkSaturation.TabIndex = 11;
			this.checkSaturation.Text = "Saturation";
			this.checkSaturation.CheckStateChanged += new System.EventHandler(this.UpdatePreview);
			// 
			// checkHue
			// 
			this.checkHue.Location = new System.Drawing.Point(5, 120);
			this.checkHue.Name = "checkHue";
			this.checkHue.Size = new System.Drawing.Size(56, 21);
			this.checkHue.TabIndex = 9;
			this.checkHue.Text = "Hue";
			this.checkHue.CheckStateChanged += new System.EventHandler(this.UpdatePreview);
			// 
			// trackLightness
			// 
			this.trackLightness.LargeChange = 30;
			this.trackLightness.Location = new System.Drawing.Point(156, 220);
			this.trackLightness.Maximum = 180;
			this.trackLightness.Minimum = -180;
			this.trackLightness.Name = "trackLightness";
			this.trackLightness.Size = new System.Drawing.Size(282, 56);
			this.trackLightness.SmallChange = 10;
			this.trackLightness.TabIndex = 13;
			this.trackLightness.TickFrequency = 10;
			this.trackLightness.ValueChanged += new System.EventHandler(this.UpdatePreview);
			// 
			// trackSaturation
			// 
			this.trackSaturation.LargeChange = 30;
			this.trackSaturation.Location = new System.Drawing.Point(156, 168);
			this.trackSaturation.Maximum = 359;
			this.trackSaturation.Name = "trackSaturation";
			this.trackSaturation.Size = new System.Drawing.Size(282, 56);
			this.trackSaturation.SmallChange = 10;
			this.trackSaturation.TabIndex = 12;
			this.trackSaturation.TickFrequency = 10;
			this.trackSaturation.ValueChanged += new System.EventHandler(this.UpdatePreview);
			// 
			// trackHue
			// 
			this.trackHue.LargeChange = 30;
			this.trackHue.Location = new System.Drawing.Point(156, 120);
			this.trackHue.Maximum = 320;
			this.trackHue.Name = "trackHue";
			this.trackHue.Size = new System.Drawing.Size(282, 56);
			this.trackHue.SmallChange = 10;
			this.trackHue.TabIndex = 10;
			this.trackHue.TickFrequency = 10;
			this.trackHue.Value = 135;
			this.trackHue.ValueChanged += new System.EventHandler(this.UpdatePreview);
			// 
			// groupBox3
			// 
			this.groupBox3.Controls.Add(this.label2);
			this.groupBox3.Controls.Add(this.rbCustom);
			this.groupBox3.Controls.Add(this.rbWinForms);
			this.groupBox3.Controls.Add(this.rbSlimDX);
			this.groupBox3.Enabled = false;
			this.groupBox3.Location = new System.Drawing.Point(12, 12);
			this.groupBox3.Name = "groupBox3";
			this.groupBox3.Size = new System.Drawing.Size(296, 115);
			this.groupBox3.TabIndex = 1;
			this.groupBox3.TabStop = false;
			this.groupBox3.Text = "User Interface";
			// 
			// label2
			// 
			this.label2.ForeColor = System.Drawing.Color.Firebrick;
			this.label2.Location = new System.Drawing.Point(157, 58);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(101, 17);
			this.label2.TabIndex = 25;
			this.label2.Text = "Does not work!";
			// 
			// rbCustom
			// 
			this.rbCustom.Enabled = false;
			this.rbCustom.Location = new System.Drawing.Point(15, 84);
			this.rbCustom.Name = "rbCustom";
			this.rbCustom.Size = new System.Drawing.Size(92, 21);
			this.rbCustom.TabIndex = 3;
			this.rbCustom.Text = "Custom ...";
			// 
			// rbWinForms
			// 
			this.rbWinForms.Location = new System.Drawing.Point(15, 57);
			this.rbWinForms.Name = "rbWinForms";
			this.rbWinForms.Size = new System.Drawing.Size(147, 21);
			this.rbWinForms.TabIndex = 2;
			this.rbWinForms.Text = "WinFormsInterface";
			// 
			// rbSlimDX
			// 
			this.rbSlimDX.Checked = true;
			this.rbSlimDX.Location = new System.Drawing.Point(15, 30);
			this.rbSlimDX.Name = "rbSlimDX";
			this.rbSlimDX.Size = new System.Drawing.Size(129, 21);
			this.rbSlimDX.TabIndex = 1;
			this.rbSlimDX.TabStop = true;
			this.rbSlimDX.Text = "SlimDXInterface";
			// 
			// groupBox4
			// 
			this.groupBox4.Controls.Add(this.lblPathInvalid);
			this.groupBox4.Controls.Add(this.btnBrowse);
			this.groupBox4.Controls.Add(this.label9);
			this.groupBox4.Controls.Add(this.txtPath);
			this.groupBox4.Controls.Add(this.label8);
			this.groupBox4.Controls.Add(this.cbScheme);
			this.groupBox4.Controls.Add(this.label5);
			this.groupBox4.Controls.Add(this.label4);
			this.groupBox4.Controls.Add(this.numTileset);
			this.groupBox4.Location = new System.Drawing.Point(12, 133);
			this.groupBox4.Name = "groupBox4";
			this.groupBox4.Size = new System.Drawing.Size(631, 142);
			this.groupBox4.TabIndex = 3;
			this.groupBox4.TabStop = false;
			this.groupBox4.Text = "Game Settings";
			// 
			// lblPathInvalid
			// 
			this.lblPathInvalid.AutoSize = true;
			this.lblPathInvalid.ForeColor = System.Drawing.Color.Firebrick;
			this.lblPathInvalid.Location = new System.Drawing.Point(175, 18);
			this.lblPathInvalid.Name = "lblPathInvalid";
			this.lblPathInvalid.Size = new System.Drawing.Size(60, 17);
			this.lblPathInvalid.TabIndex = 8;
			this.lblPathInvalid.Text = "INVALID";
			// 
			// btnBrowse
			// 
			this.btnBrowse.Location = new System.Drawing.Point(596, 38);
			this.btnBrowse.Name = "btnBrowse";
			this.btnBrowse.Size = new System.Drawing.Size(29, 23);
			this.btnBrowse.TabIndex = 2;
			this.btnBrowse.Text = "...";
			this.btnBrowse.UseVisualStyleBackColor = true;
			this.btnBrowse.Click += new System.EventHandler(this.btnBrowse_Click);
			// 
			// label9
			// 
			this.label9.AutoSize = true;
			this.label9.Location = new System.Drawing.Point(6, 18);
			this.label9.Name = "label9";
			this.label9.Size = new System.Drawing.Size(163, 17);
			this.label9.TabIndex = 10;
			this.label9.Text = "Atomic Bomberman Path";
			// 
			// txtPath
			// 
			this.txtPath.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
			this.txtPath.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.FileSystemDirectories;
			this.txtPath.Location = new System.Drawing.Point(9, 38);
			this.txtPath.Name = "txtPath";
			this.txtPath.Size = new System.Drawing.Size(576, 22);
			this.txtPath.TabIndex = 1;
			this.txtPath.TextChanged += new System.EventHandler(this.txtPath_TextChanged);
			// 
			// label8
			// 
			this.label8.AutoSize = true;
			this.label8.Location = new System.Drawing.Point(342, 93);
			this.label8.Name = "label8";
			this.label8.Size = new System.Drawing.Size(59, 17);
			this.label8.TabIndex = 4;
			this.label8.Text = "Scheme";
			// 
			// cbScheme
			// 
			this.cbScheme.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.Append;
			this.cbScheme.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
			this.cbScheme.Location = new System.Drawing.Point(407, 90);
			this.cbScheme.Name = "cbScheme";
			this.cbScheme.Size = new System.Drawing.Size(197, 24);
			this.cbScheme.TabIndex = 4;
			// 
			// label5
			// 
			this.label5.AutoSize = true;
			this.label5.Location = new System.Drawing.Point(23, 101);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(85, 17);
			this.label5.TabIndex = 2;
			this.label5.Text = "-1 = random";
			// 
			// label4
			// 
			this.label4.AutoSize = true;
			this.label4.Location = new System.Drawing.Point(6, 76);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(42, 17);
			this.label4.TabIndex = 1;
			this.label4.Text = "Tilset";
			// 
			// numTileset
			// 
			this.numTileset.Location = new System.Drawing.Point(58, 76);
			this.numTileset.Maximum = new decimal(new int[] {
            10,
            0,
            0,
            0});
			this.numTileset.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            -2147483648});
			this.numTileset.Name = "numTileset";
			this.numTileset.Size = new System.Drawing.Size(50, 22);
			this.numTileset.TabIndex = 3;
			this.numTileset.Value = new decimal(new int[] {
            1,
            0,
            0,
            -2147483648});
			// 
			// btnCancel
			// 
			this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.btnCancel.Location = new System.Drawing.Point(548, 577);
			this.btnCancel.Name = "btnCancel";
			this.btnCancel.Size = new System.Drawing.Size(95, 38);
			this.btnCancel.TabIndex = 7;
			this.btnCancel.Text = "Discard";
			this.btnCancel.UseVisualStyleBackColor = true;
			this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
			// 
			// btnOK
			// 
			this.btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.btnOK.Location = new System.Drawing.Point(447, 577);
			this.btnOK.Name = "btnOK";
			this.btnOK.Size = new System.Drawing.Size(95, 38);
			this.btnOK.TabIndex = 6;
			this.btnOK.Text = "Save && Exit";
			this.btnOK.UseVisualStyleBackColor = true;
			this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
			// 
			// btnRun
			// 
			this.btnRun.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.btnRun.Location = new System.Drawing.Point(346, 577);
			this.btnRun.Name = "btnRun";
			this.btnRun.Size = new System.Drawing.Size(95, 38);
			this.btnRun.TabIndex = 5;
			this.btnRun.Text = "Save && Run";
			this.btnRun.UseVisualStyleBackColor = true;
			this.btnRun.Click += new System.EventHandler(this.btnRun_Click);
			// 
			// ConfigForm
			// 
			this.AcceptButton = this.btnRun;
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
			this.CancelButton = this.btnCancel;
			this.ClientSize = new System.Drawing.Size(655, 627);
			this.Controls.Add(this.btnRun);
			this.Controls.Add(this.btnOK);
			this.Controls.Add(this.btnCancel);
			this.Controls.Add(this.groupBox4);
			this.Controls.Add(this.groupBox3);
			this.Controls.Add(this.groupBox2);
			this.Controls.Add(this.groupBox1);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
			this.Name = "ConfigForm";
			this.Text = "Configure Bomber Stuff";
			this.groupBox1.ResumeLayout(false);
			this.groupBox1.PerformLayout();
			this.groupBox2.ResumeLayout(false);
			this.groupBox2.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.numPlayers)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.picPreviewColor)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.trackLightness)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.trackSaturation)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.trackHue)).EndInit();
			this.groupBox3.ResumeLayout(false);
			this.groupBox4.ResumeLayout(false);
			this.groupBox4.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.numTileset)).EndInit();
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.ComboBox cbSize;
		private System.Windows.Forms.GroupBox groupBox2;
		private System.Windows.Forms.ComboBox cbPlayer;
		private System.Windows.Forms.Label lblLightnessValue;
		private System.Windows.Forms.Label lblSaturationValue;
		private System.Windows.Forms.Label lblHueValue;
		private System.Windows.Forms.PictureBox picPreviewColor;
		private System.Windows.Forms.Label lblLightness;
		private System.Windows.Forms.CheckBox checkSaturation;
		private System.Windows.Forms.CheckBox checkHue;
		private System.Windows.Forms.TrackBar trackLightness;
		private System.Windows.Forms.TrackBar trackSaturation;
		private System.Windows.Forms.TrackBar trackHue;
		private System.Windows.Forms.TextBox txtHeight;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.TextBox txtWidth;
		private System.Windows.Forms.GroupBox groupBox3;
		private System.Windows.Forms.RadioButton rbCustom;
		private System.Windows.Forms.RadioButton rbWinForms;
		private System.Windows.Forms.RadioButton rbSlimDX;
		private System.Windows.Forms.CheckBox checkFullscreen;
		private System.Windows.Forms.NumericUpDown numPlayers;
		private System.Windows.Forms.GroupBox groupBox4;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.NumericUpDown numTileset;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.CheckBox checkBox5;
		private System.Windows.Forms.CheckBox checkBox4;
		private System.Windows.Forms.CheckBox checkBox3;
		private System.Windows.Forms.CheckBox checkBox2;
		private System.Windows.Forms.CheckBox checkBox1;
		private System.Windows.Forms.CheckBox checkLeft;
		private System.Windows.Forms.Label label7;
		private System.Windows.Forms.Label label6;
		private System.Windows.Forms.Button btnCancel;
		private System.Windows.Forms.Button btnOK;
		private System.Windows.Forms.Label label8;
		private System.Windows.Forms.ComboBox cbScheme;
		private System.Windows.Forms.TextBox txtPath;
		private System.Windows.Forms.Label label9;
		private System.Windows.Forms.FolderBrowserDialog browsePath;
		private System.Windows.Forms.Button btnBrowse;
		private System.Windows.Forms.Label lblPathInvalid;
		private System.Windows.Forms.Button btnRun;
	}
}