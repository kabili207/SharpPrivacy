//
// This file is part of the source code distribution of SharpPrivacy.
// SharpPrivacy is an Open Source OpenPGP implementation and can be 
// found at http://www.sharpprivacy.net
// It is released under Gnu General Public License and can be used 
// and modified as long as the result is released under GPL too. 
// For a copy of the GPL, please go to www.gnu.org/copyleft/gpl.html 
//
// PublicKeyRing.cs: 
// 	Class for handling public key rings.
//
// Author:
//	Daniel Fabian (df@sharpprivacy.net)
//
//
// Version: 0.1.0 (initial release)
//
// Changelog:
//	- 10.03.2003: Created this file.
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
	
	public class PublicKeyRing {
		
		private ArrayList alPublicKeys;
		private bool bIsUpdated = false;
		private string strLoadingPath;
		
		public bool IsUpdated {
			get {
				return bIsUpdated;
			}
		}
		public ArrayList PublicKeys {
			get {
				return alPublicKeys;
			}
			set {
				alPublicKeys = value;
			}
		}
		
		public PublicKeyRing() {
			alPublicKeys = new ArrayList();
		}
		
		public void Load(string strPath) {
			this.strLoadingPath = strPath;
			System.IO.StreamReader srInput = new StreamReader(strPath);
			string strKeys = srInput.ReadToEnd();
			srInput.Close();
			
			this.PublicKeys = new ArrayList();
			
			ArmorTypes atType = new ArmorTypes();
			string strKey = Armor.RemoveArmor(strKeys, ref atType, ref strKeys);
			while (strKey.Length > 0) {
				TransportablePublicKey tpkKey = new TransportablePublicKey(strKey);
				this.PublicKeys.Add(tpkKey);
				
				strKey = Armor.RemoveArmor(strKeys, ref atType, ref strKeys);
			}
			this.bIsUpdated = false;
		}
		
		public void Save() {
			Save(this.strLoadingPath);
		}
		
		public void Reload() {
			if (this.strLoadingPath.Length == 0)
				return;
			
			Load(strLoadingPath);
		}
		
		public void Save(string strPath) {
			System.IO.StreamWriter swOutput = new StreamWriter(strPath);
			IEnumerator ieKeys = this.PublicKeys.GetEnumerator();
			while (ieKeys.MoveNext()) {
				if (ieKeys.Current is TransportablePublicKey) {
					try {
						TransportablePublicKey tpkKey = (TransportablePublicKey)ieKeys.Current;
						byte[] bKey = tpkKey.Generate();
						string strKey = Armor.WrapPublicKey(bKey);
						swOutput.Write(strKey);
					} catch (Exception e) {
						MessageBox.Show("Error while trying to save a public key: " + e.Message, "Error...", MessageBoxButtons.OK, MessageBoxIcon.Warning);
					}
				}
			}
			swOutput.Close();
			this.bIsUpdated = false;
		}
		
		public void Add(TransportablePublicKey tpkKey) {
			bIsUpdated = true;
			alPublicKeys.Add(tpkKey);
		}
		
		public void Delete(ulong lKeyID) {
			bIsUpdated = true;
			alPublicKeys.Remove(Find(lKeyID));
		}
		
		public void Delete(TransportablePublicKey tpkKey) {
			bIsUpdated = true;
			alPublicKeys.Remove(tpkKey);
		}
		
		public TransportablePublicKey Find(ulong lKeyID) {
			IEnumerator ieKeys = alPublicKeys.GetEnumerator();
			while (ieKeys.MoveNext()) {
				TransportablePublicKey tpkKey = (TransportablePublicKey)ieKeys.Current;
				if (tpkKey.PrimaryKey.KeyID == lKeyID) {
					return tpkKey;
				}
				IEnumerator ieSubkeys = tpkKey.SubKeys.GetEnumerator();
				while (ieSubkeys.MoveNext()) {
					CertifiedPublicSubkey cpsSubkey = (CertifiedPublicSubkey)ieSubkeys.Current;
					if (cpsSubkey.Subkey.KeyID == lKeyID)
						return tpkKey;
				}
			}
			return null;
		}
		
	}
	
}
