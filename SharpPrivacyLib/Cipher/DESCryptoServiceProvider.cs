﻿//
// System.Security.Cryptography.DESCryptoServiceProvider
//
// Authors:
//	Sergey Chaban (serge@wildwestsoftware.com)
//	Sebastien Pouliot (spouliot@motus.com)
//
// Portions (C) 2002 Motus Technologies Inc. (http://www.motus.com)
//
// Modified by Daniel Fabian to fit SharpPrivacy's needs.
// This file is part of the SharpPrivacy source code contribution.
// Get get the original SymmetricAlgorithm class, please visit the
// mono project at http://www.go-mono.com.
//

using System;

namespace SharpPrivacy.SharpPrivacyLib.Cipher {

	// References:
	// a.	FIPS PUB 46-3: Data Encryption Standard
	//	http://csrc.nist.gov/publications/fips/fips46-3/fips46-3.pdf
	
	internal class DESTransform : SymmetricTransform {
	
		internal static readonly int KEY_BIT_SIZE = 64;
		internal static readonly int KEY_BYTE_SIZE = KEY_BIT_SIZE / 8;
		internal static readonly int BLOCK_BIT_SIZE = 64;
		internal static readonly int BLOCK_BYTE_SIZE = BLOCK_BIT_SIZE / 8;
	
		private byte [] keySchedule;
		private byte [] byteBuff;
		private uint [] dwordBuff;
	
