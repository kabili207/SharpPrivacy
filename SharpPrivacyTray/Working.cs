//
// This file is part of the source code distribution of SharpPrivacy.
// SharpPrivacy is an Open Source OpenPGP implementation and can be 
// found at http://www.sharpprivacy.net
// It is released under Gnu General Public License and can be used 
// and modified as long as the result is released under GPL too. 
// For a copy of the GPL, please go to www.gnu.org/copyleft/gpl.html 
//
// Working.cs: 
// 	GUI for showing that the program is currently working.
//
// Author:
//	Daniel Fabian (df@sharpprivacy.net)
//
//
// Version: 0.1.0 (initial release)
//
// Changelog:
//	- 02.04.2003: Created this file.
//	- 01.06.2003: Added this header for the first beta release.
//  - 14.06.2003: Changed Namespace to SharpPrivacy.SharpPrivacyTray
//
// (C) 2003, Daniel Fabian
//
using System;
using System.Windows.Forms;

namespace SharpPrivacy.SharpPrivacyTray {
	public class Working : System.Windows.Forms.Form {
		private System.Windows.Forms.Label label;
		private System.Windows.Forms.Button cmdCancel;
		private System.Windows.Forms.ProgressBar pbProgress;
		private System.Windows.Forms.PictureBox pbIcon;
		
		public Working() {
			InitializeComponent();
			System.Resources.ResourceManager resources = new System.Resources.ResourceManager("SharpPrivacy", System.Reflection.Assembly.GetExecutingAssembly()); 
			this.pbIcon.Image = ((System.Drawing.Icon)resources.GetObject("iconWorking")).ToBitmap();
			this.Icon = (System.Drawing.Icon)resources.GetObject("iconWorking");
			this.pbProgress.Minimum = 0;
			this.pbProgress.Maximum = 100;
			Application.DoEvents();
		}
		
		public void Progress(int iProgress) {
			this.pbProgress.Increment(iProgress);
			Application.DoEvents();
		}
		
		
		private void InitializeComponent() {
			this.label = new System.Windows.Forms.Label();
			this.cmdCancel = new System.Windows.Forms.Button();
			this.pbProgress = new System.Windows.Forms.ProgressBar();
			this.pbIcon = new System.Windows.Forms.PictureBox();
			this.SuspendLayout();
			// 
			// label
			// 
			this.label.Location = new System.Drawing.Point(16, 8);
			this.label.Name = "label";
			this.label.Size = new System.Drawing.Size(264, 16);
			this.label.TabIndex = 1;
			this.label.Text = "Please allow a second for the action to be taken...";
			// 
			// cmdCancel
			// 
			this.cmdCancel.Enabled = false;
			this.cmdCancel.Location = new System.Drawing.Point(196, 72);
			this.cmdCancel.Name = "cmdCancel";
			this.cmdCancel.Size = new System.Drawing.Size(88, 24);
			this.cmdCancel.TabIndex = 3;
			this.cmdCancel.Text = "Cancel";
			// 
			// pbProgress
			// 
			this.pbProgress.Location = new System.Drawing.Point(68, 40);
			this.pbProgress.Name = "pbProgress";
			this.pbProgress.Size = new System.Drawing.Size(216, 16);
			this.pbProgress.TabIndex = 0;
			// 
			// pbIcon
			// 
			this.pbIcon.Location = new System.Drawing.Point(12, 36);
			this.pbIcon.Name = "pbIcon";
			this.pbIcon.Size = new System.Drawing.Size(36, 32);
			this.pbIcon.TabIndex = 2;
			this.pbIcon.TabStop = false;
			// 
			// Working
			// 
			this.Text = "Please wait...";
			this.ClientSize = new System.Drawing.Size(292, 105);
			this.ShowInTaskbar = false;
			this.Controls.AddRange(new System.Windows.Forms.Control[] {
						this.cmdCancel,
						this.pbIcon,
						this.label,
						this.pbProgress});
			
			
		}
	}
}
