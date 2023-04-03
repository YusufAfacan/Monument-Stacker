using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Voodoo.Render
{
	[Serializable]
	internal class SphereProperty
	{
		public string matcapId;
		public string shaderName;
		public List<MatProperty> properties;

		public SphereProperty(string guid, string shaderName, MaterialProperty[] materialProperties)
		{
			matcapId = guid;
			this.shaderName = shaderName;
			properties = new List<MatProperty>();
			foreach (MaterialProperty materialProperty in materialProperties)
			{
				properties.Add(new MatProperty(materialProperty));
			}
		}
		
		public void ApplyTo(Material material)
		{
			material.shader = Shader.Find(shaderName);
	
			foreach (MatProperty property in properties)
			{
				switch (property.type)
				{
					case PropertyType.Float:
					case PropertyType.Range:
						material.SetFloat(property.name, float.Parse(property.value));
						break;
					case PropertyType.Vector:
						string[] parameters = property.value.Split('|');
						Vector4 result = Vector4.zero;
						for (var i = 0; i < parameters.Length; i++)
						{
							result[i] = float.Parse(parameters[i]);
						}
	
						material.SetVector(property.name, result);
						break;
					case PropertyType.Color:
						ColorUtility.TryParseHtmlString(string.Concat("#", property.value), out Color res);
						material.SetColor(property.name, res);
						break;
					case PropertyType.Texture:
						if (string.IsNullOrEmpty(property.value))
						{
							var importer = AssetImporter.GetAtPath(AssetDatabase.GetAssetPath(material.shader)) as ShaderImporter;
							material.SetTexture(property.name, importer != null ? importer.GetDefaultTexture(property.name) : null);
							property.textureScaleAndOffset = new Vector4(1, 1, 0, 0);
							break;
						}

						material.SetTexture(property.name, AssetDatabase.LoadAssetAtPath<Texture>(AssetDatabase.GUIDToAssetPath(property.value)));
						material.SetTextureScale(property.name, new Vector2(property.textureScaleAndOffset.x, property.textureScaleAndOffset.y));
						material.SetTextureOffset(property.name, new Vector2(property.textureScaleAndOffset.z, property.textureScaleAndOffset.w));
						break;
				}
			}
		}
	}
}