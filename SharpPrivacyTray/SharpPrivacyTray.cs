//
// This file is part of the source code distribution of SharpPrivacy.
// SharpPrivacy is an Open Source OpenPGP implementation and can be 
// found at http://www.sharpprivacy.net
// It is released under Gnu General Public License and can be used 
// and modified as long as the result is released under GPL too. 
// For a copy of the GPL, please go to www.gnu.org/copyleft/gpl.html 
//
// SharpPrivacyMain.cs: 
// 	The SharpPrivacy Tray Menu
//
// Author:
//	Daniel Fabian (df@sharpprivacy.net)
//
// Changelog:
//	- 21.02.2002: Created this file
//	- 01.06.2003: Added this header for the first beta release.
//  - 14.06.2003: Changed Namespace to SharpPrivacy.SharpPrivacyTray
//  - 21.06.2003: Finished making the changes to act as SharpPrivacy
//                plugin.
//
// Version: 0.2.0 (initial release)
//
using System;
using System.Windows.Forms;
using System.Drawing;
using System.IO;
using Crownwood.Magic.Menus;
using System.Collections;
using System.Reflection;
using System.Xml;
using System.Xml.XPath;


namespace SharpPrivacy.SharpPrivacyTray {
	class SharpPrivacyTray : System.Windows.Forms.Form {
		
		private Bitmap bmpMnuClipboard;
		private Bitmap bmpMnuCurrentWindow;
		private Bitmap bmpMnuKeyManager;
		private Bitmap bmpMnuFileSystem;
		private Bitmap bmpMnuAbout;
		
		private NotifyIcon niTrayIcon = new NotifyIcon();
		private PopupMenu pmnuTrayMenu = new PopupMenu();
		
		private MenuCommand mnuKeyManager = new MenuCommand("Key Manager");
		private MenuCommand mnuSeperator2 = new MenuCommand("-");
		private MenuCommand mnuClipboard = new MenuCommand("Clipboard");
		private MenuCommand mnuFileSystem = new MenuCommand("File System");
		private MenuCommand mnuCurrentWindow = new MenuCommand("CurrentWindow");
		private MenuCommand mnuSeperator1 = new MenuCommand("-");
		private MenuCommand mnuAbout = new MenuCommand("About");
		private MenuCommand mnuExit = new MenuCommand("Exit");
		
		private MenuCommand mnuClipboardEmpty = new MenuCommand("Empty");
		private MenuCommand mnuClipboardEdit = new MenuCommand("Edit");
		private	MenuCommand mnuClipboardSeperator1 = new MenuCommand("-");
		private	MenuCommand mnuClipboardDecryptVerify = new MenuCommand("Decrypt && Verify");
		private	MenuCommand mnuClipboardEncryptSign = new MenuCommand("Encrypt && Sign");
		private	MenuCommand mnuClipboardSign = new MenuCommand("Sign");
		private	MenuCommand mnuClipboardEncrypt = new MenuCommand("Encrypt");
		
		private	MenuCommand mnuFileSystemDecryptVerify = new MenuCommand("Decrypt && Verify");
		private	MenuCommand mnuFileSystemEncryptSign = new MenuCommand("Encrypt && Sign");
		private	MenuCommand mnuFileSystemSign = new MenuCommand("Sign");
		private	MenuCommand mnuFileSystemEncrypt = new MenuCommand("Encrypt");
		
		private	MenuCommand mnuCurrentWindowDecryptVerify = new MenuCommand("Decrypt && Verify");
		private	MenuCommand mnuCurrentWindowEncyrptSign = new MenuCommand("Encrypt && Sign");
		private	MenuCommand mnuCurrentWindowSign = new MenuCommand("Sign");
		private	MenuCommand mnuCurrentWindowEncrypt = new MenuCommand("Encrypt");
		
