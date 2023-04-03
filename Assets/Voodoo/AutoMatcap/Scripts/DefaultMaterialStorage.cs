using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

#if UNITY_EDITOR
using System.IO;
using UnityEditor;
#endif

namespace Voodoo.Render
{
	[CreateAssetMenu(fileName = "Default Material Storage", menuName = "Voodoo/AutoMatCap/Create default material storage")]
	public class DefaultMaterialStorage : ScriptableObject
	{
#if UNITY_EDITOR
		private const string scriptablePath = "Assets/Voodoo/AutoMatcap/Data/Default Material Storage.asset";
		
		private const string defaultMaterialName = "Default-Material.mat";
		private const string urpShaderName = "Universal Render Pipeline/Lit";

		private static DefaultMaterialStorage _instance;
		
		public Material currentMaterial;

		public void Reset()
		{
			currentMaterial = GraphicsSettings.renderPipelineAsset == null ? AssetDatabase.GetBuiltinExtraResource<Material>(defaultMaterialName) : GetURPMaterial();
		}

		private Material GetURPMaterial()
		{
			string shaderPath = AssetDatabase.GetAssetPath(Shader.Find(urpShaderName));
			string[] allMaterials = AssetDatabase.FindAssets("t:Material");
			
			List<string> materials = new List<string>();
			for (int i = 0; i < allMaterials.Length; i++)
			{
				allMaterials[i] = AssetDatabase.GUIDToAssetPath(allMaterials[i]);
				string[] dep = AssetDatabase.GetDependencies(allMaterials[i]);
				if (ArrayUtility.Contains(dep, shaderPath) && allMaterials[i].EndsWith(".mat"))
				{
					materials.Add(allMaterials[i]);
				}
			}
			
			return AssetDatabase.LoadAssetAtPath<Material>(materials[0]);
		}
		
		public static DefaultMaterialStorage Find()
		{
			if (_instance == null)
			{
				_instance = AssetDatabase.LoadAssetAtPath<DefaultMaterialStorage>(scriptablePath);
			}

			if (_instance != null)
			{
				return _instance;
			}
			
			string[] guids = AssetDatabase.FindAssets("t:" + nameof(DefaultMaterialStorage));

			if (guids.Length > 0)
			{
				DefaultMaterialStorage[] defaultMaterialManagerInstances = new DefaultMaterialStorage[guids.Length];

				for (int i = 0; i < guids.Length; i++)
				{
					string path = AssetDatabase.GUIDToAssetPath(guids[i]);
					defaultMaterialManagerInstances[i] = AssetDatabase.LoadAssetAtPath<DefaultMaterialStorage>(path);
				}

				if (defaultMaterialManagerInstances.Length > 0)
				{
					if (defaultMaterialManagerInstances.Length > 1)
					{
						Debug.LogWarning("There are more than 1 defaultMaterialManager in your project. Taking the first one");
					}

					_instance = defaultMaterialManagerInstances[0];
				}
			}

			if (_instance == null)
			{
				_instance = CreateInstance<DefaultMaterialStorage>();
				FileInfo fileInfo = new FileInfo(scriptablePath);
				fileInfo.Directory?.Create();
				AssetDatabase.CreateAsset(_instance, scriptablePath);
				_instance.Reset();
			}

			return _instance;
		}

		public void OnGUI()
		{
			EditorGUI.BeginChangeCheck();
			currentMaterial = (Material)EditorGUILayout.ObjectField("Sphere Default Material", currentMaterial, typeof(Material), false);
			if (EditorGUI.EndChangeCheck() && currentMaterial == null)
			{
				Reset();
			}
		}
#endif
	}
}