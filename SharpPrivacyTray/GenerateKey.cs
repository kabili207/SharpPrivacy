//
// This file is part of the source code distribution of SharpPrivacy.
// SharpPrivacy is an Open Source OpenPGP implementation and can be 
// found at http://www.sharpprivacy.net
// It is released under Gnu General Public License and can be used 
// and modified as long as the result is released under GPL too. 
// For a copy of the GPL, please go to www.gnu.org/copyleft/gpl.html 
//
// GenerateKey.cs: 
// 	This class is a key generation wizard.
//
// Author:
//	Daniel Fabian (df@sharpprivacy.net)
//
//
// Version: 0.1.0 (initial release)
//
// Changelog:
//	- 13.05.2003: Created this file.
//	- 01.06.2003: Added this header for the first beta release.
//  - 14.06.2003: Changed Namespace to SharpPrivacy.SharpPrivacyTray
//
// (C) 2003, Daniel Fabian
//
using System;
using System.Windows.Forms;
using Crownwood.Magic.Controls;

namespace SharpPrivacy.SharpPrivacyTray {
	public class GenerateKey : Crownwood.Magic.Forms.WizardDialog {
		private Crownwood.Magic.Controls.WizardPage wpWelcome;
		private Crownwood.Magic.Controls.WizardPage wpKeyData;
		private Crownwood.Magic.Controls.WizardPage wpPassphrase;
		private Crownwood.Magic.Controls.WizardPage wpGeneration;
		private Crownwood.Magic.Controls.WizardPage wpDone;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.TextBox txtName;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.TextBox txtEmail;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.ComboBox cmbKeyType;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.ComboBox cmbKeySize;
		private System.Windows.Forms.Label label6;
		private System.Windows.Forms.RadioButton rbExpiration;
		private System.Windows.Forms.RadioButton rbNoExpiration;
		private System.Windows.Forms.DateTimePicker dtExpiration;
		private System.Windows.Forms.CheckBox chkEncryptionKey;
		private System.Windows.Forms.CheckBox chkSignatureKey;
		private System.Windows.Forms.CheckBox chkSelfSignatures;
		private System.Windows.Forms.ProgressBar pbTotalProgress;
		private System.Windows.Forms.Label label7;
		private System.Windows.Forms.ProgressBar pbCurrentProgress;
		private System.Windows.Forms.Timer timProgressBar;
		private System.Windows.Forms.Label label8;
		private System.Windows.Forms.Label label9;
		private System.Windows.Forms.Label lblPassphrase;
		private System.Windows.Forms.Label lblPassphraseText;
		private System.Windows.Forms.Label lblConfirmation;
		private System.Windows.Forms.TextBox txtPassphrase;
		private System.Windows.Forms.TextBox txtConfirmation;
		
		private bool bRising = true;
		private bool bCanceled = false;
		
		private System.Threading.Thread tThread;
		
		private static int iKeySize;
		
		public bool Canceled {
			get {
				return bCanceled;
			}
		}
		