		public SharpPrivacyTray() {
			DateTime dtStart = DateTime.Now;
			SplashScreen ssSplash = new SplashScreen();
			ssSplash.Show();
			Application.DoEvents();
			
			// check if private and public keyring exists
			bool bMessageShowed = false;
			string strPath = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
			if (!System.IO.File.Exists(strPath + "/SharpPrivacy/pub_keyring.txt")) {
				MessageBox.Show("SharpPrivacy was unable to find a keyring. This might be due to you starting this program for the first time. A new keyring will be created in your home directory.", "Keyring...", MessageBoxButtons.OK, MessageBoxIcon.Asterisk, MessageBoxDefaultButton.Button1);
				bMessageShowed = true;
				try {
					System.IO.Directory.CreateDirectory(strPath + "/SharpPrivacy");
				} catch (Exception e) {
					MessageBox.Show("Error creating the SharpPrivacy home directory:" + e.Message, "Error...", MessageBoxButtons.OK, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1);
				}
				try {
					FileStream fsTmp = System.IO.File.Create(strPath + "/SharpPrivacy/pub_keyring.txt");
					fsTmp.Close();
				} catch (Exception e) {
					MessageBox.Show("Error creating the public keyring file:" + e.Message, "Error...", MessageBoxButtons.OK, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1);
				}
			}
			
			if (!System.IO.File.Exists(strPath + "/SharpPrivacy/sec_keyring.txt")) {
				if (!bMessageShowed)
					MessageBox.Show("SharpPrivacy was unable to find a keyring. This might be due to you starting this program for the first time. A new keyring will be created in your home directory.", "Keyring...", MessageBoxButtons.OK, MessageBoxIcon.Asterisk, MessageBoxDefaultButton.Button1);
				bMessageShowed = true;
				try {
					FileStream fsTmp = System.IO.File.Create(strPath + "/SharpPrivacy/sec_keyring.txt");
					fsTmp.Close();
				} catch (Exception e) {
					MessageBox.Show("Error creating the secret keyring file:" + e.Message, "Error...", MessageBoxButtons.OK, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1);
				}
			}
			
			SharpPrivacy.ReloadKeyRing();
			
			InitializeComponent();
			InitializeCustomComponents();
			this.WindowState = FormWindowState.Minimized;			
			this.ResumeLayout(false);
			
			int iCount = 0;
			while (dtStart.AddSeconds(5) > DateTime.Now) {
				if (iCount % 10 == 0)
					Application.DoEvents();
				iCount++;
				System.Threading.Thread.Sleep(50);
			}
			ssSplash.Hide();
			
		}
		
