using UnityEditor;
using UnityEngine;

namespace Voodoo.Render
{
	[System.Serializable]
	internal class SceneCamera
	{
		private Vector3 pivot;
		private Quaternion rotation;
		private bool orthographic;
		private bool isRotationLocked;
		private bool in2DMode;
		private float size;
		private Object activeObject;

		public SceneCamera(SceneView sceneView)
		{
			pivot            = sceneView.pivot;
			rotation         = sceneView.rotation;
			orthographic     = sceneView.orthographic;
			isRotationLocked = sceneView.isRotationLocked;
			in2DMode         = sceneView.in2DMode;
			size             = sceneView.size;
			activeObject = Selection.activeObject;
		}

		public void ReloadView()
		{
			SceneView.lastActiveSceneView.in2DMode = in2DMode;
			if (in2DMode == false)
			{
				SceneView.lastActiveSceneView.rotation = rotation;
			}
			
			SceneView.lastActiveSceneView.pivot = pivot;
			SceneView.lastActiveSceneView.orthographic = orthographic;
			SceneView.lastActiveSceneView.size = size;
			SceneView.lastActiveSceneView.isRotationLocked = isRotationLocked;
			Selection.activeObject = activeObject;
		}
	}
}