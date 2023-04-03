using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace Voodoo.Render
{
	public static class MatcapFieldSaver
	{
		private static string filePath = string.Empty;
		private static string FilePath
		{
			get
			{
				if (string.IsNullOrEmpty(filePath))
				{
					filePath = Path.Combine(Path.GetDirectoryName(Application.dataPath), "Voodoo", "AutoMatcap", "Data", "MatcapFields.json");
				}

				return filePath;
			}
		}

		public static void SaveMatcapField(MatcapField matcapField)
		{
			if (matcapField.originalMaterial == null || matcapField.matcapMaterial == null)
			{
				return;
			}
			
			FileInfo fileInfo = new FileInfo(FilePath);
			if (File.Exists(FilePath) == false)
			{
				fileInfo.Directory?.Create();

				using (StreamWriter sw = fileInfo.CreateText())
				{
					sw.Write(matcapField.GetGUIDs());
				}
			}
			else
			{
				string[] lines = File.ReadAllLines(FilePath);
				bool alreadyExist = false;
				for (int i = 0; i < lines.Length; i++)
				{
					string line = lines[i];
					AssetDatabase.TryGetGUIDAndLocalFileIdentifier(matcapField.originalMaterial, out string originalMatGuid, out long _);
					AssetDatabase.TryGetGUIDAndLocalFileIdentifier(matcapField.matcapMaterial, out string matcapMatGuid, out long _);
					if (line.Contains(originalMatGuid) || line.Contains(matcapMatGuid))
					{
						lines[i] = matcapField.GetGUIDs();
						alreadyExist = true;
						break;
					}
				}

				List<string> linesToSave = new List<string>(lines);
				if (alreadyExist == false)
				{
					linesToSave.Add(matcapField.GetGUIDs());
				}
				
				var finalJson = new StringBuilder();
				for (int i = 0; i < linesToSave.Count; i++)
				{
					if (i < linesToSave.Count - 1)
					{
						finalJson.AppendLine(linesToSave[i]);
					}
					else
					{
						finalJson.Append(linesToSave[i]);
					}
				}

				using (StreamWriter sw = fileInfo.CreateText())
				{
					sw.Write(finalJson);
				}
			}
		}

		public static void LoadMatcapFields(List<MatcapField> matcapFields)
		{
			if (File.Exists(FilePath) == false)
			{
				return;
			}

			string[] lines = File.ReadAllLines(FilePath);
			foreach (MatcapField field in matcapFields)
			{
				if (field.originalMaterial != null)
				{
					bool succeeded = AssetDatabase.TryGetGUIDAndLocalFileIdentifier(field.originalMaterial, out string guid, out long localId);
					if (succeeded == false)
					{
						Debug.LogError("There were an error retrieving the material id");
						continue;
					}
					
					string line = lines.FirstOrDefault(x => x.Contains(guid));
					field.LoadFromGUIDs(line);
					continue;
				}

				if (field.matcapMaterial != null)
				{
					bool succeeded = AssetDatabase.TryGetGUIDAndLocalFileIdentifier(field.matcapMaterial, out string guid, out long localId);
					if (succeeded == false)
					{
						Debug.LogError("There were an error retrieving the material id");
						continue;
					}
					
					string line = lines.FirstOrDefault(x => x.Contains(guid));
					if (string.IsNullOrEmpty(line))
					{
						continue;
					}
					field.LoadFromGUIDs(line);
				}
			}
		}
	}
}