		void InitializeCustomComponents() {
			
			System.Resources.ResourceManager resources = new System.Resources.ResourceManager("SharpPrivacyTray", Assembly.GetExecutingAssembly()); 
			this.bmpMnuClipboard = ((System.Drawing.Icon)resources.GetObject("menuClipboard")).ToBitmap();
			this.bmpMnuCurrentWindow = ((System.Drawing.Icon)resources.GetObject("menuCurrentWindow")).ToBitmap();
			this.bmpMnuFileSystem = ((System.Drawing.Icon)resources.GetObject("menuFileSystem")).ToBitmap();
			this.bmpMnuKeyManager = ((System.Drawing.Icon)resources.GetObject("menuKeyManager")).ToBitmap();
			this.bmpMnuAbout = ((Icon)resources.GetObject("menuAbout")).ToBitmap();
			
			Icon iTrayIcon = (System.Drawing.Icon)resources.GetObject("trayIcon");
			this.Icon = iTrayIcon;
			
			mnuExit.Click += new EventHandler(this.mnuExit_Click);
			mnuKeyManager.Click += new EventHandler(this.mnuKeyManager_Click);
			mnuClipboardDecryptVerify.Click += new EventHandler(this.mnuClipboardDecryptVerify_Click);
			mnuClipboardEncrypt.Click += new EventHandler(this.mnuClipboardEncrypt_Click);
			mnuClipboardSign.Click += new EventHandler(this.mnuClipboardSign_Click);
			mnuClipboardEncryptSign.Click += new EventHandler(this.mnuClipboardEncryptSign_Click);
			mnuAbout.Click += new EventHandler(this.mnuAbout_Click);
			
			mnuFileSystemDecryptVerify.Click += new EventHandler(this.mnuFileSystemDecryptVerify_Click);
			mnuFileSystemEncrypt.Click += new EventHandler(this.mnuFileSystemEncrypt_Click);
			mnuFileSystemSign.Click += new EventHandler(this.mnuFileSystemSign_Click);
			mnuFileSystemEncryptSign.Click += new EventHandler(this.mnuFileSystemEncryptSign_Click);
			
			mnuKeyManager.Image = bmpMnuKeyManager;
			mnuClipboard.Image = bmpMnuClipboard;
			mnuCurrentWindow.Image = bmpMnuCurrentWindow;
			mnuFileSystem.Image = bmpMnuFileSystem;
			mnuAbout.Image = bmpMnuAbout;
			
			mnuFileSystem.MenuCommands.Add(mnuFileSystemDecryptVerify);
			mnuFileSystem.MenuCommands.Add(mnuFileSystemEncryptSign);
			mnuFileSystem.MenuCommands.Add(mnuFileSystemSign);
			mnuFileSystem.MenuCommands.Add(mnuFileSystemEncrypt);
			
			mnuClipboard.MenuCommands.Add(mnuClipboardEmpty);
			mnuClipboard.MenuCommands.Add(mnuClipboardEdit);
			mnuClipboard.MenuCommands.Add(mnuClipboardSeperator1);
			mnuClipboard.MenuCommands.Add(mnuClipboardDecryptVerify);
			mnuClipboard.MenuCommands.Add(mnuClipboardEncryptSign);
			mnuClipboard.MenuCommands.Add(mnuClipboardSign);
			mnuClipboard.MenuCommands.Add(mnuClipboardEncrypt);
			
			mnuClipboardDecryptVerify.Image = ((Icon)resources.GetObject("listSecretKey")).ToBitmap();
			mnuClipboardEncrypt.Image = ((Icon)resources.GetObject("listPublicKey")).ToBitmap();
			mnuFileSystemDecryptVerify.Image = ((Icon)resources.GetObject("listSecretKey")).ToBitmap();
			mnuFileSystemEncrypt.Image = ((Icon)resources.GetObject("listPublicKey")).ToBitmap();
			
			mnuCurrentWindow.Visible = false;
			mnuCurrentWindow.MenuCommands.Add(mnuCurrentWindowDecryptVerify);
			mnuCurrentWindow.MenuCommands.Add(mnuCurrentWindowEncyrptSign);
			mnuCurrentWindow.MenuCommands.Add(mnuCurrentWindowSign);
			mnuCurrentWindow.MenuCommands.Add(mnuCurrentWindowEncrypt);
			
			pmnuTrayMenu.MenuCommands.Add(mnuAbout);
			pmnuTrayMenu.MenuCommands.Add(mnuKeyManager);
			pmnuTrayMenu.MenuCommands.Add(mnuSeperator2);
			pmnuTrayMenu.MenuCommands.Add(mnuCurrentWindow);
			pmnuTrayMenu.MenuCommands.Add(mnuFileSystem);
			pmnuTrayMenu.MenuCommands.Add(mnuClipboard);
			pmnuTrayMenu.MenuCommands.Add(mnuSeperator1);
			pmnuTrayMenu.MenuCommands.Add(mnuExit);
			
			pmnuTrayMenu.Animate = Crownwood.Magic.Menus.Animate.Yes;
			pmnuTrayMenu.AnimateTime = 100;
			pmnuTrayMenu.Style = Crownwood.Magic.Common.VisualStyle.IDE;
			
			niTrayIcon.Icon = iTrayIcon;
			niTrayIcon.MouseUp += new MouseEventHandler(niTrayIcon_MouseUp);
			niTrayIcon.Visible = true;
			
			this.ShowInTaskbar = false;
			
			
		}
		
