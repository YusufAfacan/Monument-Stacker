using System;
using UnityEditor;
using UnityEngine;

namespace Voodoo.Render
{
	[Serializable]
	public class MaterialViewer
	{
		public MaterialEditor editor;
		public Vector2 scrollPosition;

		private readonly Type StandardShaderGUIType = typeof(ShaderGUI).Assembly.GetType("UnityEditor.StandardShaderGUI");

		public MaterialViewer(Material material)
		{
			editor = Editor.CreateEditor(material) as MaterialEditor;
		}
		
		public void OnGUI()
		{
			scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition, false, false);
			{
				editor.DrawHeader();
				if (editor.isVisible == false)
				{
					EditorGUILayout.EndScrollView();
					return;
				}
				
				MaterialProperty[] materialProperties = MaterialEditor.GetMaterialProperties(editor.targets);
				if (editor.customShaderGUI != null && editor.customShaderGUI.GetType() != StandardShaderGUIType)
				{
					editor.customShaderGUI.OnGUI(editor, materialProperties);
				}
				else
				{
					editor.PropertiesDefaultGUI(materialProperties);
				}
			}
			EditorGUILayout.EndScrollView();
		}
	}
}