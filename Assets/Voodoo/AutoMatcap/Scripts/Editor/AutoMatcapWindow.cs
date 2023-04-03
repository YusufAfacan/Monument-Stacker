using System;
using UnityEditor;
using UnityEngine;

namespace Voodoo.Render
{
	public class AutoMatcapWindow : EditorWindow
	{
		public static AutoMatcapWindow window;
		public bool editMode;

		private const int bakingFrameRate = 3;
		
		private int currentFrame = 0;
		private GameObject model;
		private MaterialManager materialManager;
		private AutoMatCapBakery bakery;
		private ModelViewer modelViewer;

		[MenuItem("Voodoo/AutoMatCap", false)]
		private static void Init()
		{
			if (window != null)
			{
				window.Close();
			}
			

			OpenWindow();
			window.Show();
		}
		
		private void OnEnable()
		{
			if (window == null)
			{
				OpenWindow();
			}
		}
		
		private static void OpenWindow()
		{
			Type windowType = typeof(EditorWindow).Assembly.GetType("UnityEditor.InspectorWindow");
			window = GetWindow<AutoMatcapWindow>("AutoMatCap", false, windowType);
		}

		private void OnGUI()
		{
			EditorGUI.BeginDisabledGroup(editMode);
			PartUtility.Begin("Setup");
			DrawSetup();
			PartUtility.End();
			EditorGUI.EndDisabledGroup();

			if (model == null)
			{
				return;
			}
			
			EditorGUI.BeginDisabledGroup(editMode == false);
			PartUtility.Begin("Edition");
			DrawEdition();
			PartUtility.End();
			EditorGUI.EndDisabledGroup();

			DrawModelViewer();
			
			if (editMode)
			{
				materialManager.DrawCurrentSphereMaterial();
			}
		}
		
		private void DrawSetup()
		{
			EditorGUI.BeginChangeCheck();
			model = (GameObject) EditorGUILayout.ObjectField("Model", model, typeof(GameObject), false);

			if (model != null && PrefabUtility.GetPrefabAssetType(model) == PrefabAssetType.Model)
			{
				EditorGUILayout.HelpBox("The prefab your are trying to modify is a model. Material modifications will not be applied.", MessageType.Warning);
			}

			if (EditorGUI.EndChangeCheck())
			{
				if (materialManager == null)
				{
					materialManager = new MaterialManager();
				}
				
				materialManager.Recalculate(model);
				modelViewer = model != null ? new ModelViewer(model) : null;
			}
			
			materialManager?.OnGUI();
		}

		private void DrawModelViewer()
		{
			if (modelViewer != null && modelViewer.editor == null)
			{
				modelViewer = new ModelViewer(model);
			}
			
			modelViewer?.OnGUI();
		}

		private void DrawEdition()
		{
			if (bakery == null)
			{
				bakery = new AutoMatCapBakery(materialManager);
			}

			bakery.OnGUI();
		}
		
		private void OnInspectorUpdate()
		{
			currentFrame++;

			if (currentFrame % bakingFrameRate != 0)
			{
				return;
			}
			
			if (bakery?.TryRealtimeBaking() ?? false)
			{
				window.Repaint();
				modelViewer.Repaint();
			}
		}

		public static void Quit()
		{
			if (window.editMode == false)
			{
				return;
			}
			
			window.bakery?.Reset();
			window.editMode = false;
			SceneLoader.ReloadPreviousScene();
		}
		
		private void OnDestroy()
		{
			Quit();
		}
	}
}