		// This method is used in the forms designer.
		// Change this method on you own risk
		void InitializeComponent() {
			this.SuspendLayout();
			// 
			// frmMain
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(292, 273);
			this.Name = "SharpPrivacyTray";
			this.Text = "SharpPrivacy Tray Plugin";
			this.ResumeLayout(false);
		}
		
		void mnuKeyManager_Click(Object sender, System.EventArgs e) {
			try {
				KeyManager keyManagerForm = new KeyManager();
				keyManagerForm.Show();
			} catch (Exception ex) {
				MessageBox.Show("An error occured while trying to open the key manager:" + ex.Message, "Error...", MessageBoxButtons.OK, MessageBoxIcon.Hand, MessageBoxDefaultButton.Button1);
			}
			
		}
		
		void mnuExit_Click(Object sender, System.EventArgs e) {
			this.Hide();
			niTrayIcon.Dispose();
			pmnuTrayMenu = null;
			this.Dispose();
			this.Close();
			Application.Exit();
		}
		
		void mnuClipboardDecryptVerify_Click(Object sender, System.EventArgs e) {
			string strMessage = Clipboard.GetDataObject().GetData(DataFormats.Text).ToString();
			
			bool bAskForPassphrase = true;
			ulong lDecryptionKey = 0;
			try {
				SharpPrivacy.ReloadKeyRing();
				lDecryptionKey = SharpPrivacy.Instance.GetDecryptionKey(strMessage);
			} catch (Exception ex) {
				bAskForPassphrase = false;
				Console.WriteLine(ex.Message);
			}
			
			string strPassphrase = "";
			if (bAskForPassphrase) {
				QueryPassphrase qpPassphrase = new QueryPassphrase();
				if (lDecryptionKey > 0) {
					//Find the fitting key
					XmlElement xmlKey = FindSecretKey(lDecryptionKey);
					if (xmlKey == null) {
						MessageBox.Show("An unexpected error occured: The secret key used to decrypt the message could not be found.", "Error...", MessageBoxButtons.OK, MessageBoxIcon.Hand, MessageBoxDefaultButton.Button1);
						return;
					}
					qpPassphrase.ShowSingleKeyDialog(xmlKey);
					strPassphrase = qpPassphrase.Passphrase;
				} else {
					qpPassphrase.ShowMyDialog();
					strPassphrase = qpPassphrase.Passphrase;
				}
			}
			
			try {
				PlaintextViewer pvPlaintext = new PlaintextViewer();
				SharpPrivacy.ReloadKeyRing();
				pvPlaintext.XmlMessage = SharpPrivacy.Instance.DecryptAndVerify(strMessage, strPassphrase);
				pvPlaintext.ShowPlaintext();
			} catch (Exception ex) {
				MessageBox.Show("An error occured while decrypting the message: " + ex.Message, "Error...", MessageBoxButtons.OK, MessageBoxIcon.Hand, MessageBoxDefaultButton.Button1);
			}
		}
		
		XmlElement FindSecretKey(ulong lKeyID) {
			XmlNodeList xnlSecretKeys = SharpPrivacy.SecretKeyRing.GetElementsByTagName("SecretKey");
			IEnumerator ieKeys = xnlSecretKeys.GetEnumerator();
			while (ieKeys.MoveNext()) {
				XmlElement xmlKey = (XmlElement)ieKeys.Current;
				
				string strKeyID = xmlKey.GetAttribute("keyid");
				ulong lThisKey = UInt64.Parse(strKeyID.Substring(2), System.Globalization.NumberStyles.HexNumber);
				if (lKeyID == lThisKey)
					return xmlKey;
				
				XmlNodeList xnlSubkeys = xmlKey.GetElementsByTagName("Subkey");
				IEnumerator ieSubkeys = xnlSubkeys.GetEnumerator();
				while (ieSubkeys.MoveNext()) {
					XmlElement xmlSubkey = (XmlElement)ieSubkeys.Current;
					
					strKeyID = xmlSubkey.GetAttribute("keyid");
					lThisKey = UInt64.Parse(strKeyID.Substring(2), System.Globalization.NumberStyles.HexNumber);
					
					if (lKeyID == lThisKey)
						return xmlKey;
				}
				
			}
			
			return null;
		}
		