		public GenerateKey() {
			InitializeMyComponent();
		}
		
		
		void InitializeMyComponent() {
			System.Resources.ResourceManager resources = new System.Resources.ResourceManager("SharpPrivacyTray", System.Reflection.Assembly.GetExecutingAssembly()); 
			this.Icon = (System.Drawing.Icon)resources.GetObject("menuKeyPair");
			this.ShowInTaskbar = false;
			
			this.wpWelcome = new Crownwood.Magic.Controls.WizardPage();
			this.label1 = new System.Windows.Forms.Label();
			this.wpKeyData = new Crownwood.Magic.Controls.WizardPage();
			this.wpPassphrase = new Crownwood.Magic.Controls.WizardPage();
			this.dtExpiration = new System.Windows.Forms.DateTimePicker();
			this.rbNoExpiration = new System.Windows.Forms.RadioButton();
			this.rbExpiration = new System.Windows.Forms.RadioButton();
			this.label6 = new System.Windows.Forms.Label();
			this.cmbKeySize = new System.Windows.Forms.ComboBox();
			this.label5 = new System.Windows.Forms.Label();
			this.cmbKeyType = new System.Windows.Forms.ComboBox();
			this.label4 = new System.Windows.Forms.Label();
			this.txtEmail = new System.Windows.Forms.TextBox();
			this.label3 = new System.Windows.Forms.Label();
			this.txtName = new System.Windows.Forms.TextBox();
			this.label2 = new System.Windows.Forms.Label();
			this.wpGeneration = new Crownwood.Magic.Controls.WizardPage();
			this.wpDone = new Crownwood.Magic.Controls.WizardPage();
			this.chkEncryptionKey = new System.Windows.Forms.CheckBox();
			this.chkSignatureKey = new System.Windows.Forms.CheckBox();
			this.chkSelfSignatures = new System.Windows.Forms.CheckBox();
			this.pbTotalProgress = new System.Windows.Forms.ProgressBar();
			this.label7 = new System.Windows.Forms.Label();
			this.pbCurrentProgress = new System.Windows.Forms.ProgressBar();
			this.timProgressBar = new System.Windows.Forms.Timer();
			this.label8 = new System.Windows.Forms.Label();
			this.label9 = new System.Windows.Forms.Label();
			this.lblConfirmation = new System.Windows.Forms.Label();
			this.lblPassphrase = new System.Windows.Forms.Label();
			this.lblPassphraseText = new System.Windows.Forms.Label();
			this.txtPassphrase = new System.Windows.Forms.TextBox();
			this.txtConfirmation = new System.Windows.Forms.TextBox();
			this.wpWelcome.SuspendLayout();
			this.wpKeyData.SuspendLayout();
			this.wpPassphrase.SuspendLayout();
			this.wpGeneration.SuspendLayout();
			this.wpDone.SuspendLayout();
			this.SuspendLayout();
			
			// 
			// wcKeyWizard
			// 
			this.wizardControl.Name = "wcKeyWizard";
			this.wizardControl.Profile = Crownwood.Magic.Controls.WizardControl.Profiles.Install;
			this.wizardControl.SelectedIndex = 3;
			this.wizardControl.Size = new System.Drawing.Size(506, 359);
			this.wizardControl.TabIndex = 0;
			this.wizardControl.WizardPages.AddRange(new Crownwood.Magic.Controls.WizardPage[] {
																								this.wpWelcome,
																								this.wpKeyData,
																								this.wpPassphrase,
																								this.wpGeneration,
																								this.wpDone});
			// 
			// wpWelcome
			// 
			this.wpWelcome.CaptionTitle = "Key Generation Wizard...";
			this.wpWelcome.Controls.AddRange(new System.Windows.Forms.Control[] {
																					this.label1});
			this.wpWelcome.FullPage = false;
			this.wpWelcome.Name = "wpWelcome";
			this.wpWelcome.Selected = false;
			this.wpWelcome.Size = new System.Drawing.Size(512, 231);
			this.wpWelcome.SubTitle = "This wizard guides you through the progress of creating a new OpenPGP keypair.";
			this.wpWelcome.TabIndex = 3;
			this.wpWelcome.Title = "Welcome to the Key Generation Wizard";
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(16, 16);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(432, 204);
			this.label1.TabIndex = 0;
			this.label1.Text = "This wizard guides you through the progress of creating a new OpenPGP keypair.\n\n" + 
			                   "You can use a pair of keys to sign data and enable your business partners to send you confidental data encrypted to your secret key.\n" + 
			                   "Effectively, you need a pair of keys to use SharpPrivacy to its full extent. Only if you just want to verify signed documents, you will not need a keypair of your own.\n\n" +
			                   "For more information about public key cryptography, please have a look into the helpfile.";
			// 
			// wpKeyData
			// 
			this.wpKeyData.CaptionTitle = "Key Generation Wizard...";
			this.wpKeyData.Controls.AddRange(new System.Windows.Forms.Control[] {
																					this.dtExpiration,
																					this.rbNoExpiration,
																					this.rbExpiration,
																					this.label6,
																					this.cmbKeySize,
																					this.label5,
																					this.cmbKeyType,
																					this.label4,
																					this.txtEmail,
																					this.label3,
																					this.txtName,
																					this.label2});
			this.wpKeyData.FullPage = false;
			this.wpKeyData.Name = "wpKeyData";
			this.wpKeyData.Selected = false;
			this.wpKeyData.Size = new System.Drawing.Size(512, 231);
			this.wpKeyData.SubTitle = "Here you can enter your personal information stored in the key, as well as " +
			                          "properties such as key length or type for the keypair itself.";
			this.wpKeyData.TabIndex = 4;
			this.wpKeyData.Title = "Personal Data";
			// 
			// dtExpiration
			// 
			this.dtExpiration.Location = new System.Drawing.Point(156, 144);
			this.dtExpiration.Name = "dtExpiration";
			this.dtExpiration.Size = new System.Drawing.Size(192, 21);
			this.dtExpiration.TabIndex = 11;
			// 
			// rbNoExpiration
			// 
			this.rbNoExpiration.Checked = true;
			this.rbNoExpiration.Location = new System.Drawing.Point(132, 172);
			this.rbNoExpiration.Name = "rbNoExpiration";
			this.rbNoExpiration.Size = new System.Drawing.Size(140, 16);
			this.rbNoExpiration.TabIndex = 10;
			this.rbNoExpiration.TabStop = true;
			this.rbNoExpiration.Text = "Key does not expire";
			// 
			// rbExpiration
			// 
			this.rbExpiration.Location = new System.Drawing.Point(132, 148);
			this.rbExpiration.Name = "rbExpiration";
			this.rbExpiration.Size = new System.Drawing.Size(16, 16);
			this.rbExpiration.TabIndex = 9;
			// 
			// label6
			// 
			this.label6.Location = new System.Drawing.Point(16, 148);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(92, 16);
			this.label6.TabIndex = 8;
			this.label6.Text = "Key Valid Until";
			// 
			// cmbKeySize
			// 
			this.cmbKeySize.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.cmbKeySize.Items.AddRange(new object[] {
															"768",
															"1024",
															"2048",
															"4096"});
			this.cmbKeySize.Location = new System.Drawing.Point(132, 108);
			this.cmbKeySize.Name = "cmbKeySize";
			this.cmbKeySize.Size = new System.Drawing.Size(216, 21);
			this.cmbKeySize.TabIndex = 7;
			this.cmbKeySize.SelectedIndex = 2;
			// 
			// label5
			// 
			this.label5.Location = new System.Drawing.Point(16, 112);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(88, 16);
			this.label5.TabIndex = 6;
			this.label5.Text = "Key Size";
			// 
			// cmbKeyType
			// 
			this.cmbKeyType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.cmbKeyType.Items.AddRange(new object[] {
															"ElGamal/DSA",
															"RSA"});
			this.cmbKeyType.Location = new System.Drawing.Point(132, 80);
			this.cmbKeyType.Name = "cmbKeyType";
			this.cmbKeyType.Size = new System.Drawing.Size(216, 21);
			this.cmbKeyType.TabIndex = 5;
			this.cmbKeyType.SelectedIndex = 0;
			// 
			// label4
			// 
			this.label4.Location = new System.Drawing.Point(16, 84);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(100, 16);
			this.label4.TabIndex = 4;
			this.label4.Text = "Key Type";
			// 
			// txtEmail
			// 
			this.txtEmail.Location = new System.Drawing.Point(132, 44);
			this.txtEmail.MaxLength = 50;
			this.txtEmail.Name = "txtEmail";
			this.txtEmail.Size = new System.Drawing.Size(216, 21);
			this.txtEmail.TabIndex = 3;
			this.txtEmail.Text = "";
			// 
			// label3
			// 
			this.label3.Location = new System.Drawing.Point(16, 48);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(108, 16);
			this.label3.TabIndex = 2;
			this.label3.Text = "Your Email Address";
			// 
			// txtName
			// 
			this.txtName.Location = new System.Drawing.Point(132, 16);
			this.txtName.MaxLength = 50;
			this.txtName.Name = "txtName";
			this.txtName.Size = new System.Drawing.Size(216, 21);
			this.txtName.TabIndex = 1;
			this.txtName.Text = "";
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(16, 20);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(76, 16);
			this.label2.TabIndex = 0;
			this.label2.Text = "Your Name";
			// 
			// wpPassphrase
			// 
			this.wpPassphrase.CaptionTitle = "Key Generation Wizard...";
			this.wpPassphrase.Controls.AddRange(new System.Windows.Forms.Control[] {
																					   this.lblPassphraseText,
																					   this.txtConfirmation,
																					   this.lblConfirmation,
																					   this.txtPassphrase,
																					   this.lblPassphrase});
			this.wpPassphrase.FullPage = false;
			this.wpPassphrase.Name = "wpPassphrase";
			this.wpPassphrase.Size = new System.Drawing.Size(512, 231);
			this.wpPassphrase.SubTitle = "Your private key will be encrypted to a passphrase, that only you know. Please en" +
				"ter a safe passphrase.";
			this.wpPassphrase.TabIndex = 7;
			this.wpPassphrase.Title = "Key Generation Wizard";
			// 
			// lblPassphrase
			// 
			this.lblPassphrase.Location = new System.Drawing.Point(16, 128);
			this.lblPassphrase.Name = "lblPassphrase";
			this.lblPassphrase.Size = new System.Drawing.Size(116, 16);
			this.lblPassphrase.TabIndex = 0;
			this.lblPassphrase.Text = "Passphrase";
			// 
			// txtPassphrase
			// 
			this.txtPassphrase.Location = new System.Drawing.Point(16, 144);
			this.txtPassphrase.Name = "txtPassphrase";
			this.txtPassphrase.Size = new System.Drawing.Size(436, 21);
			this.txtPassphrase.TabIndex = 1;
			this.txtPassphrase.Text = "";
			this.txtPassphrase.PasswordChar = '*';
			// 
			// lblConfirmation
			// 
			this.lblConfirmation.Location = new System.Drawing.Point(16, 180);
			this.lblConfirmation.Name = "lblConfirmation";
			this.lblConfirmation.Size = new System.Drawing.Size(172, 16);
			this.lblConfirmation.TabIndex = 2;
			this.lblConfirmation.Text = "Confirm your passphrase";
			// 
			// txtConfirmation
			// 
			this.txtConfirmation.Location = new System.Drawing.Point(16, 196);
			this.txtConfirmation.Name = "txtConfirmation";
			this.txtConfirmation.Size = new System.Drawing.Size(436, 21);
			this.txtConfirmation.TabIndex = 3;
			this.txtConfirmation.Text = "";
			this.txtConfirmation.PasswordChar = '*';
			// 
			// lblPassphraseText
			// 
			this.lblPassphraseText.Location = new System.Drawing.Point(16, 8);
			this.lblPassphraseText.Name = "lblPassphraseText";
			this.lblPassphraseText.Size = new System.Drawing.Size(460, 112);
			this.lblPassphraseText.TabIndex = 4;
			this.lblPassphraseText.Text = "To save your passphrase while it is stored on your harddisk, it is encrypted to a secret passphrase, that only you should know.\n\nThe passphrase should be at least 8 characters long and contain special characters like '$&§/=', upper and lower case letters, as well as numbers.\n\nBe sure to remember your passphrase, as you will not be able to use your private key without your passphrase!";
			// 
			// wpGeneration
			// 
			this.wpGeneration.CaptionTitle = "Key Generation Wizard...";
			this.wpGeneration.Controls.AddRange(new System.Windows.Forms.Control[] {
																					   this.pbCurrentProgress,
																					   this.label7,
																					   this.pbTotalProgress,
																					   this.chkSelfSignatures,
																					   this.chkSignatureKey,
																					   this.chkEncryptionKey,
																					   this.label9});
			this.wpGeneration.FullPage = false;
			this.wpGeneration.Name = "wpGeneration";
			this.wpGeneration.Selected = false;
			this.wpGeneration.Size = new System.Drawing.Size(512, 231);
			this.wpGeneration.SubTitle = "Your keys are being created. This progress may take several minutes on older comp" +
				"uters and cannot be canceled.";
			this.wpGeneration.TabIndex = 5;
			this.wpGeneration.Title = "Key Generation";
			//
			// timProgressBar
			//
			this.timProgressBar.Interval = 200;
			this.timProgressBar.Tick += new EventHandler(this.timProgressBar_Tick);
			
			// 
			// wpDone
			// 
			this.wpDone.CaptionTitle = "Key Generation Wizard";
			this.wpDone.Controls.AddRange(new System.Windows.Forms.Control[] {
																				 this.label8});
			this.wpDone.FullPage = false;
			this.wpDone.Name = "wpDone";
			this.wpDone.Size = new System.Drawing.Size(512, 231);
			this.wpDone.SubTitle = "Your keypair has been created and is ready for use.";
			this.wpDone.TabIndex = 6;
			this.wpDone.Title = "Key Generation is Complete";
			// 
			// chkEncryptionKey
			// 
			this.chkEncryptionKey.AutoCheck = false;
			this.chkEncryptionKey.Location = new System.Drawing.Point(24, 24);
			this.chkEncryptionKey.Name = "chkEncryptionKey";
			this.chkEncryptionKey.Size = new System.Drawing.Size(176, 16);
			this.chkEncryptionKey.TabIndex = 0;
			this.chkEncryptionKey.Text = "Generating Encryption Key...";
			// 
			// chkSignatureKey
			// 
			this.chkSignatureKey.AutoCheck = false;
			this.chkSignatureKey.Location = new System.Drawing.Point(24, 48);
			this.chkSignatureKey.Name = "chkSignatureKey";
			this.chkSignatureKey.Size = new System.Drawing.Size(176, 16);
			this.chkSignatureKey.TabIndex = 1;
			this.chkSignatureKey.Text = "Generating Signature Key...";
			// 
			// chkSelfSignatures
			// 
			this.chkSelfSignatures.AutoCheck = false;
			this.chkSelfSignatures.Location = new System.Drawing.Point(24, 72);
			this.chkSelfSignatures.Name = "chkSelfSignatures";
			this.chkSelfSignatures.Size = new System.Drawing.Size(176, 20);
			this.chkSelfSignatures.TabIndex = 2;
			this.chkSelfSignatures.Text = "Selfsigning Keys...";
			// 
			// pbTotalProgress
			// 
			this.pbTotalProgress.Location = new System.Drawing.Point(20, 192);
			this.pbTotalProgress.Name = "pbTotalProgress";
			this.pbTotalProgress.Size = new System.Drawing.Size(476, 24);
			this.pbTotalProgress.TabIndex = 3;
			this.pbTotalProgress.Minimum = 0;
			this.pbTotalProgress.Maximum = 100;
			this.pbTotalProgress.Value = 0;
			// 
			// label7
			// 
			this.label7.Location = new System.Drawing.Point(20, 176);
			this.label7.Name = "label7";
			this.label7.Size = new System.Drawing.Size(104, 15);
			this.label7.TabIndex = 4;
			this.label7.Text = "Total Progress";
			// 
			// label9
			// 
			this.label9.Location = new System.Drawing.Point(20, 136);
			this.label9.Name = "label8";
			this.label9.Size = new System.Drawing.Size(250, 15);
			this.label9.TabIndex = 4;
			this.label9.Text = "WARNING! This may take up to 30 minutes!!!";
			//
			// pbCurrentProgress
			// 
			this.pbCurrentProgress.Location = new System.Drawing.Point(224, 68);
			this.pbCurrentProgress.Name = "pbCurrentProgress";
			this.pbCurrentProgress.Size = new System.Drawing.Size(268, 20);
			this.pbCurrentProgress.Step = 1;
			this.pbCurrentProgress.TabIndex = 5;
			this.pbCurrentProgress.Minimum = 0;
			this.pbCurrentProgress.Maximum = 30;
			// 
			// label8
			// 
			this.label8.Location = new System.Drawing.Point(20, 16);
			this.label8.Name = "label8";
			this.label8.Size = new System.Drawing.Size(464, 204);
			this.label8.TabIndex = 0;
			this.label8.Text = "The creation of your keys has been successfully completed. To add more userids to" +
				" the key open the key in the key manager. Furthermore you can use your key to si" +
				"gn data, as well as decrypt data that has been encrypted to your secret key.";
			// 
			// GenerateKey
			// 
			this.Name = "GenerateKey";
			this.wizardControl.Title = "Welcome to the Key Generation Wizard";
			this.Text = "Key Generator Wizard";
			this.Size = new System.Drawing.Size(506, 379);
			this.wpWelcome.ResumeLayout(false);
			this.wpKeyData.ResumeLayout(false);
			this.wpGeneration.ResumeLayout(false);
			this.wpPassphrase.ResumeLayout(false);
			this.wpDone.ResumeLayout(false);
			this.ResumeLayout(false);

		}
		