		// S-boxes from FIPS 46-3, Appendix 1, page 17
		private static byte [] sBoxes = {
						/* S1 */
			14,  4, 13,  1,  2, 15, 11,  8,  3, 10,  6, 12,  5,  9,  0,  7,
			0, 15,  7,  4, 14,  2, 13,  1, 10,  6, 12, 11,  9,  5,  3,  8,
			4,  1, 14,  8, 13,  6,  2, 11, 15, 12,  9,  7,  3, 10,  5,  0,
			15, 12,  8,  2,  4,  9,  1,  7,  5, 11,  3, 14, 10,  0,  6, 13,
	
						/* S2 */
			15,  1,  8, 14,  6, 11,  3,  4,  9,  7,  2, 13, 12,  0,  5, 10,
			3, 13,  4,  7, 15,  2,  8, 14, 12,  0,  1, 10,  6,  9, 11,  5,
			0, 14,  7, 11, 10,  4, 13,  1,  5,  8, 12,  6,  9,  3,  2, 15,
			13,  8, 10,  1,  3, 15,  4,  2, 11,  6,  7, 12,  0,  5, 14,  9,
	
						/* S3 */
			10,  0,  9, 14,  6,  3, 15,  5,  1, 13, 12,  7, 11,  4,  2,  8,
			13,  7,  0,  9,  3,  4,  6, 10,  2,  8,  5, 14, 12, 11, 15,  1,
			13,  6,  4,  9,  8, 15,  3,  0, 11,  1,  2, 12,  5, 10, 14,  7,
			1, 10, 13,  0,  6,  9,  8,  7,  4, 15, 14,  3, 11,  5,  2, 12,
	
						/* S4 */
			7, 13, 14,  3,  0,  6,  9, 10,  1,  2,  8,  5, 11, 12,  4, 15,
			13,  8, 11,  5,  6, 15,  0,  3,  4,  7,  2, 12,  1, 10, 14,  9,
			10,  6,  9,  0, 12, 11,  7, 13, 15,  1,  3, 14,  5,  2,  8,  4,
			3, 15,  0,  6, 10,  1, 13,  8,  9,  4,  5, 11, 12,  7,  2, 14,
	
						/* S5 */
			2, 12,  4,  1,  7, 10, 11,  6,  8,  5,  3, 15, 13,  0, 14,  9,
			14, 11,  2, 12,  4,  7, 13,  1,  5,  0, 15, 10,  3,  9,  8,  6,
			4,  2,  1, 11, 10, 13,  7,  8, 15,  9, 12,  5,  6,  3,  0, 14,
			11,  8, 12,  7,  1, 14,  2, 13,  6, 15,  0,  9, 10,  4,  5,  3,
	
						/* S6 */
			12,  1, 10, 15,  9,  2,  6,  8,  0, 13,  3,  4, 14,  7,  5, 11,
			10, 15,  4,  2,  7, 12,  9,  5,  6,  1, 13, 14,  0, 11,  3,  8,
			9, 14, 15,  5,  2,  8, 12,  3,  7,  0,  4, 10,  1, 13, 11,  6,
			4,  3,  2, 12,  9,  5, 15, 10, 11, 14,  1,  7,  6,  0,  8, 13,
	
						/* S7 */
			4, 11,  2, 14, 15,  0,  8, 13,  3, 12,  9,  7,  5, 10,  6,  1,
			13,  0, 11,  7,  4,  9,  1, 10, 14,  3,  5, 12,  2, 15,  8,  6,
			1,  4, 11, 13, 12,  3,  7, 14, 10, 15,  6,  8,  0,  5,  9,  2,
			6, 11, 13,  8,  1,  4, 10,  7,  9,  5,  0, 15, 14,  2,  3, 12,
	
						/* S8 */
			13,  2,  8,  4,  6, 15, 11,  1, 10,  9,  3, 14,  5,  0, 12,  7,
			1, 15, 13,  8, 10,  3,  7,  4, 12,  5,  6, 11,  0, 14,  9,  2,
			7, 11,  4,  1,  9, 12, 14,  2,  0,  6, 10, 13, 15,  3,  5,  8,
			2,  1, 14,  7,  4, 10,  8, 13, 15, 12,  9,  0,  3,  5,  6, 11
		};
	
	
		// P table from page 15, also in Appendix 1, page 18
		private static byte [] pTab = {	
			16-1,  7-1, 20-1, 21-1,
			29-1, 12-1, 28-1, 17-1,
			1-1, 15-1, 23-1, 26-1,
			5-1, 18-1, 31-1, 10-1,
			2-1,  8-1, 24-1, 14-1,
			32-1, 27-1,  3-1,  9-1,
			19-1, 13-1, 30-1,  6-1,
			22-1, 11-1,  4-1, 25-1
		};
	
	
		// Permuted choice 1 table, PC-1, page 19
		// Translated to zero-based format.
		private static byte [] PC1 = {
			57-1, 49-1, 41-1, 33-1, 25-1, 17-1,  9-1,
			1-1, 58-1, 50-1, 42-1, 34-1, 26-1, 18-1,
			10-1,  2-1, 59-1, 51-1, 43-1, 35-1, 27-1,
			19-1, 11-1,  3-1, 60-1, 52-1, 44-1, 36-1,
	
			63-1, 55-1, 47-1, 39-1, 31-1, 23-1, 15-1,
			7-1, 62-1, 54-1, 46-1, 38-1, 30-1, 22-1,
			14-1,  6-1, 61-1, 53-1, 45-1, 37-1, 29-1,
			21-1, 13-1,  5-1, 28-1, 20-1, 12-1,  4-1
		};
	
	
		private static byte [] leftRot = {
			1, 1, 2, 2, 2, 2, 2, 2, 1, 2, 2, 2, 2, 2, 2, 1
		};
	
		private static byte [] leftRotTotal;
	