		void mnuFileSystemSign_Click(Object sender, System.EventArgs e) {
			
			// Check if we have some secret keys at all.
			XmlNodeList xnlSecretKeys = SharpPrivacy.SecretKeyRing.GetElementsByTagName("SecretKey");
			
			if (xnlSecretKeys.Count == 0) {
				MessageBox.Show("You cannot sign data because you do not own a private key. Please generate a new key pair, before you sign data.", "Action not possible...", MessageBoxButtons.OK, MessageBoxIcon.Asterisk, MessageBoxDefaultButton.Button1);
				return;
			}
			
			System.Windows.Forms.OpenFileDialog ofdOpen = new System.Windows.Forms.OpenFileDialog();
			
			ofdOpen.Multiselect = true;
			ofdOpen.Filter = "All Files (*.*)|*.*";
			ofdOpen.ShowDialog();
			
			if (ofdOpen.FileNames.Length == 0)
				return;
			
			QueryPassphrase qpPassphrase = new QueryPassphrase();
			qpPassphrase.ShowMultiKeyDialog(SharpPrivacy.SecretKeyRing);
			ulong lSignatureKeyID = qpPassphrase.SelectedKey;
			string strPassphrase = qpPassphrase.Passphrase;

			for (int i=0; i<ofdOpen.FileNames.Length; i++) {
				string strPath = ofdOpen.FileNames[i];
				SharpPrivacy.ReloadKeyRing();
				SharpPrivacy.Instance.SignFile(strPath, strPath + ".asc", lSignatureKeyID, strPassphrase);
			}
		}
		
		void mnuClipboardEncrypt_Click(Object sender, System.EventArgs e) {
			
			string strMessage = Clipboard.GetDataObject().GetData(DataFormats.Text).ToString();
			
			PublicKeySelector pksSelector = new PublicKeySelector(SharpPrivacy.PublicKeyRing);
			pksSelector.ShowDialog();
			
			ulong[] lTargetKeyIDs = (ulong[])pksSelector.SelectedKeys.ToArray(Type.GetType("System.UInt64"));
			
			try {
				SharpPrivacy.ReloadKeyRing();
				string strReturn = SharpPrivacy.Instance.EncryptText(strMessage, lTargetKeyIDs);
				Clipboard.SetDataObject(strReturn);
			} catch (Exception ex) {
				MessageBox.Show("An error occured while encrypting the message: " + ex.Message + "\n\n" + ex.StackTrace, "Error...", MessageBoxButtons.OK, MessageBoxIcon.Hand, MessageBoxDefaultButton.Button1);
			}
		
			
		}
		
		void mnuClipboardEncryptSign_Click(Object sender, System.EventArgs e) {
			// Check if we have some secret keys at all.
			XmlNodeList xnlSecretKeys = SharpPrivacy.SecretKeyRing.GetElementsByTagName("SecretKey");
			
			if (xnlSecretKeys.Count == 0) {
				MessageBox.Show("You cannot sign data because you do not own a private key. Please generate a new key pair, before you sign data.", "Action not possible...", MessageBoxButtons.OK, MessageBoxIcon.Asterisk, MessageBoxDefaultButton.Button1);
				return;
			}
			
			string strMessage = Clipboard.GetDataObject().GetData(DataFormats.Text).ToString();
			
			PublicKeySelector pksSelector = new PublicKeySelector(SharpPrivacy.PublicKeyRing);
			pksSelector.ShowDialog();
			
			ulong[] lTargetKeyIDs = (ulong[])pksSelector.SelectedKeys.ToArray(Type.GetType("System.UInt64"));
			
			QueryPassphrase qpPassphrase = new QueryPassphrase();
			qpPassphrase.ShowMultiKeyDialog(SharpPrivacy.SecretKeyRing);
			ulong lSignatureKeyID = qpPassphrase.SelectedKey;
			string strPassphrase = qpPassphrase.Passphrase;
			
			try {
				SharpPrivacy.ReloadKeyRing();
				string strReturn = SharpPrivacy.Instance.EncryptAndSignText(strMessage, lTargetKeyIDs, lSignatureKeyID, strPassphrase);
				Clipboard.SetDataObject(strReturn);
			} catch (Exception ex) {
				MessageBox.Show("An error occured while encrypting the message: " + ex.Message + "\n\n" + ex.StackTrace, "Error...", MessageBoxButtons.OK, MessageBoxIcon.Hand, MessageBoxDefaultButton.Button1);
			}
		
		}

