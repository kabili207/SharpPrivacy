//
// This file is part of the source code distribution of SharpPrivacy.
// SharpPrivacy is an Open Source OpenPGP implementation and can be 
// found at http://www.sharpprivacy.net
// It is released under Gnu General Public License and can be used 
// and modified as long as the result is released under GPL too. 
// For a copy of the GPL, please go to www.gnu.org/copyleft/gpl.html 
//
// SecretKeyRing.cs: 
// 	Class for handling secret key rings.
//
// Author:
//	Daniel Fabian (df@sharpprivacy.net)
//
//
// Version: 0.1.0 (initial release)
//
// Changelog:
//	- 23.02.2003: Created this file.
//	- 01.06.2003: Added this header for the first beta release.
//
// (C) 2003, Daniel Fabian
//
using System;
using System.Windows.Forms;
using SharpPrivacy.SharpPrivacyLib.OpenPGP;
using SharpPrivacy.SharpPrivacyLib.OpenPGP.Messages;
using System.Collections;
using System.IO;

namespace SharpPrivacy.SharpPrivacyLib {
	
	public class SecretKeyRing {
		
		private ArrayList alSecretKeys;
		private bool bIsUpdated = false;
		private string strLoadingPath;
		
		public bool IsUpdated {
			get {
				return bIsUpdated;
			}
		}
		
		public ArrayList SecretKeys {
			get {
				return alSecretKeys;
			}
			set {
				alSecretKeys = value;
			}
		}
		
		public SecretKeyRing() {
			alSecretKeys = new ArrayList();
		}
		
		public void Reload() {
			if (this.strLoadingPath.Length == 0)
				return;
			
			Load(strLoadingPath);
		}
		
		public void Load(string strPath) {
			strLoadingPath = strPath;
			System.IO.StreamReader srInput = new StreamReader(strPath);
			string strKeys = srInput.ReadToEnd();
			srInput.Close();
			
			this.SecretKeys = new ArrayList();
			
			ArmorTypes atType = new ArmorTypes();
			string strKey = Armor.RemoveArmor(strKeys, ref atType, ref strKeys);
			while (strKey.Length > 0) {
				TransportableSecretKey tskKey = new TransportableSecretKey(strKey);
				this.SecretKeys.Add(tskKey);
				
				strKey = Armor.RemoveArmor(strKeys, ref atType, ref strKeys);
			}
			bIsUpdated = false;
		}
		
		public void Save() {
			Save(this.strLoadingPath);
		}
		
		public void Save(string strPath) {
			System.IO.StreamWriter swOutput = new StreamWriter(strPath);
			IEnumerator ieKeys = this.SecretKeys.GetEnumerator();
			while (ieKeys.MoveNext()) {
				if (ieKeys.Current is TransportableSecretKey) {
					try {
						TransportableSecretKey tskKey = (TransportableSecretKey)ieKeys.Current;
						byte[] bKey = tskKey.Generate();
						string strKey = Armor.WrapPrivateKey(bKey);
						swOutput.Write(strKey);
					} catch (Exception e) {
						MessageBox.Show("Error while trying to save a private key: " + e.Message, "Error...", MessageBoxButtons.OK, MessageBoxIcon.Warning);
					}
				}
			}
			swOutput.Close();
			bIsUpdated = false;
		}
		
		public void Add(TransportableSecretKey tskKey) {
			bIsUpdated = true;
			SecretKeys.Add(tskKey);
		}
		
		public void Delete(TransportableSecretKey tskKey) {
			bIsUpdated = true;
			SecretKeys.Remove(tskKey);
		}
		
		public void Delete(ulong lKeyID) {
			bIsUpdated = true;
			SecretKeys.Remove(Find(lKeyID));
		}
		
		public TransportableSecretKey Find(ulong lKeyID) {
			IEnumerator ieKeys = SecretKeys.GetEnumerator();
			while (ieKeys.MoveNext()) {
				TransportableSecretKey tskKey = (TransportableSecretKey)ieKeys.Current;
				if (tskKey.PrimaryKey.PublicKey.KeyID == lKeyID) {
					return tskKey;
				}
				
				IEnumerator ieSubkeys = tskKey.SubKeys.GetEnumerator();
				while (ieSubkeys.MoveNext()) {
					if (!(ieSubkeys.Current is SecretKeyPacket))
						throw new Exception("Expected a secret key packet, but did not find one.");
					
					SecretKeyPacket skpKey = (SecretKeyPacket)ieSubkeys.Current;
					if (skpKey.PublicKey.KeyID == lKeyID) {
						return tskKey;
					}
				}
			}
			return null;
		}
		
		public TransportableSecretKey Find(ulong lKeyID, string strPassphrase) {
			IEnumerator ieKeys = SecretKeys.GetEnumerator();
			while (ieKeys.MoveNext()) {
				TransportableSecretKey tskKey = (TransportableSecretKey)ieKeys.Current;
				if (tskKey.PrimaryKey.PublicKey.KeyID == lKeyID) {
					return tskKey;
				}
				
				IEnumerator ieSubkeys = tskKey.SubKeys.GetEnumerator();
				while (ieSubkeys.MoveNext()) {
					if (!(ieSubkeys.Current is SecretKeyPacket))
						throw new Exception("Expected a secret key packet, but did not find one.");
					
					SecretKeyPacket skpKey = (SecretKeyPacket)ieSubkeys.Current;
					if (skpKey.PublicKey.KeyID == lKeyID) {
						try {
							skpKey.GetDecryptedKeyMaterial(strPassphrase);
						} catch (Exception) {
							return null;
						}
						return tskKey;
					}
				}
			}
			return null;
		}
	}
	
}
