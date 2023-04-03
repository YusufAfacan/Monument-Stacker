using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Voodoo.Render
{
	[System.Serializable]
	public class MatcapField
	{
		public Material originalMaterial;
		public Material matcapMaterial;
		public MatCapButton button;
		public MatCapFieldState buttonState;

		public MatcapField(Material originalMaterial, Material matcapMaterial)
		{
			this.originalMaterial = originalMaterial;
			this.matcapMaterial = matcapMaterial;
		}

		public void UpdateButtonState(bool isBeingEdited)
		{
			if (button == null)
			{
				button = new MatCapButton();
			}
			
			buttonState = isBeingEdited ? MatCapFieldState.Editing : MatCapFieldState.Idle;
			string tooltip = isBeingEdited ? "Stop editing" : "Start editing";
			button.buttonContent = EditorGUIUtility.IconContent("d_Record On", tooltip);
		}
		
		public MatcapField OnGUI()
		{
			MatcapField res = null;
			EditorGUILayout.BeginHorizontal();
			{
				originalMaterial = (Material) EditorGUILayout.ObjectField(originalMaterial, typeof(Material), false);
				matcapMaterial = (Material) EditorGUILayout.ObjectField(matcapMaterial, typeof(Material), false);
				if (button == null)
				{
					UpdateButtonState(false);
				}
				
				if (buttonState == MatCapFieldState.Editing)
				{
					GUI.backgroundColor = new Color(0.8f,0,0,1);
					GUI.contentColor = Color.red;
				}
				
				bool isGUIEnabled = GUI.enabled;
				GUI.enabled = true;
				
				if (button.OnGUI())
				{
					res = this;
				}
				
				GUI.enabled = isGUIEnabled;
				GUI.backgroundColor = Color.white;
				GUI.contentColor = Color.white;
			}
			EditorGUILayout.EndHorizontal();

			return res;
		}

		public string GetGUIDs()
		{
			if (originalMaterial != null && matcapMaterial != null)
			{
				bool originalSuccess = AssetDatabase.TryGetGUIDAndLocalFileIdentifier(originalMaterial, out string originalMatGuid, out long _);
				bool matcapSuccess = AssetDatabase.TryGetGUIDAndLocalFileIdentifier(matcapMaterial, out string matcapMatGuid, out long _);
				if (originalSuccess && matcapSuccess)
				{
					return string.Concat(originalMatGuid, ",", matcapMatGuid);
				}
				
				Debug.LogError("There were an error retrieving the material id");
			}
			
			return string.Empty;
		}

		public void LoadFromGUIDs(string GUIDs)
		{
			if (string.IsNullOrEmpty(GUIDs))
			{
				return;
			}

			List<string> materialIds = GUIDs.Split(',').ToList();
			List<string> paths = materialIds.Select(AssetDatabase.GUIDToAssetPath).ToList();
			List<Material> materials = paths.Select(AssetDatabase.LoadAssetAtPath<Material>).ToList();

			originalMaterial = materials[0];
			matcapMaterial = materials[1];
		}
	}
}