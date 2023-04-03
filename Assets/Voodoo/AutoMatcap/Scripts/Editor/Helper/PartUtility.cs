using UnityEditor;
using UnityEngine;

namespace Voodoo.Render
{
	public static class PartUtility
	{
		private static GUIStyle titleStyle;

		private static GUIStyle TitleStyle => titleStyle ?? (titleStyle = new GUIStyle
		{ 
			normal = { textColor = Color.white },
			fontSize = 32,
			alignment = TextAnchor.MiddleCenter,
			wordWrap = false,
		});
		
		public static void Begin(string partTitle)
		{
			EditorGUILayout.BeginVertical("box", GUILayout.ExpandWidth(true));
			GUIContent titleLabel = new GUIContent(partTitle);
			EditorGUILayout.LabelField(titleLabel, TitleStyle, GUILayout.Height(TitleStyle.CalcSize(titleLabel).y));
		}

		public static void End()
		{
			EditorGUILayout.EndVertical();
		}
	}
}