		private void timProgressBar_Tick(Object sender, EventArgs e) {
			if (this.bRising)
				this.pbCurrentProgress.Value++;
			else
				this.pbCurrentProgress.Value--;
			
			if (this.pbCurrentProgress.Maximum == this.pbCurrentProgress.Value)
				this.bRising = false;
			else if (this.pbCurrentProgress.Minimum == this.pbCurrentProgress.Value)
				this.bRising = true;
			
			Application.DoEvents();
		}
		
		protected override void OnWizardPageEnter(Crownwood.Magic.Controls.WizardPage wp, Crownwood.Magic.Controls.WizardControl wc) {
			if (this.wizardControl.SelectedIndex == 3) {
				iKeySize = Convert.ToInt32(cmbKeySize.Text);
				this.pbCurrentProgress.Value = 0;
				this.timProgressBar.Start();
				this.wizardControl.CancelButton.Enabled = false;
				
				iKeySize = Int32.Parse(this.cmbKeySize.Text);
				SharpPrivacy.ReloadKeyRing();
				tThread = new System.Threading.Thread(new System.Threading.ThreadStart(Start));
				tThread.Start();
				while (tThread.IsAlive) {
					System.Threading.Thread.Sleep(100);
					Application.DoEvents();
				}
				SharpPrivacy.ReloadKeyRing();
			
				this.wizardControl.CancelButton.Enabled = true;
				
				// Move to last page
				this.wizardControl.SelectedIndex = this.wizardControl.WizardPages.Count - 1;
			}
		}
		
