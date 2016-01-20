﻿/*
 * THIS PROGRAM AND ITS SOURCE CODE ARE PROVIDED TO YOU FREE OF CHARGE.
 * YOU ARE PROHIBITED FROM REMOVING THIS NOTICE FROM THIS SOURCE DOCUMENT
 * OR OTHERWISE MODIFYING IT.
 * 
 * FireCryptEx is a free (and by now, open-source) program that allows
 * the user to create encrypted volumes to store files.
 * 
 * FireCryptEx was and continues to be developed by 0xFireball, and is
 * the successor to another free program, FireCrypt.
 * 
 * About the Author:
 * Pseudonym: 0xFireball, ExaPhaser
 * Websites:
 * http://acetylene.x10.bz
 * http://omnibean.x10.bz
 * http://exaphaser.x10.bz
 * 
 * Contact:
 * 0xFireball on GitHub
 * 0xFireball on GitLab
 * exaphaser on GitLab
 * 
 * omnibean@outlook.com
 * 
 *   
 * You are hereby allowed to make changes to the below source code and
 * improve this program. In doing so, you are bound by the terms and 
 * conditions of the AGPLv3 License.
 * 
 */


/*
 */
using System;
using System.IO;
using System.Web.Script.Serialization;
using System.Collections.Generic;
using System.IO.Compression;
using System.Text;
using System.Globalization;

using OmniBean.PowerCrypt4;

namespace FireCrypt
{
	static class ByteConverter
	{
		public static byte[] GetBytes(this string str)
		{
			Encoding iso = Encoding.GetEncoding("ISO-8859-1");
			return iso.GetBytes(str);
		}
		
		public static string GetString(this byte[] bytes)
		{
			Encoding iso = Encoding.GetEncoding("ISO-8859-1");
			return iso.GetString(bytes);
		}
		
		public static byte[] RawGetBytes(this string str)
		{
		    byte[] bytes = new byte[str.Length * sizeof(char)];
		    System.Buffer.BlockCopy(str.ToCharArray(), 0, bytes, 0, bytes.Length);
		    return bytes;
		}
		
		public static string RawGetString(this byte[] bytes)
		{
		    char[] chars = new char[bytes.Length / sizeof(char)];
		    System.Buffer.BlockCopy(bytes, 0, chars, 0, bytes.Length);
		    return new string(chars);
		}
	}
	/// <summary>
	/// FireCryptVolume.
	/// </summary>
	public class FireCryptVolume
	{
		public string RawLocation;
		public string VaultLocation;
		public string VolumeLocation;
		public string OpenVaultLocation;
		public string UID;
		public string Label;
		public string VolumeVersion;
		
		Dictionary<string,string> MetadataValues = new Dictionary<string, string>();
		bool _unlocked;
		string _metadata;
		string _unlockPath;
		
		
		private static string UnlockLocation = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)+"\\FireCrypt\\";
		
		public bool Unlocked
		{
			get
			{
				return _unlocked;
			}
		}
		
		public string UnlockPath
		{
			get
			{
				return _unlockPath;
			}
		}
		
		public static void CreateNewVolume(string location, string label, string password, string version)
		{
			string fnwoext = Path.GetFileNameWithoutExtension(location); //filenamewithout extension
			string volN = Path.GetDirectoryName(location)+"\\"+fnwoext+".vault\\"+fnwoext+".FireCrypt";
			string vaultL = Path.GetDirectoryName(volN);
			Directory.CreateDirectory(Path.GetDirectoryName(volN));
			string vid = Guid.NewGuid().ToString();
			Dictionary<string,string> volMeta = new Dictionary<string,string>();
			volMeta["UID"] = vid;
			volMeta["Label"] = label;
			volMeta["VolumeVersion"]=version;
			string ed = Path.GetTempPath()+"\\"+Guid.NewGuid();
			Directory.CreateDirectory(ed);
			switch (version)
			{
				case "1.0":
					string unlLoc = ed;
					string DecVolumeLocation = unlLoc+".dec";
					ZipFile.CreateFromDirectory(unlLoc, DecVolumeLocation);
					string dVolume = File.ReadAllBytes(DecVolumeLocation).GetString();
					File.WriteAllBytes(volN, PowerAES.Encrypt(dVolume, password).GetBytes());
					break;
				default:
					throw new InvalidOperationException("Cannot perform operation on unsupported volume version!");
			}
			string metaS = new JavaScriptSerializer().Serialize(volMeta);
			File.WriteAllText(vaultL+"\\vault.metadata",metaS);
		}
		
		
		public void UnlockVolume(string key)
		{
			switch (VolumeVersion)
			{
				case "1.0":
					Unlock_1_0(key);
					break;
				default:
					throw new InvalidOperationException("Cannot perform operation on unsupported volume version!");
			}		
		}
		
