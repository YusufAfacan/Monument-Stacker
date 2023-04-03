using System;
using System.IO;
using UnityEngine;

namespace Voodoo.Render
{
	[Serializable]
	internal class SphereMaterialSaver : SpherePropertyList
	{
		private static string filePath = string.Empty;
		private static string FilePath
		{
			get
			{
				if (string.IsNullOrEmpty(filePath))
				{
					filePath = Path.Combine(Path.GetDirectoryName(Application.dataPath), "Voodoo", "AutoMatcap", "Data", "SphereMaterials.json");
				}

				return filePath;
			}
		}
		
		public void Save()
		{
			FileInfo fileInfo = new FileInfo(FilePath);
			fileInfo.Directory?.Create();
			using (StreamWriter sw = fileInfo.CreateText())
			{
				sw.Write(JsonUtility.ToJson(this, true));
			}
		}
		
		public static SphereMaterialSaver Load()
		{
			FileInfo fileInfo = new FileInfo(FilePath);
			fileInfo.Directory?.Create();
			if (fileInfo.Exists == false)
			{
				return new SphereMaterialSaver();
			}
			
			using (StreamReader sr = fileInfo.OpenText())
			{
				return JsonUtility.FromJson<SphereMaterialSaver>(sr.ReadToEnd());
			}
		}
	}
}