		// Permuted choice 2 table, PC-2, page 21
		// Translated to zero-based format.
		private static byte [] PC2 = {
			14-1, 17-1, 11-1, 24-1,  1-1,  5-1,
			3-1, 28-1, 15-1,  6-1, 21-1, 10-1,
			23-1, 19-1, 12-1,  4-1, 26-1,  8-1,
			16-1,  7-1, 27-1, 20-1, 13-1,  2-1,
			41-1, 52-1, 31-1, 37-1, 47-1, 55-1,
			30-1, 40-1, 51-1, 45-1, 33-1, 48-1,
			44-1, 49-1, 39-1, 56-1, 34-1, 53-1,
			46-1, 42-1, 50-1, 36-1, 29-1, 32-1
		};
	
	
		// Initial permutation IP, page 10.
		// Transposed to 0-based format.
		private static byte [] ipBits = {
			58-1, 50-1, 42-1, 34-1, 26-1, 18-1, 10-1,  2-1,
			60-1, 52-1, 44-1, 36-1, 28-1, 20-1, 12-1,  4-1,
			62-1, 54-1, 46-1, 38-1, 30-1, 22-1, 14-1,  6-1,
			64-1, 56-1, 48-1, 40-1, 32-1, 24-1, 16-1,  8-1,
			57-1, 49-1, 41-1, 33-1, 25-1, 17-1,  9-1,  1-1,
			59-1, 51-1, 43-1, 35-1, 27-1, 19-1, 11-1,  3-1,
			61-1, 53-1, 45-1, 37-1, 29-1, 21-1, 13-1,  5-1,
			63-1, 55-1, 47-1, 39-1, 31-1, 23-1, 15-1,  7-1
		};
	
	
		// Final permutation FP = IP^(-1), page 10.
		// Transposed to 0-based format.
		private static byte [] fpBits = {
			40-1,  8-1, 48-1, 16-1, 56-1, 24-1, 64-1, 32-1,
			39-1,  7-1, 47-1, 15-1, 55-1, 23-1, 63-1, 31-1,
			38-1,  6-1, 46-1, 14-1, 54-1, 22-1, 62-1, 30-1,
			37-1,  5-1, 45-1, 13-1, 53-1, 21-1, 61-1, 29-1,
			36-1,  4-1, 44-1, 12-1, 52-1, 20-1, 60-1, 28-1,
			35-1,  3-1, 43-1, 11-1, 51-1, 19-1, 59-1, 27-1,
			34-1,  2-1, 42-1, 10-1, 50-1, 18-1, 58-1, 26-1,
			33-1,  1-1, 41-1,  9-1, 49-1, 17-1, 57-1, 25-1
		};
	
		private static uint [] spBoxes;
		private static int [] ipTab;
		private static int [] fpTab;
	
		static DESTransform() {
			spBoxes = new uint [64 * 8];
	
			int [] pBox = new int [32];
	
			for (int p = 0; p < 32; p++) {
				for (int i = 0; i < 32; i++) {
					if (p == pTab [i]) {
						pBox [p] = i;
						break;
					}
				}
			}
	
			for (int s = 0; s < 8; s++) { // for each S-box
				int sOff = s << 6;
	
				for (int i = 0; i < 64; i++) { // inputs
					uint sp=0;
	
					int indx = (i & 0x20) | ((i & 1) << 4) | ((i >> 1) & 0xF);
	
					for (int j = 0; j < 4; j++) { // for each bit in the output
						if ((sBoxes [sOff + indx] & (8 >> j)) != 0) {
							sp |= (uint) (1 << (31 - pBox [(s << 2) + j]));
						}
					}
	
					spBoxes [sOff + i] = sp;
				}
			}
	
			leftRotTotal = new byte [leftRot.Length];
	
			for (int i = 0; i < leftRot.Length; i++) {
				int r = 0;
				for (int j = 0; j <= i; r += leftRot [j++]);
				leftRotTotal [i]  = (byte) r;
			}
	
			InitPermutationTable (ipBits, out ipTab);
			InitPermutationTable (fpBits, out fpTab);
		} // class constructor
	
		// Default constructor.
		internal DESTransform(SymmetricAlgorithm symmAlgo, bool encryption, byte[] key, byte[] iv) : base (symmAlgo, encryption, iv) {
			keySchedule = new byte [KEY_BYTE_SIZE * 16];
			byteBuff = new byte [BLOCK_BYTE_SIZE];
			dwordBuff = new uint [BLOCK_BYTE_SIZE / 4];
			SetKey (key);
		}
	
		private static void InitPermutationTable (byte [] pBits, out int [] permTab) {
			permTab = new int [8*2 * 8*2 * (64/32)];
	
			for (int i = 0; i < 16; i++) {
				for (int j = 0; j < 16; j++) {
					int offs = (i << 5) + (j << 1);
					for (int n = 0; n < 64; n++) {
						int bitNum = (int) pBits [n];
						if ((bitNum >> 2 == i) &&
							0 != (j & (8 >> (bitNum & 3)))) {
							permTab [offs + (n >> (3+2))] |= (int) ((0x80808080 & (0xFF << (n & (3 << 3)))) >> (n & 7));
						}
					}
				}
			}
		}
	