		public void Unlock_1_0(string key)
		{
			string eVolume = File.ReadAllBytes(VolumeLocation).GetString();
			string unlockName = UnlockLocation+UID;
			string DecVolumeLocation = unlockName+".dec";
			if (!Directory.Exists(UnlockLocation))
			{
				Directory.CreateDirectory(UnlockLocation);
			}
			if (Directory.Exists(unlockName))
			{
				Directory.Delete(unlockName,true);
			}
			File.WriteAllBytes(DecVolumeLocation, PowerAES.Decrypt(eVolume,key).GetBytes());
			ZipFile.ExtractToDirectory(DecVolumeLocation, unlockName);
			File.Delete(DecVolumeLocation);
			_unlocked = true;
			_unlockPath = unlockName;
		}
		
		public void LockVolume(string key)
		{
			switch (VolumeVersion)
			{
				case "1.0":
					Lock_1_0(key);
					break;
				default:
					throw new InvalidOperationException("Cannot perform operation on unsupported volume version!");
			}
		}
		
		public static void DeleteDirectory(string target_dir)
		{
		    string[] files = Directory.GetFiles(target_dir);
		    string[] dirs = Directory.GetDirectories(target_dir);
		
		    foreach (string file in files)
		    {
		        File.SetAttributes(file, FileAttributes.Normal);
		        File.Delete(file);
		    }
		
		    foreach (string dir in dirs)
		    {
		        DeleteDirectory(dir);
		    }
		
		    Directory.Delete(target_dir, false);
		}
		
		private void Lock_1_0(string key)
		{
			string unlockName = UnlockLocation+UID;
			string DecVolumeLocation = unlockName+".dec";
			if (File.Exists(DecVolumeLocation))
			{
				File.Delete(DecVolumeLocation);
			}
			ZipFile.CreateFromDirectory(unlockName, DecVolumeLocation);
			Directory.Delete(unlockName, true);
			string dVolume = File.ReadAllBytes(DecVolumeLocation).GetString();
			File.Delete(DecVolumeLocation);
			File.WriteAllBytes(VolumeLocation, PowerAES.Encrypt(dVolume, key).GetBytes());
            _unlocked = false;
			_unlockPath = null;
            DeleteDirectory(unlockName);
        }
		
		private void Lock_1_BC(string key)
		{
			
		}
		
		private dynamic TryLoadProperty(string key, dynamic defaultValue)
		{
			try
			{
				string mV = MetadataValues[key];
				return mV;
			}
			catch
			{
				return defaultValue;
			}
		}
		
		private void LoadBackwardsCompatiableProperties()
		{
			//To load properties that were not yet implemented in the free version, to not break compatibility
			VolumeVersion = TryLoadProperty("VolumeVersion","1.0");
		}
		
		public FireCryptVolume()
		{
			DoInit();
		}
		
		
		public FireCryptVolume(string location)
		{
			RawLocation = location;
			DoInit();
		}
		
		static bool IsMicrosoftCLR()
		{
			return (Type.GetType ("Mono.Runtime") == null);
		}
		
		
		
		private void DoInit()
		{
			//Initialize Required Objects
			
			//Initialize Volume
			string fnwoext = Path.GetFileNameWithoutExtension(RawLocation); //filenamewithout extension
			string volN = RawLocation;
			if (Path.GetExtension(volN)!=".FireCrypt")
				volN = Path.GetDirectoryName(RawLocation)+"\\"+fnwoext+".vault\\"+fnwoext+".FireCrypt";
			VolumeLocation = volN;
			VaultLocation = Path.GetDirectoryName(volN);
			_unlocked = Directory.Exists(OpenVaultLocation);
			_metadata = File.ReadAllText(VaultLocation+"\\vault.metadata");
			var jss = new JavaScriptSerializer();
			MetadataValues = jss.Deserialize<Dictionary<string,string>>(_metadata);
			UID = MetadataValues["UID"];
			Label = MetadataValues["Label"];
			LoadBackwardsCompatiableProperties();
		}
		
	}
}
