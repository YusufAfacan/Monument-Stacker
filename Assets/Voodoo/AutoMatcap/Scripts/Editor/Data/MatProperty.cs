using System;
using UnityEditor;
using UnityEngine;

namespace Voodoo.Render
{
	[Serializable]
	internal class MatProperty
	{
		public PropertyType type;
		public string name;
		public string value;
		public Vector4 textureScaleAndOffset;

		public MatProperty(MaterialProperty mp)
		{
			name = mp.name;
			
			switch (mp.type)
			{
				case MaterialProperty.PropType.Float:
					type = PropertyType.Float;
					value = mp.floatValue.ToString();
					break;
				case MaterialProperty.PropType.Range:
					type = PropertyType.Range;
					value = mp.floatValue.ToString();
					break;
				case MaterialProperty.PropType.Vector:
					type = PropertyType.Vector;
					object[] vector = {mp.vectorValue.x, mp.vectorValue.y, mp.vectorValue.z, mp.vectorValue.w};
					value = string.Join<object>("|", vector);
					break;
				case MaterialProperty.PropType.Color:
					type = PropertyType.Color;
					value = ColorUtility.ToHtmlStringRGBA(mp.colorValue);
					break;
				case MaterialProperty.PropType.Texture:
					type = PropertyType.Texture;
					if (mp.textureValue != null)
					{
						bool succeeded = AssetDatabase.TryGetGUIDAndLocalFileIdentifier(mp.textureValue, out string guid, out long _);
						if (succeeded)
						{
							value = guid;
							textureScaleAndOffset = mp.textureScaleAndOffset;
							return;
						}
					}
					
					value = string.Empty;
					textureScaleAndOffset = new Vector4(1, 1, 0, 0);

					break;
			}
		}

		public override string ToString()
		{
			return $"{name} : of type {Enum.GetName(typeof(PropertyType),type)}, with value {value}";
		}
	}
}