		private uint CipherFunct(uint r, int n) {
			uint res = 0;
			byte [] subkey = keySchedule;
			int i = n << 3;
	
			uint rt = (r >> 1) | (r << 31); // ROR32(r)
			res |= spBoxes [0*64 + (((rt >> 26) ^ subkey [i++]) & 0x3F)];
			res |= spBoxes [1*64 + (((rt >> 22) ^ subkey [i++]) & 0x3F)];
			res |= spBoxes [2*64 + (((rt >> 18) ^ subkey [i++]) & 0x3F)];
			res |= spBoxes [3*64 + (((rt >> 14) ^ subkey [i++]) & 0x3F)];
			res |= spBoxes [4*64 + (((rt >> 10) ^ subkey [i++]) & 0x3F)];
			res |= spBoxes [5*64 + (((rt >>  6) ^ subkey [i++]) & 0x3F)];
			res |= spBoxes [6*64 + (((rt >>  2) ^ subkey [i++]) & 0x3F)];
			rt = (r << 1) | (r >> 31); // ROL32(r)
			res |= spBoxes [7*64 + ((rt ^ subkey [i]) & 0x3F)];
	
			return res;
		}
	
	
		private static void Permutation(byte [] input, byte [] _output, int [] permTab, bool preSwap) {
			if (preSwap)
				BSwap(input);
	
			byte [] output = _output;
	
			int offs1 = (((int)(input [0]) >> 4)) << 1;
			int offs2 = (1 << 5) + ((((int)input [0]) & 0xF) << 1);
	
			int d1 = permTab [offs1++] | permTab [offs2++];
			int d2 = permTab [offs1]   | permTab [offs2];
	
	
			int max = BLOCK_BYTE_SIZE << 1;
			for (int i = 2, indx = 1; i < max; i += 2, indx++) {
				int ii = (int) input [indx];
				offs1 = (i << 5) + ((ii >> 4) << 1);
				offs2 = ((i + 1) << 5) + ((ii & 0xF) << 1);
	
				d1 |= permTab [offs1++] | permTab [offs2++];
				d2 |= permTab [offs1]   | permTab [offs2];
			}
	
			if (preSwap) {
				output [0] = (byte) (d1);
				output [1] = (byte) (d1 >> 8);
				output [2] = (byte) (d1 >> 16);
				output [3] = (byte) (d1 >> 24);
				output [4] = (byte) (d2);
				output [5] = (byte) (d2 >> 8);
				output [6] = (byte) (d2 >> 16);
				output [7] = (byte) (d2 >> 24);
			} else {
				output [0] = (byte) (d1 >> 24);
				output [1] = (byte) (d1 >> 16);
				output [2] = (byte) (d1 >> 8);
				output [3] = (byte) (d1);
				output [4] = (byte) (d2 >> 24);
				output [5] = (byte) (d2 >> 16);
				output [6] = (byte) (d2 >> 8);
				output [7] = (byte) (d2);
			}
		}
	
		private static void BSwap(byte [] byteBuff) {
			byte t;
	
			t = byteBuff [0];
			byteBuff [0] = byteBuff [3];
			byteBuff [3] = t;
	
			t = byteBuff [1];
			byteBuff [1] = byteBuff [2];
			byteBuff [2] = t;
	
			t = byteBuff [4];
			byteBuff [4] = byteBuff [7];
			byteBuff [7] = t;
	
			t = byteBuff [5];
			byteBuff [5] = byteBuff [6];
			byteBuff [6] = t;
		}
	
		internal void SetKey(byte[] key) {
			// NOTE: see Fig. 3, Key schedule calculation, at page 20.
			Array.Clear (keySchedule, 0, keySchedule.Length);
	
			int keyBitSize = PC1.Length;
	
			byte[] keyPC1 = new byte [keyBitSize]; // PC1-permuted key
			byte[] keyRot = new byte [keyBitSize]; // PC1 & rotated
	
			int indx = 0;
	
			foreach (byte bitPos in PC1) {
				keyPC1 [indx++] = (byte)((key [(int)bitPos >> 3] >> (7 ^ (bitPos & 7))) & 1);
			}
	
			int j;
			for (int i = 0; i < KEY_BYTE_SIZE*2; i++) {
				int b = keyBitSize >> 1;
	
				for (j = 0; j < b; j++) {
					int s = j + (int) leftRotTotal [i];
					keyRot [j] = keyPC1 [s < b ? s : s - b];
				}
	
				for (j = b; j < keyBitSize; j++) {
					int s = j + (int) leftRotTotal [i];
					keyRot[j] = keyPC1[s < keyBitSize ? s : s - b];
				}
	
				int keyOffs = i * KEY_BYTE_SIZE;
	
				j = 0;
				foreach (byte bitPos in PC2) {
					if (keyRot[(int)bitPos] != 0) {
						keySchedule[keyOffs + (j/6)] |= (byte)(0x80 >> ((j % 6) + 2));
					}
					j++;
				}
			}
		}
	