		void mnuFileSystemDecryptVerify_Click(Object sender, System.EventArgs e) {
			System.Windows.Forms.OpenFileDialog ofdOpen = new System.Windows.Forms.OpenFileDialog();
			
			ofdOpen.Multiselect = false;
			ofdOpen.Filter = "OpenPGP Files (*.asc;*.enc)|*.asc;*.enc|All Files (*.*)|*.*";
			ofdOpen.ShowDialog();
			
			bool bAskForPassphrase = true;
			ulong lDecryptionKey = 0;
			try {
				SharpPrivacy.ReloadKeyRing();
				lDecryptionKey = SharpPrivacy.Instance.GetDecryptionKeyFromFile(ofdOpen.FileName);
			} catch (Exception) {
				bAskForPassphrase = false;
			}
			
			string strPassphrase = "";
			if (bAskForPassphrase) {
				QueryPassphrase qpPassphrase = new QueryPassphrase();
				if (lDecryptionKey > 0) {
					//Find the fitting key
					XmlElement xmlKey = FindSecretKey(lDecryptionKey);
					qpPassphrase.ShowSingleKeyDialog(xmlKey);
					strPassphrase = qpPassphrase.Passphrase;
				} else {
					qpPassphrase.ShowMyDialog();
					strPassphrase = qpPassphrase.Passphrase;
				}
			}
			
			try {
				PlaintextViewer pvPlaintext = new PlaintextViewer();
				SharpPrivacy.ReloadKeyRing();
				pvPlaintext.XmlMessage = SharpPrivacy.Instance.DecryptAndVerifyFile(ofdOpen.FileName, strPassphrase);
				pvPlaintext.ShowPlaintext();
			} catch (Exception ex) {
				MessageBox.Show("An error occured while decrypting the message: " + ex.Message, "Error...", MessageBoxButtons.OK, MessageBoxIcon.Hand, MessageBoxDefaultButton.Button1);
			}
			
				
		}
		
		void mnuClipboardSign_Click(Object sender, System.EventArgs e) {
			// Check if we have some secret keys at all.
			XmlNodeList xnlSecretKeys = SharpPrivacy.SecretKeyRing.GetElementsByTagName("SecretKey");
			
			if (xnlSecretKeys.Count == 0) {
				MessageBox.Show("You cannot sign data because you do not own a private key. Please generate a new key pair, before you sign data.", "Action not possible...", MessageBoxButtons.OK, MessageBoxIcon.Asterisk, MessageBoxDefaultButton.Button1);
				return;
			}
			
			string strMessage = Clipboard.GetDataObject().GetData(DataFormats.Text).ToString();
			
			QueryPassphrase qpPassphrase = new QueryPassphrase();
			qpPassphrase.ShowMultiKeyDialog(SharpPrivacy.SecretKeyRing);
			ulong lSignatureKeyID = qpPassphrase.SelectedKey;
			string strPassphrase = qpPassphrase.Passphrase;
			
			try {
				SharpPrivacy.ReloadKeyRing();
				string strReturn = SharpPrivacy.Instance.SignText(strMessage, lSignatureKeyID, strPassphrase);
				Clipboard.SetDataObject(strReturn);
			} catch (Exception ex) {
				MessageBox.Show("An error occured while encrypting the message: " + ex.Message + "\n\n" + ex.StackTrace, "Error...", MessageBoxButtons.OK, MessageBoxIcon.Hand, MessageBoxDefaultButton.Button1);
			}
		
		}
		
