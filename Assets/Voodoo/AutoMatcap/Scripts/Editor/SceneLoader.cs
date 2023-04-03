using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Voodoo.Render
{
	public static class SceneLoader
	{
		public static string autoMatcapScenePath = "Assets/Voodoo/AutoMatcap/Scenes/AutoMatcap.unity";
		private const string sceneKey = "MatCap_PreviousSceneName";
		private const string sceneParamsKey = "MatCap_PreviousScene_Parameters";
		
		private static SceneCamera previousSceneCamera;
		private static Scene previousScene;

		public static bool OpenMatCapScene()
		{
			bool saved = EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo();
			if (saved == false)
			{
				return false;
			}
			
			previousScene = SceneManager.GetActiveScene();
			
			if (previousScene.path == null)
			{
				Debug.LogError("Can't open MatCap");
				previousScene = default;
				return false;
			}

			previousSceneCamera = new SceneCamera(SceneView.lastActiveSceneView);

			if (previousScene.path != autoMatcapScenePath)
			{
				Scene autoMatcapScene = EditorSceneManager.OpenScene(autoMatcapScenePath, OpenSceneMode.Additive);
				SceneManager.SetActiveScene(autoMatcapScene);
				EditorSceneManager.CloseScene(previousScene, false);
			}

			Save();
			
			return true;
		}

		public static void ReloadPreviousScene()
		{
			if (string.IsNullOrEmpty(previousScene.path))
			{
				Load();
			}
			
			Clear();
			
			if (string.IsNullOrEmpty(previousScene.path) == false)
			{
				EditorSceneManager.OpenScene(previousScene.path);
				previousScene = default;
			}

			previousSceneCamera?.ReloadView();
			previousSceneCamera = null;
		}

		#region DataManagement
		
		private static void Save()
		{
			EditorPrefs.SetString(sceneKey, previousScene.name);
			EditorPrefs.SetString(sceneParamsKey, JsonUtility.ToJson(previousSceneCamera));
		}

		private static void Load()
		{
			if (EditorPrefs.HasKey(sceneKey))
			{
				previousScene = SceneManager.GetSceneByName(EditorPrefs.GetString(sceneKey));
			}
			
			if (EditorPrefs.HasKey(sceneParamsKey))
			{
				previousSceneCamera = JsonUtility.FromJson<SceneCamera>(EditorPrefs.GetString(sceneParamsKey));
			}
		}

		private static void Clear()
		{
			EditorPrefs.DeleteKey(sceneKey);
			EditorPrefs.DeleteKey(sceneParamsKey);
		}
		
		#endregion
	}
}