		// public helper for TripleDES
		public void ProcessBlock(byte[] input, byte[] output) {
			ECB(input, output);
		}
	
		protected override void ECB(byte[] input, byte[] output) {
			byte [] byteBuff = this.byteBuff;
			uint [] dwordBuff = this.dwordBuff;
	
			Permutation(input, byteBuff, ipTab, false);
			Buffer.BlockCopy(byteBuff, 0, dwordBuff, 0, BLOCK_BYTE_SIZE);
	
			if (encrypt) {
				uint d0 = dwordBuff[0];
				uint d1 = dwordBuff[1];
	
				// 16 rounds
				d0 ^= CipherFunct(d1,  0);
				d1 ^= CipherFunct(d0,  1);
				d0 ^= CipherFunct(d1,  2);
				d1 ^= CipherFunct(d0,  3);
				d0 ^= CipherFunct(d1,  4);
				d1 ^= CipherFunct(d0,  5);
				d0 ^= CipherFunct(d1,  6);
				d1 ^= CipherFunct(d0,  7);
				d0 ^= CipherFunct(d1,  8);
				d1 ^= CipherFunct(d0,  9);
				d0 ^= CipherFunct(d1, 10);
				d1 ^= CipherFunct(d0, 11);
				d0 ^= CipherFunct(d1, 12);
				d1 ^= CipherFunct(d0, 13);
				d0 ^= CipherFunct(d1, 14);
				d1 ^= CipherFunct(d0, 15);
	
				dwordBuff [0] = d1;
				dwordBuff [1] = d0;
			} else {
				uint d1 = dwordBuff[0];
				uint d0 = dwordBuff[1];
	
				// 16 rounds in reverse order
				d1 ^= CipherFunct(d0, 15);
				d0 ^= CipherFunct(d1, 14);
				d1 ^= CipherFunct(d0, 13);
				d0 ^= CipherFunct(d1, 12);
				d1 ^= CipherFunct(d0, 11);
				d0 ^= CipherFunct(d1, 10);
				d1 ^= CipherFunct(d0,  9);
				d0 ^= CipherFunct(d1,  8);
				d1 ^= CipherFunct(d0,  7);
				d0 ^= CipherFunct(d1,  6);
				d1 ^= CipherFunct(d0,  5);
				d0 ^= CipherFunct(d1,  4);
				d1 ^= CipherFunct(d0,  3);
				d0 ^= CipherFunct(d1,  2);
				d1 ^= CipherFunct(d0,  1);
				d0 ^= CipherFunct(d1,  0);
	
				dwordBuff [0] = d0;
				dwordBuff [1] = d1;
			}
	
			Buffer.BlockCopy(dwordBuff, 0, byteBuff, 0, BLOCK_BYTE_SIZE);
			Permutation(byteBuff, output, fpTab, true);
		}
	} 
	
	public sealed class DESCryptoServiceProvider : DES {
	
		public DESCryptoServiceProvider() : base() {}
	
		public override ICryptoTransform CreateDecryptor(byte[] rgbKey, byte[] rgbIV) {
			Key = rgbKey;
			IV = rgbIV;
			return new DESTransform(this, false, rgbKey, rgbIV);
		}
	
		public override ICryptoTransform CreateEncryptor(byte[] rgbKey, byte[] rgbIV) {
			Key = rgbKey;
			IV = rgbIV;
			return new DESTransform(this, true, rgbKey, rgbIV);
		}
	
		public override void GenerateIV() {
			IVValue = KeyBuilder.IV(BlockSizeValue >> 3);
		}
	
		public override void GenerateKey () {
			KeyValue = KeyBuilder.Key(KeySizeValue >> 3);
			while (IsWeakKey(KeyValue) || IsSemiWeakKey(KeyValue))
				KeyValue = KeyBuilder.Key(KeySizeValue >> 3);
		}
	
	} // DESCryptoServiceProvider

} // System.Security.Cryptography
