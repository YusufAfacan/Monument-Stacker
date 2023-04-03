using UnityEditor;
using UnityEngine;

namespace Voodoo.Render
{
	public static class AutoMatcapTextureImporter
	{
		public static void UpdateTextureImporter(Texture texture)
		{
			string texturePath = AssetDatabase.GetAssetPath(texture);
			UpdateTextureImporter(texturePath);
		}
		
		public static void UpdateTextureImporter(string texturePath)
		{
			string relativePath = texturePath;
			if (relativePath.StartsWith(Application.dataPath))
			{
				relativePath = "Assets" + relativePath.Substring(Application.dataPath.Length);
			}
        
			TextureImporter importer = (TextureImporter)AssetImporter.GetAtPath(relativePath);
			importer.isReadable = true;
			importer.alphaIsTransparency = true;
			importer.textureCompression = TextureImporterCompression.Uncompressed;
			importer.mipmapEnabled = false;
			importer.maxTextureSize = 512;
        
			importer.SaveAndReimport();
		}
	}
}