		private void Start() {
			long lExpiration = this.dtExpiration.Value.Ticks;
			string strPassphrase = this.txtPassphrase.Text;
			SharpPrivacy.Instance.GenerateKey(txtName.Text, txtEmail.Text, cmbKeyType.Text, iKeySize, lExpiration, strPassphrase);
		}
		
		protected override void OnNextClick(Object sender, System.ComponentModel.CancelEventArgs e) {
			if (this.wizardControl.SelectedIndex == 2) {
				if (this.txtConfirmation.Text != this.txtPassphrase.Text) {
					MessageBox.Show("Your passphrase and confirmation are not the same. Please correct your passphrase.", "Error...", MessageBoxButtons.OK, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1);
					e.Cancel = true;
					return;
				}
				if (this.txtConfirmation.Text.Length < 8) {
					DialogResult drResult = MessageBox.Show("Your passphrase is shorter than 8 characters. It is STRONGLY recommended to use a passphrase of at least 8 characters. Are you sure you want to continue?", "Short Passphrase...", MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1);
					if (drResult == DialogResult.No) {
						e.Cancel = true;
						return;
					}
				}
			}
			if (this.wizardControl.SelectedIndex == 1) {
				if ((this.txtEmail.Text.Length < 5) || (this.txtName.Text.Length < 3)) {
					MessageBox.Show("Please enter your full name as well as your email address!", "Error...", MessageBoxButtons.OK, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1);
					e.Cancel = true;
					return;
				}
				if (this.cmbKeyType.SelectedIndex == 1) {
					MessageBox.Show("This first beta version does not support the creation of RSA keys. Please wait for the next releases!", "Not Supported...", MessageBoxButtons.OK, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1);
					cmbKeyType.SelectedIndex = 0;
					e.Cancel = true;
					return;
				}
			}
		}
		
		protected override void OnCancelClick(object sender, System.EventArgs e) {
			base.OnCancelClick(sender, e);
			try {
				tThread.Abort();
			} catch (Exception) {}
			this.timProgressBar.Stop();
			this.bCanceled = true;
		}
		
	}
}
