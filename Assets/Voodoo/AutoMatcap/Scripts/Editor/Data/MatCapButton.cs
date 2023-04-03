using System;
using UnityEditor;
using UnityEngine;

namespace Voodoo.Render
{
	[Serializable]
	public class MatCapButton
	{
		public GUIContent buttonContent;
		// public Action onClick;
		
		private GUIStyle buttonStyle;
		private GUIStyle ButtonStyle => buttonStyle ?? (buttonStyle = new GUIStyle(GUI.skin.button)
		{
			padding = new RectOffset(4, 4, 2, 2)
		});
		
		public bool OnGUI()
		{
			// if (GUILayout.Button(buttonContent, ButtonStyle, GUILayout.Width(EditorGUIUtility.singleLineHeight), GUILayout.Height(EditorGUIUtility.singleLineHeight)))
			// {
			// 	onClick?.Invoke();
			// }

			return GUILayout.Button(buttonContent, ButtonStyle, GUILayout.Width(EditorGUIUtility.singleLineHeight), GUILayout.Height(EditorGUIUtility.singleLineHeight));
		}
	}
}