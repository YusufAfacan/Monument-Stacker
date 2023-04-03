using UnityEditor;
using UnityEngine;

namespace Voodoo.Render
{
	[System.Serializable]
	public class ModelViewer
	{
		public Editor editor;

		public ModelViewer(GameObject model)
		{
			editor = Editor.CreateEditor(model);
		}

		public void Repaint()
		{
			editor.Repaint();
		}
		
		public void OnGUI()
		{
			editor.ReloadPreviewInstances();
			
			EditorGUILayout.Space();
			editor.OnInteractivePreviewGUI(GUILayoutUtility.GetRect(EditorGUIUtility.currentViewWidth, 256), GUIStyle.none);
		}
	}
}