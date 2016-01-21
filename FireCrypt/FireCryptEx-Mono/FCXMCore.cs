
using System;
using System.IO;
using FireCrypt;

namespace FireCryptEx
{
	/// <summary>
	/// Description of FCXMCore.
	/// </summary>
	public class FCXMCore
	{
		string VolumePath;
		string VolumeLabel;
		string Password;
		public FCXMCore()
		{
		}
		public void Run(string path, int action, string volName, string pw)
		{
			Password = pw;
			VolumePath = Path.GetFullPath(path);
			VolumeLabel = volName;
			//2 create, 1 unlock, 0 lock
			switch (action)
			{
				case 0:
					LockVol();
					break;
				case 1:
					UnlockVol();
					break;
				case 2:
					CreateVol();
					break;
			}
		}
		void LockVol()
		{
			FCXMono.Debug("Initiating lock volume...");
			FireCryptVolume FinalVolume;
			bool ValidVolume;
			string VolumeFileLocation = VolumePath;
			string fnwoext = Path.GetFileNameWithoutExtension(VolumeFileLocation); //filenamewithout extension
            string volN = Path.GetDirectoryName(VolumeFileLocation) + Path.DirectorySeparatorChar + fnwoext + ".vault"+Path.DirectorySeparatorChar + fnwoext + ".FireCrypt";
            //dont change it before createvolume because it uses the older standard.
            VolumeFileLocation = volN; //point it to the actual .FireCrypt file.
			FireCryptVolume fcv  = new FireCryptVolume(VolumeFileLocation);
			try
			{
				ValidVolume = true;
				FinalVolume = fcv;
				FCXMono.Debug("Volume locked successfully.");
			}
			catch (Exception e)
			{
				ValidVolume = false;
				Console.WriteLine("Volume locking was unsuccessful: {0}",e);
			}
			fcv.LockVolume(Password);
		}
		void UnlockVol()
		{
			FCXMono.Debug("Initiating open volume...");
			FireCryptVolume FinalVolume;
			bool ValidVolume;
			string VolumeFileLocation = VolumePath;
			string fnwoext = Path.GetFileNameWithoutExtension(VolumeFileLocation); //filenamewithout extension
            string volN = Path.GetDirectoryName(VolumeFileLocation) + Path.DirectorySeparatorChar + fnwoext + ".vault" + Path.DirectorySeparatorChar + fnwoext + ".FireCrypt";
            //dont change it before createvolume because it uses the older standard.
            VolumeFileLocation = volN; //point it to the actual .FireCrypt file.
			FireCryptVolume fcv  = new FireCryptVolume(VolumeFileLocation);
			try
			{
				ValidVolume = true;
				FinalVolume = fcv;
				FCXMono.Debug("Volume opened successfully.");
			}
			catch (Exception e)
			{
				ValidVolume = false;
				Console.WriteLine("Volume opening was unsuccessful: {0}",e);
			}
			fcv.UnlockVolume(Password);
            Console.WriteLine("Vault unlocked to {0}",fcv.UnlockPath);
		}
		void CreateVol()
		{
			FCXMono.Debug("Initiating create volume...");
			FireCryptVolume FinalVolume;
			bool ValidVolume;
			string VolumeFileLocation = VolumePath;
			string fnwoext = Path.GetFileNameWithoutExtension(VolumeFileLocation); //filenamewithout extension
            string volN = Path.GetDirectoryName(VolumeFileLocation) + Path.DirectorySeparatorChar + fnwoext + ".vault" + Path.DirectorySeparatorChar + fnwoext + ".FireCrypt";
            FireCryptVolume.CreateNewVolume(VolumeFileLocation, VolumeLabel, Password, "1.0");
			//dont change it before createvolume because it uses the older standard.
			VolumeFileLocation = volN; //point it to the actual .FireCrypt file.
			try
			{
				FireCryptVolume fcv  = new FireCryptVolume(VolumeFileLocation);
				ValidVolume = true;
				FinalVolume = fcv;
				FCXMono.Debug("Volume created successfully.");
			}
			catch (Exception e)
			{
				ValidVolume = false;
				Console.WriteLine("Volume creation was unsuccessful: {0}",e);
			}
		}
	}
}
