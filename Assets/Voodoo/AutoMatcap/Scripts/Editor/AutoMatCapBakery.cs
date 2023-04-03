using UnityEditor;
using UnityEngine;

namespace Voodoo.Render
{
	public class AutoMatCapBakery
	{
	    private readonly MaterialManager materialManager;
	    private bool enableRealtimeUpdate;
	    
	    public AutoMatCapBakery(MaterialManager materialManager)
	    {
		    this.materialManager = materialManager;
	    }

	    public bool TryRealtimeBaking()
	    {
		    if (enableRealtimeUpdate)
		    {
			    Bake();
		    }
		    
		    return enableRealtimeUpdate;
	    }

		private void Bake()
		{
			SceneBaker.Instance.Bake(materialManager.MatcapTexture, AssetDatabase.GetAssetPath(materialManager.MatcapTexture));
		}

		public void OnGUI()
		{
			EditorGUILayout.BeginHorizontal();
			{
				string buttonState = enableRealtimeUpdate ? "Disable" : "Enable";
				if (GUILayout.Button($"{buttonState} live baking"))
				{
					enableRealtimeUpdate = !enableRealtimeUpdate;
				}

				EditorGUI.BeginDisabledGroup(enableRealtimeUpdate);
				if (GUILayout.Button("Bake", GUILayout.Width(50f)))
				{
					Bake();
				}
		
				EditorGUI.EndDisabledGroup();
			}
			EditorGUILayout.EndHorizontal();
		}

		public void Reset()
		{
			enableRealtimeUpdate = false;
		}
	}
}