		void mnuFileSystemEncrypt_Click(Object sender, System.EventArgs e) {
			System.Windows.Forms.OpenFileDialog ofdOpen = new System.Windows.Forms.OpenFileDialog();
			
			ofdOpen.Multiselect = true;
			ofdOpen.Filter = "All Files (*.*)|*.*";
			ofdOpen.ShowDialog();
			
			if (ofdOpen.FileNames.Length == 0)
				return;
			
			PublicKeySelector pksSelector = new PublicKeySelector(SharpPrivacy.PublicKeyRing);
			pksSelector.ShowDialog();
			
			ulong[] lTargetKeyIDs = (ulong[])pksSelector.SelectedKeys.ToArray(Type.GetType("System.UInt64"));
			
			for (int i=0; i<ofdOpen.FileNames.Length; i++) {
				string strPath = ofdOpen.FileNames[i];
				SharpPrivacy.ReloadKeyRing();
				SharpPrivacy.Instance.EncryptFile(strPath, strPath + ".asc", lTargetKeyIDs);
			}
			
		}
		
		void mnuFileSystemEncryptSign_Click(Object sender, System.EventArgs e) {
			// Check if we have some secret keys at all.
			XmlNodeList xnlSecretKeys = SharpPrivacy.SecretKeyRing.GetElementsByTagName("SecretKey");
			
			if (xnlSecretKeys.Count == 0) {
				MessageBox.Show("You cannot sign data because you do not own a private key. Please generate a new key pair, before you sign data.", "Action not possible...", MessageBoxButtons.OK, MessageBoxIcon.Asterisk, MessageBoxDefaultButton.Button1);
				return;
			}
			
			System.Windows.Forms.OpenFileDialog ofdOpen = new System.Windows.Forms.OpenFileDialog();
			
			ofdOpen.Multiselect = true;
			ofdOpen.Filter = "All Files (*.*)|*.*";
			ofdOpen.ShowDialog();
			
			if (ofdOpen.FileNames.Length == 0)
				return;
			
			PublicKeySelector pksSelector = new PublicKeySelector(SharpPrivacy.PublicKeyRing);
			pksSelector.ShowDialog();
			
			QueryPassphrase qpPassphrase = new QueryPassphrase();
			qpPassphrase.ShowMultiKeyDialog(SharpPrivacy.SecretKeyRing);
			ulong lSignatureKeyID = qpPassphrase.SelectedKey;
			string strPassphrase = qpPassphrase.Passphrase;
			
			ulong[] lTargetKeyIDs = (ulong[])pksSelector.SelectedKeys.ToArray(Type.GetType("System.UInt64"));
			
			for (int i=0; i<ofdOpen.FileNames.Length; i++) {
				string strPath = ofdOpen.FileNames[i];
				SharpPrivacy.ReloadKeyRing();
				SharpPrivacy.Instance.EncryptAndSignFile(strPath, strPath + ".asc", lTargetKeyIDs, lSignatureKeyID, strPassphrase);
			}
			
		}
		
		void mnuAbout_Click(Object sender, System.EventArgs e) {
			AboutDialog adAbout = new AboutDialog();
			adAbout.Show();
		}
		
		void niTrayIcon_MouseUp(Object sender, System.Windows.Forms.MouseEventArgs e) {
			pmnuTrayMenu.DestroyHandle();
			pmnuTrayMenu.Dismiss();
			if (e.Button == MouseButtons.Right) {
				MenuCommand mcSelected = pmnuTrayMenu.TrackPopup(Cursor.Position);
			}
		}

			
		[STAThread]
		public static void Main(string[] args) {
			Application.Run(new SharpPrivacyTray());
		}
	}			
}
